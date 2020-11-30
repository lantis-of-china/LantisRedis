using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LantisRedisCore
{
    public class LantisRedisConditionGroup : LantisPool.LantisPoolInterface
    {
        public List<LantisRedisCondition> conditionList;

        public void OnCreate()
        {
            conditionList = new List<LantisRedisCondition>();
        }

        public void OnEnable()
        {            
        }

        public void OnDisable()
        {
            for (var i = 0; i < conditionList.Count; ++i)
            {
                LantisPool.LantisPoolSystem.GetPool<LantisRedisCondition>().DisposeObject(conditionList[i]);
            }

            conditionList.Clear();
        }

        public void OnDestroy()
        {
            conditionList = null;
        }
    }
}
