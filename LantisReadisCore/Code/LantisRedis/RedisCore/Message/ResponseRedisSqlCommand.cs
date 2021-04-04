using Lantis.Pool;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lantis.Redis.Message
{
	[Serializable]
	public class ResponseRedisSqlCommand : LantisPoolInterface
	{
		public int requestId;
		public byte result;
		public int count;
		public RedisSerializableData redisSerializableData;

		void LantisPoolInterface.OnPoolDespawn()
		{
			requestId = 0;
			result = 0;
			count = 0;

			if (redisSerializableData != null)
			{
				LantisPoolSystem.GetPool<RedisSerializableData>().DisposeObject(redisSerializableData);
				redisSerializableData = null;
			}
		}

		void LantisPoolInterface.OnPoolSpawn()
		{
			redisSerializableData = LantisPoolSystem.GetPool<RedisSerializableData>().NewObject();
		}
	}
}
