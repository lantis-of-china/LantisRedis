using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lantis.ReadisOperation;
using System.Net.Sockets;
using Lantis.Redis;
using Lantis.Network;

namespace WordProcess
{
    //处理方法类 必须继承ProcessBase
    /// <summary>
    /// 玩家注册账号
    /// </summary>
    public class CheckDatabaseProcess : MessageProcess
    {
        public CheckDatabaseProcess()
        {
            ID = (int)MessageIdDefine.CheckDatabase;
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

        public override void Process(byte[] DateBuf, Socket NetSocket, string ip, int port)
        {
            try
            {
                //var requestMsg = RedisSerializable.DeSerialize<RequestRedisGet>(data);
            }
            catch
            {
                Logger.Error("request get exception!");
                return;
            }
        }
    }
}
