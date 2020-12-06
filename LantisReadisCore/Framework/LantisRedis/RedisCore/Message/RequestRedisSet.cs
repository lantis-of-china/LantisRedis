﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.Redis.Message
{
    public class RequestRedisSet : LantisPoolInterface
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
            conditionGroup = LantisPoolSystem.GetPool<LantisRedisConditionGroup>().NewObject();
        }

        public void OnPoolDespawn()
        {
            databaseName = string.Empty;
            tableName = string.Empty;
            data = null;
            LantisPoolSystem.GetPool<LantisRedisConditionGroup>().DisposeObject(conditionGroup);
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
