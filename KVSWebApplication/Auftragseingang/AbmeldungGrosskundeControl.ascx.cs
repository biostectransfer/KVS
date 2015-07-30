﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.IO;
using System.Configuration;
using KVSCommon.Enums;
using KVSCommon.Managers;
namespace KVSWebApplication.Auftragseingang
{
    /// <summary>
    ///  Abmeldung Großkunde
    /// </summary>
    public partial class AbmeldungGrosskundeControl : CancellationOrderBase
    {
        #region Members     

        protected override Label CustomerHistoryLabel { get { return this.SmallCustomerHistorie; } }
        protected override RadTreeView ProductTree { get { return DienstleistungTreeView; } }
        protected override RadScriptManager RadScriptManager { get { return ((AbmeldungGrosskunde)Page).getScriptManager(); } }
        protected override RadNumericTextBox Discount { get { return this.txbDiscount; } }
        protected override HiddenField SmallCustomerOrder { get { return this.smallCustomerOrderHiddenField; } }
        protected override RequiredFieldValidator InvoiceValidator { get { return this.InvoiceRecValidator; } }

        #region Dates

        protected override RadMonthYearPicker Registration_GeneralInspectionDatePicker { get { return this.Registration_GeneralInspectionDateBox; } }
        protected override RadDatePicker FirstRegistrationDatePicker { get { return this.FirstRegistrationDateBox; } }

        #endregion

        #region DropDowns

        protected override RadComboBox CustomerDropDown { get { return this.CustomerDropDownList; } }
        protected override RadComboBox LocationDropDown { get { return this.LocationDropDownList; } }
        protected override RadComboBox CostCenterDropDown { get { return this.CostCenterDropDownList; } }
        protected override RadComboBox AdmissionPointDropDown { get { return this.ZulassungsstelleComboBox; } }
        protected override RadComboBox ProductDropDown { get { return this.ProductAbmDropDownList; } }
        protected override RadComboBox RegistrationOrderDropDown { get { return null; } }
        #endregion

        #region TextBoxes

        protected override RadTextBox AccountNumberTextBox { get { return this.BankAccount_AccountnumberBox; } }
        protected override RadTextBox BankCodeTextBox { get { return this.BankAccount_BankCodeBox; } }
        protected override RadTextBox BankNameTextBox { get { return this.BankAccount_BankNameBox; } }
        protected override RadTextBox IBANTextBox { get { return this.txbBancAccountIban; } }
        protected override RadTextBox BICTextBox { get { return this.txbBankAccount_Bic; } }
        protected override RadTextBox Adress_Street_TextBox { get { return this.Adress_StreetBox; } }
        protected override RadTextBox Adress_StreetNumber_TextBox { get { return this.Adress_StreetNumberBox; } }
        protected override RadTextBox Adress_Zipcode_TextBox { get { return this.Adress_ZipcodeBox; } }
        protected override RadTextBox Adress_City_TextBox { get { return this.Adress_CityBox; } }
        protected override RadTextBox Adress_Country_TextBox { get { return this.Adress_CountryBox; } }
        protected override RadTextBox CarOwner_Name_TextBox { get { return this.CarOwner_NameBox; } }
        protected override RadTextBox CarOwner_FirstName_TextBox { get { return this.CarOwner_FirstnameBox; } }
        protected override RadTextBox Registration_eVBNumber_TextBox { get { return this.Registration_eVBNumberBox; } }
        protected override TextBox Street_TextBox { get { return this.StreetTextBox; } }
        protected override TextBox StreetNumber_TextBox { get { return this.StreetNumberTextBox; } }
        protected override TextBox Zipcode_TextBox { get { return this.ZipcodeTextBox; } }
        protected override TextBox City_TextBox { get { return this.CityTextBox; } }
        protected override TextBox Country_TextBox { get { return this.CountryTextBox; } }
        protected override TextBox InvoiceRecipient_TextBox { get { return this.InvoiceRecipient; } }
        #endregion

        #region Panels

