using Lantis.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Lantis.Redis;
using Lantis.Redis.Message;

namespace Lantis.RedisExecute.NetProcess
{
    public class ExecuteCommandProcess : MessageProcess
    {
        public ExecuteCommandProcess()
        {
            ID = MessageIdDefine.ExecuteCommand;
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
            ResponseRedisSqlCommand responseMsg = null;

            try
            {
                var requestMsg = RedisSerializable.DeSerialize<RequestRedisSqlCommand>(data);
                responseMsg = Pool.LantisPoolSystem.GetPool<ResponseRedisSqlCommand>().NewObject();
                responseMsg.requestId = requestMsg.requestId;
                responseMsg.result = 1;
                responseMsg.count = 0;

                if (requestMsg.executeType == 0)
                {
                    var count = Program.DatabaseBranch.DatabaseCoreComponent.ExecuteNonQuery(requestMsg.sqlCmd, requestMsg.dbParameterList);
                    responseMsg.count = count;
                }
                else if (requestMsg.executeType == 1)
                {
                    var dataTable = Program.DatabaseBranch.DatabaseCoreComponent.ExecuteDataTable(requestMsg.sqlCmd, requestMsg.dbParameterList);
                    var redisTable = Program.RedisMemoryBranch.GetTable(requestMsg.databaseName, requestMsg.tableName);

                    if (redisTable != null)
                    {
                        var redisTableData = RedisCore.DataTableToRedisTableData(redisTable.GetRedisFieldCollects(), dataTable);
                        var redisSerializableData = RedisCore.RedisTableDataToRedisSerializableData(redisTableData);
                        responseMsg.redisSerializableData = redisSerializableData;
                    }
                    else
                    {
                        responseMsg.result = 0;
                    }
                }

                Program.NetBranch.NetServerComponent.SendMessage(MessageIdDefine.ExecuteCommandBack, RedisSerializable.Serialize(responseMsg), socket);
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
                    Pool.LantisPoolSystem.GetPool<ResponseRedisSqlCommand>().DisposeObject(responseMsg);
                }
            }
        }
    }
}
