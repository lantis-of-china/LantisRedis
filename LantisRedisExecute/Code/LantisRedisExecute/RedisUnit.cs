using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;
using Lantis.Extend;
using Lantis.Redis;
using Lantis.Locker;
namespace Lantis.RedisExecute
{
    public class RedisUnit :SafeLocker ,LantisPoolInterface
    {
        public string databaseName;
        private static LantisDictronaryList<string, RedisTable> tableCollects;

        public void OnCreate()
        {
        }

        public void OnPoolSpawn()
        {
            SafeRun(delegate
            {
                tableCollects = LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTable>>().NewObject();
            });
        }

        public void OnPoolDespawn()
        {
            SafeRun(delegate
            {
                var poolHandle = LantisPoolSystem.GetPool<RedisTable>();

                tableCollects.SafeWhile(delegate (string tableName, RedisTable table)
                {
                    poolHandle.DisposeObject(table);
                });

                tableCollects.Clear();
                tableCollects = null;
            });
        }

        public void OnDestroy()
        {
            SafeRun(delegate
            {
                tableCollects = null;
            });
        }

        public RedisTable GetRedisTable(string tableName)
        {
            return SafeRunFunction(delegate()
            {
                if (tableCollects.HasKey(tableName))
                {
                    return tableCollects[tableName];
                }

                return null;
            });
        }

        public void AddRedisTable(RedisTable redistable)
        {
            SafeRun(delegate
            {
                tableCollects.AddValue(redistable.tableName, redistable);
            });
        }
    }
}
