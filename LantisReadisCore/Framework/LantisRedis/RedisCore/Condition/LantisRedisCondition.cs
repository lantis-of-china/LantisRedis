using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LantisRedisCore
{
    public class LantisRedisCondition : LantisPool.LantisPoolInterface
    {
        public string fieldName;
        public string operation;
        public string fieldValue;

        public void OnCreate()
        {
            fieldName = string.Empty;
            operation = string.Empty;
            fieldValue = string.Empty;
        }

        public void OnPoolSpawn()
        {
        }

        public void OnPoolDespawn()
        {
            fieldName = string.Empty;
            operation = string.Empty;
            fieldValue = string.Empty;
        }

        public void OnDestroy()
        {
            fieldName = null;
            operation = null;
            fieldValue = null;
        }
    }
}
