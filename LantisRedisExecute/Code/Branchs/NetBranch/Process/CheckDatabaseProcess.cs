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
            try
            {
                var requestMsg = RedisSerializable.DeSerialize<RequestRedisCheckDatabase>(data);
                Program.RedisMemoryBranch.CheckMemory(requestMsg);
            }
            catch(Exception e)
            {
                Logger.Error(e.ToString());
                return;
            }           
        }
    }
}
