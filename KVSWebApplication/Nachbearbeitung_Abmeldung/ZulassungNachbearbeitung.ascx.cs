using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using KVSCommon.Database;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Transactions;

namespace KVSWebApplication.Nachbearbeitung_Abmeldung
{
    /// <summary>
    ///Codebehind für die Maske (Reiter) Abmeldung Nachbearbeitung
    /// </summary>
    public partial class ZulassungNachbearbeitung : System.Web.UI.UserControl
    {
        private string customer = string.Empty;      
        protected void RadGridAbmeldung_PreRender(object sender, EventArgs e)
        {
            HideExpandColumnRecursive(RadGridAbmeldung.MasterTableView);
        }     
        public void HideExpandColumnRecursive(GridTableView tableView)
        {
            GridItem[] nestedViewItems = tableView.GetItems(GridItemType.NestedView);
            foreach (GridNestedViewItem nestedViewItem in nestedViewItems)
            {
                foreach (GridTableView nestedView in nestedViewItem.NestedTableViews)
                {
                    nestedView.ParentItem.Expanded = true;
                    HideExpandColumnRecursive(nestedView);
                }
            }
        }      
        protected void Page_Load(object sender, EventArgs e)
        {
            string target = Request["__EVENTTARGET"] == null ? "" : Request["__EVENTTARGET"];
            if (String.IsNullOrEmpty(target))
                if (Session["orderNumberSearch"] != null)
                    if (!String.IsNullOrEmpty(Session["orderNumberSearch"].ToString()))
                        target = "IamFromSearch";
            StornierungErfolgLabel.Visible = false;
            if (Session["CustomerIndex"] != null)
            {                  
                if (target == null | (!target.Contains("CustomerDropDownListAbmeldungZulassunsstelle") && !target.Contains("CustomerDropDownListAbmeldungZulassunsstelle") && !target.Contains("NewPositionZulButton") && !target.Contains("StornierenButton")))
                {
                    if (Session["CustomerId"] != null)
                    {
                        if (!Page.IsPostBack)
                        {
                            if (RadComboCustomerAbmeldungZulassunsstelle.Items.Count > 0 && CustomerDropDownListAbmeldungZulassunsstelle.Items.Count() > 0)
                            {
                                RadComboCustomerAbmeldungZulassunsstelle.SelectedValue = Session["CustomerIndex"].ToString();
                                CustomerDropDownListAbmeldungZulassunsstelle.SelectedValue = Session["CustomerId"].ToString();
                            }
                        }
                        RadGridAbmeldung.Enabled = true;
                        if(target.Contains("AuftragsbearbeitungAbmeldung") && target.Contains("EditButton"))
                            target = "Editbutton";
                        if (target.Contains("AuftragsbearbeitungAbmeldung") || target.Contains("RadTabStrip1") || target.Contains("IamFromSearch"))
                            RadGridAbmeldung.DataBind();
                    }
                }
            }          
        }
        protected void CheckOpenedOrders()
        {
            ordersCount.Text = Order.getUnfineshedOrdersCount(new DataClasses1DataContext(), "Abmeldung", 400).ToString();
            if (ordersCount.Text == "" || ordersCount.Text == "0")
            {
                go.Visible = false;
            }
            else
            {
                go.Visible = true;
            }
        }
        protected void AbmeldungenLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
         
