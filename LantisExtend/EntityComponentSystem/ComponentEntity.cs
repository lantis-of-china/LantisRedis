using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponentSystem
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
    }
}
