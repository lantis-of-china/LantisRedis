using System;
using System.Collections.Generic;
using System.Text;
using Lantis.EntityComponentSystem;
using Lantis.Extend;
using Lantis.Redis;
using Lantis.Pool;
using Lantis.Redis.Message;

namespace Lantis.RedisExecute
{
    public class RedisMemoryComponent : ComponentEntity
    {
        public LantisDictronaryList<string, RedisUnit> redisMemory;
        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();

            SafeRun(delegate
            {
                redisMemory = LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisUnit>>().NewObject();
            });
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();

            SafeRun(delegate
            {
                redisMemory.SafeWhile(delegate(string key, RedisUnit redisUnit)
                {
                    LantisPoolSystem.GetPool<RedisUnit>().DisposeObject(redisUnit);
                });

                LantisPoolSystem.GetPool<LantisDictronaryList<string, RedisUnit>>().DisposeObject(redisMemory);
                redisMemory = null;
            });
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);
        }

        public bool CheckHasUnit(string databaseName)
        {
            return SafeRunFunction(delegate
            {
                return redisMemory.HasKey(databaseName);
            });
        }

        public RedisUnit GetUnit(string databaseName)
        {
            return SafeRunFunction(delegate
            {
                return redisMemory[databaseName];
            });
        }

        public void AddUnitByDatabaseName(string databaseName)
        {
            SafeRun(delegate
            {
                var redisUnit = LantisPoolSystem.GetPool<RedisUnit>().NewObject();
                redisUnit.databaseName = databaseName;
                redisMemory.AddValue(redisUnit.databaseName, redisUnit);
            });
        }

        public void AddByRedisTab(RedisCheckDatabase redisCheckDatabase)
        {
            SafeRun(delegate
            {
                var unit = GetUnit(redisCheckDatabase.databaseName);
                var redisTable = LantisPoolSystem.GetPool<RedisTable>().NewObject();
                redisTable.tableName = redisCheckDatabase.tableName;
                unit.AddRedisTable(redisTable);

                redisCheckDatabase.tableInfos.ForEach(delegate (RedisTableFieldDefine tableFieldDefine)
                {
                    var redisTableFieldDefine = LantisPoolSystem.GetPool<RedisTableFieldDefine>().NewObject();
                    redisTableFieldDefine.fieldName = tableFieldDefine.fieldName;
                    redisTableFieldDefine.fieldType = tableFieldDefine.fieldType;
                    redisTable.AddFieldInfo(redisTableFieldDefine);
                });
            });
        }
    }
}
