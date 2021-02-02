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
        NET_NO_RESPONSE,
        HTTP_ERROR,
        RESPONSE_ERROR
    }

    public class ListView_Model 
    {
        public ListView_Model() 
        {
            ErrorCode = ErrorCode.NORMAL;
        }

        public int ID { get; set; }
        public string IP_SourceAddress { get; set; }
        public string IP_DestinationAddress { get; set; }
        public int TCP_SourcePort { get; set; }
        public int TCP_DestinationPort { get; set; }
        public ulong Time { get; set; }
        public string Head { get; set; }
        public string Body { get; set; }
        public string Content { get; set; }

        public int ContentLength { get; set; }
        public string PLC_PC { get; set; }
        public string Kind { get; set; }

        public ErrorCode ErrorCode { get; set; }

    }
}
