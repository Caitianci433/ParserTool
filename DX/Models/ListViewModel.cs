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


    }
}
