using DX.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX.Models
{
    class CompareFileModel
    {
        public string IP { get; set; }

        public int Port { get; set; }

        public string CommandType { get; set; }

        public int LeftCommandInterval { get; set; }

        public int LeftReqResInterval { get; set; }

        public int RightCommandInterval { get; set; }

        public int RightReqResInterval { get; set; }

    }
}
