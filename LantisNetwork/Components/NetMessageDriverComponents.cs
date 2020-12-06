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
using System.Reflection;

namespace Lantis.Network
{
    public class NetMessageDriverComponents : ComponentEntity
    {
        private const int typeSize = 4;
        private Timer threadTimer;
        private LantisQueue<MessageContent> msgQueue;
        private LantisDictronaryList<int, Action<byte[], Socket, string, int>> netProcessMap;
        private LantisList<string> processNameSpaceList;

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
                processNameSpaceList = LantisPoolSystem.GetPool<LantisList<string>>().NewObject();
            });
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();

            SafeRun(delegate
            {
                msgQueue.DequeueAll(delegate (MessageContent messageContent)
                {
                    LantisPoolSystem.GetPool<MessageContent>().DisposeObject(messageContent);
                });

                LantisPoolSystem.GetPool<LantisQueue<MessageContent>>().DisposeObject(msgQueue);
                msgQueue = null;

                LantisPoolSystem.GetPool<LantisDictronaryList<int, Action<byte[], Socket, string, int>>>().DisposeObject(netProcessMap);
                netProcessMap = null;

                LantisPoolSystem.GetPool<LantisList<string>>().DisposeObject(processNameSpaceList);
                processNameSpaceList = null;
            });
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);

            SafeRun(delegate
            {
                for (var i = 0; i < paramsData.Length; ++i)
                {
                    processNameSpaceList.AddValue(paramsData[i] as string);
                }

                threadTimer.Interval = 10.0f;
                threadTimer.Elapsed += OnDriverMessage;
                threadTimer.Start();

                CollectAllMessageProcess();
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

        public byte[] OnGetSenderMessage(int msgId, byte[] datas)
        {
            var idData = System.BitConverter.GetBytes((int)msgId);
            var lenght = idData.Length + datas.Length;
            byte[] sendBuffer = null;

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (System.IO.BinaryWriter bs = new System.IO.BinaryWriter(ms))
                {
                    bs.Write(lenght);
                    bs.Write(idData);
                    bs.Write(datas);
                }

                ms.Flush();
                sendBuffer = ms.ToArray();
            }

            return sendBuffer;
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
                    var typeBuf = new byte[typeSize];
                    var msgDate = new byte[messageContent.data.Length - typeSize];
                    System.Array.Copy(messageContent.data, typeBuf, typeSize);
                    var msgType = System.BitConverter.ToInt32(typeBuf, 0);
                    System.Array.ConstrainedCopy(messageContent.data, typeSize, msgDate, 0, msgDate.Length);

                    if (netProcessMap.HasKey(msgType))
                    {
                        netProcessMap[msgType](msgDate, messageContent.socket, messageContent.ip, messageContent.port);
                    }

                    LantisPoolSystem.GetPool<MessageContent>().DisposeObject(messageContent);
                });
            });
        }

        public void CollectAllMessageProcess()
        {
            SafeRun(delegate
            {
                var assembly = Assembly.GetExecutingAssembly();
                var assemblyTypes = assembly.GetTypes();

                for (int indexType = 0; indexType < assemblyTypes.Length; indexType++)
                {
                    if (processNameSpaceList.HasValue(assemblyTypes[indexType].Namespace) && !assemblyTypes[indexType].IsAbstract)
                    {
                        var fieldArray = assemblyTypes[indexType].GetFields();
                        var methodInfo = assemblyTypes[indexType].GetMethod("GetProcessType");

                        for (int fieldIndex = 0; fieldIndex < fieldArray.Length; fieldIndex++)
                        {
                            if (fieldArray[fieldIndex].Name == "ID")
                            {
                                for (int insIndex = 0; insIndex < fieldArray.Length; insIndex++)
                                {
                                    if (fieldArray[insIndex].Name == "_Instance")
                                    {
                                        var msgProcess = methodInfo.Invoke(null, null) as MessageProcess;
                                        var instance = fieldArray[insIndex].GetValue(null);
                                        var msgId = (int)fieldArray[fieldIndex].GetValue(instance);
                                        netProcessMap.AddValue(msgId, msgProcess.Process);
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}
