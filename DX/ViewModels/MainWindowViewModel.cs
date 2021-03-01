using DX.Common;
using DX.Models;
using DX.Servers;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Win32;
using DX.Views;

namespace DX.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private CompareWindow compareWindow = null;

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
        public ICommand MenuOpenCommand => new Command((o) =>
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
            var progressbartestWindow = new ProgressbartestWindow();
            progressbartestWindow.Show();
            StartParser(txtFile);
            progressbartestWindow.Close();
        });

        public ICommand MenuSaveCommand => new Command((o) =>
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
                FileWriterServer.WriteTheFile(filename, HttpList);

            }
        });

        public ICommand MenuCompareCommand => new Command((o) =>
        {
            if (compareWindow == null)
            {
                compareWindow = new CompareWindow();
                compareWindow.Closed += (s, e) => this.compareWindow = null;
                compareWindow.Show();
            }
        });
        #endregion

        private void InitData(string path ) 
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
                        if (packetData.IP_Protocol==6)
                        {
                            Packist.Add(packetData);
                        }
                    }
                }

            }
            DispacherPacket(Packist);
        }

        private void DispacherPacket(List<PacketData> Packist) 
        {
            IEnumerator itor = Packist.GetEnumerator();
            List<int> pl = new List<int>();
            Dictionary<int, List<PacketData>> mp = new Dictionary<int, List<PacketData>>();
            while (itor.MoveNext())
            {
                PacketData data = itor.Current as PacketData;
                if (data.TCP_SourcePort==80)
                {
                    // res
                    if (!mp.ContainsKey(data.TCP_DestinationPort))
                    {
                        List<PacketData> newlist = new List<PacketData>();
                        newlist.Add(data);
                        mp.Add(data.TCP_DestinationPort, newlist);
                    }
                    else
                    {
                        List<PacketData> oldlist;
                        mp.TryGetValue(data.TCP_DestinationPort, out oldlist);
                        oldlist.Add(data);
                    }
                }
                else if(data.TCP_DestinationPort==80)
                {
                    // req
                    if (!mp.ContainsKey(data.TCP_SourcePort))
                    {
                        List<PacketData> newlist = new List<PacketData>();
                        newlist.Add(data);
                        mp.Add(data.TCP_SourcePort, newlist);
                    }
                    else
                    {
                        List<PacketData> oldlist;
                        mp.TryGetValue(data.TCP_SourcePort, out oldlist);
                        oldlist.Add(data);
                    }
                }
            }

            PortList.Clear();
            foreach (var item in mp.Keys)
            {
                pl.Add(item);
            }

            PortList = pl;
            HttpList = HttpFromPacketData(mp);
        }

        private List<HttpModel> HttpFromPacketData(Dictionary<int, List<PacketData>> mp ) 
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


        private HttpModel CreatHttpModel(PacketData data,string message) 
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
        public void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

        }

        public void Grid_DragEnter(object sender, DragEventArgs e)
        {
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            StartParser(fileName);
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
        } 
        private void OnParser()
        {
            ParserServer.Parser(HttpList);
            // state 
            var ErrorList   = from iteam
                              in HttpList
                              where iteam.ErrorCode == ErrorCode.RESPONSE_ERROR || iteam.ErrorCode == ErrorCode.NET_NO_RESPONSE
                              select iteam;

            var WarningList = from iteam
                              in HttpList
                              where iteam.ErrorCode == ErrorCode.NET_TIMEOUT || iteam.ErrorCode == ErrorCode.HTTP_ERROR
                              select iteam;

            if (WarningList.Count()>0)
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
