using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.Redis.Message
{
    [Serializable]
    public class RequestRedisGetPage : LantisPoolInterface
    {
        public string tableName;
        public LantisRedisConditionGroup conditionGroup;
        public int everPageCount;
        public int page;

        public void OnCreate()
        {
            tableName = string.Empty;
            everPageCount = 0;
            page = 0;
        }

        public void OnPoolSpawn()
        {
            conditionGroup = LantisPoolSystem.GetPool<LantisRedisConditionGroup>().NewObject();
        }

        public void OnPoolDespawn()
        {
            tableName = string.Empty;
            everPageCount = 0;
            page = 0;
            LantisPoolSystem.GetPool<LantisRedisConditionGroup>().DisposeObject(conditionGroup);
            conditionGroup = null;
        }

        public void OnDestroy()
        {
            tableName = null;
            conditionGroup = null;
        }
    }
}
