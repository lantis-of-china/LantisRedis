using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LantisRedisCore.Message
{
    public class RequestRedisSet : LantisPool.LantisPoolInterface
    {
        public string databaseName;
        public string tableName;
        public LantisRedisConditionGroup conditionGroup;
        public byte[] data;

        public void OnCreate()
        {
            databaseName = string.Empty;
            tableName = string.Empty;
            data = null;
        }

        public void OnPoolSpawn()
        {
            conditionGroup = LantisPool.LantisPoolSystem.GetPool<LantisRedisConditionGroup>().NewObject();
        }

        public void OnPoolDespawn()
        {
            databaseName = string.Empty;
            tableName = string.Empty;
            data = null;
            LantisPool.LantisPoolSystem.GetPool<LantisRedisConditionGroup>().DisposeObject(conditionGroup);
            conditionGroup = null;
        }

        public void OnDestroy()
        {
            databaseName = null;
            tableName = null;
            data = null;
            conditionGroup = null;
        }
    }
}
