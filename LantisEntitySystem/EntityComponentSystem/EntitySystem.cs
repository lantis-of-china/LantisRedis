using Lantis.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.EntityComponentSystem
{
    public class EntitySystem
    {
        private static int idRecord;
        private static List<int> idGc = new List<int>();
        private static LantisDictronaryList<long, Entity> entityList = new LantisDictronaryList<long, Entity>();

        private static int GetId()
        {
            if (idGc.Count > 0)
            {
                int backId = idGc[0];
                idGc.RemoveAt(0);

                return backId;
            }

            idRecord++;

            return idRecord;
        }

        private static void GcId(int id)
        {
            idGc.Add(id);
        }

        private static void SetEntityId(Entity entity)
        {
            entity.SetEntityId(GetId());
        }

        private static void AddEntity<T>(T entity,params object[] paramsData) where T : Entity
        {
            entityList.AddValue(entity.GetEntityId(), entity);
            entity.OnAwake(paramsData);
            entity.OnEnable();
        }

        private static void RemoveEntity(int entityId)
        {
            entityList.RemoveKey(entityId);
            GcId(entityId);
        }

        private static Entity GetEntity(int entityId)
        {
            if (entityList.HasKey(entityId))
            {
                return entityList[entityId];
            }

            return null;
        }

        public static T CreateEntity<T>(params object[] paramsData) where T :Entity,new()
        {
            var entity = LantisPoolSystem.GetPool<T>().NewObject();
            SetEntityId(entity);
            AddEntity<T>(entity, paramsData);
            return entity;
        }

        public static T CreateObject<T>(T objectInstance,params object[] paramsData) where T : ObjectEntity<T>,new()
        {
            var entity = LantisPoolSystem.GetPool<T>().NewObject();
            SetEntityId(entity);
            entity.objectInstance = objectInstance;
            AddEntity<T>(entity, paramsData);

            return entity;
        }

        public static T CreateComponent<T>(Entity baseEntity,params object[] paramsData) where T : ComponentEntity, new()
        {
            var entity = LantisPoolSystem.GetPool<T>().NewObject();
            SetEntityId(entity);
            entity.SetEntity(baseEntity);
            var components = baseEntity.GetComponentsContenter();
            components.AddValue(entity.GetEntityId(), entity);
            AddEntity<T>(entity, paramsData);

            return entity;
        }

        public static void Destroy<T>(T entity) where T : Entity, new()
        {
            if (GetEntity(entity.GetEntityId()) != null)
            {
                if (entity.GetActive())
                {
                    SetDisableAll(entity);
                }

                entity.OnDestroy();
                RemoveEntity(entity.GetEntityId());
                LantisPoolSystem.GetPool<T>().DisposeObject(entity);
            }
        }

        public static void SetEnableAll<T>(T entity) where T : Entity
        {
            entity.SetActive(true);
            entity.OnEnable();
            var componentList = entity.GetComponentList();

            for (var i = 0; i < componentList.Count; ++i)
            {
                var thisEntity = componentList[i];

                if (thisEntity.GetActive())
                {
                    thisEntity.OnEnable();
                }
            }
        }

        public static void SetDisableAll<T>(T entity) where T : Entity
        {
            entity.SetActive(false);
            entity.OnDisable();
            var componentList = entity.GetComponentList();

            for (var i = 0; i < componentList.Count; ++i)
            {
                var thisEntity = componentList[i];

                if (thisEntity.GetActive())
                {
                    thisEntity.OnDisable();
                }
            }
        }
    }
}
