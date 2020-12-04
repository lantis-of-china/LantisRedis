using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;
using System.Net.Sockets;

namespace Lantis.Network
{
    public class MessageContent : LantisPoolInterface
    {
        public byte[] data;
        public Socket socket;
        public string ip;
        public int port;

        public void OnPoolDespawn()
        {
        }

        public void OnPoolSpawn()
        {
            data = null;
            socket = null;
            ip = string.Empty;
            port = 0;
        }
    }
}
