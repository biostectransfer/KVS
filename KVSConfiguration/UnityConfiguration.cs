using KVSCommon.Database;
using KVSCommon.Entities;
using KVSCommon.Managers;
using KVSDataAccess;
using KVSDataAccess.Managers;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSConfiguration
{
    public static partial class UnityConfiguration
    {
        public static void ConfigureContainer(IUnityContainer container)
        {
            //TODO add save actors
            //container.RegisterType<IContainerStarEntities, ContainerStarEntities>(new PerRequestLifetimeManager(),
            //    new InjectionConstructor(new ResolvedParameter<IContainerStarSaveActorManager>(), ConfigurationManager.ConnectionStrings["ContainerStarEntities"].ConnectionString));

            container.RegisterType<IKVSEntities, KVSEntities>(new PerRequestLifetimeManager(),
                new InjectionConstructor(ConfigurationManager.ConnectionStrings["KVSConnectionString"].ConnectionString));

            container.RegisterType<IBicManager, BicManager>(new PerRequestLifetimeManager());
            container.RegisterType<IUserManager, UserManager>(new PerRequestLifetimeManager());
            container.RegisterType<IOrderManager, OrderManager>(new PerRequestLifetimeManager());
            container.RegisterType<IPriceManager, PriceManager>(new PerRequestLifetimeManager());
            container.RegisterType<IProductManager, ProductManager>(new PerRequestLifetimeManager());
        }
    }
}
