using DX.ViewModels;
using System.Windows;

namespace DX.Views
{
    /// <summary>
    /// Interaction logic for FliterWindow.xaml
    /// </summary>
    public partial class FliterWindow : Window
    {
        public FliterWindow(MainWindowViewModel dataContext)
        {
            InitializeComponent();
            this.DataContext = new FliterWindowViewModel(dataContext);
        }
    }
}
