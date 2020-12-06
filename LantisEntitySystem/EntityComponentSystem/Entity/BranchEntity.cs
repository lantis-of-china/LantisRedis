using Lantis.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lantis.EntityComponentSystem
{
    public class BranchEntity : ComponentEntity
    {
        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();
        }

        public override void BackPoolSelf()
        {
            LantisPoolSystem.GetPool<BranchEntity>().DisposeObject(this);
        }
    }
}
