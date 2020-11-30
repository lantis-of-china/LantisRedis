using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LantisPool
{
    public class LantisPool<T> where T : LantisPoolInterface, new()
    {
        private object lockObject = new object();
        private List<T> poolList = new List<T>();
        private int generalCount;
        private int releseCount;
        private Timer timer;

        public void Init()
        {
            timer = new Timer();
            timer.Elapsed += CheckPool;
            timer.Interval = 1000;
            timer.Start();
        }

        public void SetPoolParamar(int generalCount, int releseCount)
        {
            this.generalCount = generalCount;
            this.releseCount = releseCount;
        }

        public T CreateObject()
        {
            var newObject = new T();
            newObject.OnCreate();

            return newObject;
        }

        public T NewObject()
        {
            lock (lockObject)
            {
                if (poolList.Count > 0)
                {
                    var targetObject = poolList[0];
                    poolList.RemoveAt(0);
                    targetObject.OnEnable();

                    return targetObject;
                }
            }

            var newObject = CreateObject();
            newObject.OnEnable();

            return newObject;
        }

        public void DisposeObject(T targetObject)
        {
            targetObject.OnDisable();

            lock (lockObject)
            {
                poolList.Add(targetObject);
            }
        }

        public void CheckPool(object sender, ElapsedEventArgs e)
        {
            lock (lockObject)
            {
                if (poolList.Count > generalCount)
                {
                    if (releseCount > 0)
                    {
                        for (var i = 0; i < releseCount; ++i)
                        {
                            var targetData = poolList[0];
                            poolList.RemoveAt(0);
                            targetData.OnDestroy();

                            if (poolList.Count <= generalCount)
                            {
                                break;
                            }
                        }
                    }
                }
                else if (poolList.Count < generalCount)
                {
                    while (poolList.Count < generalCount)
                    {
                        var newObject = CreateObject();
                        poolList.Add(newObject);
                    }
                }
            }
        }
    }
}
