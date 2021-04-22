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
                Logger.Log($"getdata database:{requestMsg.databaseName} tablename:{requestMsg.tableName}");
                var redisTableData = Program.RedisMemoryBranch.GetMemory(requestMsg);
                reponseMsg = LantisPoolSystem.GetPool<ResponseRedisGet>().NewObject();
                reponseMsg.result = 0;

                if (redisTableData != null)
                {
                    var serializableData = RedisCore.RedisTableDataToRedisSerializableData(redisTableData);
                    reponseMsg.redisSerializableData = serializableData;
                    reponseMsg.result = 1;
                }

                reponseMsg.requestId = requestMsg.requestId;
                Program.NetBranch.NetServerComponent.SendMessage(MessageIdDefine.GetDataBack, RedisSerializable.Serialize(reponseMsg),socket);
            }
            catch(Exception e)
            {
                Logger.Error("request get exception!");
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
