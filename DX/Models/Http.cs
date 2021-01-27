using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX.Models
{
    public class Http 
    {
        public string http;
    }

    public class HttpRequest:Http
    {
        public int[] IP_SourceAddress { get; set; }
        public int[] IP_DestinationAddress { get; set; }
        public int TCP_SourcePort { get; set; }
        public int TCP_DestinationPort { get; set; }
        public string RequestMessage { get; set; }
        public string RequestHead { get; set; }
        public string RequestBlank { get; set; }
        public string RequestContent { get; set; }


    }

    public class HttpResponse : Http
    {
        public int[] IP_SourceAddress { get; set; }
        public int[] IP_DestinationAddress { get; set; }
        public int TCP_SourcePort { get; set; }
        public int TCP_DestinationPort { get; set; }
        public string ResponseMessage { get; set; }
        public string ResponseHead { get; set; }
        public string ResponseBlank { get; set; }
        public string ResponseContent { get; set; }

    }
}
