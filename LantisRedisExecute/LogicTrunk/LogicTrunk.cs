using Lantis.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;
using Lantis.EntityComponentSystem;
using Lantis.Extend;

namespace Lantis
{
    public class LogicTrunkEntity : Entity
    {
        private static LogicTrunkEntity instance;

        public static LogicTrunkEntity Instance
        {
            get
            {                
                if (instance == null)
                {
                    instance = EntitySystem.CreateEntity<LogicTrunkEntity>();
                }

                return instance;
            }
        }

        private LantisDictronaryList<Type, Entity> components;

        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();

            SafeRun(delegate
            {
                components = LantisPoolSystem.GetPool<LantisDictronaryList<Type, Entity>>().NewObject();
            });
        }


        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();

            SafeRun(delegate
            {
                LantisPoolSystem.GetPool<LantisDictronaryList<Type, Entity>>().DisposeObject(components);
                components = null;
            });
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);
        }

        public override T AddComponentEntity<T>(params object[] paramsData)
        {
            var component = base.AddComponentEntity<T>(paramsData);

            return SafeRunFunction<T>(new Func<T>(delegate
            {
                components.AddValue(typeof(T), component);

                return component;
            }));
        }

        public override void RemoveComponentEntity<T>(T component)
        {
            base.RemoveComponentEntity<T>(component);

            SafeRun(delegate
            {
                components.RemoveKey(typeof(T));
            });
        }
    }
}
