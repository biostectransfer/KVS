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
namespace KVSWebApplication.Auftragseingang
{
    /// <summary>
    ///  Abmeldung Gruoßkunde
    /// </summary>
    public partial class AbmeldungGrosskunde1 : System.Web.UI.UserControl
    {
        List<Control> controls = new List<Control>();
        protected void Page_Load(object sender, EventArgs e)
        {
            AbmeldungGrosskunde auftragsEingang = Page as AbmeldungGrosskunde;
            RadScriptManager script = auftragsEingang.getScriptManager() as RadScriptManager;
            script.RegisterPostBackControl(AddAdressButton);
            LicenceBox1.Enabled = true;
            LicenceBox2.Enabled = true;
            LicenceBox3.Enabled = true;
            //first registration bekommt immer heutige Datum by default
            FirstRegistrationDateBox.SelectedDate = DateTime.Now;
            if (Session["CurrentUserId"] != null)
            {
                if (!String.IsNullOrEmpty(Session["CurrentUserId"].ToString()))
                {
                    CheckUserPermissions();
                }
            }
        }
        protected void CheckUserPermissions()
        {
            List<string> userPermissions = new List<string>();
            userPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(Int32.Parse(Session["CurrentUserId"].ToString())));
            if (userPermissions.Count > 0)
            {
                if (userPermissions.Contains("ABMELDEAUFTRAG_ANLEGEN"))
                {
                    EingangAbmeldungPanel.Enabled = true;
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
                    var bicNr = dataContext.BIC_DE.FirstOrDefault(q => q.Bankleitzahl.Contains(BankAccount_BankCodeBox.Text) && (q.Bezeichnung.Contains(BankAccount_BankNameBox.Text) || q.Kurzbezeichnung.Contains(BankAccount_BankNameBox.Text)));
                    if (bicNr != null)
                    {
                        if (!String.IsNullOrEmpty(bicNr.BIC.ToString()))
                            txbBankAccount_Bic.Text = bicNr.BIC.ToString();
                    }
                }
            }
        }
        protected void CheckUmsatzForSmallCustomer()
        {
            SmallCustomerHistorie.Visible = true;
            DataClasses1DataContext con = new DataClasses1DataContext();
            var newQuery = from ord in con.Order
                           let registration = ord.DeregistrationOrder != null ? ord.DeregistrationOrder.Registration : ord.DeregistrationOrder.Registration
                           where ord.Status == 900
                           select new
                           {
                               OrderId = ord.Id,
                               CustomerId = ord.CustomerId,
                               OrderNumber = ord.Ordernumber,
                           };
            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue))
            {
                try
                {
                    newQuery = newQuery.Where(q => q.CustomerId == Int32.Parse(CustomerDropDownList.SelectedValue));
                }
                catch
                {
                }
            }
            // Allgemein
            string countAuftrag = newQuery.Count().ToString();
            decimal gebuehren = 0;
            decimal umsatz = 0;
            //Amtliche Gebühr
            foreach (var newOrder in newQuery)
            {
                var order = con.Order.SingleOrDefault(q => q.Id == newOrder.OrderId);
                if (order != null)
                {
                    foreach (OrderItem orderItem in order.OrderItem)
                    {
                        if (orderItem.IsAuthorativeCharge && orderItem.Status == 900)
                            gebuehren = gebuehren + orderItem.Amount;
                        else if (!orderItem.IsAuthorativeCharge && orderItem.Status == 900)
                            umsatz = umsatz + orderItem.Amount;
                    }
                }
            }
            SmallCustomerHistorie.Text = "Historie: <br/> Gesamt: " + countAuftrag + " <br/> Umsatz: " + umsatz.ToString("C2") + "<br/> Gebühren: " + gebuehren.ToString("C2");
        }

        private void LoadCustomerProductsInTreeView(int selectedCustomer, int location)
        {
            using (DataClasses1DataContext con = new DataClasses1DataContext())
            {
                var productQuery = from p in con.Price
                                   join prA in con.PriceAccount on p.Id equals prA.PriceId
                                   where (p.Product.OrderType.Name == "Abmeldung" || p.Product.OrderType.Name == "Allgemein")
                                   && p.Location.CustomerId == selectedCustomer
                                   && p.LocationId == location
                                   select new
                                   {
                                       ItemNumber = p.Product.ItemNumber,
                                       Name = p.Product.Name,
                                       Value = p.Product.Id,
                                       Category = p.Product.ProductCategory.Name
                                   };


                var custProds = con.CustomerProduct.Where(q => q.CustomerId == selectedCustomer);
                if (custProds != null)
                {
                    foreach (var product in productQuery)
                    {
                        var myProd = custProds.SingleOrDefault(q => q.ProductId == product.Value);
                        if (myProd != null)
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
                }
            }
        }



        protected void setCarOwnerData()
        {
            using (DataClasses1DataContext con = new DataClasses1DataContext())
            {
                Adress_StreetBox.Text = string.Empty;
                Adress_StreetNumberBox.Text = string.Empty;
                Adress_ZipcodeBox.Text = string.Empty;
                Adress_CityBox.Text = string.Empty;
                Adress_CountryBox.Text = string.Empty;
                CarOwner_NameBox.Text = string.Empty;
                Registration_eVBNumberBox.Text = string.Empty;
                if (!String.IsNullOrEmpty(LocationDropDownList.SelectedValue) && !String.IsNullOrEmpty(CustomerDropDownList.SelectedValue))
                {
                    var customerDetils = con.Location.FirstOrDefault(q => q.Id == Int32.Parse(LocationDropDownList.SelectedValue) && 
                        q.CustomerId == Int32.Parse(CustomerDropDownList.SelectedValue));
                    if (customerDetils != null)
                    {
                        CarOwner_NameBox.Text = customerDetils.LargeCustomer.Customer.Name;
                        Registration_eVBNumberBox.Text = customerDetils.LargeCustomer.Customer.eVB_Number;
                        if (customerDetils.LargeCustomer.Person != null)
                            CarOwner_FirstnameBox.Text = customerDetils.LargeCustomer.Person.Name + " " + customerDetils.LargeCustomer.Person.FirstName;
                        if (customerDetils.Adress != null)
                        {
                            Adress_StreetBox.Text = customerDetils.Adress.Street;
                            Adress_StreetNumberBox.Text = customerDetils.Adress.StreetNumber;
                            Adress_ZipcodeBox.Text = customerDetils.Adress.Zipcode;
                            Adress_CityBox.Text = customerDetils.Adress.City;
                            Adress_CountryBox.Text = customerDetils.Adress.Country;
                        }
                    }
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
                            DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                            Price newPrice;
                            ProduktId = splited[0];
                            CostCenterId = splited[1];
                            if (!String.IsNullOrEmpty(ProduktId))
                            {
                                var productId = Int32.Parse(ProduktId);
                                int costCenterId = 0;
                                KVSCommon.Database.Product newProduct = dbContext.Product.SingleOrDefault(q => q.Id == productId);
                                costCenterId = Int32.Parse(CostCenterId);
                                newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == locationId);
                                if (newPrice == null)
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
            else if (!String.IsNullOrEmpty(CheckIfAllProduktsHavingPrice(locationId)))
            {
                ErrorLeereTextBoxenLabel.Text = "Für " + CheckIfAllProduktsHavingPrice(locationId) + " wurde kein Price gefunden!";
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
                    DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
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
                        newRegistration = newVehicle.Registration1;
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
                    price = findPrice(ProduktId);
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
        // findet alle textboxen und macht die leer ohne die ganze Seite neu zu laden
        protected void MakeAllControlsEmpty()
        {
            List<Control> allControls = getAllControls();
            DateTime? nullDate = null;
            Registration_GeneralInspectionDateBox.SelectedDate = nullDate;
            FirstRegistrationDateBox.SelectedDate = DateTime.Now;
            HSNSearchLabel.Visible = false;
            CustomerDropDownList.ClearSelection();
            ProductAbmDropDownList.ClearSelection();
            LocationDropDownList.ClearSelection();
            CostCenterDropDownList.ClearSelection();
            ZulassungsstelleComboBox.ClearSelection();
            DienstleistungTreeView.Nodes.Clear();
            foreach (Control control in allControls)
            {
                foreach (Control subControl in control.Controls)
                {
                    if (subControl is RadTextBox)
                    {
                        RadTextBox box = subControl as RadTextBox;
                        if (box.Enabled == true)
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
            bool iFound1VisibleBox = false;
            bool carOwnerMin = false;
            int count = 0;
            List<Control> allControls = getAllControls();
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
                try
                {
                    VINBox.Text = VINBox.Text.ToUpper();
                    FahrzeugLabel.Visible = false;
                    DataClasses1DataContext dbContext = new DataClasses1DataContext();
                    var autoQuery = dbContext.Vehicle.Single(q => q.VIN == VINBox.Text);
                    vehicleIdField.Value = autoQuery.Id.ToString();
                    if (autoQuery.Registration1 != null)
                    {
                        string kennzeichen = string.Empty;
                        VINBox.Text = VINBox.Text;
                        Vehicle_VariantBox.Text = autoQuery.Variant;
                        kennzeichen = autoQuery.Registration1.Licencenumber;
                        string[] newKennzeichen = kennzeichen.Split('-');
                        if (newKennzeichen.Length == 3)
                        {
                            LicenceBox1.Text = newKennzeichen[0];
                            LicenceBox2.Text = newKennzeichen[1];
                            LicenceBox3.Text = newKennzeichen[2];
                        }
                        Registration_GeneralInspectionDateBox.SelectedDate = autoQuery.Registration1.GeneralInspectionDate;
                        Vehicle_VariantBox.Text = autoQuery.Variant;
                        HSNAbmBox.Text = autoQuery.HSN;
                        TSNAbmBox.Text = autoQuery.TSN;
                        Vehicle_ColorBox.Text = autoQuery.ColorCode.ToString();
                        RegDocNumBox.Text = autoQuery.Registration1.RegistrationDocumentNumber;
                        EmissionsCodeBox.Text = autoQuery.Registration1.EmissionCode;
                        CarOwner_NameBox.Text = autoQuery.Registration1.CarOwner.Name;
                        Adress_StreetBox.Text = autoQuery.Registration1.CarOwner.Adress.Street;
                        CarOwner_FirstnameBox.Text = autoQuery.Registration1.CarOwner.FirstName;
                        Adress_StreetNumberBox.Text = autoQuery.Registration1.CarOwner.Adress.StreetNumber;
                        Adress_ZipcodeBox.Text = autoQuery.Registration1.CarOwner.Adress.Zipcode;
                        Adress_CityBox.Text = autoQuery.Registration1.CarOwner.Adress.City;
                        Adress_CountryBox.Text = autoQuery.Registration1.CarOwner.Adress.Country;
                        Contact_PhoneBox.Text = autoQuery.Registration1.CarOwner.Contact.Phone;
                        Contact_FaxBox.Text = autoQuery.Registration1.CarOwner.Contact.Fax;
                        Contact_MobilePhoneBox.Text = autoQuery.Registration1.CarOwner.Contact.MobilePhone;
                        Contact_EmailBox.Text = autoQuery.Registration1.CarOwner.Contact.Email;
                        BankAccount_BankNameBox.Text = autoQuery.Registration1.CarOwner.BankAccount.BankName;
                        BankAccount_AccountnumberBox.Text = autoQuery.Registration1.CarOwner.BankAccount.Accountnumber;
                        BankAccount_BankCodeBox.Text = autoQuery.Registration1.CarOwner.BankAccount.BankCode;
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
        protected void CostCenterLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue.ToString()))
            {
                var costCenterQuery = from cost in con.CostCenter
                                      join cust in con.Customer on cost.CustomerId equals cust.Id
                                      where cost.CustomerId == Int32.Parse(CustomerDropDownList.SelectedValue)
                                      select new
                                      {
                                          Name = cost.Name,
                                          Value = cost.Id
                                      };
                e.Result = costCenterQuery;
            }
            else
            {
                var costCenterQuery = from cost in con.CostCenter
                                      join cust in con.Customer on cost.CustomerId equals cust.Id
                                      where cost.CustomerId == 0 //TODO
                                      select new
                                      {
                                          Name = cost.Name,
                                          Value = cost.Id
                                      };
                e.Result = costCenterQuery;
            }
        }
        private Price findPrice(string produktId)
        {
            Price newPrice = null;
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            if (!String.IsNullOrEmpty(LocationDropDownList.SelectedValue.ToString()))
            {
                newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == Int32.Parse(produktId) && q.LocationId == Int32.Parse(LocationDropDownList.SelectedValue));
            }

            if (String.IsNullOrEmpty(this.LocationDropDownList.SelectedValue) || newPrice == null)
            {
                newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == Int32.Parse(produktId) && q.LocationId == null);
            }
            return newPrice;
        }
        #endregion
        protected void ProductAbmDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var selectedCustomer = 0;
            var location = 0;
            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue))
                selectedCustomer = Int32.Parse(CustomerDropDownList.SelectedValue);
            if (!String.IsNullOrEmpty(LocationDropDownList.SelectedValue))
                location = Int32.Parse(LocationDropDownList.SelectedValue);
            IQueryable productQuery1 = null;
            productQuery1 = from p in con.Price
                            join prA in con.PriceAccount on p.Id equals prA.PriceId
                            where (p.Product.OrderType.Name == "Abmeldung" || p.Product.OrderType.Name == "Allgemein")
                            && p.Location.CustomerId == selectedCustomer
                            && p.LocationId == location
                            select new
                            {
                                ItemNumber = p.Product.ItemNumber,
                                Name = p.Product.Name,
                                Value = p.Product.Id,
                                Category = p.Product.ProductCategory.Name
                            };

            if (productQuery1 != null && location != 0 && selectedCustomer != 0)
            {
                LoadCustomerProductsInTreeView(selectedCustomer, location);
            }

            e.Result = productQuery1;


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
                CheckFields(getAllControls());
                SmallCustomerHistorie.Visible = false;
            }
        }
        protected void LocationDropDownIndex_Changed(object sender, EventArgs e)
        {
            ProductAbmDropDownList.DataSource = null;
            ProductAbmDropDownList.DataBind();
            setCarOwnerData();
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
                                DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                                var orderId = deRegOrd.OrderId;
                                Price newPrice;
                                OrderItem newOrderItem1 = null;
                                OrderItem newOrderItem2 = null;
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
                                    var orderToUpdate = dbContext.Order.SingleOrDefault(q => q.Id == orderId);
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
        protected void LocationLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
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
            DataClasses1DataContext con = new DataClasses1DataContext();
            var customerQuery = from cust in con.Customer
                                where cust.Id == cust.LargeCustomer.CustomerId
                                orderby cust.Name
                                select new
                                {
                                    Name = cust.Name,
                                    Value = cust.Id,
                                    Matchcode = cust.MatchCode,
                                    Kundennummer = cust.CustomerNumber
                                };
            e.Result = customerQuery;
        }
        #endregion
        protected void CheckFields(List<Control> listOfControls)
        {
            HideAllControls();
            DataClasses1DataContext con = new DataClasses1DataContext();
            var cont = from largCust in con.LargeCustomerRequiredField
                       join reqFiled in con.RequiredField on largCust.RequiredFieldId equals reqFiled.Id
                       join ordTyp in con.OrderType on reqFiled.OrderTypeId equals ordTyp.Id
                       where largCust.LargeCustomerId == Int32.Parse(CustomerDropDownList.SelectedValue) && ordTyp.Name == "Abmeldung"
                       select reqFiled.Name;
            foreach (var nameCon in cont)
            {
                foreach (Control control in listOfControls)
                {
                    if (control.ID == nameCon)
                    {
                        control.Visible = true;
                        if (control.ID == "Vehicle_VIN" || control.ID == "Vehicle_Variant" || control.ID == "Vehicle_Color" || control.ID == "Registration_Licencenumber" || 
                            control.ID == "RegistrationOrder_PreviousLicencenumber" || control.ID == "Registration_GeneralInspectionDate" || control.ID == "Vehicle_FirstRegistrationDate" ||
                            control.ID == "Vehicle_TSN" || control.ID == "Vehicle_HSN" || control.ID == "Registration_eVBNumber" || control.ID == "Registration_RegistrationDocumentNumber" || 
                            control.ID == "Registration_EmissionCode" || control.ID == "Order_Freitext")
                            FahrzeugLabel.Visible = true;
                        if (control.ID == "CarOwner_Name" || control.ID == "CarOwner_Firstname" || control.ID == "Adress_Street" || control.ID == "Adress_StreetNumber" || 
                            control.ID == "Adress_Zipcode" || control.ID == "Adress_City" || control.ID == "Adress_Country")
                            HalterLabel.Visible = true;
                        if (control.ID == "BankAccount_BankName" || control.ID == "BankAccount_Accountnumber" || control.ID == "BankAccount_BankCode")
                        {
                            HalterdatenLabel.Visible = true;
                            IBANPanel.Visible = true;
                        }
                        if (control.ID == "Contact_Phone" || control.ID == "Contact_Fax" || control.ID == "Contact_MobilePhone" || control.ID == "Contact_Email")
                            KontaktdatenLabel.Visible = true;
                    }
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
        protected void CheckIfButtonShouldBeEnabled()
        {
            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue)
                && !String.IsNullOrEmpty(LocationDropDownList.SelectedValue)
                && !String.IsNullOrEmpty(ProductAbmDropDownList.SelectedValue)
                && !String.IsNullOrEmpty(CostCenterDropDownList.SelectedValue))
                AbmeldenButton.Enabled = true;
            else if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue)
                && !String.IsNullOrEmpty(ProductAbmDropDownList.SelectedValue)) //small without costcenter and location
                AbmeldenButton.Enabled = true;
            else
                AbmeldenButton.Enabled = false;

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
            DataClasses1DataContext con = new DataClasses1DataContext();
            var cont = from largCust in con.LargeCustomerRequiredField
                       join reqFiled in con.RequiredField on largCust.RequiredFieldId equals reqFiled.Id
                       join ordTyp in con.OrderType on reqFiled.OrderTypeId equals ordTyp.Id
                       where ordTyp.Name == "Abmeldung"
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
                        IBANPanel.Visible = false;
                        KontaktdatenLabel.Visible = false;
                    }
                }
            }
        }
        protected void MakeInvoiceForSmallCustomer(int customerId, DeregistrationOrder regOrder)
        {
            try
            {
                DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                var newOrder = dbContext.Order.Single(q => q.CustomerId == customerId && q.Id == regOrder.OrderId);
                smallCustomerOrderHiddenField.Value = regOrder.OrderId.ToString();
                //updating order status
                newOrder.LogDBContext = dbContext;
                newOrder.Status = 600;
                //updating orderitems status                          
                foreach (OrderItem ordItem in newOrder.OrderItem)
                {
                    ordItem.LogDBContext = dbContext;
                    if (ordItem.Status != (int)OrderItemState.Storniert)
                    {
                        ordItem.Status = 600;
                    }
                }
                dbContext.SubmitChanges();
                //updating order und items status one more time to make it abgerechnet
                newOrder.LogDBContext = dbContext;
                newOrder.Status = 900;
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
                InvoiceRecValidator.Enabled = true;
                if (CustomerDropDownList.SelectedIndex == 1) // small
                {
                    ZusatzlicheInfoLabel.Visible = true;
                }
            }
        }
        // Create new Adress in der DatenBank
        protected void OnAddAdressButton_Clicked(object sender, EventArgs e)
        {
            InvoiceRecValidator.Enabled = false;
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
            try
            {
                using (DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())))
                {
                    var newAdress = Adress.CreateAdress(street, streetNumber, zipcode, city, country, dbContext);
                    var newInvoice = Invoice.CreateInvoice(dbContext, Int32.Parse(Session["CurrentUserId"].ToString()), invoiceRecipient, newAdress,
                        Int32.Parse(CustomerDropDownList.SelectedValue), txbDiscount.Value, "Einzelrechnung");
                    //Submiting new Invoice and Adress
                    dbContext.SubmitChanges();
                    var orderQuery = dbContext.Order.SingleOrDefault(q => q.Id == Int32.Parse(smallCustomerOrderHiddenField.Value));
                    foreach (OrderItem ordItem in orderQuery.OrderItem)
                    {
                        ProductName = ordItem.ProductName;
                        Amount = ordItem.Amount;

                        //TODO 
                        //if (!String.IsNullOrEmpty(ordItem.CostCenterId.ToString()) && ordItem.CostCenterId.ToString().Length > 8)
                        //{
                        //    costCenterId = Int32.Parse(ordItem.CostCenterId.ToString());
                        //}
                        itemCount = ordItem.Count;

                        CostCenter costCenter = null;
                        if (ordItem.CostCenterId.HasValue)
                        {
                            costCenter = dbContext.CostCenter.FirstOrDefault(o => o.Id == ordItem.CostCenterId.Value);
                        }

                        InvoiceItem newInvoiceItem = newInvoice.AddInvoiceItem(ProductName, Convert.ToDecimal(Amount), itemCount, ordItem, costCenter, dbContext);
                        ordItem.LogDBContext = dbContext;
                        ordItem.Status = 900;
                        dbContext.SubmitChanges();
                    }
                    dbContext.SubmitChanges();
                    Print(newInvoice, dbContext);
                }
            }
            catch (Exception ex)
            {
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
        protected void Print(Invoice newInvoice, DataClasses1DataContext dbContext)
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
    }
}