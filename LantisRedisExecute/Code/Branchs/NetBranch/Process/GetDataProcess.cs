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
            try
            {
                var requestMsg = RedisSerializable.DeSerialize<RequestRedisGet>(data);
            }
            catch
            {
                Logger.Error("request get exception!");
                return;
            }           
        }
    }
}
