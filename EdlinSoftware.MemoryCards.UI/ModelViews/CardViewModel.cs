using System.Diagnostics;

namespace EdlinSoftware.MemoryCards.UI.ModelViews
{
    internal class CardViewModel : BaseViewModel
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CardState _state;

        [DebuggerStepThrough]
        public CardViewModel(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public CardState State
        {
            [DebuggerStepThrough]
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    internal enum CardState
    {
        Hidden,
        TemporarilyOpened,
        Opened
    }
}