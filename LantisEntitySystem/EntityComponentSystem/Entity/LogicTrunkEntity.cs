using Lantis.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Extend;
using Lantis.Pool;

namespace Lantis.EntityComponentSystem
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
            try
            {
                var component = base.AddComponentEntity<T>(paramsData);

                return SafeRunFunction<T>(new Func<T>(delegate
                {
                    components.AddValue(typeof(T), component);

                    return component;
                }));
            }
            catch(Exception e)
            {
                Logger.Error(e.ToString());
            }
            finally
            {
            }

            return default(T);
        }

        public override void RemoveComponentEntity<T>(T component)
        {
            SafeRun(delegate
            {
                components.RemoveKey(typeof(T));
            });

            base.RemoveComponentEntity<T>(component);
        }
    }
}
