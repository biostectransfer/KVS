using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.IO;
using System.Configuration;
using System.Transactions;
using KVSCommon.Enums;
using KVSCommon.Managers;
namespace KVSWebApplication.Auftragseingang
{
    /// <summary>
    /// Abmeldung Laufkunde
    /// </summary>
    public partial class AbmeldungLaufkundeControl : CancellationOrderBase
    {
        #region Members

        protected override Label CustomerHistoryLabel { get { return this.SmallCustomerHistorie; } }
        protected override RadTreeView ProductTree { get { return DienstleistungTreeView; } }
        protected override RadScriptManager RadScriptManager { get { return ((AbmeldungLaufkunde)Page).getScriptManager(); } }
        protected override RadNumericTextBox Discount { get { return this.txbDiscount; } }
        protected override HiddenField SmallCustomerOrder { get { return this.smallCustomerOrderHiddenField; } }
        protected override HiddenField VehicleId { get { return this.vehicleIdField; } }
        protected override RadWindow RadWindow { get { return this.AddAdressRadWindow; } }

        #region Dates

        protected override RadMonthYearPicker Registration_GeneralInspectionDatePicker { get { return this.Registration_GeneralInspectionDateBox; } }
        protected override RadDatePicker FirstRegistrationDatePicker { get { return this.FirstRegistrationDateBox; } }
        protected override RadDatePicker RegistrationDatePicker { get { return null; } }

        #endregion

        #region DropDowns

        protected override RadComboBox CustomerDropDown { get { return this.CustomerDropDownList; } }
        protected override RadComboBox LocationDropDown { get { return null; } }
        protected override RadComboBox CostCenterDropDown { get { return null; } }
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
        protected override RadTextBox VIN_TextBox { get { return this.VINBox; } }
        protected override RadTextBox HSN_TextBox { get { return this.HSNAbmBox; } }
        protected override RadTextBox TSN_TextBox { get { return this.TSNAbmBox; } }
        protected override RadTextBox Variant_TextBox { get { return this.Vehicle_VariantBox; } }
        protected override RadTextBox Color_TextBox { get { return this.Vehicle_ColorBox; } }
        protected override RadTextBox Contact_Phone_TextBox { get { return this.Contact_PhoneBox; } }
        protected override RadTextBox Contact_Fax_TextBox { get { return this.Contact_FaxBox; } }
        protected override RadTextBox Contact_MobilePhone_TextBox { get { return this.Contact_MobilePhoneBox; } }
        protected override RadTextBox Contact_Email_TextBox { get { return this.Contact_EmailBox; } }
        protected override RadTextBox EmissionsCode_TextBox { get { return this.EmissionsCodeBox; } }
        protected override RadTextBox RegistrationDocumentNumber_TextBox { get { return this.RegDocNumBox; } }
        protected override RadTextBox FreeTextBox { get { return this.FreiTextBox; } }
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
        protected override Panel FreeText_Panel { get { return this.Order_Freitext; } }

        #endregion

        #region Labels

        protected override Label FahrzeugCaption { get { return this.FahrzeugLabel; } }
        protected override Label HalterCaption { get { return this.HalterLabel; } }
        protected override Label HalterdatenCaption { get { return this.HalterdatenLabel; } }
        protected override Label KontaktdatenCaption { get { return this.KontaktdatenLabel; } }
        protected override Label HSNSearchCaption { get { return this.HSNSearchLabel; } }
        protected override Label ErrorLeereTextBoxenCaption { get { return this.ErrorLeereTextBoxenLabel; } }
        protected override Label AdditionalInfoCaption { get { return this.ZusatzlicheInfoLabel; } }
        protected override Label LocationWindowCaption { get { return this.LocationLabelWindow; } }
        #endregion

