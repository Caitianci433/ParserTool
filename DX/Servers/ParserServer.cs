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
        public HttpModel req { get; set; }
        public HttpModel res { get; set; }
    }
    public class ParserServer
    {
        readonly static int timeout = 5;

        public static Dictionary<int, List<HttpModel>> mp = new Dictionary<int, List<HttpModel>>();
        //req,res
        public static List<ReqRse> list = new List<ReqRse>();

        public static void DispacherPacket(List<HttpModel> Packist)
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
        public static void ParserCheck() 
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
                    reqRse.req = httplist[i];
                    list.Add(reqRse);
                    i++;
                   continue;
               }
               if (httplist[i].TCP_DestinationPort == 80 && httplist[i + 1].TCP_DestinationPort != 80)
               {
                   //req+res
                   if (httplist[i].Time - httplist[i + 1].Time < (ulong)timeout)
                   {

                   }
                   else
                   {
                       httplist[i].ErrorCode = ErrorCode.NET_TIMEOUT;
                       httplist[i + 1].ErrorCode = ErrorCode.NET_TIMEOUT;
                   }
                    ReqRse reqRse = new ReqRse();
                    reqRse.req = httplist[i];
                    reqRse.res = httplist[i+1];
                    list.Add(reqRse);
                    i += 2;
               }
               else if (httplist[i].TCP_DestinationPort == 80 && httplist[i + 1].TCP_DestinationPort == 80)
               {
                   //req+req
                   httplist[i].ErrorCode = ErrorCode.NET_NO_RESPONSE;
                    ReqRse reqRse = new ReqRse();
                    reqRse.req = httplist[i];
                    list.Add(reqRse);
                    i++;

               }
               else if (httplist[i].TCP_DestinationPort != 80 && httplist[i + 1].TCP_DestinationPort != 80)
               {
                   //res+res
                   httplist[i].ErrorCode = ErrorCode.NET_NO_RESPONSE;
                    ReqRse reqRse = new ReqRse();
                    reqRse.res = httplist[i];
                    list.Add(reqRse);
                    i++;
               }

           }



        }

        private static void ParserCheckHTTP(List<ReqRse> httplist)
        {
            foreach (var http in httplist)
            {
                if (http.req !=null && http.res != null)
                {
                    if (http.res.Head != "HTTP/1.1 200 OK")
                    {
                        http.res.ErrorCode = ErrorCode.HTTP_ERROR;
                    }
                    
                }
            }
        }

        private static void ParserCheckResponse(List<ReqRse> httplist)
        {
            foreach (var http in httplist)
            {
                if (http.req != null && http.res != null)
                {
                    //download
                    if (http.req.Content.Contains("FileList_get") || 
                        http.req.Content.Contains("FileList_put") ||
                        http.req.Content.Contains("FileList_getFileInfo") ||
                        http.req.Content.Contains("FileList_getDefaultFileInfo") ||
                        http.req.Content.Contains("File_beginDownload") ||
                        http.req.Content.Contains("File_beginPartiallyDownload") ||
                        http.req.Content.Contains("File_prepareDownload") ||
                        http.req.Content.Contains("File_download") ||
                        http.req.Content.Contains("File_flushDownload") ||
                        http.req.Content.Contains("File_endDownload") ||
                        http.req.Content.Contains("File_flush") ||
                        http.req.Content.Contains("File_beginLazyDownload") ||
                        http.req.Content.Contains("CPU_notifyParameterUpdated") ||
                        http.req.Content.Contains("CPU_isChangeConnectingIpAddr") ||
                        http.req.Content.Contains("File_beginUpload") ||
                        http.req.Content.Contains("File_upload") ||
                        http.req.Content.Contains("File_endUpload") ||
                        http.req.Content.Contains("Sync_lock") ||
                        http.req.Content.Contains("Sync_unlock") ||
                        http.req.Content.Contains("File_beginTarceDownload") ||
                        http.req.Content.Contains("File_endTraceDownload") 
                        )
                    {
                        
                        
                    }

                }
            }
        }

    }


}
