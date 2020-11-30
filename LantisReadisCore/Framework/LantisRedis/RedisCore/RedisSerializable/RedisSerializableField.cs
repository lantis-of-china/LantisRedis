using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LantisPool;

namespace LantisRedisCore
{
    [Serializable]
    public class RedisSerializableField : LantisPoolInterface
    {
        public string fieldName;
        public string fieldType;
        public object fieldValue;

        public void OnCreate()
        {
        }

        public void OnEnable()
        {
        }

        public void OnDisable()
        {
        }

        public void OnDestroy()
        {
        }
    }
}
