using LantisPool;
using LantisRedisCore;
using LantisRedisCore.Message;
using System;
using System.Collections.Generic;

namespace LantisRedis
{
    public static class MemoryReadisOperation
    {
        /// <summary>
        /// set data to redis
        /// </summary>
        /// <param name="menberId"></param>
        /// <param name="data"></param>
        public static void SetData<T>(object id, T data, Action<object> finisCallBack) where T : RedisBase
        {
            var redisRequest = LantisPoolSystem.GetPool<RequestRedisSet>().NewObject();
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

        public static void SubmitRequestRedisSet(RequestRedisSet requestRedisSet,Action<object> finisCallBack)
        {
        }

        public static void SubmitRequestRedisGet(RequestRedisGet requestRedisSet, Action<object> finisCallBack)
        {
        }

        public static void SubmitRequestRedisGetPage(RequestRedisGetPage requestRedisGetPage, Action<object> finishCallBack)
        {
        }
    }
}
