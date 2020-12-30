﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.Redis.Message
{
    [Serializable]
    public class RequestRedisSqlCommand : LantisPoolInterface
    {
        public int requestId;
        public string sqlCmd;
        public byte executeType;

        public void OnCreate()
        {
            requestId = 0;
            executeType = 0;
            sqlCmd = string.Empty;
        }

        public void OnPoolSpawn()
        {
        }

        public void OnPoolDespawn()
        {
            sqlCmd = string.Empty;
            requestId = 0;
        }

        public void OnDestroy()
        {
            sqlCmd = string.Empty;
            requestId = 0;
        }
    }
}
