using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LantisExtend;
using LantisPool;
using LantisRedisCore;
using LantisRedisCore.Message;

namespace LantisRedisExecute.LantisRedisExecute
{
    public class RedisExecute
    {
        private static object lockObject;
        private static LantisDictronaryList<string, RedisUnit> redisUnitCollects;

        public static void Init()
        {
            lockObject = new object();
            redisUnitCollects = new LantisDictronaryList<string, RedisUnit>();
        }

        public static void Close()
        {
            lock (lockObject)
            {
                var redisUnitList = redisUnitCollects.ValueToList();
                var dataCount = redisUnitList.Count;

                for (var i = 0; i < dataCount; ++i)
                {
                    var unitItem = redisUnitList[i];
                    LantisPoolSystem.GetPool<RedisUnit>().DisposeObject(unitItem);
                }

                redisUnitCollects.Clear();
                redisUnitCollects = null;
            }

            lockObject = null;
        }

        public static RedisUnit FindRedisUnit(string databaseName)
        {
            lock (lockObject)
            {
                if(redisUnitCollects.HasKey(databaseName))
                {
                    return redisUnitCollects[databaseName];
                }
            }

            return null;
        }

        public static void SetData(RequestRedisSet requestData)
        {
            if (requestData.data != null && requestData.data.Length > 0)
            {
                if (!string.IsNullOrEmpty(requestData.databaseName) && !string.IsNullOrEmpty(requestData.tableName))
                {
                    var redisUnit = FindRedisUnit(requestData.databaseName);

                    if (redisUnit != null)
                    {
                        var redisTable = redisUnit.GetRedisTable(requestData.tableName);

                        if (redisTable != null)
                        {
                            var operationCondition = requestData.conditionGroup.conditionList[0];

                            if (operationCondition.fieldName == RedisConst.id)
                            {
                                var redisTableData = redisTable.GetDataById(operationCondition.fieldValue);

                                if (redisTableData == null)
                                {
                                    redisTableData = LantisPoolSystem.GetPool<RedisTableData>().NewObject();
                                    redisTable.AddDataById(operationCondition.fieldValue, redisTableData);
                                }

                                redisTableData.FieldDataFromString(requestData.data);
                            }
                            else
                            {
                                Logger.Error($"can't find redis table data by id,the id field name is not:{RedisConst.id}");
                            }
                        }
                        else
                        {
                            Logger.Error($"can't find redisTable by talbeName:{requestData.tableName}");
                        }
                    }
                    else
                    {
                        Logger.Error($"con't find redisUnit by databaseName:{requestData.databaseName}");
                    }
                }
                else
                {
                    Logger.Error($"databaseName or tableName null,databaseName:{requestData.databaseName} tableName:{requestData.tableName}");
                }
            }
            else
            {
                Logger.Error("can't set data,becuse is null data");
            }
        }

        public static void GetData(RequestRedisGet requestData)
        {
            if (!string.IsNullOrEmpty(requestData.databaseName) && !string.IsNullOrEmpty(requestData.tableName))
            {
                var redisUnit = FindRedisUnit(requestData.databaseName);

                if (redisUnit != null)
                {
                    if (redisUnit != null)
                    {
                        var redisTable = redisUnit.GetRedisTable(requestData.tableName);

                        if (redisTable != null)
                        {
                            var operationCondition = requestData.conditionGroup.conditionList[0];

                            if (operationCondition.fieldName == RedisConst.id)
                            {
                                var redisTableData = redisTable.GetDataById(operationCondition.fieldValue);

                                if (redisTableData != null)
                                {
                                }
                                else
                                {
                                    Logger.Log("todo redis table data null,should be write logic");
                                }
                            }
                            else
                            {
                                Logger.Error($"can't find redis table data by id,the id field name is not:{RedisConst.id}");
                            }
                        }
                        else
                        {
                            Logger.Error($"can't find redisTable by talbeName:{requestData.tableName}");
                        }
                    }
                    else
                    {
                        Logger.Error($"con't find redisUnit by databaseName:{requestData.databaseName}");
                    }
                }
                else
                {
                    Logger.Error($"con't find redisUnit by databaseName:{requestData.databaseName}");
                }
            }
            else
            {
                Logger.Error($"databaseName or tableName null,databaseName:{requestData.databaseName} tableName:{requestData.tableName}");
            }
        }
    }
}
