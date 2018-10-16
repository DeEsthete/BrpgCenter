using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class TcpClientObject
    {
        public TcpTypeEnum TcpType { get; set; }
        public TcpClient TcpClient { get; set; }
        public NetworkStream Stream { get; set; }
    }
}
