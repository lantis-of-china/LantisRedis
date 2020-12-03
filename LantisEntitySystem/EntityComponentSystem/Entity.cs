using Lantis.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;
using Lantis.Locker;

namespace Lantis.EntityComponentSystem
{
    public class Entity : SafeLocker, LantisPoolInterface
    {
        private bool entityIdLock = false;
        private int entityId;
        private bool selfActive;
        private LantisDictronaryList<int,ComponentEntity> entityList;

        public Entity()
        { }

        public void SetEntityId(int id)
        {
            if (entityIdLock)
            {
                return;
            }

            entityIdLock = true;
            entityId = id;
        }

        public int GetEntityId()
        {
            return entityId;
        }

        public virtual void OnPoolSpawn()
        {
            entityList = LantisPoolSystem.GetPool<LantisDictronaryList<int, ComponentEntity>>().NewObject();
        }

        public virtual void OnPoolDespawn()
        {
            LantisPoolSystem.GetPool<LantisDictronaryList<int, ComponentEntity>>().DisposeObject(entityList);
            entityList = null;
        }

        public virtual void BackPoolSelf()
        {
            LantisPoolSystem.GetPool<Entity>().DisposeObject(this);
        }

        public virtual void OnAwake()
        { }

        public virtual void OnEnable()
        { }

        public virtual void OnDisable()
        { }

        public virtual void OnDestroy()
        { }

        public virtual void SetActive(bool active)
        {
            selfActive = active;

            if (active)
            {
                if (!selfActive)
                {
                    EntitySystem.SetEnableAll(this);
                }
            }
            else
            {
                if (selfActive)
                {
                    EntitySystem.SetDisableAll(this);
                }
            }
        }

        public bool GetActive()
        {
            return selfActive;
        }

        public virtual void AddComponentEntity<T>() where T : ComponentEntity, new()
        {
            EntitySystem.CreateComponent<T>(this);
        }

        public virtual void RemoveComponentEntity<T>(T component) where T : ComponentEntity, new()
        {
            entityList.RemoveKey(component.GetEntityId());
            EntitySystem.Destroy<T>(component);
        }

        public void RemoveComponentEntityAll()
        {
            var valueList = entityList.ValueToList();

            for (var i = valueList.Count - 1; i >= 0; --i)
            {
                RemoveComponentEntity(valueList[i]);
            }
        }

        public List<ComponentEntity> GetComponentList()
        {
            return entityList.ValueToList();
        }

        public LantisDictronaryList<int, ComponentEntity> GetComponentsContenter()
        {
            return entityList;
        }
    }
}
