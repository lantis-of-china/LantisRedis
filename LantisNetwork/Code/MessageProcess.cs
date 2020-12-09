using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lantis.Network
{
    public abstract class MessageProcess
    {
        public int ID = 0;

        public abstract void Process(byte[] dates,Socket socket, string ip, int port);
    }
}