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

        public void OnCreate()
        {
            lockObject = new object();
            redisFieldCollects = new LantisDictronaryList<string, RedisTableFieldDefine>();
            redisDataCollects = new LantisDictronaryList<string, RedisTableData>();
        }

        public void OnEnable()
        {
        }

        public void OnDisable()
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
