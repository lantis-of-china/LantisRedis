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
            try
            {
                var requestMsg = RedisSerializable.DeSerialize<RequestRedisSqlCommand>(data);
                var responseMsg = Pool.LantisPoolSystem.GetPool<ResponseRedisSqlCommand>().CreateObject();
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
                }
            }
            catch(Exception e)
            {
                Logger.Error($"request get exception!{e.ToString()}");
                return;
            }           
        }
    }
}
