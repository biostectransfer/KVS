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
    public partial class FehlerhaftZulassung : EditOrdersBase
    {
        #region Members  

        protected override RadGrid OrderGrid { get { return this.RadGridFehlerhaftZulassung; } }
        protected override RadDatePicker RegistrationDatePicker { get { return null; } }
        protected override RadComboBox CustomerTypeDropDown { get { return this.RadComboBoxCustomer; } }
        protected override RadComboBox CustomerDropDown { get { return this.CustomerDropDownList; } }
        protected override PermissionTypes PagePermission { get { return PermissionTypes.LOESCHEN_AUFTRAGSPOSITION; } }
        protected override OrderTypes OrderType { get { return OrderTypes.Admission; } }
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
                    !target.Contains("NewPositionZulButton") &&
                    !target.Contains("StornierenButton"))
                {
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

            UpdateOrderData(orderNumber, locationId, VINBox.Text, VariantBox.Text, LicenceBox.Text, PrevLicenceBox.Text, selectedDate,
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
        protected void UpdateOrderData(int orderNumber, int? locationId, string vin, string variant, string kennzeichen, string prevkennzeichen,
            string inspection, string tsn, string hsn, string insurance, string name, string street, string firstname, string streetnum,
            string zip, string city, string country, string phone, string fax, string mobile, string email, string bankname, string account, string bankcode, string freitext)
        {
            try
            {
                FehlerhaftErrorMessage.Text = "";
                Order orderToUpdate = null;

                if (RadComboBoxCustomer.SelectedValue == "1")
                {
                    orderToUpdate = OrderManager.GetById(orderNumber);
                }
                else
                {
                    orderToUpdate = OrderManager.GetEntities(q => q.OrderNumber == orderNumber && q.LocationId == locationId).SingleOrDefault();
                }

                if (orderToUpdate != null)
                {
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
                        orderItem.Status = (int)OrderItemStatusTypes.Open;
                    }

                    OrderManager.SaveChanges();
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

        #endregion
    }
}