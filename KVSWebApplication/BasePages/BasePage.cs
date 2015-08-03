using KVSCommon.Database;
using KVSCommon.Enums;
using KVSCommon.Managers;
using Microsoft.Practices.Unity;
using System;
using System.Collections;
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
    ///  BasePage class
    /// </summary>
    public abstract class BasePage : Page
    {
        #region Members

        protected IEnumerable<OrderStatus> OrderStatuses { get; set; }
        protected IEnumerable<OrderType> OrderTypesCollection { get; set; }
        protected IEnumerable<ProductCategory> ProductCategoryCollection { get; set; }
        protected IEnumerable<RegistrationOrderType> RegistrationOrderTypeCollection { get; set; }
        

        PageStatePersister _pers;
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                if (_pers == null)
                {
                    _pers = new SessionPageStatePersister(Page);
                }
                return _pers;
            }
        }

        public BasePage()
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

        #region Methods

        protected IEnumerable GetOrderPositions(string orderNumberStr)
        {
            var orderNumber = Int32.Parse(orderNumberStr);
            var positions = OrderManager.GetOrderItems(orderNumber).Where(o => !o.SuperOrderItemId.HasValue).ToList();
            var positionIds = positions.Select(o => o.Id);
            var authChargePositions = OrderManager.GetOrderItems().Where(o =>
                o.SuperOrderItemId.HasValue && positionIds.Contains(o.SuperOrderItemId.Value)).ToList();

            return positions.
                 Select(ordItem =>
                 {
                     var authCharge = authChargePositions.FirstOrDefault(o => o.SuperOrderItemId == ordItem.Id);

                     return new
                     {
                         OrderItemId = ordItem.Id,
                         Amount = ordItem.Amount == 0 ? "kein Preis" : (Math.Round(ordItem.Amount, 2, MidpointRounding.AwayFromZero)).ToString(),
                         ProductName = ordItem.IsAuthorativeCharge ? ordItem.ProductName + " (Amtl.Gebühr)" : ordItem.ProductName,
                         AmtGebuhr = authCharge == null ? false : true,
                         AuthCharge = authCharge == null || authCharge.Amount == 0 ? "kein Preis" : (Math.Round(authCharge.Amount, 2, MidpointRounding.AwayFromZero)).ToString(),
                         AuthChargeId = authCharge == null ? (int?)null : authCharge.Id,
                     };
                 }).ToList();
        }

        protected IEnumerable<OrderViewModel> GetSmallCustomerOrders()
        {
            return OrderManager.GetEntities(o => o.Customer.SmallCustomer != null &&
                (o.RegistrationOrder != null || o.DeregistrationOrder != null)).Select(ord =>
                {
                    Vehicle vehicle = null;
                    Registration registration = null;
                    string licenceNumber = String.Empty;
                    string previousLicenceNumber = String.Empty;

                    if (ord.RegistrationOrder != null)
                    {
                        vehicle = ord.RegistrationOrder.Vehicle;
                        registration = ord.RegistrationOrder.Registration;
                        licenceNumber = ord.RegistrationOrder.Licencenumber;
                        previousLicenceNumber = ord.RegistrationOrder.PreviousLicencenumber;
                    }
                    else
                    {
                        vehicle = ord.DeregistrationOrder.Vehicle;
                        registration = ord.DeregistrationOrder.Registration;
                        licenceNumber = registration.Licencenumber;
                    }

                    return new OrderViewModel()
                    {
                        OrderNumber = ord.OrderNumber,
                        customerId = ord.CustomerId,
                        CreateDate = ord.CreateDate,
                        Status = OrderStatuses.FirstOrDefault(o => o.Id == ord.Status).Name,
                        CustomerName = ord.Customer.SmallCustomer.Person != null ?
                            ord.Customer.SmallCustomer.Person.FirstName + "  " + ord.Customer.SmallCustomer.Person.Name : ord.Customer.Name,
                        CustomerLocation = "",
                        Kennzeichen = licenceNumber,
                        VIN = vehicle.VIN,
                        TSN = vehicle.TSN,
                        HSN = vehicle.HSN,
                        OrderTyp = OrderTypesCollection.FirstOrDefault(o => o.Id == ord.OrderTypeId).Name,
                        Freitext = ord.FreeText,
                        Geprueft = ord.Geprueft == null ? "Nein" : "Ja",
                        Datum = registration.RegistrationDate,
                        HasError = ord.HasError,
                        ErrorReason = ord.ErrorReason,

                        OrderStatusId = ord.Status,
                        OrderTypeId = ord.OrderTypeId,
                        ReadyToSend = ord.ReadyToSend,

                        Inspection = registration.GeneralInspectionDate,
                        Variant = registration.Vehicle.Variant,
                        eVBNum = registration.eVBNumber,
                        Name = registration.CarOwner.Name,
                        FirstName = registration.CarOwner.FirstName,
                        BankName = registration.CarOwner.BankAccount.BankName,
                        AccountNum = registration.CarOwner.BankAccount.Accountnumber,
                        Prevkennzeichen = previousLicenceNumber,
                        BankCode = registration.CarOwner.BankAccount.BankCode,
                        Street = registration.CarOwner.Adress.Street,
                        StreetNr = registration.CarOwner.Adress.StreetNumber,
                        Zip = registration.CarOwner.Adress.Zipcode,
                        City = registration.CarOwner.Adress.City,
                        Country = registration.CarOwner.Adress.Country,
                        Phone = registration.CarOwner.Contact.Phone,
                        Mobile = registration.CarOwner.Contact.MobilePhone,
                        Fax = registration.CarOwner.Contact.Fax,
                        Email = registration.CarOwner.Contact.Email,

                        VisibleWeiterleitung = (ord.Status == (int)OrderStatusTypes.Payed || ord.Status == (int)OrderStatusTypes.Cancelled || ord.Status == (int)OrderStatusTypes.AdmissionPoint) ? false : true,
                        ZumAuftragText = ord.Status == (int)OrderStatusTypes.Payed ? "Schon abgerechnet" : ord.Status == (int)OrderStatusTypes.Cancelled ? "Schon storniert" : "Zum Auftrag",
                        Haltername = registration.CarOwner.Name != String.Empty && registration.CarOwner.FirstName == String.Empty
                                  ? registration.CarOwner.Name : registration.CarOwner.FirstName,

                        HasErrorAsString = ord.HasError == true ? "Ja" : "Nein",
                    };
                });
        }

        protected IEnumerable<OrderViewModel> GetLargeCustomerOrders()
        {
            return OrderManager.GetEntities(o => o.Customer.LargeCustomer != null &&
                (o.RegistrationOrder != null || o.DeregistrationOrder != null)).Select(ord =>
                {
                    Vehicle vehicle = null;
                    Registration registration = null;
                    string licenceNumber = String.Empty;
                    string previousLicenceNumber = String.Empty;

                    if (ord.RegistrationOrder != null)
                    {
                        vehicle = ord.RegistrationOrder.Vehicle;
                        registration = ord.RegistrationOrder.Registration;
                        licenceNumber = ord.RegistrationOrder.Licencenumber;
                        previousLicenceNumber = ord.RegistrationOrder.PreviousLicencenumber;
                    }
                    else
                    {
                        vehicle = ord.DeregistrationOrder.Vehicle;
                        registration = ord.DeregistrationOrder.Registration;
                        licenceNumber = registration.Licencenumber;
                    }

                    return new OrderViewModel()
                    {
                        OrderNumber = ord.OrderNumber,
                        locationId = ord.LocationId.HasValue ? ord.LocationId.Value : 0,
                        customerId = ord.CustomerId,
                        CreateDate = ord.CreateDate,
                        Status = OrderStatuses.FirstOrDefault(o => o.Id == ord.Status).Name,
                        CustomerName = ord.Customer.Name,
                        CustomerLocation = ord.Location != null ? ord.Location.Name : String.Empty,
                        Kennzeichen = licenceNumber,
                        VIN = vehicle.VIN,
                        TSN = vehicle.TSN,
                        HSN = vehicle.HSN,
                        OrderTyp = OrderTypesCollection.FirstOrDefault(o => o.Id == ord.OrderTypeId).Name,
                        Freitext = ord.FreeText,
                        Geprueft = ord.Geprueft == null ? "Nein" : "Ja",
                        Datum = registration.RegistrationDate,
                        HasError = ord.HasError,
                        ErrorReason = ord.ErrorReason,

                        OrderStatusId = ord.Status,
                        OrderTypeId = ord.OrderTypeId,
                        ReadyToSend = ord.ReadyToSend,

                        Inspection = registration.GeneralInspectionDate,
                        Variant = registration.Vehicle.Variant,
                        eVBNum = registration.eVBNumber,
                        Name = registration.CarOwner.Name,
                        FirstName = registration.CarOwner.FirstName,
                        BankName = registration.CarOwner.BankAccount.BankName,
                        AccountNum = registration.CarOwner.BankAccount.Accountnumber,
                        Prevkennzeichen = previousLicenceNumber,
                        BankCode = registration.CarOwner.BankAccount.BankCode,
                        Street = registration.CarOwner.Adress.Street,
                        StreetNr = registration.CarOwner.Adress.StreetNumber,
                        Zip = registration.CarOwner.Adress.Zipcode,
                        City = registration.CarOwner.Adress.City,
                        Country = registration.CarOwner.Adress.Country,
                        Phone = registration.CarOwner.Contact.Phone,
                        Mobile = registration.CarOwner.Contact.MobilePhone,
                        Fax = registration.CarOwner.Contact.Fax,
                        Email = registration.CarOwner.Contact.Email,

                        //PostBackUrl = (ord.DocketList != null) ? "<a href=" + '\u0022' + ord.DocketList..FileName + '\u0022' + " target=" + '\u0022' + "_blank" + '\u0022' + "> Laufzettel öffnen</a>" : "",
                        VisibleWeiterleitung = (ord.Status == (int)OrderStatusTypes.Payed || ord.Status == (int)OrderStatusTypes.Cancelled || ord.Status == (int)OrderStatusTypes.AdmissionPoint) ? false : true,
                        ZumAuftragText = ord.Status == (int)OrderStatusTypes.Payed ? "Schon abgerechnet" : ord.Status == (int)OrderStatusTypes.Cancelled ? "Schon storniert" : "Zum Auftrag",
                        Haltername = registration.CarOwner.Name != String.Empty && registration.CarOwner.FirstName == String.Empty
                                  ? registration.CarOwner.Name : registration.CarOwner.FirstName,

                        HasErrorAsString = ord.HasError == true ? "Ja" : "Nein",
                    };
                });
        }

        #endregion
    }
}