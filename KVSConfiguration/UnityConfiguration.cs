using KVSCommon.Managers;
using KVSDataAccess;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSConfiguration
{
    public static partial class UnityConfiguration
    {
        public static void ConfigureContainer(IUnityContainer container)
        {
            //container.RegisterType<IContainerStarEntities, ContainerStarEntities>(new PerRequestLifetimeManager(),
            //    new InjectionConstructor(new ResolvedParameter<IContainerStarSaveActorManager>(), ConfigurationManager.ConnectionStrings["ContainerStarEntities"].ConnectionString));

            container.RegisterType<IBicManager, BicManager>();//new PerRequestLifetimeManager());
        }
    }
}
