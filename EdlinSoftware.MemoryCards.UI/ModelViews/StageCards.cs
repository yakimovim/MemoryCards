using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using EdlinSoftware.MemoryCards.UI.Models;
using JetBrains.Annotations;

namespace EdlinSoftware.MemoryCards.UI.ModelViews
{
    internal class StageCards
    {
        private readonly GameStage _stage;
        private readonly CardViewModel[] _cards;

        [DebuggerStepThrough]
        public StageCards([NotNull] GameStage stage, [NotNull] CardViewModel[] cards)
        {
            Contract.Requires(stage != null);
            Contract.Requires(cards != null);
            _stage = stage;
            _cards = cards;
        }

        public void ProcessClickOnCard([NotNull] CardViewModel card)
        {
            Contract.Requires(card != null);

            if (card.State != CardState.Hidden)
                return;

            var temporaryOpenedCards = _cards
                .Where(c => c.State == CardState.TemporarilyOpened)
                .ToArray();

            if (temporaryOpenedCards.Length < _stage.CardsInGroup - 1)
            {
                card.State = CardState.TemporarilyOpened;
                return;
            }

            if (temporaryOpenedCards.Length == _stage.CardsInGroup - 1)
            {
                if (temporaryOpenedCards.All(c => c.Value == card.Value))
                {
                    card.State = CardState.Opened;
                    Array.ForEach(temporaryOpenedCards, c => c.State = CardState.Opened);

                    if (_cards.All(c => c.State == CardState.Opened))
                        EventBroker.Instance.SendMessage(new StageIsWon(_stage));

                    return;
                }

                card.State = CardState.TemporarilyOpened;
                return;
            }

            Array.ForEach(temporaryOpenedCards, c => c.State = CardState.Hidden);
            card.State = CardState.TemporarilyOpened;
        }
    }
}