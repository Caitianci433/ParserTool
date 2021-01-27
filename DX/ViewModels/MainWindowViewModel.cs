using DX.Common;
using DX.Models;
using Prism.Mvvm;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace DX.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private List<Http> httplist = new List<Http>();

        private string _title = "";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
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

        private PacketData _tcppacket;
        public PacketData TcpPacket
        {
            get { return _tcppacket; }
            set { SetProperty(ref _tcppacket, value); }
        }




        public MainWindowViewModel()
        {

        }

        public void Grid_DragEnter(object sender, DragEventArgs e)
        {
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            InitData(fileName);

        }

        
        private void InitData(string path ) 
        {
            StringBuilder str = new StringBuilder();
            List<Block> BlockList = Tools.BytesToBlock(Tools.ReadPcapngFile(path));
            List<PacketData> Packist = new List<PacketData>();
            IEnumerator itor = BlockList.GetEnumerator();
            while (itor.MoveNext())
            {
                if (Tools.bytesToInt((itor.Current as Block).BlockType, 0) == 6)
                {
                    BlockBody blockBody = new BlockBody((itor.Current as Block).BlockBody);
                    PacketData packetData = new PacketData(blockBody.PacketData);
                    Packist.Add(packetData);
                    str.Append(packetData.HTTP);

                }

            }


            DispacherPacket(Packist);
            TextMessage = str.ToString().Length.ToString();
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
            List<ListView_Model> listViewModel = new List<ListView_Model>();
            int no = 0;
            foreach (var datalist in mp.Values)
            {
                foreach (var item in datalist)
                {
                    no += 1;
                    ListView_Model ls = new ListView_Model();
                    ls.ID = no;
                    ls.IP_DestinationAddress = string.Join(",", item.IP_DestinationAddress);
                    ls.IP_SourceAddress = string.Join(",", item.IP_SourceAddress);
                    ls.TCP_DestinationPort = item.TCP_DestinationPort;
                    ls.TCP_SourcePort = item.TCP_SourcePort;
                    listViewModel.Add(ls);
                }


                CreateHttpFromTCP(datalist);
            }
            TcpPackets = listViewModel;

        }
        private void CreateHttpFromTCP(List<PacketData> Packist) 
        {
            Http http = null;
            int TCP_DestinationPort = -1;

            for (int i = 0; i < Packist.Count; i++)
            {
                
                PacketData data = Packist[i];
                


                if (data.TCP_DestinationPort != 80)
                {
                    // res       
                    TCP_DestinationPort = 5000;
                }
                if (data.TCP_DestinationPort == 80)
                {
                    // req
                    TCP_DestinationPort = 80;
                }

                if (TCP_DestinationPort!= data.TCP_DestinationPort)
                {
                   
                    Http httpadd = new Http();

                    httpadd.http = http.http;
                    http = null;
                    httplist.Add(httpadd);
                }
                else
                {
                    if (data.TCP_DestinationPort == 80 && http == null)
                    {
                        http = new HttpRequest();
                        http.http += data.HTTP;
                    }
                    if (data.TCP_DestinationPort != 80 && http == null)
                    {
                        http = new HttpResponse();
                        http.http += data.HTTP;
                    }
                    if (data.TCP_DestinationPort == 80 && http != null)
                    {
                        http.http += data.HTTP;
                    }
                    if (data.TCP_DestinationPort != 80 && http != null)
                    {
                        http.http += data.HTTP;
                    }




                }



            }
                
            
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
