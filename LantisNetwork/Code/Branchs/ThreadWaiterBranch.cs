using System;
using System.Collections.Generic;
using System.Text;
using Lantis.EntityComponentSystem;
using Lantis.Network;
using Lantis.Pool;

namespace Lantis.Network
{
    public class ThreadWaiterBranch : ComponentEntity
    {
        public override T AddComponentEntity<T>(params object[] paramsData)
        {
            return base.AddComponentEntity<T>(paramsData);
        }

        public ThreadWaiter GetThreadWaiter()
        {
            return EntitySystem.CreateEntity<ThreadWaiter>();
        }

        public void RunWaiter(ThreadWaiter threadWaiter)
        {
            threadWaiter.Run();
            EntitySystem.Destroy(threadWaiter);
        }
    }
}
