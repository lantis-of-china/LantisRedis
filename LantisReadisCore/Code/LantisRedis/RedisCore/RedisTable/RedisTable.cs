using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;
using Lantis.Extend;
using Lantis.Locker;

namespace Lantis.Redis
{
    public class RedisTable : SafeLocker,LantisPoolInterface
    {
        private string tableName;
        private LantisDictronaryList<string, RedisTableFieldDefine> redisFieldCollects;
        private LantisDictronaryList<string, RedisTableData> redisDataCollects;

        public RedisTable()
        {
        }

        public void OnPoolSpawn()
        {
            redisFieldCollects = LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableFieldDefine>>().NewObject();
            redisDataCollects = LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableData>>().NewObject();
        }

        public void OnPoolDespawn()
        {
            SafeRun(delegate
            {
                var valueList = redisDataCollects.ValueToList();
                var poolHandle = LantisPoolSystem.GetPool<RedisTableData>();

                for (var i = 0; i < valueList.Count; ++i)
                {
                    var valueItem = valueList[i];
                    poolHandle.DisposeObject(valueItem);
                }

                var valueList2 = redisFieldCollects.ValueToList();
                var poolHandle2 = LantisPoolSystem.GetPool<RedisTableFieldDefine>();

                for (var i = 0; i < valueList2.Count; ++i)
                {
                    var valueItem = valueList2[i];
                    poolHandle2.DisposeObject(valueItem);
                }

                redisDataCollects.Clear();
                redisFieldCollects.Clear();
                LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableFieldDefine>>().DisposeObject(redisFieldCollects);
                LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableData>>().DisposeObject(redisDataCollects);
                redisDataCollects = null;
                redisFieldCollects = null;
            });
        }

        public void OnDestroy()
        {
            SafeRun(delegate 
            {
                redisDataCollects = null;
                redisFieldCollects = null;
            });
        }

        public RedisTableData GetDataById(string id)
        {
            return SafeRunFunction(new Func<RedisTableData>(delegate
            {
                if (redisDataCollects.HasKey(id))
                {
                    return redisDataCollects[id];
                }

                return null;
            }));

        }

        public void AddDataById(string id, RedisTableData data)
        {
            SafeRun(delegate 
            {
                if (!redisDataCollects.HasKey(id))
                {
                    redisDataCollects.AddValue(id, data);

                    return;
                }

                Logger.Error($"the data is has in table:{tableName} by id:{id}");
            });
        }
    }
}
