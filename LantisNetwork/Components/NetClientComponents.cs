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
    public class NetClientComponents : ComponentEntity
    {
        private bool isOpen;
        private Socket clientSocket;
        private Action<byte[], Socket, string, int> reciveMessageCall;
        private Func<int, byte[], byte[]> sendMessageDataGetCall;
        private Action exceptionCall;
        private MessageReciver messageReciver;

        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();
        }

        /// <summary>
        ///  create component awake params
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="reciveMessageCall"></param>
        /// <param name="getSendMessageCall"></param>
        /// <param name="exceptionCall"></param>
        /// <returns></returns>
        public static object[] ParamCreate(string ip, int port, Action<byte[], Socket, string, int> reciveMessageCall,Func<int,byte[],byte[]> getSendMessageCall, Action exceptionCall)
        {
            return new object[] { ip, port, reciveMessageCall, getSendMessageCall, exceptionCall };
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);
            
            SafeRun(delegate
            {
                var ip = paramsData[0] as string;
                var port = (int)paramsData[1];
                reciveMessageCall = (Action<byte[], Socket, string, int>)paramsData[2];
                sendMessageDataGetCall = (Func<int, byte[], byte[]>)paramsData[3];
                exceptionCall = (Action)paramsData[4];
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var entity = GetEntity<Entity>();

                if (reciveMessageCall == null)
                {
                    var messageDriver = entity.GetComponent<NetMessageDriverComponents>();

                    if (messageDriver != null)
                    {
                        reciveMessageCall = messageDriver.OnReciveMessage;
                        sendMessageDataGetCall = messageDriver.OnGetSenderMessage;
                    }
                    else
                    {
                        Logger.Error("you can't use message recive,becuse you must be add NetMessageDriverComponents befrom current component added!");
                    }
                }

                BeginConnect(ip, port);
            });
        }

        public override void OnEnable()
        {
            base.OnEnable();

            SafeRun(delegate
            {
                isOpen = true;
            });
        }

        public override void OnDisable()
        {
            base.OnDisable();

            SafeRun(delegate
            {
                isOpen = false;
                OnExeception();
            });
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void BeginConnect(string ip,int port)
        {
            SafeRun(delegate
            {
                try
                {
                    clientSocket.Blocking = true;
                    clientSocket.SendTimeout = 3000;
                    clientSocket.ReceiveTimeout = 0;
                    clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), OnConnectCallback, clientSocket);
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                    OnExeception();
                }
            });
        }

        private void OnConnectCallback(IAsyncResult ar)
        {
            SafeRun(delegate 
            {
                var client = ar.AsyncState as Socket;

                try
                {
                    Logger.Log("end connect");
                    client.EndConnect(ar);
                    messageReciver = LantisPoolSystem.GetPool<MessageReciver>().NewObject();
                    messageReciver.Start(client, OnReciveMessage, OnExeception);
                }
                catch(SocketException e)
                {
                    if (e.ErrorCode == 10061)
                    {
                        Logger.Error("can't find server online address");
                    }
                    else
                    {
                        Logger.Error(e.ToString());
                    }

                    OnExeception();
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
                if (isOpen)
                {
                    if (messageReciver != null)
                    {
                        LantisPoolSystem.GetPool<MessageReciver>().DisposeObject(messageReciver);
                        messageReciver = null;
                    }

                    if (exceptionCall != null)
                    {
                        exceptionCall();
                    }
                }
            });
        }

        public void SendMessage(byte[] datas, Socket remoteSocket)
        {
            SafeRun(delegate
            {
                var messageSender = LantisPoolSystem.GetPool<MessageSender>().NewObject();
                messageSender.SetSender(remoteSocket, datas);
                MessageSenderManager.AddSender(messageSender);
            });
        }
    }
}
