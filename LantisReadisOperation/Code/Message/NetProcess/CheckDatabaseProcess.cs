using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lantis.ReadisOperation;
using System.Net.Sockets;
using Lantis.Redis;
using Lantis.Network;
using Lantis.Redis.Message;

namespace Lantis.ReadisOperation.NetProcess
{
    /// <summary>
    /// 玩家注册账号
    /// </summary>
    public class CheckDatabaseProcess : MessageProcess
    {
        public CheckDatabaseProcess()
        {
            ID = (int)MessageIdDefine.CheckDatabaseBack;
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

        public override void Process(byte[] data, Socket NetSocket, string ip, int port)
        {
            try
            {
                var requestMsg = RedisSerializable.DeSerialize<ResponseRedisCheckDatabase>(data);
                NetCallbackSystem.CallAndRemoveById(requestMsg.requestId,requestMsg);
            }
            catch
            {
                Logger.Error("request get exception!");
                return;
            }
        }
    }
}
