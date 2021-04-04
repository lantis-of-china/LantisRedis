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
    public class UpdateDataProcess : MessageProcess
    {
        public UpdateDataProcess()
        {
            ID = MessageIdDefine.UpdateRedis;
        }

        public static MessageProcess _Instance;

        public static MessageProcess GetProcessType()
        {
            if (_Instance == null)
            {
                _Instance = new UpdateDataProcess();
            }
            return _Instance;
        }

        public override void Process(byte[] data,Socket socket, string ip, int port)
        {
            ResponseRedisUpdate responseMsg = null;

            try
            {
                var requestMsg = RedisSerializable.DeSerialize<RequestRedisUpdate>(data);
                Program.RedisMemoryBranch.UpdateMemory(requestMsg);
                responseMsg = LantisPoolSystem.GetPool<ResponseRedisUpdate>().NewObject();
                responseMsg.requestId = requestMsg.requestId;
                responseMsg.result = 1;
                Program.NetBranch.NetServerComponent.SendMessage(MessageIdDefine.UpdateRedisBack, RedisSerializable.Serialize(responseMsg), socket);
            }
            catch (Exception e)
            {
                Logger.Error($"request get exception!{e.ToString()}");
            }
            finally 
            {
                if (responseMsg != null)
                {
                    LantisPoolSystem.GetPool<ResponseRedisUpdate>().DisposeObject(responseMsg);
                }
            }
        }
    }
}
