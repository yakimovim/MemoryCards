using System.Diagnostics.Contracts;
using System.Linq;
using EdlinSoftware.MemoryCards.UI.Views.Controls;

namespace EdlinSoftware.MemoryCards.UI.Models
{
    internal class CardsPositionProvider
    {
        public static CardsPositionsInformation GetCardsPositionsInformation(
            double fieldWidth,
            double fieldHeight,
            GameStage stage)
        {
            const double cardSizeRatio = 3.5 / 2.25;
            const double horizontalSpacingRatio = 2.0;
            const double verticalSpacingRatio = 3.0;

            var numberOfRows = stage.CardsRows.Length;
            var maxRowLenght = stage.CardsRows.Max();

            var desiredCardWidth = fieldWidth / (maxRowLenght + 2.0 + ((maxRowLenght - 1) / horizontalSpacingRatio));
            var desiredCardHeight = fieldHeight / (numberOfRows + 1.0 + ((numberOfRows - 1) / verticalSpacingRatio));

            var cardWidth = desiredCardWidth;
            var cardHeight = desiredCardWidth * cardSizeRatio;
            if (cardSizeRatio > desiredCardHeight)
            {
                cardHeight = desiredCardHeight;
                cardWidth = desiredCardHeight / cardSizeRatio;
            }

            Contract.Assume(cardWidth > 0);
            Contract.Assume(cardHeight > 0);

            return new CardsPositionsInformation(
                cardWidth,
                cardHeight,
                cardWidth / horizontalSpacingRatio,
                cardHeight / verticalSpacingRatio,
                (fieldHeight - numberOfRows * cardHeight - (numberOfRows - 1) * cardHeight / verticalSpacingRatio) / 2.0,
                stage.CardsRows.Select(cardsInRow => (fieldWidth - cardsInRow * cardWidth - (cardsInRow - 1) * cardWidth / horizontalSpacingRatio) / 2.0).ToArray()
                );
        }
    }
}