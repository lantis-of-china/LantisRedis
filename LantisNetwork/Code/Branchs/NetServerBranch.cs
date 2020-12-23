using Lantis.EntityComponentSystem;
using Lantis.Network;
using Lantis.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lantis.Network
{
    public class NetServerBranch : ComponentEntity
    {
        private string ip = "";
        private int port = 0;
        private string[] processNameSpace;
        private NetMessageDriverComponents netMessageDriverComponent;
        public NetMessageDriverComponents NetMessageDriverComponent
        {
            get 
            {
                return netMessageDriverComponent;
            }
        }
        private NetServerComponents netServerComponent;
        public NetServerComponents NetServerComponent
        {
            get 
            {
                return netServerComponent;
            }
        }
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

        public static object[] ParamCreate(string ip, int port, string[] processNamespace, Action serverSucess, Action serverException)
        {
            return new object[] { ip,port,processNamespace, serverSucess, serverException };
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);

            SafeRun(delegate
            {
                ip = paramsData[0] as string;
                port = (int)paramsData[1];
                processNameSpace = paramsData[2] as string[];
                serverSucess = paramsData[3] as Action;
                serverException = paramsData[4] as Action;
                netMessageDriverComponent = AddComponentEntity<NetMessageDriverComponents>(NetMessageDriverComponents.ParamCreate(Assembly.GetExecutingAssembly(),processNameSpace));
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
