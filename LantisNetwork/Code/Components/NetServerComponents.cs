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
        private Func<int, byte[], byte[]> sendMessageDataGetCall;
        private Action sucessCall;
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


        /// <summary>
        /// create component awake params
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="reciveMessageCall"></param>
        /// <param name="getSendMessageCall"></param>
        /// <param name="exceptionCall"></param>
        /// <returns></returns>
        public static object[] ParamCreate(string ip, int port, Action<byte[], Socket, string, int> reciveMessageCall, Func<int, byte[], byte[]> getSendMessageCall, Action sucessCall,Action exceptionCall)
        {
            return new object[] { ip, port, reciveMessageCall, getSendMessageCall, sucessCall,exceptionCall };
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);
            
            SafeRun(delegate
            {
                var ip = paramsData[0] as string;
                var port = (int)paramsData[1];
                reciveMessageCall = (Action<byte[], Socket, string, int>)paramsData[2];
                sendMessageDataGetCall = (Func<int,byte[],byte[]>)paramsData[3];
                sucessCall = (Action)paramsData[4];
                exceptionCall = (Action)paramsData[5];

                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var entity = GetEntity<Entity>();

                if (reciveMessageCall == null)
                {
                    var messageDriver = entity.GetComponent<NetMessageDriverComponents>();

                    if (messageDriver != null)
                    {
                        reciveMessageCall = messageDriver.OnReciveMessage;
                    }
                    else
                    {
                        Logger.Error("you can't use message recive,becuse you must be add NetMessageDriverComponents befrom current component added!");
                    }
                }

                BindAddress(ip, port);
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

        private void BindAddress(string ip, int port)
        {
            SafeRun(delegate
            {
                try
                {
                    serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                    serverSocket.Listen(100);
                    OnSucessCall();
                    BeginAccept();
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                    OnExeception();
                }
            });
        }

        private void BeginAccept()
        {
            SafeRun(delegate
            {
                try
                {
                    serverSocket.BeginAccept(OnAccept, serverSocket);
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                    OnExeception();
                }
            });
        }

        private void OnAccept(IAsyncResult ar)
        {
            SafeRun(delegate 
            {
                var server = ar.AsyncState as Socket;

                try
                {
                    var client = server.EndAccept(ar);
                    var messageReciver = LantisPoolSystem.GetPool<MessageReciver>().NewObject();
                    messageReciver.Start(client, OnReciveMessage, OnExeception);
                    messageReciverMap.AddValue(client, messageReciver);
                }
                catch(Exception e)
                {
                    Logger.Error(e.ToString());
                    OnExeception();
                }
                finally
                {
                    server.BeginAccept(OnAccept, server);
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

        private void OnSucessCall()
        {
            SafeRun(delegate
            {
                if (sucessCall != null)
                {
                    sucessCall();
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

        public void SendMessage(int id,byte[] datas, Socket remoteSocket)
        {
            SafeRun(delegate
            {
                if (messageReciverMap.HasKey(remoteSocket))
                {
                    var messageSender = LantisPoolSystem.GetPool<MessageSender>().NewObject();
                    messageSender.SetSender(remoteSocket, sendMessageDataGetCall(id, datas));
                    MessageSenderManager.AddSender(messageSender);
                }
            });
        }
    }
}
