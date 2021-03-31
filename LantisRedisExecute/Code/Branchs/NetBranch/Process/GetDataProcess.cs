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
    public class GetDataProcess : MessageProcess
    {
        public GetDataProcess()
        {
            ID = MessageIdDefine.GetData;
        }

        public static MessageProcess _Instance;

        public static MessageProcess GetProcessType()
        {
            if (_Instance == null)
            {
                _Instance = new GetDataProcess();
            }
            return _Instance;
        }

        public override void Process(byte[] data,Socket socket, string ip, int port)
        {
            ResponseRedisGet reponseMsg = null;

            try
            {
                var requestMsg = RedisSerializable.DeSerialize<RequestRedisGet>(data);
                var redisTableData = Program.RedisMemoryBranch.GetMemory(requestMsg);
                var serializableData = RedisCore.RedisTableDataToRedisSerializableData(redisTableData);
                reponseMsg = LantisPoolSystem.GetPool<ResponseRedisGet>().CreateObject();
                reponseMsg.requestId = requestMsg.requestId;
                reponseMsg.result = 1;
                Program.NetBranch.NetServerComponent.SendMessage(MessageIdDefine.GetDataBack, RedisSerializable.SerializableToBytes(reponseMsg),socket);
            }
            catch
            {
                Logger.Error("request get exception!");
                return;
            }
            finally 
            {
                if (reponseMsg != null)
                {
                    LantisPoolSystem.GetPool<ResponseRedisGet>().DisposeObject(reponseMsg);
                }
            }
        }
    }
}
