using Lantis.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Lantis.Redis;
using Lantis.Redis.Message;
using Lantis.Pool;

namespace Lantis.RedisExecute.NetProcess
{
    public class SetDataProcess : MessageProcess
    {
        public SetDataProcess()
        {
            ID = MessageIdDefine.SetData;
        }

        public static MessageProcess _Instance;

        public static MessageProcess GetProcessType()
        {
            if (_Instance == null)
            {
                _Instance = new SetDataProcess();
            }
            return _Instance;
        }

        public override void Process(byte[] data,Socket socket, string ip, int port)
        {
            ResponseRedisSet responseMsg = null;

            try
            {
                var requestMsg = RedisSerializable.DeSerialize<RequestRedisSet>(data);
                Program.RedisMemoryBranch.SetMemory(requestMsg);
                responseMsg = LantisPoolSystem.GetPool<ResponseRedisSet>().CreateObject();
                responseMsg.requestId = requestMsg.requestId;
                responseMsg.result = 1;
                Program.NetBranch.NetServerComponent.SendMessage(MessageIdDefine.SetDataBack, RedisSerializable.SerializableToBytes(responseMsg), socket);
            }
            catch (Exception e)
            {
                Logger.Error($"request get exception!{e.ToString()}");
                return;
            }
            finally 
            {
                if (responseMsg != null)
                {
                    LantisPoolSystem.GetPool<ResponseRedisSet>().DisposeObject(responseMsg);
                }
            }
        }
    }
}
