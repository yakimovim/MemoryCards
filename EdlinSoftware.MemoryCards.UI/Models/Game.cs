using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace EdlinSoftware.MemoryCards.UI.Models
{
    /// <summary>
    /// Represents one memory cards game.
    /// </summary>
    internal class Game
    {
        /// <summary>
        /// Gets title of the game.
        /// </summary>
        public string Title {[NotNull] get; }

        /// <summary>
        /// Gets stages of the game.
        /// </summary>
        public GameStage[] Stages {[NotNull] get; }

        public Game([NotNull] string title, [NotNull] GameStage[] stages)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(title));
            Contract.Requires(stages != null);
            Contract.Requires(stages.Length > 0);

            Title = title;
            Stages = stages;
        }
    }
}