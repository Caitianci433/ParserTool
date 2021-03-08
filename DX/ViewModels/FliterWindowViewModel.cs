using DX.Models;
using DX.Views;
using Mvvm;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace DX.ViewModels
{
    public class FliterWindowViewModel : ViewModelBase
    {
        private readonly Action<HttpModel> refreshMessage;
        
        public FliterWindowViewModel(MainWindowViewModel dataContext)
        {
            refreshMessage = (date) => dataContext.TcpPacket = date;

            ErrorList = from iteam 
                        in dataContext.HttpList 
                        where iteam.ErrorCode== ErrorCode.RESPONSE_ERROR  || iteam.ErrorCode == ErrorCode.NET_NO_RESPONSE
                        select iteam;

            WarningList = from iteam 
                          in dataContext.HttpList 
                          where iteam.ErrorCode == ErrorCode.NET_TIMEOUT || iteam.ErrorCode == ErrorCode.HTTP_ERROR
                          select iteam;

            Dictionary<string, List<HttpModel>> mp = new Dictionary<string, List<HttpModel>>();
            IPList.Clear();
            foreach (var item in ErrorList)
            {
                if (!mp.ContainsKey(item.IP_DestinationAddress))
                {
                    List<HttpModel> newlist = new List<HttpModel>();
                    newlist.Add(item);
                    mp.Add(item.IP_DestinationAddress, newlist);
                }
                else
                {
                    List<HttpModel> oldlist;
                    mp.TryGetValue(item.IP_DestinationAddress, out oldlist);
                    oldlist.Add(item);
                }
            }

            foreach (var key in mp.Keys)
            {
                IPList.Add(key);
            }
            
        }


        private IEnumerable<HttpModel> _errorlist;
        public IEnumerable<HttpModel> ErrorList
        {
            get { return _errorlist; }
            set { SetProperty(ref _errorlist, value); }
        }


        private IEnumerable<HttpModel> _warninglist;
        public IEnumerable<HttpModel> WarningList
        {
            get { return _warninglist; }
            set { SetProperty(ref _warninglist, value); }
        }

        private List<String> _iplist = new List<string>();
        public List<String> IPList 
        {
            get { return _iplist; }
            set { SetProperty(ref _iplist, value); }
        }

        private bool isErrorExpanded = true;
        public bool IsErrorExpanded
        {
            get => isErrorExpanded;
            set => SetProperty(ref isErrorExpanded, value);
        }

        private bool isWaringExpanded = true;
        public bool IsWaringExpanded
        {
            get => isWaringExpanded;
            set => SetProperty(ref isWaringExpanded, value);
        }

        public ICommand SwitchFoldCommand => new Command<string>((e) =>
        {
            IsErrorExpanded = IsWaringExpanded = e == "1";
        });

        public ICommand ListSelectionChangedCommand => new Command<SelectionChangedEventArgs>((e) =>
        {
            if (((ListView)e.Source).SelectedItem is HttpModel httpModel)
            {
                refreshMessage(httpModel);
            }
        });
    }
}
