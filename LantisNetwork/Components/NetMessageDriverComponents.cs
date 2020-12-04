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
using System.Timers;

namespace Lantis.Network
{
    public class NetMessageDriverComponents : ComponentEntity
    {
        private Timer threadTimer;
        private LantisQueue<MessageContent> msgQueue;
        private LantisDictronaryList<int, Action<byte[], Socket, string, int>> netProcessMap;

        public NetMessageDriverComponents()
        {
            threadTimer = new Timer();
        }

        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();

            SafeRun(delegate
            {
                msgQueue = LantisPoolSystem.GetPool<LantisQueue<MessageContent>>().NewObject();
                netProcessMap = LantisPoolSystem.GetPool<LantisDictronaryList<int, Action<byte[], Socket, string, int>>>().NewObject();
            });
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();

            SafeRun(delegate
            {
                msgQueue.DequeueAll(delegate(MessageContent messageContent)
                {
                    LantisPoolSystem.GetPool<MessageContent>().DisposeObject(messageContent);
                });

                LantisPoolSystem.GetPool<LantisQueue<MessageContent>>().DisposeObject(msgQueue);
                msgQueue = null;

                LantisPoolSystem.GetPool<LantisDictronaryList<int, Action<byte[], Socket, string, int>>>().DisposeObject(netProcessMap);
                netProcessMap = null;
            });
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);
            
            SafeRun(delegate
            {
                threadTimer.Interval = 10.0f;
                threadTimer.Elapsed += OnDriverMessage;
                threadTimer.Start();
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

            SafeRun(delegate
            {
                threadTimer.Elapsed -= OnDriverMessage;
                threadTimer.Stop();
            });
        }

        public void OnReciveMessage(byte[] datas, Socket socket, string ip, int port)
        {
            SafeRun(delegate
            {
                var messageContent = LantisPoolSystem.GetPool<MessageContent>().NewObject();
                messageContent.data = datas;
                messageContent.socket = socket;
                messageContent.ip = ip;
                messageContent.port = port;
                msgQueue.Enqueue(messageContent);
            });
        }

        public void OnDriverMessage(object sender, ElapsedEventArgs e)
        {
            SafeRun(delegate
            {
                msgQueue.DequeueAll(delegate (MessageContent messageContent)
                {                   
                    LantisPoolSystem.GetPool<MessageContent>().DisposeObject(messageContent);
                });
            });
        }
    }
}
