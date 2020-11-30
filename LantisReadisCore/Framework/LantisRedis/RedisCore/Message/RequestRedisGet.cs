using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LantisRedisCore.Message
{
    public class RequestRedisGet : LantisPool.LantisPoolInterface
    {
        public string databaseName;
        public string tableName;
        public LantisRedisConditionGroup conditionGroup;

        public void OnCreate()
        {
            databaseName = string.Empty;
            tableName = string.Empty;
        }

        public void OnEnable()
        {
            conditionGroup = LantisPool.LantisPoolSystem.GetPool<LantisRedisConditionGroup>().NewObject();
        }

        public void OnDisable()
        {
            databaseName = string.Empty;
            tableName = string.Empty;
            LantisPool.LantisPoolSystem.GetPool<LantisRedisConditionGroup>().DisposeObject(conditionGroup);
            conditionGroup = null;
        }

        public void OnDestroy()
        {
            tableName = null;
            databaseName = null;
            conditionGroup = null;
        }
    }
}
