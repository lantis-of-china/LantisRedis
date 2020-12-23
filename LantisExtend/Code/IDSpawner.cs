using System;
using System.Collections.Generic;
using System.Text;

namespace Lantis.Extend
{
    public class IDSpawner
    {
        private object lockHandle = new object();
        public List<int> spawnerList = new List<int>();
        private int id;

        public IDSpawner()
        {
            id = 0;
        }

        public int GetId()
        {
            lock (lockHandle)
            {
                if (spawnerList.Count > 0)
                {
                    var id = spawnerList[0];
                    spawnerList.RemoveAt(0);
                    return id;
                }
                else
                {
                    if (id < int.MaxValue)
                    {
                        id++;
                        return id;
                    }
                    else
                    {
                        Logger.Error("id is none");
                        return 0;
                    }
                }
            }
        }

        public void CollectId(int id)
        {
            lock (lockHandle)
            {
                spawnerList.Add(id);
            }
        }
    }
}
