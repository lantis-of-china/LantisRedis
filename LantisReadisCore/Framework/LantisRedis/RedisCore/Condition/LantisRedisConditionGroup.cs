using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.RedisCore
{
    public class LantisRedisConditionGroup : LantisPoolInterface
    {
        public List<LantisRedisCondition> conditionList;

        public void OnCreate()
        {
            conditionList = new List<LantisRedisCondition>();
        }

        public void OnPoolSpawn()
        {            
        }

        public void OnPoolDespawn()
        {
            for (var i = 0; i < conditionList.Count; ++i)
            {
                LantisPoolSystem.GetPool<LantisRedisCondition>().DisposeObject(conditionList[i]);
            }

            conditionList.Clear();
        }

        public void OnDestroy()
        {
            conditionList = null;
        }
    }
}
