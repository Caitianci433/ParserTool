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
        
        public FliterWindowViewModel(MainWindowViewModel dataContext)
        {
            TcpPackets = from iteam in dataContext.HttpList where iteam.Content.Length<15 && iteam.Content.Length>0 select iteam;
        }


        private IEnumerable<HttpModel> _tcppackets;
        public IEnumerable<HttpModel> TcpPackets
        {
            get { return _tcppackets; }
            set { SetProperty(ref _tcppackets, value); }
        }
    }
}