        protected override Panel Panel { get { return this.EingangAbmeldungPanel; } }
        protected override Panel Vehicle_Variant_Panel { get { return this.Vehicle_Variant; } }
        protected override Panel Registration_GeneralInspectionDate_Panel { get { return this.Registration_GeneralInspectionDate; } }
        protected override Panel CarOwner_Name_Panel { get { return this.CarOwner_Name; } }
        protected override Panel CarOwner_Firstname_Panel { get { return this.CarOwner_Firstname; } }
        protected override Panel Adress_StreetNumber_Panel { get { return this.Adress_StreetNumber; } }
        protected override Panel Adress_Street_Panel { get { return this.Adress_Street; } }
        protected override Panel Adress_Zipcode_Panel { get { return this.Adress_Zipcode; } }
        protected override Panel Adress_City_Panel { get { return this.Adress_City; } }
        protected override Panel Adress_Country_Panel { get { return this.Adress_Country; } }
        protected override Panel Contact_Phone_Panel { get { return this.Contact_Phone; } }
        protected override Panel Contact_Fax_Panel { get { return this.Contact_Fax; } }
        protected override Panel Contact_MobilePhone_Panel { get { return this.Contact_MobilePhone; } }
        protected override Panel Contact_Email_Panel { get { return this.Contact_Email; } }
        protected override Panel BankAccount_BankName_Panel { get { return this.BankAccount_BankName; } }
        protected override Panel BankAccount_Accountnumber_Panel { get { return this.BankAccount_Accountnumber; } }
        protected override Panel BankAccount_BankCode_Panel { get { return this.BankAccount_BankCode; } }
        protected override Panel Registration_eVBNumber_Panel { get { return this.Registration_eVBNumber; } }
        protected override Panel Vehicle_HSN_Panel { get { return this.Vehicle_HSN; } }
        protected override Panel Vehicle_TSN_Panel { get { return this.Vehicle_TSN; } }
        protected override Panel Vehicle_VIN_Panel { get { return this.Vehicle_VIN; } }
        protected override Panel Registration_Licencenumber_Panel { get { return this.Registration_Licencenumber; } }
        protected override Panel RegistrationOrder_PreviousLicencenumber_Panel { get { return this.RegistrationOrder_PreviousLicencenumber; } }
        protected override Panel Registration_EmissionCode_Panel { get { return this.Registration_EmissionCode; } }
        protected override Panel Registration_RegistrationDocumentNumber_Panel { get { return this.Registration_RegistrationDocumentNumber; } }
        protected override Panel Vehicle_FirstRegistrationDate_Panel { get { return this.Vehicle_FirstRegistrationDate; } }
        protected override Panel Vehicle_Color_Panel { get { return this.Vehicle_Color; } }
        protected override Panel IBANPanel_Panel { get { return this.IBANPanel; } }

        #endregion

        #region Labels

        protected override Label FahrzeugCaption { get { return this.FahrzeugLabel; } }
        protected override Label HalterCaption { get { return this.HalterLabel; } }
        protected override Label HalterdatenCaption { get { return this.HalterdatenLabel; } }
        protected override Label KontaktdatenCaption { get { return this.KontaktdatenLabel; } }
        protected override Label HSNSearchCaption { get { return this.HSNSearchLabel; } }
        protected override Label ErrorLeereTextBoxenCaption { get { return this.ErrorLeereTextBoxenLabel; } }

        #endregion

        #endregion

        #region Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            RadScriptManager.RegisterPostBackControl(AddAdressButton);
            LicenceBox1.Enabled = true;
            LicenceBox2.Enabled = true;
            LicenceBox3.Enabled = true;
            //first registration get today date by default
            FirstRegistrationDateBox.SelectedDate = DateTime.Now;
            if (Session["CurrentUserId"] != null)
            {
                if (!String.IsNullOrEmpty(Session["CurrentUserId"].ToString()))
                {
                    CheckUserPermissions();
                }
            }
        }


