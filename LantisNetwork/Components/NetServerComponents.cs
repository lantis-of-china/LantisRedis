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

namespace Lantis.Network
{
    public class NetServerComponents : ComponentEntity
    {
        private Socket serverSocket;
        private Action<byte[], Socket, string, int> reciveMessageCall;
        private Action exceptionCall;
        private LantisDictronaryList<Socket,MessageReciver> messageReciverMap;

        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();

            SafeRun(delegate
            {
                messageReciverMap = LantisPoolSystem.GetPool<LantisDictronaryList<Socket, MessageReciver>>().NewObject();
            });
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();

            SafeRun(delegate
            {
                LantisPoolSystem.GetPool<LantisDictronaryList<Socket, MessageReciver>>().DisposeObject(messageReciverMap);
                messageReciverMap = null;
            });
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);
            
            SafeRun(delegate
            {
                var ip = paramsData[0] as string;
                var port = (int)paramsData[1];
                reciveMessageCall = (Action<byte[], Socket, string, int>)paramsData[2];
                exceptionCall = (Action)paramsData[3];
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var entity = GetEntity<Entity>();
                var messageDriver = entity.GetComponent<NetMessageDriverComponents>();

                if (reciveMessageCall == null)
                {
                    if (messageDriver != null)
                    {
                        reciveMessageCall = messageDriver.OnReciveMessage;
                    }
                    else
                    {
                    }
                }

                try
                {
                    serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                    serverSocket.Listen(50);
                    serverSocket.BeginAccept(OnAccept, serverSocket);
                }
                catch
                {
                    
                }
            });
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
            SafeRun(delegate 
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
            });
        }

        private void OnReciveMessage(byte[] datas, Socket socket, string ip, int port)
        {
            SafeRun(delegate
            {
                if (reciveMessageCall != null)
                {
                    reciveMessageCall(datas, socket, ip, port);
                }
            });
        }

        private void OnExeception()
        {
            SafeRun(delegate
            {
                if (exceptionCall != null)
                {
                    exceptionCall();
                }
            });
        }

        public void SendMessage(byte[] datas, Socket remoteSocket)
        {
            SafeRun(delegate
            {
                if (messageReciverMap.HasKey(remoteSocket))
                {
                    var messageSender = LantisPoolSystem.GetPool<MessageSender>().NewObject();
                    messageSender.SetSender(remoteSocket, datas);
                    MessageSenderManager.AddSender(messageSender);
                }
            });
        }
    }
}
