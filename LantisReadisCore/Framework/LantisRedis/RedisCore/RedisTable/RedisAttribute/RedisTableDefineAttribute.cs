using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LantisRedisCore
{
    [System.Serializable]
    public class RedisTableDefineAttribute : Attribute
    {
        private string databaseName;

        public RedisTableDefineAttribute(string databaseName)
        {
            this.databaseName = databaseName;
        }

        public string GetDatabaseName()
        {
            return databaseName;
        }

        public void SetDatabaseName(string databaseName)
        {
            this.databaseName = databaseName;
        }
    }
}
