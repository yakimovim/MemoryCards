using System.Windows;
using EdlinSoftware.MemoryCards.UI.Models;
using EdlinSoftware.MemoryCards.UI.ModelViews;

namespace EdlinSoftware.MemoryCards.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel(new Person());
        }
    }
}
