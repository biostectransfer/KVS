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
using System.Web.Caching;
using System.Transactions;
using KVSCommon.Enums;
using KVSCommon.Managers;
namespace KVSWebApplication.Auftragseingang
{
    //Neuzulassung Laufkunde
    public partial class ZulassungLaufkundeControl : AdmissionOrderBase
    {
        #region Members
        
        protected override Label CustomerHistoryLabel { get { return this.SmallCustomerHistorie; } }
        protected override RadTreeView ProductTree { get { return DienstleistungTreeView; } }
        protected override RadScriptManager RadScriptManager { get { return ((ZulassungLaufkunde)Page).getScriptManager(); } }
        protected override RadNumericTextBox Discount { get { return this.txbDiscount; } }
        protected override HiddenField SmallCustomerOrder { get { return this.smallCustomerOrderHiddenField; } }

        #region Dates

        protected override RadMonthYearPicker Registration_GeneralInspectionDatePicker { get { return this.Registration_GeneralInspectionDateBox; } }
        protected override RadDatePicker FirstRegistrationDatePicker { get { return this.FirstRegistrationDateBox; } }

        #endregion

        #region DropDowns

        protected override RadComboBox CustomerDropDown { get { return this.CustomerDropDownList; } }
        protected override RadComboBox LocationDropDown { get { return null; } }
        protected override RadComboBox CostCenterDropDown { get { return null; } }
        protected override RadComboBox AdmissionPointDropDown { get { return this.ZulassungsstelleComboBox; } }
        protected override RadComboBox ProductDropDown { get { return this.ProductDropDownList; } }
        protected override RadComboBox RegistrationOrderDropDown { get { return this.RegistrationOrderDropDownList; } }

        #endregion
        #region TextBoxes

        protected override RadTextBox AccountNumberTextBox { get { return this.BankAccount_AccountnumberBox; } }
        protected override RadTextBox BankCodeTextBox { get { return this.BankAccount_BankCodeBox; } }
        protected override RadTextBox BankNameTextBox { get { return this.BankAccount_BankNameBox; } }
        protected override RadTextBox IBANTextBox { get { return this.txbBancAccountIban; } }
        protected override RadTextBox BICTextBox { get { return txbBankAccount_Bic; } }
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

        protected override Panel Panel { get { return this.ZulassungPanel; } }
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

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            RadScriptManager.RegisterPostBackControl(AddAdressButton);
            //first registration get today date by default
            FirstRegistrationDateBox.SelectedDate = DateTime.Now;
            SaveState();
            string target = Request["__EVENTARGUMENT"];

            if (target != null && target == "CreateOrder")
            {
                AuftragZulassenButton_Clicked(sender, e);
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
            if (!String.IsNullOrEmpty(ProductDropDownList.Text) && !String.IsNullOrEmpty(ProductDropDownList.SelectedValue))
            {
                string costCenter = "";
                string value = ProductDropDownList.SelectedValue.ToString() + ";" + costCenter;
                var addedNode = new RadTreeNode(ProductDropDownList.Text, value);
                target.Nodes.Add(addedNode);
            }
        }

        protected void RegistrationTyp_Changed(object sender, EventArgs e)
        {
            KennzeichenTauschButton.Visible = false;
            ProductDropDownList.ClearSelection();
            ProductDropDownList.ClearCheckedItems();
            ProductDropDownList.SelectedValue = "";
            ProductDropDownList.Items.Clear();
            ProductDropDownList.DataBind();
            ProductDropDownList.Focus();
            DienstleistungTreeView.Nodes.Clear();
        }

