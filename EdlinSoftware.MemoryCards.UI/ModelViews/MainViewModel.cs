using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Windows;
using System.Windows.Input;
using EdlinSoftware.MemoryCards.UI.Models;
using EdlinSoftware.MemoryCards.UI.Properties;

namespace EdlinSoftware.MemoryCards.UI.ModelViews
{
    internal class MainViewModel : BaseViewModel
    {
        private Game _game;
        private string _currentGameFolder;
        private int _currentStageIndex;
        private StageViewModel _currentStage;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ObservableCollection<GameMenuItemViewModel> _games =
            new ObservableCollection<GameMenuItemViewModel>();

        [DebuggerStepThrough]
        public MainViewModel()
        {
            ReadExistingGames();
        }

        private void ReadExistingGames()
        {
            var startGameCommand = new DelegateCommand(arg =>
            {
                var gameFolder = (string)arg;

                var gameResult = GameReader.ReadGame(gameFolder);
                if (gameResult.Status == ResultStatus.Failure)
                {
                    MessageBox.Show(gameResult.ErrorMessage);
                    return;
                }

                _game = gameResult.Value;
                _currentGameFolder = gameFolder;

                StartStage(0);
            });

            foreach (var gameFolder in GameReader.GetGameFolders())
            {
                Contract.Assume(gameFolder != null);

                _games.Add(new GameMenuItemViewModel(
                    Path.GetFileName(gameFolder),
                    gameFolder,
                    startGameCommand
                    ));
            }
        }

        private void StartStage(int stageIndex)
        {
            if (CurrentStage != null)
                CurrentStage.StageIsFinished -= OnStageFinished;

            _currentStageIndex = stageIndex;
            if (_currentStageIndex >= _game.Stages.Length)
            {
                CurrentStage = null;
            }
            else
            {
                CurrentStage = new StageViewModel(_game.Stages[_currentStageIndex], _currentGameFolder);
                CurrentStage.StageIsFinished += OnStageFinished;
            }
        }

        private void OnStageFinished(object sender, EventArgs e)
        {
            StartStage(_currentStageIndex + 1);
        }

        public ObservableCollection<GameMenuItemViewModel> Games
        {
            [DebuggerStepThrough]
            get { return _games; }
        }

        public StageViewModel CurrentStage
        {
            [DebuggerStepThrough]
            get { return _currentStage; }
            set
            {
                if (_currentStage != value)
                {
                    _currentStage = value;
                    OnPropertyChanged();
                }
            }
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
    }
}
