using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Extend;
using Lantis.Pool;

namespace Lantis.Redis
{
    [Serializable]
    public class RedisTableData : LantisPoolInterface
    {
        public string databaseName;
        private LantisDictronaryList<string, RedisTableField> fieldCollects;

        public void OnPoolSpawn()
        {
            databaseName = string.Empty;
            fieldCollects = LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableField>>().NewObject();
        }

        public void OnPoolDespawn()
        {
            ClearFields();
            LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableField>>().DisposeObject(fieldCollects);
            fieldCollects = null;
        }

        public LantisDictronaryList<string, RedisTableField> GetFieldCollects()
        {
            return fieldCollects;
        }

        public void ClearFields()
        {
            var valueList = fieldCollects.ValueToList();
            var poolHandle = LantisPoolSystem.GetPool<RedisTableField>();

            for (var i = 0; i < valueList.Count; ++i)
            {
                poolHandle.DisposeObject(valueList[i]);
            }

            fieldCollects.Clear();
        }

        public void AddField(RedisTableField field)
        {
            fieldCollects.AddValue(field.fieldName,field);
        }

        public void FieldDataFromString(byte[] data)
        {
            ClearFields();
            RedisCore.DataToRedisTableData(data, this);
        }
    }
}
