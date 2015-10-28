using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using EdlinSoftware.MemoryCards.UI.Properties;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace EdlinSoftware.MemoryCards.UI.Models
{
    internal class GameReader
    {
        [NotNull]
        public static IEnumerable<string> GetGameFolders()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var executingAssemblyDirectoryName = Path.GetDirectoryName(executingAssembly.Location);

            Contract.Assume(executingAssemblyDirectoryName != null);

            var gamesFolder = Path.Combine(
                executingAssemblyDirectoryName,
                Settings.Default.GamesFolderName);

            if (!Directory.Exists(gamesFolder))
                yield break;

            foreach (var gameFolder in Directory.GetDirectories(gamesFolder))
            {
                Contract.Assume(gameFolder != null);

                if (File.Exists(Path.Combine(gameFolder, Settings.Default.GameConfigFileName)))
                {
                    yield return gameFolder;
                }
            }
        }

        [NotNull]
        public static Result<Game> ReadGame([NotNull] string gameFolder)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(gameFolder));
            Contract.Ensures(Contract.Result<Result<Game>>() != null);

            var jsonStagesResult = ReadJsonStages(gameFolder);
            if (jsonStagesResult.Status == ResultStatus.Failure)
            {
                return new Failure<Game>(jsonStagesResult.ErrorMessage);
            }

            var validationResult = ValidateStages(jsonStagesResult.Value);
            if (validationResult.Status == ResultStatus.Failure)
            {
                return new Failure<Game>(validationResult.ErrorMessage);
            }

            return new Success<Game>(CreateGame(gameFolder, jsonStagesResult.Value));
        }

        [NotNull]
        private static Result<JsonStage[]> ReadJsonStages([NotNull] string gameFolder)
        {
            try
            {
                // deserialize JSON directly from a file
                using (StreamReader file = File.OpenText(Path.Combine(gameFolder, Settings.Default.GameConfigFileName)))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var jsonStages = (JsonStage[])serializer.Deserialize(file, typeof(JsonStage[]));
                    return jsonStages != null
                        ? (Result<JsonStage[]>)new Success<JsonStage[]>(jsonStages)
                        : new Failure<JsonStage[]>("Unable to deserialize JSON");
                }
            }
            catch (Exception ex)
            {
                return new Failure<JsonStage[]>(ex.Message);
            }
        }

        [NotNull]
        private static Result ValidateStages(JsonStage[] stages)
        {
            if ((stages?.Length ?? 0) == 0)
                return new Failure("No stages");

            Contract.Assume(stages != null);

            for (int i = 0; i < stages.Length; i++)
            {
                var stage = stages[i];
                if (string.IsNullOrWhiteSpace(stage.Name))
                    return new Failure($"Stage #{i + 1}. Name can't be null or empty");
                if (stage.CardsInGroup < 2)
                    return new Failure($"Stage #{i + 1}. Number of cards in group should be greater that 1");
                if ((stage.CardsRows?.Length ?? 0) == 0)
                    return new Failure($"Stage #{i + 1}. Cards in rows array must be specified");
                Contract.Assume(stage.CardsRows != null);
                if ((stage.CardsRows.Sum() % stage.CardsInGroup) != 0)
                    return new Failure($"Stage #{i + 1}. Total number of cards in all rows should be divisible by size of group");
            }

            return new Success();
        }

        private static Game CreateGame([NotNull] string gameFolder, JsonStage[] stages)
        {
            return new Game(Path.GetFileName(gameFolder),
                stages
                    .Select(s => new GameStage(s.Name, s.CardsInGroup, s.CardsRows))
                    .ToArray());
        }
    }
}