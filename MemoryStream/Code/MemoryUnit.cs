using System;
using System.Collections.Generic;
using System.Text;
using Lantis.Pool;
using Lantis.EntityComponentSystem;

namespace Lantis.IO
{
	public class MemoryUnit : Entity,LantisPoolInterface
	{
		public byte[] datas;

		public MemoryUnit()
		{
			datas = new byte[128];
		}

		public override void OnPoolSpawn()
		{
			base.OnPoolSpawn();
		}

		public override void OnPoolDespawn()
		{
			base.OnPoolDespawn();
		}

		public override void OnAwake(params object[] paramsData)
		{
			base.OnAwake(paramsData);
		}

		public override void OnEnable()
		{
			base.OnEnable();
		}

		public override void OnDisable()
		{
			base.OnDisable();
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
		}

		public byte ReadByte(int offset)
		{
			return SafeRunFunction(delegate
			{
				return datas[offset];
			});
		}

		public void WriteByte(int offset, byte data)
		{
			SafeRun(delegate
			{
				datas[offset] = data;
			});
		}

		public void WriteByte(int offset, byte[] data,int lenght)
		{
			SafeRun(delegate
			{
				Array.Copy(data, 0, datas, offset, lenght);
			});
		}
	}
}
