using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.Redis.Message
{
    [Serializable]
    public class RequestRedisCheckDatabase : LantisPoolInterface
    {
        public int requestId;
        public List<RedisCheckDatabase> redisTableFieldDefine;

        public RequestRedisCheckDatabase()
        {
            redisTableFieldDefine = new List<RedisCheckDatabase>();
        }

        public void OnPoolSpawn()
        {
        }

        public void OnPoolDespawn()
        {
            for (var i = 0; i < redisTableFieldDefine.Count; ++i)
            {
                LantisPoolSystem.GetPool<RedisCheckDatabase>().DisposeObject(redisTableFieldDefine[i]);
            }

            redisTableFieldDefine.Clear();
        }

        public void OnDestroy()
        {
            redisTableFieldDefine = null;
        }
    }
}
