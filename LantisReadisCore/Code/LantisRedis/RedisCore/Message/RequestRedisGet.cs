﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.Redis.Message
{
    [Serializable]
    public class RequestRedisGet : LantisPoolInterface
    {
        public int requestId;
        public string databaseName;
        public string tableName;
        public LantisRedisConditionGroup conditionGroup;

        public void OnCreate()
        {
            databaseName = string.Empty;
            tableName = string.Empty;
        }

        public void OnPoolSpawn()
        {
            conditionGroup = LantisPoolSystem.GetPool<LantisRedisConditionGroup>().NewObject();
        }

        public void OnPoolDespawn()
        {
            databaseName = string.Empty;
            tableName = string.Empty;
            LantisPoolSystem.GetPool<LantisRedisConditionGroup>().DisposeObject(conditionGroup);
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
