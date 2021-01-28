using DX.Common;
using DX.Models;
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

        private string _httpContent = "";
        public string HttpContent
        {
            get { return _httpContent; }
            set { SetProperty(ref _httpContent, value); }
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
                    if (blockBody.PacketData.Length > 60)
                    {
                        PacketData packetData = new PacketData(blockBody.PacketData);
                        packetData.Time = blockBody.Time;
                        if (packetData.IP_Protocol==6)
                        {
                            Packist.Add(packetData);
                        }
                        
                        str.Append(packetData.HTTP);
                    }
                    
                    

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
            int no = -1;
            foreach (var datalist in mp.Values)
            {
                List<string> httpreqreslist = new List<string>();
                int indx = 0;
                StringBuilder builder = new StringBuilder("");
                PacketData packbuffer = datalist[0];
                if (!datalist[0].HTTP.StartsWith("GET")&& !datalist[0].HTTP.StartsWith("POST"))
                {
                    indx = 2;
                }
                for (int i = indx; i < datalist.Count; i++)
                {
                    if (datalist[i].HTTP.StartsWith("GET")|| datalist[i].HTTP.StartsWith("POST"))
                    {
                        httpreqreslist.Add(builder.ToString());
                        //
                        no += 1;
                        listViewModel.Add(CreatListViewModel(packbuffer, builder.ToString(),true,no));
                        builder.Clear();
                    }
                    if (datalist[i].HTTP.StartsWith("HTTP"))
                    {
                        httpreqreslist.Add(builder.ToString());
                        no += 1;
                        listViewModel.Add(CreatListViewModel(packbuffer, builder.ToString(), false, no));
                        builder.Clear();
                    }
                    packbuffer = datalist[i];
                    builder.Append(datalist[i].HTTP);

                }
            }
            listViewModel.Remove(listViewModel[0]);
            TcpPackets = listViewModel;
        }




        private ListView_Model CreatListViewModel(PacketData data,string message, bool reqres,int no) 
        {
            
            ListView_Model ls = new ListView_Model();
            ls.ID = no;
            ls.IP_DestinationAddress = string.Join(",", data.IP_DestinationAddress);
            ls.IP_SourceAddress = string.Join(",", data.IP_SourceAddress);
            ls.TCP_DestinationPort = data.TCP_DestinationPort;
            ls.TCP_SourcePort = data.TCP_SourcePort;
            ls.Message = message;
            ls.isreqres = reqres;
            

            if (message!="")
            {
                string[] a = message.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
                ls.Content = a[1];
            }
            



            return ls;

        }
        #region dropped source
        private void CreateHttpFromStringbuffer(string buffer)
        {
            List<string> strlist = new List<string>();
            StringBuilder httpbuffer = new StringBuilder("");
            int length = -1;
            string[] strArray = buffer.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            for (int i = 0; i < strArray.Length; i++)
            {
                httpbuffer.Append(strArray[i]);

                if (strArray[i].ToString().Contains("Content-Length: "))
                {
                    length = httplength(strArray[i]);
                }
                else if (httpbuffer.ToString().StartsWith("GET") &&
                          httpbuffer.ToString().Contains("HTTP/") &&
                          strArray[i] == "")
                {
                    strlist.Add(httpbuffer.ToString().Clone() as string);
                }
                else if (httpbuffer.ToString().StartsWith("POST") && httpbuffer.ToString().Contains("HTTP/")
                   && httpbuffer.ToString().Contains("\r\n") && httpbuffer.ToString().Contains("Content-Length: ")
                   && length != -1)
                {
                    string[] a = strArray[i].Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    string str = httpbuffer.ToString();
                    str.Replace(a[1], "");
                    strlist.Add(str.Clone() as string);
                    httpbuffer.Clear();
                    httpbuffer.Append(a[0]);
                    length = -1;
                }
                else if (httpbuffer.ToString().StartsWith("HTTP/")
                    && httpbuffer.ToString().Contains("\r\n") && httpbuffer.ToString().Contains("Content-Length: ")
                    && length != -1)
                {
                    string[] a = strArray[i].Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    string str = httpbuffer.ToString();
                    str.Replace(a[1], "");
                    strlist.Add(str.Clone() as string);
                    httpbuffer.Clear();
                    httpbuffer.Append(a[0]);
                    length = -1;
                }

            }
        }


        private int httplength(string str)
        {
            string c = "Content-Length: ";
            char[] charArray = c.ToCharArray();

            return int.Parse(str.Remove(0, 16));
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

                if (TCP_DestinationPort != data.TCP_DestinationPort)
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
        #endregion







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
