using Lantis.EntityComponentSystem;
using Lantis.Network;
using Lantis.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lantis.RedisExecute
{
    public class NetBranch : ComponentEntity
    {
        private string ip = "127.0.0.1";
        private int port = 9980;
        private NetMessageDriverComponents netMessageDriverComponent;
        private NetServerComponents netServerComponent;
        private Action serverSucess;
        private Action serverException;

        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();
        }

        public static object[] ParamCreate(Action serverSucess, Action serverException)
        {
            return new object[] { serverSucess, serverException };
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);

            SafeRun(delegate
            {
                serverSucess = paramsData[0] as Action;
                serverException = paramsData[1] as Action;
                netMessageDriverComponent = AddComponentEntity<NetMessageDriverComponents>(NetMessageDriverComponents.ParamCreate(Assembly.GetExecutingAssembly(),new string[] { "Lantis.RedisExecute.NetProcess" }));
                netServerComponent = AddComponentEntity<NetServerComponents>(NetServerComponents.ParamCreate(ip, port, null, null, OnServerSucess,OnServerException));
            });
        }

        private void OnServerSucess()
        {
            SafeRun(delegate
            {
                if (serverSucess != null)
                {
                    serverSucess();
                }
            });
        }

        private void OnServerException()
        {
            SafeRun(delegate
            {
                if (serverException != null)
                {
                    serverException();
                }
            });
        }
    }
}
