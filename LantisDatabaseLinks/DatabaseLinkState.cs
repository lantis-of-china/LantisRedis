using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lantis.DatabaseLinks
{
    public class DatabaseLinkState
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string connectStr;
        /// <summary>
        /// 连接实例
        /// </summary>
        public DbConnection sqlConnectInstance;
        /// <summary>
        /// 使用中
        /// </summary>
        public bool isUsed;

        public DatabaseLinkState(string conStr)
        {
            connectStr = conStr;
            sqlConnectInstance = SqlFactorWarp.CreateConnection(connectStr);
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
