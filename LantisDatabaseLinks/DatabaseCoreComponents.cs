using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Data.Common;
using Lantis.EntityComponentSystem;
using Lantis.Pool;

namespace Lantis.DatabaseLinks
{
	public class DatabaseCoreComponents : ComponentEntity
	{
		public DatabaseLinksPool sqlLinkPool;

        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();
        }

		public static object[] ParamCreate(string sqlLink)
		{
			return new object[] { sqlLink };
		}


		public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);

			SafeRun(delegate
			{
				var sqlLinkString = paramsData[0] as string;
				sqlLinkPool = LantisPoolSystem.GetPool<DatabaseLinksPool>().NewObject();
				sqlLinkPool.Instance(sqlLinkString);
			});
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

		public int ExecuteNonQuery(string sqlStr, List<DbParameter> parameters = null)
		{
			return SafeRunFunction(delegate
			{
				DatabaseLinkState sqlLink = sqlLinkPool.SpawnInstance();
				DbConnection SqlCon = sqlLink.sqlConnectInstance;

				using (DbCommand SqlCmd = SqlCon.CreateCommand())
				{
					SqlCmd.CommandText = sqlStr;

					if (parameters != null)
					{
						foreach (DbParameter parameter in parameters)
						{
							SqlCmd.Parameters.Add(parameter);
						}
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
			});
		}

		public object ExecuteScalar(string sqlStr, List<DbParameter> parameters = null)
		{
			return SafeRunFunction(delegate
			{
				var sqlLink = sqlLinkPool.SpawnInstance();
				DbConnection SqlCon = sqlLink.sqlConnectInstance;

				using (DbCommand SqlCmd = SqlCon.CreateCommand())
				{
					SqlCmd.CommandText = sqlStr;

					if (parameters != null)
					{
						foreach (DbParameter parameter in parameters)
						{
							SqlCmd.Parameters.Add(parameter);
						}
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
			});
		}

		public DataTable ExecuteDataTable(string sqlString, List<DbParameter> parameters = null)
		{
			return SafeRunFunction(delegate
			{
				{
					var sqlLink = sqlLinkPool.SpawnInstance();
					DbConnection SqlConnect = sqlLink.sqlConnectInstance;

					using (DbCommand SqlCmd = SqlConnect.CreateCommand())
					{
						SqlCmd.CommandText = sqlString;

						if (parameters != null)
						{
							foreach (DbParameter parameter in parameters)
							{
								SqlCmd.Parameters.Add(parameter);
							}
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
			});
		}

		public DbDataReader ExecuteReader(string sqlStr, List<DbParameter> parameters = null)
		{
			return SafeRunFunction(delegate
			{
				var sqlLink = sqlLinkPool.SpawnInstance();
				var SqlConnect = sqlLink.sqlConnectInstance;

				using (DbCommand SqlCmd = SqlConnect.CreateCommand())
				{
					SqlCmd.CommandText = sqlStr;

					if (parameters != null)
					{
						foreach (DbParameter parameter in parameters)
						{
							SqlCmd.Parameters.Add(parameter);
						}
					}

					sqlLinkPool.Despawn(sqlLink);

					return SqlCmd.ExecuteReader();
				}
			});
		}
	}
}
