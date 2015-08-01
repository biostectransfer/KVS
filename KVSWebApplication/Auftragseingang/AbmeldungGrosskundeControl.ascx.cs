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
        protected override HiddenField VehicleId { get { return this.vehicleIdField; } }
        protected override RequiredFieldValidator InvoiceValidator { get { return this.InvoiceRecValidator; } }
        protected override RadWindow RadWindow { get { return this.AddAdressRadWindow; } }

        #region Dates

        protected override RadMonthYearPicker Registration_GeneralInspectionDatePicker { get { return this.Registration_GeneralInspectionDateBox; } }
        protected override RadDatePicker FirstRegistrationDatePicker { get { return this.FirstRegistrationDateBox; } }
        protected override RadDatePicker RegistrationDatePicker { get { return null; } }

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
            if (Session["CurrentUserId"] != null)
            {
                if (!String.IsNullOrEmpty(Session["CurrentUserId"].ToString()))
                {
                    CheckUserPermissions();
                }
            }
        }

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
                var addedNode = new RadTreeNode(ProductAbmDropDownList.Text + "(" + CostCenterDropDownList.Text + ")", value);
                target.Nodes.Add(addedNode);
            }
        }

        #endregion

        #region Button Clicked
        //Fahrzeug abmelden
        protected void AbmeldenButton_Clicked(object sender, EventArgs e)
        {
            AbmeldungOkLabel.Visible = false;
            SubmitChangesErrorLabel.Visible = false;
            ErrorLeereTextBoxenLabel.Visible = false;

            int? locationId = null;
            if (!String.IsNullOrEmpty(LocationDropDownList.SelectedValue))
                locationId = Int32.Parse(LocationDropDownList.SelectedValue);


            if (CheckRegistrationFields()) //exists empty required fields
            {
                var productsPriceCheck = CheckIfAllProduktsHavingPrice(locationId);
                if (!String.IsNullOrEmpty(productsPriceCheck))
                {
                    ErrorLeereTextBoxenLabel.Text = String.Format("Für {0} wurde kein Price gefunden!", productsPriceCheck);
                    ErrorLeereTextBoxenLabel.Visible = true;
                    return;
                }


                if (DienstleistungTreeView.Nodes.Count > 0)
                {
                    ErrorLeereTextBoxenLabel.Visible = false;
                    SubmitChangesErrorLabel.Visible = false;
                    RadTreeNode node = DienstleistungTreeView.Nodes[0];
                    string[] splited = node.Value.Split(';');

                    var productId = Int32.Parse(splited[0]);

                    var price = FindPrice(productId);
                    if (price == null)
                    {
                        ErrorLeereTextBoxenLabel.Text = "Kein Preis gefunden!";
                        ErrorLeereTextBoxenLabel.Visible = true;
                        return;
                    }

                    AbmeldungOkLabel.Visible = false;
                    SubmitChangesErrorLabel.Visible = false;
                    ErrorLeereTextBoxenLabel.Visible = false;

                    int? costCenterId = null;
                    if (!String.IsNullOrEmpty(splited[1]))
                        costCenterId = Int32.Parse(splited[1]);
                    else if(!String.IsNullOrEmpty(CostCenterDropDownList.SelectedValue))
                        costCenterId = Int32.Parse(CostCenterDropDownList.SelectedValue);

                    try
                    {
                        AbmeldungOkLabel.Visible = false;
                        string licenceNumber = String.Empty;
                        var oldLicenceNumber = String.Empty;
                        Vehicle vehicle = null;
                        Registration newRegistration = null;
                                                

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


                        CostCenter costCenter = null;
                        if (costCenterId.HasValue)
                        {
                            costCenter = CostCenterManager.GetById(costCenterId.Value);
                        }

                        //neues DeregistrationOrder erstellen
                        var newDeregOrder = DeregistrationOrderManager.CreateDeregistrationOrder(Int32.Parse(CustomerDropDownList.SelectedValue), vehicle, newRegistration, 
                            locationId, Int32.Parse(ZulassungsstelleComboBox.SelectedValue));
                        
                        //adding new Deregestrationorder Items
                        var newOrderItem1 = OrderManager.AddOrderItem(newDeregOrder.Order, productId, price.Amount, 1, costCenter, null, false);
                        if (price.AuthorativeCharge.HasValue)
                        {
                            var newOrderItem2 = OrderManager.AddOrderItem(newDeregOrder.Order, productId, price.AuthorativeCharge.Value, 1, costCenter, newOrderItem1.Id, true);
                        }


                        if (DienstleistungTreeView.Nodes.Count > 1)
                        {
                            AddAnotherProducts(newDeregOrder, locationId);
                        }
                        
                        if(!String.IsNullOrEmpty(vehicleIdField.Value)) //falls Auto schon gefunden wurde
                        {
                            vehicle.CurrentRegistrationId = newRegistration.Id;
                            VehicleManager.SaveChanges();
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

        #region Linq Data Sources

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
                
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetAllSmallCustomers();
        }

        #endregion
        
        #region Methods
        
        protected bool AddAnotherProducts(DeregistrationOrder deRegOrd, int? locationId)
        {
            bool result = false;
            string produktId = "";
            string costCenterId = "";
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
                                costCenterId = splited[1];

                                if (!String.IsNullOrEmpty(produktId))
                                {
                                    var newProduct = ProductManager.GetById(Int32.Parse(produktId));
                                    
                                    CostCenter costCenter = null;
                                    if (!String.IsNullOrEmpty(costCenterId))
                                    {
                                        costCenter = CostCenterManager.GetById(Int32.Parse(costCenterId));
                                    }

                                    var newPrice = PriceManager.GetEntities(q => q.ProductId == newProduct.Id && q.LocationId == locationId).SingleOrDefault();

                                    if (newPrice == null)
                                        newPrice = PriceManager.GetEntities(q => q.ProductId == newProduct.Id && q.LocationId == null).SingleOrDefault();

                                    var orderToUpdate = OrderManager.GetEntities(q => q.OrderNumber == deRegOrd.OrderNumber).SingleOrDefault();

                                    if (orderToUpdate != null)
                                    {
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
            return result;
        }

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

                var addedNode = new RadTreeNode(product.Name, value);
                target.Nodes.Add(addedNode);
            }
        }

        #endregion
    }
}