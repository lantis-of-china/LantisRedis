using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections;
using System.Data.Common;
using System.Data;

namespace Lantis.DatabaseLinks
{
	/// <summary>
	/// 数据库连接池
	/// </summary>
	public class DatabaseLinks
    {
		public string linkPoolStr;
		public int maxCount;
		public int minCount;
		public int curUseCount;
		public List<DatabaseLinkState> sqlLinkPoolInstance = new List<DatabaseLinkState>();

		/// <summary>
		/// 实例化数据
		/// </summary>
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
			DatabaseLinkState item = new DatabaseLinkState(linkStr);

			item.isUsed = false;

			lock (((ICollection)sqlLinkPoolInstance).SyncRoot)
			{
				sqlLinkPoolInstance.Add(item);
			}
			return item;
		}

		public DatabaseLinkState SpawnInstance()
		{
			CheckClear();

			lock (((ICollection)sqlLinkPoolInstance).SyncRoot)
			{
				for (int i = sqlLinkPoolInstance.Count - 1; i >= 0; --i)
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
			}

			//这里没有找到
			if (sqlLinkPoolInstance.Count < maxCount)
			{
				//这里可以创建
				DatabaseLinkState curItem = CreateInstance(linkPoolStr);

				curItem.isUsed = true;
				curUseCount++;
				return curItem;
			}
			else
			{
				System.Threading.Thread.Sleep(1);
				DatabaseLinkState curItem = SpawnInstance();

				return curItem;
			}
		}

		public void Despawn(DatabaseLinkState desItem)
		{
			if (desItem.isUsed)
			{
				curUseCount--;
				desItem.isUsed = false;

				Console.WriteLine("当前线程池 使用中:" + curUseCount);
			}
		}

		/// <summary>
		/// 检测清理池
		/// </summary>
		public void CheckClear()
		{
			lock (((ICollection)sqlLinkPoolInstance).SyncRoot)
			{
				for (int i = sqlLinkPoolInstance.Count - 1; i >= 0; --i)
				{
					if (sqlLinkPoolInstance[i].sqlConnectInstance.State == ConnectionState.Closed
						|| sqlLinkPoolInstance[i].sqlConnectInstance.State == ConnectionState.Broken)
					{
						sqlLinkPoolInstance[i].Dispose();

						sqlLinkPoolInstance.RemoveAt(i);
					}
				}
			}
		}
	}
}