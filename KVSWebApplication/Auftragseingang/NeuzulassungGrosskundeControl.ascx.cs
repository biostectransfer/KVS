using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using System.IO;
using System.Configuration;
using Telerik.Web.UI;
using KVSCommon.Enums;
using KVSCommon.Managers;
using System.Drawing;

namespace KVSWebApplication.Auftragseingang
{
    /// <summary>
    /// Neuzulassung Grosskunde
    /// </summary>
    public partial class NeuzulassungGrosskundeControl : AdmissionOrderBase
    {
        #region Members
       
        protected override Label CustomerHistoryLabel { get { return this.SmallCustomerHistorie; } }
        protected override RadTreeView ProductTree { get { return DienstleistungTreeView; } }
        protected override RadScriptManager RadScriptManager { get { return ((NeuzulassungGrosskunde)Page).getScriptManager(); } }
        protected override RadNumericTextBox Discount { get { return this.txbDiscount; } }
        protected override HiddenField SmallCustomerOrder { get { return this.smallCustomerOrderHiddenField; } }
        protected override HiddenField VehicleId { get { return this.VehicleIdField; } }
        protected override RadWindow RadWindow { get { return this.AddAdressRadWindow; } }

        #region Dates

        protected override RadMonthYearPicker Registration_GeneralInspectionDatePicker { get { return this.Registration_GeneralInspectionDateBox; } }
        protected override RadDatePicker FirstRegistrationDatePicker { get { return this.FirstRegistrationDateBox; } }
        protected override RadDatePicker RegistrationDatePicker { get { return this.ZulassungsdatumPicker; } }

        #endregion

        #region DropDowns

        protected override RadComboBox CustomerDropDown { get { return this.CustomerDropDownList; } }
        protected override RadComboBox LocationDropDown { get { return this.LocationDropDownList; } }
        protected override RadComboBox CostCenterDropDown { get { return this.CostCenterDropDownList; } }
        protected override RadComboBox AdmissionPointDropDown { get { return this.ZulassungsstelleComboBox; } }
        protected override RadComboBox ProductDropDown { get { return this.ProductDropDownList; } }
        protected override RadComboBox RegistrationOrderDropDown { get { return this.RegistrationOrderDropDownList; } }
        #endregion

        #region TextBoxes

        protected override RadTextBox AccountNumberTextBox { get { return this.BankAccount_AccountnumberBox; } }
        protected override RadTextBox BankCodeTextBox { get { return this.BankAccount_BankCodeBox; } }
        protected override RadTextBox BankNameTextBox { get { return this.BankAccount_BankNameBox; } }
        protected override RadTextBox IBANTextBox { get { return this.txbBancAccountIban; } }
        protected override RadTextBox BICTextBox { get { return txbBancAccountBIC; } }
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
        protected override RadTextBox HSN_TextBox { get { return this.HSNBox; } }
        protected override RadTextBox TSN_TextBox { get { return this.TSNBox; } }
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
        protected override Label AdditionalInfoCaption { get { return this.ZusatzlicheInfoLabel; } }
        protected override Label LocationWindowCaption { get { return this.LocationLabelWindow; } }
        #endregion

        #endregion

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            RadScriptManager.RegisterPostBackControl(AddAdressButton);
            //first registration get today date by default
            FirstRegistrationDateBox.SelectedDate = DateTime.Now;
            if (Session["CurrentUserId"] != null && !String.IsNullOrEmpty(Session["CurrentUserId"].ToString()))
            {
                CheckUserPermissions();
            }
        }

        protected void SmallLargeCustomer_Changed(object sender, EventArgs e)
        {
            CustomerDropDownList.DataBind();
            LocationDropDownList.Enabled = true;
            CostCenterDropDownList.Enabled = true;
            CustomerDropDownList.Focus();
            ProductDropDownList.DataSource = null;
            ProductDropDownList.DataBind();
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
                if (!String.IsNullOrEmpty(CostCenterDropDownList.SelectedValue.ToString()))
                {
                    costCenter = (CostCenterDropDownList.SelectedValue.ToString());
                }
                else
                    costCenter = "";
                string value = ProductDropDownList.SelectedValue.ToString() + ";" + costCenter;
                var addedNode = new RadTreeNode(ProductDropDownList.Text + "(" + CostCenterDropDownList.Text + ")", value);
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
            SetCarOwnerData();
        }

