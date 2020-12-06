using Lantis.EntityComponentSystem;
using Lantis.Network;
using Lantis.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lantis.RedisExecute
{
    public class BranchEntity : ComponentEntity
    {
        private string ip;
        private int port;
        private NetMessageDriverComponents netMessageDriverComponent;
        private NetServerComponents netServerComponent;

        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);

            SafeRun(delegate
            {
                netMessageDriverComponent = AddComponentEntity<NetMessageDriverComponents>("Lantis.RedisExecute.NetProcess");
                netServerComponent = AddComponentEntity<NetServerComponents>(NetServerComponents.ParamCreate(ip, port, null, null, OnServerException));
            });
        }

        private void OnServerException()
        {
            SafeRun(delegate
            {
            });
        }
    }
}
