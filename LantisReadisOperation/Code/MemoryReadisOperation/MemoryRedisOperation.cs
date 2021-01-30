﻿using System;
using System.Data.Common;
using System.Collections.Generic;
using Lantis.Pool;
using Lantis.Redis;
using Lantis.Redis.Message;
using Lantis.EntityComponentSystem;
using Lantis.Network;
using Lantis.Extend;

namespace Lantis.ReadisOperation
{
    public static class MemoryReadisOperation
    {
        private static List<Type> tableList = new List<Type>
        {
           typeof(TestRedisDataSleep3)
        };

        private static IDSpawner idSpawner = new IDSpawner();

        public static List<DbParameter> GetParameterList(params DbParameter[] dbParameters)
		{
            List<DbParameter> dbParameterList = null;

            if (dbParameters != null)
            {
                dbParameterList = new List<DbParameter>();

                foreach (var dbParameter in dbParameters)
                {
                    dbParameterList.Add(dbParameter);
                }
            }

            return dbParameterList;
        }

        public static void ExecuteNonQuery(string sqlComd, Action<object> finishCall,params DbParameter[] dbParameters)
        {
            var request = LantisPoolSystem.GetPool<RequestRedisSqlCommand>().NewObject();
            request.sqlCmd = sqlComd;
            SubmitExecuteNonQuery(request, finishCall, GetParameterList(dbParameters));
        }

        public static void ExecuteDataQuery(string sqlComd, Action<object> finishCall, params DbParameter[] dbParameters)
        {
            var request = LantisPoolSystem.GetPool<RequestRedisSqlCommand>().NewObject();
            request.sqlCmd = sqlComd;
            SubmitExecuteData(request, finishCall, GetParameterList(dbParameters));
        }

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

        public static void SingleSelectData<T>(string fieldName,string operation,object value, Action<object> finisCallBack)
        {
            var redisRequest = LantisPoolSystem.GetPool<RequestRedisGet>().NewObject();
            redisRequest.tableName = RedisCore.GetTypeName<T>();
            var condition = LantisPoolSystem.GetPool<LantisRedisCondition>().NewObject();
            redisRequest.conditionGroup.conditionList.Add(condition);
            condition.fieldName = fieldName;
            condition.operation = operation;
            condition.fieldValue = RedisCore.GetStringValueObject(value);
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

        public static void SubmitExecuteNonQuery(RequestRedisSqlCommand request, Action<object> finishCallBack, List<DbParameter> parameterList = null)
        {
            request.requestId = idSpawner.GetId();
            NetCallbackSystem.AddNetCallback(request.requestId, finishCallBack);
            request.executeType = 0;
            request.dbParameterList = parameterList;
            var data = RedisSerializable.Serialize(request);
            LogicTrunkEntity.Instance.GetComponent<NetClientBranch>().GetComponent<NetClientComponents>().SendMessage(MessageIdDefine.ExecuteCommand, data);
        }

        public static void SubmitExecuteData(RequestRedisSqlCommand request, Action<object> finishCallBack, List<DbParameter> parameterList = null)
        {
            request.requestId = idSpawner.GetId();
            NetCallbackSystem.AddNetCallback(request.requestId, finishCallBack);
            request.executeType = 1;
            request.dbParameterList = parameterList;
            var data = RedisSerializable.Serialize(request);
            LogicTrunkEntity.Instance.GetComponent<NetClientBranch>().GetComponent<NetClientComponents>().SendMessage(MessageIdDefine.ExecuteCommand, data);
        }

        public static void SubmitRequestRedisCheck(RequestRedisCheckDatabase request, Action<object> finisCallBack)
        {
            request.requestId = idSpawner.GetId();
            NetCallbackSystem.AddNetCallback(request.requestId, finisCallBack);
            var data = RedisSerializable.Serialize(request);
            LogicTrunkEntity.Instance.GetComponent<NetClientBranch>().GetComponent<NetClientComponents>().SendMessage(MessageIdDefine.CheckDatabase,data);
        }

        public static void SubmitRequestRedisSet(RequestRedisSet request, Action<object> finisCallBack)
        {
            request.requestId = idSpawner.GetId();
            NetCallbackSystem.AddNetCallback(request.requestId, finisCallBack);
            var data = RedisSerializable.Serialize(request);
            LogicTrunkEntity.Instance.GetComponent<NetClientBranch>().GetComponent<NetClientComponents>().SendMessage(MessageIdDefine.SetData, data);
        }

        public static void SubmitRequestRedisGet(RequestRedisGet request, Action<object> finisCallBack)
        {
            request.requestId = idSpawner.GetId();
            NetCallbackSystem.AddNetCallback(request.requestId, finisCallBack);
            var data = RedisSerializable.SerializableToBytes(request);
            LogicTrunkEntity.Instance.GetComponent<NetClientBranch>().GetComponent<NetClientComponents>().SendMessage(MessageIdDefine.GetData, data);
        }

        public static void SubmitRequestRedisGetPage(RequestRedisGetPage request, Action<object> finishCallBack)
        {
            request.requestId = idSpawner.GetId();
            NetCallbackSystem.AddNetCallback(request.requestId, finishCallBack);
        }
    }
}
