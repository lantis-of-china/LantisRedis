using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.EntityComponentSystem
{
    public class ComponentEntity : Entity
    {
        private Entity bindEntity;

        public void SetEntity(Entity entity)
        {
            bindEntity = entity;
        }

        public T GetEntity<T>() where T : Entity
        {
            return bindEntity as T;
        }

        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();

            if (bindEntity != null)
            {
                bindEntity.BackPoolSelf();
                bindEntity = null;
            }
        }

        public override void BackPoolSelf()
        {
            LantisPoolSystem.GetPool<ComponentEntity>().DisposeObject(this);
        }
    }
}
