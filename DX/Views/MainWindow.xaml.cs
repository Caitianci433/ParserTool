using DX.Common;
using DX.Models;
using DX.Servers;
using DX.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DX.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FliterWindow fliterWindow=null;
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
      

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pcapng Parser\r\nVerson 0.1\r\nAuthor: caitianci@hyron.com");
        }

        private void OnSearch(object sender, TextChangedEventArgs e)
        {
            List<HttpModel> list = (this.DataContext as MainWindowViewModel).HttpList;
            (this.DataContext as MainWindowViewModel).TcpPackets = (from iteam in list where iteam.Content.Contains(searchtext.Text) select iteam).ToList();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combobox.SelectedItem!=null)
            {
                int port = (int)combobox.SelectedItem;
                List<HttpModel> list = (this.DataContext as MainWindowViewModel).HttpList;
                (this.DataContext as MainWindowViewModel).TcpPackets = (from iteam in list where (iteam.TCP_DestinationPort == port) || (iteam.TCP_SourcePort == port) select iteam).ToList();
            }
            else
            {
                (this.DataContext as MainWindowViewModel).TcpPackets = (this.DataContext as MainWindowViewModel).HttpList;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            combobox.SelectedItem = null;
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            if (fliterWindow==null)
            {
                fliterWindow = new FliterWindow(this.DataContext as MainWindowViewModel);
                fliterWindow.Closed += FliterWindow_Closed;
                fliterWindow.Show();
            }
        }

        private void FliterWindow_Closed(object sender, System.EventArgs e)
        {
            this.fliterWindow = null;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView.ScrollIntoView(ListView.SelectedItem);
        }
    }
}
