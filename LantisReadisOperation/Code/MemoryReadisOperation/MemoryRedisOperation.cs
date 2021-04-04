using System;
using System.Data.Common;
using System.Collections.Generic;
using Lantis.Pool;
using Lantis.Redis;
using Lantis.Redis.Message;
using Lantis.EntityComponentSystem;
using Lantis.Network;
using Lantis.Extend;
using System.Data.SqlClient;

namespace Lantis.ReadisOperation
{
    public static class MemoryReadisOperation
    {
        private static IDSpawner idSpawner = new IDSpawner();

        public static List<SqlParameter> GetParameterList(params SqlParameter[] dbParameters)
		{
            List<SqlParameter> dbParameterList = null;

            if (dbParameters != null)
            {
                dbParameterList = new List<SqlParameter>();

                foreach (var dbParameter in dbParameters)
                {
                    dbParameterList.Add(dbParameter);
                }
            }

            return dbParameterList;
        }

        public static void ExecuteNonQuery(string sqlComd, Action<object> finishCall,params SqlParameter[] dbParameters)
        {
            var request = LantisPoolSystem.GetPool<RequestRedisSqlCommand>().NewObject();
            request.sqlCmd = sqlComd;
            SubmitExecuteNonQuery(request, finishCall, GetParameterList(dbParameters));
        }

        public static void ExecuteDataQuery(string sqlComd, Action<object> finishCall, params SqlParameter[] dbParameters)
        {
            var request = LantisPoolSystem.GetPool<RequestRedisSqlCommand>().NewObject();
            request.sqlCmd = sqlComd;
            SubmitExecuteData(request, finishCall, GetParameterList(dbParameters));
        }

        public static void CheckTable(List<Type> types,Action<object> finishCall)
        {
            var checkDb = LantisPoolSystem.GetPool<RequestRedisCheckDatabase>().NewObject();

            for (var i = 0; i < types.Count; ++i)
            {
                var type = types[i];
                checkDb.redisTableFieldDefine.Add(RedisCore.GetTypeField(type));
            }

            SubmitRequestRedisCheck(checkDb, finishCall);
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
            var dataSerializeble = RedisSerializable.SerializableToBytes(data);
            redisRequest.data = CompressEncryption.CompressEncryptionData(dataSerializeble);
            var condition = LantisPoolSystem.GetPool<LantisRedisCondition>().NewObject();
            redisRequest.conditionGroup.conditionList.Add(condition);
            condition.fieldName = RedisConst.id;
            condition.operation = RedisConditionOperationType.Equal;
            condition.fieldValue = RedisCore.GetStringValueObject(id);
            SubmitRequestRedisSet(redisRequest, finisCallBack);
            LantisPoolSystem.GetPool<RequestRedisSet>().DisposeObject(redisRequest);
        }

        public static void UpdateData<T>(Dictionary<string,object> updateDatas,LantisRedisCondition[] conditionArray, Action<object> finisCallBack) where T : RedisBase
        {
            var redisRequest = LantisPoolSystem.GetPool<RequestRedisUpdate>().NewObject();
            redisRequest.databaseName = RedisCore.GetDatabaseName<T>();
            redisRequest.tableName = RedisCore.GetTypeName<T>();

            for (var i = 0; i < conditionArray.Length; ++i)
            {
                redisRequest.conditionGroup.conditionList.Add(conditionArray[i]);
            }

            redisRequest.data = updateDatas;
            SubmitRequestRedisUpdate(redisRequest, finisCallBack);
            LantisPoolSystem.GetPool<RequestRedisUpdate>().DisposeObject(redisRequest);
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
            redisRequest.databaseName = RedisCore.GetDatabaseName<T>();
            redisRequest.tableName = RedisCore.GetTypeName<T>();
            var condition = LantisPoolSystem.GetPool<LantisRedisCondition>().NewObject();
            redisRequest.conditionGroup.conditionList.Add(condition);
            condition.fieldName = RedisConst.id;
            condition.operation = RedisConditionOperationType.Equal;
            condition.fieldValue = RedisCore.GetStringValueObject(id);
            SubmitRequestRedisGet(redisRequest, finisCallBack);
            LantisPoolSystem.GetPool<RequestRedisGet>().DisposeObject(redisRequest);
        }

