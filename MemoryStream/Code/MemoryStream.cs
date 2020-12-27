using Lantis.Pool;
using Lantis.EntityComponentSystem;
using Lantis.Extend;
using System;

namespace Lantis.IO
{
	public class MemoryStream : Entity,LantisPoolInterface
	{
		private const int unitSize = 128;
		private int position;
		private int allSize;
		private LantisList<MemoryUnit> unitList;

		public override void OnPoolSpawn()
		{
			base.OnPoolSpawn();

			SafeRun(delegate
			{
				unitList = LantisPoolSystem.GetPool<LantisList<MemoryUnit>>().NewObject();
			});
		}

		public override void OnPoolDespawn()
		{
			base.OnPoolDespawn();

			SafeRun(delegate
			{
				if (unitList != null)
				{
					Clear();
					LantisPoolSystem.GetPool<LantisList<MemoryUnit>>().DisposeObject(unitList);
					unitList = null;
				}
			});
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

		public void Clear()
		{
			SafeRun(delegate
			{
				position = -1;

				unitList.SafeWhile(delegate (MemoryUnit unit)
				{
					EntitySystem.Destroy(unit);
				});

				unitList.Clear();
			});
		}

		public int GetPosition()
		{
			return SafeRunFunction(delegate
			{
				return position;
			});
		}

		private void GetNewUnit()
		{
			SafeRun(delegate
			{
				var unit = EntitySystem.CreateEntity<MemoryUnit>();
				unitList.AddValue(unit);
				allSize += unitSize;
			});
		}

		public bool ReadByte(int index,out byte value)
		{
			var readValue = byte.MinValue;

			if (SafeRunFunction(delegate ()
			 {
				 if (index < position)
				 {
					 var unitIndex = index / unitSize;
					 var offset = index % unitSize;
					 var unitData = unitList[unitIndex];
					 readValue = unitData.ReadByte(offset);

					 return true;
				 }

				 return false;
			 }))
			{
				value = readValue;

				return true;
			}
			else
			{
				value = readValue;

				return false;
			}
		}

		public void WriteByte(byte value)
		{
			SafeRun(delegate
			{
				var index = position + 1;

				if (position >= (allSize - 1))
				{
					GetNewUnit();
				}

				var unitIndex = index / unitSize;
				var offset = index % unitSize;
				var unitData = unitList[unitIndex];
				unitData.WriteByte(offset, value);
				position++;
			});
		}

		public void WriteData(byte[] data, int index)
		{
			SafeRun(delegate
			{
				var hasSize = allSize - (position + 1);
				var writeSize = 0;

				while (writeSize < data.Length)
				{
					if ((data.Length - writeSize) <= hasSize)
					{
						var unitData = unitList[unitList.GetCount()];
						unitData.WriteByte(index, data,data.Length);
						writeSize += data.Length;
						
						hasSize -= data.Length;
					}
					else
					{
						var unitData = unitList[unitList.GetCount()];
						unitData.WriteByte(index, data, hasSize);
						writeSize += hasSize;

						if (writeSize < data.Length)
						{
							GetNewUnit();
						}

						hasSize = allSize - (position + 1);
					}
				}
			});
		}

		public int GetMemorySize()
		{
			return SafeRunFunction(delegate 
			{
				return allSize;
			});
		}
	}
}
