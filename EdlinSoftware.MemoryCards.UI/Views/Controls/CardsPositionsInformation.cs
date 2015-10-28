using System.Diagnostics.Contracts;
using System.Linq;

namespace EdlinSoftware.MemoryCards.UI.Views.Controls
{
    internal class CardsPositionsInformation
    {
        public CardsPositionsInformation(
            double cardWidth, 
            double cardHeight, 
            double horizontalSpacing, 
            double verticalSpacing, 
            double firstRowTop, 
            double[] rowsLefts)
        {
            Contract.Requires(cardWidth > 0);
            Contract.Requires(cardHeight > 0);
            Contract.Requires(horizontalSpacing > 0);
            Contract.Requires(verticalSpacing > 0);
            Contract.Requires(firstRowTop > 0);
            Contract.Requires(rowsLefts.All(l => l > 0));

            CardWidth = cardWidth;
            CardHeight = cardHeight;
            HorizontalSpacing = horizontalSpacing;
            VerticalSpacing = verticalSpacing;
            FirstRowTop = firstRowTop;
            RowsLefts = rowsLefts;
        }

        public double CardWidth { get; }
        public double CardHeight { get; }
        public double HorizontalSpacing { get; }
        public double VerticalSpacing { get; }
        public double FirstRowTop { get; }
        public double[] RowsLefts { get; }
    }
}