        protected void CustomerIndex_Changed(object sender, EventArgs e)
        {
            ProductDropDownList.Text = null;
            ProductDropDownList.ClearSelection();
            LocationDropDownList.Text = null;
            LocationDropDownList.ClearSelection();
            RegistrationOrderDropDownList.Text = null;
            RegistrationOrderDropDownList.ClearSelection();
            CostCenterDropDownList.Text = null;
            CostCenterDropDownList.ClearSelection();
            LocationDropDownList.DataBind();
            ProductDropDownList.DataBind();
            CostCenterDropDownList.DataBind();
            LocationDropDownList.Focus();
            ZulassungOkLabel.Visible = false;
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

        //VIN ist eingegeben, versuch das Fahrzeug zu finden
        protected void VinBoxZulText_Changed(object sender, EventArgs e)
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

                        var owner = registration.CarOwner;
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

        // findet alle textboxen und macht die leer ohne die ganze Seite neu zu laden
        protected void NaechtenAuftragButton_Clicked(object sender, EventArgs e)
        {
            MakeAllControlsEmpty();
            ZulassungOkLabel.Visible = false;
            SubmitChangesErrorLabel.Visible = false;
        }

        #endregion

        #region Linq Data Source

        protected void ProductDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            var selectedCustomer = 0;
            var location = 0;
            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue))
                selectedCustomer = Int32.Parse(CustomerDropDownList.SelectedValue);
            if (!String.IsNullOrEmpty(LocationDropDownList.SelectedValue))
                location = Int32.Parse(LocationDropDownList.SelectedValue);
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
                    }).OrderBy(o => o.Name).ToList();               
            }
            else
            {
                products = PriceManager.GetEntities(o => o.PriceAccount.Count != 0 && o.Location != null &&  !o.Location.CustomerId.HasValue).
                       Select(o => new
                       {
                           ItemNumber = o.Product.ItemNumber,
                           Name = o.Product.Name,
                           Value = o.Product.Id,
                           Category = o.Product.ProductCategory.Name,
                       }).OrderBy(o => o.Name).ToList();
            }

            //werden die vordefinierte Kundenprodukte in TreeNode geladen
            if (products.Count() != 0 && location != 0 && selectedCustomer != 0 && !String.IsNullOrEmpty(RegistrationOrderDropDownList.SelectedValue))
            {
                LoadCustomerProductsInTreeView(selectedCustomer, location);
            }

            e.Result = products;
        }

        private void LoadCustomerProductsInTreeView(int selectedCustomer, int location)
        {
            var products = PriceManager.GetEntities(o => o.PriceAccount.Count != 0 &&
                o.Product.CustomerProduct.Any() &&
                o.Location != null && o.LocationId == location && o.Location.CustomerId == selectedCustomer &&
                (o.Product.RegistrationOrderTypeId == Int32.Parse(RegistrationOrderDropDownList.SelectedValue) ||
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
                else
                    costCenter = "";

                string value = product.Value + ";" + costCenter;

                RadTreeNode addedNode = new RadTreeNode(product.Name, value);
                target.Nodes.Add(addedNode);
            }
        }

        // Auswahl der KundenName
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetAllLargeCustomers();
        }

        protected void CostCenterLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetCostCenters();
        }

        protected void RegistrationOrderDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = RegistrationOrderTypeManager.GetEntities().Select(o => new
            {
                Name = o.Name,
                Value = o.Id
            }).ToList();
        }

        #endregion

        #region Button Clicked
        //Neue Auftragseingang
        protected void AuftragZulassenButton_Clicked(object sender, EventArgs e)
        {
            ZulassungOkLabel.Visible = false;
            SubmitChangesErrorLabel.Visible = false;
            ErrorLeereTextBoxenLabel.Visible = false;

            if (CheckRegistrationFields()) //exists empty required fields
            {
                int? locationId = null;
                if (!String.IsNullOrEmpty(LocationDropDownList.SelectedValue))
                    locationId = Int32.Parse(LocationDropDownList.SelectedValue);

                var productsPriceCheck = CheckIfAllProduktsHavingPrice(locationId);
                if (!String.IsNullOrEmpty(productsPriceCheck))
                {
                    ErrorLeereTextBoxenLabel.Text = String.Format("Für {0} wurde kein Price gefunden!", productsPriceCheck);
                    ErrorLeereTextBoxenLabel.Visible = true;
                    return;
                }


                if (DienstleistungTreeView.Nodes.Count > 0)
                {
                    RadTreeNode node = DienstleistungTreeView.Nodes[0];
                    string[] splited = node.Value.Split(';');

                    var costCenterId = splited[1];
                    if (costCenterId == string.Empty)
                        costCenterId = CostCenterDropDownList.SelectedValue;

                    var productId = Int32.Parse(splited[0]);

                    var price = FindPrice(productId);
                    if (price == null)
                    {
                        ErrorLeereTextBoxenLabel.Text = "Kein Price gefunden!";
                        ErrorLeereTextBoxenLabel.Visible = true;
                        return;
                    }


                    ErrorLeereTextBoxenLabel.Visible = false;
                    try
                    {
                        ZulassungOkLabel.Visible = false;
                        SubmitChangesErrorLabel.Visible = false;
                        string licenceNumber = String.Empty,
                        oldLicenceNumber = String.Empty;
                        
                        RegistrationOrder newRegistrationOrder = null;
   
                        if (!String.IsNullOrEmpty(LicenceBox1.Text))
                            licenceNumber = LicenceBox1.Text + "-" + LicenceBox2.Text + "-" + LicenceBox3.Text;
                        if (!String.IsNullOrEmpty(PreviousLicenceBox1.Text))
                            oldLicenceNumber = PreviousLicenceBox1.Text + "-" + PreviousLicenceBox2.Text + "-" + PreviousLicenceBox3.Text;
                        
                        CostCenter costCenter = null;
                        if (!String.IsNullOrEmpty(costCenterId))
                        {
                            costCenter = CostCenterManager.GetById(Int32.Parse(costCenterId));
                        }

                        var newVehicle = GetVehicle();
                        var newCarOwner = GetCarOwner();
                        var newRegistration = CreateRegistration(newCarOwner, newVehicle, licenceNumber);
                        RegistrationOrderTypes? registrationOrderType = null;

                        if (RegistrationOrderDropDownList.Text.Contains("Umkennzeichnung")) // Umkennzeichnung
                        {
                            FahrzeugLabel.Text = "Fahrzeug";
                            FahrzeugLabel.ForeColor = Color.Blue;
                            if (!String.IsNullOrEmpty(licenceNumber))
                            {
                                registrationOrderType = RegistrationOrderTypes.Renumbering;
                            }
                            else
                            {
                                FahrzeugLabel.Text = "Für die Umkennzeichnung mind. neues Kennzeichen erforderlich!";
                                FahrzeugLabel.ForeColor = Color.Red;
                            }
                        }
                        else if (RegistrationOrderDropDownList.Text.Contains("Wiederzulassung")) // Wiederzulassung
                        {
                            registrationOrderType = RegistrationOrderTypes.Readmission;
                        }
                        else // Neuzulassung
                        {
                            registrationOrderType = RegistrationOrderTypes.NewAdmission;
                        }


                        if (registrationOrderType.HasValue)
                        {
                            newRegistrationOrder = CreateRegistrationOrder(registrationOrderType.Value, licenceNumber, oldLicenceNumber, newVehicle,
                                    newRegistration, locationId);

                            if (newRegistrationOrder != null)
                            {
                                ProcessRegistrationOrderForLargeCustomer(newRegistrationOrder, productId, price, costCenter, newRegistration, newVehicle, locationId,
                                    registrationOrderType.Value, ((RadButton)(sender)).ID);
                            }
                        }

                        VehicleIdField.Value = "";
                    }
                    catch (Exception ex)
                    {
                        SubmitChangesErrorLabel.Visible = true;
                        SubmitChangesErrorLabel.Text = ex.Message;
                    }
                }
            }
        }

        #endregion

        #region Methods

        protected override bool AddAnotherProducts(RegistrationOrder regOrd, int? locationId)
        {
            bool result = false;
            string produktId = "";
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
                                OrderItem newOrderItem = null;

                                produktId = splited[0];

                                if (!String.IsNullOrEmpty(produktId))
                                {
                                    int? costCenterId = null;
                                    Price newPrice = null;

                                    var newProduct = ProductManager.GetById(Int32.Parse(produktId));
                                    if (locationId == null) //small
                                    {
                                        newPrice = PriceManager.GetEntities(q => q.ProductId == newProduct.Id && q.LocationId == null).SingleOrDefault();
                                    }
                                    else //large
                                    {
                                        costCenterId = Int32.Parse(splited[1]);
                                        newPrice = PriceManager.GetEntities(q => q.ProductId == newProduct.Id && q.LocationId == locationId).SingleOrDefault();
                                        if (newPrice == null)
                                            newPrice = PriceManager.GetEntities(q => q.ProductId == newProduct.Id && q.LocationId == null).SingleOrDefault();
                                    }

                                    var orderToUpdate = OrderManager.GetEntities(q => q.OrderNumber == regOrd.OrderNumber).SingleOrDefault();

                                    if (orderToUpdate != null)
                                    {
                                        CostCenter costCenter = null;
                                        if (costCenterId.HasValue)
                                        {
                                            costCenter = CostCenterManager.GetById(costCenterId.Value);
                                        }

                                        newOrderItem = OrderManager.AddOrderItem(orderToUpdate, newProduct.Id, newPrice.Amount, 1, costCenter, null, false);
                                        if (newPrice.AuthorativeCharge.HasValue)
                                        {
                                            OrderManager.AddOrderItem(orderToUpdate, newProduct.Id, newPrice.AuthorativeCharge.Value, 1, costCenter, newOrderItem.Id,
                                                newPrice.AuthorativeCharge.HasValue);
                                        }

                                        result = true;
                                    }
                                }
                            }
                            catch (Exception ex)
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
            return result;
        }
        
        // findet alle angezeigte textboxen und überprüft ob die nicht leer sind
        protected override bool CheckIfBoxenEmpty()
        {
            bool gibtsBoxenDieLeerSind = false;
            bool iFound1VisibleBox = false;
            bool carOwnerMin = false;
            int count = 0;
            List<Control> allControls = GetAllControls();
            //fallse leer - soll aus der Logik rausgenommen
            if (String.IsNullOrEmpty(PruefzifferBox.Text))
                PruefzifferBox.Enabled = false;
            if (String.IsNullOrEmpty(RegistrationOrderDropDownList.SelectedValue) || String.IsNullOrEmpty(ZulassungsstelleComboBox.SelectedValue) ||
                String.IsNullOrEmpty(CostCenterDropDownList.SelectedValue) || String.IsNullOrEmpty(LocationDropDownList.SelectedValue) || DienstleistungTreeView.Nodes.Count == 0)
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

                            else if (box.Enabled == true && String.IsNullOrEmpty(box.Text))
                            {
                                box.BorderColor = System.Drawing.Color.Red;
                                gibtsBoxenDieLeerSind = true;
                            }
                            else
                            {
                                box.BorderColor = System.Drawing.Color.Black;
                            }
                        }
                    }
                }
            }
            if (!PruefzifferBox.Enabled)
                PruefzifferBox.Enabled = true;
            if (iFound1VisibleBox == false)
                gibtsBoxenDieLeerSind = true;
            return gibtsBoxenDieLeerSind;
        }
        
        #endregion
    }
}