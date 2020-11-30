using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LantisExtend;
using LantisPool;

namespace LantisRedisCore
{
    [Serializable]
    public class RedisTableData : LantisPool.LantisPoolInterface
    {
        public string databaseName;
        private LantisDictronaryList<string, RedisTableField> fieldCollects;

        public void OnCreate()
        {
            fieldCollects = new LantisDictronaryList<string, RedisTableField>();
        }

        public void OnEnable()
        {
        }

        public void OnDisable()
        {
            ClearFields();
        }

        public void OnDestroy()
        {
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
