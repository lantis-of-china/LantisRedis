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

        private static NetBranch netBranch;
        public static NetBranch NetBranch 
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

        static void Main(string[] args)
        {
            databaseCommandBranch = LogicTrunkEntity.Instance.AddComponentEntity<DatabaseCommandBranch>();
            redisMemoryBranch = LogicTrunkEntity.Instance.AddComponentEntity<MemoryBranch>();
            databaseBranch = LogicTrunkEntity.Instance.AddComponentEntity<DatabaseBranch>(DatabaseBranch.ParamCreate("Data Source=140.143.94.127;User Id = loginsa;Password=lantis2019;Database=LantisRedis;Integrated Security = False;Connect Timeout = 30;Encrypt=False;TrustServerCertificate=False;"));
            netBranch = LogicTrunkEntity.Instance.AddComponentEntity<NetBranch>(NetBranch.ParamCreate(
            ()=> 
            {
                Logger.Log("server run sucess");

                LogicTrunkEntity.Instance.AddComponentEntity<RedisExecuteBranch>();
            },
            ()=> 
            {
                Logger.Error("server run exception");
            }));

            Console.ReadLine();
        }
    }
}