        /// <summary>
        /// select data from by value equla
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        /// <param name="finisCallBack"></param>
        public static void SingleSelectData<T>(string fieldName,string operation,object value, Action<object> finisCallBack)
        {
            var redisRequest = LantisPoolSystem.GetPool<RequestRedisGet>().NewObject();
            redisRequest.databaseName = RedisCore.GetDatabaseName<T>();
            redisRequest.tableName = RedisCore.GetTypeName<T>();
            var condition = LantisPoolSystem.GetPool<LantisRedisCondition>().NewObject();
            redisRequest.conditionGroup.conditionList.Add(condition);
            condition.fieldName = fieldName;
            condition.operation = operation;
            condition.fieldValue = RedisCore.GetStringValueObject(value);
            SubmitRequestRedisGet(redisRequest, finisCallBack);
            LantisPoolSystem.GetPool<RequestRedisGet>().DisposeObject(redisRequest);
        }

        /// <summary>
        /// select data from by conditions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        /// <param name="finisCallBack"></param>
        public static void SelectGetData<T>(LantisRedisConditionGroup conditionGroup, Action<object> finisCallBack)
        {
            var redisRequest = LantisPoolSystem.GetPool<RequestRedisGet>().NewObject();
            redisRequest.databaseName = RedisCore.GetDatabaseName<T>();
            redisRequest.tableName = RedisCore.GetTypeName<T>();
            var condition = LantisPoolSystem.GetPool<LantisRedisCondition>().NewObject();
            redisRequest.conditionGroup = conditionGroup;
            SubmitRequestRedisGet(redisRequest, finisCallBack);
            LantisPoolSystem.GetPool<RequestRedisGet>().DisposeObject(redisRequest);
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
            LantisPoolSystem.GetPool<RequestRedisGetPage>().DisposeObject(redisRequest);
        }

        /// <summary>
        /// submit execute non query to the redis
        /// </summary>
        /// <param name="request"></param>
        /// <param name="finishCallBack"></param>
        /// <param name="parameterList"></param>
        public static void SubmitExecuteNonQuery(RequestRedisSqlCommand request, Action<object> finishCallBack, List<SqlParameter> parameterList = null)
        {
            request.requestId = idSpawner.GetId();
            NetCallbackSystem.AddNetCallback(request.requestId, finishCallBack);
            request.executeType = 0;
            request.dbParameterList = parameterList;
            var data = RedisSerializable.Serialize(request);
            LogicTrunkEntity.Instance.GetComponent<NetClientBranch>().GetComponent<NetClientComponents>().SendMessage(MessageIdDefine.ExecuteCommand, data);
        }

        /// <summary>
        /// submit execute command to the redis
        /// </summary>
        /// <param name="request"></param>
        /// <param name="finishCallBack"></param>
        /// <param name="parameterList"></param>
        public static void SubmitExecuteData(RequestRedisSqlCommand request, Action<object> finishCallBack, List<SqlParameter> parameterList = null)
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

        public static void SubmitRequestRedisUpdate(RequestRedisUpdate request, Action<object> finisCallBack)
        {
            request.requestId = idSpawner.GetId();
            NetCallbackSystem.AddNetCallback(request.requestId, finisCallBack);
            var data = RedisSerializable.Serialize(request);
            LogicTrunkEntity.Instance.GetComponent<NetClientBranch>().GetComponent<NetClientComponents>().SendMessage(MessageIdDefine.UpdateRedis, data);
        }

        public static void SubmitRequestRedisGet(RequestRedisGet request, Action<object> finisCallBack)
        {
            request.requestId = idSpawner.GetId();
            NetCallbackSystem.AddNetCallback(request.requestId, finisCallBack);
            var data = RedisSerializable.Serialize(request);
            LogicTrunkEntity.Instance.GetComponent<NetClientBranch>().GetComponent<NetClientComponents>().SendMessage(MessageIdDefine.GetData, data);
        }

        public static void SubmitRequestRedisGetPage(RequestRedisGetPage request, Action<object> finishCallBack)
        {
            request.requestId = idSpawner.GetId();
            NetCallbackSystem.AddNetCallback(request.requestId, finishCallBack);
        }

        public static LantisRedisCondition SpawnCondition(string fieldName,string operation,object value)
        {
            var condition = LantisPoolSystem.GetPool<LantisRedisCondition>().NewObject();
            condition.fieldName = fieldName;
            condition.operation = operation;

            try
            {
                condition.fieldValue = RedisCore.GetStringValueObject(value);
            }
            catch(Exception e)
            {
                Logger.Error(e.ToString());
            }

            return condition;
        }

        public static RedisTableData SerializableToRedisTableData(RedisSerializableData serializableData)
        {
            var redisTableData = LantisPoolSystem.GetPool<RedisTableData>().NewObject();
            return RedisCore.SerializableToRedisTableData(serializableData, redisTableData);
        }
    }
}