        #endregion

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            RadScriptManager.RegisterPostBackControl(AddAdressButton);
            LicenceBox1.Enabled = true;
            LicenceBox2.Enabled = true;
            LicenceBox3.Enabled = true;
            //first registration get today date by default
            FirstRegistrationDateBox.SelectedDate = DateTime.Now;
            string target = Request["__EVENTARGUMENT"];
            if (target != null && target == "CreateOrder")
            {
                AbmeldenButton_Clicked(sender, e);
            }
            if (Session["CurrentUserId"] != null && !String.IsNullOrEmpty(Session["CurrentUserId"].ToString()))
            {
                CheckUserPermissions();

                if (!Page.IsPostBack)
                {
                    ShowControls();
                    btnClearSelection_Click(this, null);
                }
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
                string value = ProductAbmDropDownList.SelectedValue.ToString() + ";" + costCenter;
                var addedNode = new RadTreeNode(ProductAbmDropDownList.Text, value);
                target.Nodes.Add(addedNode);
            }
        }

        protected void DeleteNewPosButton_Clicked(object sender, EventArgs e)
        {
            if (DienstleistungTreeView.SelectedNodes.Count > 0)
            {
                DienstleistungTreeView.SelectedNode.Remove();
            }
        }

        protected void btnClearSelection_Click(object sender, EventArgs e)
        {
            ShowControls();
            CustomerDropDownList.ClearSelection();
            MakeAllControlsEmpty();
            ClearAdressData(string.Empty);

            var maxCustomerNumber = CustomerManager.GetEntities().Max(o => o.CustomerNumber);
            txbSmallCustomerNumber.Text = EmptyStringIfNull.generateIndividualNumber(maxCustomerNumber);

            setCustomerTXBEnable(true);
        }

        #endregion

