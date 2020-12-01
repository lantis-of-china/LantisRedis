using LantisExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LantisPool;

namespace LantisRedisCore
{
    public class RedisTable : LantisPool.LantisPoolInterface
    {
        private object lockObject;
        private string tableName;
        private LantisDictronaryList<string, RedisTableFieldDefine> redisFieldCollects;
        private LantisDictronaryList<string, RedisTableData> redisDataCollects;

        public RedisTable()
        {
            lockObject = new object();            
        }

        public void OnPoolSpawn()
        {
            redisFieldCollects = LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableFieldDefine>>().NewObject();
            redisDataCollects = LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableData>>().NewObject();
        }

        public void OnPoolDespawn()
        {
            lock (lockObject)
            {
                var valueList = redisDataCollects.ValueToList();
                var poolHandle = LantisPoolSystem.GetPool<RedisTableData>();

                for (var i = 0; i < valueList.Count; ++i)
                {
                    var valueItem = valueList[i];
                    poolHandle.DisposeObject(valueItem);
                }
            }

            lock (lockObject)
            {
                var valueList = redisFieldCollects.ValueToList();
                var poolHandle = LantisPoolSystem.GetPool<RedisTableFieldDefine>();

                for (var i = 0; i < valueList.Count; ++i)
                {
                    var valueItem = valueList[i];
                    poolHandle.DisposeObject(valueItem);
                }
            }

            redisDataCollects.Clear();
            redisFieldCollects.Clear();
            LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableFieldDefine>>().DisposeObject(redisFieldCollects);
            LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableData>>().DisposeObject(redisDataCollects);
            redisDataCollects = null;
            redisFieldCollects = null;
        }

        public void OnDestroy()
        {
            redisDataCollects = null;
            redisFieldCollects = null;
            lockObject = null;
        }

        public RedisTableData GetDataById(string id)
        {
            lock (lockObject)
            {
                if (redisDataCollects.HasKey(id))
                {
                    return redisDataCollects[id];
                }
            }

            return null;
        }

        public void AddDataById(string id, RedisTableData data)
        {
            lock (lockObject)
            {
                if (!redisDataCollects.HasKey(id))
                {
                    redisDataCollects.AddValue(id, data);

                    return;
                }
            }

            Logger.Error($"the data is has in table:{tableName} by id:{id}");
        }
    }
}
