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

namespace DX.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private List<ListView_Model> HttpList = new List<ListView_Model>();
        Dictionary<int, List<PacketData>> HttpMap = new Dictionary<int, List<PacketData>>();


        public MainWindowViewModel()
        {
            
        }

        private string _text = "";
        public string TextMessage
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }


        private List<ListView_Model> _tcppackets = new List<ListView_Model>();
        public  List<ListView_Model> TcpPackets
        {
            get { return _tcppackets; }
            set { SetProperty(ref _tcppackets, value); }
        }

        public void Grid_DragEnter(object sender, DragEventArgs e)
        {
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
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
                TcpPackets = HttpList;
            }
            catch (Exception)
            {
                MessageBox.Show("Parser file ERROR!");
                return;
            }
        }

        
        private void InitData(string path ) 
        {
            List<Block> BlockList = Tools.BytesToBlock(Tools.ReadPcapngFile(path));
            List<PacketData> Packist = new List<PacketData>();
            IEnumerator itor = BlockList.GetEnumerator();
            while (itor.MoveNext())
            {
                if (Tools.bytesToInt((itor.Current as Block).BlockType, 0) == 6)
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

            HttpList = HttpFromPacketData(mp);
        }

        private List<ListView_Model> HttpFromPacketData(Dictionary<int, List<PacketData>> mp ) 
        {
            List<ListView_Model> listViewModelforView = new List<ListView_Model>();
            int no = 0;
            foreach (var datalist in mp.Values)
            {
                StringBuilder builder = new StringBuilder("");
                List<ListView_Model> listViewModel = new List<ListView_Model>();
                builder.Append(datalist[0].HTTP);
                for (int i = 1; i < datalist.Count; i++)
                {
                    if (datalist[i].HTTP.StartsWith("GET") || datalist[i].HTTP.StartsWith("POST") || datalist[i].HTTP.StartsWith("HTTP"))
                    {
                        //
                        no += 1;
                        listViewModel.Add(CreatListViewModel(datalist[i - 1], builder.ToString(), no));
                        builder.Clear();
                    }

                    builder.Append(datalist[i].HTTP);
                }
                // buffer flush
                if (builder.ToString().StartsWith("GET") || builder.ToString().StartsWith("POST") || builder.ToString().StartsWith("HTTP"))
                {
                    no += 1;
                    listViewModel.Add(CreatListViewModel(datalist[datalist.Count - 1], builder.ToString(), no));
                    builder.Clear();
                }

                foreach (var item in listViewModel)
                {
                    listViewModelforView.Add(item);
                }
            }

            ParserServer.DispacherPacket(listViewModelforView);
            ParserServer.ParserCheck();
            var a = ParserServer.list;
            var b = ParserServer.mp;

            return listViewModelforView;
        }


        private ListView_Model CreatListViewModel(PacketData data,string message,int no) 
        {
            
            ListView_Model ls = new ListView_Model();
            ls.ID = no;
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

            ls.ContentLength = ls.Content.Length;

            if (data.TCP_DestinationPort == 80)
            {
                ls.PLC_PC = data.TCP_SourcePort + "--------->"+ data.TCP_DestinationPort;
                ls.Kind = "リクエスト";
            }
            else
            {
                ls.PLC_PC = data.TCP_DestinationPort + "<---------" + data.TCP_SourcePort;
                ls.Kind = "レスポンス";
            }
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



    }
}
