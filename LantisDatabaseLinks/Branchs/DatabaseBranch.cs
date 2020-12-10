using System;
using System.Collections.Generic;
using System.Text;
using Lantis.EntityComponentSystem;

namespace Lantis.DatabaseLinks
{
    public class DatabaseBranch : BranchEntity
    {
        private DatabaseCoreComponents databaseCoreComponent;
        public DatabaseCoreComponents DatabaseCoreComponent
        {
            get 
            {
                return databaseCoreComponent;
            }
        }

        public override void OnPoolSpawn()
        {
            base.OnPoolSpawn();
        }

        public override void OnPoolDespawn()
        {
            base.OnPoolDespawn();
        }

        public static object[] ParamCreate(string sqlLink)
        {
            return new object[] { sqlLink};
        }

        public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);

            SafeRun(delegate
            {
                var sqlLink = paramsData[0] as string;
                databaseCoreComponent = AddComponentEntity<DatabaseCoreComponents>(DatabaseCoreComponents.ParamCreate(sqlLink));
            });
        }
    }
}
