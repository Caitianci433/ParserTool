using DX.Models;
using DX.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;

namespace DX.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();
            this.DataContext = mainWindowViewModel;

            this.AllowDrop = true;

            this.DragEnter += (s, e) =>
            {
                mainWindowViewModel.Grid_DragEnter(s, e);
            };

            this.Drop += (s, e) =>
            {
                mainWindowViewModel.Grid_Drop(s, e);
            };


        }

        private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            (this.DataContext as MainWindowViewModel).TextMessage  = (ListView.SelectedItem as ListView_Model).Message;
            (this.DataContext as MainWindowViewModel).HttpContent = (ListView.SelectedItem as ListView_Model).Content;
        }
    }
}
