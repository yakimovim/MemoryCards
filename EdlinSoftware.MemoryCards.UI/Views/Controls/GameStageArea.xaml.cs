using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using EdlinSoftware.MemoryCards.UI.Models;
using EdlinSoftware.MemoryCards.UI.ModelViews;
using EdlinSoftware.MemoryCards.UI.Views.Support.Converters;
using JetBrains.Annotations;

namespace EdlinSoftware.MemoryCards.UI.Views.Controls
{
    public partial class GameStageArea
    {
        public static readonly DependencyProperty StageProperty;

        private CardViewModel[] _cards;
        private string[] _images;

        static GameStageArea()
        {
            FrameworkPropertyMetadata stageMetadata = new FrameworkPropertyMetadata
            {
                PropertyChangedCallback = OnStageChanged
            };

            StageProperty = DependencyProperty.Register("Stage",
                typeof(StageViewModel),
                typeof(GameStageArea),
                stageMetadata);
        }

        internal StageViewModel Stage
        {
            get { return (StageViewModel)GetValue(StageProperty); }
            set { SetValue(StageProperty, value); }
        }

        private static void OnStageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GameStageArea area = (GameStageArea)d;

            area.RecreateStage(e.OldValue != null && e.NewValue == null);
        }

        public GameStageArea()
        {
            InitializeComponent();
        }

        private void RecreateStage(bool gameIsOver)
        {
            _cards = null;
            _images = null;
            GameCanvas.Children.Clear();
            if (gameIsOver)
            {
                GameCanvas.Children.Add(GameOverTextBox);
                GameOverTextBox.Visibility = Visibility.Visible;
            }

            var stage = Stage;
            if (stage == null)
                return;

            _images = CreateImagesArray(stage.GameFolder);

            var cardsPositionsInfo = GetCardsPositionsInformation(stage.Stage);

            var cardsValues = CreateCardsValues(stage.Stage);

            var cards = new List<CardViewModel>();

            var top = cardsPositionsInfo.FirstRowTop;

            var rowIndex = 0;
            foreach (var numberOfCardsInTheRow in stage.Stage.CardsRows)
            {
                var left = cardsPositionsInfo.RowsLefts[rowIndex];

                for (int i = 0; i < numberOfCardsInTheRow; i++)
                {
                    var card = new CardViewModel(cardsValues.Pop());
                    cards.Add(card);

                    var rect = new Rectangle
                    {
                        Height = cardsPositionsInfo.CardHeight,
                        Width = cardsPositionsInfo.CardWidth,
                        DataContext = card
                    };

                    Binding cardBinding = new Binding
                    {
                        Source = card,
                        Path = new PropertyPath("State"),
                        Converter = new CardStateConverter(),
                        ConverterParameter = _images[card.Value]
                    };
                    BindingOperations.SetBinding(rect, Shape.FillProperty, cardBinding);

                    Canvas.SetLeft(rect, left);
                    Canvas.SetTop(rect, top);

                    rect.MouseDown += OnClickOnCard;

                    GameCanvas.Children.Add(rect);

                    left += cardsPositionsInfo.CardWidth + cardsPositionsInfo.HorizontalSpacing;
                }

                top += cardsPositionsInfo.CardHeight + cardsPositionsInfo.VerticalSpacing;
                rowIndex++;
            }

            _cards = cards.ToArray();
        }

        private string[] CreateImagesArray(string gameFolder)
        {
            return Directory.GetFiles(gameFolder, "*.jpg");
        }

        private CardsPositionsInformation GetCardsPositionsInformation(GameStage stage)
        {
            const double cardSizeRatio = 3.5 / 2.25;
            const double horizontalSpacingRatio = 2.0;
            const double verticalSpacingRatio = 3.0;

            var fieldWidth = GameCanvas.Width;
            var fieldHeight = GameCanvas.Height;

            var numberOfRows = stage.CardsRows.Length;
            var maxRowLenght = stage.CardsRows.Max();

            var desiredCardWidth = fieldWidth / (maxRowLenght + 2.0 + ((maxRowLenght - 1) / horizontalSpacingRatio));
            var desiredCardHeight = fieldHeight / (numberOfRows + 1.0 + ((numberOfRows - 1) / verticalSpacingRatio));

            var cardWidth = desiredCardWidth;
            var cardHeight = desiredCardWidth*cardSizeRatio;
            if (cardSizeRatio > desiredCardHeight)
            {
                cardHeight = desiredCardHeight;
                cardWidth = desiredCardHeight/cardSizeRatio;
            }

            Contract.Assume(cardWidth > 0);
            Contract.Assume(cardHeight > 0);

            return new CardsPositionsInformation(
                cardWidth, 
                cardHeight, 
                cardWidth / horizontalSpacingRatio, 
                cardHeight / verticalSpacingRatio, 
                (fieldHeight - numberOfRows * cardHeight - (numberOfRows - 1) * cardHeight / verticalSpacingRatio) / 2.0, 
                stage.CardsRows.Select(cardsInRow => (fieldWidth - cardsInRow*cardWidth - (cardsInRow - 1)*cardWidth/horizontalSpacingRatio)/2.0).ToArray()
                );
        }

        private Stack<int> CreateCardsValues([NotNull] GameStage stage)
        {
            var totalNumberOfCards = stage.CardsRows.Sum();
            var numberOfDifferentCards = totalNumberOfCards / stage.CardsInGroup;

            var cardsValues = new List<int>();

            for (int i = 0; i < numberOfDifferentCards; i++)
            {
                cardsValues.AddRange(Enumerable.Repeat(i, stage.CardsInGroup));
            }

            var rnd = new Random((int)DateTime.UtcNow.Ticks);

            cardsValues.Sort((x, y) => rnd.Next(0, totalNumberOfCards) - (totalNumberOfCards / 2));

            return new Stack<int>(cardsValues);
        }

        private void OnClickOnCard(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var rectangle = (Rectangle)e.Source;

            var card = (CardViewModel)rectangle.DataContext;

            if (card.State != CardState.Hidden)
                return;

            var stage = Stage.Stage;

            var temporaryOpenedCards = _cards
                .Where(c => c.State == CardState.TemporarilyOpened)
                .ToArray();

            if (temporaryOpenedCards.Length < stage.CardsInGroup - 1)
            {
                card.State = CardState.TemporarilyOpened;
                return;
            }

            if (temporaryOpenedCards.Length == stage.CardsInGroup - 1)
            {
                if (temporaryOpenedCards.All(c => c.Value == card.Value))
                {
                    card.State = CardState.Opened;
                    Array.ForEach(temporaryOpenedCards, c => c.State = CardState.Opened);

                    if (_cards.All(c => c.State == CardState.Opened))
                        Stage.FinishStage();

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
