using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Redis;
using Lantis.Redis.Message;
using Lantis.Extend;
using Lantis.Pool;
using Lantis.RedisExecute;
using Lantis.Locker;
using Lantis.EntityComponentSystem;

namespace Lantis.RedisExecute
{
    public class RedisExecuteBranch : BranchEntity
    {
        private LantisDictronaryList<string, RedisUnit> redisUnitCollects;

        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();

            SafeRun(delegate
            {
                redisUnitCollects = LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisUnit>>().NewObject();
            });
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();

            SafeRun(delegate
            {
                LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisUnit>>().DisposeObject(redisUnitCollects);
                redisUnitCollects = null;
            });
        }

        public RedisUnit FindRedisUnit(string databaseName)
        {
            return SafeRunFunction<RedisUnit>(new Func<RedisUnit>(() => 
            {
                if (redisUnitCollects.HasKey(databaseName))
                {
                    return redisUnitCollects[databaseName];
                }

                return null;
            }));
        }

        public void SetData(RequestRedisSet requestData)
        {
            SafeRun(delegate 
            {
                if (requestData.data != null)
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

                                    redisTableData.FieldDataFromSeriizable(requestData.data);
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
            });
        }

        public void GetData(RequestRedisGet requestData)
        {
            SafeRun(delegate 
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
            });
        }
    }
}
