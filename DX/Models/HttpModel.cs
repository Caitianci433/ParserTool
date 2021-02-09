using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX.Models
{
    public enum ErrorCode
    {
        NORMAL = 0,
        NET_TIMEOUT,
        NET_DELAY_RESPONSE,
        NET_NO_RESPONSE,
        HTTP_ERROR,
        RESPONSE_ERROR
    }

    public class HttpModel :IComparable
    {
        public HttpModel() 
        {
            ErrorCode = ErrorCode.NORMAL;
        }

        
        public string IP_SourceAddress { get; set; }
        public string IP_DestinationAddress { get; set; }
        public int TCP_SourcePort { get; set; }
        public int TCP_DestinationPort { get; set; }
        public ulong Time { get; set; }
        public string Head { get; set; }
        public string Body { get; set; }
        public string Content { get; set; }


        public ErrorCode ErrorCode { get; set; }

        public int CompareTo(object obj)
        {
            return Time.CompareTo((obj as HttpModel).Time);
        }
    }
}
