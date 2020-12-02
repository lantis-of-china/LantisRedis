using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.RedisCore
{
    [Serializable]
    public class RedisSerializableField : LantisPoolInterface
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
