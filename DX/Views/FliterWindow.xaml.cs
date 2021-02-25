using DX.ViewModels;
using System.Windows;

namespace DX.Views
{
    /// <summary>
    /// Interaction logic for FliterWindow.xaml
    /// </summary>
    public partial class FliterWindow : Window
    {
        private MainWindowViewModel maindataContext;

        public FliterWindow(MainWindowViewModel dataContext)
        {
            InitializeComponent();
            this.maindataContext = dataContext;
            FliterWindowViewModel fliterWindowViewModel = new FliterWindowViewModel(dataContext);
            this.DataContext = fliterWindowViewModel;

        }

        private void errorlist_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (errorlist.SelectedItem!=null)
            {
                maindataContext.TcpPacket = (Models.HttpModel)errorlist.SelectedItem;
            }
        }

        private void warninglist_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (warninglist.SelectedItem != null)
            {
                maindataContext.TcpPacket = (Models.HttpModel)warninglist.SelectedItem;
            }
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            
        }
    }
}