            //select all values for small customers
            if (RadComboCustomerAbmeldungZulassunsstelle.SelectedValue == "1") // Small
            {
                DataClasses1DataContext con = new DataClasses1DataContext();
                RadGridAbmeldung.Columns.FindByUniqueName("CustomerLocation").Visible = false;
                var smallCustomerQuery = from ord in con.Order
                                         join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                         join cust in con.Customer on ord.CustomerId equals cust.Id
                                         join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                         join derord in con.DeregistrationOrder on ord.OrderNumber equals derord.OrderNumber
                                         join reg in con.Registration on derord.RegistrationId equals reg.Id
                                         join veh in con.Vehicle on derord.VehicleId equals veh.Id
                                         join smc in con.SmallCustomer on cust.Id equals smc.CustomerId
                                         orderby ord.OrderNumber descending
                                         where  ord.Status == 400 && ordtype.Name == "Abmeldung" && ord.HasError.GetValueOrDefault(false) != true
                                         select new
                                         {
                                             OrderId = ord.Id,
                                             customerID = ord.CustomerId,
                                             OrderNumber = ord.OrderNumber,
                                             CreateDate = ord.CreateDate,
                                             Status = ordst.Name,
                                             CustomerName = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + "  " + cust.SmallCustomer.Person.Name : cust.Name,
                                             CustomerLocation = "",
                                             Kennzeichen = reg.Licencenumber,
                                             VIN = veh.VIN,
                                             TSN = veh.TSN,
                                             HSN = veh.HSN,
                                             OrderTyp = ordtype.Name,
                                             Freitext = ord.FreeText
                                         };
                if (CustomerDropDownListAbmeldungZulassunsstelle.SelectedValue != string.Empty)
                {
                    var custId = Int32.Parse(CustomerDropDownListAbmeldungZulassunsstelle.SelectedValue);
                    smallCustomerQuery = smallCustomerQuery.Where(q => q.customerID == custId);
                }
                e.Result = smallCustomerQuery;
            }
            //select all values for large customers
            else if (RadComboCustomerAbmeldungZulassunsstelle.SelectedValue == "2") // Large
            {
                DataClasses1DataContext con = new DataClasses1DataContext();               
                var largeCustomerQuery1 = from ord in con.Order
                                          join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                          join cust in con.Customer on ord.CustomerId equals cust.Id
                                          join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                          join loc in con.Location on ord.LocationId equals loc.Id
                                          join derord in con.DeregistrationOrder on ord.OrderNumber equals derord.OrderNumber
                                          join reg in con.Registration on derord.RegistrationId equals reg.Id
                                          join veh in con.Vehicle on derord.VehicleId equals veh.Id
                                          join lmc in con.LargeCustomer on cust.Id equals lmc.CustomerId
                                          orderby ord.OrderNumber descending
                                          where  ord.Status == 400 && ordtype.Name == "Abmeldung" && ord.HasError.GetValueOrDefault(false) != true
                                          select new
                                          {
                                              OrderId = ord.Id,
                                              locationId = loc.Id,
                                              customerID = ord.CustomerId,
                                              OrderNumber = ord.OrderNumber,
                                              CreateDate = ord.CreateDate,
                                              Status = ordst.Name,
                                              CustomerName = cust.Name,
                                              Kennzeichen = reg.Licencenumber,
                                              VIN = veh.VIN,
                                              TSN = veh.TSN,
                                              HSN = veh.HSN,
                                              CustomerLocation = loc.Name,
                                              OrderTyp = ordtype.Name,
                                              Freitext = ord.FreeText
                                          };
                if (CustomerDropDownListAbmeldungZulassunsstelle.SelectedValue != string.Empty)
                {
                    var custId = Int32.Parse(CustomerDropDownListAbmeldungZulassunsstelle.SelectedValue);
                    largeCustomerQuery1 = largeCustomerQuery1.Where(q => q.customerID == custId);
                }
                if (Session["orderNumberSearch"] != null)
                {
                    if (!String.IsNullOrEmpty(Session["orderNumberSearch"].ToString()))
                    {
                        if (Session["orderStatusSearch"].ToString().Contains("Zulassungsstelle"))
                        {
                            int orderNumber = Convert.ToInt32(Session["orderNumberSearch"].ToString());
                            largeCustomerQuery1 = largeCustomerQuery1.Where(q => q.OrderNumber == orderNumber);
                        } 
                    }
                }
                e.Result = largeCustomerQuery1;
            }
            CheckOpenedOrders();
        }
        protected void ShowAllButton_Click(object sender, EventArgs e)
        {          
            CustomerDropDownListAbmeldungZulassunsstelle.ClearSelection();
            RadGridAbmeldung.Enabled = true;
            RadGridAbmeldung.Rebind();
        }
        // Small oder Large -> Auswahl der KundenName
        protected void CustomerZulLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            if (RadComboCustomerAbmeldungZulassunsstelle.SelectedValue == "1") //Small Customers
            {
                var customerQuery = from cust in con.Customer
                                    where cust.Id == cust.SmallCustomer.CustomerId
                                    select new
                                    {Name = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name,
                                     Value = cust.Id, Matchcode = cust.MatchCode, Kundennummer = cust.CustomerNumber };
                e.Result = customerQuery;
            }
            else if (RadComboCustomerAbmeldungZulassunsstelle.SelectedValue == "2") //Large Customers
            {
                var customerQuery = from cust in con.Customer
                                    where cust.Id == cust.LargeCustomer.CustomerId
                                    select new { Name = cust.Name, Value = cust.Id, Matchcode = cust.MatchCode, Kundennummer = cust.CustomerNumber };
                e.Result = customerQuery;
            }
        }
        // Large oder small Customer
        protected void SmLrCustomerIndex_Changed(object sender, EventArgs e)
        {
            CustomerDropDownListAbmeldungZulassunsstelle.Enabled = true;
            this.CustomerDropDownListAbmeldungZulassunsstelle.DataBind();
            this.RadGridAbmeldung.DataBind();
            Session["CustomerIndex"] = RadComboCustomerAbmeldungZulassunsstelle.SelectedValue;
        }
        // Auswahl von Kunde
        protected void CustomerZulassungIndex_Changed(object sender, EventArgs e)
        {
            this.RadGridAbmeldung.Enabled = true;
            this.RadGridAbmeldung.DataBind();
            Session["CustomerIndex"] = RadComboCustomerAbmeldungZulassunsstelle.SelectedValue;
            Session["CustomerId"] = CustomerDropDownListAbmeldungZulassunsstelle.SelectedValue;
        }        
        protected void AbmeldungFertigStellen_Command(object sender, EventArgs e)
        {
                AbmeldungErfolgtLabel.Visible = false;
                string VIN = string.Empty,
                      kennzeichen = string.Empty,
                      HSN = string.Empty,
                      TSN = string.Empty;
                int customerId = 0;
                Button editButton = sender as Button;

                Panel item = editButton.Parent as Panel;
                (((GridNestedViewItem)(((GridTableCell)(item.Parent.Parent)).Parent))).ParentItem.Selected = true;
                RadDateTimePicker picker = item.FindControl("CreateDateBox") as RadDateTimePicker;
                TextBox kennzeichenBox = item.FindControl("KennzeichenBox") as TextBox;
                TextBox vinBox = item.FindControl("VINBox") as TextBox;
                TextBox orderIdBox = item.FindControl("orderIdBox") as TextBox;
          
                CheckBox errorCheckBox = item.FindControl("ErrorZulCheckBox") as CheckBox;
                TextBox errorReasonTextBox = item.FindControl("ErrorReasonZulTextBox") as TextBox;
                TextBox HSNBox = item.FindControl("HSNAbmBox") as TextBox;
                TextBox TSNBox = item.FindControl("TSNAbmBox") as TextBox;
                TextBox amtGebBox = item.FindControl("AmtGebTextBox") as TextBox;
                ErrorZulLabel.Visible = false;
                kennzeichen = kennzeichenBox.Text.ToUpper();
                VIN = vinBox.Text;
                var orderId = Int32.Parse(orderIdBox.Text);

                if (!String.IsNullOrEmpty(CustomerDropDownListAbmeldungZulassunsstelle.SelectedValue.ToString()))
                    customerId = Int32.Parse(CustomerDropDownListAbmeldungZulassunsstelle.SelectedValue);
                else
                {
                    TextBox customerid = item.FindControl("customerIdBox") as TextBox;
                    customerId = Int32.Parse(customerid.Text);
                }
                HSN = HSNBox.Text;
                TSN = TSNBox.Text;
                if (errorCheckBox.Checked) // falls Auftrag als Fehler gemeldet sollte
                {
                    string errorReason = errorReasonTextBox.Text;
                    try
                    {
                        DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                        var OrderToUpdate = dbContext.Order.SingleOrDefault(q => q.Id == orderId && q.CustomerId == customerId);
                        OrderToUpdate.LogDBContext = dbContext;
                        OrderToUpdate.HasError = true;
                        OrderToUpdate.ErrorReason = errorReason;
                        dbContext.SubmitChanges();
                        RadGridAbmeldung.MasterTableView.ClearChildEditItems();
                        RadGridAbmeldung.MasterTableView.ClearEditItems();
                        RadGridAbmeldung.Rebind();
                    }
                    catch (Exception ex)
                    {
                        AbmeldungErrLabel.Text = "Fehler: " + ex.Message;
                        AbmeldungErrLabel.Visible = true;
                    }
                }
                else  // falls normales Update 
                {
                    bool amtlGebVor = false;
                    DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                    var myOrder = dbContext.Order.SingleOrDefault(q => q.Id == orderId && q.CustomerId == customerId);
                    foreach (var orderItem in myOrder.OrderItem)
                    {
                        if (orderItem.IsAuthorativeCharge)
                            amtlGebVor = true;
                    }                    
                    if (String.IsNullOrEmpty(kennzeichen))
                    {
                        ErrorZulLabel.Text = "Bitte geben Sie die Kennzeichen ein!";
                        ErrorZulLabel.Visible = true;
                    }
                    else if (!amtlGebVor)
                    {
                        ErrorZulLabel.Text = "Bitte fügen Sie eine Amtl.Gebühr hinzu!";
                        ErrorZulLabel.Visible = true;
                    }
                    else
                    {
                        ErrorZulLabel.Visible = false;
                        try
                        {
                            updateDataBase(kennzeichen, VIN, TSN, HSN, orderId, customerId);
                            UpdateOrderAndItemsStatus();
                            AbmeldungErfolgtLabel.Visible = true;
                        }
                        catch(Exception ex)
                        {
                            AbmeldungErrLabel.Text = "Fehler:"+ex.Message;
                            AbmeldungErrLabel.Visible = true;
                        }
                    }  
                if (Session["orderNumberSearch"] != null)
                    Session["orderNumberSearch"] = string.Empty; //after search should be empty
                RadGridAbmeldung.MasterTableView.ClearChildEditItems();
                RadGridAbmeldung.MasterTableView.ClearEditItems();
                RadGridAbmeldung.Rebind();
                CheckOpenedOrders();
            }         
        }
        //falls benötigt, wird der Status per Email gesendet
        protected void SendStatusByEmail(int customerId, Order orderToSend)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
            var customerAuswahl = dbContext.LargeCustomer.SingleOrDefault(q => q.CustomerId == customerId);
            string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
            string smtp = ConfigurationManager.AppSettings["smtpHost"];
            if (customerAuswahl.OrderFinishedNoteSendType == 1) //sofort gesendet
            {
                try
                {
                    Order.SendOrderFinishedNote(orderToSend, fromEmail, smtp, dbContext);
                }
                catch
                {
                    throw new Exception("Email mit OrderStatus konnte nicht gesendet werden.");
                }
            }
        }
        // Create new Adress in der DatenBank
        protected void OnAddAdressButton_Clicked(object sender, EventArgs e)
        {
            //Adress Eigenschaften
            string street = string.Empty,
                streetNumber = string.Empty,
                zipcode = string.Empty,
                city = string.Empty,
                country = string.Empty,
                invoiceRecipient = string.Empty;
            // OrderItem Eigenschaften
            string ProductName = string.Empty;
            decimal Amount = 0;
            int customerId = 0;
            if (!String.IsNullOrEmpty(CustomerDropDownListAbmeldungZulassunsstelle.SelectedValue))
            {
                customerId = Int32.Parse(CustomerDropDownListAbmeldungZulassunsstelle.SelectedValue);
            }
            else if (!String.IsNullOrEmpty(CustomerIdHiddenField.Value))
            {
                customerId = Int32.Parse(CustomerIdHiddenField.Value);
            }
            else
            {
            }
            int? costCenterId = null;
            street = StreetTextBox.Text;
            streetNumber = StreetNumberTextBox.Text;
            zipcode = ZipcodeTextBox.Text;
            city = CityTextBox.Text;
            country = CountryTextBox.Text;
            invoiceRecipient = InvoiceRecipient.Text;
            int itemCount = 0;
            TransactionScope scope = null;
            AbmeldungErrLabel.Visible = false;
            try
            {
                using (DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())))
                {
                    using (scope = new TransactionScope())
                    {
                        var newAdress = Adress.CreateAdress(street, streetNumber, zipcode, city, country, dbContext);
                        var myCustomer = dbContext.Customer.FirstOrDefault(q => q.Id == customerId);
                        var newInvoice = Invoice.CreateInvoice(dbContext, Int32.Parse(Session["CurrentUserId"].ToString()), invoiceRecipient, 
                            newAdress, customerId, txbDiscount.Value, "Einzelrechnung");
                        InvoiceIdHidden.Value = newInvoice.Id.ToString();
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
                            if (costCenterId.HasValue)
                            {
                                costCenter = dbContext.CostCenter.FirstOrDefault(o => o.Id == costCenterId.Value);
                            }

                            InvoiceItem newInvoiceItem = newInvoice.AddInvoiceItem(ProductName, Convert.ToDecimal(Amount), itemCount, ordItem, costCenter, dbContext);
                            ordItem.LogDBContext = dbContext;
                            newInvoiceItem.VAT = myCustomer.VAT;
                            ordItem.Status = 900;
                            dbContext.SubmitChanges();
                        }
                        // Submiting new InvoiceItems
                        dbContext.SubmitChanges();
                        // Closing RadWindow
                        string script = "function f(){$find(\"" + AddAdressRadWindow.ClientID + "\").close(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
                        Print(newInvoice, dbContext);
                        orderQuery.LogDBContext = dbContext;
                        orderQuery.Status = 900;
                        dbContext.SubmitChanges();
                        scope.Complete();                       
                    }
                    RadGridAbmeldung.Rebind();
                }
            }
            catch (Exception ex)
            {
                AbmeldungErrLabel.Text = " Fehler:" + ex.Message;
                AbmeldungErrLabel.Visible = true;
            }
        }
        protected void Print(Invoice newInvoice, DataClasses1DataContext dbContext)
        {
            using (MemoryStream memS = new MemoryStream())
            {               
                string serverPath = ConfigurationManager.AppSettings["DataPath"] + "\\UserData";
                InvoiceHelper.CreateAccounts(dbContext, newInvoice);
                newInvoice.Print(dbContext, memS, "");
                string fileName = "Rechnung_" + newInvoice.InvoiceNumber.Number + "_" + newInvoice.CreateDate.Day + "_" + newInvoice.CreateDate.Month + "_" + newInvoice.CreateDate.Year + ".pdf";
                if (!Directory.Exists(serverPath)) Directory.CreateDirectory(serverPath);
                if (!Directory.Exists(serverPath + "\\" + Session["CurrentUserId"].ToString())) Directory.CreateDirectory(serverPath + "\\" + Session["CurrentUserId"].ToString());
                serverPath = serverPath + "\\" + Session["CurrentUserId"].ToString();
                File.WriteAllBytes(serverPath + "\\" + fileName, memS.ToArray());
                string url = ConfigurationManager.AppSettings["BaseUrl"];
                string path = url + "UserData/" + Session["CurrentUserId"].ToString() + "/" + fileName;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Invoice", "<script>openFile('" + path + "');</script>", false);
                dbContext.SubmitChanges();
            }
        }
        protected void OnItemCommand_Fired(object sender, GridCommandEventArgs e)
        {
            try
            {
                AbmeldungErrLabel.Text = "";
                AbmeldungErrLabel.Visible = false;
                if (e.CommandName == "AmtGebuhrSetzen")
                {
                    GridEditableItem editedItem = e.Item as GridEditableItem;
                    RadTextBox tbEditPrice = editedItem["ColumnPrice"].FindControl("tbEditPrice") as RadTextBox;
                    string itemId = editedItem["ItemIdColumn"].Text;
                    RadTextBox tbAuthPrice = editedItem["AuthCharge"].FindControl("tbAuthChargePrice") as RadTextBox;

                    int? authChargeId = null;
                    if (!String.IsNullOrEmpty(editedItem["AuthChargeId"].Text))
                    {
                        authChargeId = Int32.Parse(editedItem["AuthChargeId"].Text);
                    }

                    using (DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())))
                    {
                        if (Order.GenerateAuthCharge(dbContext, authChargeId, itemId, tbAuthPrice.Text))
                        {
                            dbContext.SubmitChanges();
                            tbAuthPrice.ForeColor = System.Drawing.Color.Green;
                        }
                    }
                    UpdatePosition(itemId, tbEditPrice.Text);
                    tbEditPrice.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    if (e.Item is GridDataItem)
                    {
                        var button = sender as RadButton;
                        GridDataItem dataItem = e.Item as GridDataItem;
                        dataItem.Selected = true;
                        itemIndexHiddenField.Value = dataItem.ItemIndex.ToString();
                    }
                }
            }          
            catch (Exception ex)
            {
                AbmeldungErrLabel.Text = "Fehler: " + ex.Message;
                AbmeldungErrLabel.Visible = true;
            }
        }
        // Updating ausgewählten OrderItem
        protected void UpdatePosition(string itemId, string amount)
        {
            string amoutToSave = amount;
            if (amoutToSave.Contains("."))
                amoutToSave = amoutToSave.Replace(".", ",");
            if (!EmptyStringIfNull.IsNumber(amount))
                throw new Exception("Achtung, Sie haben keinen gültigen Preis eingegeben");
 
            if (!String.IsNullOrEmpty(itemId))
            {
                try
                {
                    var orderItemId = Int32.Parse(itemId);
                    DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                    var positionUpdateQuery = dbContext.OrderItem.SingleOrDefault(q => q.Id == orderItemId);
                    positionUpdateQuery.LogDBContext = dbContext;
                    positionUpdateQuery.Amount = Convert.ToDecimal(amoutToSave);
                    dbContext.SubmitChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception("Die ausgewählte Position kann nicht updatet werden <br /> Error: " + ex.Message);
                }
            }
        }
        // Automatische Suche nach HSN
        protected void HSNBox_TextChanged(object sender, EventArgs e)
        {
            RadTextBox hsnTextBox = sender as RadTextBox;
            Label hsnLabel = hsnTextBox.Parent.FindControl("HSNSearchLabel") as Label;
            TextBox tsnBox = hsnTextBox.Parent.FindControl("TSNAbmBox") as TextBox;          
            hsnLabel.Text = "";
            if (!String.IsNullOrEmpty(hsnTextBox.Text))
            {
                hsnLabel.Visible = true;
                hsnLabel.Text = Make.GetMakeByHSN(hsnTextBox.Text);
            }
            tsnBox.Focus();
        }
        // Updating Status 600 für Order und OrderItems
        protected void UpdateOrderAndItemsStatus()
        {
            if (RadGridAbmeldung.SelectedItems.Count > 0)
            {
                AbmeldungErrLabel.Visible = false;
                AbmeldungErfolgtLabel.Visible = false;
                    foreach (GridDataItem item in RadGridAbmeldung.SelectedItems)
                    {
                        // Vorbereitung für Update
                        int customerID = 0;
                        if (!String.IsNullOrEmpty(CustomerDropDownListAbmeldungZulassunsstelle.SelectedValue.ToString()))
                            customerID = Int32.Parse(CustomerDropDownListAbmeldungZulassunsstelle.SelectedValue);
                        else
                            customerID = Int32.Parse(item["customerID"].Text);
                        var orderId = Int32.Parse(item["OrderNumber"].Text);
                        smallCustomerOrderHiddenField.Value = orderId.ToString();
                        CustomerIdHiddenField.Value = customerID.ToString();
                        try
                        {
                            DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                            var newOrder = dbContext.Order.Single(q => q.CustomerId == customerID && q.Id == orderId);
                            if (newOrder != null)
                            {
                                if (RadComboCustomerAbmeldungZulassunsstelle.SelectedIndex == 1) // small
                                {
                                    //updating order status
                                    newOrder.LogDBContext = dbContext;
                                    newOrder.Status = 600;
                                    newOrder.ExecutionDate = DateTime.Now;
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
                                    //opening window for adress
                                    string script = "function f(){$find(\"" + AddAdressRadWindow.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
                                    SetValuesForAdressWindow(customerID);
                                }
                                else //large
                                {
                                    //updating order status
                                    newOrder.LogDBContext = dbContext;
                                    newOrder.Status = 600;
                                    newOrder.ExecutionDate = DateTime.Now;
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
                                    SendStatusByEmail(customerID, newOrder);
                                }                
                            }
                        }
                        catch(Exception ex)
                        {
                            AbmeldungErrLabel.Text = "Fehler: "+ex.Message;
                            AbmeldungErrLabel.Visible = true;
                        }
                    }
                    // erfolgreich
                    RadGridAbmeldung.DataBind();
                    AbmeldungErfolgtLabel.Visible = true;
            }
            else
            {
                AbmeldungErrLabel.Text = "Sie haben keinen Auftrag ausgewählt!";
                AbmeldungErrLabel.Visible = true;
            }
        }
        // getting adress from small customer
        protected void SetValuesForAdressWindow(int customerId)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            var locationQuery = (from adr in dbContext.Adress
                                 join cust in dbContext.Customer on adr.Id equals cust.InvoiceAdressId
                                 where cust.Id == customerId
                                 select adr).SingleOrDefault();
            StreetTextBox.Text = locationQuery.Street;
            StreetNumberTextBox.Text = locationQuery.StreetNumber;
            ZipcodeTextBox.Text = locationQuery.Zipcode;
            CityTextBox.Text = locationQuery.City;
            CountryTextBox.Text = locationQuery.Country;
            LocationLabelWindow.Text = "Fügen Sie bitte die Adresse für " + CustomerDropDownListAbmeldungZulassunsstelle.Text + " hinzu";
            ZusatzlicheInfoLabel.Visible = false;
            if (RadComboCustomerAbmeldungZulassunsstelle.SelectedIndex == 1) // small
            {
                ZusatzlicheInfoLabel.Visible = true;
            }
        }
        //Prüfen ob alle Werte da sind um den Auftrag auf "Zulassungstelle" zu setzen
        private bool CheckIfAllExistsToUpdate()
        {
            bool shouldBeUpdated = true;
            AbmeldungErrLabel.Visible = false;
            foreach (GridDataItem item in RadGridAbmeldung.SelectedItems)
            {
                if (String.IsNullOrEmpty(item["VIN"].Text))
                {
                    shouldBeUpdated = false;
                    AbmeldungErrLabel.Text = "Bitte fügen Sie FIN ein";
                    AbmeldungErrLabel.Visible = true;
                }
                if (String.IsNullOrEmpty(item["TSN"].Text))
                {
                    shouldBeUpdated = false;
                    AbmeldungErrLabel.Text = "Bitte fügen Sie TSN ein";
                    AbmeldungErrLabel.Visible = true;
                }
                if (String.IsNullOrEmpty(item["HSN"].Text))
                {
                    shouldBeUpdated = false;
                    AbmeldungErrLabel.Text = "Bitte fügen Sie HSN ein";
                    AbmeldungErrLabel.Visible = true;
                }
                if (String.IsNullOrEmpty(item["CustomerLocation"].Text))
                {
                    shouldBeUpdated = false;
                    AbmeldungErrLabel.Text = "Bitte fügen Sie Standort ein";
                    AbmeldungErrLabel.Visible = true;
                }
            }
            return shouldBeUpdated;
        }
        // Updating Order und OrderItems auf Status 600 - Abgeschlossen
        protected void updateDataBase(string kennzeichen, string vin, string tsn, string hsn, int orderId, int customerId)
        {
            using (DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())))
            {
                var orderUpdateQuery = dbContext.Order.SingleOrDefault(q => q.Id == orderId && q.CustomerId == customerId);
                orderUpdateQuery.LogDBContext = dbContext;
                orderUpdateQuery.DeregistrationOrder.Registration.LogDBContext = dbContext;
                orderUpdateQuery.DeregistrationOrder.Registration.Licencenumber = kennzeichen;
                orderUpdateQuery.DeregistrationOrder.Vehicle.VIN = vin;
                orderUpdateQuery.DeregistrationOrder.Vehicle.TSN = tsn;
                orderUpdateQuery.DeregistrationOrder.Vehicle.HSN = hsn;

                dbContext.SubmitChanges();
            }
        }
        //Row Index wird in hiddenfield gespeichert
        protected void Edit_Command(object sender, GridCommandEventArgs e)
        {
            var button = sender as RadButton;
            GridDataItem dataItem = e.Item as GridDataItem;
            dataItem.Selected = true;
            itemIndexHiddenField.Value = dataItem.ItemIndex.ToString();
        }
        protected void RadGridAbmeldung_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            GridDataItem item = (GridDataItem)e.DetailTableView.ParentItem;
            var orderId = Int32.Parse(item["OrderNumber"].Text);
            var positionQuery = from ord in dbContext.Order
                                join orditem in dbContext.OrderItem on ord.OrderNumber equals orditem.OrderNumber
                                let authCharge = dbContext.OrderItem.FirstOrDefault(s => s.SuperOrderItemId == orditem.Id)
                                where ord.Id == orderId && (orditem.SuperOrderItemId == null)
                                select new
                                {
                                    OrderItemId = orditem.Id,
                                    Amount = orditem.Amount == null ? "kein Preis" : (Math.Round(orditem.Amount, 2, MidpointRounding.AwayFromZero)).ToString(),
                                    ProductName = orditem.IsAuthorativeCharge ? orditem.ProductName + " (Amtl.Gebühr)" : orditem.ProductName,
                                    AmtGebuhr = authCharge == null ? false : true,
                                    AuthCharge = authCharge == null || authCharge.Amount == null ? "kein Preis" : (Math.Round(authCharge.Amount, 2, MidpointRounding.AwayFromZero)).ToString(),
                                    AuthChargeId = authCharge == null ? (int?)null : authCharge.Id
                                };
            GridDataItem dataItem = item;
            GridNestedViewItem nestedItem = (GridNestedViewItem)dataItem.ChildItem;
            RadGrid radGrdEnquiriesVarients = (RadGrid)nestedItem.FindControl("RadGridNeuzulassungDetails");
            radGrdEnquiriesVarients.DataSource = positionQuery;
            radGrdEnquiriesVarients.DataBind();
        }
        protected void NewPositionButton_Clicked(object sender, EventArgs e)
        {
            if (RadGridAbmeldung.SelectedItems.Count > 0)
            {
                   try
                {
                AbmeldungErrLabel.Visible = false;
                DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                Button button = sender as Button;
                int locationId = 0;
                Price newPrice = null;
                OrderItem newOrderItem1 = null;
                var productDropDown = button.NamingContainer.FindControl("NewProductDropDownList") as RadComboBox;
                var costCenterDropDown = button.NamingContainer.FindControl("CostCenterDropDownList") as DropDownList;
                var product = Int32.Parse(productDropDown.SelectedValue);

                foreach (GridDataItem item in RadGridAbmeldung.SelectedItems)
                {
                    var orderId = Int32.Parse(item["OrderNumber"].Text);
                    KVSCommon.Database.Product newProduct = dbContext.Product.SingleOrDefault(q => q.Id == Int32.Parse(productDropDown.SelectedValue));
                    if (CustomerDropDownListAbmeldungZulassunsstelle.SelectedValue == "2") // if small customer
                    {
                        locationId = Int32.Parse(item["locationId"].Text);
                        newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == locationId);
                    }                                                       
                    if (newPrice == null)
                    {
                        newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == null);
                    }
                    var orderToUpdate = dbContext.Order.SingleOrDefault(q => q.Id == orderId);
                    if (newPrice == null || newProduct == null || orderToUpdate == null)
                        throw new Exception("Achtung, die Position kann nicht hinzugefügt werden, es konnte entweder kein Preis, Produkt oder der Auftrag gefunden werden!");
                    orderToUpdate.LogDBContext = dbContext;
                    if (orderToUpdate != null)
                    {

                        orderToUpdate.LogDBContext = dbContext;
                        var orderItemCostCenter = orderToUpdate.OrderItem.FirstOrDefault(q => q.CostCenter != null);

                        newOrderItem1 = orderToUpdate.AddOrderItem(newProduct.Id, newPrice.Amount, 1, (orderItemCostCenter != null) ? orderItemCostCenter.CostCenter : null,
                            null, false, dbContext);
                        if (newPrice.AuthorativeCharge.HasValue)
                        {
                            orderToUpdate.AddOrderItem(newProduct.Id, newPrice.AuthorativeCharge.Value, 1, (orderItemCostCenter != null) ? orderItemCostCenter.CostCenter : null,
                                newOrderItem1.Id, newPrice.AuthorativeCharge.HasValue, dbContext);
                        }
                        dbContext.SubmitChanges();
                    }
                }
                RadGridAbmeldung.Rebind();
                }
                   catch (Exception ex)
                   {
                       AbmeldungErrLabel.Visible = true;
                       AbmeldungErrLabel.Text = "Fehler: " + ex.Message;
                   }
            }
            else
            {
                AbmeldungErrLabel.Text = "Sie haben keinen Auftrag ausgewählt!";
                AbmeldungErrLabel.Visible = true;
            }
        }
        protected void ProductLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var productQuery = from prod in con.Product
                               select new
                               {
                                   ItemNumber = prod.ItemNumber,
                                   Name = prod.Name,
                                   Value = prod.Id,
                                   Category = prod.ProductCategory.Name
                               };
            e.Result = productQuery;
        }
        protected void CostCenterDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var costCenterQuery = from cost in con.CostCenter
                                  select new
                                  {
                                      Name = cost.Name,
                                      Value = cost.Id
                                  };
            e.Result = costCenterQuery;
        }
        // Ändert das Text von Button entweder nach Fehler oder Zulassung
        protected void ErrorCheckBox_Clicked(object sender, EventArgs e)
        {
            CheckBox errorCheckBox = sender as CheckBox;
            Button saveButton = errorCheckBox.FindControl("FertigStellenButton") as Button;
            if (errorCheckBox.Checked)
                saveButton.Text = "Als Fehler markieren";
            else
                saveButton.Text = "Speichern und zulassen";
        }
        protected void StornierenButton_Clicked(object sender, EventArgs e)
        {
            if (RadGridAbmeldung.SelectedItems.Count > 0)
            {
                AbmeldungErrLabel.Visible = false;
                StornierungErfolgLabel.Visible = false;
                foreach (GridDataItem item in RadGridAbmeldung.SelectedItems)
                {
                    var orderId = Int32.Parse(item["OrderNumber"].Text);
                    try
                    {
                        DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                        var newOrder = dbContext.Order.SingleOrDefault(q => q.Id == orderId);
                        //updating order status
                        newOrder.LogDBContext = dbContext;
                        newOrder.Status = (int)OrderItemState.Storniert;
                        //updating orderitems status                          
                        foreach (OrderItem ordItem in newOrder.OrderItem)
                        {
                            ordItem.LogDBContext = dbContext;
                            ordItem.Status = (int)OrderItemState.Storniert;
                        }
                        dbContext.SubmitChanges();
                        RadGridAbmeldung.Rebind();
                        StornierungErfolgLabel.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        AbmeldungErrLabel.Text = "Fehler" + ex.Message;
                        AbmeldungErrLabel.Visible = true;
                    }
                }
            }
            else
            {
                AbmeldungErrLabel.Visible = true;
            }
        }
    }
}