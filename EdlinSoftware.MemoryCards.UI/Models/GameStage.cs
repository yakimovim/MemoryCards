using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;

namespace EdlinSoftware.MemoryCards.UI.Models
{
    /// <summary>
    /// Represents one stage of the game.
    /// </summary>
    internal class GameStage
    {
        /// <summary>
        /// Gets name of the stage.
        /// </summary>
        public string Name {[NotNull] get; }

        /// <summary>
        /// Gets or sets time to solve the stage in milliseconds.
        /// </summary>
        public int TimeToSolve { get; }

        /// <summary>
        /// Gets number of cards in the group of the same cards.
        /// </summary>
        public int CardsInGroup {[NotNull] get; }

        /// <summary>
        /// Gets number of cards in each row.
        /// </summary>
        public int[] CardsRows {[NotNull] get; }

        public GameStage([NotNull] string name, int timeToSolve, int cardsInGroup, [NotNull] int[] cardsRows)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(timeToSolve >= 0);
            Contract.Requires(cardsInGroup > 1);
            Contract.Requires(cardsRows != null);
            Contract.Requires(cardsRows.Length > 0);
            Contract.Requires(cardsRows.Sum() % cardsInGroup == 0);

            Name = name;
            TimeToSolve = timeToSolve;
            CardsInGroup = cardsInGroup;
            CardsRows = cardsRows;
        }
    }
}