using System.Diagnostics.Contracts;
using System.Linq;
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
        public string Title { get; }

        /// <summary>
        /// Gets stages of the game.
        /// </summary>
        public GameStage[] Stages { get; }

        public Game([NotNull] string title, [NotNull] GameStage[] stages)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(title));
            Contract.Requires(stages != null);
            Contract.Requires(stages.Length > 0);

            Title = title;
            Stages = stages;
        }
    }

    /// <summary>
    /// Represents one stage of the game.
    /// </summary>
    internal class GameStage
    {
        /// <summary>
        /// Gets name of the stage.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets number of cards in the group of the same cards.
        /// </summary>
        public int CardsInGroup { get; }

        /// <summary>
        /// Gets number of cards in each row.
        /// </summary>
        public int[] CardsRows { get; }

        public GameStage([NotNull] string name, int cardsInGroup, [NotNull] int[] cardsRows)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(cardsInGroup > 1);
            Contract.Requires(cardsRows != null);
            Contract.Requires(cardsRows.Length > 0);
            Contract.Requires(cardsRows.Sum() % cardsInGroup == 0);

            Name = name;
            CardsInGroup = cardsInGroup;
            CardsRows = cardsRows;
        }
    }
}