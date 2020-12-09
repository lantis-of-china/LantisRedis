using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lantis.Locker
{
    public class SafeLocker
    {
        private const bool safeRun = true;
        private object lockHandle = new object();

        public void SafeRun(Action action)
        {
            if (safeRun)
            {
                lock (lockHandle)
                {
                    action();
                }
            }
            else
            {
                action();
            }
        }

        public A SafeRunFunction<A>(Func<A> action)
        {
            if (safeRun)
            {
                lock (lockHandle)
                {
                    return action();
                }
            }
            else
            {
                return action();
            }
        }
    }
}
