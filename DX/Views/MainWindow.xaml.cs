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
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();

            this.AllowDrop = true;

            this.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effects = DragDropEffects.Link;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            };

            this.Closed += (s,e) => Application.Current.Shutdown();
        }
      
        
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pcapng Parser\r\nVerson 0.1\r\n");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            combobox.SelectedItem = null;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView.ScrollIntoView(ListView.SelectedItem);
        }
    }
}
