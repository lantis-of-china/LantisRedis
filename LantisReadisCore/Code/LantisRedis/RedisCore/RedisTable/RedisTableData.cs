using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Extend;
using Lantis.Pool;
using Lantis.Locker;

namespace Lantis.Redis
{
    [Serializable]
    public class RedisTableData : SafeLocker,LantisPoolInterface
    {
        public string databaseName;
        private LantisDictronaryList<string, RedisTableField> fieldCollects;

        public void OnPoolSpawn()
        {
            SafeRun(delegate
            {
                databaseName = string.Empty;
                fieldCollects = LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableField>>().NewObject();
            });
        }

        public void OnPoolDespawn()
        {
            SafeRun(delegate
            {
                ClearFields();
                LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableField>>().DisposeObject(fieldCollects);
                fieldCollects = null;
            });
        }

        public void RemoveAllFields()
        {
            SafeRun(delegate
            {
                fieldCollects.Clear();
            });
        }

        public LantisDictronaryList<string, RedisTableField> GetFieldCollects()
        {
            return fieldCollects;
        }

        public void ClearFields()
        {
            SafeRun(delegate
            {
                var valueList = fieldCollects.ValueToList();
                var poolHandle = LantisPoolSystem.GetPool<RedisTableField>();

                for (var i = 0; i < valueList.Count; ++i)
                {
                    poolHandle.DisposeObject(valueList[i]);
                }

                fieldCollects.Clear();
            });
        }

        public void AddField(RedisTableField field)
        {
            SafeRun(delegate
            {
                fieldCollects.AddValue(field.fieldName, field);
            });
        }

        public RedisTableField GetFieldObject(string fieldName)
        {
            return SafeRunFunction(delegate 
            {
                if (fieldCollects.HasKey(fieldName))
                {
                    return fieldCollects[fieldName];
                }

                return null;
            });
        }

        public void FieldDataFromString(byte[] data)
        {
            SafeRun(delegate
            {
                ClearFields();
                RedisCore.DataToRedisTableData(data, this);
            });
        }

        public void FieldDataFromSeriizable(RedisSerializableData data)
        {
            SafeRun(delegate
            {
                ClearFields();
                RedisCore.RedisSerializableToRedisTableData(data, this);
            });            
        }
    }
}
