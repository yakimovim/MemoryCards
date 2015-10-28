using System.Diagnostics.Contracts;
using System.Windows.Input;
using JetBrains.Annotations;

namespace EdlinSoftware.MemoryCards.UI.ModelViews
{
    /// <summary>
    /// Represents view model for information about one game of memory cards.
    /// </summary>
    internal class GameMenuItemViewModel : BaseViewModel
    {
        public GameMenuItemViewModel([NotNull] string title, [NotNull] string gameFolder, [NotNull] ICommand startGameCommand)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(title));
            Contract.Requires(!string.IsNullOrWhiteSpace(gameFolder));
            Contract.Requires(startGameCommand != null);

            Title = title;
            GameFolder = gameFolder;
            StartGameCommand = startGameCommand;
        }

        /// <summary>
        /// Gets title of the game.
        /// </summary>
        public string Title {[NotNull] get; }

        /// <summary>
        /// Gets game folder.
        /// </summary>
        public string GameFolder {[NotNull]  get; }

        /// <summary>
        /// Gets or sets command to start the game.
        /// </summary>
        public ICommand StartGameCommand {[NotNull]  get; }
    }
}