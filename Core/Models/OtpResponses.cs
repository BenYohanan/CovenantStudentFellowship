using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class OtpResponses
    {
        public Data Data { get; set; }
        public int _0 { get; set; }
    }

    public class Data
    {
        public string Status { get; set; }
        public string MsgId { get; set; }
        public string Units { get; set; }
        public double Balance { get; set; }
        public string Msg { get; set; }
    }
}
