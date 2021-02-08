using DX.ViewModels;
using System.Windows;

namespace DX.Views
{
    /// <summary>
    /// Interaction logic for FliterWindow.xaml
    /// </summary>
    public partial class FliterWindow : Window
    {
        public FliterWindow()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (listbox.SelectedItem!=null)
            {
                (this.DataContext as FliterWindowViewModel).mainwindow.ListView.SelectedItem = listbox.SelectedItem;
            }
        }
    }
}
