using DX.Common;
using DX.Models;
using DX.Servers;
using DX.Views;
using Microsoft.Win32;
using Mvvm;
using Prism.Mvvm;
using Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DX.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private FliterWindow fliterWindow = null;

        public List<HttpModel> HttpList = new List<HttpModel>();

        public MainWindowViewModel()
        {

        }

        private List<int> _portlist = new List<int>();
        public List<int> PortList
        {
            get { return _portlist; }
            set { SetProperty(ref _portlist, value); }
        }

        private string _title = "PcapngParser";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }


        private List<HttpModel> _tcppackets = new List<HttpModel>();
        public List<HttpModel> TcpPackets
        {
            get { return _tcppackets; }
            set { SetProperty(ref _tcppackets, value); }
        }

        private HttpModel _tcppacket;
        public HttpModel TcpPacket
        {
            get { return _tcppacket; }
            set { SetProperty(ref _tcppacket, value); }
        }

        private StateCode _state = StateCode.DEFALUT;
        public StateCode State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        #region Command
        public ICommand DropCommand => new Command<DragEventArgs>((e) =>
        {
            string fileName = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            _ = new ProgressbarWindow(() => StartParser(fileName));
        });

        public ICommand MenuOpenCommand => new Command(() =>
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
            _ = new ProgressbarWindow(() => StartParser(txtFile));
        });

        public ICommand MenuSaveCommand => new Command(() =>
        {
            if (Title== "PcapngParser")
            {
                MessageBox.Show("Please Import File");
                return;
            }
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = Title.Split(new string[] { ".pcapng" }, StringSplitOptions.None)[0]; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "Csv documents (.csv)|*.csv"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document 
                string filename = dlg.FileName;
                FileWriterServer.WriteTheFile(filename, HttpList);

            }
        });

        public ICommand MenuCompareCommand => new Command(() =>
        {
            var compareWindow = new CompareWindow();
            compareWindow.ShowDialog();
        });

        public ICommand SearchTextChangedCommand => new Command<TextChangedEventArgs>((e) =>
        {
            string text = (e.Source as TextBox)?.Text;
            TcpPackets = (from iteam in HttpList where iteam.Content.Contains(text ?? "") select iteam).ToList();
        });

        public ICommand SelectionChangedCommand => new Command<SelectionChangedEventArgs>((e) =>
        {
            object selectedItem = (e.Source as ComboBox).SelectedItem;
            if (selectedItem != null)
            {
                int port = (int)selectedItem;
                List<HttpModel> list = HttpList;
                TcpPackets = (from iteam in list where (iteam.TCP_DestinationPort == port) || (iteam.TCP_SourcePort == port) select iteam).ToList();
            }
            else
            {
                TcpPackets = HttpList;

            }
        });

        public ICommand StatisticsClickedCommand => new Command(() =>
        {
            if (fliterWindow == null)
            {
                fliterWindow = new FliterWindow(this);
                fliterWindow.Closed += (s, args) => this.fliterWindow = null;
                fliterWindow.Show();
            }
            else
            {
                fliterWindow.Activate();
            }
        });
        #endregion

        private void InitData(string path)
        {
            List<Block> BlockList = Tools.BytesToBlock(Tools.ReadPcapngFile(path));
            List<PacketData> Packist = new List<PacketData>();
            IEnumerator itor = BlockList.GetEnumerator();
            while (itor.MoveNext())
            {
                if (Tools.BytesToInt((itor.Current as Block).BlockType, 0) == 6)
                {
                    BlockBody blockBody = new BlockBody((itor.Current as Block).BlockBody);
                    if (blockBody.PacketData.Length > 66)
                    {
                        PacketData packetData = new PacketData(blockBody.PacketData);
                        packetData.Time = blockBody.Time;
                        if (packetData.IP_Protocol == 6)
                        {
                            Packist.Add(packetData);
                        }
                    }
                }

            }
            DispatcherPacket(Packist);
        }

        private void DispatcherPacket(List<PacketData> Packist)
        {
            IEnumerator<PacketData> itor = Packist.GetEnumerator();
            Dictionary<int, List<PacketData>> mp = new Dictionary<int, List<PacketData>>();
            while (itor.MoveNext())
            {
                PacketData data = itor.Current;
                if (data.TCP_SourcePort == 80)
                {
                    if (mp.TryGetValue(data.TCP_DestinationPort, out List<PacketData> dataList))
                    {
                        dataList.Add(data);
                    }
                    else
                    {
                        mp.Add(data.TCP_DestinationPort, new List<PacketData>() { data });
                    }
                }
                else if (data.TCP_DestinationPort == 80)
                {
                    if (mp.TryGetValue(data.TCP_SourcePort, out List<PacketData> dataList))
                    {
                        dataList.Add(data);
                    }
                    else
                    {
                        mp.Add(data.TCP_SourcePort, new List<PacketData> { data });
                    }
                }
            }

            PortList = mp.Keys.ToList();
            HttpList = HttpFromPacketData(mp);
        }

        private List<HttpModel> HttpFromPacketData(Dictionary<int, List<PacketData>> mp)
        {
            List<HttpModel> listViewModelforView = new List<HttpModel>();

            foreach (var datalist in mp.Values)
            {
                StringBuilder builder = new StringBuilder("");
                List<HttpModel> listViewModel = new List<HttpModel>();
                builder.Append(datalist[0].HTTP);
                for (int i = 1; i < datalist.Count; i++)
                {
                    if (datalist[i].HTTP.StartsWith("GET") || datalist[i].HTTP.StartsWith("POST") || datalist[i].HTTP.StartsWith("HTTP"))
                    {
                        listViewModel.Add(CreatHttpModel(datalist[i - 1], builder.ToString()));
                        builder.Clear();
                    }

                    builder.Append(datalist[i].HTTP);
                }
                // buffer flush
                if (builder.ToString().StartsWith("GET") || builder.ToString().StartsWith("POST") || builder.ToString().StartsWith("HTTP"))
                {
                    listViewModel.Add(CreatHttpModel(datalist[datalist.Count - 1], builder.ToString()));
                    builder.Clear();
                }

                foreach (var item in listViewModel)
                {
                    listViewModelforView.Add(item);
                }
            }

            return listViewModelforView;
        }


        private HttpModel CreatHttpModel(PacketData data, string message)
        {

            HttpModel ls = new HttpModel();
            ls.IP_DestinationAddress = string.Join(",", data.IP_DestinationAddress);
            ls.IP_SourceAddress = string.Join(",", data.IP_SourceAddress);
            ls.TCP_DestinationPort = data.TCP_DestinationPort;
            ls.TCP_SourcePort = data.TCP_SourcePort;
            ls.Time = data.Time;

            string[] a = message.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
            ls.Content = a[1];
            string[] b = message.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            ls.Head = b[0];
            ls.Body = a[0].Replace(b[0] + "\r\n", "");
            return ls;
        }

        public void StartParser(string fileName)
        {
            Title = fileName + " is loading";

            if (!fileName.EndsWith(".pcapng"))
            {
                MessageBox.Show("Can not Parser this file!");
                return;
            }

            TcpPackets.Clear();
            try
            {
                HttpList.Clear();
                InitData(fileName);
                Title = fileName + " is Parsering";

                OnParser();
                Title = fileName + " Parser Over";
            }
            catch (Exception)
            {
                MessageBox.Show("Parser file ERROR!");
                return;
            }

            if (fliterWindow != null)
            {
                fliterWindow.Close();
                fliterWindow = new FliterWindow(this);
                fliterWindow.Closed += (s, args) => this.fliterWindow = null;
                fliterWindow.Show();
            }
        }
        private void OnParser()
        {
            ParserServer.Parser(HttpList);
            // state 
            var ErrorList = from iteam
                            in HttpList
                            where iteam.ErrorCode == ErrorCode.RESPONSE_ERROR || iteam.ErrorCode == ErrorCode.NET_NO_RESPONSE
                            select iteam;

            var WarningList = from iteam
                              in HttpList
                              where iteam.ErrorCode == ErrorCode.NET_TIMEOUT || iteam.ErrorCode == ErrorCode.HTTP_ERROR
                              select iteam;

            if (WarningList.Count() > 0)
            {
                State = StateCode.ERROR;
            }
            else
            {
                if (ErrorList.Count() > 0)
                {
                    State = StateCode.WARNING;
                }
                else
                {
                    State = StateCode.NORMAL;
                }

            }

            HttpList.Sort();
            TcpPackets = HttpList;
        }
    }
}
