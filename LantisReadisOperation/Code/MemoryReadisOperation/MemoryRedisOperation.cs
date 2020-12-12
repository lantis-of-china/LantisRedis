using System;
using System.Collections.Generic;
using Lantis.Pool;
using Lantis.Redis;
using Lantis.Redis.Message;
using Lantis.EntityComponentSystem;
using Lantis.Network;

namespace Lantis.ReadisOperation
{
    public static class MemoryReadisOperation
    {
        private static List<Type> tableList = new List<Type>
        {
           typeof(TestRedisDataSleep3)
        };

        public static void CheckTable()
        {
            var checkDb = LantisPoolSystem.GetPool<RequestRedisCheckDatabase>().NewObject();

            for (var i = 0; i < tableList.Count; ++i)
            {
                var type = tableList[i];
                checkDb.redisTableFieldDefine.Add(RedisCore.GetTypeField(type));
            }

            SubmitRequestRedisCheck(checkDb,null);
            LantisPoolSystem.GetPool<RequestRedisCheckDatabase>().DisposeObject(checkDb);
        }

        /// <summary>
        /// set data to redis
        /// </summary>
        /// <param name="menberId"></param>
        /// <param name="data"></param>
        public static void SetData<T>(object id, T data, Action<object> finisCallBack) where T : RedisBase
        {
            var redisRequest = LantisPoolSystem.GetPool<RequestRedisSet>().NewObject();
            redisRequest.databaseName = RedisCore.GetDatabaseNameFromData(data);
            redisRequest.tableName = RedisCore.GetMemoryRedisDataTypeName(data);
            redisRequest.data = RedisSerializable.SerializableToBytes(data);
            var condition = LantisPoolSystem.GetPool<LantisRedisCondition>().NewObject();
            redisRequest.conditionGroup.conditionList.Add(condition);
            condition.fieldName = "id";
            condition.operation = "=";
            condition.fieldValue = RedisCore.GetStringValueObject(id);
            SubmitRequestRedisSet(redisRequest, finisCallBack);
        }

        /// <summary>
        /// get data from redis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="finisCallBack"></param>
        public static void GetData<T>(object id, Action<object> finisCallBack) where T : RedisBase
        {
            var redisRequest = LantisPoolSystem.GetPool<RequestRedisGet>().NewObject();
            redisRequest.tableName = RedisCore.GetTypeName<T>();
            var condition = LantisPoolSystem.GetPool<LantisRedisCondition>().NewObject();
            redisRequest.conditionGroup.conditionList.Add(condition);
            condition.fieldName = "id";
            condition.operation = "=";
            condition.fieldValue = RedisCore.GetStringValueObject(id);
            SubmitRequestRedisGet(redisRequest, finisCallBack);            
        }

        /// <summary>
        /// get page datas with conditions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditionList"></param>
        /// <param name="everyCount"></param>
        /// <param name="page"></param>
        /// <param name="finisCallBack"></param>
        public static void GetPageDatasWithConditions<T>(List<LantisRedisCondition> conditionList, int everyCount, int page,Action<object> finisCallBack) where T : RedisBase
        {
            var redisRequest = LantisPoolSystem.GetPool<RequestRedisGetPage>().NewObject();
            redisRequest.tableName = RedisCore.GetTypeName<T>();
            redisRequest.conditionGroup.conditionList = conditionList;
            redisRequest.everPageCount = everyCount;
            redisRequest.page = page;
            SubmitRequestRedisGetPage(redisRequest, finisCallBack);
        }

        public static void SubmitRequestRedisCheck(RequestRedisCheckDatabase request, Action<object> finisCallBack)
        {
            var data = RedisSerializable.Serialize(request);
            LogicTrunkEntity.Instance.GetComponent<NetBranch>().GetComponent<NetClientComponents>().SendMessage(MessageIdDefine.CheckDatabase,data);
            //Program.NetBranchHandle.NetClientComponentHandle.SendMessage(MessageIdDefine.CheckDatabase, data);
        }

        public static void SubmitRequestRedisSet(RequestRedisSet requestRedisSet,Action<object> finisCallBack)
        {
            var data = RedisSerializable.Serialize(requestRedisSet);
            LogicTrunkEntity.Instance.GetComponent<NetBranch>().GetComponent<NetClientComponents>().SendMessage(MessageIdDefine.SetData, data);
            //Program.NetBranchHandle.NetClientComponentHandle.SendMessage(MessageIdDefine.SetData,data);
        }

        public static void SubmitRequestRedisGet(RequestRedisGet request, Action<object> finisCallBack)
        {
            var data = RedisSerializable.SerializableToBytes(request);
            Program.NetBranchHandle.NetClientComponentHandle.SendMessage(MessageIdDefine.GetData, data);
        }

        public static void SubmitRequestRedisGetPage(RequestRedisGetPage requestRedisGetPage, Action<object> finishCallBack)
        {
        }
    }
}