        #region Button Clicked
        //Fahrzeug abmelden
        protected void AbmeldenButton_Clicked(object sender, EventArgs e)
        {
            int? locationId = null;
            string ProduktId = "";
            string CostCenterId = "";
            AbmeldungOkLabel.Visible = false;
            SubmitChangesErrorLabel.Visible = false;
            ErrorLeereTextBoxenLabel.Visible = false;
            if (!String.IsNullOrEmpty(LocationDropDownList.SelectedValue))
                locationId = Int32.Parse(LocationDropDownList.SelectedValue);
            else
                locationId = null;

            if (CheckIfBoxenNotEmpty()) //gibt es leer boxen, die angezeigt sind.
            {
                if (DienstleistungTreeView.Nodes.Count == 0)
                {
                    ErrorLeereTextBoxenLabel.Text = "Bitte Dienstleistung hinzufügen!";
                    ErrorLeereTextBoxenLabel.Visible = true;
                }
                else if (String.IsNullOrEmpty(CostCenterDropDownList.SelectedValue))
                {
                    ErrorLeereTextBoxenLabel.Text = "Bitte tragen Sie die Kostenstelle ein!";
                    ErrorLeereTextBoxenLabel.Visible = true;
                }

                else if (String.IsNullOrEmpty(LocationDropDownList.SelectedValue))
                {
                    ErrorLeereTextBoxenLabel.Text = "Bitte wählen Sie der Standort aus!";
                    ErrorLeereTextBoxenLabel.Visible = true;
                }
                else if (String.IsNullOrEmpty(ZulassungsstelleComboBox.SelectedValue))
                {
                    ErrorLeereTextBoxenLabel.Text = "Bitte wählen Sie die Zulassungsstelle aus!";
                    ErrorLeereTextBoxenLabel.Visible = true;
                }
                else
                {
                    ErrorLeereTextBoxenLabel.Text = "Bitte Pflichtfelder überprüfen!";
                    ErrorLeereTextBoxenLabel.Visible = true;
                }
            }
            else
            {
                var productsPriceCheck = CheckIfAllProduktsHavingPrice(locationId);
                if (!String.IsNullOrEmpty(productsPriceCheck))
                {
                    ErrorLeereTextBoxenLabel.Text = String.Format("Für {0} wurde kein Price gefunden!", productsPriceCheck);
                    ErrorLeereTextBoxenLabel.Visible = true;
                    return;
                }
                else if (DienstleistungTreeView.Nodes.Count > 0)
                {
                    ErrorLeereTextBoxenLabel.Visible = false;
                    SubmitChangesErrorLabel.Visible = false;
                    RadTreeNode node = DienstleistungTreeView.Nodes[0];
                    string[] splited = node.Value.Split(';');
                    ProduktId = splited[0];
                    CostCenterId = splited[1];
                    if (CostCenterId == string.Empty)
                        CostCenterId = CostCenterDropDownList.SelectedValue;
                    try
                    {
                        KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                        Adress newAdress = null;
                        Contact newContact = null;
                        BankAccount newBankAccount = null;
                        CarOwner newCarOwner = null;
                        Registration newRegistration = null;
                        Price price = null;
                        OrderItem newOrderItem1 = null;
                        OrderItem newOrderItem2 = null;
                        Vehicle newVehicle = null;
                        DateTime? FirstRegistrationDate = null;
                        int? costCenterId = null;
                        DeregistrationOrder newDeregOrder = null;
                        string kennzeichen = string.Empty,
                                  oldKennzeichen = string.Empty;
                        int? color = null;
                        if (!String.IsNullOrEmpty(LicenceBox1.Text))
                            kennzeichen = LicenceBox1.Text + "-" + LicenceBox2.Text + "-" + LicenceBox3.Text;
                        if (!String.IsNullOrEmpty(PreviousLicenceBox1.Text))
                            oldKennzeichen = PreviousLicenceBox1.Text + "-" + PreviousLicenceBox2.Text + "-" + PreviousLicenceBox3.Text;
                        AbmeldungOkLabel.Visible = false;
                        if (!String.IsNullOrEmpty(FirstRegistrationDateBox.SelectedDate.ToString()))
                            FirstRegistrationDate = FirstRegistrationDateBox.SelectedDate;
                        if (!String.IsNullOrEmpty(CostCenterId))
                            costCenterId = Int32.Parse(CostCenterId);
                        if (!String.IsNullOrEmpty(Vehicle_ColorBox.Text))
                            color = Convert.ToInt32(Vehicle_ColorBox.Text);
                        if (!String.IsNullOrEmpty(vehicleIdField.Value)) //falls Auto schon gefunden wurde
                        {
                            newVehicle = dbContext.Vehicle.Single(q => q.Id == Int32.Parse(vehicleIdField.Value));

                            if (newVehicle.CurrentRegistrationId.HasValue)
                            {
                                newRegistration = dbContext.Registration.Single(q => q.Id == newVehicle.CurrentRegistrationId.Value);
                            }
                        }
                        else // falls ein neues Auto soll erstellt werden
                        {
                            newVehicle = Vehicle.CreateVehicle(VINBox.Text, HSNAbmBox.Text, TSNAbmBox.Text, Vehicle_VariantBox.Text, FirstRegistrationDate, color, dbContext);
                            newAdress = Adress.CreateAdress(Adress_StreetBox.Text, Adress_StreetNumberBox.Text, Adress_ZipcodeBox.Text, Adress_CityBox.Text, Adress_CountryBox.Text, dbContext);
                            newContact = Contact.CreateContact(Contact_PhoneBox.Text, Contact_FaxBox.Text, Contact_MobilePhoneBox.Text, Contact_EmailBox.Text, dbContext);
                            newBankAccount = BankAccount.CreateBankAccount(dbContext, BankAccount_BankNameBox.Text, BankAccount_AccountnumberBox.Text,
                                BankAccount_BankCodeBox.Text, txbBancAccountIban.Text, txbBankAccount_Bic.Text);
                            newCarOwner = CarOwner.CreateCarOwner(CarOwner_NameBox.Text, CarOwner_FirstnameBox.Text, newBankAccount, newContact, newAdress, dbContext);
                            DateTime newAbmeldeDatum = DateTime.Now;
                            if (AbmeldedatumPicker.SelectedDate != null)
                            {
                                if (!string.IsNullOrEmpty(AbmeldedatumPicker.SelectedDate.ToString()))
                                {
                                    newAbmeldeDatum = (DateTime)AbmeldedatumPicker.SelectedDate;
                                }
                            }
                            newRegistration = Registration.CreateRegistration(newCarOwner, newVehicle, kennzeichen, Registration_eVBNumberBox.Text,
                                Registration_GeneralInspectionDateBox.SelectedDate, newAbmeldeDatum, RegDocNumBox.Text, EmissionsCodeBox.Text, dbContext);
                        }
                        //weitere Logik für die Abmeldung 
                        price = FindPrice(ProduktId);
                        if (price == null)
                        {
                            ErrorLeereTextBoxenLabel.Text = "Kein Preis gefunden!";
                            ErrorLeereTextBoxenLabel.Visible = true;
                            return;
                        }

                        CostCenter costCenter = null;
                        if (costCenterId.HasValue)
                        {
                            costCenter = dbContext.CostCenter.FirstOrDefault(o => o.Id == costCenterId.Value);
                        }

                        //neues DeregistrationOrder erstellen
                        newDeregOrder = DeregistrationOrder.CreateDeregistrationOrder(Int32.Parse(Session["CurrentUserId"].ToString()),
                            Int32.Parse(CustomerDropDownList.SelectedValue), newVehicle, newRegistration, locationId, Int32.Parse(ZulassungsstelleComboBox.SelectedValue), dbContext);
                        //adding new Deregestrationorder Items
                        newOrderItem1 = newDeregOrder.Order.AddOrderItem(Int32.Parse(ProduktId), price.Amount, 1, costCenter, null, false, dbContext);
                        if (price.AuthorativeCharge.HasValue)
                        {
                            newOrderItem2 = newDeregOrder.Order.AddOrderItem(Int32.Parse(ProduktId), price.AuthorativeCharge.Value, 1, costCenter, newOrderItem1.Id, true, dbContext);
                        }
                        dbContext.SubmitChanges();
                        if (DienstleistungTreeView.Nodes.Count > 1)
                        {
                            bool inOrdnung = AddAnotherProducts(newDeregOrder, locationId);
                        }
                        if (String.IsNullOrEmpty(vehicleIdField.Value)) //falls Auto schon gefunden wurde
                        {
                            newVehicle.CurrentRegistrationId = newRegistration.Id;
                            dbContext.SubmitChanges();
                        }
                        AbmeldungOkLabel.Visible = true;
                        if (((RadButton)(sender)).ID != "rbtSameOrder")
                            MakeAllControlsEmpty();
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "finishedMessage", "alert('Auftrag wurde erfolgreich angelegt.');", true);
                    }
                    catch (Exception ex)
                    {
                        SubmitChangesErrorLabel.Visible = true;
                        SubmitChangesErrorLabel.Text = "Fehler:" + ex.Message;
                    }
                }
            }
        }

        // findet alle angezeigte textboxen und überprüft ob die nicht leer sind
        protected bool CheckIfBoxenNotEmpty()
        {
            bool gibtsBoxenDieLeerSind = false;
            bool iFound1VisibleBox = false;
            bool carOwnerMin = false;
            int count = 0;
            List<Control> allControls = GetAllControls();
            //fallse leer - soll aus der Logik rausgenommen
            if (String.IsNullOrEmpty(PruefzifferBox.Text))
                PruefzifferBox.Enabled = false;
            if (String.IsNullOrEmpty(CostCenterDropDownList.SelectedValue) || String.IsNullOrEmpty(LocationDropDownList.SelectedValue) || DienstleistungTreeView.Nodes.Count == 0 || String.IsNullOrEmpty(ZulassungsstelleComboBox.SelectedValue))
            {
                return true;
            }
            foreach (Control control in allControls)
            {
                if (control.Visible == true)
                {
                    iFound1VisibleBox = true;
                    foreach (Control subControl in control.Controls)
                    {
                        if (subControl is RadTextBox)
                        {
                            RadTextBox box = subControl as RadTextBox;
                            if (box.ID == "CarOwner_NameBox" || box.ID == "CarOwner_FirstnameBox")
                            {

                                if (box.Text != string.Empty)
                                {
                                    carOwnerMin = true;
                                }
                                else if (!carOwnerMin)
                                {
                                    count++;
                                }
                                else
                                {
                                    box.BorderColor = System.Drawing.Color.Black;
                                }
                                if (count > 1)
                                {
                                    box.BorderColor = System.Drawing.Color.Red;
                                    gibtsBoxenDieLeerSind = true;

                                }
                                continue;
                            }
                            if (box.Enabled == true && String.IsNullOrEmpty(box.Text))
                            {
                                box.BorderColor = System.Drawing.Color.Red;
                                gibtsBoxenDieLeerSind = true;
                                //break;
                            }
                            else
                            {
                                box.BorderColor = System.Drawing.Color.Black;
                            }
                        }
                    }
                }
            }
            if (iFound1VisibleBox == false)
                gibtsBoxenDieLeerSind = true;
            return gibtsBoxenDieLeerSind;
        }
        // findet alle textboxen und macht die leer ohne die ganze Seite neu zu laden
        protected void NaechtenAuftragButton_Clicked(object sender, EventArgs e)
        {
            MakeAllControlsEmpty();
        }
        //VIN ist eingegeben, versuch das Fahrzeug zu finden
        protected void VinBoxText_Changed(object sender, EventArgs e)
        {
            bool finIsOkey = false;
            FahrzeugLabel.Text = "Fahrzeug";
            FahrzeugLabel.ForeColor = System.Drawing.Color.Blue;
            if (VINBox.Text.Length == 18 && !VINBox.Text.Contains('O') && !VINBox.Text.Contains('o'))
            {
                finIsOkey = true;
                PruefzifferBox.Text = VINBox.Text.Substring(17);
                VINBox.Text = VINBox.Text.Substring(0, 17);
                PruefzifferBox.Focus();
            }
            else if (VINBox.Text.Length == 17 && !VINBox.Text.Contains('O') && !VINBox.Text.Contains('o'))
            {
                finIsOkey = true;
                PruefzifferBox.Focus();
            }
            else if (VINBox.Text.Length == 8)
            {
                finIsOkey = true;
                PruefzifferBox.Focus();
            }
            else // fin ist nicht korrekt
            {
                if (VINBox.Text.Contains('O') || VINBox.Text.Contains('o'))
                {
                    FahrzeugLabel.Text = "FIN darf nicht 'O' oder 'o' beinhalten!";
                    FahrzeugLabel.ForeColor = System.Drawing.Color.Red;
                    VINBox.Focus();
                }
                if (VINBox.Text.Length > 18 || VINBox.Text.Length < 17)
                {
                    FahrzeugLabel.Text = "FIN kann nur entweder 17, 18(mit Prüfzifern) oder 8-stellig sein!";
                    FahrzeugLabel.ForeColor = System.Drawing.Color.Red;
                    VINBox.Focus();
                }
            }

            if (finIsOkey == true)
            {
                VINBox.Text = VINBox.Text.ToUpper();
                FahrzeugLabel.Visible = false;

                var vehicle = VehicleManager.GetEntities(q => q.VIN == VINBox.Text).FirstOrDefault();
                vehicleIdField.Value = vehicle.Id.ToString();
                if (vehicle.CurrentRegistrationId.HasValue)
                {
                    var registration = RegistrationManager.GetById(vehicle.CurrentRegistrationId.Value);

                    VINBox.Text = VINBox.Text;
                    Vehicle_VariantBox.Text = vehicle.Variant;
                    var kennzeichen = registration.Licencenumber;
                    string[] newKennzeichen = kennzeichen.Split('-');

                    if (newKennzeichen.Length == 3)
                    {
                        LicenceBox1.Text = newKennzeichen[0];
                        LicenceBox2.Text = newKennzeichen[1];
                        LicenceBox3.Text = newKennzeichen[2];
                    }

                    Registration_GeneralInspectionDateBox.SelectedDate = registration.GeneralInspectionDate;
                    Vehicle_VariantBox.Text = vehicle.Variant;
                    HSNAbmBox.Text = vehicle.HSN;
                    TSNAbmBox.Text = vehicle.TSN;
                    Vehicle_ColorBox.Text = vehicle.ColorCode.ToString();
                    RegDocNumBox.Text = registration.RegistrationDocumentNumber;
                    EmissionsCodeBox.Text = registration.EmissionCode;
                    CarOwner_NameBox.Text = registration.CarOwner.Name;
                    Adress_StreetBox.Text = registration.CarOwner.Adress.Street;
                    CarOwner_FirstnameBox.Text = registration.CarOwner.FirstName;
                    Adress_StreetNumberBox.Text = registration.CarOwner.Adress.StreetNumber;
                    Adress_ZipcodeBox.Text = registration.CarOwner.Adress.Zipcode;
                    Adress_CityBox.Text = registration.CarOwner.Adress.City;
                    Adress_CountryBox.Text = registration.CarOwner.Adress.Country;
                    Contact_PhoneBox.Text = registration.CarOwner.Contact.Phone;
                    Contact_FaxBox.Text = registration.CarOwner.Contact.Fax;
                    Contact_MobilePhoneBox.Text = registration.CarOwner.Contact.MobilePhone;
                    Contact_EmailBox.Text = registration.CarOwner.Contact.Email;
                    BankAccount_BankNameBox.Text = registration.CarOwner.BankAccount.BankName;
                    BankAccount_AccountnumberBox.Text = registration.CarOwner.BankAccount.Accountnumber;
                    BankAccount_BankCodeBox.Text = registration.CarOwner.BankAccount.BankCode;
                    PruefzifferBox.Focus();
                }
            }
        }

        protected void CostCenterLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetCostCenters();
        }

        #endregion
        protected void ProductAbmDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            var selectedCustomer = 0;
            var location = 0;
            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue))
                selectedCustomer = Int32.Parse(CustomerDropDownList.SelectedValue);
            if (!String.IsNullOrEmpty(LocationDropDownList.SelectedValue))
                location = Int32.Parse(LocationDropDownList.SelectedValue);

            var products = PriceManager.GetEntities(o => o.PriceAccount.Count != 0 &&
             (o.Product.OrderTypeId == (int)OrderTypes.Cancellation || 
                o.Product.OrderType.Id == (int)OrderTypes.Common)).
                    Select(o => new
                    {
                        ItemNumber = o.Product.ItemNumber,
                        Name = o.Product.Name,
                        Value = o.Product.Id,
                        Category = o.Product.ProductCategory.Name
                    }).OrderBy(o => o.Name).ToList();

            if (products.Count != 0 && location != 0 && selectedCustomer != 0)
            {
                LoadCustomerProductsInTreeView(selectedCustomer, location);
            }

            e.Result = products;
        }

        #region Index Changed
        protected void SmallLargeCustomerIndex_Changed(object sender, EventArgs e)
        {
            CustomerDropDownList.Enabled = true;
            CustomerDropDownList.DataBind();
            LocationDropDownList.Enabled = true;
            CostCenterDropDownList.Enabled = true;
            ProductAbmDropDownList.DataSource = null;
            ProductAbmDropDownList.DataBind();
        }
        protected void CustomerIndex_Changed(object sender, EventArgs e)
        {
            ProductAbmDropDownList.Text = null;
            ProductAbmDropDownList.ClearSelection();
            LocationDropDownList.Text = null;
            LocationDropDownList.ClearSelection();
            CostCenterDropDownList.Text = null;
            CostCenterDropDownList.ClearSelection();
            LocationDropDownList.DataBind();
            ProductAbmDropDownList.DataBind();
            CostCenterDropDownList.DataBind();
            AbmeldungOkLabel.Visible = false;
            Adress_StreetBox.Text = string.Empty;
            Adress_StreetNumberBox.Text = string.Empty;
            Adress_ZipcodeBox.Text = string.Empty;
            Adress_CityBox.Text = string.Empty;
            Adress_CountryBox.Text = string.Empty;
            CarOwner_NameBox.Text = string.Empty;
            Registration_eVBNumberBox.Text = string.Empty;
            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue.ToString()))
            {
                CheckFields();
                SmallCustomerHistorie.Visible = false;
            }
        }
        protected void LocationDropDownIndex_Changed(object sender, EventArgs e)
        {
            ProductAbmDropDownList.DataSource = null;
            ProductAbmDropDownList.DataBind();
            SetCarOwnerData();
        }
        protected bool AddAnotherProducts(DeregistrationOrder deRegOrd, int? locationId)
        {
            bool allesHatGutGelaufen = false;
            string ProduktId = "";
            string CostCenterId = "";
            int skipFirst = 0;
            foreach (RadTreeNode node in DienstleistungTreeView.Nodes)
            {
                if (skipFirst > 0)
                {
                    if (!String.IsNullOrEmpty(node.Value))
                    {
                        string[] splited = node.Value.Split(';');
                        if (splited.Length == 2)
                        {
                            try
                            {
                                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                                var orderNumber = deRegOrd.OrderNumber;
                                Price newPrice;
                                OrderItem newOrderItem1 = null;

                                ProduktId = splited[0];
                                CostCenterId = splited[1];
                                if (!String.IsNullOrEmpty(ProduktId))
                                {
                                    var productId = Int32.Parse(ProduktId);
                                    KVSCommon.Database.Product newProduct = dbContext.Product.SingleOrDefault(q => q.Id == productId);
      

                                    CostCenter costCenter = null;
                                    if (!String.IsNullOrEmpty(CostCenterId))
                                    {
                                        costCenter = dbContext.CostCenter.FirstOrDefault(o => o.Id == Int32.Parse(CostCenterId));
                                    }

                                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == locationId);
                                    if (newPrice == null)
                                        newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == null);
                                    var orderToUpdate = dbContext.Order.SingleOrDefault(q => q.OrderNumber == orderNumber);
                                    orderToUpdate.LogDBContext = dbContext;
                                    if (orderToUpdate != null)
                                    {
                                        newOrderItem1 = orderToUpdate.AddOrderItem(newProduct.Id, newPrice.Amount, 1, costCenter, null, false, dbContext);
                                        if (newPrice.AuthorativeCharge.HasValue)
                                        {
                                            orderToUpdate.AddOrderItem(newProduct.Id, newPrice.AuthorativeCharge.Value, 1, costCenter, newOrderItem1.Id, 
                                                newPrice.AuthorativeCharge.HasValue, dbContext);
                                        }
                                        dbContext.SubmitChanges();
                                        allesHatGutGelaufen = true;
                                    }
                                }
                                if (allesHatGutGelaufen)
                                    dbContext.SubmitChanges();
                            }
                            catch
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    skipFirst = 1;
                }
            }
            return allesHatGutGelaufen;
        }
        #endregion
        #region Linq Data Sources

        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetAllSmallCustomers();
        }
        #endregion

        protected void DeleteNewPosButton_Clicked(object sender, EventArgs e)
        {
            if (DienstleistungTreeView.SelectedNodes.Count > 0)
            {
                DienstleistungTreeView.SelectedNode.Remove();
            }
        }
        protected void NewPosButton_Clicked(object sender, EventArgs e)
        {
            IRadTreeNodeContainer target = DienstleistungTreeView;
            if (DienstleistungTreeView.SelectedNode != null)
            {
                DienstleistungTreeView.SelectedNode.Expanded = true;
                target = DienstleistungTreeView.SelectedNode;
            }
            if (!String.IsNullOrEmpty(ProductAbmDropDownList.Text) && !String.IsNullOrEmpty(ProductAbmDropDownList.SelectedValue))
            {
                string costCenter = "";
                if (!String.IsNullOrEmpty(CostCenterDropDownList.SelectedValue.ToString()))
                {
                    costCenter = (CostCenterDropDownList.SelectedValue.ToString());
                }
                else
                    costCenter = "";
                string value = ProductAbmDropDownList.SelectedValue.ToString() + ";" + costCenter;
                RadTreeNode addedNode = new RadTreeNode(ProductAbmDropDownList.Text + "(" + CostCenterDropDownList.Text + ")", value);//+ ";" + CostCenterDropDownList.SelectedValue == "SmallCustomer" ? "" : CostCenterDropDownList.SelectedValue);
                target.Nodes.Add(addedNode);
            }
        }


        #endregion


        private void LoadCustomerProductsInTreeView(int selectedCustomer, int location)
        {
            var products = PriceManager.GetEntities(o => o.PriceAccount.Count != 0 &&
                o.Product.CustomerProduct.Any() &&
                o.Location != null && o.LocationId == location && o.Location.CustomerId == selectedCustomer &&
                    (o.Product.OrderType.Id == (int)OrderTypes.Cancellation ||
                     o.Product.OrderType.Id == (int)OrderTypes.Common)).
                    Select(o => new
                    {
                        ItemNumber = o.Product.ItemNumber,
                        Name = o.Product.Name,
                        Value = o.Product.Id,
                        Category = o.Product.ProductCategory.Name,
                        //CustomerProduct = o.Product.CustomerProduct.FirstOrDefault().Product.Name
                    }).OrderBy(o => o.Name).ToList();


            foreach (var product in products)
            {
                IRadTreeNodeContainer target = DienstleistungTreeView;

                string costCenter = "";
                if (!String.IsNullOrEmpty(CostCenterDropDownList.SelectedValue.ToString()))
                {
                    costCenter = (CostCenterDropDownList.SelectedValue.ToString());
                }

                string value = product.Value + ";" + costCenter;

                RadTreeNode addedNode = new RadTreeNode(product.Name, value);
                target.Nodes.Add(addedNode);
            }
        }
    }
}