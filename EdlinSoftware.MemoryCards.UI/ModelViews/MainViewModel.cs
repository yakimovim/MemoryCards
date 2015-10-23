using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using EdlinSoftware.MemoryCards.UI.Models;
using EdlinSoftware.MemoryCards.UI.Properties;
using Newtonsoft.Json;

namespace EdlinSoftware.MemoryCards.UI.ModelViews
{
    internal class MainViewModel : BaseViewModel
    {
        private readonly Person _person;
        private Game _game;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ObservableCollection<GameMenuItemViewModel> _games =
            new ObservableCollection<GameMenuItemViewModel>();

        [DebuggerStepThrough]
        public MainViewModel(Person person)
        {
            if (person == null) throw new ArgumentNullException("person");
            _person = person;

            ReadExistingGames();
        }

        private void ReadExistingGames()
        {
            var startGameCommand = new DelegateCommand(arg =>
            {
                var gameFolder = (string)arg;

                var jsonStagesResult = ReadJsonStages(gameFolder);
                if (jsonStagesResult.Status == ResultStatus.Failure)
                {
                    MessageBox.Show(jsonStagesResult.ErrorMessage);
                    return;
                }

                var validationResult = ValidateStages(jsonStagesResult.Value);
                if (validationResult.Status == ResultStatus.Failure)
                {
                    MessageBox.Show(validationResult.ErrorMessage);
                    return;
                }

                _game = CreateGame(gameFolder, jsonStagesResult.Value);
            });

            var gamesFolder = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                Settings.Default.GamesFolderName);

            if(!Directory.Exists(gamesFolder))
                return;

            foreach (var gameFolder in Directory.GetDirectories(gamesFolder))
            {
                Contract.Assume(gameFolder != null);

                if (File.Exists(Path.Combine(gameFolder, Settings.Default.GameConfigFileName)))
                {
                    _games.Add(new GameMenuItemViewModel(
                        Path.GetFileName(gameFolder),
                        gameFolder,
                        startGameCommand
                        ));
                }
            }
        }

        private Result<JsonStage[]> ReadJsonStages(string gameFolder)
        {
            try
            {
                // deserialize JSON directly from a file
                using (StreamReader file = File.OpenText(Path.Combine(gameFolder, Settings.Default.GameConfigFileName)))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var jsonStages = (JsonStage[])serializer.Deserialize(file, typeof(JsonStage[]));
                    return jsonStages != null
                        ? Result<JsonStage[]>.CreateSuccess(jsonStages)
                        : Result<JsonStage[]>.CreateFailure("Unable to deserialize JSON");
                }
            }
            catch (Exception ex)
            {
                return Result<JsonStage[]>.CreateFailure(ex.Message);
            }
        }

        private Result ValidateStages(JsonStage[] stages)
        {
            if((stages?.Length ?? 0) == 0)
                return Result.CreateFailure("No stages");

            for (int i = 0; i < stages.Length; i++)
            {
                var stage = stages[i];
                if(string.IsNullOrWhiteSpace(stage.Name))
                    return Result.CreateFailure($"Stage #{i + 1}. Name can't be null or empty");
                if (stage.CardsInGroup < 2)
                    return Result.CreateFailure($"Stage #{i + 1}. Number of cards in group should be greater that 1");
                if ((stage.CardsRows?.Length ?? 0) == 0)
                    return Result.CreateFailure($"Stage #{i + 1}. Cards in rows array must be specified");
                Contract.Assume(stage.CardsRows != null);
                if ((stage.CardsRows.Sum() % stage.CardsInGroup) != 0)
                    return Result.CreateFailure($"Stage #{i + 1}. Total number of cards in all rows should be divisible by size of group");
            }

            return Result.CreateSuccess();
        }

        private Game CreateGame(string gameFolder, JsonStage[] stages)
        {
            return new Game(Path.GetFileName(gameFolder), 
                stages
                    .Select(s => new GameStage(s.Name, s.CardsInGroup, s.CardsRows))
                    .ToArray());
        }

        public ObservableCollection<GameMenuItemViewModel> Games
        {
            [DebuggerStepThrough]
            get { return _games; }
        }

        public string Name
        {
            get { return _person.Name; }
            set
            {
                if (value != _person.Name)
                {
                    _person.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SayHello
        {
            get
            {
                return new DelegateCommand((arg) =>
                {
                    MessageBox.Show(string.Format("Hello, {0}", Name));
                },
                (arg) => !string.IsNullOrWhiteSpace(Name));
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                return new DelegateCommand(arg =>
                {
                    var result = MessageBox.Show(
                        Resources.ExitConfirmationText, 
                        Resources.ExitConfirmationTitle, 
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Application.Current.Shutdown(0);
                    }
                });
            }
        }
    }
}
