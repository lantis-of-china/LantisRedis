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
            try
            {
                var requestMsg = RedisSerializable.DeSerialize<RequestRedisSet>(data);
                var serializableData = RedisSerializable.BytesToSerializable(requestMsg.data);
            }
            catch
            {
                Logger.Error("request get exception!");
                return;
            }           
        }
    }
}
