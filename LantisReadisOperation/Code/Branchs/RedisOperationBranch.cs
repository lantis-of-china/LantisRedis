using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Redis;
using Lantis.Pool;
using Lantis.EntityComponentSystem;
using System.Threading;
using Lantis.Network;
using Lantis.Redis.Message;

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

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);

            netBranch = LogicTrunkEntity.Instance.AddComponentEntity<NetClientBranch>(NetClientBranch.ParamCreate(
            "127.0.0.1",
            9980,
            new string[] { "Lantis.ReadisOperation.NetProcess" },
            () =>
            {
                Logger.Error("socket connect finish");
                MemoryReadisOperation.CheckTable();
                Thread.Sleep(1000);
                var testData = new TestRedisDataSleep3();
                testData.id = "12";
                MemoryReadisOperation.SetData(testData.id, testData, null);
            },
            () =>
            {
                Logger.Error("socket exception");
            }));
        }

        public void CheckTable(RequestRedisCheckDatabase request,Action<object> finishCall)
        {
            MemoryReadisOperation.SubmitRequestRedisCheck(request, finishCall);
        }

        public void GetData<T>(int id, Action<object> finishCall) where T : RedisBase
        {
            MemoryReadisOperation.GetData<T>(id, finishCall);
        }

        public void SetData<T>(object id,T data, Action<object> finishCall) where T : RedisBase
        {
            MemoryReadisOperation.SetData<T>(id, data,finishCall);
        }

        public void ExecuteNonQuery(string sqlComd, Action<object> finishCall)
        {
            
        }
    }
}