        #region Button Clicked
        //Fahrzeug abmelden
        protected void AbmeldenButton_Clicked(object sender, EventArgs e)
        {
            AbmeldungOkLabel.Visible = false;
            SubmitChangesErrorLabel.Visible = false;
            ErrorLeereTextBoxenLabel.Visible = false;

            if (CheckRegistrationFields()) //exists empty required fields
            { 
                var productsPriceCheck = CheckIfAllProduktsHavingPrice(null);
                if (!String.IsNullOrEmpty(productsPriceCheck))
                {
                    ErrorLeereTextBoxenLabel.Text = String.Format("Für {0} wurde kein Price gefunden!", productsPriceCheck);
                    ErrorLeereTextBoxenLabel.Visible = true;
                    return;
                }

                if (DienstleistungTreeView.Nodes.Count > 0)
                {
                    AddCustomer();
                    ErrorLeereTextBoxenLabel.Visible = false;
                    SubmitChangesErrorLabel.Visible = false;
                    RadTreeNode node = DienstleistungTreeView.Nodes[0];
                    string[] splited = node.Value.Split(';');
                    var produktId = Int32.Parse(splited[0]);

                    try
                    {
                        AbmeldungOkLabel.Visible = false;
                        string licenceNumber = string.Empty;
                        var oldLicenceNumber = string.Empty;
                        Registration newRegistration = null;
                        Vehicle vehicle = null;


                        if (!String.IsNullOrEmpty(LicenceBox1.Text))
                            licenceNumber = LicenceBox1.Text + "-" + LicenceBox2.Text + "-" + LicenceBox3.Text;
                        if (!String.IsNullOrEmpty(PreviousLicenceBox1.Text))
                            oldLicenceNumber = PreviousLicenceBox1.Text + "-" + PreviousLicenceBox2.Text + "-" + PreviousLicenceBox3.Text;
                        

                        if (!String.IsNullOrEmpty(vehicleIdField.Value)) //falls Auto schon gefunden wurde
                        {
                            vehicle = VehicleManager.GetById(Int32.Parse(vehicleIdField.Value));

                            if (vehicle.CurrentRegistrationId.HasValue)
                            {
                                newRegistration = RegistrationManager.GetById(vehicle.CurrentRegistrationId.Value);
                            }
                        }
                        else // falls ein neues Auto soll erstellt werden
                        {
                            vehicle = GetVehicle();
                            var newCarOwner = GetCarOwner();

                            DateTime newAbmeldeDatum = DateTime.Now;
                            if (AbmeldedatumPicker.SelectedDate != null)
                            {
                                if (!string.IsNullOrEmpty(AbmeldedatumPicker.SelectedDate.ToString()))
                                {
                                    newAbmeldeDatum = (DateTime)AbmeldedatumPicker.SelectedDate;
                                }
                            }

                            newRegistration = RegistrationManager.CreateRegistration(newCarOwner, vehicle, licenceNumber, Registration_eVBNumberBox.Text,
                                Registration_GeneralInspectionDateBox.SelectedDate, newAbmeldeDatum, RegDocNumBox.Text, EmissionsCodeBox.Text);
                        }
                        
                        
                        //neues DeregistrationOrder erstellen
                        var newDeregOrder = DeregistrationOrderManager.CreateDeregistrationOrder(Int32.Parse(CustomerDropDownList.SelectedValue), vehicle, newRegistration,
                            null, Int32.Parse(ZulassungsstelleComboBox.SelectedValue));
                        
                        //adding new Deregestrationorder Items
                        AddAnotherProducts(newDeregOrder, null);

                        if (!String.IsNullOrEmpty(vehicleIdField.Value)) //falls Auto schon gefunden wurde
                        {
                            vehicle.CurrentRegistrationId = newRegistration.Id;
                            VehicleManager.SaveChanges();
                        }

                        if (invoiceNow.Checked == true && invoiceNow.Enabled == true)
                        {
                            MakeInvoiceForSmallCustomer(newDeregOrder.Id);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "finishedMessage", "alert('Auftrag wurde erfolgreich angelegt.');", true);
                            MakeAllControlsEmpty();
                        }
                    }
                    catch (Exception ex)
                    {
                        SubmitChangesErrorLabel.Visible = true;
                        SubmitChangesErrorLabel.Text = "Fehler: " + ex.Message;
                    }
                }
            }
        }

        // findet alle textboxen und macht die leer ohne die ganze Seite neu zu laden
        protected override void MakeSpecialControlsEmpty()
        {            
            txbSmallCustomerZahlungsziel.Text = "";
            txbSmallCustomerVorname.Text = "";
            txbSmallCustomerNachname.Text = "";
            txbSmallCustomerTitle.Text = "";
            cmbSmallCustomerGender.Text = "";
            txbSmallCustomerStreet.Text = "";
            txbSmallCustomerNr.Text = "";
            txbSmallCustomerZipCode.Text = "";
            cmbSmallCustomerCity.Text = "";
            txbSmallCustomerPhone.Text = "";
            txbSmallCustomerFax.Text = "";
            txbSmallCustomerEmail.Text = "";
            txbSmallCustomerNumber.Text = "";
            txbSmallCustomerMobil.Text = "";
        }

        // findet alle angezeigte textboxen und überprüft ob die nicht leer sind
        protected override bool CheckIfBoxenEmpty()
        {
            bool gibtsBoxenDieLeerSind = false;
            List<Control> allControls = GetAllControls();

            //fallse leer - soll aus der Logik rausgenommen
            if (String.IsNullOrEmpty(PruefzifferBox.Text))
                PruefzifferBox.Enabled = false;


            if (DienstleistungTreeView.Nodes.Count == 0)
            {
                return true;
            }

            if (!PruefzifferBox.Enabled)
                PruefzifferBox.Enabled = true;
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
            FahrzeugLabel.ForeColor = System.Drawing.Color.FromArgb(234, 239, 244);
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

        #endregion

        #region Linq Data Source

        protected void ProductAbmDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            var products = PriceManager.GetEntities(o => o.PriceAccount.Count != 0 && o.Location == null &&
             o.Product.OrderTypeId == (int)OrderTypes.Cancellation).
                    Select(o => new
                    {
                        ItemNumber = Int32.Parse(o.Product.ItemNumber),
                        Name = o.Product.Name,
                        Value = o.Product.Id,
                        Category = o.Product.ProductCategory.Name,
                        Price = Math.Round(o.Amount, 2, MidpointRounding.AwayFromZero).ToString(),
                        AuthCharge = o.AuthorativeCharge.HasValue ? Math.Round(o.AuthorativeCharge.Value, 2, MidpointRounding.AwayFromZero).ToString().Trim() : ""
                    }).OrderBy(o => o.ItemNumber).ToList();
 
            e.Result = products;
        }

        protected void CustomerIndex_Changed(object sender, EventArgs e)
        {
            ProductAbmDropDownList.Text = null;
            ProductAbmDropDownList.ClearSelection();
            ProductAbmDropDownList.DataBind();
            DienstleistungTreeView.Nodes.Clear();
            AbmeldungOkLabel.Visible = false;
            GetCustomerInfo();
            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue.ToString()))
            {
                CheckUmsatzForSmallCustomer();
            }
        }

        protected void LocationDropDownIndex_Changed(object sender, EventArgs e)
        {
            ProductAbmDropDownList.DataSource = null;
            ProductAbmDropDownList.DataBind();
        }

        protected void AddAnotherProducts(DeregistrationOrder regOrd, int? locationId)
        {
            string productId = "";
            int itemIndexValue = 0;
            decimal amount = 0;

            var nodes = this.Request.Form.AllKeys.Where(q => q.Contains("txtItemPrice_"));
            foreach (var node in nodes)
            {
                if (!String.IsNullOrEmpty(node))
                {
                    Price newPrice = null;
                    productId = node.Split('_')[1];

                    if (!String.IsNullOrEmpty(productId))
                    {
                        var newProduct = ProductManager.GetById(Int32.Parse(productId));
                        if (locationId == null) //small
                        {
                            newPrice = PriceManager.GetEntities(q => q.ProductId == newProduct.Id && q.LocationId == null).FirstOrDefault();
                        }

                        var orderToUpdate = OrderManager.GetById(regOrd.Id);

                        if (orderToUpdate != null)
                        {
                            if (this.Request.Form.GetValues(node) != null)
                            {
                                string itemPrice = this.Request.Form.GetValues(node)[0];
                                if (EmptyStringIfNull.IsNumber(itemPrice))
                                {
                                    amount = Convert.ToDecimal(itemPrice.Replace('.', ','));
                                }
                                else if (itemPrice == "")
                                {
                                    amount = 0;
                                }
                                else
                                {
                                    amount = newPrice.Amount;
                                }
                            }
                            else
                            {
                                amount = newPrice.Amount;
                            }

                            var newOrderItem = OrderManager.AddOrderItem(orderToUpdate, newProduct.Id, amount, 1, null, null, false);
                            if (newPrice.AuthorativeCharge.HasValue)
                            {
                                if (this.Request.Form.GetValues(node.Replace("txtItemPrice_", "txtAuthPrice_")) != null)
                                {
                                    string itemPrice = this.Request.Form.GetValues(node.Replace("txtItemPrice_", "txtAuthPrice_"))[0];
                                    if (EmptyStringIfNull.IsNumber(itemPrice))
                                    {
                                        amount = Convert.ToDecimal(itemPrice.Replace('.', ','));
                                    }
                                    else if (itemPrice == "")
                                    {
                                        amount = 0;
                                    }
                                    else
                                    {
                                        amount = newPrice.AuthorativeCharge.Value;
                                    }
                                }
                                else
                                {
                                    amount = newPrice.AuthorativeCharge.Value;
                                }

                                OrderManager.AddOrderItem(orderToUpdate, newProduct.Id, amount, 1, null, newOrderItem.Id, newPrice.AuthorativeCharge.HasValue);
                            }
                            itemIndexValue = itemIndexValue + 1;
                        }
                    }
                }
            }
        }

        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetAllSmallCustomers();
        }

        #endregion

        #region Methods
        
        protected void setCustomerTXBEnable(bool value)
        {
            txbSmallCustomerVat.Enabled = value;
            txbSmallCustomerZahlungsziel.Enabled = value;
            txbSmallCustomerVorname.Enabled = value;
            txbSmallCustomerNachname.Enabled = value;
            txbSmallCustomerTitle.Enabled = value;
            cmbSmallCustomerGender.Enabled = value;
            txbSmallCustomerStreet.Enabled = value;
            txbSmallCustomerNr.Enabled = value;
            txbSmallCustomerZipCode.Enabled = value;
            cmbSmallCustomerCity.Enabled = value;
            txbSmallCustomerCountry.Enabled = value;
            txbSmallCustomerPhone.Enabled = value;
            txbSmallCustomerFax.Enabled = value;
            txbSmallCustomerEmail.Enabled = value;
            txbSmallCustomerNumber.Enabled = value;
            txbSmallCustomerMobil.Enabled = value;
        }

        protected void ClearAdressData(string value)
        {
            txbSmallCustomerZahlungsziel.Text = value;
            txbSmallCustomerVorname.Text = value;
            txbSmallCustomerNachname.Text = value;
            txbSmallCustomerTitle.Text = value;
            cmbSmallCustomerGender.Text = value;
            txbSmallCustomerStreet.Text = value;
            txbSmallCustomerNr.Text = value;
            txbSmallCustomerZipCode.Text = value;
            cmbSmallCustomerCity.Text = value;
            txbSmallCustomerPhone.Text = value;
            txbSmallCustomerFax.Text = value;
            txbSmallCustomerEmail.Text = value;
            txbSmallCustomerNumber.Text = value;
            txbSmallCustomerMobil.Text = value;
        }

        protected void GetCustomerInfo()
        {
            try
            {
                var customerId = 0;
                if (CustomerDropDownList.SelectedValue != string.Empty)
                    customerId = Int32.Parse(CustomerDropDownList.SelectedValue);

                var checkThisCustomer = CustomerManager.GetEntities(q => q.Id == customerId).SingleOrDefault();
                if (checkThisCustomer != null)
                {
                    //Kundendaten
                    txbSmallCustomerVat.Text = checkThisCustomer.VAT.ToString();
                    txbSmallCustomerZahlungsziel.Text = checkThisCustomer.TermOfCredit != null ? checkThisCustomer.TermOfCredit.Value.ToString() : "";
                    txbSmallCustomerVorname.Text = checkThisCustomer.Name;
                    txbSmallCustomerNachname.Text = checkThisCustomer.SmallCustomer.Person != null ? checkThisCustomer.SmallCustomer.Person.FirstName : "";
                    txbSmallCustomerTitle.Text = checkThisCustomer.SmallCustomer.Person != null ? checkThisCustomer.SmallCustomer.Person.Title : "";
                    cmbSmallCustomerGender.Text = checkThisCustomer.SmallCustomer.Person != null ? checkThisCustomer.SmallCustomer.Person.Gender : "";
                    txbSmallCustomerStreet.Text = checkThisCustomer.Adress != null ? checkThisCustomer.Adress.Street : "";
                    txbSmallCustomerNr.Text = checkThisCustomer.Adress != null ? checkThisCustomer.Adress.StreetNumber : "";
                    txbSmallCustomerZipCode.Text = checkThisCustomer.Adress != null ? checkThisCustomer.Adress.Zipcode : "";
                    cmbSmallCustomerCity.Text = checkThisCustomer.Adress != null ? checkThisCustomer.Adress.City : "";
                    txbSmallCustomerCountry.Text = checkThisCustomer.Adress != null ? checkThisCustomer.Adress.Country : "";
                    txbSmallCustomerPhone.Text = checkThisCustomer.Contact != null ? checkThisCustomer.Contact.Phone : "";
                    txbSmallCustomerFax.Text = checkThisCustomer.Contact != null ? checkThisCustomer.Contact.Fax : "";
                    txbSmallCustomerEmail.Text = checkThisCustomer.Contact != null ? checkThisCustomer.Contact.Email : "";
                    txbSmallCustomerNumber.Text = checkThisCustomer.CustomerNumber;
                    //Halterdaten
                    CarOwner_FirstnameBox.Text = txbSmallCustomerVorname.Text;
                    CarOwner_NameBox.Text = txbSmallCustomerNachname.Text;
                    Adress_StreetNumberBox.Text = txbSmallCustomerNr.Text;
                    Adress_StreetBox.Text = txbSmallCustomerStreet.Text;
                    Adress_ZipcodeBox.Text = txbSmallCustomerZipCode.Text;
                    Adress_CityBox.Text = cmbSmallCustomerCity.Text;
                    Adress_CountryBox.Text = txbSmallCustomerCountry.Text;
                }
                setCustomerTXBEnable(false);
            }
            catch (Exception ex)
            {
                ErrorLeereTextBoxenLabel.Text = "Fehler: " + ex.Message;
                ErrorLeereTextBoxenLabel.Visible = true;
            }
        }

        protected void AddCustomer()
        {
            if (CustomerDropDownList.SelectedValue == string.Empty)
            {
                try
                {
                    var checkThisCustomer = CustomerManager.GetEntities(q => q.CustomerNumber == txbSmallCustomerNumber.Text).SingleOrDefault();

                    while (checkThisCustomer != null)
                    {
                        var maxCustomerNumber = CustomerManager.GetEntities().Max(o => o.CustomerNumber);
                        txbSmallCustomerNumber.Text = EmptyStringIfNull.generateIndividualNumber(maxCustomerNumber);
                        checkThisCustomer = CustomerManager.GetEntities(q => q.CustomerNumber == txbSmallCustomerNumber.Text).SingleOrDefault();
                    }

                    if (checkThisCustomer == null)
                    {
                        decimal vat = 0;
                        txbSmallCustomerVat.Text = txbSmallCustomerVat.Text.Trim();
                        txbSmallCustomerVat.Text = txbSmallCustomerVat.Text.Replace('.', ',');
                        try
                        {
                            if (txbSmallCustomerVat.Text != string.Empty)
                                vat = decimal.Parse(txbSmallCustomerVat.Text);
                        }
                        catch
                        {
                            throw new Exception("Die MwSt muss eine Dezimalzahl sein");
                        }

                        int payment = 0;
                        txbSmallCustomerZahlungsziel.Text = txbSmallCustomerZahlungsziel.Text.Trim();
                        if (txbSmallCustomerZahlungsziel.Text != string.Empty)
                        {
                            try
                            {
                                payment = int.Parse(txbSmallCustomerZahlungsziel.SelectedValue);
                            }
                            catch
                            {
                                throw new Exception("Das Zahlungsziel muss eine Gleitkommazahl sein");
                            }
                        }

                        var newSmallCustomer = CustomerManager.CreateSmallCustomer(txbSmallCustomerVorname.Text, txbSmallCustomerNachname.Text, txbSmallCustomerTitle.Text, 
                            cmbSmallCustomerGender.SelectedValue, txbSmallCustomerStreet.Text, txbSmallCustomerNr.Text, txbSmallCustomerZipCode.Text, cmbSmallCustomerCity.Text, 
                            txbSmallCustomerCountry.Text, txbSmallCustomerPhone.Text, txbSmallCustomerFax.Text, txbSmallCustomerMobil.Text, txbSmallCustomerEmail.Text, vat, 
                            payment, txbSmallCustomerNumber.Text);

                        CustomerDropDownList.DataBind();
                        CustomerDropDownList.FindItemByValue(newSmallCustomer.CustomerId.ToString()).Selected = true;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLeereTextBoxenLabel.Text = "Fehler: " + ex.Message;
                    ErrorLeereTextBoxenLabel.Visible = true;
                }
            }
        }

        #endregion
    }
}