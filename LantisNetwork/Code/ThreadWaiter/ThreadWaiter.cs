using System;
using System.Collections.Generic;
using System.Text;
using Lantis.EntityComponentSystem;
using System.Threading;

namespace Lantis.Network
{
    public class ThreadWaiter : Entity
    {
        private bool waitState;

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);

            SafeRun(delegate
            {
                waitState = true;
            });
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            SafeRun(delegate
            {
                waitState = true;
            });
        }

        public void Stop()
        {
            SafeRun(delegate
            {
                waitState = false;
            });
        }

        public void Run()
        {
            while (waitState)
            {
                Thread.Sleep(5);
            }
        }
    }
}
