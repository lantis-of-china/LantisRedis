﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LantisRedisCore.Message
{
    public class RequestRedisGetPage : LantisPool.LantisPoolInterface
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

        public void OnEnable()
        {
            conditionGroup = LantisPool.LantisPoolSystem.GetPool<LantisRedisConditionGroup>().NewObject();
        }

        public void OnDisable()
        {
            tableName = string.Empty;
            everPageCount = 0;
            page = 0;
            LantisPool.LantisPoolSystem.GetPool<LantisRedisConditionGroup>().DisposeObject(conditionGroup);
            conditionGroup = null;
        }

        public void OnDestroy()
        {
            tableName = null;
            conditionGroup = null;
        }
    }
}
