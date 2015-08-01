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
            container.RegisterType<IOrderStatusManager, OrderStatusManager>(new PerRequestLifetimeManager());
            container.RegisterType<IOrderTypeManager, OrderTypeManager>(new PerRequestLifetimeManager());
            container.RegisterType<IPriceManager, PriceManager>(new PerRequestLifetimeManager());
            container.RegisterType<IProductManager, ProductManager>(new PerRequestLifetimeManager()); 
            container.RegisterType<ILargeCustomerRequiredFieldManager, LargeCustomerRequiredFieldManager>(new PerRequestLifetimeManager()); 
            container.RegisterType<ILocationManager, LocationManager>(new PerRequestLifetimeManager()); 
            container.RegisterType<IInvoiceManager, InvoiceManager>(new PerRequestLifetimeManager()); 
            container.RegisterType<IInvoiceItemAccountItemManager, InvoiceItemAccountItemManager>(new PerRequestLifetimeManager()); 
            container.RegisterType<IInvoiceItemManager, InvoiceItemManager>(new PerRequestLifetimeManager());
            container.RegisterType<IAdressManager, AdressManager>(new PerRequestLifetimeManager()); 
            container.RegisterType<ICostCenterManager, CostCenterManager>(new PerRequestLifetimeManager()); 
            container.RegisterType<ICustomerManager, CustomerManager>(new PerRequestLifetimeManager()); 
            container.RegisterType<IVehicleManager, VehicleManager>(new PerRequestLifetimeManager()); 
            container.RegisterType<IRegistrationLocationManager, RegistrationLocationManager>(new PerRequestLifetimeManager()); 
            container.RegisterType<IRegistrationManager, RegistrationManager>(new PerRequestLifetimeManager()); 
            container.RegisterType<IRegistrationOrderTypeManager, RegistrationOrderTypeManager>(new PerRequestLifetimeManager()); 
            container.RegisterType<IRegistrationOrderManager, RegistrationOrderManager>(new PerRequestLifetimeManager());
            container.RegisterType<IDeregistrationOrderManager, DeregistrationOrderManager>(new PerRequestLifetimeManager());
            container.RegisterType<IContactManager, ContactManager>(new PerRequestLifetimeManager()); 
            container.RegisterType<IBankAccountManager, BankAccountManager>(new PerRequestLifetimeManager()); 
            container.RegisterType<ICarOwnerManager, CarOwnerManager>(new PerRequestLifetimeManager()); 
        }
    }
}
