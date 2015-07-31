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
        }

        #region Common

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
                var allCustomers = new List<OrderViewModel>(GetSmallCustomerOrders().ToList());
                allCustomers.AddRange(GetLargeCustomerOrders().ToList());

                e.Result = allCustomers;
            }
            else if (CustomerTypeDropDown.SelectedValue == "1")
            {
                SmallCustomerOrdersFunctions();

                var smallCustomers = GetSmallCustomerOrders();

                if (!String.IsNullOrEmpty(CustomerDropDown.SelectedValue))
                {
                    smallCustomers = smallCustomers.Where(q => q.customerId == Int32.Parse(CustomerDropDown.SelectedValue));
                }

                e.Result = smallCustomers.ToList();
            }
            //select all values for large customers
            else if (CustomerTypeDropDown.SelectedValue == "2")
            {
                var largeCustomers = GetLargeCustomerOrders();

                if (!String.IsNullOrEmpty(CustomerDropDown.SelectedValue))
                {
                    largeCustomers = largeCustomers.Where(q => q.customerId == Int32.Parse(CustomerDropDown.SelectedValue));
                }
                
                if (Session["orderNumberSearch"] != null && Session["orderStatusSearch"] != null)
                {
                    if (!String.IsNullOrEmpty(Session["orderNumberSearch"].ToString()))
                    {
                        if (Session["orderStatusSearch"].ToString().Contains("Offen"))
                        {
                            int orderNumber = Convert.ToInt32(Session["orderNumberSearch"].ToString());
                            largeCustomers = largeCustomers.Where(q => q.OrderNumber == orderNumber);

                        }
                    }
                }

                LargeCustomerOrdersFunctions();

                e.Result = largeCustomers.ToList();
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
                var customerQuery = from cust in con.Customer
                                    where cust.Id == cust.SmallCustomer.CustomerId
                                    select new
                                    {
                                        Name = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name,
                                        Value = cust.Id,
                                        Matchcode = cust.MatchCode,
                                        Kundennummer = cust.CustomerNumber
                                    };

                e.Result = customerQuery;
            }
            else if (CustomerTypeDropDown.SelectedValue == "2") //Large Customers
            {
                var customerQuery = from cust in con.Customer
                                    where cust.Id == cust.LargeCustomer.CustomerId
                                    select new { Name = cust.Name, Value = cust.Id, Matchcode = cust.MatchCode, Kundennummer = cust.CustomerNumber };

                e.Result = customerQuery;
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

        protected IQueryable<OrderViewModel> GetSmallCustomerOrders()
        {
            KVSEntities con = new KVSEntities();

            var smallCustomerQuery = from ord in con.Order
                                     join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                     join cust in con.Customer on ord.CustomerId equals cust.Id
                                     join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                     join regord in con.RegistrationOrder on ord.OrderNumber equals regord.OrderNumber
                                     join reg in con.Registration on regord.RegistrationId equals reg.Id
                                     join veh in con.Vehicle on regord.VehicleId equals veh.Id
                                     join smc in con.SmallCustomer on cust.Id equals smc.CustomerId
                                     orderby ord.OrderNumber descending
                                     where ord.Status == (int)this.OrderStatusType && ordtype.Id == (int)this.OrderType && 
                                     ord.HasError.GetValueOrDefault(false) != true
                                     select new OrderViewModel()
                                     {
                                         OrderNumber = ord.OrderNumber,
                                         customerId = cust.Id,
                                         //customerID = ord.CustomerId,
                                         CreateDate = ord.CreateDate,
                                         Status = ordst.Name,
                                         CustomerName = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + "  " +
                                         cust.SmallCustomer.Person.Name : cust.Name,
                                         CustomerLocation = "",
                                         Kennzeichen = reg.Licencenumber,
                                         VIN = veh.VIN,
                                         TSN = veh.TSN,
                                         HSN = veh.HSN,
                                         OrderTyp = ordtype.Name,
                                         Freitext = ord.FreeText,
                                         Geprueft = ord.Geprueft == null ? "Nein" : "Ja",
                                         Datum = ord.RegistrationOrder.Registration.RegistrationDate
                                     };

            return smallCustomerQuery;
        }
        
        protected IQueryable<OrderViewModel> GetLargeCustomerOrders()
        {
            KVSEntities con = new KVSEntities();

            var zulassungQuery = from ord in con.Order
                                 join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                 join cust in con.Customer on ord.CustomerId equals cust.Id
                                 join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                 join loc in con.Location on ord.LocationId equals loc.Id
                                 join regord in con.RegistrationOrder on ord.OrderNumber equals regord.OrderNumber
                                 join reg in con.Registration on regord.RegistrationId equals reg.Id
                                 join veh in con.Vehicle on regord.VehicleId equals veh.Id
                                 join lmc in con.LargeCustomer on cust.Id equals lmc.CustomerId
                                 orderby ord.OrderNumber descending
                                 where ord.Status == (int)this.OrderStatusType && ordtype.Id == (int)this.OrderType &&
                                 ord.HasError.GetValueOrDefault(false) != true
                                 select new OrderViewModel()
                                 {
                                     OrderNumber = ord.OrderNumber,
                                     locationId = loc.Id,
                                     //customerID = cust.Id,
                                     CreateDate = ord.CreateDate,
                                     Status = ordst.Name,
                                     CustomerName = cust.Name,
                                     Kennzeichen = reg.Licencenumber,
                                     VIN = veh.VIN,
                                     TSN = veh.TSN,
                                     HSN = veh.HSN,
                                     CustomerLocation = loc.Name,
                                     OrderTyp = ordtype.Name,
                                     Freitext = ord.FreeText,
                                     Geprueft = ord.Geprueft == null ? "Nein" : "Ja",
                                     Datum = ord.RegistrationOrder.Registration.RegistrationDate
                                 };

            return zulassungQuery;
        }

        #endregion

        #region Private Methods


        #endregion
    }
}