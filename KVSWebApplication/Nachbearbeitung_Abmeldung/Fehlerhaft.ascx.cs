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
using KVSWebApplication.Auftragsbearbeitung_Neuzulassung;

namespace KVSWebApplication.Nachbearbeitung_Abmeldung
{
    /// <summary>
    /// Codebehind fuer die Abmeldung Fehlerhaft Reiter
    /// </summary>
    public partial class Fehlerhaft : EditOrdersBase
    {
        #region Members  

        protected override RadGrid OrderGrid { get { return this.RadGridFehlerhaft; } }
        protected override RadDatePicker RegistrationDatePicker { get { return null; } }
        protected override RadComboBox CustomerTypeDropDown { get { return this.RadComboBoxCustomer; } }
        protected override RadComboBox CustomerDropDown { get { return this.CustomerDropDownList; } }
        protected override PermissionTypes PagePermission { get { return PermissionTypes.LOESCHEN_AUFTRAGSPOSITION; } }
        protected override OrderTypes OrderType { get { return OrderTypes.Cancellation; } }
        protected override OrderStatusTypes OrderStatusType { get { return OrderStatusTypes.Open; } }
        protected override string OrderStatusSearch { get { return "Error"; } }
        protected override bool OrderWithErrors { get { return true; } }
        #endregion

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            string target = Request["__EVENTTARGET"] == null ? "" : Request["__EVENTTARGET"];

            if (String.IsNullOrEmpty(target) && Session["orderNumberSearch"] != null && !String.IsNullOrEmpty(Session["orderNumberSearch"].ToString()))
                target = "IamFromSearch";

            if (Session["CustomerIndex"] != null)
            {
                if (!target.Contains("RadComboBoxCustomerZulassungsstelle") &&
                    !target.Contains("CustomerDropDownListZulassungsstelle") && 
                    !target.Contains("NewPositionZulButton") && !target.Contains("StornierenButton"))
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

            UpdateOrderData(orderNumber, VINBox.Text, VariantBox.Text, LicenceBox.Text, PrevLicenceBox.Text, InspectionDatePicker.SelectedDate,
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
        
        // Automatische Suche nach HSN
        protected void HSNBox_TextChanged(object sender, EventArgs e)
        {
            var hsnTextBox = sender as RadTextBox;
            var hsnLabel = hsnTextBox.Parent.FindControl("HSNSearchLabel") as Label;
            hsnLabel.Text = "";
            if (!String.IsNullOrEmpty(hsnTextBox.Text))
            {
                hsnLabel.Visible = true;
                hsnLabel.Text = Make.GetMakeByHSN(hsnTextBox.Text);
            }
            hsnTextBox.Focus();
        }

        #endregion

        #region Methods

        //Updating all values in Order
        protected void UpdateOrderData(int orderNumber, string vin, string variant, string kennzeichen, string prevkennzeichen,
            DateTime? inspection, string tsn, string hsn, string insurance, string name, string street, string firstname, string streetnum,
            string zip, string city, string country, string phone, string fax, string mobile, string email, string bankname, string account, string bankcode, string freitext)
        {
            try
            {
                FehlerhaftErrorMessage.Text = "";
                var orderToUpdate = OrderManager.GetById(orderNumber);

                if (orderToUpdate != null)
                {
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
                        orderItem.Status = (int)OrderItemStatusTypes.Open;
                    }

                    OrderManager.SaveChanges();
                }
            }
            catch (SqlException e)
            {
                FehlerhaftErrorMessage.Text = "Fehler: " + e.Message;
            }
        }

        #endregion
    }
}