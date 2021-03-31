using Lantis.Pool;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lantis.Redis.Message
{
	[Serializable]
	public class ResponseRedisGet : LantisPoolInterface
	{
		public int requestId;
		public byte result;
		public RedisSerializableData redisSerializableData;

		void LantisPoolInterface.OnPoolDespawn()
		{
			requestId = 0;
			result = 0;

			if (redisSerializableData != null)
			{
				LantisPoolSystem.GetPool<RedisSerializableData>().DisposeObject(redisSerializableData);
				redisSerializableData = null;
			}
		}

		void LantisPoolInterface.OnPoolSpawn()
		{
			redisSerializableData = LantisPoolSystem.GetPool<RedisSerializableData>().CreateObject();
		}
	}
}
