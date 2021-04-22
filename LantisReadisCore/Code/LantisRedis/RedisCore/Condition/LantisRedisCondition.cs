using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.Redis
{
    [Serializable]
    public class LantisRedisCondition : LantisPoolInterface
    {
        public string fieldName;
        public string operation;
        public string fieldValue;

        public void OnCreate()
        {
            fieldName = "";
            operation = "";
            fieldValue = "";
        }

        public void OnPoolSpawn()
        {
        }

        public void OnPoolDespawn()
        {
            fieldName = "";
            operation = "";
            fieldValue = "";
        }

        public void OnDestroy()
        {
            fieldName = "";
            operation = "";
            fieldValue = "";
        }
    }
}
