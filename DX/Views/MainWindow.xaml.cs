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
            if (ListView.SelectedItem!=null)
            {

                

                this.header_detail.Text = (ListView.SelectedItem as ListView_Model).Head;
                this.body_detail.Text = (ListView.SelectedItem as ListView_Model).Body;
                this.content_detail.Text = (ListView.SelectedItem as ListView_Model).Content;

                this.endtext.Text = "IP: FROM "+ (ListView.SelectedItem as ListView_Model).IP_SourceAddress + ":"+(ListView.SelectedItem as ListView_Model).TCP_SourcePort
                                   + "     ------->     TO " + (ListView.SelectedItem as ListView_Model).IP_DestinationAddress+":" + (ListView.SelectedItem as ListView_Model).TCP_DestinationPort;

                byte[] byteArray = System.Text.Encoding.Default.GetBytes((ListView.SelectedItem as ListView_Model).Content);

                StringBuilder str = new StringBuilder();
                str.Append("ADRESS    |00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F\r\n");
                str.Append("\r\n");
                for (int i = 0,j = 0,k = 0,l = 0; i < byteArray.Length; i++,k++)
                {
                    if (k%16==0)
                    {
                        str.Append(l.ToString("X8"));
                        str.Append("  |");
                        l += 1;
                    }
                    str.Append(byteArray[i].ToString("X2"));
                    str.Append(" ");
                    j += 1;
                    if (j==16)
                    {
                        str.Append("\r\n");
                        j = 0;
                    }
                }

                (this.DataContext as MainWindowViewModel).TextMessage = str.ToString();

            }
            
        }

      

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pcapng Parser\r\nVerson 0.1\r\nAuthor: caitianci@hyron.com");
        }
    }
}
