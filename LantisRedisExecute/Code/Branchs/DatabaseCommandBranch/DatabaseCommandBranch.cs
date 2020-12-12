using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Lantis.EntityComponentSystem;
using Lantis.Extend;
using Lantis.Pool;

namespace Lantis.RedisExecute
{
    public class DatabaseCommandBranch : BranchEntity
    {
        private LantisQueue<string> databaseCommandQueue;
        private Timer threadTimer;

        public DatabaseCommandBranch()
        {
            threadTimer = new Timer();
        }

        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();

            SafeRun(delegate
            {
                databaseCommandQueue = LantisPoolSystem.GetPool<LantisQueue<string>>().NewObject();
            });
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();

            SafeRun(delegate
            {
                LantisPoolSystem.GetPool<LantisQueue<string>>().DisposeObject(databaseCommandQueue);
                databaseCommandQueue.Clear();
            });
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);

            SafeRun(delegate
            {
                threadTimer.Interval = 10.0f;
                threadTimer.Elapsed += OnTimer;
                threadTimer.Start();
            });
        }

        public void AddCommand(string command)
        {
            SafeRun(delegate
            {
                databaseCommandQueue.Enqueue(command);
            });
        }

        public void OnTimer(object sender, ElapsedEventArgs e)
        {
            string command = "";

            SafeRun(delegate
            {
                if (databaseCommandQueue.Count() > 0)
                {
                    command = databaseCommandQueue.Dequeue();
                }
            });

            if (!string.IsNullOrEmpty(command))
            {
                Program.DatabaseBranch.DatabaseCoreComponent.ExecuteNonQuery(command);
            }
        }
    }
}
