using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.Redis.Message
{
    [Serializable]
    public class RedisCheckDatabase : LantisPoolInterface
    {
        public string databaseName;
        public string tableName;
        public List<RedisTableFieldDefine> tableInfos;

        public void OnCreate()
        {
            databaseName = string.Empty;
            tableName = string.Empty;
            tableInfos = new List<RedisTableFieldDefine>();
        }

        public void OnPoolSpawn()
        {
        }

        public void OnPoolDespawn()
        {
            databaseName = string.Empty;
            tableName = string.Empty;

            for (var i = 0; i < tableInfos.Count; ++i)
            {
                LantisPoolSystem.GetPool<RedisTableFieldDefine>().DisposeObject(tableInfos[i]);
            }

            tableInfos.Clear();
        }

        public void OnDestroy()
        {
            tableName = null;
            databaseName = null;
            tableInfos = null;
        }
    }
}
