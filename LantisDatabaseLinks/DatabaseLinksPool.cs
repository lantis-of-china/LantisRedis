using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections;
using System.Data.Common;
using System.Data;
using Lantis.EntityComponentSystem;
using Lantis.Extend;
using Lantis.Pool;
using Lantis.Locker;

namespace Lantis.DatabaseLinks
{
	/// <summary>
	/// 数据库连接池
	/// </summary>
	public class DatabaseLinksPool : SafeLocker, LantisPoolInterface
    {
		public string linkPoolStr;
		public int maxCount;
		public int minCount;
		public int curUseCount;
		public LantisList<DatabaseLinkState> sqlLinkPoolInstance;

		public void OnPoolSpawn()
		{
			SafeRun(delegate
			{
				linkPoolStr = string.Empty;
				maxCount = 0;
				minCount = 0;
				curUseCount = 0;
				sqlLinkPoolInstance = LantisPoolSystem.GetPool<LantisList<DatabaseLinkState>>().NewObject();
			});
		}

		public void OnPoolDespawn()
        {
			SafeRun(delegate
			{
				sqlLinkPoolInstance.SafeWhile(delegate (DatabaseLinkState state)
				{
					LantisPoolSystem.GetPool<DatabaseLinkState>().DisposeObject(state);
				});

				LantisPoolSystem.GetPool<LantisList<DatabaseLinkState>>().DisposeObject(sqlLinkPoolInstance);
				sqlLinkPoolInstance = null;
			});
		}

        public void Instance(string linkStr)
		{
			linkPoolStr = linkStr;
			curUseCount = 0;
			maxCount = 60;
			minCount = 25;

			for (int i = 0; i < minCount; ++i)
			{
				CreateInstance(linkStr);
			}

            Logger.Log("数据持久化连接池初始化完成!");
		}

		public DatabaseLinkState CreateInstance(string linkStr)
		{
			return SafeRunFunction(delegate
			{
				DatabaseLinkState item = LantisPoolSystem.GetPool<DatabaseLinkState>().NewObject();
				item.SetLink(linkStr);
				item.isUsed = false;
				sqlLinkPoolInstance.AddValue(item);

				return item;
			});
		}

		public DatabaseLinkState SpawnInstance()
		{
			return SafeRunFunction(delegate
			{
				CheckClear();

				for (int i = sqlLinkPoolInstance.GetCount() - 1; i >= 0; --i)
				{
					if (!sqlLinkPoolInstance[i].isUsed
						&& sqlLinkPoolInstance[i].sqlConnectInstance.State != ConnectionState.Closed
						&& sqlLinkPoolInstance[i].sqlConnectInstance.State != ConnectionState.Broken)
					{
						curUseCount++;
						sqlLinkPoolInstance[i].isUsed = true;
						return sqlLinkPoolInstance[i];
					}
				}

				if (sqlLinkPoolInstance.GetCount() < maxCount)
				{
					var curItem = CreateInstance(linkPoolStr);
					curItem.isUsed = true;
					curUseCount++;
					return curItem;
				}
				else
				{
					System.Threading.Thread.Sleep(10);
					var curItem = SpawnInstance();
					return curItem;
				}
			});
		}

		public void Despawn(DatabaseLinkState desItem)
		{
			SafeRun(delegate
			{
				if (desItem.isUsed)
				{
					curUseCount--;
					desItem.isUsed = false;
					Console.WriteLine("当前线程池 使用中:" + curUseCount);
				}
			});
		}

		public void CheckClear()
		{
			SafeRun(delegate
			{
				for (int i = sqlLinkPoolInstance.GetCount() - 1; i >= 0; --i)
				{
					if (sqlLinkPoolInstance[i].sqlConnectInstance.State == ConnectionState.Closed
						|| sqlLinkPoolInstance[i].sqlConnectInstance.State == ConnectionState.Broken)
					{
						sqlLinkPoolInstance[i].Dispose();
						sqlLinkPoolInstance.RemoveAt(i);
					}
				}
			});
		}
    }
}