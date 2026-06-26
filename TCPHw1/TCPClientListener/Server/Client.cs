using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Client
    {
        public string Name { get; set; } = "noname";
        public TcpClient? TcpClient { get; set; } = null;
        public DateTime When { get; set; }
        public List<string> quotes = new List<string>();
    }
}
