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
namespace KVSWebApplication.Auftragsbearbeitung_Neuzulassung
{
    /// <summary>
    /// Codebehind fuer die Fehlerhaften Zulassen
    /// </summary>
    public partial class FehlerhaftZulassung : System.Web.UI.UserControl
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
                    if (!Page.IsPostBack) ;
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
                        RadGridFehlerhaftZulassung.Enabled = true;
                        if (target.Contains("OffenNeuzulassung") || target.Contains("RadTabStripNeuzulassung") || target.Contains("IamFromSearch"))
                            RadGridFehlerhaftZulassung.DataBind();
                    }
                }
            }
        }
        /// <summary>
        /// Event fuer das leeren der Auswahl und neues Daten holen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void clearButton_Click(object sender, EventArgs e)
        {
            CustomerDropDownList.ClearSelection();
            RadGridFehlerhaftZulassung.DataBind();
        }
        /// <summary>
        /// Datasource fuer die fehlerhaften Auftraege
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                                         join regord in con.RegistrationOrder on ord.OrderNumber equals regord.OrderNumber
                                         join reg in con.Registration on regord.RegistrationId equals reg.Id
                                         join veh in con.Vehicle on regord.VehicleId equals veh.Id
                                         where ord.HasError.GetValueOrDefault(false) != false && ordtype.Id == (int)OrderTypes.Admission
                                         select new
                                         {
                                             OrderNumber = ord.OrderNumber,
                                             CustomerId = cust.Id,
                                             locationId = "",
                                             CreateDate = ord.CreateDate,
                                             Status = ordst.Name,
                                             CustomerName = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + "  " + cust.SmallCustomer.Person.Name : cust.Name,
                                             Kennzeichen = reg.Licencenumber,
                                             VIN = veh.VIN,
                                             TSN = veh.TSN,
                                             HSN = veh.HSN,
                                             Inspection = reg.GeneralInspectionDate,
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
                                         join regord in con.RegistrationOrder on ord.OrderNumber equals regord.OrderNumber
                                         join reg in con.Registration on regord.RegistrationId equals reg.Id
                                         join veh in con.Vehicle on regord.VehicleId equals veh.Id
                                         where ordtype.Id == (int)OrderTypes.Admission && ord.HasError.GetValueOrDefault(false) != false
                                         select new
                                         {
                                             OrderNumber = ord.OrderNumber,
                                             CustomerId = cust.Id,
                                             locationId = loc.Id,
                                             CreateDate = ord.CreateDate,
                                             Status = ordst.Name,
                                             Inspection = reg.GeneralInspectionDate,
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
        /// <summary>
        ///  Small oder Large -> Auswahl des KundenNamen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            this.RadGridFehlerhaftZulassung.DataBind();
        }
        /// <summary>
        /// Kundenauswahl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CustomerIndex_Changed(object sender, EventArgs e)
        {
            RadGridFehlerhaftZulassung.Enabled = true;
            this.RadGridFehlerhaftZulassung.DataBind();
        }
        /// <summary>
        /// Event fuer das Aktualisieren des Auftrages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            string selectedDate = InspectionDatePicker.SelectedDate.ToString();
            UpdateTheWorld(orderNumber, locationId, VINBox.Text, VariantBox.Text, LicenceBox.Text, PrevLicenceBox.Text, selectedDate,
                 TSNBox.Text, HSNBox.Text, InsuranceBox.Text, OwnerNameBox.Text, OwnerStreetBox.Text, OwnerFirstNameBox.Text, OwnerStreetNumberBox.Text,
                 OwnerZipCodeBox.Text, OwnerCityBox.Text, OwnerCountryBox.Text, OwnerPhoneBox.Text, OwnerFaxBox.Text, OwnerMobilePhoneBox.Text, OwnerEmailBox.Text,
                 BankNameBox.Text, AccountNumberBox.Text, BankCodeBox.Text, FreiTextBox.Text);
            if (Session["orderNumberSearch"] != null)
                Session["orderNumberSearch"] = string.Empty; //after search should be empty
            RadGridFehlerhaftZulassung.MasterTableView.ClearEditItems();
            RadGridFehlerhaftZulassung.MasterTableView.ClearChildEditItems();
            RadGridFehlerhaftZulassung.MasterTableView.ClearSelectedItems();
            RadGridFehlerhaftZulassung.Rebind();
        }
        /// <summary>
        /// aktualisiere alle Auftragsdaten
        /// </summary>
        /// <param name="OrderNumber"></param>
        /// <param name="locationId"></param>
        /// <param name="vin"></param>
        /// <param name="variant"></param>
        /// <param name="kennzeichen"></param>
        /// <param name="prevkennzeichen"></param>
        /// <param name="inspection"></param>
        /// <param name="tsn"></param>
        /// <param name="hsn"></param>
        /// <param name="insurance"></param>
        /// <param name="name"></param>
        /// <param name="street"></param>
        /// <param name="firstname"></param>
        /// <param name="streetnum"></param>
        /// <param name="zip"></param>
        /// <param name="city"></param>
        /// <param name="country"></param>
        /// <param name="phone"></param>
        /// <param name="fax"></param>
        /// <param name="mobile"></param>
        /// <param name="email"></param>
        /// <param name="bankname"></param>
        /// <param name="account"></param>
        /// <param name="bankcode"></param>
        /// <param name="freitext"></param>
        protected void UpdateTheWorld(int orderNumber, int? locationId, string vin, string variant, string kennzeichen, string prevkennzeichen,
            string inspection, string tsn, string hsn, string insurance, string name, string street, string firstname, string streetnum,
            string zip, string city, string country, string phone, string fax, string mobile, string email, string bankname, string account, string bankcode, string freitext)
        {
            try
            {
                FehlerhaftErrorMessage.Text = "";
                Order orderToUpdate = null;
                DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
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
                    orderToUpdate.RegistrationOrder.LogDBContext = dbContext;
                    orderToUpdate.RegistrationOrder.Vehicle.LogDBContext = dbContext;
                    orderToUpdate.RegistrationOrder.Registration.LogDBContext = dbContext;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.LogDBContext = dbContext;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.Adress.LogDBContext = dbContext;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.BankAccount.LogDBContext = dbContext;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.Contact.LogDBContext = dbContext;
                    orderToUpdate.RegistrationOrder.Vehicle.VIN = vin;
                    orderToUpdate.RegistrationOrder.Vehicle.TSN = tsn;
                    orderToUpdate.RegistrationOrder.Vehicle.HSN = hsn;
                    orderToUpdate.RegistrationOrder.Vehicle.Variant = variant;
                    orderToUpdate.RegistrationOrder.Registration.Licencenumber = kennzeichen;
                    orderToUpdate.RegistrationOrder.Registration.eVBNumber = insurance;
                    orderToUpdate.FreeText = freitext;
                    if (!String.IsNullOrEmpty(inspection))
                    {
                        orderToUpdate.RegistrationOrder.Registration.GeneralInspectionDate = DateTime.Parse(inspection);
                    }
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.Name = name;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.FirstName = firstname;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.BankAccount.BankName = bankname;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.BankAccount.Accountnumber = account;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.BankAccount.BankCode = bankcode;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.Adress.Street = street;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.Adress.StreetNumber = streetnum;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.Adress.City = city;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.Adress.Country = country;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.Adress.Zipcode = zip;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.Contact.Email = email;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.Contact.Phone = phone;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.Contact.MobilePhone = mobile;
                    orderToUpdate.RegistrationOrder.Registration.CarOwner.Contact.Fax = fax;
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
            catch (SqlException ex)
            {
                FehlerhaftErrorMessage.Text = "Fehler: " + ex.Message;
            }
        }
        /// <summary>
        /// Daten fuer die Grid neu laden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FehlerhaftGridNeuLadenButton_Clicked(object sender, EventArgs e)
        {
            RadGridFehlerhaftZulassung.Rebind();
        }
        /// <summary>
        /// Automatische Suche des HSN/TSN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        }
    }
}