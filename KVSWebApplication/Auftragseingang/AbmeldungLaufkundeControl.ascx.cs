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
    public partial class AbmeldungLaufkundeControl : IncomingOrdersBase
    {
        #region Members

        protected override PermissionTypes PagePermission { get { return PermissionTypes.ABMELDEAUFTRAG_ANLEGEN; } }

        protected override Panel Panel { get { return this.EingangAbmeldungPanel; } }
        protected override RadTextBox AccountNumberTextBox { get { return this.BankAccount_AccountnumberBox; } }
        protected override RadTextBox BankCodeTextBox { get { return this.BankAccount_BankCodeBox; } }
        protected override RadTextBox BankNameTextBox { get { return this.BankAccount_BankNameBox; } }
        protected override RadTextBox IBANTextBox { get { return this.txbBancAccountIban; } }
        protected override RadTextBox BICTextBox { get { return this.txbBankAccount_Bic; } }
        protected override Label CustomerHistoryLabel { get { return this.SmallCustomerHistorie; } }
        protected override RadComboBox CustomerDropDown { get { return this.CustomerDropDownList; } }
        protected override RadComboBox LocationDropDown { get { return null; } }

        #endregion

        #region Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            AbmeldungLaufkunde auftragsEingang = Page as AbmeldungLaufkunde;
            RadScriptManager script = auftragsEingang.getScriptManager() as RadScriptManager;
            script.RegisterPostBackControl(AddAdressButton);
            LicenceBox1.Enabled = true;
            LicenceBox2.Enabled = true;
            LicenceBox3.Enabled = true;
            //first registration bekommt immer heutige Datum by default
            FirstRegistrationDateBox.SelectedDate = DateTime.Now;
            string target = Request["__EVENTARGUMENT"];
            if (target != null && target == "CreateOrder")
            {
                AbmeldenButton_Clicked(sender, e);
            }
            if (Session["CurrentUserId"] != null)
            {
                if (!String.IsNullOrEmpty(Session["CurrentUserId"].ToString()))
                {
                    CheckUserPermissions();
                }
                if (!Page.IsPostBack)
                {
                    CheckFields(getAllControls());
                    btnClearSelection_Click(this, null);
                }
            }
        }

        protected string CheckIfAllProduktsHavingPrice(int? locationId)
        {
            string allesHatGutGelaufen = "";
            string ProduktId = "";
            string CostCenterId = "";
            foreach (RadTreeNode node in DienstleistungTreeView.Nodes)
            {
                if (!String.IsNullOrEmpty(node.Value))
                {
                    string[] splited = node.Value.Split(';');
                    if (splited.Length == 2)
                    {
                        try
                        {
                            KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                            Price newPrice;
                            ProduktId = splited[0];
                            CostCenterId = splited[1];
                            if (!String.IsNullOrEmpty(ProduktId))
                            {
                                var productId = Int32.Parse(ProduktId);

                                KVSCommon.Database.Product newProduct = dbContext.Product.SingleOrDefault(q => q.Id == productId);
                                if (locationId == null) //small
                                {
                                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == null);
                                    if (newPrice == null)
                                    {
                                        allesHatGutGelaufen += " " + node.Text + " ";
                                    }
                                    else
                                    {
                                    }
                                }
                            }
                        }
                        catch
                        {
                            return "";
                        }
                    }
                }
            }
            return allesHatGutGelaufen;
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
            if (CheckIfBoxenNotEmpty()) //gibt es leer boxen, die angezeigt sind.
            {
                if (DienstleistungTreeView.Nodes.Count == 0)
                {
                    ErrorLeereTextBoxenLabel.Text = "Bitte Dienstleistung hinzufügen!";
                    ErrorLeereTextBoxenLabel.Visible = true;
                }
                else if (String.IsNullOrEmpty(ZulassungsstelleComboBox.SelectedValue))
                {
                    ErrorLeereTextBoxenLabel.Text = "Bitte wählen Sie die Zulassungsstelle aus!";
                    ErrorLeereTextBoxenLabel.Visible = true;
                }
                else
                {
                    ErrorLeereTextBoxenLabel.Text = "Bitte FIN eingeben!";
                    ErrorLeereTextBoxenLabel.Visible = true;
                }
            }
            else if (!String.IsNullOrEmpty(CheckIfAllProduktsHavingPrice(locationId)))
            {
                ErrorLeereTextBoxenLabel.Text = "Für " + CheckIfAllProduktsHavingPrice(locationId) + " wurde kein Price gefunden!";
                ErrorLeereTextBoxenLabel.Visible = true;
                return;
            }
            else if (String.IsNullOrEmpty(ZulassungsstelleComboBox.SelectedValue))
            {
                ErrorLeereTextBoxenLabel.Text = "Bitte wählen Sie die Zulassungstelle aus";
                ErrorLeereTextBoxenLabel.Visible = true;
                return;
            }
            else if (DienstleistungTreeView.Nodes.Count > 0)
            {
                AddCustomer();
                ErrorLeereTextBoxenLabel.Visible = false;
                SubmitChangesErrorLabel.Visible = false;
                RadTreeNode node = DienstleistungTreeView.Nodes[0];
                string[] splited = node.Value.Split(';');
                ProduktId = splited[0];
                CostCenterId = splited[1];
                try
                {
                    KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                    Adress newAdress = null;
                    Contact newContact = null;
                    BankAccount newBankAccount = null;
                    CarOwner newCarOwner = null;
                    Registration newRegistration = null;
                    Price price = null;

                    Vehicle newVehicle = null;
                    DateTime? FirstRegistrationDate = null;

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
                        dbContext.SubmitChanges();
                    }
                    newDeregOrder = DeregistrationOrder.CreateDeregistrationOrder(Int32.Parse(Session["CurrentUserId"].ToString()), Int32.Parse(CustomerDropDownList.SelectedValue),
                        newVehicle, newRegistration, locationId, Int32.Parse(ZulassungsstelleComboBox.SelectedValue), dbContext);
                    dbContext.SubmitChanges();
                    //adding new Deregestrationorder Items
                    AddAnotherProducts(newDeregOrder, locationId);
                    if (String.IsNullOrEmpty(vehicleIdField.Value)) //falls Auto schon gefunden wurde
                    {
                        newVehicle.CurrentRegistrationId = newRegistration.Id;
                        dbContext.SubmitChanges();
                    }
                    if (invoiceNow.Checked == true && invoiceNow.Enabled == true)
                    {
                        MakeInvoiceForSmallCustomer(Int32.Parse(CustomerDropDownList.SelectedValue), newDeregOrder);
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
        // findet alle textboxen und macht die leer ohne die ganze Seite neu zu laden
        protected void MakeAllControlsEmpty()
        {
            List<Control> allControls = getAllControls();
            DateTime? nullDate = null;
            Registration_GeneralInspectionDateBox.SelectedDate = nullDate;
            FirstRegistrationDateBox.SelectedDate = DateTime.Now;
            HSNSearchLabel.Visible = false;
            DienstleistungTreeView.Nodes.Clear();
            CustomerDropDownList.ClearSelection();
            ProductAbmDropDownList.ClearSelection();
            ZulassungsstelleComboBox.ClearSelection();
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
            foreach (Control control in allControls)
            {
                foreach (Control subControl in control.Controls)
                {
                    if (subControl is RadTextBox)
                    {
                        RadTextBox box = subControl as RadTextBox;
                        if (box.Enabled == true && box != Adress_CountryBox)
                        {
                            box.Text = "";
                        }
                    }
                }
            }
        }
        // findet alle angezeigte textboxen und überprüft ob die nicht leer sind
        protected bool CheckIfBoxenNotEmpty()
        {
            bool gibtsBoxenDieLeerSind = false;
            List<Control> allControls = getAllControls();

            //fallse leer - soll aus der Logik rausgenommen
            if (String.IsNullOrEmpty(PruefzifferBox.Text))
                PruefzifferBox.Enabled = false;


            if (DienstleistungTreeView.Nodes.Count == 0)
            {
                return true;
            }
            foreach (Control control in allControls)
            {
                if (control.Visible == true)
                {
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
                try
                {
                    VINBox.Text = VINBox.Text.ToUpper();
                    FahrzeugLabel.Visible = false;
                    KVSEntities dbContext = new KVSEntities();
                    var autoQuery = dbContext.Vehicle.Single(q => q.VIN == VINBox.Text);
                    vehicleIdField.Value = autoQuery.Id.ToString();
                    if (autoQuery.CurrentRegistrationId.HasValue)
                    {
                        var registration = dbContext.Registration.Single(q => q.Id == autoQuery.CurrentRegistrationId.Value);
                        
                        string kennzeichen = string.Empty;
                        VINBox.Text = VINBox.Text;
                        Vehicle_VariantBox.Text = autoQuery.Variant;
                        kennzeichen = registration.Licencenumber;
                        string[] newKennzeichen = kennzeichen.Split('-');
                        if (newKennzeichen.Length == 3)
                        {
                            LicenceBox1.Text = newKennzeichen[0];
                            LicenceBox2.Text = newKennzeichen[1];
                            LicenceBox3.Text = newKennzeichen[2];
                        }
                        Registration_GeneralInspectionDateBox.SelectedDate = registration.GeneralInspectionDate;
                        Vehicle_VariantBox.Text = autoQuery.Variant;
                        HSNAbmBox.Text = autoQuery.HSN;
                        TSNAbmBox.Text = autoQuery.TSN;
                        Vehicle_ColorBox.Text = autoQuery.ColorCode.ToString();
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
                // falls kein Fahrzeug gefunden
                catch (Exception ex)
                {
                    FahrzeugLabel.Text = "Fahrzeug mit dem FIN " + VINBox.Text + " wurde nicht gefunden.";
                    VINBox.Focus();
                }
            }
        }
        protected void ZulassungsstelleDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities con = new KVSEntities();
            var zulassungsstelleQuery = from zul in con.RegistrationLocation
                                        orderby zul.RegistrationLocationName
                                        select new
                                        {
                                            Name = zul.RegistrationLocationName,
                                            Value = zul.ID
                                        };
            e.Result = zulassungsstelleQuery;
        }
        private Price findPrice(string produktId)
        {
            Price newPrice = null;
            KVSEntities dbContext = new KVSEntities();
            newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == Int32.Parse(produktId) && q.LocationId == null);
            return newPrice;
        }
        #endregion
        protected void ProductAbmDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities con = new KVSEntities();

            IQueryable productQuery1 = null;
            productQuery1 = from prod in con.Product
                            let price = con.Price.SingleOrDefault(q => q.ProductId == prod.Id && q.LocationId == null)
                            join prA in con.PriceAccount on price.Id equals prA.PriceId
                            where (prod.OrderType.Id == (int)OrderTypes.Cancellation || prod.OrderType.Id == (int)OrderTypes.Common)
                            orderby prod.Name
                            select new
                            {
                                ItemNumber = prod.ItemNumber,
                                Name = prod.Name,
                                Value = prod.Id,
                                Category = prod.ProductCategory.Name,
                                Price = Math.Round(price.Amount, 2, MidpointRounding.AwayFromZero).ToString(),
                                AuthCharge = price.AuthorativeCharge.HasValue ? Math.Round(price.AuthorativeCharge.Value, 2, MidpointRounding.AwayFromZero).ToString().Trim() : ""
                            };
            e.Result = productQuery1;
        }
        #region Index Changed

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
                CheckFields(getAllControls());
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
            string ProduktId = "";
            int itemIndexValue = 0;
            decimal amount = 0;

            var nodes = this.Request.Form.AllKeys.Where(q => q.Contains("txtItemPrice_"));
            foreach (var node in nodes)
            {
                if (!String.IsNullOrEmpty(node))
                {
                    var dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                    var orderNumber = regOrd.OrderNumber;
                    Price newPrice = null;
                    OrderItem newOrderItem1 = null;
                    ProduktId = node.Split('_')[1];
                    if (!String.IsNullOrEmpty(ProduktId))
                    {
                        var productId = Int32.Parse(ProduktId);

                        KVSCommon.Database.Product newProduct = dbContext.Product.SingleOrDefault(q => q.Id == productId);
                        if (locationId == null) //small
                        {
                            newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == null);
                        }
                        var orderToUpdate = dbContext.Order.SingleOrDefault(q => q.OrderNumber == orderNumber);
                        orderToUpdate.LogDBContext = dbContext;
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
                            newOrderItem1 = orderToUpdate.AddOrderItem(newProduct.Id, amount, 1, null, null, false, dbContext);
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
                                orderToUpdate.AddOrderItem(newProduct.Id, amount, 1, null, newOrderItem1.Id, newPrice.AuthorativeCharge.HasValue, dbContext);
                            }
                            itemIndexValue = itemIndexValue + 1;
                            dbContext.SubmitChanges();
                        }
                    }
                }
            }
        }
        #endregion
        #region Linq Data Sources
        protected void LocationLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities con = new KVSEntities();
            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue.ToString()))
            {
                var locationQuery = from loc in con.Location
                                    join cust in con.Customer on loc.CustomerId equals cust.Id
                                    where loc.CustomerId == Int32.Parse(CustomerDropDownList.SelectedValue)
                                    select new
                                    {
                                        Name = loc.Name,
                                        Value = loc.Id
                                    };
                e.Result = locationQuery;
            }
            else
            {
                var locationQuery = from loc in con.Location
                                    join cust in con.Customer on loc.CustomerId equals cust.Id
                                    where loc.CustomerId == null
                                    select new
                                    {
                                        Name = loc.Name,
                                        Value = loc.Id
                                    };
                e.Result = locationQuery;
            }
        }
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities con = new KVSEntities();
            var customerQuery = from cust in con.Customer
                                where cust.Id == cust.SmallCustomer.CustomerId
                                orderby cust.Name
                                select new
                                {
                                    Name = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name,
                                    Value = cust.Id,
                                    Matchcode = cust.MatchCode,
                                    Kundennummer = cust.CustomerNumber
                                };
            e.Result = customerQuery;
        }
        #endregion
        protected void CheckFields(List<Control> listOfControls)
        {
            foreach (Control control in listOfControls)
            {
                control.Visible = true;
            }
            HalterLabel.Visible = true;
            HalterdatenLabel.Visible = true;
            KontaktdatenLabel.Visible = true;
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
                string value = ProductAbmDropDownList.SelectedValue.ToString() + ";" + costCenter;
                RadTreeNode addedNode = new RadTreeNode(ProductAbmDropDownList.Text, value);
                target.Nodes.Add(addedNode);
            }
        }
        protected void CheckIfButtonShouldBeEnabled()
        {
            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue)
                 && !String.IsNullOrEmpty(ProductAbmDropDownList.SelectedValue)) //small without costcenter and location
                AbmeldenButton.Enabled = true;
            if (AbmeldenButton.Enabled == true)
            {
                //falls keine Pflichtfelder angezeigt sind - button schließen
                List<Control> allControls = getAllControls();
                foreach (Control control in allControls)
                {
                    if (control.Visible == true)
                    {
                        AbmeldenButton.Enabled = true;
                        break;
                    }
                    else
                        AbmeldenButton.Enabled = false;
                }
            }
        }
        protected void HideAllControls()
        {
            List<Control> controlsToHide = new List<Control>();
            controlsToHide = getAllControls();
            KVSEntities con = new KVSEntities();
            var cont = from largCust in con.LargeCustomerRequiredField
                       join reqFiled in con.RequiredField on largCust.RequiredFieldId equals reqFiled.Id
                       join ordTyp in con.OrderType on reqFiled.OrderTypeId equals ordTyp.Id
                       where ordTyp.Id == (int)OrderTypes.Cancellation
                       select reqFiled.Name;
            foreach (var nameCon in cont)
            {
                foreach (Control control in controlsToHide)
                {
                    if (control.ID == nameCon)
                    {
                        control.Visible = false;
                        FahrzeugLabel.Visible = false;
                        HalterLabel.Visible = false;
                        HalterdatenLabel.Visible = false;
                        KontaktdatenLabel.Visible = false;
                    }
                }
            }
        }
        protected void MakeInvoiceForSmallCustomer(int customerId, DeregistrationOrder regOrder)
        {
            try
            {
                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                var newOrder = dbContext.Order.Single(q => q.CustomerId == customerId && q.OrderNumber == regOrder.OrderNumber);
                smallCustomerOrderHiddenField.Value = regOrder.OrderNumber.ToString();
                //updating order status
                newOrder.LogDBContext = dbContext;
                newOrder.Status = (int)OrderStatusTypes.Closed;
                //updating orderitems status                          
                foreach (OrderItem ordItem in newOrder.OrderItem)
                {
                    ordItem.LogDBContext = dbContext;
                    if (ordItem.Status != (int)OrderItemStatusTypes.Cancelled)
                    {
                        ordItem.Status = (int)OrderItemStatusTypes.Closed;
                    }
                }
                dbContext.SubmitChanges();
                //updating order und items status one more time to make it abgerechnet
                newOrder.LogDBContext = dbContext;
                newOrder.Status = (int)OrderStatusTypes.Payed;
                dbContext.SubmitChanges();
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
            KVSEntities dbContext = new KVSEntities();
            var locationQuery = (from adr in dbContext.Adress
                                 join cust in dbContext.Customer on adr.Id equals cust.InvoiceAdressId
                                 where cust.Id == Int32.Parse(CustomerDropDownList.SelectedValue)
                                 select adr).SingleOrDefault();
            if (locationQuery != null)
            {
                StreetTextBox.Text = locationQuery.Street;
                StreetNumberTextBox.Text = locationQuery.StreetNumber;
                ZipcodeTextBox.Text = locationQuery.Zipcode;
                CityTextBox.Text = locationQuery.City;
                CountryTextBox.Text = locationQuery.Country;
                LocationLabelWindow.Text = "Fügen Sie bitte die Adresse für " + CustomerDropDownList.Text + " hinzu";
                ZusatzlicheInfoLabel.Visible = false;
                if (CustomerDropDownList.SelectedIndex == 1) // small
                {
                    ZusatzlicheInfoLabel.Visible = true;
                }
            }
        }
        // Create new Adress in der DatenBank
        protected void OnAddAdressButton_Clicked(object sender, EventArgs e)
        {
            //Adress Eigenschaften
            string street = "",
                streetNumber = "",
                zipcode = "",
                city = "",
                country = "",
                invoiceRecipient = "";
            // OrderItem Eigenschaften
            string ProductName = "";
            decimal Amount = 0;

            street = StreetTextBox.Text;
            streetNumber = StreetNumberTextBox.Text;
            zipcode = ZipcodeTextBox.Text;
            city = CityTextBox.Text;
            country = CountryTextBox.Text;
            invoiceRecipient = InvoiceRecipient.Text;
            int itemCount = 0;
            TransactionScope scope = null;
            try
            {
                using (KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())))
                {
                    using (scope = new TransactionScope())
                    {
                        var newAdress = Adress.CreateAdress(street, streetNumber, zipcode, city, country, dbContext);
                        var myCustomer = dbContext.Customer.FirstOrDefault(q => q.Id == Int32.Parse(CustomerDropDownList.SelectedValue));
                        var newInvoice = Invoice.CreateInvoice(dbContext, Int32.Parse(Session["CurrentUserId"].ToString()), invoiceRecipient, newAdress,
                            Int32.Parse(CustomerDropDownList.SelectedValue), txbDiscount.Value, "Einzelrechnung");
                        //Submiting new Invoice and Adress
                        dbContext.SubmitChanges();
                        var orderQuery = dbContext.Order.SingleOrDefault(q => q.OrderNumber == Int32.Parse(smallCustomerOrderHiddenField.Value));
                        foreach (OrderItem ordItem in orderQuery.OrderItem)
                        {
                            ProductName = ordItem.ProductName;
                            Amount = ordItem.Amount;

                            CostCenter costCenter = null;
                            if (ordItem.CostCenterId.HasValue)
                            {
                                costCenter = dbContext.CostCenter.FirstOrDefault(o => o.Id == ordItem.CostCenterId.Value);
                            }

                            itemCount = ordItem.Count;
                            InvoiceItem newInvoiceItem = newInvoice.AddInvoiceItem(ProductName, Convert.ToDecimal(Amount), itemCount, ordItem, costCenter, dbContext);
                            ordItem.LogDBContext = dbContext;
                            ordItem.Status = (int)OrderItemStatusTypes.Payed;
                            //newInvoiceItem.VAT = myCustomer.VAT;
                            dbContext.SubmitChanges();
                        }
                        // Submiting new InvoiceItems
                        dbContext.SubmitChanges();
                        Print(newInvoice, dbContext);
                        scope.Complete();
                    }
                    MakeAllControlsEmpty();
                }
            }
            catch (Exception ex)
            {
                DienstleistungTreeView.Nodes.Clear();
                ErrorLeereTextBoxenLabel.Text = "Fehler: " + ex.Message;
                ErrorLeereTextBoxenLabel.Visible = true;
            }
        }
        protected string CheckIfFolderExistsAndReturnPathForPdf()
        {
            string newPdfPathAndName = "";
            if (!Directory.Exists(ConfigurationManager.AppSettings["DataPath"]))
            {
                Directory.CreateDirectory(ConfigurationManager.AppSettings["DataPath"]);
            }
            if (!Directory.Exists(ConfigurationManager.AppSettings["DataPath"] + "/" + Session["CurrentUserId"].ToString()))
            {
                Directory.CreateDirectory(ConfigurationManager.AppSettings["DataPath"] + "/" + Session["CurrentUserId"].ToString());
            }
            newPdfPathAndName = ConfigurationManager.AppSettings["DataPath"] + "/" + Session["CurrentUserId"].ToString() + "/Rechnung" + DateTime.Today.Day + "_" + DateTime.Today.Month + "_" + DateTime.Today.Year + "_" + Guid.NewGuid() + ".pdf";
            return newPdfPathAndName;
        }
        private void OpenPrintfile(string myFile)
        {
            string url = ConfigurationManager.AppSettings["BaseUrl"];
            string path = url + "UserData/" + Session["CurrentUserId"].ToString() + "/" + myFile;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Invoice", "<script>openFile('" + path + "');</script>", false);
        }
        protected void Print(Invoice newInvoice, KVSEntities dbContext)
        {
            using (MemoryStream memS = new MemoryStream())
            {
                InvoiceHelper.CreateAccounts(dbContext, newInvoice);
                newInvoice.Print(dbContext, memS, "");
                dbContext.SubmitChanges();
                string fileName = "Rechnung_" + newInvoice.InvoiceNumber.Number + "_" + newInvoice.CreateDate.Day + "_" + newInvoice.CreateDate.Month + "_" + newInvoice.CreateDate.Year + ".pdf";
                string serverPath = ConfigurationManager.AppSettings["DataPath"] + "\\UserData";
                if (!Directory.Exists(serverPath)) Directory.CreateDirectory(serverPath);
                if (!Directory.Exists(serverPath + "\\" + Session["CurrentUserId"].ToString())) Directory.CreateDirectory(serverPath + "\\" + Session["CurrentUserId"].ToString());
                serverPath = serverPath + "\\" + Session["CurrentUserId"].ToString();
                File.WriteAllBytes(serverPath + "\\" + fileName, memS.ToArray());
                OpenPrintfile(fileName);
                dbContext.SubmitChanges();
            }
        }
        protected List<Control> getAllControls()
        {
            if (controls.Count == 0)
            {
                controls.Add(Vehicle_Variant);
                controls.Add(Registration_GeneralInspectionDate);
                controls.Add(CarOwner_Name);
                controls.Add(CarOwner_Firstname);
                controls.Add(Adress_StreetNumber);
                controls.Add(Adress_Street);
                controls.Add(Adress_Zipcode);
                controls.Add(Adress_City);
                controls.Add(Adress_Country);
                controls.Add(Contact_Phone);
                controls.Add(Contact_Fax);
                controls.Add(Contact_MobilePhone);
                controls.Add(Contact_Email);
                controls.Add(BankAccount_BankName);
                controls.Add(BankAccount_Accountnumber);
                controls.Add(BankAccount_BankCode);
                controls.Add(Registration_eVBNumber);
                controls.Add(Vehicle_HSN);
                controls.Add(Vehicle_TSN);
                controls.Add(Vehicle_VIN);
                controls.Add(Registration_Licencenumber);
                controls.Add(RegistrationOrder_PreviousLicencenumber);
                controls.Add(Registration_EmissionCode);
                controls.Add(Registration_RegistrationDocumentNumber);
                controls.Add(Vehicle_FirstRegistrationDate);
                controls.Add(Vehicle_Color);
                controls.Add(IBANPanel);
            }
            return controls;
        }
        protected void btnClearSelection_Click(object sender, EventArgs e)
        {
            CustomerDropDownList.ClearSelection();
            MakeAllControlsEmpty();
            ClearAdressData(string.Empty);
            using (KVSEntities dbContext = new KVSEntities())
            {
                txbSmallCustomerNumber.Text = EmptyStringIfNull.generateIndividualNumber(dbContext.Customer.Max(q => q.CustomerNumber));
            }
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
                using (KVSEntities dbContext = new KVSEntities())
                {
                    var customerId = 0;
                    if (CustomerDropDownList.SelectedValue != string.Empty)
                        customerId = Int32.Parse(CustomerDropDownList.SelectedValue);
                    var checkThisCustomer = dbContext.Customer.SingleOrDefault(q => q.Id == customerId);
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
                using (KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())))
                {
                    try
                    {
                        var checkThisCustomer = dbContext.Customer.SingleOrDefault(q => q.CustomerNumber == txbSmallCustomerNumber.Text);
                        while (checkThisCustomer != null)
                        {
                            txbSmallCustomerNumber.Text = EmptyStringIfNull.generateIndividualNumber(dbContext.Customer.Max(q => q.CustomerNumber));
                            checkThisCustomer = dbContext.Customer.SingleOrDefault(q => q.CustomerNumber == txbSmallCustomerNumber.Text);
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
                            int zz = 0;
                            txbSmallCustomerZahlungsziel.Text = txbSmallCustomerZahlungsziel.Text.Trim();
                            if (txbSmallCustomerZahlungsziel.Text != string.Empty)
                            {
                                try
                                {
                                    zz = int.Parse(txbSmallCustomerZahlungsziel.SelectedValue);
                                }
                                catch
                                {
                                    throw new Exception("Das Zahlungsziel muss eine Gleitkommazahl sein");
                                }
                            }
                            var newSmallCustomer = SmallCustomer.CreateSmallCustomer(txbSmallCustomerVorname.Text, txbSmallCustomerNachname.Text, txbSmallCustomerTitle.Text, cmbSmallCustomerGender.SelectedValue,
                              txbSmallCustomerStreet.Text, txbSmallCustomerNr.Text, txbSmallCustomerZipCode.Text, cmbSmallCustomerCity.Text, txbSmallCustomerCountry.Text, txbSmallCustomerPhone.Text,
                                txbSmallCustomerFax.Text, txbSmallCustomerMobil.Text, txbSmallCustomerEmail.Text, vat, zz, txbSmallCustomerNumber.Text, dbContext);
                            dbContext.SubmitChanges();
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
        }

        #endregion
    }
}