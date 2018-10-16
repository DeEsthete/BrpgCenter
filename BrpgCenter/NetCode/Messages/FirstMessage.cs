using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class FirstMessage
    {
        public Player Player { get; set; }
        public TcpTypeEnum TcpType { get; set; }
    }
}
