using Lantis.Pool;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lantis.Redis.Message
{
	[Serializable]
	public class ResponseRedisCheck : LantisPoolInterface
	{
		public int requestId;
		public byte result;

		void LantisPoolInterface.OnPoolDespawn()
		{
			requestId = 0;
			result = 0;
		}

		void LantisPoolInterface.OnPoolSpawn()
		{
		}
	}
}
