using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.Redis.Message
{
    [Serializable]
    public class RequestRedisSet : LantisPoolInterface
    {
        public int requestId;
        public string databaseName = "";
        public string tableName = "";
        public LantisRedisConditionGroup conditionGroup;
        public RedisSerializableData data;

        public void OnCreate()
        {
            databaseName = "";
            tableName = "";
            data = null;
        }

        public void OnPoolSpawn()
        {
            conditionGroup = LantisPoolSystem.GetPool<LantisRedisConditionGroup>().NewObject();
        }

        public void OnPoolDespawn()
        {
            databaseName = "";
            tableName = "";

            if (data != null)
            {
                LantisPoolSystem.GetPool<RedisSerializableData>().DisposeObject(data);
                data = null;
            }

            LantisPoolSystem.GetPool<LantisRedisConditionGroup>().DisposeObject(conditionGroup);
            conditionGroup = null;
        }

        public void OnDestroy()
        {
            databaseName = "";
            tableName = "";
            data = null;
            conditionGroup = null;
        }
    }
}
