﻿using KVSCommon.Database;
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

namespace KVSWebApplication.Auftragsbearbeitung_Neuzulassung
{
    /// <summary>
    /// View model for orders
    /// </summary>
    public class OrderViewModel
    {
        public int OrderNumber { get; set; }
        public int customerId { get; set; }
        public int locationId { get; set; }
        public DateTime CreateDate { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public string CustomerLocation { get; set; }
        public string Kennzeichen { get; set; }
        public string VIN { get; set; }
        public string TSN { get; set; }
        public string HSN { get; set; }
        public string OrderTyp { get; set; }
        public string Freitext { get; set; }
        public string Geprueft { get; set; }
        public DateTime? Datum { get; set; }
    }

    /// <summary>
    ///  Incoming Orders base class
    /// </summary>
    public abstract class EditOrdersBase : UserControl
    {
        #region Members

        public EditOrdersBase()
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
            OrderTypeManager = (IOrderTypeManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IOrderTypeManager));
            OrderStatusManager = (IOrderStatusManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IOrderStatusManager));

            OrderStatuses = OrderStatusManager.GetEntities().ToList();
            OrderTypesCollection = OrderTypeManager.GetEntities().ToList();
        }

        #region Common

        protected IEnumerable<OrderStatus> OrderStatuses { get; set; }
        protected IEnumerable<OrderType> OrderTypesCollection { get; set; }
        protected abstract PermissionTypes PagePermission { get; }
        protected abstract OrderTypes OrderType { get; }
        protected abstract OrderStatusTypes OrderStatusType { get; }

        protected abstract RadGrid OrderGrid { get; }
        #endregion

        #region Dates

        protected abstract RadDatePicker RegistrationDatePicker { get; }

        #endregion

        #region DropDowns

        protected abstract RadComboBox CustomerTypeDropDown { get; }
        protected abstract RadComboBox CustomerDropDown { get; }

        #endregion

        #region TextBoxes
        
        #endregion

        #region Panels

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

        #endregion

        #region Labels

        #endregion

        #endregion

        #region Event Handlers


        #endregion

        #region Linq Data Sources

        /// <summary>
        /// Datasource fuer die Uebersichtsgrud
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AbmeldungenLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            if (CustomerTypeDropDown.SelectedValue == "0")
            {
                var allCustomerOrders = new List<OrderViewModel>(GetSmallCustomerOrders().ToList());
                allCustomerOrders.AddRange(GetLargeCustomerOrders().ToList());

                e.Result = allCustomerOrders;
            }
            else if (CustomerTypeDropDown.SelectedValue == "1")
            {
                SmallCustomerOrdersFunctions();

                var smallCustomerOrders = GetSmallCustomerOrders();

                if (!String.IsNullOrEmpty(CustomerDropDown.SelectedValue))
                {
                    smallCustomerOrders = smallCustomerOrders.Where(q => q.customerId == Int32.Parse(CustomerDropDown.SelectedValue));
                }

                e.Result = smallCustomerOrders.ToList();
            }
            //select all values for large customers
            else if (CustomerTypeDropDown.SelectedValue == "2")
            {
                var largeCustomerOrders = GetLargeCustomerOrders();

                if (!String.IsNullOrEmpty(CustomerDropDown.SelectedValue))
                {
                    largeCustomerOrders = largeCustomerOrders.ToList().Where(q => q.customerId == Int32.Parse(CustomerDropDown.SelectedValue)).AsQueryable();
                }
                
                if (Session["orderNumberSearch"] != null && Session["orderStatusSearch"] != null)
                {
                    if (!String.IsNullOrEmpty(Session["orderNumberSearch"].ToString()))
                    {
                        if (Session["orderStatusSearch"].ToString().Contains("Offen"))
                        {
                            int orderNumber = Convert.ToInt32(Session["orderNumberSearch"].ToString());
                            largeCustomerOrders = largeCustomerOrders.Where(q => q.OrderNumber == orderNumber);
                        }
                    }
                }

                LargeCustomerOrdersFunctions();

                e.Result = largeCustomerOrders.ToList();
            }
        }

        /// <summary>
        /// Small oder Large -> Auswahl der KundenName
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities con = new KVSEntities();
            if (CustomerTypeDropDown.SelectedValue == "0") //all Customers
            {
                e.Result = new List<string>();
            }
            else if (CustomerTypeDropDown.SelectedValue == "1") //Small Customers
            {
                var customerQuery = CustomerManager.GetEntities(o => o.SmallCustomer != null).
                    Select(cust => new
                    {
                        Name = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name,
                        Value = cust.Id,
                        Matchcode = cust.MatchCode,
                        Kundennummer = cust.CustomerNumber
                    });

                e.Result = customerQuery.ToList();
            }
            else if (CustomerTypeDropDown.SelectedValue == "2") //Large Customers
            {
                var customerQuery = CustomerManager.GetEntities(o => o.LargeCustomer != null).
                    Select(cust => new
                    {
                        Name = cust.Name,
                        Value = cust.Id,
                        Matchcode = cust.MatchCode,
                        Kundennummer = cust.CustomerNumber
                    });

                e.Result = customerQuery.ToList();
            }
        }

        #endregion

        #region Methods

        protected virtual void SmallCustomerOrdersFunctions()
        {
        }

        protected virtual void LargeCustomerOrdersFunctions()
        {
        }

        protected IEnumerable<OrderViewModel> GetSmallCustomerOrders()
        {
            return OrderManager.GetEntities(o => o.Customer.SmallCustomer != null && 
                o.Status == (int)this.OrderStatusType && o.OrderTypeId == (int)this.OrderType &&
                o.HasError.GetValueOrDefault(false) != true).Select(ord => new OrderViewModel()
                {
                    OrderNumber = ord.OrderNumber,
                    customerId = ord.CustomerId,
                    CreateDate = ord.CreateDate,
                    Status = OrderStatuses.FirstOrDefault(o => o.Id == ord.Status).Name,
                    CustomerName = ord.Customer.SmallCustomer.Person != null ? 
                        ord.Customer.SmallCustomer.Person.FirstName + "  " + ord.Customer.SmallCustomer.Person.Name : ord.Customer.Name,
                    CustomerLocation = "",
                    Kennzeichen = ord.RegistrationOrder.Licencenumber,
                    VIN = ord.RegistrationOrder.Vehicle.VIN,
                    TSN = ord.RegistrationOrder.Vehicle.TSN,
                    HSN = ord.RegistrationOrder.Vehicle.HSN,
                    OrderTyp = OrderTypesCollection.FirstOrDefault(o => o.Id == ord.OrderTypeId).Name,
                    Freitext = ord.FreeText,
                    Geprueft = ord.Geprueft == null ? "Nein" : "Ja",
                    Datum = ord.RegistrationOrder.Registration.RegistrationDate
                });
        }
        
        protected IEnumerable<OrderViewModel> GetLargeCustomerOrders()
        {
            return OrderManager.GetEntities(o => o.Customer.LargeCustomer != null &&
                o.Status == (int)this.OrderStatusType && o.OrderTypeId == (int)this.OrderType &&
                o.HasError.GetValueOrDefault(false) != true).Select(ord => new OrderViewModel()
                {
                    OrderNumber = ord.OrderNumber,
                    locationId = ord.LocationId.HasValue ? ord.LocationId.Value : 0,
                    customerId = ord.CustomerId,
                    CreateDate = ord.CreateDate,
                    Status = OrderStatuses.FirstOrDefault(o => o.Id == ord.Status).Name,
                    CustomerName = ord.Customer.Name,
                    CustomerLocation = ord.Location != null ? ord.Location.Name : String.Empty,
                    Kennzeichen = ord.RegistrationOrder.Licencenumber,
                    VIN = ord.RegistrationOrder.Vehicle.VIN,
                    TSN = ord.RegistrationOrder.Vehicle.TSN,
                    HSN = ord.RegistrationOrder.Vehicle.HSN,
                    OrderTyp = OrderTypesCollection.FirstOrDefault(o => o.Id == ord.OrderTypeId).Name,
                    Freitext = ord.FreeText,
                    Geprueft = ord.Geprueft == null ? "Nein" : "Ja",
                    Datum = ord.RegistrationOrder.Registration.RegistrationDate
                });
        }

        #endregion

        #region Private Methods


        #endregion
    }
}