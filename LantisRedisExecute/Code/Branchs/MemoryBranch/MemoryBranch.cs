using System;
using System.Collections.Generic;
using System.Text;
using Lantis.EntityComponentSystem;
using Lantis.Redis.Message;
using Lantis.Redis;

namespace Lantis.RedisExecute
{
    public class MemoryBranch : BranchEntity
    {
        private RedisMemoryComponent redisMemoryComponent;

        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);

            SafeRun(delegate
            {
                redisMemoryComponent = AddComponentEntity<RedisMemoryComponent>();
            });
        }

        public void CheckMemory(RequestRedisCheckDatabase redisCheckDatabase)
        {
            SafeRun(delegate
            {
                redisCheckDatabase.redisTableFieldDefine.ForEach(delegate (RedisCheckDatabase item)
                {
                    var databaseName = item.databaseName;
                    var tableName = item.tableName;

                    if (!redisMemoryComponent.CheckHasUnit(databaseName))
                    {
                        redisMemoryComponent.AddUnitByDatabaseName(databaseName);
                    }

                    var unit = redisMemoryComponent.GetUnit(databaseName);

                    if (unit.GetRedisTable(tableName) == null)
                    {
                        redisMemoryComponent.AddByRedisTab(item);
                        var sqlCheck = RedisCore.GetCheckTableString(tableName);
                        var resultValue = (int)Program.DatabaseBranch.DatabaseCoreComponent.ExecuteScalar(sqlCheck);

                        if (resultValue == 0)
                        {
                            var sqlCreateTable = RedisCore.GetCreateTableString(tableName, item.tableInfos);
                            Program.DatabaseBranch.DatabaseCoreComponent.ExecuteNonQuery(sqlCreateTable);
                        }
                    }

                    var sqlSelect = RedisCore.GetSelectTableString(tableName);
                    var dataTable = Program.DatabaseBranch.DatabaseCoreComponent.ExecuteDataTable(sqlSelect);
                    var redisTable = unit.GetRedisTable(tableName);

                    if (dataTable != null)
                    {
                        RedisCore.DataTableToMemory(redisTable,dataTable);
                    }
                });
            });
        }

        public void SetMemory(RequestRedisSet requestMsg)
        {
            SafeRun(delegate
            {
                var unit = redisMemoryComponent.GetUnit(requestMsg.databaseName);

                if (unit != null)
                {
                    var table = unit.GetRedisTable(requestMsg.tableName);

                    if (table != null)
                    {
                        var serializableData = RedisSerializable.BytesToSerializable(requestMsg.data);
                        var sqlCommand = table.SetData(requestMsg.conditionGroup, serializableData);
                        Logger.Log(sqlCommand);
                        LogicTrunkEntity.Instance.GetComponent<DatabaseCommandBranch>().AddCommand(sqlCommand);
                    }
                }
            });
        }
    }
}
