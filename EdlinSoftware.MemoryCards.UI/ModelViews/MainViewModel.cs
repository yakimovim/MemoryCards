using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using EdlinSoftware.MemoryCards.UI.Events;
using EdlinSoftware.MemoryCards.UI.Models;
using EdlinSoftware.MemoryCards.UI.Properties;

namespace EdlinSoftware.MemoryCards.UI.ModelViews
{
    internal class MainViewModel : BaseViewModel, 
        IListener<StartNewGame>,
        IListener<GameIsWon>,
        IListener<StartNewStage>,
        IListener<StageIsWon>,
        IListener<StageIsLost>
    {
        private Game _game;
        private string _currentGameFolder;
        private int _currentStageIndex;
        private GameStage _currentStage;

        private int _timeToSolve;
        private int _timeLeft;

        private DispatcherTimer _timer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ObservableCollection<GameMenuItemViewModel> _games =
            new ObservableCollection<GameMenuItemViewModel>();

        [DebuggerStepThrough]
        public MainViewModel()
        {
            ReadExistingGames();

            EventBroker.Instance.AddListener(this);
        }

        private void ReadExistingGames()
        {
            foreach (var gameFolder in GameReader.GetGameFolders())
            {
                Contract.Assume(gameFolder != null);

                _games.Add(new GameMenuItemViewModel(
                    Path.GetFileName(gameFolder),
                    gameFolder
                    ));
            }
        }

        private void StartStage(int stageIndex)
        {
            _currentStageIndex = stageIndex;
            if (_currentStageIndex >= _game.Stages.Length)
            {
                EventBroker.Instance.SendMessage(new GameIsWon());
            }
            else
            {
                _currentStage = _game.Stages[_currentStageIndex];

                EventBroker.Instance.SendMessage(new StartNewStage(_currentStage, _currentGameFolder));
            }
        }

        public ObservableCollection<GameMenuItemViewModel> Games
        {
            [DebuggerStepThrough]
            get { return _games; }
        }

        public ICommand ExitCommand
        {
            get
            {
                return new DelegateCommand(arg =>
                {
                    var result = MessageBox.Show(
                        Resources.ExitConfirmationText, 
                        Resources.ExitConfirmationTitle, 
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Application.Current.Shutdown(0);
                    }
                });
            }
        }

        public int TimeToSolve
        {
            [DebuggerStepThrough]
            get { return _timeToSolve; }
            set
            {
                if (_timeLeft != value)
                {
                    _timeToSolve = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TimeLeft
        {
            [DebuggerStepThrough]
            get { return _timeLeft; }
            set
            {
                if (_timeLeft != value)
                {
                    _timeLeft = value;
                    OnPropertyChanged();
                }
            }
        }

        private void StartTimer()
        {
            if(_currentStage == null)
                return;
            if(_currentStage.TimeToSolve == 0)
                return;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(Settings.Default.TimerIntervalInMilliseconds)
            };
            _timer.Tick += OnTimeProgress;
            _timer.Start();
        }

        private void OnTimeProgress(object sender, EventArgs e)
        {
            TimeLeft = Math.Max(0, TimeLeft - Settings.Default.TimerIntervalInMilliseconds);
            if (TimeLeft == 0)
            {
                EventBroker.Instance.SendMessage(new StageIsLost(_currentStage));
            }
        }

        private void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }

        void IListener<StartNewGame>.Handle(StartNewGame message)
        {
            _game = message.NewGame;
            _currentGameFolder = message.GameFolder;

            StartStage(0);
        }

        void IListener<GameIsWon>.Handle(GameIsWon message)
        {
            StopTimer();

            TimeToSolve = 0;
            TimeLeft = 0;

            _currentStage = null;
            _currentGameFolder = null;
        }

        void IListener<StartNewStage>.Handle(StartNewStage message)
        {
            TimeToSolve = message.NewStage.TimeToSolve;
            TimeLeft = message.NewStage.TimeToSolve;
            StartTimer();
        }

        void IListener<StageIsWon>.Handle(StageIsWon message)
        {
            StopTimer();

            StartStage(_currentStageIndex + 1);
        }

        void IListener<StageIsLost>.Handle(StageIsLost message)
        {
            StopTimer();
        }
    }
}
