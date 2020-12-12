using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Locker;
using Lantis.Pool;

namespace Lantis.DatabaseLinks
{
    public class DatabaseLinkState : SafeLocker,LantisPoolInterface
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string connectString;
        /// <summary>
        /// 连接实例
        /// </summary>
        public DbConnection sqlConnectInstance;
        /// <summary>
        /// 使用中
        /// </summary>
        public bool isUsed;

        public void OnPoolSpawn()
        {
            SafeRun(delegate
            {
                connectString = string.Empty;
                sqlConnectInstance = null;
                isUsed = false;
            });
        }

        public void OnPoolDespawn()
        {
            SafeRun(delegate
            {
                connectString = string.Empty;
                sqlConnectInstance = null;
                isUsed = false;
            });
        }

        public void SetLink(string conStr)
        {
            connectString = conStr;
            sqlConnectInstance = SqlFactorWarp.CreateConnection(connectString);
            sqlConnectInstance.Open();
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            sqlConnectInstance.Dispose();
            sqlConnectInstance.Close();
        }
    }

}
