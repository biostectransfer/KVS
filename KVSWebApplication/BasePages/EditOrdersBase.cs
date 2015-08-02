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
    ///  Incoming Orders base class
    /// </summary>
    public abstract class EditOrdersBase : BaseUserControl
    {
        #region Members

        #region Common

        protected RadScriptManager script = null;

        protected abstract PermissionTypes PagePermission { get; }
        protected abstract OrderTypes OrderType { get; }
        protected abstract OrderStatusTypes OrderStatusType { get; }

        protected abstract RadGrid OrderGrid { get; }
        protected abstract string OrderStatusSearch { get; }
        protected virtual bool OrderWithErrors { get { return false; } }
        #endregion

        #region Dates

        protected abstract RadDatePicker RegistrationDatePicker { get; }

        #endregion

        #region DropDowns

        protected abstract RadComboBox CustomerTypeDropDown { get; }
        protected abstract RadComboBox CustomerDropDown { get; }

        #endregion

        #endregion

        #region Linq Data Sources

        /// <summary>
        /// Datasource fuer die Uebersichtsgrud
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OrderLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
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
                        if (Session["orderStatusSearch"].ToString().Contains(this.OrderStatusSearch))
                        {
                            int orderNumber = Convert.ToInt32(Session["orderNumberSearch"].ToString());
                            largeCustomerOrders = largeCustomerOrders.Where(q => q.OrderNumber == orderNumber);
                        }
                    }
                }

                e.Result = largeCustomerOrders.ToList();
            }

            CheckOpenedOrders();
        }

        /// <summary>
        /// Small oder Large -> Auswahl der KundenName
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
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

        /// <summary>
        /// Datasource fuer die Dienstleistungen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProductLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = ProductManager.GetEntities().
                               Select(prod => new
                               {
                                   ItemNumber = prod.ItemNumber,
                                   Name = prod.Name,
                                   Value = prod.Id,
                                   Category = prod.ProductCategory.Name
                               }).ToList();
        }

        #endregion

        #region Methods

        protected virtual void SmallCustomerOrdersFunctions()
        {
        }

        protected virtual void CheckOpenedOrders()
        {
        }

        protected IEnumerable<OrderViewModel> GetSmallCustomerOrders()
        {
            return OrderManager.GetEntities(o => o.Customer.SmallCustomer != null &&
                ((this.OrderType == OrderTypes.Cancellation && o.DeregistrationOrder != null) ||
                (this.OrderType != OrderTypes.Cancellation && o.RegistrationOrder != null)) &&
                (this.OrderWithErrors || (!this.OrderWithErrors && o.Status == (int)this.OrderStatusType)) &&
                o.OrderTypeId == (int)this.OrderType &&
                o.HasError.GetValueOrDefault(false) != !this.OrderWithErrors).Select(ord =>
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
                    };
                });
        }

        protected IEnumerable<OrderViewModel> GetLargeCustomerOrders()
        {
            return GetLargeCustomerOrders(this.OrderType, this.OrderStatusType);
        }

        protected IEnumerable<OrderViewModel> GetLargeCustomerOrders(OrderTypes orderType, OrderStatusTypes orderStatusType)
        {
            return OrderManager.GetEntities(o => o.Customer.LargeCustomer != null &&
                ((this.OrderType == OrderTypes.Cancellation && o.DeregistrationOrder != null) ||
                (this.OrderType != OrderTypes.Cancellation && o.RegistrationOrder != null)) &&
                (this.OrderWithErrors || (!this.OrderWithErrors && o.Status == (int)this.OrderStatusType)) &&
                o.OrderTypeId == (int)orderType &&
                o.HasError.GetValueOrDefault(false) != !this.OrderWithErrors).Select(ord =>
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
                    };
                });
        }

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

        protected IEnumerable<Order> GetUnfineshedOrders(OrderTypes orderType, OrderStatusTypes orderStatus)
        {
            return OrderManager.GetEntities(q => q.Status == (int)orderStatus &&
                q.OrderType.Id == (int)orderType && q.HasError.GetValueOrDefault(false) != true);
        }

        protected int GetUnfineshedOrdersCount(OrderTypes orderType, OrderStatusTypes orderStatus)
        {
            return GetUnfineshedOrders(orderType, orderStatus).Count();
        }

        #endregion        
    }
}