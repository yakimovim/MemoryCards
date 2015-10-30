using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Input;
using EdlinSoftware.MemoryCards.UI.Models;
using JetBrains.Annotations;

namespace EdlinSoftware.MemoryCards.UI.ModelViews
{
    /// <summary>
    /// Represents view model for information about one game of memory cards.
    /// </summary>
    internal class GameMenuItemViewModel : BaseViewModel
    {
        public GameMenuItemViewModel([NotNull] string title, [NotNull] string gameFolder)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(title));
            Contract.Requires(!string.IsNullOrWhiteSpace(gameFolder));

            Title = title;
            GameFolder = gameFolder;
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
        public ICommand StartGameCommand
        {
            get
            {
                return new DelegateCommand(arg =>
                {
                    var gameFolder = (string)arg;

                    var gameResult = GameReader.ReadGame(gameFolder);
                    if (gameResult.Status == ResultStatus.Failure)
                    {
                        MessageBox.Show(gameResult.ErrorMessage);
                        return;
                    }

                    EventBroker.Instance.SendMessage(new StartNewGame(gameResult.Value, gameFolder));
                });
            }
        }
    }
}