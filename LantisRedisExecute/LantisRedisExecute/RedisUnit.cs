﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;
using Lantis.Extend;
using Lantis.Redis;

namespace Lantis.RedisExecute
{
    public class RedisUnit : LantisPoolInterface
    {
        private object lockObject;
        private string databaseName;
        private static LantisDictronaryList<string, RedisTable> tableCollects;

        public void OnCreate()
        {
            lockObject = new object();
            tableCollects = new LantisDictronaryList<string, RedisTable>();
        }

        public void OnPoolSpawn()
        {
        }

        public void OnPoolDespawn()
        {
            tableCollects.Clear();
        }

        public void OnDestroy()
        {
            tableCollects = null;
            lockObject = null;
        }

        public RedisTable GetRedisTable(string tableName)
        {
            lock (lockObject)
            {
                if (tableCollects.HasKey(tableName))
                {
                    return tableCollects[tableName];
                }
            }

            return null;
        }
    }
}
