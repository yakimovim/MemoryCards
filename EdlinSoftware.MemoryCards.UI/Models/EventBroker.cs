using System.Diagnostics;
using System.Diagnostics.Contracts;
using EdlinSoftware.MemoryCards.UI.Events;
using JetBrains.Annotations;

namespace EdlinSoftware.MemoryCards.UI.Models
{
    internal class EventBroker
    {
        public static EventAggregator Instance { get; } = new EventAggregator();
    }

    internal class StartNewGame
    {
        [DebuggerStepThrough]
        public StartNewGame([NotNull] Game newGame, [NotNull] string gameFolder)
        {
            Contract.Requires(newGame != null);
            Contract.Requires(string.IsNullOrWhiteSpace(gameFolder));

            NewGame = newGame;
            GameFolder = gameFolder;
        }

        public Game NewGame { get; }
        public string GameFolder { get; }
    }

    internal class GameIsWon
    { }

    internal class StartNewStage
    {
        [DebuggerStepThrough]
        public StartNewStage([NotNull] GameStage newStage, [NotNull] string gameFolder)
        {
            Contract.Requires(newStage != null);
            Contract.Requires(string.IsNullOrWhiteSpace(gameFolder));

            NewStage = newStage;
            GameFolder = gameFolder;
        }

        public GameStage NewStage { get; }
        public string GameFolder { get; }
    }

    internal class StageIsWon
    {
        [DebuggerStepThrough]
        public StageIsWon([NotNull] GameStage stage)
        {
            Contract.Requires(stage != null);

            Stage = stage;
        }

        public GameStage Stage { get; }
    }

    internal class StageIsLost
    {
        [DebuggerStepThrough]
        public StageIsLost([NotNull] GameStage stage)
        {
            Contract.Requires(stage != null);

            Stage = stage;
        }

        public GameStage Stage { get; }
    }
}