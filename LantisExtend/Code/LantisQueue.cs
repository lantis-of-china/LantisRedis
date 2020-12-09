using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.Extend
{
    public class LantisQueue<T> : LantisPoolInterface
    {
        private object lockHandle;
        private Queue<T> queue;

        public LantisQueue()
        {
            lockHandle = new object();
            queue = new Queue<T>();
        }

        public void OnPoolSpawn()
        {
        }

        public void OnPoolDespawn()
        {
            Clear();
        }

        public void Enqueue(T data)
        {
            lock (lockHandle)
            {
                queue.Enqueue(data);
                
            }
        }

        public T Dequeue()
        {
            lock (lockHandle)
            {
                return queue.Dequeue();
            }
        }

        public void DequeueAll(Action<T> dequeueAction)
        {
            lock (lockHandle)
            {
                while (queue.Count > 0)
                {
                    dequeueAction(queue.Dequeue());
                }
            }
        }

        public int Count()
        {
            lock (lockHandle)
            {
                return queue.Count;
            }
        }

        public void Clear()
        {
            lock (lockHandle)
            {
                queue.Clear();
            }
        }
    }
}
