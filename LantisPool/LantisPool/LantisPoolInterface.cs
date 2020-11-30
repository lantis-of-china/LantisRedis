using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LantisPool
{
    public interface LantisPoolInterface
    {
        void OnCreate();

        void OnEnable();

        void OnDisable();

        void OnDestroy();
    }
}
