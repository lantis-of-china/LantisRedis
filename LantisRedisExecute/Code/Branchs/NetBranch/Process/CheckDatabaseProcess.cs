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
    public class CheckDatabaseProcess : MessageProcess
    {
        public CheckDatabaseProcess()
        {
            ID = MessageIdDefine.CheckDatabase;
        }

        public static MessageProcess _Instance;

        public static MessageProcess GetProcessType()
        {
            if (_Instance == null)
            {
                _Instance = new CheckDatabaseProcess();
            }
            return _Instance;
        }

        public override void Process(byte[] data,Socket socket, string ip, int port)
        {
            ResponseRedisCheckDatabase responseMsg = null;

            try
            {
                var requestMsg = RedisSerializable.DeSerialize<RequestRedisCheckDatabase>(data);
                Program.RedisMemoryBranch.CheckMemory(requestMsg);
                responseMsg = LantisPoolSystem.GetPool<ResponseRedisCheckDatabase>().NewObject();
                responseMsg.requestId = requestMsg.requestId;
                responseMsg.result = 1;
                Program.NetBranch.NetServerComponent.SendMessage(MessageIdDefine.CheckDatabaseBack, RedisSerializable.Serialize(responseMsg), socket);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return;
            }
            finally 
            {
                if (responseMsg != null)
                {
                    LantisPoolSystem.GetPool<ResponseRedisCheckDatabase>().DisposeObject(responseMsg);
                }
            }
        }
    }
}
