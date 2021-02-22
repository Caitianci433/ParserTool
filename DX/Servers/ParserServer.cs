using DX.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX.Servers
{



    public class ReqRse 
    { 
        public HttpModel Req { get; set; }
        public HttpModel Res { get; set; }
    }
    public class ParserServer
    {
        readonly static int timeout = 3;

        public static Dictionary<int, List<HttpModel>> mp = new Dictionary<int, List<HttpModel>>();
        //req,res
        public static List<ReqRse> list = new List<ReqRse>();

        public static void Parser(List<HttpModel> https) 
        {
            mp.Clear();
            list.Clear();
            DispacherPacket(https);
            ParserCheck();
        }

        
        private static void DispacherPacket(List<HttpModel> Packist)
        {
            IEnumerator itor = Packist.GetEnumerator();
            while (itor.MoveNext())
            {
                HttpModel data = itor.Current as HttpModel;
                if (data.TCP_SourcePort == 80)
                {
                    // res
                    if (!mp.ContainsKey(data.TCP_DestinationPort))
                    {
                        List<HttpModel> newlist = new List<HttpModel>();
                        newlist.Add(data);
                        mp.Add(data.TCP_DestinationPort, newlist);
                    }
                    else
                    {
                        List<HttpModel> oldlist;
                        mp.TryGetValue(data.TCP_DestinationPort, out oldlist);
                        oldlist.Add(data);
                    }
                }
                else if (data.TCP_DestinationPort == 80)
                {
                    // req
                    if (!mp.ContainsKey(data.TCP_SourcePort))
                    {
                        List<HttpModel> newlist = new List<HttpModel>(); ;
                        newlist.Add(data);
                        mp.Add(data.TCP_SourcePort, newlist);
                    }
                    else
                    {
                        List<HttpModel> oldlist;
                        mp.TryGetValue(data.TCP_SourcePort, out oldlist);
                        oldlist.Add(data);
                    }
                }
            }

        }
        private static void ParserCheck() 
        {
            foreach (var httplist in mp)
            {
                ParserCheckNET(httplist.Value);

                ParserCheckHTTP(list);

                ParserCheckResponse(list);
            }

        }
        private static void ParserCheckNET(List<HttpModel> httplist)
        {

           for (int i = 0; i < httplist.Count;)
           {
               if (i == httplist.Count - 1 && httplist[i].TCP_DestinationPort == 80)
               {
                   
                   httplist[i].ErrorCode = ErrorCode.NET_NO_RESPONSE;
                    ReqRse reqRse = new ReqRse();
                    reqRse.Req = httplist[i];
                    list.Add(reqRse);
                    i++;
                   continue;
               }
               if (httplist[i].TCP_DestinationPort == 80 && httplist[i + 1].TCP_DestinationPort != 80)
               {
                   //req+res
                   if (httplist[i].Time - httplist[i + 1].Time > (ulong)timeout*100000)
                   {

                   }
                   else
                   {
                       httplist[i].ErrorCode = ErrorCode.NET_TIMEOUT;
                       httplist[i + 1].ErrorCode = ErrorCode.NET_TIMEOUT;
                   }
                    ReqRse reqRse = new ReqRse();
                    reqRse.Req = httplist[i];
                    reqRse.Res = httplist[i+1];
                    list.Add(reqRse);
                    i += 2;
               }
               else if (httplist[i].TCP_DestinationPort == 80 && httplist[i + 1].TCP_DestinationPort == 80)
               {
                   //req+req
                   httplist[i].ErrorCode = ErrorCode.NET_NO_RESPONSE;
                    ReqRse reqRse = new ReqRse();
                    reqRse.Req = httplist[i];
                    list.Add(reqRse);
                    i++;
               }
               else if (httplist[i].TCP_DestinationPort != 80 && httplist[i + 1].TCP_DestinationPort != 80)
               {
                   //res+res
                   httplist[i].ErrorCode = ErrorCode.NET_NO_RESPONSE;
                    ReqRse reqRse = new ReqRse();
                    reqRse.Res = httplist[i];
                    list.Add(reqRse);
                    i++;
               }
           }
        }

        private static void ParserCheckHTTP(List<ReqRse> httplist)
        {
            foreach (var http in httplist)
            {
                if (http.Req !=null && http.Res != null)
                {
                    if (http.Res.Head != "HTTP/1.1 200 OK")
                    {
                        http.Res.ErrorCode = ErrorCode.HTTP_ERROR;
                    }
                }
            }
        }

        private static void ParserCheckResponse(List<ReqRse> httplist)
        {
            foreach (var http in httplist)
            {
                if (http.Req != null && http.Res != null)
                {
                    //download
                    if (http.Req.Content.Contains("FileList_get") || 
                        http.Req.Content.Contains("FileList_put") ||
                        http.Req.Content.Contains("FileList_getFileInfo") ||
                        http.Req.Content.Contains("FileList_getDefaultFileInfo") ||
                        http.Req.Content.Contains("File_beginDownload") ||
                        http.Req.Content.Contains("File_beginPartiallyDownload") ||
                        http.Req.Content.Contains("File_prepareDownload") ||
                        http.Req.Content.Contains("File_download") ||
                        http.Req.Content.Contains("File_flushDownload") ||
                        http.Req.Content.Contains("File_endDownload") ||
                        http.Req.Content.Contains("File_flush") ||
                        http.Req.Content.Contains("File_beginLazyDownload") ||
                        http.Req.Content.Contains("CPU_notifyParameterUpdated") ||
                        http.Req.Content.Contains("CPU_isChangeConnectingIpAddr") ||
                        http.Req.Content.Contains("File_beginUpload") ||
                        http.Req.Content.Contains("File_upload") ||
                        http.Req.Content.Contains("File_endUpload") ||
                        http.Req.Content.Contains("Sync_lock") ||
                        http.Req.Content.Contains("Sync_unlock") ||
                        http.Req.Content.Contains("File_beginTarceDownload") ||
                        http.Req.Content.Contains("File_endTraceDownload") 
                        )
                    {
                        
                        
                    }

                }
            }
        }

    }


}
