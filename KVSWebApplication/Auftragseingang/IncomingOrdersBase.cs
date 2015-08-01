using KVSCommon.Database;
using KVSCommon.Enums;
using KVSCommon.Managers;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace KVSWebApplication.Auftragseingang
{
    /// <summary>
    ///  Incoming Orders base class
    /// </summary>
    public abstract class IncomingOrdersBase : UserControl
    {
        #region Members

        public IncomingOrdersBase()
        {
            BicManager = (IBicManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IBicManager));
            UserManager = (IUserManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IUserManager));
            OrderManager = (IOrderManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IOrderManager));
            PriceManager = (IPriceManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IPriceManager));
            ProductManager = (IProductManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IProductManager));
            LargeCustomerRequiredFieldManager = (ILargeCustomerRequiredFieldManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ILargeCustomerRequiredFieldManager));
            LocationManager = (ILocationManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ILocationManager));
            InvoiceManager = (IInvoiceManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IInvoiceManager));
            InvoiceItemAccountItemManager = (IInvoiceItemAccountItemManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IInvoiceItemAccountItemManager));
            InvoiceItemManager = (IInvoiceItemManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IInvoiceItemManager));
            AdressManager = (IAdressManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAdressManager));
            CostCenterManager = (ICostCenterManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ICostCenterManager));
            CustomerManager = (ICustomerManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ICustomerManager));
            VehicleManager = (IVehicleManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IVehicleManager));
            RegistrationLocationManager = (IRegistrationLocationManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IRegistrationLocationManager));
            RegistrationManager = (IRegistrationManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IRegistrationManager));
            RegistrationOrderTypeManager = (IRegistrationOrderTypeManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IRegistrationOrderTypeManager));
            ContactManager = (IContactManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IContactManager));
            BankAccountManager = (IBankAccountManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IBankAccountManager));
            CarOwnerManager = (ICarOwnerManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ICarOwnerManager));
            RegistrationOrderManager = (IRegistrationOrderManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IRegistrationOrderManager));
            DeregistrationOrderManager = (IDeregistrationOrderManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IDeregistrationOrderManager));
        }

        #region Common

        //TODO protected abstract HiddenField SessionId { get; }
        //protected abstract RadPersistenceManager RadPersistenceManager { get; }

        protected abstract PermissionTypes PagePermission { get; }
        protected abstract OrderTypes OrderType { get; }

        protected List<Control> controls = new List<Control>();
        protected abstract Label CustomerHistoryLabel { get; }
        protected abstract RadTreeView ProductTree { get; }
        protected abstract RadScriptManager RadScriptManager { get; }
        protected abstract RadNumericTextBox Discount { get; }
        protected abstract HiddenField SmallCustomerOrder { get; }
        protected abstract HiddenField VehicleId { get; }
        protected virtual RequiredFieldValidator InvoiceValidator { get { return null; } }
        protected abstract RadWindow RadWindow { get; }
        #endregion

        #region Dates

        protected abstract RadMonthYearPicker Registration_GeneralInspectionDatePicker { get; }
        protected abstract RadDatePicker FirstRegistrationDatePicker { get; }
        protected abstract RadDatePicker RegistrationDatePicker { get; }

        #endregion

        #region DropDowns

        protected abstract RadComboBox CustomerDropDown { get; }
        protected abstract RadComboBox LocationDropDown { get; }
        protected abstract RadComboBox CostCenterDropDown { get; }
        protected abstract RadComboBox AdmissionPointDropDown { get; }
        protected abstract RadComboBox ProductDropDown { get; }
        protected abstract RadComboBox RegistrationOrderDropDown { get; }

        #endregion

        #region TextBoxes

        protected abstract RadTextBox AccountNumberTextBox { get; }
        protected abstract RadTextBox BankCodeTextBox { get; }
        protected abstract RadTextBox BankNameTextBox { get;  }
        protected abstract RadTextBox IBANTextBox { get; }
        protected abstract RadTextBox BICTextBox { get; }
        protected abstract RadTextBox Adress_Street_TextBox { get; }
        protected abstract RadTextBox Adress_StreetNumber_TextBox { get; }
        protected abstract RadTextBox Adress_Zipcode_TextBox { get; }
        protected abstract RadTextBox Adress_City_TextBox { get; }
        protected abstract RadTextBox Adress_Country_TextBox { get; }
        protected abstract RadTextBox CarOwner_Name_TextBox { get; }
        protected abstract RadTextBox CarOwner_FirstName_TextBox { get; }
        protected abstract RadTextBox Registration_eVBNumber_TextBox { get; }
        protected abstract TextBox Street_TextBox { get; }
        protected abstract TextBox StreetNumber_TextBox { get; }
        protected abstract TextBox Zipcode_TextBox { get; }
        protected abstract TextBox City_TextBox { get; }
        protected abstract TextBox Country_TextBox { get; }
        protected abstract TextBox InvoiceRecipient_TextBox { get; }
        protected abstract RadTextBox VIN_TextBox { get; }
        protected abstract RadTextBox HSN_TextBox { get; }
        protected abstract RadTextBox TSN_TextBox { get; }
        protected abstract RadTextBox Variant_TextBox { get; }
        protected abstract RadTextBox Color_TextBox { get; }
        protected abstract RadTextBox Contact_Phone_TextBox { get; }
        protected abstract RadTextBox Contact_Fax_TextBox { get; }
        protected abstract RadTextBox Contact_MobilePhone_TextBox { get; }
        protected abstract RadTextBox Contact_Email_TextBox { get; }
        protected abstract RadTextBox EmissionsCode_TextBox { get; }
        protected abstract RadTextBox RegistrationDocumentNumber_TextBox { get; }
        protected abstract RadTextBox FreeTextBox { get; }
        #endregion

        #region Panels

        protected abstract Panel Panel { get; }
        protected abstract Panel Vehicle_Variant_Panel { get; }
        protected abstract Panel Registration_GeneralInspectionDate_Panel { get; }
        protected abstract Panel CarOwner_Name_Panel { get; }
        protected abstract Panel CarOwner_Firstname_Panel { get; }
        protected abstract Panel Adress_StreetNumber_Panel { get; }
        protected abstract Panel Adress_Street_Panel { get; }
        protected abstract Panel Adress_Zipcode_Panel { get; }
        protected abstract Panel Adress_City_Panel { get; }
        protected abstract Panel Adress_Country_Panel { get; }
        protected abstract Panel Contact_Phone_Panel { get; }
        protected abstract Panel Contact_Fax_Panel { get; }
        protected abstract Panel Contact_MobilePhone_Panel { get; }
        protected abstract Panel Contact_Email_Panel { get; }
        protected abstract Panel BankAccount_BankName_Panel { get; }
        protected abstract Panel BankAccount_Accountnumber_Panel { get; }
        protected abstract Panel BankAccount_BankCode_Panel { get; }
        protected abstract Panel Registration_eVBNumber_Panel { get; }
        protected abstract Panel Vehicle_HSN_Panel { get; }
        protected abstract Panel Vehicle_TSN_Panel { get; }
        protected abstract Panel Vehicle_VIN_Panel { get; }
        protected abstract Panel Registration_Licencenumber_Panel { get; }
        protected abstract Panel RegistrationOrder_PreviousLicencenumber_Panel { get; }
        protected abstract Panel Registration_EmissionCode_Panel { get; }
        protected abstract Panel Registration_RegistrationDocumentNumber_Panel { get; }
        protected abstract Panel Vehicle_FirstRegistrationDate_Panel { get; }
        protected abstract Panel Vehicle_Color_Panel { get; }
        protected abstract Panel IBANPanel_Panel { get; }

        #endregion

        #region Managers

        public IBicManager BicManager { get; set; }
        public IUserManager UserManager { get; set; }
        public IOrderManager OrderManager { get; set; }
        public IPriceManager PriceManager { get; set; }
        public IProductManager ProductManager { get; set; }
        public ILargeCustomerRequiredFieldManager LargeCustomerRequiredFieldManager { get; set; }        
        public ILocationManager LocationManager { get; set; }
        public IInvoiceManager InvoiceManager { get; set; }
        public IInvoiceItemAccountItemManager InvoiceItemAccountItemManager { get; set; }
        public IInvoiceItemManager InvoiceItemManager { get; set; }
        public IAdressManager AdressManager { get; set; }
        public ICostCenterManager CostCenterManager { get; set; }
        public ICustomerManager CustomerManager { get; set; }
        public IVehicleManager VehicleManager { get; set; }
        public IRegistrationLocationManager RegistrationLocationManager { get; set; }
        public IRegistrationManager RegistrationManager { get; set; }
        public IRegistrationOrderTypeManager RegistrationOrderTypeManager { get; set; }
        public IContactManager ContactManager { get; set; }
        public IBankAccountManager BankAccountManager { get; set; }
        public ICarOwnerManager CarOwnerManager { get; set; }
        public IRegistrationOrderManager RegistrationOrderManager { get; set; }
        public IDeregistrationOrderManager DeregistrationOrderManager { get; set; }
        #endregion

        #region Labels

        protected abstract Label FahrzeugCaption { get; }
        protected abstract Label HalterCaption { get; }
        protected abstract Label HalterdatenCaption { get; }
        protected abstract Label KontaktdatenCaption { get; }
        protected abstract Label HSNSearchCaption { get; }
        protected abstract Label ErrorLeereTextBoxenCaption { get; }
        protected abstract Label AdditionalInfoCaption { get; }
        protected abstract Label LocationWindowCaption { get; }
        #endregion

        #endregion

        #region Event Handlers

        protected void genIban_Click(object sender, EventArgs e)
        {
            if (EmptyStringIfNull.IsNumber(AccountNumberTextBox.Text) &&
                !String.IsNullOrEmpty(BankNameTextBox.Text) &&
                EmptyStringIfNull.IsNumber(BankCodeTextBox.Text))
            {
                IBANTextBox.Text = "DE" + (98 - ((62 * ((1 + long.Parse(BankCodeTextBox.Text) % 97)) +
                    27 * (long.Parse(AccountNumberTextBox.Text) % 97)) % 97)).ToString("D2");
                IBANTextBox.Text += long.Parse(BankCodeTextBox.Text).ToString("00000000").Substring(0, 4);
                IBANTextBox.Text += long.Parse(BankCodeTextBox.Text).ToString("00000000").Substring(4, 4);
                IBANTextBox.Text += long.Parse(AccountNumberTextBox.Text).ToString("0000000000").Substring(0, 4);
                IBANTextBox.Text += long.Parse(AccountNumberTextBox.Text).ToString("0000000000").Substring(4, 4);
                IBANTextBox.Text += long.Parse(AccountNumberTextBox.Text).ToString("0000000000").Substring(8, 2);

                if (BICTextBox != null)
                {
                    var bicNr = BicManager.GetBicByCodeAndName(BankCodeTextBox.Text, BankNameTextBox.Text);
                    if (bicNr != null && !String.IsNullOrEmpty(bicNr.BIC))
                    {
                        BICTextBox.Text = bicNr.BIC.ToString();
                    }
                }
            }
        }

        // Create new Adress in der DatenBank
        protected void OnAddAdressButton_Clicked(object sender, EventArgs e)
        {
            if(InvoiceValidator != null)
                InvoiceValidator.Enabled = false;

            var street = Street_TextBox.Text;
            var streetNumber = StreetNumber_TextBox.Text;
            var zipcode = Zipcode_TextBox.Text;
            var city = City_TextBox.Text;
            var country = Country_TextBox.Text;
            var invoiceRecipient = InvoiceRecipient_TextBox.Text;

            try
            {
                var newAdress = AdressManager.CreateAdress(street, streetNumber, zipcode, city, country);
                
                //TODO check or delete
                //var myCustomer = CustomerMa.FirstOrDefault(q => q.Id == Int32.Parse(CustomerDropDown.SelectedValue));

                var newInvoice = InvoiceManager.CreateInvoice(Int32.Parse(Session["CurrentUserId"].ToString()), invoiceRecipient, newAdress,
                    Int32.Parse(CustomerDropDown.SelectedValue), Discount.Value, InvoiceType.Single);

                var orderQuery = OrderManager.GetById(Int32.Parse(SmallCustomerOrder.Value));
                foreach (var ordItem in orderQuery.OrderItem)
                {
                    var productName = ordItem.ProductName;
                    var amount = ordItem.Amount;

                    CostCenter costCenter = null;
                    if (ordItem.CostCenterId.HasValue)
                    {
                        costCenter = CostCenterManager.GetById(ordItem.CostCenterId.Value);
                    }

                    var newInvoiceItem = InvoiceItemManager.AddInvoiceItem(newInvoice, productName, Convert.ToDecimal(amount), ordItem.Count, ordItem, costCenter);
                    ordItem.Status = (int)OrderItemStatusTypes.Payed;
                }

                Print(newInvoice);

                MakeAllControlsEmpty();
            }
            catch (Exception ex)
            {
                ProductTree.Nodes.Clear();
                ErrorLeereTextBoxenCaption.Text = "Fehler: " + ex.Message;
                ErrorLeereTextBoxenCaption.Visible = true;
            }
        }

        protected void ZulassungsstelleDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = RegistrationLocationManager.GetEntities().Select(o => new
            {
                Name = o.RegistrationLocationName,
                Value = o.ID
            }).OrderBy(o => o.Name).ToList();
        }

        protected void LocationLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            if (!String.IsNullOrEmpty(CustomerDropDown.SelectedValue.ToString()))
            {
                e.Result = LocationManager.GetEntities(o => o.CustomerId == Int32.Parse(CustomerDropDown.SelectedValue)).
                                Select(loc => new
                                {
                                    Name = loc.Name,
                                    Value = loc.Id
                                }); ;
            }
            else
            {
                e.Result = LocationManager.GetEntities(o => !o.CustomerId.HasValue).
                                Select(loc => new
                                {
                                    Name = loc.Name,
                                    Value = loc.Id
                                });
            }
        }

        #endregion

        #region Methods

        protected Price FindPrice(int productId)
        {
            int? locationId = null;
            Price newPrice = null;

            if (LocationDropDown != null && !String.IsNullOrEmpty(LocationDropDown.SelectedValue))
            {
                locationId = Int32.Parse(LocationDropDown.SelectedValue);
                newPrice = PriceManager.GetEntities(q => q.ProductId == productId && q.LocationId == locationId).FirstOrDefault();
            }

            if (LocationDropDown == null || 
               (LocationDropDown != null && String.IsNullOrEmpty(this.LocationDropDown.SelectedValue)) || newPrice == null)
            {
                newPrice = PriceManager.GetEntities(q => q.ProductId == productId && q.LocationId == null).FirstOrDefault();
            }

            return newPrice;
        }

        protected void CheckUserPermissions()
        {
            if (UserManager.CheckPermissionsForUser(Session["UserPermissions"], PagePermission))
            {
                Panel.Enabled = true;
            }
        }

        protected void CheckUmsatzForSmallCustomer()
        {
            CustomerHistoryLabel.Visible = true;

            var orders = OrderManager.GetEntities(o => o.Status == (int)OrderStatusTypes.Payed);

            if (!String.IsNullOrEmpty(CustomerDropDown.SelectedValue))
            {
                orders = orders.Where(q => q.CustomerId == Int32.Parse(CustomerDropDown.SelectedValue));
            }

            orders = orders.ToList();

            decimal gebuehren = 0;
            decimal umsatz = 0;
            //Amtliche Gebühr
            foreach (var order in orders)
            {
                foreach (OrderItem orderItem in order.OrderItem)
                {
                    if (orderItem.IsAuthorativeCharge && orderItem.Status == (int)OrderItemStatusTypes.Payed)
                        gebuehren = gebuehren + orderItem.Amount;
                    else if (!orderItem.IsAuthorativeCharge && orderItem.Status == (int)OrderItemStatusTypes.Payed)
                        umsatz = umsatz + orderItem.Amount;
                }
            }

            CustomerHistoryLabel.Text = String.Format("Historie: <br/> Gesamt: {0} <br/> Umsatz: {1}<br/> Gebühren: {2}",
                orders.Count(), umsatz.ToString("C2"), gebuehren.ToString("C2"));
        }

        protected string CheckIfAllProduktsHavingPrice(int? locationId)
        {
            string result = "";

            foreach (RadTreeNode node in ProductTree.Nodes)
            {
                if (!String.IsNullOrEmpty(node.Value))
                {
                    string[] splited = node.Value.Split(';');
                    if (splited.Length == 2)
                    {
                        try
                        {
                            if (!String.IsNullOrEmpty(splited[0]))
                            {
                                var product = ProductManager.GetById(Int32.Parse(splited[0]));
                                if (!locationId.HasValue) //small customer
                                {
                                    var price = PriceManager.GetEntities(q => q.ProductId == product.Id && q.LocationId == null).FirstOrDefault();
                                    if (price == null)
                                    {
                                        result += " " + node.Text + " ";
                                    }
                                }
                                else //large customer
                                {
                                    var newPrice = PriceManager.GetEntities(q => q.ProductId == product.Id && q.LocationId == locationId.Value).FirstOrDefault();

                                    if (newPrice == null)
                                        newPrice = PriceManager.GetEntities(q => q.ProductId == product.Id && q.LocationId == null).FirstOrDefault();

                                    if (newPrice == null)
                                    {
                                        result += " " + node.Text + " ";
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            return "";
                        }
                    }
                }
            }
            return result;
        }

        protected List<Control> GetAllControls()
        {
            if (controls.Count == 0)
            {
                controls.Add(Vehicle_Variant_Panel);
                controls.Add(Registration_GeneralInspectionDate_Panel);
                controls.Add(CarOwner_Name_Panel);
                controls.Add(CarOwner_Firstname_Panel);
                controls.Add(Adress_StreetNumber_Panel);
                controls.Add(Adress_Street_Panel);
                controls.Add(Adress_Zipcode_Panel);
                controls.Add(Adress_City_Panel);
                controls.Add(Adress_Country_Panel);
                controls.Add(Contact_Phone_Panel);
                controls.Add(Contact_Fax_Panel);
                controls.Add(Contact_MobilePhone_Panel);
                controls.Add(Contact_Email_Panel);
                controls.Add(BankAccount_BankName_Panel);
                controls.Add(BankAccount_Accountnumber_Panel);
                controls.Add(BankAccount_BankCode_Panel);
                controls.Add(Registration_eVBNumber_Panel);
                controls.Add(Vehicle_HSN_Panel);
                controls.Add(Vehicle_TSN_Panel);
                controls.Add(Vehicle_VIN_Panel);
                controls.Add(Registration_Licencenumber_Panel);
                controls.Add(RegistrationOrder_PreviousLicencenumber_Panel);
                controls.Add(Registration_EmissionCode_Panel);
                controls.Add(Registration_RegistrationDocumentNumber_Panel);
                controls.Add(Vehicle_FirstRegistrationDate_Panel);
                controls.Add(Vehicle_Color_Panel);
                controls.Add(IBANPanel_Panel);
            }
            return controls;
        }

        protected List<string> HideAllControls()
        {
            var controlsToHide = GetAllControls();

            var fields = LargeCustomerRequiredFieldManager.GetEntities(o => o.RequiredField.OrderTypeId == (int)OrderType).
                Select(o => o.RequiredField.Name).ToList();

            foreach (var field in fields)
            {
                foreach (var control in controlsToHide)
                {
                    if (control.ID == field)
                    {
                        control.Visible = false;
                        FahrzeugCaption.Visible = false;
                        HalterCaption.Visible = false;
                        HalterdatenCaption.Visible = false;
                        KontaktdatenCaption.Visible = false;
                        IBANPanel_Panel.Visible = false;
                    }
                }
            }

            return fields;
        }
        
        protected void ShowControls()
        {
            foreach (Control control in GetAllControls())
            {
                control.Visible = true;
            }

            FahrzeugCaption.Visible = true;
            HalterCaption.Visible = true;
            HalterdatenCaption.Visible = true;
            KontaktdatenCaption.Visible = true;
            IBANPanel_Panel.Visible = true;
        }

        protected void CheckFields()
        {
            var fields = HideAllControls();

            foreach (var field in fields)
            {
                foreach (var control in GetAllControls())
                {
                    if (control.ID == field)
                    {
                        control.Visible = true;
                        if (control.ID == "Vehicle_VIN" || control.ID == "Vehicle_Variant" || control.ID == "Vehicle_Color" || control.ID == "Registration_Licencenumber" ||
                            control.ID == "RegistrationOrder_PreviousLicencenumber" || control.ID == "Registration_GeneralInspectionDate" || control.ID == "Vehicle_FirstRegistrationDate" ||
                            control.ID == "Vehicle_TSN" || control.ID == "Vehicle_HSN" || control.ID == "Registration_eVBNumber" ||
                            control.ID == "Registration_RegistrationDocumentNumber" || control.ID == "Registration_EmissionCode" || control.ID == "Order_Freitext")
                            FahrzeugCaption.Visible = true;

                        if (control.ID == "CarOwner_Name" || control.ID == "CarOwner_Firstname" || control.ID == "Adress_Street" || control.ID == "Adress_StreetNumber" ||
                            control.ID == "Adress_Zipcode" || control.ID == "Adress_City" || control.ID == "Adress_Country")
                            HalterCaption.Visible = true;

                        if (control.ID == "BankAccount_BankName" || control.ID == "BankAccount_Accountnumber" || control.ID == "BankAccount_BankCode")
                        {
                            HalterdatenCaption.Visible = true;
                            IBANPanel_Panel.Visible = true;
                        }

                        if (control.ID == "Contact_Phone" || control.ID == "Contact_Fax" || control.ID == "Contact_MobilePhone" || control.ID == "Contact_Email")
                            KontaktdatenCaption.Visible = true;
                    }
                }
            }
        }

        protected void SetCarOwnerData()
        {
            Adress_Street_TextBox.Text = String.Empty;
            Adress_StreetNumber_TextBox.Text = String.Empty;
            Adress_Zipcode_TextBox.Text = String.Empty;
            Adress_City_TextBox.Text = String.Empty;
            Adress_Country_TextBox.Text = String.Empty;
            CarOwner_Name_TextBox.Text = String.Empty;
            Registration_eVBNumber_TextBox.Text = String.Empty;

            if (!String.IsNullOrEmpty(LocationDropDown.SelectedValue) && !String.IsNullOrEmpty(CustomerDropDown.SelectedValue))
            {
                var location = LocationManager.GetEntities(q => q.Id == Int32.Parse(LocationDropDown.SelectedValue) &&
                    q.CustomerId == Int32.Parse(CustomerDropDown.SelectedValue)).FirstOrDefault();

                if (location != null)
                {
                    CarOwner_Name_TextBox.Text = location.LargeCustomer.Customer.Name;
                    Registration_eVBNumber_TextBox.Text = location.LargeCustomer.Customer.eVB_Number;

                    if (location.LargeCustomer.Person != null)
                        CarOwner_FirstName_TextBox.Text = location.LargeCustomer.Person.Name + " " + location.LargeCustomer.Person.FirstName;

                    if (location.Adress != null)
                    {
                        Adress_Street_TextBox.Text = location.Adress.Street;
                        Adress_StreetNumber_TextBox.Text = location.Adress.StreetNumber;
                        Adress_Zipcode_TextBox.Text = location.Adress.Zipcode;
                        Adress_City_TextBox.Text = location.Adress.City;
                        Adress_Country_TextBox.Text = location.Adress.Country;
                    }
                }
            }
        }

        protected void MakeAllControlsEmpty()
        {
            var allControls = GetAllControls();

            Registration_GeneralInspectionDatePicker.SelectedDate = (DateTime?)null;
            FirstRegistrationDatePicker.SelectedDate = DateTime.Now;

            ProductTree.Nodes.Clear();
            CustomerDropDown.ClearSelection();
            ProductDropDown.ClearSelection();

            if(LocationDropDown != null)
                LocationDropDown.ClearSelection();

            if(CostCenterDropDown != null)
                CostCenterDropDown.ClearSelection();
            
            if(RegistrationOrderDropDown != null)
                RegistrationOrderDropDown.ClearSelection();

            AdmissionPointDropDown.ClearSelection();

            HSNSearchCaption.Visible = false;

            foreach (Control control in allControls)
            {
                foreach (Control subControl in control.Controls)
                {
                    if (subControl is RadTextBox)
                    {
                        var box = subControl as RadTextBox;
                        if (box.Enabled == true && box != Adress_Country_TextBox)
                        {
                            box.Text = "";
                        }
                    }
                }
            }

            MakeSpecialControlsEmpty();
        }

        protected virtual void MakeSpecialControlsEmpty()
        {

        }
        
        // find all showed checkboxes and check are they empty or not
        protected virtual bool CheckIfBoxenEmpty()
        {
            bool result = false;
            return result;
        }

        protected virtual bool AddAnotherProducts(RegistrationOrder registrationOrder, int? locatinId)
        {
            bool result = false;
            return result;
        }

        protected object GetAllSmallCustomers()
        {
            return CustomerManager.GetEntities(o => o.SmallCustomer != null).
                Select(cust => new
                {
                    Name = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name,
                    Value = cust.Id,
                    Matchcode = cust.MatchCode,
                    Kundennummer = cust.CustomerNumber
                }).ToList();
        }

        protected object GetAllLargeCustomers()
        {
            return CustomerManager.GetEntities(o => o.LargeCustomer != null).
                Select(cust => new
                {
                    Name = cust.Name,
                    Value = cust.Id,
                    Matchcode = cust.MatchCode,
                    Kundennummer = cust.CustomerNumber
                }).OrderBy(o => o.Name).ToList();
        }

        protected object GetCostCenters()
        {
            if (!String.IsNullOrEmpty(CustomerDropDown.SelectedValue.ToString()))
            {
                return CostCenterManager.GetEntities(o => o.CustomerId == Int32.Parse(CustomerDropDown.SelectedValue)).
                                      Select(cost => new
                                      {
                                          Name = cost.Name,
                                          Value = cost.Id
                                      }).OrderBy(o => o.Name).ToList();
            }
            else
            {
                return CostCenterManager.GetEntities().
                                      Select(cost => new
                                      {
                                          Name = cost.Name,
                                          Value = cost.Id
                                      }).OrderBy(o => o.Name).ToList();
            }
        }

        protected Vehicle GetVehicle()
        {
            Vehicle result = null;
            DateTime? FirstRegistrationDate = null;
            int? color = null;

            if (!String.IsNullOrEmpty(FirstRegistrationDatePicker.SelectedDate.ToString()))
                FirstRegistrationDate = FirstRegistrationDatePicker.SelectedDate;

            if (!String.IsNullOrEmpty(Color_TextBox.Text))
                color = Convert.ToInt32(Color_TextBox.Text);

            if (!String.IsNullOrEmpty(VehicleId.Value)) //falls auto gefunden wurde
            {
                result = VehicleManager.GetById(Int32.Parse(VehicleId.Value));
            }
            else // neues Auto muss angelegt werden
            {
                result = VehicleManager.CreateVehicle(VIN_TextBox.Text, HSN_TextBox.Text, TSN_TextBox.Text, Variant_TextBox.Text,
                    FirstRegistrationDate, color);
            }

            return result;
        }

        protected CarOwner GetCarOwner()
        {
            var newAdress = AdressManager.CreateAdress(Adress_Street_TextBox.Text, Adress_StreetNumber_TextBox.Text, Adress_Zipcode_TextBox.Text,
                                    Adress_City_TextBox.Text, Adress_Country_TextBox.Text);
            // another logic after new/existing Vehicle
            var newContact = ContactManager.CreateContact(Contact_Phone_TextBox.Text, Contact_Fax_TextBox.Text, Contact_MobilePhone_TextBox.Text, Contact_Email_TextBox.Text);
            var newBankAccount = BankAccountManager.CreateBankAccount(BankNameTextBox.Text, AccountNumberTextBox.Text,
                BankCodeTextBox.Text, IBANTextBox.Text, BICTextBox.Text);

            return CarOwnerManager.CreateCarOwner(CarOwner_Name_TextBox.Text, CarOwner_FirstName_TextBox.Text, newBankAccount, newContact, newAdress);
        }

        protected Registration CreateRegistration(CarOwner newCarOwner, Vehicle newVehicle, string licenceNumber)
        {
            var newZulassungsDatum = DateTime.Now;
            if (RegistrationDatePicker.SelectedDate.HasValue)
            {
                newZulassungsDatum = RegistrationDatePicker.SelectedDate.Value;
            }

            return RegistrationManager.CreateRegistration(newCarOwner, newVehicle, licenceNumber, Registration_eVBNumber_TextBox.Text,
                Registration_GeneralInspectionDatePicker.SelectedDate, newZulassungsDatum, RegistrationDocumentNumber_TextBox.Text,
                EmissionsCode_TextBox.Text);
        }

        protected RegistrationOrder CreateRegistrationOrder(RegistrationOrderTypes registrationOrderType, string licenceNumber,
            string oldKennzeichen, Vehicle newVehicle, Registration newRegistration, int? locationId)
        {
            return RegistrationOrderManager.CreateRegistrationOrder(
                    Int32.Parse(CustomerDropDown.SelectedValue), licenceNumber, oldKennzeichen, Registration_eVBNumber_TextBox.Text, newVehicle, newRegistration,
                    registrationOrderType, locationId, Int32.Parse(AdmissionPointDropDown.SelectedValue), FreeTextBox.Text);
        }

        protected void ProcessRegistrationOrderForLargeCustomer(RegistrationOrder newRegistrationOrder, int productId, Price price, CostCenter costCenter,
            Registration newRegistration, Vehicle newVehicle, int? locationId, RegistrationOrderTypes registrationOrderType, string senderId)
        {
            var newOrderItem1 = OrderManager.AddOrderItem(newRegistrationOrder.Order, productId, price.Amount, 1, costCenter, null, false);
            if (price.AuthorativeCharge.HasValue)
            {
                var newOrderItem2 = OrderManager.AddOrderItem(newRegistrationOrder.Order, productId, price.AuthorativeCharge.Value, 1, costCenter,
                    newOrderItem1.Id, true);
            }

            if (ProductTree.Nodes.Count > 1)
            {
                AddAnotherProducts(newRegistrationOrder, locationId);
            }

            if (registrationOrderType == RegistrationOrderTypes.NewAdmission || String.IsNullOrEmpty(VehicleId.Value)) //update CurrentRegistration Id
            {
                newVehicle.CurrentRegistrationId = newRegistration.Id;
                VehicleManager.SaveChanges();
            }

            if (senderId != "rbtSameOrder")
                MakeAllControlsEmpty();

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "finishedMessage", "alert('Auftrag wurde erfolgreich angelegt.');", true);
        }

        protected void ProcessRegistrationOrderForSmallCustomer(RegistrationOrder newRegistrationOrder, Registration newRegistration, Vehicle newVehicle, 
            RegistrationOrderTypes registrationOrderType, bool generateInvoice)
        {
            AddAnotherProducts(newRegistrationOrder, null);

            if (registrationOrderType == RegistrationOrderTypes.NewAdmission || String.IsNullOrEmpty(VehicleId.Value)) //update CurrentRegistration Id
            {
                newVehicle.CurrentRegistrationId = newRegistration.Id;
                VehicleManager.SaveChanges();
            }

            if (generateInvoice)
            {
                MakeInvoiceForSmallCustomer(Int32.Parse(CustomerDropDown.SelectedValue), newRegistrationOrder.OrderNumber);
            }
            else
            {
                MakeAllControlsEmpty();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "finishedMessage", "alert('Auftrag wurde erfolgreich angelegt.');", true);
            }
        }
        
        protected void MakeInvoiceForSmallCustomer(int customerId, int orderNumber)
        {
            try
            {
                var newOrder = OrderManager.GetEntities(q => q.CustomerId == customerId && q.OrderNumber == orderNumber).Single();
                SmallCustomerOrder.Value = orderNumber.ToString();

                //updating orderitems status                          
                foreach (OrderItem ordItem in newOrder.OrderItem)
                {
                    if (ordItem.Status != (int)OrderItemStatusTypes.Cancelled)
                    {
                        ordItem.Status = (int)OrderItemStatusTypes.Closed;
                    }
                }

                newOrder.Status = (int)OrderStatusTypes.Closed;
                newOrder.ExecutionDate = DateTime.Now;
                newOrder.Status = (int)OrderStatusTypes.Payed;
                OrderManager.SaveChanges();

                //opening window for adress
                string script = "function f(){$find(\"" + RadWindow.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
                SetValuesForAdressWindow();
            }
            catch (Exception ex)
            {
                ErrorLeereTextBoxenCaption.Text = "Error: " + ex.Message;
                ErrorLeereTextBoxenCaption.Visible = true;
            }
        }
        
        // getting adress from small customer
        protected void SetValuesForAdressWindow()
        {
            var location = AdressManager.GetEntities(o => o.Invoice.Count != 0 &&
                o.Customer.Any(q => q.Id == Int32.Parse(CustomerDropDown.SelectedValue))).FirstOrDefault();

            if (location != null)
            {
                Street_TextBox.Text = location.Street;
                StreetNumber_TextBox.Text = location.StreetNumber;
                Zipcode_TextBox.Text = location.Zipcode;
                City_TextBox.Text = location.City;
                Country_TextBox.Text = location.Country;
                LocationWindowCaption.Text = "Fügen Sie bitte die Adresse für " + CustomerDropDown.Text + " hinzu";
                AdditionalInfoCaption.Visible = false;
                if (CustomerDropDown.SelectedIndex == 1) // small
                {
                    AdditionalInfoCaption.Visible = true;
                }
            }
        }

        protected bool CheckRegistrationFields()
        {
            var boxenEmpty = CheckIfBoxenEmpty(); //gibt es leer boxen, die angezeigt sind

            if (boxenEmpty)
            {
                if (ProductTree.Nodes.Count == 0)
                {
                    ErrorLeereTextBoxenCaption.Text = "Bitte Dienstleistung hinzufügen!";
                    ErrorLeereTextBoxenCaption.Visible = true;
                }
                else if (CostCenterDropDown != null && String.IsNullOrEmpty(CostCenterDropDown.SelectedValue))
                {
                    ErrorLeereTextBoxenCaption.Text = "Bitte tragen Sie die Kostenstelle ein!";
                    ErrorLeereTextBoxenCaption.Visible = true;
                }
                else if (LocationDropDown != null && String.IsNullOrEmpty(LocationDropDown.SelectedValue))
                {
                    ErrorLeereTextBoxenCaption.Text = "Bitte wählen Sie der Standort aus!";
                    ErrorLeereTextBoxenCaption.Visible = true;
                }
                else if (AdmissionPointDropDown != null && String.IsNullOrEmpty(AdmissionPointDropDown.SelectedValue))
                {
                    ErrorLeereTextBoxenCaption.Text = "Bitte wählen Sie die Zulassungsstelle aus!";
                    ErrorLeereTextBoxenCaption.Visible = true;
                }
                else
                {
                    ErrorLeereTextBoxenCaption.Text = "Bitte Pflichtfelder überprüfen!";
                    ErrorLeereTextBoxenCaption.Visible = true;
                }
            }

            return !boxenEmpty;
        }

        #endregion

        #region Print

        protected void Print(Invoice newInvoice)
        {
            using (var stream = new MemoryStream())
            {
                InvoiceItemAccountItemManager.CreateAccounts(newInvoice);
                InvoiceManager.Print(newInvoice, stream, "", Convert.ToString(ConfigurationManager.AppSettings["DefaultAccountNumber"]));

                string fileName = "Rechnung_" + newInvoice.InvoiceNumber.Number + "_" + 
                    newInvoice.CreateDate.Day + "_" + newInvoice.CreateDate.Month + "_" + newInvoice.CreateDate.Year + ".pdf";
                string serverPath = ConfigurationManager.AppSettings["DataPath"] + "\\UserData";

                if (!Directory.Exists(serverPath))
                    Directory.CreateDirectory(serverPath);

                if (!Directory.Exists(serverPath + "\\" + Session["CurrentUserId"].ToString()))
                    Directory.CreateDirectory(serverPath + "\\" + Session["CurrentUserId"].ToString());

                serverPath = serverPath + "\\" + Session["CurrentUserId"].ToString();
                File.WriteAllBytes(serverPath + "\\" + fileName, stream.ToArray());
                OpenPrintfile(fileName);
            }
        }
        
        #endregion

        #region Private Methods

        private void OpenPrintfile(string myFile)
        {
            string url = ConfigurationManager.AppSettings["BaseUrl"];
            string path = url + "UserData/" + Session["CurrentUserId"].ToString() + "/" + myFile;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Invoice", "<script>openFile('" + path + "');</script>", false);
        }

        #endregion
    }
}