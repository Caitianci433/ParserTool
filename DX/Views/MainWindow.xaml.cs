using DX.Common;
using DX.Models;
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
            if (ListView.SelectedItem!=null)
            {
                this.header_detail.Text = (ListView.SelectedItem as HttpModel).Head;
                this.body_detail.Text = (ListView.SelectedItem as HttpModel).Body;
                this.content_detail.Text = (ListView.SelectedItem as HttpModel).Content;

                this.endtext.Text = "IP: FROM "+ (ListView.SelectedItem as HttpModel).IP_SourceAddress + ":"+(ListView.SelectedItem as HttpModel).TCP_SourcePort
                                   + "     ------->     TO " + (ListView.SelectedItem as HttpModel).IP_DestinationAddress+":" + (ListView.SelectedItem as HttpModel).TCP_DestinationPort;

                byte[] byteArray = System.Text.Encoding.Default.GetBytes((ListView.SelectedItem as HttpModel).Content);

                (this.DataContext as MainWindowViewModel).TextMessage = Tools.BytesToShowBytes(byteArray);
            }
        }

      

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pcapng Parser\r\nVerson 0.1\r\nAuthor: caitianci@hyron.com");
        }

        private void OnSearch(object sender, TextChangedEventArgs e)
        {
            List<HttpModel> list = (this.DataContext as MainWindowViewModel).HttpList;
            ListView.ItemsSource = from iteam in list where iteam.Content.Contains(searchtext.Text) select iteam;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combobox.SelectedItem!=null)
            {
                int port = (int)combobox.SelectedItem;
                List<HttpModel> list = (this.DataContext as MainWindowViewModel).HttpList;
                ListView.ItemsSource = from iteam in list where (iteam.TCP_DestinationPort == port) || (iteam.TCP_SourcePort == port) select iteam;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            combobox.SelectedItem = null;
            ListView.ItemsSource = (this.DataContext as MainWindowViewModel).HttpList;
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            FliterWindow fliter = new FliterWindow();
            FliterWindowViewModel fliterWindowViewModel = new FliterWindowViewModel();
            fliterWindowViewModel.mainwindow = this;
            fliterWindowViewModel.TcpPackets = (this.DataContext as MainWindowViewModel).TcpPackets;
            fliter.DataContext = fliterWindowViewModel;

            fliter.Show();
        }
    }
}
