using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LantisPool;

namespace LantisRedisCore
{
    [Serializable]
    public class RedisSerializableData : LantisPoolInterface
    {
        public string databaseName;
        public string tableName;
        public List<RedisSerializableField> fields;

        public void OnCreate()
        {
            databaseName = string.Empty;
            tableName = string.Empty;
            fields = new List<RedisSerializableField>();
        }

        public void OnEnable()
        {
        }

        public void OnDisable()
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

        public void OnDestroy()
        {
            fields = null;
            databaseName = null;
            tableName = null;
        }

        public void AddFieldData(RedisSerializableField fieldData)
        {
            fields.Add(fieldData);
        }
    }
}