        protected void CustomerIndex_Changed(object sender, EventArgs e)
        {
            ProductDropDownList.Text = null;
            ProductDropDownList.ClearSelection();
            RegistrationOrderDropDownList.Text = null;
            RegistrationOrderDropDownList.ClearSelection();
            ProductDropDownList.DataBind();
            DienstleistungTreeView.Nodes.Clear();
            ZulassungOkLabel.Visible = false;
            GetCustomerInfo();
            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue.ToString()))
            {
                ShowControls();
                CheckUmsatzForSmallCustomer();
            }
        }

        //VIN ist eingegeben, versuch das Fahrzeug zu finden
        protected void VinBoxZulText_Changed(object sender, EventArgs e)
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
                    FahrzeugLabel.Text = "FIN kann nur entweder 17 oder 8-stellig sein!";
                    FahrzeugLabel.ForeColor = System.Drawing.Color.Red;
                    VINBox.Focus();
                }
            }

            if (finIsOkey == true)
            {
                VINBox.Text = VINBox.Text.ToUpper();

                var vehicle = VehicleManager.GetEntities(q => q.VIN == VINBox.Text).FirstOrDefault();
                if (vehicle != null)
                {
                    //wird als cache field für die Kennzeichnung bei der Umkennzeichnung benutzt
                    if (vehicle.CurrentRegistrationId.HasValue)
                    {
                        var registration = RegistrationManager.GetById(vehicle.CurrentRegistrationId.Value);

                        LicenceNumberCacheField.Value = registration.Licencenumber;
                        RegistrationIdField.Value = registration.Id.ToString();
                        var kennzeichen = registration.Licencenumber;
                        string[] newKennzeichen = kennzeichen.Split('-');

                        if (newKennzeichen.Length == 3)
                        {
                            LicenceBox1.Text = newKennzeichen[0];
                            LicenceBox2.Text = newKennzeichen[1];
                            LicenceBox3.Text = newKennzeichen[2];
                        }

                        Registration_GeneralInspectionDateBox.SelectedDate = registration.GeneralInspectionDate;
                        RegDocNumBox.Text = registration.RegistrationDocumentNumber;
                        EmissionsCodeBox.Text = registration.EmissionCode;


                        CarOwner owner = registration.CarOwner;
                        if (owner != null)
                        {
                            CarOwner_NameBox.Text = owner.Name;
                            if (owner.Adress != null)
                            {
                                Adress_StreetBox.Text = owner.Adress.Street;
                                CarOwner_FirstnameBox.Text = owner.FirstName;
                                Adress_StreetNumberBox.Text = owner.Adress.StreetNumber;
                                Adress_ZipcodeBox.Text = owner.Adress.Zipcode;
                                Adress_CityBox.Text = owner.Adress.City;
                                Adress_CountryBox.Text = owner.Adress.Country;
                            }
                            if (owner.Contact != null)
                            {
                                Contact_PhoneBox.Text = owner.Contact.Phone;
                                Contact_FaxBox.Text = owner.Contact.Fax;
                                Contact_MobilePhoneBox.Text = owner.Contact.MobilePhone;
                                Contact_EmailBox.Text = owner.Contact.Email;
                            }
                            if (owner.BankAccount != null)
                            {
                                BankAccount_BankNameBox.Text = owner.BankAccount.BankName;
                                BankAccount_AccountnumberBox.Text = owner.BankAccount.Accountnumber;
                                BankAccount_BankCodeBox.Text = owner.BankAccount.BankCode;
                            }
                            PruefzifferBox.Focus();
                        }
                    }

                    VehicleIdField.Value = vehicle.Id.ToString();
                    Vehicle_VariantBox.Text = vehicle.Variant;
                    HSNBox.Text = vehicle.HSN;
                    TSNBox.Text = vehicle.TSN;
                    Vehicle_ColorBox.Text = vehicle.ColorCode.ToString();
                }
            }
        }

        #endregion

        #region Linq Data Source

        protected void ProductDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            var selectedCustomer = 0;

            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue))
                selectedCustomer = Int32.Parse(CustomerDropDownList.SelectedValue);
            IEnumerable<object> products = null;

            if (!String.IsNullOrEmpty(RegistrationOrderDropDownList.SelectedValue))
            {
                products = PriceManager.GetEntities(o => o.PriceAccount.Count != 0 && o.Location == null &&
                    (o.Product.RegistrationOrderTypeId == Int32.Parse(RegistrationOrderDropDownList.SelectedValue) ||
                        o.Product.OrderType.Id == (int)OrderTypes.Common)).
                    Select(o => new
                    {
                        ItemNumber = o.Product.ItemNumber,
                        Name = o.Product.Name,
                        Value = o.Product.Id,
                        Category = o.Product.ProductCategory.Name,
                        Price = Math.Round(o.Amount, 2, MidpointRounding.AwayFromZero).ToString(),
                        AuthCharge = o.AuthorativeCharge.HasValue ? Math.Round(o.AuthorativeCharge.Value, 2, MidpointRounding.AwayFromZero).ToString().Trim() : ""
                    }).OrderBy(o => o.Name).ToList();
            }
            else
            {
                products = PriceManager.GetEntities(o => o.PriceAccount.Count != 0 && o.Location != null && !o.Location.CustomerId.HasValue).
                    Select(o => new
                    {
                        ItemNumber = o.Product.ItemNumber,
                        Name = o.Product.Name,
                        Value = o.Product.Id,
                        Category = o.Product.ProductCategory.Name,
                        Price = Math.Round(o.Amount, 2, MidpointRounding.AwayFromZero).ToString(),
                        AuthCharge = o.AuthorativeCharge.HasValue ? Math.Round(o.AuthorativeCharge.Value, 2, MidpointRounding.AwayFromZero).ToString().Trim() : ""
                    }).OrderBy(o => o.Name).ToList();
            }

            e.Result = products;
        }

        // Auswahl der KundenName
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetAllSmallCustomers();
        }

        protected void RegistrationOrderDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities con = new KVSEntities();
            var regOrdQuery = from regord in con.RegistrationOrderType
                              select new
                              {
                                  Name = regord.Name,
                                  Value = regord.Id
                              };
            e.Result = regOrdQuery;
        }

        #endregion
        
        #region Button Clicked
        //Neue Auftragseingang
        protected void AuftragZulassenButton_Clicked(object sender, EventArgs e)
        {
            string ProduktId = "";

            LoadState();
            ZulassungOkLabel.Visible = false;
            SubmitChangesErrorLabel.Visible = false;
            ErrorLeereTextBoxenLabel.Visible = false;
            if (CheckIfBoxenNotEmpty()) //gibt es leer boxen, die angezeigt sind.
            {
                if (DienstleistungTreeView.Nodes.Count == 0)
                {
                    ErrorLeereTextBoxenLabel.Text = "Bitte Dienstleistung hinzufügen!";
                    ErrorLeereTextBoxenLabel.Visible = true;
                }
                else
                {
                    ErrorLeereTextBoxenLabel.Text = "Bitte FIN eingeben!";
                    ErrorLeereTextBoxenLabel.Visible = true;
                }
            }
            else if (String.IsNullOrEmpty(ZulassungsstelleComboBox.SelectedValue))
            {
                ErrorLeereTextBoxenLabel.Text = "Bitte wählen Sie die Zulassungstelle aus";
                ErrorLeereTextBoxenLabel.Visible = true;
                return;
            }
            else if (String.IsNullOrEmpty(RegistrationOrderDropDownList.SelectedValue))
            {
                ErrorLeereTextBoxenLabel.Text = "Bitte wählen Sie die Zulassungsart aus";
                ErrorLeereTextBoxenLabel.Visible = true;
                return;
            }
            else if (DienstleistungTreeView.Nodes.Count > 0)
            {
                AddCustomer();
                RadTreeNode node = DienstleistungTreeView.Nodes[0];
                string[] splited = node.Value.Split(';');
                ProduktId = splited[0];
                ErrorLeereTextBoxenLabel.Visible = false;
                try
                {
                    ZulassungOkLabel.Visible = false;
                    SubmitChangesErrorLabel.Visible = false;
                    string kennzeichen = string.Empty,
                       oldKennzeichen = string.Empty;
                    KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                    Adress newAdress = null;
                    Contact newContact = null;
                    BankAccount newBankAccount = null;
                    CarOwner newCarOwner = null;
                    Registration newRegistration = null;
                    Price price = null;
                    RegistrationOrder newKennzeichenRegOrder = null;
                    Vehicle newVehicle = null;
                    DateTime? FirstRegistrationDate = null;

                    int? color = null;

                    if (!String.IsNullOrEmpty(LicenceBox1.Text))
                        kennzeichen = LicenceBox1.Text + "-" + LicenceBox2.Text + "-" + LicenceBox3.Text;

                    if (!String.IsNullOrEmpty(PreviousLicenceBox1.Text))
                        oldKennzeichen = PreviousLicenceBox1.Text + "-" + PreviousLicenceBox2.Text + "-" + PreviousLicenceBox3.Text;

                    if (!String.IsNullOrEmpty(FirstRegistrationDateBox.SelectedDate.ToString()))
                        FirstRegistrationDate = FirstRegistrationDateBox.SelectedDate;

                    if (!String.IsNullOrEmpty(Vehicle_ColorBox.Text))
                        color = Convert.ToInt32(Vehicle_ColorBox.Text);

                    if (RegistrationOrderDropDownList.Text.Contains("Umkennzeichnung")) // Umkennzeichnung
                    {
                        FahrzeugLabel.Text = "Fahrzeug";
                        FahrzeugLabel.ForeColor = System.Drawing.Color.FromArgb(234, 239, 244);
                        if (!String.IsNullOrEmpty(kennzeichen))
                        {
                            if (!String.IsNullOrEmpty(VehicleIdField.Value)) //falls auto gefunden wurde
                            {
                                newVehicle = dbContext.Vehicle.SingleOrDefault(q => q.Id == Int32.Parse(VehicleIdField.Value));
                            }
                            else // neues Auto muss angelegt werden
                            {
                                newVehicle = Vehicle.CreateVehicle(VINBox.Text, HSNBox.Text, TSNBox.Text, Vehicle_VariantBox.Text, FirstRegistrationDate, color, dbContext);
                            }
                            // another logic after new/existing Vehicle
                            newAdress = Adress.CreateAdress(Adress_StreetBox.Text, Adress_StreetNumberBox.Text, Adress_ZipcodeBox.Text, Adress_CityBox.Text, Adress_CountryBox.Text, dbContext);
                            newContact = Contact.CreateContact(Contact_PhoneBox.Text, Contact_FaxBox.Text, Contact_MobilePhoneBox.Text, Contact_EmailBox.Text, dbContext);
                            newBankAccount = BankAccount.CreateBankAccount(dbContext, BankAccount_BankNameBox.Text, BankAccount_AccountnumberBox.Text, BankAccount_BankCodeBox.Text,
                                txbBancAccountIban.Text, txbBankAccount_Bic.Text);
                            newCarOwner = CarOwner.CreateCarOwner(CarOwner_NameBox.Text, CarOwner_FirstnameBox.Text, newBankAccount, newContact, newAdress, dbContext);
                            DateTime newZulassungsDatum = DateTime.Now;
                            if (ZulassungsdatumPicker.SelectedDate != null)
                            {
                                if (!string.IsNullOrEmpty(ZulassungsdatumPicker.SelectedDate.ToString()))
                                {
                                    newZulassungsDatum = (DateTime)ZulassungsdatumPicker.SelectedDate;
                                }
                            }
                            newRegistration = Registration.CreateRegistration(newCarOwner, newVehicle, kennzeichen, Registration_eVBNumberBox.Text,
                                Registration_GeneralInspectionDateBox.SelectedDate, newZulassungsDatum, RegDocNumBox.Text, EmissionsCodeBox.Text, dbContext);
                            newKennzeichenRegOrder = RegistrationOrder.CreateRegistrationOrder(Int32.Parse(Session["CurrentUserId"].ToString()),
                                Int32.Parse(CustomerDropDownList.SelectedValue), kennzeichen, oldKennzeichen, Registration_eVBNumberBox.Text, newVehicle, newRegistration,
                                RegistrationOrderTypes.Renumbering, null, Int32.Parse(ZulassungsstelleComboBox.SelectedValue), dbContext);
                            newKennzeichenRegOrder.Order.FreeText = FreiTextBox.Text;
                            dbContext.SubmitChanges();
                            AddAnotherProducts(newKennzeichenRegOrder, null);
                            if (String.IsNullOrEmpty(VehicleIdField.Value)) //update CurrentRegistration Id
                            {
                                newVehicle.CurrentRegistrationId = newRegistration.Id;
                                dbContext.SubmitChanges();
                            }
                            if (invoiceNow.Checked == true && invoiceNow.Enabled == true)
                            {
                                MakeInvoiceForSmallCustomer(Int32.Parse(CustomerDropDownList.SelectedValue), newKennzeichenRegOrder);
                            }
                            else
                            {
                                MakeAllControlsEmpty();
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "finishedMessage", "alert('Auftrag wurde erfolgreich angelegt.');", true);
                            }
                        }
                        else
                        {
                            FahrzeugLabel.Text = "Für die Umkennzeichnung mind. neues Kennzeichen erforderlich!";
                            FahrzeugLabel.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                    else if (RegistrationOrderDropDownList.Text.Contains("Wiederzulassung")) // Wiederzulassung
                    {
                        if (!String.IsNullOrEmpty(VehicleIdField.Value)) //falls auto gefunden wurde
                        {
                            newVehicle = dbContext.Vehicle.SingleOrDefault(q => q.Id == Int32.Parse(VehicleIdField.Value));
                        }
                        else // neues Auto muss angelegt werden
                        {
                            newVehicle = Vehicle.CreateVehicle(VINBox.Text, HSNBox.Text, TSNBox.Text, Vehicle_VariantBox.Text, FirstRegistrationDate, color, dbContext);
                        }
                        // another logic after new/existing Vehicle
                        newAdress = Adress.CreateAdress(Adress_StreetBox.Text, Adress_StreetNumberBox.Text, Adress_ZipcodeBox.Text, Adress_CityBox.Text, Adress_CountryBox.Text, dbContext);
                        newContact = Contact.CreateContact(Contact_PhoneBox.Text, Contact_FaxBox.Text, Contact_MobilePhoneBox.Text, Contact_EmailBox.Text, dbContext);
                        newBankAccount = BankAccount.CreateBankAccount(dbContext, BankAccount_BankNameBox.Text, BankAccount_AccountnumberBox.Text, BankAccount_BankCodeBox.Text,
                            txbBancAccountIban.Text, txbBankAccount_Bic.Text);
                        newCarOwner = CarOwner.CreateCarOwner(CarOwner_NameBox.Text, CarOwner_FirstnameBox.Text, newBankAccount, newContact, newAdress, dbContext);
                        DateTime newZulassungsDatum = DateTime.Now;
                        if (ZulassungsdatumPicker.SelectedDate != null)
                        {
                            if (!string.IsNullOrEmpty(ZulassungsdatumPicker.SelectedDate.ToString()))
                            {
                                newZulassungsDatum = (DateTime)ZulassungsdatumPicker.SelectedDate;
                            }
                        }
                        newRegistration = Registration.CreateRegistration(newCarOwner, newVehicle, kennzeichen, Registration_eVBNumberBox.Text,
                            Registration_GeneralInspectionDateBox.SelectedDate, newZulassungsDatum, RegDocNumBox.Text, EmissionsCodeBox.Text, dbContext);
                        price = FindPrice(ProduktId);
                        if (price == null)
                        {
                            ErrorLeereTextBoxenLabel.Text = "Kein Preis gefunden!";
                            ErrorLeereTextBoxenLabel.Visible = true;
                            return;
                        }
                        newKennzeichenRegOrder = RegistrationOrder.CreateRegistrationOrder(Int32.Parse(Session["CurrentUserId"].ToString()),
                            Int32.Parse(CustomerDropDownList.SelectedValue), kennzeichen, oldKennzeichen, Registration_eVBNumberBox.Text, newVehicle, newRegistration,
                            RegistrationOrderTypes.Readmission, null, Int32.Parse(ZulassungsstelleComboBox.SelectedValue), dbContext);
                        if (!String.IsNullOrEmpty(FreiTextBox.Text))
                        {
                            newKennzeichenRegOrder.Order.FreeText = FreiTextBox.Text;
                        }
                        dbContext.SubmitChanges();
                        AddAnotherProducts(newKennzeichenRegOrder, null);
                        if (String.IsNullOrEmpty(VehicleIdField.Value)) //update CurrentRegistration Id
                        {
                            newVehicle.CurrentRegistrationId = newRegistration.Id;
                            dbContext.SubmitChanges();
                        }
                        if (invoiceNow.Checked == true && invoiceNow.Enabled == true)
                        {
                            MakeInvoiceForSmallCustomer(Int32.Parse(CustomerDropDownList.SelectedValue), newKennzeichenRegOrder);
                        }
                        else
                        {
                            MakeAllControlsEmpty();
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "finishedMessage", "alert('Auftrag wurde erfolgreich angelegt.');", true);
                        }
                    }
                    else // Neuzulassung
                    {
                        if (!String.IsNullOrEmpty(VehicleIdField.Value)) //falls auto gefunden wurde
                        {
                            newVehicle = dbContext.Vehicle.SingleOrDefault(q => q.Id == Int32.Parse(VehicleIdField.Value));
                        }
                        else // neues Auto muss angelegt werden
                        {
                            newVehicle = Vehicle.CreateVehicle(VINBox.Text, HSNBox.Text, TSNBox.Text, Vehicle_VariantBox.Text, FirstRegistrationDate, color, dbContext);
                        }
                        newAdress = Adress.CreateAdress(Adress_StreetBox.Text, Adress_StreetNumberBox.Text, Adress_ZipcodeBox.Text, Adress_CityBox.Text, Adress_CountryBox.Text, dbContext);
                        newContact = Contact.CreateContact(Contact_PhoneBox.Text, Contact_FaxBox.Text, Contact_MobilePhoneBox.Text, Contact_EmailBox.Text, dbContext);
                        newBankAccount = BankAccount.CreateBankAccount(dbContext, BankAccount_BankNameBox.Text, BankAccount_AccountnumberBox.Text, BankAccount_BankCodeBox.Text,
                            txbBancAccountIban.Text, txbBankAccount_Bic.Text);
                        newCarOwner = CarOwner.CreateCarOwner(CarOwner_NameBox.Text, CarOwner_FirstnameBox.Text, newBankAccount, newContact, newAdress, dbContext);
                        DateTime newZulassungsDatum = DateTime.Now;
                        if (ZulassungsdatumPicker.SelectedDate != null)
                        {
                            if (!string.IsNullOrEmpty(ZulassungsdatumPicker.SelectedDate.ToString()))
                            {
                                newZulassungsDatum = (DateTime)ZulassungsdatumPicker.SelectedDate;
                            }
                        }
                        newRegistration = Registration.CreateRegistration(newCarOwner, newVehicle, kennzeichen, Registration_eVBNumberBox.Text,
                            Registration_GeneralInspectionDateBox.SelectedDate, newZulassungsDatum, RegDocNumBox.Text, EmissionsCodeBox.Text, dbContext);
                        price = FindPrice(ProduktId);
                        if (price == null)
                        {
                            ErrorLeereTextBoxenLabel.Text = "Kein Preis gefunden!";
                            ErrorLeereTextBoxenLabel.Visible = true;
                            return;
                        }
                        newKennzeichenRegOrder = RegistrationOrder.CreateRegistrationOrder(Int32.Parse(Session["CurrentUserId"].ToString()),
                            Int32.Parse(CustomerDropDownList.SelectedValue), kennzeichen, oldKennzeichen, Registration_eVBNumberBox.Text, newVehicle, newRegistration,
                            RegistrationOrderTypes.NewAdmission, null, Int32.Parse(ZulassungsstelleComboBox.SelectedValue), dbContext);
                        if (!String.IsNullOrEmpty(FreiTextBox.Text))
                        {
                            newKennzeichenRegOrder.Order.FreeText = FreiTextBox.Text;
                        }
                        dbContext.SubmitChanges();

                        AddAnotherProducts(newKennzeichenRegOrder, null);
                        newVehicle.CurrentRegistrationId = newRegistration.Id;
                        dbContext.SubmitChanges();
                        if (invoiceNow.Checked == true && invoiceNow.Enabled == true)
                        {
                            MakeInvoiceForSmallCustomer(Int32.Parse(CustomerDropDownList.SelectedValue), newKennzeichenRegOrder);
                        }
                        else
                        {
                            MakeAllControlsEmpty();
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "finishedMessage", "alert('Auftrag wurde erfolgreich angelegt.');", true);
                        }
                    }
                    VehicleIdField.Value = "";
                }
                catch (Exception ex)
                {
                    DienstleistungTreeView.Nodes.Clear();
                    SubmitChangesErrorLabel.Visible = true;
                    SubmitChangesErrorLabel.Text = "Fehler" + ex.Message;
                }
            }
        }

        #endregion

        #region Methods

        protected void AddAnotherProducts(RegistrationOrder regOrd, int? locationId)
        {
            string produktId = "";
            int itemIndexValue = 0;
            decimal amount = 0;

            var nodes = this.Request.Form.AllKeys.Where(q => q.Contains("txtItemPrice_"));
            foreach (var node in nodes)
            {
                if (!String.IsNullOrEmpty(node))
                {
                    Price newPrice = null;
                    OrderItem newOrderItem = null;
                    produktId = node.Split('_')[1];

                    if (!String.IsNullOrEmpty(produktId))
                    {
                        var newProduct = ProductManager.GetById(Int32.Parse(produktId));
                        if (locationId == null) //small
                        {
                            newPrice = PriceManager.GetEntities(q => q.ProductId == newProduct.Id && q.LocationId == null).SingleOrDefault();
                        }

                        var orderToUpdate = OrderManager.GetEntities(q => q.OrderNumber == regOrd.OrderNumber).SingleOrDefault();

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
                            newOrderItem = OrderManager.AddOrderItem(orderToUpdate, newProduct.Id, amount, 1, null, null, false);
                            if (newPrice != null && newPrice.AuthorativeCharge.HasValue)
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

        protected void MakeInvoiceForSmallCustomer(int customerId, RegistrationOrder regOrder)
        {
            try
            {
                var newOrder = OrderManager.GetEntities(q => q.CustomerId == customerId && q.OrderNumber == regOrder.OrderNumber).Single();
                smallCustomerOrderHiddenField.Value = regOrder.OrderNumber.ToString();
                newOrder.Status = (int)OrderStatusTypes.Closed;
                //updating orderitems status                          
                foreach (OrderItem ordItem in newOrder.OrderItem)
                {
                    if (ordItem.Status != (int)OrderItemStatusTypes.Cancelled)
                    {
                        ordItem.Status = (int)OrderItemStatusTypes.Closed;
                    }
                }

                newOrder.ExecutionDate = DateTime.Now;
                newOrder.Status = (int)OrderStatusTypes.Payed;
                OrderManager.SaveChanges();
                
                //opening window for adress
                string script = "function f(){$find(\"" + AddAdressRadWindow.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
                SetValuesForAdressWindow();
            }
            catch (Exception ex)
            {
                ErrorLeereTextBoxenLabel.Text = "Error: " + ex.Message;
                ErrorLeereTextBoxenLabel.Visible = true;
            }
        }

        // getting adress from small customer
        protected void SetValuesForAdressWindow()
        {
            var location = AdressManager.GetEntities(o => o.Invoice.Count != 0 &&
                o.Customer.Any(q => q.Id == Int32.Parse(CustomerDropDownList.SelectedValue))).FirstOrDefault();

            if (location != null)
            {
                StreetTextBox.Text = location.Street;
                StreetNumberTextBox.Text = location.StreetNumber;
                ZipcodeTextBox.Text = location.Zipcode;
                CityTextBox.Text = location.City;
                CountryTextBox.Text = location.Country;
                LocationLabelWindow.Text = "Fügen Sie bitte die Adresse für " + CustomerDropDownList.Text + " hinzu";
                ZusatzlicheInfoLabel.Visible = false;
                if (CustomerDropDownList.SelectedIndex == 1) // small
                {
                    ZusatzlicheInfoLabel.Visible = true;
                }
            }
        }

        //Tausch die Information aus neues zu altes Kennzeichen Boxen
        protected void KennzeichenTauschButton_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(PreviousLicenceBox1.Text))
            {
                PreviousLicenceBox1.Text = LicenceBox1.Text;
                PreviousLicenceBox2.Text = LicenceBox2.Text;
                PreviousLicenceBox3.Text = LicenceBox3.Text;
                LicenceBox1.Text = "";
                LicenceBox2.Text = "";
                LicenceBox3.Text = "";
            }
            else if (String.IsNullOrEmpty(LicenceBox1.Text))
            {
                LicenceBox1.Text = PreviousLicenceBox1.Text;
                LicenceBox2.Text = PreviousLicenceBox2.Text;
                LicenceBox3.Text = PreviousLicenceBox3.Text;
                PreviousLicenceBox1.Text = "";
                PreviousLicenceBox2.Text = "";
                PreviousLicenceBox3.Text = "";
            }
        }

        // findet alle angezeigte textboxen und überprüft ob die nicht leer sind
        protected bool CheckIfBoxenNotEmpty()
        {
            bool gibtsBoxenDieLeerSind = false;
            bool iFound1VisibleBox = false;
            List<Control> allControls = GetAllControls();
            //fallse leer - soll aus der Logik rausgenommen
            if (String.IsNullOrEmpty(PruefzifferBox.Text))
                PruefzifferBox.Enabled = false;
            if (String.IsNullOrEmpty(RegistrationOrderDropDownList.SelectedValue) || DienstleistungTreeView.Nodes.Count == 0)
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
                            if (box.Enabled == true && String.IsNullOrEmpty(box.Text) && (box.ID == "VINBox"))
                            {
                                box.BorderColor = System.Drawing.Color.Red;
                                gibtsBoxenDieLeerSind = true;
                            }
                            else
                            {
                            }
                        }
                    }
                }
            }
            if (iFound1VisibleBox == false)
                gibtsBoxenDieLeerSind = true;
            return gibtsBoxenDieLeerSind;
        }

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

        protected void NaechtenAuftragButton_Clicked(object sender, EventArgs e)
        {
            MakeAllControlsEmpty();
            ZulassungOkLabel.Visible = false;
            SubmitChangesErrorLabel.Visible = false;
        }
        
        protected void btnClearSelection_Click(object sender, EventArgs e)
        {
            CustomerDropDownList.ClearSelection();
            MakeAllControlsEmpty();

            var maxCustomerNumber = CustomerManager.GetEntities().Max(o => o.CustomerNumber);
            txbSmallCustomerNumber.Text = EmptyStringIfNull.generateIndividualNumber(maxCustomerNumber);

            setCustomerTXBEnable(true);
        }
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
                    // Kundendaten
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
                    var checkThisCustomer = CustomerManager.GetEntities(q => q.CustomerNumber == txbSmallCustomerNumber.Text).FirstOrDefault();
                    var maxCustomerNumber = CustomerManager.GetEntities().Max(q => q.CustomerNumber);
                    while (checkThisCustomer != null)
                    {
                        txbSmallCustomerNumber.Text = EmptyStringIfNull.generateIndividualNumber(maxCustomerNumber);
                        checkThisCustomer = CustomerManager.GetEntities(q => q.CustomerNumber == txbSmallCustomerNumber.Text).FirstOrDefault();
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
                            txbSmallCustomerCountry.Text, txbSmallCustomerPhone.Text,
                            txbSmallCustomerFax.Text, txbSmallCustomerMobil.Text, txbSmallCustomerEmail.Text, vat, payment, txbSmallCustomerNumber.Text);
                        
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

        #region Private Methods

        private void AddCacheDependency(HttpContext context, string tempFileName, TimeSpan timeToLive, string fullPath)
        {
            if (context.Cache.Get(tempFileName) == null)
                context.Cache.Insert(tempFileName, fullPath, null, DateTime.Now.Add(timeToLive), TimeSpan.Zero,
                    CacheItemPriority.NotRemovable, RemovedCallback);
        }

        private void RemovedCallback(String key, Object value, CacheItemRemovedReason reason)
        {
            var path = (string)value;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private void SaveState()
        {
            SessionID.Value = Guid.NewGuid().ToString();
            RadPersistenceManager1.StorageProviderKey = SessionID.Value;
            RadPersistenceManager1.SaveState();
            // The following method is called for the purpose of the demo
            AddCacheDependency(System.Web.HttpContext.Current, SessionID.Value, new TimeSpan(2, 0, 0), Server.MapPath("~/App_Data") + "\\" + SessionID.Value);
        }

        private void LoadState()
        {
            if (File.Exists(Server.MapPath("~/App_Data") + "\\" + SessionID.Value))
            {
                RadPersistenceManager1.StorageProviderKey = SessionID.Value;
                RadPersistenceManager1.LoadState();
            }
        }

        #endregion
    }
}