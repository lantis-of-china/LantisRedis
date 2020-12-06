using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.Extend
{
    [Serializable]
    public class LantisList<T> : LantisPoolInterface
    {
        private object lockHandle;
        private List<T> listValue;

        public LantisList()
        {
            lockHandle = new object();
            listValue = new List<T>();
        }

        public void OnPoolSpawn()
        {
        }

        public void OnPoolDespawn()
        {
            Clear();
        }

        public int AddValue(T value)
        {
            lock (lockHandle)
            {
                listValue.Add(value);

                return listValue.Count;
            }
        }

        public bool HasValue(T value)
        {
            lock (lockHandle)
            {
                return listValue.Exists(item => item as object == value as object);
            }
        }

        public int Remove(T item)
        {
            lock (lockHandle)
            {
                listValue.Remove(item);

                return listValue.Count;
            }
        }

        public int RemoveAt(int index)
        {
            lock (lockHandle)
            {
                listValue.RemoveAt(index);

                return listValue.Count;
            }
        }

        public T this[int index]
        {
            get
            {
                lock (lockHandle)
                {
                    return listValue[index];
                }
            }
        }

        public void Clear()
        {
            lock (lockHandle)
            {
                listValue.Clear();
            }
        }

        public void SafeWhile(Action<T> callfun)
        {
            lock (lockHandle)
            {
                for (var i = 0; i < listValue.Count; ++i)
                {
                    var value = listValue[i];
                    callfun(value);
                }
            }
        }

        public void SafeWhileBreak(Func<T, bool> callfun)
        {
            lock (lockHandle)
            {
                for (var i = 0; i < listValue.Count; ++i)
                {
                    var value = listValue[i];

                    if (!callfun(value))
                    {
                        return;
                    }
                }
            }
        }
    }
}