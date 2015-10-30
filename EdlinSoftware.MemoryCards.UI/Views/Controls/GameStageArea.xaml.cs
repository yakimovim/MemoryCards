using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using EdlinSoftware.MemoryCards.UI.Events;
using EdlinSoftware.MemoryCards.UI.Models;
using EdlinSoftware.MemoryCards.UI.ModelViews;
using EdlinSoftware.MemoryCards.UI.Views.Support.Converters;

namespace EdlinSoftware.MemoryCards.UI.Views.Controls
{
    public partial class GameStageArea : 
        IListener<StartNewStage>, 
        IListener<GameIsWon>,
        IListener<StageIsLost>
    {
        private GameStage _stage;
        private string _gameFolder;
        private StageCards _cards;

        public GameStageArea()
        {
            InitializeComponent();

            EventBroker.Instance.AddListener(this);
        }

        private void OnClickOnCard(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var rectangle = (Rectangle)e.Source;

            var card = (CardViewModel)rectangle.DataContext;

            _cards.ProcessClickOnCard(card);
        }

        private void OnRestartLevel(object sender, RoutedEventArgs e)
        {
            EventBroker.Instance.SendMessage(new StartNewStage(_stage, _gameFolder));
        }

        void IListener<StartNewStage>.Handle(StartNewStage message)
        {
            _stage = message.NewStage;
            _gameFolder = message.GameFolder;

            DrawCards(message);
        }

        private void DrawCards(StartNewStage message)
        {
            GameCanvas.Children.Clear();

            var images = new ImagesProvider().GetImagesPaths(message.GameFolder).Value;

            var cardsPositionsInfo = CardsPositionProvider.GetCardsPositionsInformation(
                GameCanvas.Width,
                GameCanvas.Height,
                _stage);

            var cardsValues = CardValuesProvider.CreateCardsValues(_stage);

            var cards = new List<CardViewModel>();

            var top = cardsPositionsInfo.FirstRowTop;

            var rowIndex = 0;
            foreach (var numberOfCardsInTheRow in _stage.CardsRows)
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
                        ConverterParameter = images[card.Value]
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

            _cards = new StageCards(_stage, cards.ToArray());
        }

        void IListener<GameIsWon>.Handle(GameIsWon message)
        {
            GameCanvas.Children.Clear();
            GameCanvas.Children.Add(GameWonTextBox);
            GameWonTextBox.Visibility = Visibility.Visible;
        }

        void IListener<StageIsLost>.Handle(StageIsLost message)
        {
            GameCanvas.Children.Clear();
            GameCanvas.Children.Add(StageIsLostPanel);
            StageIsLostPanel.Visibility = Visibility.Visible;
        }
    }
}
