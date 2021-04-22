using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis;
using Lantis.Network;
using System.Threading;
using Lantis.EntityComponentSystem;
using Lantis.DatabaseLinks;
using System.Diagnostics;
using System.Reflection;

namespace Lantis.RedisExecute
{
    public class Program
    {
        private static MemoryBranch redisMemoryBranch;
        public static MemoryBranch RedisMemoryBranch
        {
            get
            {
                return redisMemoryBranch;
            }
        }

        private static DatabaseBranch databaseBranch;
        public static DatabaseBranch DatabaseBranch
        {
            get 
            {
                return databaseBranch;
            }
        }

        private static NetServerBranch netBranch;
        public static NetServerBranch NetBranch 
        {
            get 
            {
                return netBranch;
            }
        }

        private static DatabaseCommandBranch databaseCommandBranch;
        public static DatabaseCommandBranch DatabaseCommandBranch
        {
            get 
            {
                return databaseCommandBranch;
            }
        }

        private static ThreadWaiterBranch threadWaiterBranch;
        public static ThreadWaiterBranch ThreadWaiterBranch
        {
            get 
            {
                return threadWaiterBranch;
            }
        }

        static void Main(string[] args)
        {
            threadWaiterBranch = LogicTrunkEntity.Instance.AddComponentEntity<ThreadWaiterBranch>();
            databaseCommandBranch = LogicTrunkEntity.Instance.AddComponentEntity<DatabaseCommandBranch>();
            redisMemoryBranch = LogicTrunkEntity.Instance.AddComponentEntity<MemoryBranch>();
            databaseBranch = LogicTrunkEntity.Instance.AddComponentEntity<DatabaseBranch>(DatabaseBranch.ParamCreate("Data Source=47.93.216.176;User Id = sa;Password=Lantis2021lantis.;Database=LantisRedisDatabase;Integrated Security = False;Connect Timeout = 30;Encrypt=False;TrustServerCertificate=False;"));
            netBranch = LogicTrunkEntity.Instance.AddComponentEntity<NetServerBranch>(NetServerBranch.ParamCreate(
            "0.0.0.0",
            9980,
            new string[] { "Lantis.RedisExecute.NetProcess" },
            ()=> 
            {
                Logger.Log("server run sucess");
                LogicTrunkEntity.Instance.AddComponentEntity<RedisExecuteBranch>();
            },
            ()=> 
            {
                Logger.Error("server run exception");
            },
            Assembly.GetExecutingAssembly()));

            var threadWaiter = ThreadWaiterBranch.GetThreadWaiter();
            ThreadWaiterBranch.RunWaiter(threadWaiter);
        }
    }
}
