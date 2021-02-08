using DX.Models;
using DX.Views;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DX.ViewModels
{
    public class FliterWindowViewModel : BindableBase
    {
        
        public FliterWindowViewModel()
        {

        }

        public MainWindow mainwindow { get; set; }

        private List<HttpModel> _tcppackets = new List<HttpModel>();
        public List<HttpModel> TcpPackets
        {
            get { return _tcppackets; }
            set { SetProperty(ref _tcppackets, value); }
        }
    }
}
