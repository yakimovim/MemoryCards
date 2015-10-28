using EdlinSoftware.MemoryCards.UI.ModelViews;

namespace EdlinSoftware.MemoryCards.UI.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }
    }
}
