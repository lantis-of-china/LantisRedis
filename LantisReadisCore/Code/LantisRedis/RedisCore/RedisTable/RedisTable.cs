using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;
using Lantis.Extend;
using Lantis.Locker;

namespace Lantis.Redis
{
    public class RedisTable : SafeLocker, LantisPoolInterface
    {
        public string tableName;
        private LantisDictronaryList<string, RedisTableFieldDefine> redisFieldCollects;
        private LantisDictronaryList<string, RedisTableData> redisDataCollects;

        public LantisDictronaryList<string, RedisTableFieldDefine> GetRedisFieldCollects()
        {
            return redisFieldCollects;
        }

        public RedisTable()
        {
        }

        public void OnPoolSpawn()
        {
            redisFieldCollects = LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableFieldDefine>>().NewObject();
            redisDataCollects = LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableData>>().NewObject();
        }

        public void OnPoolDespawn()
        {
            SafeRun(delegate
            {
                var valueList = redisDataCollects.ValueToList();
                var poolHandle = LantisPoolSystem.GetPool<RedisTableData>();

                for (var i = 0; i < valueList.Count; ++i)
                {
                    var valueItem = valueList[i];
                    poolHandle.DisposeObject(valueItem);
                }

                var valueList2 = redisFieldCollects.ValueToList();
                var poolHandle2 = LantisPoolSystem.GetPool<RedisTableFieldDefine>();

                for (var i = 0; i < valueList2.Count; ++i)
                {
                    var valueItem = valueList2[i];
                    poolHandle2.DisposeObject(valueItem);
                }

                redisDataCollects.Clear();
                redisFieldCollects.Clear();
                LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableFieldDefine>>().DisposeObject(redisFieldCollects);
                LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisTableData>>().DisposeObject(redisDataCollects);
                redisDataCollects = null;
                redisFieldCollects = null;
            });
        }

        public void OnDestroy()
        {
            SafeRun(delegate 
            {
                redisDataCollects = null;
                redisFieldCollects = null;
            });
        }

        public RedisTableData GetDataById(string id)
        {
            return SafeRunFunction(new Func<RedisTableData>(delegate
            {
                if (redisDataCollects.HasKey(id))
                {
                    return redisDataCollects[id];
                }

                return null;
            }));

        }

        public void AddDataById(string id, RedisTableData data)
        {
            SafeRun(delegate 
            {
                if (!redisDataCollects.HasKey(id))
                {
                    redisDataCollects.AddValue(id, data);

                    return;
                }

                redisDataCollects.RemoveKey(id);
                redisDataCollects.AddValue(id, data);
            });
        }

        public void OverData(string id,RedisTableData data)
        {
            SafeRun(delegate
            {
                if (!redisDataCollects.HasKey(id))
                {
                    var redisTableData = redisDataCollects[id];
                    redisTableData.ClearFields();
                    var fields = data.GetFieldCollects();

                    fields.SafeWhile(delegate(string fieldName,RedisTableField redisTableField)
                    {
                        redisTableData.AddField(redisTableField);
                    });

                    data.RemoveAllFields();
                }
            });
        }

        public void AddFieldInfo(RedisTableFieldDefine redisTableFieldDefine)
        {
            SafeRun(delegate
            {
                redisFieldCollects.AddValue(redisTableFieldDefine.fieldName, redisTableFieldDefine);
            });
        }

        public List<RedisTableFieldDefine> GetRedisTableFieldList()
        {
            return SafeRunFunction(delegate
            {
                return redisFieldCollects.ValueToList();
            });
        }

        public RedisTableData GetData(LantisRedisConditionGroup redisConditions)
        {
            return SafeRunFunction(delegate
            {
                for (var i = 0; i < redisConditions.conditionList.Count; ++i)
                {
                    var condition = redisConditions.conditionList[i];

                    if (condition.fieldName == RedisConst.id && condition.operation == "=")
                    {
                        if (redisDataCollects.HasKey(condition.fieldValue))
                        {
                            return redisDataCollects[condition.fieldValue];
                        }
                    }
                    else
                    {
                    }
                }

                return null;
            });
        }

        public string SetData(LantisRedisConditionGroup redisConditions,RedisSerializableData data)
        {
            return SafeRunFunction(delegate
            {
                string command = "";
                var newTableData = RedisCore.RedisSerializableToRedisTableData(data);
                var tableField = newTableData.GetFieldObject(RedisConst.id);
                var id = tableField.fieldValue.ToString();
                var tableData = GetDataById(id);

                if (tableData == null)
                {
                    AddDataById(id, newTableData);
                    command = RedisCore.GetInsertCommand(tableName, newTableData);
                }
                else 
                {
                    OverData(id, newTableData);
                    command = RedisCore.GetUpdataCommand(tableName, newTableData,redisConditions);
                }

                LantisPoolSystem.GetPool<RedisTableData>().DisposeObject(newTableData);

                return command;
            });
        }

        public string UpdateData(LantisRedisConditionGroup redisConditions, Dictionary<string,object> data)
        {
            return SafeRunFunction(delegate
            {
                RedisTableData findData = null;

                redisDataCollects.SafeWhileBreak((fieldName, redistableData) =>
                {
                    if (RedisCore.RedisTableDataCondition(redistableData, redisConditions))
                    {
                        findData = redistableData;

                        return false;
                    }

                    return true;
                });

                if (findData != null)
                {
                    foreach (var kv in data)
                    {
                        var fieldName = kv.Key;
                        var fieldValue = kv.Value;
                        var redisDataField = findData.GetFieldObject(fieldName);

                        if (redisDataField != null)
                        {
                            redisDataField.fieldValue = fieldValue;
                        }
                    }

                    var command = RedisCore.GetUpdataCommandFromFields(tableName, data, redisConditions);

                    return command;
                }
                else
                {
                    return string.Empty;
                }
            });
        }
    }
}