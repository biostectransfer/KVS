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
namespace KVSWebApplication.Auftragseingang
{
    //Neuzulassung Laufkunde
    public partial class NeuzulassungLaufkunde : System.Web.UI.UserControl
    {
        List<Control> controls = new List<Control>();
        protected void Page_Load(object sender, EventArgs e)
        {
            ZulassungLaufkunde auftrag = Page as ZulassungLaufkunde;
            RadScriptManager script = auftrag.getScriptManager() as RadScriptManager;
            script.RegisterPostBackControl(AddAdressButton);
            //first registration bekommt immer heutige Datum by default
            FirstRegistrationDateBox.SelectedDate = DateTime.Now;
            SaveState();
            string target = Request["__EVENTARGUMENT"]; 

            if (target !=null && target=="CreateOrder")
            {
                AuftragZulassenButton_Clicked(sender, e);
            }
            if (Session["CurrentUserId"] != null)
            {
                if (!String.IsNullOrEmpty(Session["CurrentUserId"].ToString()))
                {
                    CheckUserPermissions();
                    if (!Page.IsPostBack)
                    {
                        CheckFields(getAllControls());
                        btnClearSelection_Click(this, null);
                    }
                }
            }
        }
        protected void CheckUserPermissions()
        {
            List<string> userPermissions = new List<string>();
            userPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(Int32.Parse(Session["CurrentUserId"].ToString())));
            if (userPermissions.Count > 0)
            {
                if (userPermissions.Contains("ZULASSUNGSAUFTRAG_ANLEGEN"))
                {
                    ZulassungPanel.Enabled = true;
                }
            }
        }
        protected void CheckUmsatzForSmallCustomer()
        {
            SmallCustomerHistorie.Visible = true;
            DataClasses1DataContext con = new DataClasses1DataContext();
            var newQuery = from ord in con.Order
                           let registration = ord.RegistrationOrder != null ? ord.RegistrationOrder.Registration : ord.DeregistrationOrder.Registration
                           where ord.Status == (int)OrderStatusTypes.Payed
                           select new
                           {
                               CustomerId = ord.CustomerId,
                               OrderNumber = ord.OrderNumber,
                           };
            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue))
            {
                try
                {
                    newQuery = newQuery.Where(q => q.CustomerId == Int32.Parse(CustomerDropDownList.SelectedValue));
                }
                catch (Exception ex)
                {
                    SubmitChangesErrorLabel.Text = ex.Message;
                    SubmitChangesErrorLabel.Visible = true;
                }
            }
            // Allgemein
            string countAuftrag = newQuery.Count().ToString();
            decimal gebuehren = 0;
            decimal umsatz = 0;
            //Amtliche Gebühr
            foreach (var newOrder in newQuery)
            {
                var order = con.Order.SingleOrDefault(q => q.OrderNumber == newOrder.OrderNumber);
                if (order != null)
                {
                    foreach (OrderItem orderItem in order.OrderItem)
                    {
                        if (orderItem.IsAuthorativeCharge && orderItem.Status == (int)OrderItemStatusTypes.Payed)
                            gebuehren = gebuehren + orderItem.Amount;
                        else if (!orderItem.IsAuthorativeCharge && orderItem.Status == (int)OrderItemStatusTypes.Payed)
                            umsatz = umsatz + orderItem.Amount;
                    }
                }
            }
            SmallCustomerHistorie.Text = "Historie: <br/> Gesamt: " + countAuftrag + " <br/> Umsatz: " + umsatz.ToString("C2") + "<br/> Gebühren: " + gebuehren.ToString("C2");
        }
        #region Methods
        // Suche nach Price. Falls keine gibt - stand.Price nehmen
        private Price findPrice(string productId)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            Price newPrice = null;
            newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == Int32.Parse(productId) && q.LocationId == null);
            return newPrice;
        }
        #endregion
        #region Index Changed
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
                RadTreeNode addedNode = new RadTreeNode(ProductDropDownList.Text, value);
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
                CheckFields(getAllControls());
                CheckUmsatzForSmallCustomer();
            }
        }
        #endregion
        #region Linq Data Source
        protected void ProductDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var selectedCustomer = 0;

            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue))
                selectedCustomer = Int32.Parse(CustomerDropDownList.SelectedValue);
            IQueryable productQuery = null;
            if (!String.IsNullOrEmpty(RegistrationOrderDropDownList.SelectedValue))
            {
                productQuery = from prd in con.Product
                               let price = con.Price.SingleOrDefault(q => q.ProductId == prd.Id && q.LocationId == null)
                               join prA in con.PriceAccount on price.Id equals prA.PriceId
                               where (prd.RegistrationOrderTypeId == Int32.Parse(RegistrationOrderDropDownList.SelectedValue) || prd.OrderType.Name == "Allgemein")
                               orderby prd.Name
                               select new
                               {
                                   ItemNumber = prd.ItemNumber,
                                   Name = prd.Name,
                                   Value = prd.Id,
                                   Category = prd.ProductCategory.Name,
                                   Price = Math.Round(price.Amount, 2, MidpointRounding.AwayFromZero).ToString(),
                                   AuthCharge = price.AuthorativeCharge.HasValue ? Math.Round(price.AuthorativeCharge.Value, 2, MidpointRounding.AwayFromZero).ToString().Trim() : ""
                               };
            }
            else
            {
                productQuery = from p in con.Price
                               join prA in con.PriceAccount on p.Id equals prA.PriceId
                               where p.Location.CustomerId == null
                               select new
                               {
                                   ItemNumber = p.Product.ItemNumber,
                                   Name = p.Product.Name,
                                   Value = p.Product.Id,
                                   Category = p.Product.ProductCategory.Name,
                                   Price = Math.Round(p.Amount, 2, MidpointRounding.AwayFromZero).ToString(),
                                   AuthCharge = p.AuthorativeCharge.HasValue ? Math.Round(p.AuthorativeCharge.Value, 2, MidpointRounding.AwayFromZero).ToString().Trim() : ""
                               };
            }
            e.Result = productQuery;
        }
        // Auswahl der KundenName
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
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
        protected void ZulassungsstelleDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var zulassungsstelleQuery = from zul in con.RegistrationLocation
                                        orderby zul.RegistrationLocationName
                                        select new
                                        {
                                            Name = zul.RegistrationLocationName,
                                            Value = zul.ID
                                        };
            e.Result = zulassungsstelleQuery;
        }
        protected void RegistrationOrderDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var regOrdQuery = from regord in con.RegistrationOrderType
                              select new
                              {
                                  Name = regord.Name,
                                  Value = regord.Id
                              };
            e.Result = regOrdQuery;
        }
        #endregion
        protected void CheckIfButtonShouldBeEnabled()
        {
            if (AuftragZulassenButton.Enabled == true)
            {
                //falls keine Pflichtfelder angezeigt sind - button schließen
                List<Control> allControls = getAllControls();
                foreach (Control control in allControls)
                {
                    if (control.Visible == true)
                    {
                        AuftragZulassenButton.Enabled = true;
                        break;
                    }
                    else
                        AuftragZulassenButton.Enabled = false;
                }
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
                try
                {
                    VINBox.Text = VINBox.Text.ToUpper();
                    DataClasses1DataContext dbContext = new DataClasses1DataContext();
                    var autoQuery = dbContext.Vehicle.SingleOrDefault(q => q.VIN == VINBox.Text);
                    if (autoQuery != null)
                    {
                        //wird als cache field für die Kennzeichnung bei der Umkennzeichnung benutzt
                        if (autoQuery.Registration1 != null)
                        {
                            string kennzeichen = string.Empty;
                            LicenceNumberCacheField.Value = autoQuery.Registration1.Licencenumber;
                            RegistrationIdField.Value = autoQuery.Registration1.Id.ToString();
                            kennzeichen = autoQuery.Registration1.Licencenumber;
                            string[] newKennzeichen = kennzeichen.Split('-');
                            if (newKennzeichen.Length == 3)
                            {
                                LicenceBox1.Text = newKennzeichen[0];
                                LicenceBox2.Text = newKennzeichen[1];
                                LicenceBox3.Text = newKennzeichen[2];
                            }
                            Registration_GeneralInspectionDateBox.SelectedDate = autoQuery.Registration1.GeneralInspectionDate;
                            RegDocNumBox.Text = autoQuery.Registration1.RegistrationDocumentNumber;
                            EmissionsCodeBox.Text = autoQuery.Registration1.EmissionCode;
                        }
                        VehicleIdField.Value = autoQuery.Id.ToString();
                        Vehicle_VariantBox.Text = autoQuery.Variant;
                        HSNBox.Text = autoQuery.HSN;
                        TSNBox.Text = autoQuery.TSN;
                        Vehicle_ColorBox.Text = autoQuery.ColorCode.ToString();
                        if (autoQuery.Registration1 != null)
                        {
                            CarOwner owner = autoQuery.Registration1.CarOwner;
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
                    }
                }
                // falls kein Fahrzeug gefunden
                catch (Exception ex)
                {
                    VINBox.Focus();
                }
            }
        }
        #region Button Clicked
        //Neue Auftragseingang
        protected void AuftragZulassenButton_Clicked(object sender, EventArgs e)
        {          
            string ProduktId = "";
            string CostCenterId = "";
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
                    DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
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
                        price = findPrice(ProduktId);
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
                        price = findPrice(ProduktId);
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
                    SubmitChangesErrorLabel.Text = "Fehler"+ ex.Message;
                }
            }
        }
        protected void genIban_Click(object sender, EventArgs e)
        {
            if (BankAccount_AccountnumberBox.Text != string.Empty && BankAccount_BankCodeBox.Text != string.Empty
                && EmptyStringIfNull.IsNumber(BankAccount_AccountnumberBox.Text) && !String.IsNullOrEmpty(BankAccount_BankNameBox.Text) && EmptyStringIfNull.IsNumber(BankAccount_BankCodeBox.Text))
            {
                txbBancAccountIban.Text = "DE" + (98 - ((62 * ((1 + long.Parse(BankAccount_BankCodeBox.Text) % 97)) +
                    27 * (long.Parse(BankAccount_AccountnumberBox.Text) % 97)) % 97)).ToString("D2");
                txbBancAccountIban.Text += long.Parse(BankAccount_BankCodeBox.Text).ToString("00000000").Substring(0, 4);
                txbBancAccountIban.Text += long.Parse(BankAccount_BankCodeBox.Text).ToString("00000000").Substring(4, 4);
                txbBancAccountIban.Text += long.Parse(BankAccount_AccountnumberBox.Text).ToString("0000000000").Substring(0, 4);
                txbBancAccountIban.Text += long.Parse(BankAccount_AccountnumberBox.Text).ToString("0000000000").Substring(4, 4);
                txbBancAccountIban.Text += long.Parse(BankAccount_AccountnumberBox.Text).ToString("0000000000").Substring(8, 2);
                using (DataClasses1DataContext dataContext = new DataClasses1DataContext())
                {
                    var bicNr = dataContext.BIC_DE.FirstOrDefault(q => q.Bankleitzahl.Contains(BankAccount_BankCodeBox.Text) && (q.Bezeichnung.Contains(BankAccount_BankNameBox.Text) || 
                        q.Kurzbezeichnung.Contains(BankAccount_BankNameBox.Text)));
                    if (bicNr != null)
                    {
                        if (!String.IsNullOrEmpty(bicNr.BIC.ToString()))
                            txbBankAccount_Bic.Text = bicNr.BIC.ToString();
                    }
                }
            }
        }
        #endregion
        //protected string CheckIfAllProduktsHavingPrice(int? locationId)
        //{
        //    string allesHatGutGelaufen = "";
        //    string ProduktId = "";
        //    string CostCenterId = "";
        //    foreach (RadTreeNode node in DienstleistungTreeView.Nodes)
        //    {
        //        if (!String.IsNullOrEmpty(node.Value))
        //        {
        //            string[] splited = node.Value.Split(';');
        //            if (splited.Length == 2)
        //            {
        //                try
        //                {
        //                    DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
        //                    Price newPrice;
        //                    ProduktId = splited[0];
        //                    CostCenterId = splited[1];
        //                    if (!String.IsNullOrEmpty(ProduktId))
        //                    {
        //                        var productId = Int32.Parse(ProduktId);

        //                        KVSCommon.Database.Product newProduct = dbContext.Product.SingleOrDefault(q => q.Id == productId);
        //                        if (locationId == null) //small
        //                        {
        //                            newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == null);
        //                            if (newPrice == null)
        //                            {
        //                                allesHatGutGelaufen += " " + node.Text + " ";
        //                            }
        //                            else
        //                            {
        //                            }
        //                        }
        //                    }
        //                }
        //                catch
        //                {
        //                    return "";
        //                }
        //            }
        //        }
        //    }
        //    return allesHatGutGelaufen;
        //}
        protected void AddAnotherProducts(RegistrationOrder regOrd, int? locationId)
        {
            string ProduktId = "";    
            int itemIndexValue = 0;
            decimal amount = 0;
            decimal authCharge = 0;
            var nodes = this.Request.Form.AllKeys.Where(q => q.Contains("txtItemPrice_"));
            foreach(var node in nodes)
            {
                if (!String.IsNullOrEmpty(node))
                {
                    DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
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
                            if (newPrice!= null && newPrice.AuthorativeCharge.HasValue)
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
                            itemIndexValue= itemIndexValue+1;
                            dbContext.SubmitChanges();                                   
                        }
                    }
                }            
            }
        }
        protected void MakeInvoiceForSmallCustomer(int customerId, RegistrationOrder regOrder)
        {
            try
            {
                DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
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
                newOrder.ExecutionDate = DateTime.Now;
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
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
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
                using (DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())))
                {
                    using (scope = new TransactionScope())
                    {
                        var newAdress = Adress.CreateAdress(street, streetNumber, zipcode, city, country, dbContext);
                        var newInvoice = Invoice.CreateInvoice(dbContext, Int32.Parse(Session["CurrentUserId"].ToString()), invoiceRecipient, newAdress,
                            Int32.Parse(CustomerDropDownList.SelectedValue), txbDiscount.Value, "Einzelrechnung");
                        var myCustomer = dbContext.Customer.FirstOrDefault(q => q.Id == Int32.Parse(CustomerDropDownList.SelectedValue));
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
                           // newInvoiceItem.VAT = myCustomer.VAT;
                            ordItem.Status = (int)OrderItemStatusTypes.Payed;
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
                ErrorLeereTextBoxenLabel.Text = ex.Message;//"Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator " + ex.Message;
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
            newPdfPathAndName = ConfigurationManager.AppSettings["DataPath"] + "/" + Session["CurrentUserId"].ToString() + "/Lieferschein_" + DateTime.Today.Day + "_" + 
                DateTime.Today.Month + "_" + DateTime.Today.Year + "_" + Guid.NewGuid() + ".pdf";
            return newPdfPathAndName;
        }
        private void OpenPrintfile(string myFile)
        {
            string url = ConfigurationManager.AppSettings["BaseUrl"];
            string path = url + "UserData/" + Session["CurrentUserId"].ToString() + "/" + myFile;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Invoice", "<script>openFile('" + path + "');</script>", false);
        }
        protected void Print(Invoice newInvoice, DataClasses1DataContext dbContext)
        {
            using (MemoryStream memS = new MemoryStream())
            {
                InvoiceHelper.CreateAccounts(dbContext, newInvoice);
                newInvoice.Print(dbContext, memS, "");
                dbContext.SubmitChanges();
                string fileName = "Rechnung_"+ newInvoice.InvoiceNumber.Number+"_" + newInvoice.CreateDate.Day + "_" + newInvoice.CreateDate.Month + "_" + newInvoice.CreateDate.Year + ".pdf";
                string serverPath = ConfigurationManager.AppSettings["DataPath"] + "\\UserData";
                if (!Directory.Exists(serverPath)) Directory.CreateDirectory(serverPath);
                if (!Directory.Exists(serverPath + "\\" + Session["CurrentUserId"].ToString())) Directory.CreateDirectory(serverPath + "\\" + Session["CurrentUserId"].ToString());
                serverPath = serverPath + "\\" + Session["CurrentUserId"].ToString();
                File.WriteAllBytes(serverPath + "\\" + fileName, memS.ToArray());
                OpenPrintfile(fileName);
                dbContext.SubmitChanges();
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
            List<Control> allControls = getAllControls();
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
            ProductDropDownList.ClearSelection();
            RegistrationOrderDropDownList.ClearSelection();
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
                        if (box.Enabled == true && (box != Adress_CountryBox))
                        {
                            box.Text = "";
                        }
                    }
                }
            }
        }
        protected void NaechtenAuftragButton_Clicked(object sender, EventArgs e)
        {
            MakeAllControlsEmpty();
            ZulassungOkLabel.Visible = false;
            SubmitChangesErrorLabel.Visible = false;
        }
        protected void CheckFields(List<Control> listOfControls)
        {
            foreach (Control control in listOfControls)
            {
                control.Visible = true;
            }
            FahrzeugLabel.Visible = true;
            HalterLabel.Visible = true;
            HalterdatenLabel.Visible = true;
            KontaktdatenLabel.Visible = true;
        }
        protected void HideAllControls()
        {
            List<Control> controlsToHide = new List<Control>();
            controlsToHide = getAllControls();
            DataClasses1DataContext con = new DataClasses1DataContext();
            var cont = from largCust in con.LargeCustomerRequiredField
                       join reqFiled in con.RequiredField on largCust.RequiredFieldId equals reqFiled.Id
                       join ordTyp in con.OrderType on reqFiled.OrderTypeId equals ordTyp.Id
                       where ordTyp.Name == "Zulassung"
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
        //Gibt die Liste mit alle Controls aus der ASCX Seite zurück
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
            using (DataClasses1DataContext dbContext = new DataClasses1DataContext())
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
                using (DataClasses1DataContext dbContext = new DataClasses1DataContext())
                {
                    var customerId = 0;
                    if (CustomerDropDownList.SelectedValue != string.Empty)
                        customerId = Int32.Parse(CustomerDropDownList.SelectedValue);
                    var checkThisCustomer = dbContext.Customer.SingleOrDefault(q => q.Id == customerId);
                    if (checkThisCustomer != null)
                    {
                        // Kundendaten
                        txbSmallCustomerVat.Text = checkThisCustomer.VAT.ToString();
                        txbSmallCustomerZahlungsziel.Text = checkThisCustomer.TermOfCredit != null ? checkThisCustomer.TermOfCredit.Value.ToString() : "";
                        txbSmallCustomerVorname.Text = checkThisCustomer.Name;
                        txbSmallCustomerNachname.Text = checkThisCustomer.SmallCustomer.Person != null ? checkThisCustomer.SmallCustomer.Person.FirstName:"";
                        txbSmallCustomerTitle.Text = checkThisCustomer.SmallCustomer.Person != null ? checkThisCustomer.SmallCustomer.Person.Title : "";
                        cmbSmallCustomerGender.Text = checkThisCustomer.SmallCustomer.Person != null ? checkThisCustomer.SmallCustomer.Person.Gender : "";
                        txbSmallCustomerStreet.Text = checkThisCustomer.Adress!=null ? checkThisCustomer.Adress.Street : "";
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
                    using (DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())))
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
                            var newSmallCustomer = SmallCustomer.CreateSmallCustomer(txbSmallCustomerVorname.Text, txbSmallCustomerNachname.Text, txbSmallCustomerTitle.Text, 
                                cmbSmallCustomerGender.SelectedValue, txbSmallCustomerStreet.Text, txbSmallCustomerNr.Text, txbSmallCustomerZipCode.Text, cmbSmallCustomerCity.Text,
                                txbSmallCustomerCountry.Text, txbSmallCustomerPhone.Text,
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
        private void AddCacheDependency(HttpContext context, string tempFileName, TimeSpan timeToLive, string fullPath)
        {
            if (context.Cache.Get(tempFileName) == null)
                context.Cache.Insert(tempFileName, fullPath, null, DateTime.Now.Add(timeToLive), TimeSpan.Zero, CacheItemPriority.NotRemovable, RemovedCallback);
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
    }
}