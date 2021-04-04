using Lantis.Pool;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lantis.Redis
{
	public class RedisBase : LantisPoolInterface
	{
		public virtual void OnPoolDespawn()
		{
		}

		public virtual void OnPoolSpawn()
		{
		}
	}
}
