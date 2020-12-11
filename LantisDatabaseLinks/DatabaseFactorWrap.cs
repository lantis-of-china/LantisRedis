using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lantis.DatabaseLinks
{
    public class SqlFactorWarp
    {
        private const string sqlClassName = "System.Data.SqlClient";
        private static DbProviderFactory factory;

        public static void Init()
        {
            if (factory == null)
            {
                DbProviderFactories.RegisterFactory(sqlClassName, SqlClientFactory.Instance);
                factory = DbProviderFactories.GetFactory(sqlClassName);
            }
        }

        public static DbConnection CreateConnection(string connectionString)
        {
            var connection = factory.CreateConnection();

            connection.ConnectionString = connectionString;

            return connection;
        }

        public static DbDataAdapter CreateAdapter(DbCommand command)
        {
            var adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = command;
            adapter.UpdateCommand = command;
            adapter.InsertCommand = command;
            adapter.DeleteCommand = command;

            return adapter;
        }
    }
}
