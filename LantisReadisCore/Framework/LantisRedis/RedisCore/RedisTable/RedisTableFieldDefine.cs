using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.RedisCore
{
    public class RedisTableFieldDefine : LantisPoolInterface
    {
        public string fieldName;
        public string fieldType;

        public void OnPoolSpawn()
        {
            fieldName = string.Empty;
            fieldType = string.Empty;
        }

        public void OnPoolDespawn()
        {
            fieldName = string.Empty;
            fieldType = string.Empty;
        }
    }
}
