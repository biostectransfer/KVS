using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Configuration;
using System.Transactions;
namespace KVSWebApplication.Abrechnung
{
    public partial class AbrechnungSave : System.Web.UI.UserControl
    {
        List<Guid> locations = new List<Guid>(); 
        Abrechnung abr;
        protected void Page_Load(object sender, EventArgs e)
        {
            abr = Page as Abrechnung;
            RadScriptManager script = abr.getScriptManager() as RadScriptManager;
            script.RegisterPostBackControl(btnPreviewInvoice);
            if (!Page.IsPostBack)
            {
                Session["currentLocationIndex"] = 0;
            }
        }
        #region Methods
        #endregion
        #region Index Changed
        /// <summary>
        /// Event fuer die Kundenauswahl aenderung (Kundentyp)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Customer_Selected(object sender, EventArgs e)
        {
            CustomerDropDownList.Enabled = true;
            this.CustomerDropDownList.DataBind();
            if (RadComboBoxCustomer.SelectedValue == "1") // small
            {
                RechnungsTypComboBox.Enabled = false;
                StandortDropDown.Enabled = false;
            }
            else //large
            {
                RechnungsTypComboBox.Enabled = true;
                StandortDropDown.Enabled = true;
            }                                 
        }
        /// <summary>
        /// Event fuer die Kundenauswahl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CustomerChanged_Selected(object sender, EventArgs e)
        {
            //falls large customer - Rechnungstypauswahl true
            if (RadComboBoxCustomer.SelectedValue == "2") //large
            {
                RechnungsTypComboBox.Enabled = true;
                StandortDropDown.Enabled = true;
                StandortDropDown.DataBind();
                this.RadGridAbrechnung.DataBind();
            }
            if (RadComboBoxCustomer.SelectedValue == "1") // small
            {
                AllLocationsCheckBox.Enabled = false;
                StandortDropDown.Enabled = false;
                RechnungsTypComboBox.Enabled = false;
           
                this.RadGridAbrechnung.DataBind();
            }
            StandortDropDown.DataBind();      
        }
        /// <summary>
        /// Event fuer die Auswahl der Rechnungstypen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RechnungsTyp_Selected(object sender, EventArgs e)
        {
            if (RechnungsTypComboBox.SelectedValue != "Einzel")
            {
                AllLocationsCheckBox.Enabled = true;
            }
            else
            {
                AllLocationsCheckBox.Enabled = false;
            }
            RadGridAbrechnung.DataBind();
        }
        #endregion
        #region Linq Data Sources
        /// <summary>
        /// Datasource fuer die Kundenauswahl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            if (RadComboBoxCustomer.SelectedValue == "1") //Small Customers
            {
                var customerQuery = from cust in con.Customer
                                    where cust.Id == cust.SmallCustomer.CustomerId
                                    select new
                                    {   Name = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name,
                                        Value = cust.Id, Matchcode = cust.MatchCode, Kundennummer = cust.CustomerNumber };
                e.Result = customerQuery;
            }
            else if (RadComboBoxCustomer.SelectedValue == "2") //Large Customers
            {
                var customerQuery = from cust in con.Customer
                                    where cust.Id == cust.LargeCustomer.CustomerId
                                    select new { Name = cust.Name, Value = cust.Id, Matchcode = cust.MatchCode, Kundennummer = cust.CustomerNumber };
                e.Result = customerQuery;
            }             
        }
       /// <summary>
       /// Datasource fuer die Abrechnungsgrid
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        protected void AbrechnungLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            if (CustomerDropDownList.SelectedValue == string.Empty)
                CustomerDropDownList.SelectedValue = Guid.Empty.ToString();
            if (String.IsNullOrEmpty(StandortDropDown.SelectedValue.ToString()))
            {
                StandortDropDown.DataBind();
            }
            //select all values for small customers
            if (CustomerDropDownList.SelectedValue != null && RadComboBoxCustomer.SelectedValue == "1" && CustomerDropDownList.SelectedValue != "")
            {
                Guid customerId = new Guid(CustomerDropDownList.SelectedValue);
                DataClasses1DataContext con = new DataClasses1DataContext();
                var query = from cust in con.Customer
                            join ord in con.Order on cust.Id equals ord.CustomerId
                            join orditm in con.OrderItem on ord.Id equals orditm.OrderId
                            join orditmsts in con.OrderItemStatus on orditm.Status equals orditmsts.Id
                            join lrcust in con.SmallCustomer on cust.Id equals lrcust.CustomerId
                            where cust.Id == customerId && orditm.Status == 600 && ord.Status == 600
                            orderby ord.Ordernumber descending
                            select new
                            {
                                OrderId = ord.Id,
                                OrderItemId = orditm.Id,
                                CostCenterId = orditm.CostCenterId,
                                Location = "",
                                ItemCount = orditm.Count,
                                Amount = orditm.Amount,
                                OrderNumber = ord.Ordernumber,
                                ProductName = orditm.ProductName,
                                ItemStatus = orditmsts.Name,
                                ExecutionDate = ord.ExecutionDate,
                                OrderLocation = ord.LocationId,
                                OrderDate = ord.ExecutionDate
                            };           
                e.Result = query;
            }
            //select all values for large customers
            else if (CustomerDropDownList.SelectedValue != null && CustomerDropDownList.SelectedValue != "" && RadComboBoxCustomer.SelectedValue == "2")
            {
                DataClasses1DataContext con = new DataClasses1DataContext();
                Guid customerId = new Guid(CustomerDropDownList.SelectedValue);
                 if (RechnungsTypComboBox.SelectedValue != "Einzel")
                {
                    RadGridAbrechnung.AllowMultiRowSelection = true;
                }
                if (RechnungsTypComboBox.SelectedValue == "Einzel")
                {
                    var query = from cust in con.Customer
                                join ord in con.Order on cust.Id equals ord.CustomerId
                                join orditm in con.OrderItem on ord.Id equals orditm.OrderId
                                join orditmsts in con.OrderItemStatus on orditm.Status equals orditmsts.Id
                                join lrcust in con.LargeCustomer on cust.Id equals lrcust.CustomerId
                                where cust.Id == customerId && (ord.Status == 600 || ord.Status == 700) && orditm.Status == 600
                                orderby ord.Ordernumber descending
                                select new
                                {
                                    OrderId = ord.Id,
                                    OrderItemId = orditm.Id,
                                    Location = ord.Location.Name,
                                    CostCenterId = orditm.CostCenterId,
                                    CostCenterName = orditm.CostCenter.Name,
                                    Amount = orditm.Amount,
                                    ItemCount = orditm.Count,
                                    OrderNumber = ord.Ordernumber,
                                    ProductName = orditm.ProductName,
                                    ItemStatus = orditmsts.Name,
                                    ExecutionDate = ord.ExecutionDate,
                                    OrderLocation = ord.LocationId,
                                    OrderDate = ord.ExecutionDate
                                };
                    if (AllLocationsCheckBox.Checked == false)
                    {
                        if (!String.IsNullOrEmpty(StandortDropDown.SelectedValue.ToString()))
                        {
                            query = query.Where(q => q.OrderLocation == new Guid(StandortDropDown.SelectedValue));
                        }
                    }
                    RadGridAbrechnung.AllowMultiRowSelection = false;
                    e.Result = query;
                }
                else if (RechnungsTypComboBox.SelectedValue == "Sammel")
                {
                    var query = from cust in con.Customer
                                join ord in con.Order on cust.Id equals ord.CustomerId
                                join orditm in con.OrderItem on ord.Id equals orditm.OrderId
                                join orditmsts in con.OrderItemStatus on orditm.Status equals orditmsts.Id
                                join lrcust in con.LargeCustomer on cust.Id equals lrcust.CustomerId
                                where cust.Id == customerId && (ord.Status == 600 || ord.Status == 700) && orditm.Status == 600
                                orderby ord.Ordernumber descending
                                select new
                                {
                                    OrderId = ord.Id,
                                    OrderItemId = orditm.Id,
                                    Location = ord.Location.Name,
                                    CostCenterId = orditm.CostCenterId,
                                    CostCenterName = orditm.CostCenter.Name,
                                    Amount = orditm.Amount,
                                    ItemCount = orditm.Count,
                                    OrderNumber = ord.Ordernumber,
                                    ProductName = orditm.ProductName,
                                    ItemStatus = orditmsts.Name,
                                    ExecutionDate = ord.ExecutionDate,
                                    OrderLocation = ord.LocationId,
                                    OrderDate = ord.ExecutionDate
                                };
                    if (AllLocationsCheckBox.Checked == false)
                    {
                        if (!String.IsNullOrEmpty(StandortDropDown.SelectedValue.ToString()))
                        {
                            query = query.Where(q => q.OrderLocation == new Guid(StandortDropDown.SelectedValue));
                        }
                    }
                    e.Result = query;
                }
                else if (RechnungsTypComboBox.SelectedValue == "Woche")
                {
                    DayOfWeek weekStart = DayOfWeek.Monday;
                    DateTime startingDate = DateTime.Today;
                    while (startingDate.DayOfWeek != weekStart)
                        startingDate = startingDate.AddDays(-1);
                    DateTime endDate = startingDate.AddDays(7);
                    var query = from cust in con.Customer
                                join ord in con.Order on cust.Id equals ord.CustomerId
                                join orditm in con.OrderItem on ord.Id equals orditm.OrderId
                                join orditmsts in con.OrderItemStatus on orditm.Status equals orditmsts.Id
                                join lrcust in con.LargeCustomer on cust.Id equals lrcust.CustomerId
                                where 
                                cust.Id == customerId && (ord.Status == 600 || ord.Status == 700) && orditm.Status == 600
                                && ord.ExecutionDate.Value > startingDate && ord.ExecutionDate < endDate
                                orderby ord.Ordernumber descending
                                select new
                                {
                                    OrderId = ord.Id,
                                    OrderItemId = orditm.Id,
                                    Location = ord.Location.Name,
                                    CostCenterId = orditm.CostCenterId,
                                    CostCenterName = orditm.CostCenter.Name,
                                    Amount = orditm.Amount,
                                    ItemCount = orditm.Count,
                                    OrderNumber = ord.Ordernumber,
                                    ProductName = orditm.ProductName,
                                    ItemStatus = orditmsts.Name,
                                    ExecutionDate = ord.ExecutionDate,
                                    OrderLocation = ord.LocationId,
                                    OrderDate = ord.ExecutionDate
                                };
                    if (AllLocationsCheckBox.Checked == false)
                    {
                        if (!String.IsNullOrEmpty(StandortDropDown.SelectedValue.ToString()))
                        {
                            query = query.Where(q => q.OrderLocation == new Guid(StandortDropDown.SelectedValue));
                        }
                    }
                    e.Result = query;
                }
                else if (RechnungsTypComboBox.SelectedValue == "Monat")
                {
                    var query = from cust in con.Customer
                                join ord in con.Order on cust.Id equals ord.CustomerId
                                join orditm in con.OrderItem on ord.Id equals orditm.OrderId
                                join orditmsts in con.OrderItemStatus on orditm.Status equals orditmsts.Id
                                join lrcust in con.LargeCustomer on cust.Id equals lrcust.CustomerId
                                where cust.Id == customerId && (ord.Status == 600 || ord.Status == 700) && orditm.Status == 600
                                && ord.ExecutionDate.Value.Month == DateTime.Now.Month
                                orderby ord.Ordernumber descending
                                select new
                                {
                                    OrderId = ord.Id,
                                    OrderItemId = orditm.Id,
                                    Location = ord.Location.Name,
                                    CostCenterId = orditm.CostCenterId,
                                    CostCenterName = orditm.CostCenter.Name,
                                    Amount = orditm.Amount,
                                    ItemCount = orditm.Count,
                                    OrderNumber = ord.Ordernumber,
                                    ProductName = orditm.ProductName,
                                    ItemStatus = orditmsts.Name,
                                    ExecutionDate = ord.ExecutionDate,
                                    OrderLocation = ord.LocationId,
                                    OrderDate = ord.ExecutionDate
                                };
                    if (AllLocationsCheckBox.Checked == false)
                    {
                        if (!String.IsNullOrEmpty(StandortDropDown.SelectedValue.ToString()))
                        {
                            query = query.Where(q => q.OrderLocation == new Guid(StandortDropDown.SelectedValue));
                        }
                    }
                    e.Result = query;
                }
                else
                {
                    var query = from cust in con.Customer
                                join ord in con.Order on cust.Id equals ord.CustomerId
                                join orditm in con.OrderItem on ord.Id equals orditm.OrderId
                                join orditmsts in con.OrderItemStatus on orditm.Status equals orditmsts.Id
                                where cust.Id == customerId
                                orderby ord.Ordernumber descending
                                select new
                                {
                                    OrderId = ord.Id,
                                    OrderItemId = orditm.Id,
                                    Location = ord.Location.Name,
                                    CostCenterId = orditm.CostCenterId,
                                    CostCenterName = orditm.CostCenter.Name,
                                    Amount = orditm.Amount,
                                    ItemCount = orditm.Count,
                                    OrderNumber = ord.Ordernumber,
                                    ProductName = orditm.ProductName,
                                    ItemStatus = orditmsts.Name,
                                    ExecutionDate = ord.ExecutionDate,
                                    OrderLocation = ord.LocationId,
                                    OrderDate = ord.ExecutionDate
                                };
                    if (AllLocationsCheckBox.Checked == false)
                    {
                        query = query.Where(q => q.OrderLocation == new Guid(StandortDropDown.SelectedValue));
                    }
                    e.Result = query;
                }
            }       
        }
        #endregion  
        #region Button Clicked
        protected bool SetValuesForAdressWindow()
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            Adress newAdress = null;
            AbrechnungSaveErrorLabel.Visible = false;        
            if (String.IsNullOrEmpty(CustomerDropDownList.SelectedValue))
            {
                AbrechnungSaveErrorLabel.Visible = true;
                AbrechnungSaveErrorLabel.Text = "Bitte wählen Sie einen Kunden aus!";
                return true;
            }
            if (RadGridAbrechnung.SelectedItems.Count <= 0)
            {
                AbrechnungSaveErrorLabel.Visible = true;
                AbrechnungSaveErrorLabel.Text = "Bitte wählen Sie mindestens eine Auftragsposition aus!";
                return true;
            }       
                try
                {
                List<SelectedInvoiceItems> virtualItems = new List<SelectedInvoiceItems>();
                SelectedInvoiceItems currItem = new SelectedInvoiceItems();
                foreach (GridDataItem item in RadGridAbrechnung.SelectedItems)
                {
                        currItem = new SelectedInvoiceItems();
                        currItem.ProductName = item["ProductName"].Text;
                        currItem.Amount = decimal.Parse(item["Amount"].Text, NumberStyles.Currency).ToString();
                        currItem.OrderItemId = new Guid(item["OrderItemId"].Text);
                        if (!String.IsNullOrEmpty(item["CostCenterId"].Text) && item["CostCenterId"].Text.Length > 8)
                        {
                            currItem.CostCenterId = new Guid(item["CostCenterId"].Text);
                        }
                        currItem.ItemCount = Convert.ToInt32(item["ItemCount"].Text);
                        currItem.OrderLocationName=item["Location"].Text;
                        currItem.OrderId = new Guid(item["OrderId"].Text);
                        currItem.OrderLocationId=!EmptyStringIfNull.IsGuid(item["OrderLocation"].Text)? Guid.Empty : new Guid(item["OrderLocation"].Text);
                        virtualItems.Add(currItem);
                }
                var groupedInvoiceItems = virtualItems.GroupBy(q => q.OrderLocationId).ToList();
                if (groupedInvoiceItems.Count > 0 && Session["currentLocationIndex"] != null && EmptyStringIfNull.IsNumber(Session["currentLocationIndex"].ToString()))
                {
                    if(new Guid(groupedInvoiceItems[((int)Session["currentLocationIndex"])].Key.ToString())!=Guid.Empty)
                    newAdress = Invoice.GetInitialInvoiceAdress(new Guid(CustomerDropDownList.SelectedValue), new Guid(groupedInvoiceItems[((int)Session["currentLocationIndex"])].Key.ToString()), dbContext);
                    else
                        newAdress = Invoice.GetInitialInvoiceAdress(new Guid(CustomerDropDownList.SelectedValue), null, dbContext);
                    StreetTextBox.Text = newAdress.Street;
                    StreetNumberTextBox.Text = newAdress.StreetNumber;
                    ZipcodeTextBox.Text = newAdress.Zipcode;
                    CityTextBox.Text = newAdress.City;
                    CountryTextBox.Text = newAdress.Country;
                    FooterTextBox.Text = Invoice.GetDefaultInvoiceText(dbContext, new Guid(CustomerDropDownList.SelectedValue), new Guid(Session["CurrentUserId"].ToString()));
                    InvoiceRecipient.Text = groupedInvoiceItems[((int)Session["currentLocationIndex"])].First().OrderLocationName == "&nbsp;" ? "" : groupedInvoiceItems[((int)Session["currentLocationIndex"])].First().OrderLocationName;
                    AllesIstOkeyLabel.Text = string.Empty;                  
                }
                else
                {
                    throw new Exception("Achtung die Session ist abgelaufen! Bitte loggen Sie sich erneut ein.");
                }            
                }
                catch(Exception ex)
                {
                    WindowManager1.RadAlert("Fehler:" + Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                    AbrechnungSaveErrorLabel.Visible = true;
                    RadGridAbrechnung.DataBind();
                    return true;
                }  
            LocationLabelWindow.Text = "Fügen Sie bitte die Adresse für " + InvoiceRecipient.Text + " hinzu";
            return false;
        }
        protected void AddAdressButton_Click(object sender, EventArgs e)
        {
            if (!SetValuesForAdressWindow())
            {              
                string script = "function f(){$find(\"" + AddAdressRadWindow.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);          
            }
        }
        private string getFullInvoiceName(string selectCase)
        {
            string helpString = string.Empty;
            switch (selectCase)
            {
                case "Einzel":
                    helpString = "Einzelrechnung";
                        break;
                case  "Sammel":
                     helpString = "Sammelrechnung";
                        break;
                case "Monat":
                     helpString = "Monatsrechnung";
                        break;
                case "Woche":
                     helpString = "Wochenrechnung";
                        break;
                case "Frei":
                     helpString = "freie Rechnung";
                        break;
                default: 
                    helpString = "";
                        break;
            }
            return helpString;
        }
        // Create new Adress in der DatenBank
        protected void OnAddAdressButton_Clicked(object sender, EventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext(new Guid(Session["CurrentUserId"].ToString()));
            GenerateInvoice(dbContext, false);
        
        }
        protected void btnPreviewInvoice_Clicked(object sender, EventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext(new Guid(Session["CurrentUserId"].ToString()));
            GenerateInvoice(dbContext, true);

        }
        protected void Cell_Selected(object sender, EventArgs e)
        {
            GridDataItem item = RadGridAbrechnung.SelectedItems[0] as GridDataItem;
            if (RechnungsTypComboBox.SelectedValue == "Einzel")
            {
                RadGridAbrechnung.AllowMultiRowSelection = true;
                foreach (GridDataItem _item in RadGridAbrechnung.Items)
                {
                    if (_item["Ordernumber"].Text == item["Ordernumber"].Text)
                    {
                        _item.Selected = true;
                    }
                }
            }
        }
        private void GenerateInvoice(DataClasses1DataContext dbContext, bool preview)
        {
            //Adress Eigenschaften
            string street = string.Empty,
            streetNumber = string.Empty,
            zipcode = string.Empty,
            city = string.Empty,
            country = string.Empty,
            invoiceRecipient = string.Empty,
            footerText = string.Empty;
            // OrderItem Eigenschaften
            string ProductName = string.Empty,
            Amount = string.Empty;
            Guid customerId = new Guid(CustomerDropDownList.SelectedValue);
            street = StreetTextBox.Text;
            streetNumber = StreetNumberTextBox.Text;
            zipcode = ZipcodeTextBox.Text;
            city = CityTextBox.Text;
            country = CountryTextBox.Text;
            invoiceRecipient = InvoiceRecipient.Text;
            footerText = FooterTextBox.Text;
            Invoice newInvoice = null;
            Adress newAdress = null;
            try
            {     
                newAdress = Adress.CreateAdress(street, streetNumber, zipcode, city, country, dbContext);
                newInvoice = Invoice.CreateInvoice(dbContext, new Guid(Session["CurrentUserId"].ToString()), invoiceRecipient, newAdress.Id, new Guid(CustomerDropDownList.SelectedValue), null, getFullInvoiceName(RechnungsTypComboBox.SelectedValue));
                newInvoice.InvoiceText = footerText;
                //Submiting new Invoice and Adress
                if (!preview)
                {
                    dbContext.SubmitChanges();
                }
                else
                {
                    newInvoice.Adress = newAdress;
                    newInvoice.Customer = dbContext.Customer.SingleOrDefault(q => q.Id == new Guid(CustomerDropDownList.SelectedValue));
                }
                List<SelectedInvoiceItems> virtualItems = new List<SelectedInvoiceItems>();
                SelectedInvoiceItems currItem = new SelectedInvoiceItems();
                foreach (GridDataItem item in RadGridAbrechnung.SelectedItems)
                {
                    currItem = new SelectedInvoiceItems();
                    currItem.ProductName = item["ProductName"].Text;
                    currItem.Amount = decimal.Parse(item["Amount"].Text, NumberStyles.Currency).ToString();

                    currItem.OrderItemId = new Guid(item["OrderItemId"].Text);
                    if (!String.IsNullOrEmpty(item["CostCenterId"].Text) && item["CostCenterId"].Text.Length > 8)
                    {
                        currItem.CostCenterId = new Guid(item["CostCenterId"].Text);
                    }
                    currItem.ItemCount = Convert.ToInt32(item["ItemCount"].Text);
                    currItem.OrderLocationName = item["Location"].Text == "&nbsp;" ? "" : item["Location"].Text;
                    currItem.OrderId = new Guid(item["OrderId"].Text);
                    currItem.OrderLocationId = !EmptyStringIfNull.IsGuid(item["OrderLocation"].Text) ? Guid.Empty : new Guid(item["OrderLocation"].Text);
                    virtualItems.Add(currItem);
                }
                var groupedInvoiceItems = virtualItems.GroupBy(q => q.OrderLocationId.ToString()).ToList();
                if (groupedInvoiceItems.Count > 0 && Session["currentLocationIndex"] != null && EmptyStringIfNull.IsNumber(Session["currentLocationIndex"].ToString()))
                {
                    var myCustomer = dbContext.Customer.FirstOrDefault(q => q.Id == customerId);
                    using (TransactionScope scope = new TransactionScope())
                    {
                        foreach (var item in groupedInvoiceItems[((int)Session["currentLocationIndex"])])
                        {
                            InvoiceItem newInvoiceItem = newInvoice.AddInvoiceItem(item.ProductName, Convert.ToDecimal(item.Amount), item.ItemCount, item.OrderItemId, item.CostCenterId, dbContext);
                            if (newInvoiceItem != null)
                            {
                                var OrderItemToUpdate = dbContext.OrderItem.SingleOrDefault(q => q.Id == item.OrderItemId);
                                OrderItemToUpdate.LogDBContext = dbContext;
                                OrderItemToUpdate.Status = 900;
                                if (myCustomer.SmallCustomer != null)
                                {
                                    newInvoiceItem.VAT = myCustomer.VAT;
                                }
                                dbContext.SubmitChanges();
                                UpdateOrderStatus(dbContext, item.OrderId);
                            }
                        }                  
                        if (!preview)
                        {
                            dbContext.SubmitChanges();
                            scope.Complete();

                        }
                        else if (groupedInvoiceItems.Count > 0)
                        {
                            using (MemoryStream memS = new MemoryStream())
                            {
                                string fileName = "RechnungsVorschau_" + DateTime.Now.Ticks.ToString() + ".pdf";
                                string serverPath = ConfigurationManager.AppSettings["DataPath"] + "\\UserData";
                                newInvoice.PrintPreview(dbContext, memS, "");
                                if (!Directory.Exists(serverPath)) Directory.CreateDirectory(serverPath);
                                if (!Directory.Exists(serverPath + "\\" + Session["CurrentUserId"].ToString())) Directory.CreateDirectory(serverPath + "\\" + Session["CurrentUserId"].ToString());
                                serverPath = serverPath + "\\" + Session["CurrentUserId"].ToString();
                                File.WriteAllBytes(serverPath + "\\" + fileName, memS.ToArray());
                                OpenPrintfile(fileName);
                            }
                        }
                        AllesIstOkeyLabel.Text = "Rechnung für " + myCustomer.Name + " wurde erfolgreich hinzugefügt";
                    }
                        if (Session["currentLocationIndex"] != null && ((int)Session["currentLocationIndex"]) < groupedInvoiceItems.Count() - 1)
                        {
                            Session["currentLocationIndex"] = ((int)Session["currentLocationIndex"]) + 1;
                            AddAdressButton_Click(this, new EventArgs());
                        }
                        else
                        {
                            if (!preview)
                            {
                                RadGridAbrechnung.MasterTableView.ClearChildEditItems();
                                RadGridAbrechnung.MasterTableView.ClearEditItems();
                                RadGridAbrechnung.Rebind();
                                string script = "function f(){$find(\"" + AddAdressRadWindow.ClientID + "\").close(); Sys.Application.remove_load(f);}Sys.Application.add_load(f); $find(\"" + RadGridAbrechnung.ClientID + "\").get_masterTableView().rebind();";
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
                            }
                            Session["currentLocationIndex"] = 0;
                        }                    
                }
                else
                {
                    throw new Exception("Achtung die Session ist abgelaufen! Bitte loggen Sie sich erneut ein.");
                }
            }
            catch (Exception ex)
            {
                try
                {
                    if (newInvoice != null)
                    {

                        dbContext.Invoice.DeleteOnSubmit(newInvoice);
                        dbContext.SubmitChanges();
                        if (newAdress != null)
                            dbContext.Adress.DeleteOnSubmit(newAdress);
                            dbContext.SubmitChanges();
                    }
                }
                catch { AbrechnungSaveErrorLabel.Text = "Leider war es nicht möglich durch das System die defekte Rechnung zu löschen (Datenbankkonflikt), Sie können diese auch manuell  im Reiter 'Rechnung erstellen' stornieren"; }
                AbrechnungSaveErrorLabel.Visible = true;
                AbrechnungSaveErrorLabel.Text += "Fehler:" + ex.Message;
               // WindowManager1.RadAlert("Fehler:" + ex.Message, 380, 180, "Fehler", "");
            }       
        }
        private void OpenPrintfile(string myFile)
        {
            string url = ConfigurationManager.AppSettings["BaseUrl"];
            string path = url + "UserData/" + Session["CurrentUserId"].ToString() + "/" + myFile;   
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Invoice", "<script>openFile('" + path + "');</script>", false);         
        }
        #endregion
        /// <summary>
        /// Orders werden nach OrderItemStatus geprüft
        /// Falls nicht alle Items abgerechnet sind - Status 700 (Teilabgerechnet)
        ///Falls alle Items schon abgerechnet - Status 900 (Abgerechnet)
        /// </summary>
        /// <param name="con"></param>
        /// <param name="orderId"></param>
        protected void UpdateOrderStatus(DataClasses1DataContext con ,Guid orderId)
        {
            bool shouldBeUpdated = false;
            bool hasAbgerechnetItem = false;         
            var orderQuery = con.Order.SingleOrDefault(q => q.Id == orderId);
            orderQuery.LogDBContext = con;
            foreach (OrderItem item in orderQuery.OrderItem)
            {
                if (item.Status == 600)
                {
                    shouldBeUpdated = true;
                }

                if(item.Status == 900)
                {
                    hasAbgerechnetItem = true;
                }
            }
            if (shouldBeUpdated == true && hasAbgerechnetItem == true) // teil
            {
                orderQuery.Status = 700;
            }
            else if (shouldBeUpdated == false && hasAbgerechnetItem == true) //komplet abgerechnet
            {
                orderQuery.Status = 900;
            }
            con.SubmitChanges();
        }
        /// <summary>
        /// Datasource fuer den Standort
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void StandortLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            Guid myId = Guid.Empty;
            if (CustomerDropDownList.SelectedValue == "")
            {
                myId = Guid.Empty;
            }
            else
            {
                myId = new Guid(CustomerDropDownList.SelectedValue);

            }
                var standortQuery = from stand in con.Location
                                    join cust in con.Customer on stand.CustomerId equals cust.Id
                                    where cust.Id == myId
                                    select new { Name = stand.Name, Value = stand.Id };
                e.Result = standortQuery;                   
        }
       /// <summary>
       /// Event fuer den Standort
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        protected void Standort_Changed(object sender, EventArgs e)
        {
            AddAdressButton.Enabled = true;
            RadGridAbrechnung.Rebind();
        }
        /// <summary>
        /// Event fuer die Checkbox die alle Standorte anzeigt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AllLocationsCheckBox_Changed(object sender, EventArgs e)
        {
            //AddAdressButton.Enabled = true;
            if(AllLocationsCheckBox.Checked == true)
            {
                StandortDropDown.Enabled = false; 
                GridGroupByExpression expression1 = GridGroupByExpression.Parse("Group By Location");
                GridGroupByField existing = expression1.SelectFields.FindByName("Location");               
                if (existing == null) //field is not present
                {
                    //Construct and add a new aggregate field
                    GridGroupByField field = new GridGroupByField();
                    field.FieldName = "Location";
                    //field.FieldAlias = "SubTotal";
                    //field.Aggregate = GridAggregateFunction.Sum;
                    field.FormatString = "{0:C}";
                    expression1.SelectFields.Add(field);
                }
                else //field is present then set a format string
                {
                    existing.FormatString = "{0:C}";
                }
                this.RadGridAbrechnung.MasterTableView.GroupByExpressions.Add(expression1);
                RadGridAbrechnung.Rebind();         
            }
            else
            {
                StandortDropDown.Enabled = true;
                StandortDropDown.DataBind();
                this.RadGridAbrechnung.MasterTableView.GroupByExpressions.Clear();
                this.RadGridAbrechnung.Rebind();
            }            
        }
    }
}