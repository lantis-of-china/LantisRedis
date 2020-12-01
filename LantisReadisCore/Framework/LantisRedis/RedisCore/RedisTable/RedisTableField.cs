using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LantisPool;

namespace LantisRedisCore
{
    [Serializable]
    public class RedisTableField : LantisPoolInterface
    {
        public string fieldName;
        public string fieldType;
        public object fieldValue;

        public void OnPoolSpawn()
        {
            fieldName = string.Empty;
            fieldType = string.Empty;
            fieldValue = null;
        }

        public void OnPoolDespawn()
        {
            fieldName = string.Empty;
            fieldType = string.Empty;
            fieldValue = null;
        }
    }
}
