namespace EdlinSoftware.MemoryCards.UI.Models
{
    /// <summary>
    /// Represents JSON description of one stage of game.
    /// </summary>
    internal class JsonStage
    {
        /// <summary>
        /// Gets or sets name of the stage.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets time to solve the stage in milliseconds.
        /// </summary>
        public int TimeToSolve { get; set; }
        /// <summary>
        /// Gets or sets number of cards in the group of the same cards.
        /// </summary>
        public int CardsInGroup { get; set; }
        /// <summary>
        /// Gets or sets number of cards in each row.
        /// </summary>
        public int[] CardsRows { get; set; }
    }
}