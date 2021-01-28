using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX.Models
{
    public class ListView_Model
    {
        public int ID { get; set; }
        public string IP_SourceAddress { get; set; }
        public string IP_DestinationAddress { get; set; }
        public int TCP_SourcePort { get; set; }
        public int TCP_DestinationPort { get; set; }

        public DateTime Time { get; set; }

        public string Message { get; set; }
        public string Head { get; set; }
        public string Body { get; set; }
        public string Content { get; set; }





        // 0 req
        // 1 res
        public bool isreqres  { get; set; }

    }
}
