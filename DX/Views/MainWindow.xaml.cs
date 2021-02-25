using DX.Common;
using DX.Models;
using DX.Servers;
using DX.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
        private CompareWindow compareWindow = null;
        private ProgressbartestWindow progressbartestWindow ;
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();
            this.DataContext = mainWindowViewModel;

            this.AllowDrop = true;

            this.DragEnter += (s, e) =>
            {
                progressbartestWindow = new ProgressbartestWindow();
                progressbartestWindow.Show();
                mainWindowViewModel.Grid_DragEnter(s, e);
                progressbartestWindow.Close();
            };

            this.Drop += (s, e) =>
            {
                mainWindowViewModel.Grid_Drop(s, e);
            };

            this.Closed += (s,e) => { Application.Current.Shutdown(); };
        }
      
        
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pcapng Parser\r\nVerson 0.1\r\n");
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
        
        private void Menu_OpenOnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select the file";
            openFileDialog.Filter = "pcapng|*.pcapng";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "pcapng";
            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }
            string txtFile = openFileDialog.FileName;
            progressbartestWindow = new ProgressbartestWindow();
            progressbartestWindow.Show();
            (this.DataContext as MainWindowViewModel).StartParser(txtFile);
            progressbartestWindow.Close();
        }

        private void Menu_SaveOnClick(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".text"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document 
                string filename = dlg.FileName;
                FileWriterServer.WriteTheFile(filename);
                
            }
        }

        private void Menu_CompareOnClick(object sender, RoutedEventArgs e)
        {
            if (compareWindow == null)
            {
                compareWindow = new CompareWindow();
                compareWindow.Closed += CompareWindow_Closed;
                compareWindow.Show();
            }
        }
        private void CompareWindow_Closed(object sender, System.EventArgs e)
        {
            this.compareWindow = null;
        }
    }
}
