using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.Redis
{
    [Serializable]
    public class RedisSerializableData : LantisPoolInterface
    {
        public string databaseName;
        public string tableName;
        public List<RedisSerializableField> fields;

        public RedisSerializableData()
        {
            fields = new List<RedisSerializableField>();
        }

        public void OnPoolSpawn()
        {
            databaseName = string.Empty;
            tableName = string.Empty;
        }

        public void OnPoolDespawn()
        {
            databaseName = string.Empty;
            tableName = string.Empty;
            var poolHandle = LantisPoolSystem.GetPool<RedisSerializableField>();

            for (var i = 0; i < fields.Count; ++i)
            {
                poolHandle.DisposeObject(fields[i]);
            }

            fields.Clear();
        }

        public void AddFieldData(RedisSerializableField fieldData)
        {
            fields.Add(fieldData);
        }
    }
}
