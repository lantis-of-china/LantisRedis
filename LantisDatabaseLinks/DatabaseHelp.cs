using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Data.Common;

namespace Lantis.DatabaseLinks
{

	public class DatabaseHelp
	{
		public static object lockSelf = new object();
		public static DatabaseLinksPool sqlLinkPool;

		public static void CreateSqlLinkPool(string sqlLinkStr)
		{
			lock (lockSelf)
			{
				sqlLinkPool = new DatabaseLinksPool();
				sqlLinkPool.Instance(sqlLinkStr);
			}
		}

		public static int ExecuteNonQuery(string sqlStr, params DbParameter[] parameters)
		{
			lock (lockSelf)
			{
				DatabaseLinkState sqlLink = sqlLinkPool.SpawnInstance();
				DbConnection SqlCon = sqlLink.sqlConnectInstance;

				using (DbCommand SqlCmd = SqlCon.CreateCommand())
				{
					SqlCmd.CommandText = sqlStr;

					foreach (DbParameter parameter in parameters)
					{
						SqlCmd.Parameters.Add(parameter);

					}

					string st = SqlCmd.CommandText;

					try
					{
						int result = SqlCmd.ExecuteNonQuery();
						sqlLinkPool.Despawn(sqlLink);

						return result;
					}
					catch (Exception e)
					{
						Logger.Error("Sql ExecuteNonQuery Exception:" + e.ToString());
						sqlLinkPool.Despawn(sqlLink);

						return 0;
					}
				}
			}
		}

		public static object ExecuteScalar(string sqlStr, params DbParameter[] parameters)
		{
			lock (lockSelf)
			{				
				var sqlLink = sqlLinkPool.SpawnInstance();
				DbConnection SqlCon = sqlLink.sqlConnectInstance;

				using (DbCommand SqlCmd = SqlCon.CreateCommand())
				{
					SqlCmd.CommandText = sqlStr;

					foreach (DbParameter parameter in parameters)
					{
						SqlCmd.Parameters.Add(parameter);
					}

					try
					{
						object result = SqlCmd.ExecuteScalar();
						sqlLinkPool.Despawn(sqlLink);

						return result;
					}
					catch (Exception e)
					{
						Logger.Error("Sql ExecuteNonQuery Exception:" + e.ToString());
						sqlLinkPool.Despawn(sqlLink);

						return null;
					}
				}
			}
		}

		public static DataTable ExecuteDataTable(string sqlStr, params DbParameter[] parameters)
		{
			lock (lockSelf)
			{
				var sqlLink = sqlLinkPool.SpawnInstance();
				DbConnection SqlCon = sqlLink.sqlConnectInstance;

				using (DbCommand SqlCmd = SqlCon.CreateCommand())
				{
					SqlCmd.CommandText = sqlStr;

					foreach (DbParameter parameter in parameters)
					{
						SqlCmd.Parameters.Add(parameter);
					}

					DataSet dataSet = new DataSet();
					DbDataAdapter dataAdapter = SqlFactorWarp.CreateAdapter(SqlCmd);
					dataAdapter.Fill(dataSet);					
					sqlLinkPool.Despawn(sqlLink);

					if (dataSet.Tables == null)
					{
						return null;
					}

					if (dataSet.Tables.Count == 0)
					{
						return null;
					}

					return dataSet.Tables[0];
				}
			}
		}

		public static DbDataReader ExecuteReader(string sqlStr, params DbParameter[] parameters)
		{
			lock (lockSelf)
			{
				var sqlLink = sqlLinkPool.SpawnInstance();
				DbConnection SqlCon = sqlLink.sqlConnectInstance;

				using (DbCommand SqlCmd = SqlCon.CreateCommand())
				{
					SqlCmd.CommandText = sqlStr;

					foreach (DbParameter parameter in parameters)
					{
						SqlCmd.Parameters.Add(parameter);
					}

					sqlLinkPool.Despawn(sqlLink);
					return SqlCmd.ExecuteReader();
				}
			}
		}
	}
}
