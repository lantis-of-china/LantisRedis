using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.EntityComponentSystem;
using System.Net.Sockets;
using System.Net;
using Lantis.Extend;
using Lantis.Pool;
using Lantis.Network;

namespace Lantis.RedisExecute.Components
{
    public class NetWorkComponents : Entity
    {
        private Socket serverSocket;
        private LantisDictronaryList<Socket,MessageReciver> messageReciverMap;

        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();
            messageReciverMap = LantisPoolSystem.GetPool<LantisDictronaryList<Socket, MessageReciver>>().NewObject();
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();
            LantisPoolSystem.GetPool<LantisDictronaryList<Socket, MessageReciver>>().DisposeObject(messageReciverMap);
            messageReciverMap = null;
        }

        public override void OnAwake()
        {
            base.OnAwake();
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9990));
                serverSocket.Listen(50);
                serverSocket.BeginAccept(OnAccept, serverSocket);
            }
            catch
            {
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void OnAccept(IAsyncResult ar)
        {
            var myServer = ar.AsyncState as Socket;

            try
            {
                var client = myServer.EndAccept(ar);
                var messageReciver = LantisPoolSystem.GetPool<MessageReciver>().NewObject();
                messageReciver.Start(client, OnReciveMessage, OnExeception);
                messageReciverMap.AddValue(client, messageReciver);
            }
            catch
            {
            }
            finally
            {
                myServer.BeginAccept(OnAccept, myServer);
            }
        }

        private void OnReciveMessage(byte[] datas, Socket socket, string ip, int port)
        { }

        private void OnExeception()
        { }

        public void SendMessage(byte[] datas, Socket remoteSocket)
        {
            if (messageReciverMap.HasKey(remoteSocket))
            {
                var messageSender = LantisPoolSystem.GetPool<MessageSender>().NewObject();
                messageSender.SetSender(remoteSocket, datas);
                MessageSenderManager.AddSender(messageSender);
            }
        }
    }
}
