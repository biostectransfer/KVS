using KVSCommon.Database;
using KVSCommon.Enums;
using KVSCommon.Managers;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace KVSWebApplication.BasePages
{
    /// <summary>
    ///  Base UserControl class
    /// </summary>
    public abstract class BaseUserControl : UserControl
    {
        #region Members

        protected IEnumerable<OrderStatus> OrderStatuses { get; set; }
        protected IEnumerable<OrderType> OrderTypesCollection { get; set; }
        protected IEnumerable<ProductCategory> ProductCategoryCollection { get; set; }
        protected IEnumerable<RegistrationOrderType> RegistrationOrderTypeCollection { get; set; }

        public BaseUserControl()
        {
            BicManager = (IBicManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IBicManager));
            UserManager = (IUserManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IUserManager));
            OrderManager = (IOrderManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IOrderManager));
            PriceManager = (IPriceManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IPriceManager));
            ProductManager = (IProductManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IProductManager));
            LargeCustomerRequiredFieldManager = (ILargeCustomerRequiredFieldManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ILargeCustomerRequiredFieldManager));
            LocationManager = (ILocationManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ILocationManager));
            InvoiceManager = (IInvoiceManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IInvoiceManager));
            InvoiceItemAccountItemManager = (IInvoiceItemAccountItemManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IInvoiceItemAccountItemManager));
            InvoiceItemManager = (IInvoiceItemManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IInvoiceItemManager));
            AdressManager = (IAdressManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAdressManager));
            CostCenterManager = (ICostCenterManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ICostCenterManager));
            CustomerManager = (ICustomerManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ICustomerManager));
            VehicleManager = (IVehicleManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IVehicleManager));
            RegistrationLocationManager = (IRegistrationLocationManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IRegistrationLocationManager));
            RegistrationManager = (IRegistrationManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IRegistrationManager));
            RegistrationOrderTypeManager = (IRegistrationOrderTypeManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IRegistrationOrderTypeManager));
            ContactManager = (IContactManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IContactManager));
            BankAccountManager = (IBankAccountManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IBankAccountManager));
            CarOwnerManager = (ICarOwnerManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ICarOwnerManager));
            RegistrationOrderManager = (IRegistrationOrderManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IRegistrationOrderManager));
            DeregistrationOrderManager = (IDeregistrationOrderManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IDeregistrationOrderManager));

            OrderTypeManager = (IOrderTypeManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IOrderTypeManager));
            OrderStatusManager = (IOrderStatusManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IOrderStatusManager));
            DocketListManager = (IDocketListManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IDocketListManager));
            PackingListManager = (IPackingListManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IPackingListManager));
            ProductCategoryManager = (IProductCategoryManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IProductCategoryManager));
            InvoiceTypesManager = (IInvoiceTypesManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IInvoiceTypesManager));

            OrderStatuses = OrderStatusManager.GetEntities().ToList();
            OrderTypesCollection = OrderTypeManager.GetEntities().ToList();
            ProductCategoryCollection = ProductCategoryManager.GetEntities().ToList();
            RegistrationOrderTypeCollection = RegistrationOrderTypeManager.GetEntities().ToList();
        }

        #endregion

        #region Managers

        public IBicManager BicManager { get; set; }
        public IUserManager UserManager { get; set; }
        public IOrderManager OrderManager { get; set; }
        public IOrderTypeManager OrderTypeManager { get; set; }
        public IOrderStatusManager OrderStatusManager { get; set; }
        public IPriceManager PriceManager { get; set; }
        public IProductManager ProductManager { get; set; }
        public ILargeCustomerRequiredFieldManager LargeCustomerRequiredFieldManager { get; set; }
        public ILocationManager LocationManager { get; set; }
        public IInvoiceManager InvoiceManager { get; set; }
        public IInvoiceItemAccountItemManager InvoiceItemAccountItemManager { get; set; }
        public IInvoiceItemManager InvoiceItemManager { get; set; }
        public IAdressManager AdressManager { get; set; }
        public ICostCenterManager CostCenterManager { get; set; }
        public ICustomerManager CustomerManager { get; set; }
        public IVehicleManager VehicleManager { get; set; }
        public IRegistrationLocationManager RegistrationLocationManager { get; set; }
        public IRegistrationManager RegistrationManager { get; set; }
        public IRegistrationOrderTypeManager RegistrationOrderTypeManager { get; set; }
        public IContactManager ContactManager { get; set; }
        public IBankAccountManager BankAccountManager { get; set; }
        public ICarOwnerManager CarOwnerManager { get; set; }
        public IRegistrationOrderManager RegistrationOrderManager { get; set; }
        public IDeregistrationOrderManager DeregistrationOrderManager { get; set; }
        public IDocketListManager DocketListManager { get; set; }
        public IPackingListManager PackingListManager { get; set; }
        public IProductCategoryManager ProductCategoryManager { get; set; }
        public IInvoiceTypesManager InvoiceTypesManager { get; set; }

        #endregion
    }
}