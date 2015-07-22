using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.Data.SqlClient;
using KVSCommon.Enums;

namespace KVSWebApplication.Nachbearbeitung_Abmeldung
{
    /// <summary>
    /// Codebehind fuer die Abmeldung Fehlerhaft Reiter
    /// </summary>
    public partial class Fehlerhaft : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string target = Request["__EVENTTARGET"] == null ? "" : Request["__EVENTTARGET"];

            if (String.IsNullOrEmpty(target))
                if (Session["orderNumberSearch"] != null)
                    if (!String.IsNullOrEmpty(Session["orderNumberSearch"].ToString()))
                        target = "IamFromSearch";
            if (Session["CustomerIndex"] != null)
            {
                if (!target.Contains("RadComboBoxCustomerZulassungsstelle") && !target.Contains("CustomerDropDownListZulassungsstelle") && !target.Contains("NewPositionZulButton") && !target.Contains("StornierenButton"))
                {
                    CustomerDropDownList.DataBind();
                    if (Session["CustomerId"] != null)
                    {
                        if (!Page.IsPostBack)
                        {
                            if (CustomerDropDownList.Items.Count > 0 && RadComboBoxCustomer.Items.Count() > 0)
                            {
                                CustomerDropDownList.SelectedValue = Session["CustomerId"].ToString();
                                RadComboBoxCustomer.SelectedValue = Session["CustomerIndex"].ToString();
                            }
                        }
                        RadGridFehlerhaft.Enabled = true;
                        if (target.Contains("OffenNeuzulassung") || target.Contains("RadTabStripNeuzulassung") || target.Contains("IamFromSearch"))
                            RadGridFehlerhaft.DataBind();
                    }
                }
            }
        }
        protected void clearButton_Click(object sender, EventArgs e)
        {
            CustomerDropDownList.ClearSelection();
            RadGridFehlerhaft.DataBind();
        }
        protected void FehlerhaftLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            //select all values for small customers
            if (RadComboBoxCustomer.SelectedValue == "1")
            {
                DataClasses1DataContext con = new DataClasses1DataContext();
                var smallCustomerQuery = from ord in con.Order
                                         join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                         join cust in con.Customer on ord.CustomerId equals cust.Id
                                         join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                         join derord in con.DeregistrationOrder on ord.OrderNumber equals derord.OrderNumber
                                         join reg in con.Registration on derord.RegistrationId equals reg.Id
                                         join veh in con.Vehicle on derord.VehicleId equals veh.Id
                                         where ordtype.Id == (int)OrderTypes.Cancellation && ord.HasError.GetValueOrDefault(false) != false
                                         select new
                                         {
                                             OrderNumber = ord.OrderNumber,
                                             locationId = "",
                                             CustomerId = cust.Id,
                                             CreateDate = ord.CreateDate,
                                             Status = ordst.Name,
                                             CustomerName = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + "  " + cust.SmallCustomer.Person.Name : cust.Name,
                                             Kennzeichen = reg.Licencenumber,
                                             VIN = veh.VIN,
                                             TSN = veh.TSN,
                                             HSN = veh.HSN,
                                             Inspection = derord.Registration.GeneralInspectionDate,
                                             CustomerLocation = "",
                                             OrderTyp = ordtype.Name,
                                             Variant = veh.Variant,
                                             ErrorReason = ord.ErrorReason,
                                             eVBNum = reg.eVBNumber,
                                             Name = reg.CarOwner.Name,
                                             FirstName = reg.CarOwner.FirstName,
                                             BankName = reg.CarOwner.BankAccount.BankName,
                                             AccountNum = reg.CarOwner.BankAccount.Accountnumber,
                                             Prevkennzeichen = ord.RegistrationOrder.PreviousLicencenumber,
                                             BankCode = reg.CarOwner.BankAccount.BankCode,
                                             Street = reg.CarOwner.Adress.Street,
                                             StreetNr = reg.CarOwner.Adress.StreetNumber,
                                             Zip = reg.CarOwner.Adress.Zipcode,
                                             City = reg.CarOwner.Adress.City,
                                             Country = reg.CarOwner.Adress.Country,
                                             Phone = reg.CarOwner.Contact.Phone,
                                             Mobile = reg.CarOwner.Contact.MobilePhone,
                                             Fax = reg.CarOwner.Contact.Fax,
                                             Email = reg.CarOwner.Contact.Email,
                                             Freitext = ord.FreeText
                                         };
                if (CustomerDropDownList.SelectedValue != null && CustomerDropDownList.SelectedValue != string.Empty)
                {
                    smallCustomerQuery = smallCustomerQuery.Where(q => q.CustomerId == Int32.Parse(CustomerDropDownList.SelectedValue));
                }
                e.Result = smallCustomerQuery;
            }
            //select all values for large customers
            else if (RadComboBoxCustomer.SelectedValue == "2")
            {
                DataClasses1DataContext con = new DataClasses1DataContext();
                var largeCustomerQuery = from ord in con.Order
                                         join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                         join cust in con.Customer on ord.CustomerId equals cust.Id
                                         join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                         join loc in con.Location on ord.LocationId equals loc.Id
                                         join derord in con.DeregistrationOrder on ord.OrderNumber equals derord.OrderNumber
                                         join reg in con.Registration on derord.RegistrationId equals reg.Id
                                         join veh in con.Vehicle on derord.VehicleId equals veh.Id
                                         where ord.HasError.GetValueOrDefault(false) != false && ordtype.Id == (int)OrderTypes.Cancellation
                                         select new
                                         {
                                             OrderNumber = ord.OrderNumber,
                                             locationId = loc.Id,
                                             CustomerId = cust.Id,
                                             CreateDate = ord.CreateDate,
                                             Status = ordst.Name,
                                             Inspection = derord.Registration.GeneralInspectionDate,
                                             CustomerName = cust.Name,
                                             Kennzeichen = reg.Licencenumber,
                                             Variant = veh.Variant,
                                             Prevkennzeichen = ord.RegistrationOrder.PreviousLicencenumber,
                                             VIN = veh.VIN,
                                             TSN = veh.TSN,
                                             HSN = veh.HSN,
                                             CustomerLocation = loc.Name,
                                             OrderTyp = ordtype.Name,
                                             ErrorReason = ord.ErrorReason,
                                             eVBNum = reg.eVBNumber,
                                             Name = reg.CarOwner.Name,
                                             FirstName = reg.CarOwner.FirstName,
                                             BankName = reg.CarOwner.BankAccount.BankName,
                                             AccountNum = reg.CarOwner.BankAccount.Accountnumber,
                                             BankCode = reg.CarOwner.BankAccount.BankCode,
                                             Street = reg.CarOwner.Adress.Street,
                                             StreetNr = reg.CarOwner.Adress.StreetNumber,
                                             Zip = reg.CarOwner.Adress.Zipcode,
                                             City = reg.CarOwner.Adress.City,
                                             Country = reg.CarOwner.Adress.Country,
                                             Phone = reg.CarOwner.Contact.Phone,
                                             Mobile = reg.CarOwner.Contact.MobilePhone,
                                             Fax = reg.CarOwner.Contact.Fax,
                                             Email = reg.CarOwner.Contact.Email,
                                             Freitext = ord.FreeText
                                         };
                if (CustomerDropDownList.SelectedValue != null && CustomerDropDownList.SelectedValue != string.Empty)
                {
                    largeCustomerQuery = largeCustomerQuery.Where(q => q.CustomerId == Int32.Parse(CustomerDropDownList.SelectedValue));
                }
                if (Session["orderNumberSearch"] != null)
                {
                    if (!String.IsNullOrEmpty(Session["orderNumberSearch"].ToString()))
                    {
                        if (Session["orderStatusSearch"].ToString().Contains("Error"))
                        {
                            int orderNumber = Convert.ToInt32(Session["orderNumberSearch"].ToString());
                            largeCustomerQuery = largeCustomerQuery.Where(q => q.OrderNumber == orderNumber);
                        }
                    }
                }
                e.Result = largeCustomerQuery;
            }
        }
        // Small oder Large -> Auswahl der KundenName
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            if (RadComboBoxCustomer.SelectedValue == "1") //Small Customers
            {
                var customerQuery = from cust in con.Customer
                                    where cust.Id == cust.SmallCustomer.CustomerId
                                    select new { Name = cust.Name, Value = cust.Id, Matchcode = cust.MatchCode, Kundennummer = cust.CustomerNumber };

                e.Result = customerQuery;
            }
            else if (RadComboBoxCustomer.SelectedValue == "2") //Large Customers
            {
                var customerQuery = from cust in con.Customer
                                    where cust.Id == cust.LargeCustomer.CustomerId
                                    select new { Name = cust.Name, Value = cust.Id, Matchcode = cust.MatchCode, Kundennummer = cust.CustomerNumber };
                e.Result = customerQuery;
            }
        }
        // Large oder small Customer
        protected void SmallLargeCustomerIndex_Changed(object sender, EventArgs e)
        {
            CustomerDropDownList.Enabled = true;
            this.CustomerDropDownList.DataBind();
            this.RadGridFehlerhaft.DataBind();
        }
        // Auswahl von Kunde
        protected void CustomerIndex_Changed(object sender, EventArgs e)
        {
            RadGridFehlerhaft.Enabled = true;
            this.RadGridFehlerhaft.DataBind();
        }
        protected void OrderUpdateButton_Clicked(object sender, EventArgs e)
        {
            string begruendung = string.Empty;
            string newStatus = string.Empty;
            int? locationId = null;
            //vorbereitung für update
            var editButton = sender as Button;
            var item = editButton.NamingContainer as GridEditFormItem;

            var orderNumber = Int32.Parse(item.SavedOldValues["OrderNumber"].ToString());
            if (RadComboBoxCustomer.SelectedValue == "2")
            {
                locationId = Int32.Parse(item.SavedOldValues["locationId"].ToString());
            }
            var VINBox = item.FindControl("VINBox") as RadTextBox;
            var VariantBox = item.FindControl("VariantBox") as RadTextBox;
            var LicenceBox = item.FindControl("LicenceBox") as RadTextBox;
            var PrevLicenceBox = item.FindControl("PreviousLicenceBox") as RadTextBox;
            var InspectionDatePicker = item.FindControl("InspectionDatePicker") as RadDatePicker;
            var TSNBox = item.FindControl("TSNBox") as RadTextBox;
            var HSNBox = item.FindControl("HSNBox") as RadTextBox;
            var InsuranceBox = item.FindControl("InsuranceBox") as RadTextBox;
            var OwnerNameBox = item.FindControl("OwnerNameBox") as RadTextBox;
            var OwnerStreetBox = item.FindControl("OwnerStreetBox") as RadTextBox;
            var OwnerFirstNameBox = item.FindControl("OwnerFirstNameBox") as RadTextBox;
            var OwnerStreetNumberBox = item.FindControl("OwnerStreetNubmerBox") as RadTextBox;
            var OwnerZipCodeBox = item.FindControl("OwnerZipCodeBox") as RadTextBox;
            var OwnerCityBox = item.FindControl("OwnerCityBox") as RadTextBox;
            var OwnerCountryBox = item.FindControl("OwnerCountryBox") as RadTextBox;
            var OwnerPhoneBox = item.FindControl("OwnerPhoneBox") as RadTextBox;
            var OwnerFaxBox = item.FindControl("OwnerFaxBox") as RadTextBox;
            var OwnerMobilePhoneBox = item.FindControl("OwnerMobilePhoneBox") as RadTextBox;
            var OwnerEmailBox = item.FindControl("OwnerEmailBox") as RadTextBox;
            var BankNameBox = item.FindControl("BankNameBox") as RadTextBox;
            var AccountNumberBox = item.FindControl("AccountNumberBox") as RadTextBox;
            var BankCodeBox = item.FindControl("BankCodeBox") as RadTextBox;
            var FreiTextBox = item.FindControl("Freitext") as RadTextBox;
            UpdateTheWorld(orderNumber, locationId, VINBox.Text, VariantBox.Text, LicenceBox.Text, PrevLicenceBox.Text, InspectionDatePicker.SelectedDate,
                TSNBox.Text, HSNBox.Text, InsuranceBox.Text, OwnerNameBox.Text, OwnerStreetBox.Text, OwnerFirstNameBox.Text, OwnerStreetNumberBox.Text,
                OwnerZipCodeBox.Text, OwnerCityBox.Text, OwnerCountryBox.Text, OwnerPhoneBox.Text, OwnerFaxBox.Text, OwnerMobilePhoneBox.Text, OwnerEmailBox.Text,
                BankNameBox.Text, AccountNumberBox.Text, BankCodeBox.Text, FreiTextBox.Text);
            if (Session["orderNumberSearch"] != null)
                Session["orderNumberSearch"] = string.Empty; //after search should be empty
            RadGridFehlerhaft.MasterTableView.ClearEditItems();
            RadGridFehlerhaft.MasterTableView.ClearChildEditItems();
            RadGridFehlerhaft.MasterTableView.ClearSelectedItems();
            RadGridFehlerhaft.Rebind();
        }
        //Updating all values in Order
        protected void UpdateTheWorld(int orderNumber, int? locationId, string vin, string variant, string kennzeichen, string prevkennzeichen,
            DateTime? inspection, string tsn, string hsn, string insurance, string name, string street, string firstname, string streetnum,
            string zip, string city, string country, string phone, string fax, string mobile, string email, string bankname, string account, string bankcode, string freitext)
        {
            try
            {
                FehlerhaftErrorMessage.Text = "";
                DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                Order orderToUpdate = null;
                if (RadComboBoxCustomer.SelectedValue == "1")
                {
                    orderToUpdate = dbContext.Order.SingleOrDefault(q => q.OrderNumber == orderNumber);
                }
                else
                {
                    orderToUpdate = dbContext.Order.SingleOrDefault(q => q.OrderNumber == orderNumber && q.LocationId == locationId);
                }
                if (orderToUpdate != null)
                {
                    orderToUpdate.LogDBContext = dbContext;
                    orderToUpdate.DeregistrationOrder.LogDBContext = dbContext;
                    orderToUpdate.DeregistrationOrder.Vehicle.LogDBContext = dbContext;
                    orderToUpdate.DeregistrationOrder.Registration.LogDBContext = dbContext;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.LogDBContext = dbContext;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.Adress.LogDBContext = dbContext;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.BankAccount.LogDBContext = dbContext;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.Contact.LogDBContext = dbContext;
                    orderToUpdate.DeregistrationOrder.Vehicle.VIN = vin;
                    orderToUpdate.DeregistrationOrder.Vehicle.TSN = tsn;
                    orderToUpdate.DeregistrationOrder.Vehicle.HSN = hsn;
                    orderToUpdate.DeregistrationOrder.Vehicle.Variant = variant;
                    orderToUpdate.DeregistrationOrder.Registration.Licencenumber = kennzeichen;
                    orderToUpdate.DeregistrationOrder.Registration.eVBNumber = insurance;
                    orderToUpdate.FreeText = freitext;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.Name = name;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.FirstName = firstname;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.BankAccount.BankName = bankname;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.BankAccount.Accountnumber = account;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.BankAccount.BankCode = bankcode;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.Adress.Street = street;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.Adress.StreetNumber = streetnum;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.Adress.City = city;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.Adress.Country = country;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.Adress.Zipcode = zip;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.Contact.Email = email;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.Contact.Phone = phone;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.Contact.MobilePhone = mobile;
                    orderToUpdate.DeregistrationOrder.Registration.CarOwner.Contact.Fax = fax;
                    orderToUpdate.Status = (int)OrderStatusTypes.Open;
                    orderToUpdate.HasError = false;
                    orderToUpdate.ErrorReason = string.Empty;
                    foreach (var orderItem in orderToUpdate.OrderItem)
                    {
                        orderItem.LogDBContext = dbContext;
                        orderItem.Status = (int)OrderItemStatusTypes.Open;
                    }
                    dbContext.SubmitChanges();
                }
            }
            catch (SqlException e)
            {
                FehlerhaftErrorMessage.Text = "Fehler: " + e.Message;
            }
        }
        // Automatische Suche nach HSN
        protected void HSNBox_TextChanged(object sender, EventArgs e)
        {
            RadTextBox hsnTextBox = sender as RadTextBox;
            Label hsnLabel = hsnTextBox.Parent.FindControl("HSNSearchLabel") as Label;
            hsnLabel.Text = "";
            if (!String.IsNullOrEmpty(hsnTextBox.Text))
            {
                hsnLabel.Visible = true;
                hsnLabel.Text = Make.GetMakeByHSN(hsnTextBox.Text);
            }
            hsnTextBox.Focus();
        }
    }
}