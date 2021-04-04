using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Lantis.Redis;
using Lantis.Pool;
using Lantis.EntityComponentSystem;
using System.Threading;
using Lantis.Network;
using Lantis.Redis.Message;
using System.Data.SqlClient;
using System.Reflection;

namespace Lantis.ReadisOperation
{
    public class RedisOperationBranch :BranchEntity
    {
        private static NetClientBranch netBranch;
        public static NetClientBranch NetBranchHandle
        {
            get
            {
                return netBranch;
            }
        }

        public static object[] ParamCreate(string ip, int port,Action successCall,Action failedCall)
        {
            return new object[] { ip, port, successCall, failedCall };
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);
            var ip = paramsData[0] as string;
            var port = (int)paramsData[1];
            var successCall = paramsData[2] as Action;
            var failedCall = paramsData[3] as Action;
            netBranch = LogicTrunkEntity.Instance.AddComponentEntity<NetClientBranch>(NetClientBranch.ParamCreate(
            ip,
            port,
            new string[] { "Lantis.ReadisOperation.NetProcess" },
            successCall,
            failedCall,
            Assembly.GetAssembly(typeof(RedisOperationBranch))));            
        }

        public void CheckTable(List<Type> tableTypeList,Action<object> finishCall)
        {
            MemoryReadisOperation.CheckTable(tableTypeList, finishCall);
        }

        public void GetData<T>(int id, Action<object> finishCall) where T : RedisBase
        {
            MemoryReadisOperation.GetData<T>(id, finishCall);
        }

        public void SetData<T>(object id,T data, Action<object> finishCall) where T : RedisBase
        {
            MemoryReadisOperation.SetData<T>(id, data,finishCall);
        }

        public void UpdateData<T>(Dictionary<string, object> updateDatas, LantisRedisCondition[] conditionArray, Action<object> finisCallBack) where T : RedisBase
        {
            MemoryReadisOperation.UpdateData<T>(updateDatas, conditionArray, finisCallBack);
        }

        public void SingleSelectData<T>(string fieldName, string operation, object value, Action<object> finishCall)
        {
            MemoryReadisOperation.SingleSelectData<T>(fieldName, operation, value, finishCall);
        }

        public void SelectGetData<T>(LantisRedisConditionGroup conditionGroup, Action<object> finishCall)
        {
            MemoryReadisOperation.SelectGetData<T>(conditionGroup, finishCall);
        }

        public void ExecuteNonQuery(string sqlComd,Action<object> finishCall,params SqlParameter[] parameters)
        {
            MemoryReadisOperation.ExecuteNonQuery(sqlComd, finishCall, parameters);
        }

        public void ExecuteDataQuery(string sqlComd, Action<object> finishCall, params SqlParameter[] parameters)
        {
            MemoryReadisOperation.ExecuteDataQuery(sqlComd, finishCall, parameters);
        }

        public LantisRedisCondition SpawnCondition(string fieldName, string operation, object value)
        {
            return MemoryReadisOperation.SpawnCondition(fieldName, operation, value);
        }

        public RedisTableData SerializableToRedisTableData(RedisSerializableData serializableData)
        {
            return MemoryReadisOperation.SerializableToRedisTableData(serializableData);  
        }
    }
}
