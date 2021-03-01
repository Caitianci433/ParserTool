using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX.Models
{
    class SaveFileModel
    {
        public string IP { get; set; }

        public int Port { get; set; }

        public string CommandType { get; set; }

        public int CommandInterval { get; set; }

        public int ReqResInterval { get; set; }

    }
}
