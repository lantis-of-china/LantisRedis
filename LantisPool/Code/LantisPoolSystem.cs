using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lantis.Pool
{
    public class LantisPoolSystem
    {
        private static object lockObject = new object();
        private static Dictionary<Type, object> poolMap = new Dictionary<Type, object>();

        public static LantisPool<T> GetPool<T>() where T : LantisPoolInterface,new()
        {
            var type = typeof(T);
            LantisPool<T> lantisPool = null;

            lock (lockObject)
            {
                if (poolMap.ContainsKey(type))
                {
                    return poolMap[type] as LantisPool<T>;
                }

                lantisPool = new LantisPool<T>();
                poolMap.Add(type, lantisPool);
            }

            lantisPool.SetPoolParamar(10, 1);
            lantisPool.Init();

            return lantisPool as LantisPool<T>;
        }
    }
}
