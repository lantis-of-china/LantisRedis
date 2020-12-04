using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.Extend
{
    [Serializable]
    public class LantisDictronaryList<K, V> : LantisPoolInterface
    {
        private object lockObject;
        private Dictionary<K, V> dictionary;
        private List<K> listKey;
        private List<V> listValue;

        public LantisDictronaryList()
        {
            lockObject = new object();
            dictionary = new Dictionary<K, V>();
            listKey = new List<K>();
            listValue = new List<V>();
        }

        public void OnPoolSpawn()
        {
        }

        public void OnPoolDespawn()
        {
            Clear();
        }

        public bool HasKey(K key)
        {
            lock (lockObject)
            {
                return dictionary.ContainsKey(key);
            }
        }

        public bool AddValue(K key, V value)
        {
            if (!HasKey(key))
            {
                lock (lockObject)
                {
                    dictionary.Add(key, value);
                    listKey.Add(key);
                    listValue.Add(value);
                }
            }

            return false;
        }

        public void RemoveKey(K key)
        {
            if (HasKey(key))
            {
                lock (lockObject)
                {
                    listKey.Remove(key);
                    listValue.Remove(dictionary[key]);
                    dictionary.Remove(key);
                }
            }
        }

        public List<K> KeyToList()
        {
            return listKey;
        }

        public List<V> ValueToList()
        {
            return listValue;
        }

        public V this[K key]
        {
            get
            {
                lock (lockObject)
                {
                    return dictionary[key];
                }
            }
        }

        public void Clear()
        {
            lock (lockObject)
            {
                dictionary.Clear();
                listKey.Clear();
                listValue.Clear();
            }
        }

        public void SafeWhile(Action<K,V> callfun)
        {
            lock (lockObject)
            {
                for (var i = 0; i < listKey.Count; ++i)
                {
                    var key = listKey[i];
                    var value = listValue[i];
                    callfun(key, value);
                }
            }
        }

        public void SafeWhileBreak(Func<K, V, bool> callfun)
        {
            lock (lockObject)
            {
                for (var i = 0; i < listKey.Count; ++i)
                {
                    var key = listKey[i];
                    var value = listValue[i];

                    if (!callfun(key, value))
                    {
                        return;
                    }
                }
            }
        }
    }
}