using System;
using System.Diagnostics.Contracts;
using EdlinSoftware.MemoryCards.UI.Models;
using JetBrains.Annotations;

namespace EdlinSoftware.MemoryCards.UI.ModelViews
{
    internal class StageViewModel : BaseViewModel
    {
        public GameStage Stage { get; }
        public string GameFolder { get; }

        public event EventHandler StageIsFinished;

        public StageViewModel([NotNull] GameStage gameStage, [NotNull] string gameFolder)
        {
            Contract.Requires(gameStage != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(gameFolder));

            Stage = gameStage;
            GameFolder = gameFolder;
        }

        public virtual void FinishStage()
        {
            StageIsFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}