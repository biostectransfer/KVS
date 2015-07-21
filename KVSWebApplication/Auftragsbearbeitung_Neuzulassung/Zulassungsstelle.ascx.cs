using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.Collections;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using System.Transactions;
namespace KVSWebApplication.Auftragsbearbeitung_Neuzulassung
{
    public partial class Zulassungsstelle : System.Web.UI.UserControl
    {
        private string customer = string.Empty;
        RadScriptManager script = null;
        public bool comeFromOrder
        {
            set;
            get;
        }
        protected void RadGridNeuzulassung_PreRender(object sender, EventArgs e)
        {

            HideExpandColumnRecursive(RadGridNeuzulassung.MasterTableView);
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
            if (comeFromOrder == true)
            {
                KVSWebApplication.Auftragseingang.ZulassungLaufkunde auftrag = Page as KVSWebApplication.Auftragseingang.ZulassungLaufkunde;
                RadScriptManager script = auftrag.getScriptManager() as RadScriptManager;
                script.RegisterPostBackControl(AddAdressButton);
            }
            else
            {
                AuftragsbearbeitungNeuzulassung auftragNeu = Page as AuftragsbearbeitungNeuzulassung;
                script = auftragNeu.getScriptManager() as RadScriptManager;
                script.RegisterPostBackControl(AddAdressButton);
            }
            CheckOpenedOrders();
            string target = Request["__EVENTTARGET"] == null ? "" : Request["__EVENTTARGET"];
            ZulassungErfolgtLabel.Visible = false;
            ZulassungErrLabel.Visible = false;
            if (String.IsNullOrEmpty(target))
                if (Session["orderNumberSearch"] != null)
                    if (!String.IsNullOrEmpty(Session["orderNumberSearch"].ToString()))
                        target = "IamFromSearch";
            StornierungErfolgLabel.Visible = false;
            if (Session["CustomerIndex"] != null)
            {
                if (!target.Contains("RadComboBoxCustomerZulassungsstelle") && !target.Contains("CustomerDropDownListZulassungsstelle") && !target.Contains("NewPositionZulButton") && !target.Contains("StornierenButton"))
                {
                    if (RadComboBoxCustomerZulassungsstelle.Items.Count() > 0)
                        RadComboBoxCustomerZulassungsstelle.SelectedValue = Session["CustomerIndex"].ToString();
                    // CustomerDropDownListZulassungsstelle.DataBind();
                    if (Session["CustomerId"] != null)
                    {
                        if (CustomerDropDownListZulassungsstelle.Items.Count() > 0)
                            CustomerDropDownListZulassungsstelle.SelectedValue = Session["CustomerId"].ToString();

                        RadGridNeuzulassung.Enabled = true;

                        if (target.Contains("OffenNeuzulassung") || target.Contains("RadTabStripNeuzulassung") || target.Contains("IamFromSearch") || target.Contains("ShowAll"))
                            RadGridNeuzulassung.DataBind();
                    }
                }
            }
        }
        protected void CheckOpenedOrders()
        {
            ordersCount.Text = Order.getUnfineshedOrdersCount(new DataClasses1DataContext(), "Zulassung", 400).ToString();
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
            if (RadComboBoxCustomerZulassungsstelle.SelectedValue == "1") // Small
            {
                DataClasses1DataContext con = new DataClasses1DataContext();
                RadGridNeuzulassung.Columns.FindByUniqueName("CustomerLocation").Visible = false;

                var smallCustomerQuery = from ord in con.Order
                                         join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                         join cust in con.Customer on ord.CustomerId equals cust.Id
                                         join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                         join regord in con.RegistrationOrder on ord.Id equals regord.OrderId
                                         join reg in con.Registration on regord.RegistrationId equals reg.Id
                                         join veh in con.Vehicle on regord.VehicleId equals veh.Id
                                         join smc in con.SmallCustomer on cust.Id equals smc.CustomerId
                                         orderby ord.Ordernumber descending
                                         where ord.Status == 400 && ordtype.Name == "Zulassung" && ord.HasError.GetValueOrDefault(false) != true
                                         select new
                                         {
                                             OrderId = ord.Id,
                                             locationId = "",
                                             OrderNumber = ord.Ordernumber,
                                             CreateDate = ord.CreateDate,
                                             customerID = ord.CustomerId,
                                             Status = ordst.Name,
                                             CustomerName = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + "  " + cust.SmallCustomer.Person.Name : cust.Name,
                                             Kennzeichen = reg.Licencenumber,
                                             VIN = veh.VIN,
                                             TSN = veh.TSN,
                                             HSN = veh.HSN,
                                             CustomerLocation = "",
                                             OrderTyp = ordtype.Name,
                                             Freitext = ord.FreeText
                                         };
                if (CustomerDropDownListZulassungsstelle.SelectedValue != string.Empty)
                {
                    var custId = Int32.Parse(CustomerDropDownListZulassungsstelle.SelectedValue);
                    smallCustomerQuery = smallCustomerQuery.Where(q => q.customerID == custId);
                }
                e.Result = smallCustomerQuery;
            }
            //select all values for large customers
            else if (RadComboBoxCustomerZulassungsstelle.SelectedValue == "2") // Large
            {
                DataClasses1DataContext con = new DataClasses1DataContext();
                var largeCustomerQuery1 = from ord in con.Order
                                          join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                          join cust in con.Customer on ord.CustomerId equals cust.Id
                                          join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                          join loc in con.Location on ord.LocationId equals loc.Id
                                          join regord in con.RegistrationOrder on ord.Id equals regord.OrderId
                                          join reg in con.Registration on regord.RegistrationId equals reg.Id
                                          join veh in con.Vehicle on regord.VehicleId equals veh.Id
                                          join lmc in con.LargeCustomer on cust.Id equals lmc.CustomerId
                                          orderby ord.Ordernumber descending
                                          where ord.Status == 400 && ordtype.Name == "Zulassung" && ord.HasError.GetValueOrDefault(false) != true
                                          select new
                                          {
                                              OrderId = ord.Id,
                                              locationId = loc.Id,
                                              customerID = ord.CustomerId,
                                              OrderNumber = ord.Ordernumber,
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
                if (CustomerDropDownListZulassungsstelle.SelectedValue != string.Empty)
                {
                    var custId = Int32.Parse(CustomerDropDownListZulassungsstelle.SelectedValue);
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
                CheckOpenedOrders();
            }
        }
        protected void ShowAllButton_Click(object sender, EventArgs e)
        {
            CustomerDropDownListZulassungsstelle.ClearSelection();
            RadGridNeuzulassung.Enabled = true;
            RadGridNeuzulassung.Rebind();
        }
        // Small oder Large -> Auswahl der KundenName
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            if (RadComboBoxCustomerZulassungsstelle.SelectedValue == "1") //Small Customers
            {
                var customerQuery = from cust in con.Customer
                                    where cust.Id == cust.SmallCustomer.CustomerId
                                    select new
                                    {
                                        Name = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name,
                                        Value = cust.Id,
                                        Matchcode = cust.MatchCode,
                                        Kundennummer = cust.CustomerNumber
                                    };
                e.Result = customerQuery;
            }
            else if (RadComboBoxCustomerZulassungsstelle.SelectedValue == "2") //Large Customers
            {
                var customerQuery = from cust in con.Customer
                                    where cust.Id == cust.LargeCustomer.CustomerId
                                    select new { Name = cust.Name, Value = cust.Id, Matchcode = cust.MatchCode, Kundennummer = cust.CustomerNumber };
                e.Result = customerQuery;
            }
        }
        // Large oder small Customer
        protected void SmallLargeCustomerIndex_Changed(object sender, EventArgs e)
        {
            CustomerDropDownListZulassungsstelle.Enabled = true;
            this.CustomerDropDownListZulassungsstelle.DataBind();
            this.RadGridNeuzulassung.DataBind();
            Session["CustomerIndex"] = RadComboBoxCustomerZulassungsstelle.SelectedValue;
        }
        // Auswahl von Kunde
        protected void CustomerIndex_Changed(object sender, EventArgs e)
        {
            this.RadGridNeuzulassung.Enabled = true;
            this.RadGridNeuzulassung.DataBind();
            Session["CustomerIndex"] = RadComboBoxCustomerZulassungsstelle.SelectedValue;
            Session["CustomerId"] = CustomerDropDownListZulassungsstelle.SelectedValue;
        }
        // Automatische Suche nach HSN
        protected void HSNBox_TextChanged(object sender, EventArgs e)
        {
            TextBox hsnTextBox = sender as TextBox;
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
        //Checked if amt.gebühr UND mind.eine Dienstleistung vorhanden ist
        protected void AmtGebuhrCheckBoxZulDataBinding(object sender, EventArgs e)
        {
            CheckBox amtGebCheckBox = sender as CheckBox;
            using (DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())))
            {
                TextBox orderIdBox = amtGebCheckBox.FindControl("orderIdBox") as TextBox;
                Label dienstleistungLabel = amtGebCheckBox.FindControl("DienstleisungZulLabel") as Label;
                dienstleistungLabel.Text = "";
                CheckBox dienstleistungVorhandenCheckBox = amtGebCheckBox.FindControl("DiensleistungVorhandenZulCheckBox") as CheckBox;
                dienstleistungVorhandenCheckBox.Checked = false;
                TextBox amtGebBox = amtGebCheckBox.FindControl("AmtGebTextBox") as TextBox;
                Label amtGebLabel = amtGebCheckBox.FindControl("AmtGebLabel") as Label;
                amtGebBox.Visible = false;
                amtGebLabel.Visible = false;
                var orderId = Int32.Parse(orderIdBox.Text);
                var searchOrderQuery = dbContext.Order.SingleOrDefault(q => q.Id == orderId);
                if (searchOrderQuery != null)
                {
                    foreach (OrderItem item in searchOrderQuery.OrderItem)
                    {
                        if (item.IsAuthorativeCharge == true)
                        {
                            amtGebCheckBox.Checked = true;
                        }
                        else if (item.IsAuthorativeCharge == false)
                        {
                            dienstleistungLabel.Text += item.ProductName;
                            dienstleistungVorhandenCheckBox.Checked = true;
                            orderitemidHiddenField.Value = item.Id.ToString();
                        }
                    }
                }
                if (!amtGebCheckBox.Checked)
                {
                    amtGebBox.Visible = true;
                    amtGebLabel.Visible = true;
                }
            }
        }
        protected void OnItemCommand_Fired(object sender, GridCommandEventArgs e)
        {
            try
            {
                ZulassungErrLabel.Text = "";
                ZulassungErrLabel.Visible = false;
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
                ZulassungErrLabel.Text = "Fehler: " + ex.Message;
                ZulassungErrLabel.Visible = true;
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
                var orderItemId = Int32.Parse(itemId);
                try
                {
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
        protected void RadGridNeuzulassung_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            var dbContext = new DataClasses1DataContext();
            var item = (GridDataItem)e.DetailTableView.ParentItem;
            var orderId = Int32.Parse(item["OrderId"].Text);
            var positionQuery = from ord in dbContext.Order
                                join orditem in dbContext.OrderItem on ord.Id equals orditem.OrderId
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
        protected void AuftragFertigStellen_Command(object sender, EventArgs e)
        {
            string kennzeichen = string.Empty,
                VIN = string.Empty,
                hsn = string.Empty,
                tsn = string.Empty;
            int customerId = 0;
            Button editButton = sender as Button;
            Panel item = editButton.Parent as Panel;
            (((GridNestedViewItem)(((GridTableCell)(item.Parent.Parent)).Parent))).ParentItem.Selected = true;
            TextBox kennzeichenBox = item.FindControl("KennzeichenBox") as TextBox;
            TextBox vinBox = item.FindControl("VINBox") as TextBox;
            TextBox orderIdBox = item.FindControl("orderIdBox") as TextBox;
            TextBox TSNBox = item.FindControl("TSNAbmBox") as TextBox;
            TextBox HSNBox = item.FindControl("HSNAbmBox") as TextBox;
            CheckBox errorCheckBox = item.FindControl("ErrorZulCheckBox") as CheckBox;
            TextBox errorReasonTextBox = item.FindControl("ErrorReasonZulTextBox") as TextBox;
            TextBox amtGebBox = item.FindControl("AmtGebTextBox") as TextBox;
            kennzeichen = kennzeichenBox.Text.ToUpper();
            VIN = vinBox.Text;
            var orderId = Int32.Parse(orderIdBox.Text);
            if (!String.IsNullOrEmpty(CustomerDropDownListZulassungsstelle.SelectedValue.ToString()))
                customerId = Int32.Parse(CustomerDropDownListZulassungsstelle.SelectedValue);
            else
            {
                TextBox customerid = item.FindControl("customerIdBox") as TextBox;
                customerId = Int32.Parse(customerid.Text);
            }
            tsn = TSNBox.Text;
            hsn = HSNBox.Text;
            ZulassungErfolgtLabel.Visible = false;
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
                    RadGridNeuzulassung.MasterTableView.ClearChildEditItems();
                    RadGridNeuzulassung.MasterTableView.ClearEditItems();
                    RadGridNeuzulassung.Rebind();
                }
                catch (Exception ex)
                {
                    ZulassungErrLabel.Text = "Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator <br /> Error:" + ex.Message;
                    ZulassungErfolgtLabel.Visible = true;
                }
            }
            else  // falls normales Update 
            {
                bool amtlGebVor = false;
                bool kennzeichenVorh = false;
                DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                var myOrder = dbContext.Order.SingleOrDefault(q => q.Id == orderId && q.CustomerId == customerId);
                foreach (var orderItem in myOrder.OrderItem)
                {
                    if (orderItem.IsAuthorativeCharge)
                        amtlGebVor = true;
                }
                if (String.IsNullOrEmpty(kennzeichen))
                {
                    ZulassungErrLabel.Text = "Bitte geben Sie die Kennzeichen ein!";
                    ZulassungErrLabel.Visible = true;
                }
                else if (!amtlGebVor)
                {
                    ZulassungErrLabel.Text = "Bitte fügen Sie eine Amtl.Gebühr hinzu!";
                    ZulassungErrLabel.Visible = true;
                }
                else
                {
                    ZulassungErrLabel.Visible = false;
                    try
                    {
                        updateDataBase(kennzeichen, VIN, tsn, hsn, orderId, customerId);
                        UpdateOrderAndItemsStatus();
                        ZulassungErfolgtLabel.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        ZulassungErrLabel.Text = "Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator <br /> Error: " + ex.Message;
                        ZulassungErfolgtLabel.Visible = true;
                    }
                }
            }
            if (Session["orderNumberSearch"] != null)
                Session["orderNumberSearch"] = string.Empty; //after search should be empty
            RadGridNeuzulassung.MasterTableView.ClearChildEditItems();
            RadGridNeuzulassung.MasterTableView.ClearEditItems();
            RadGridNeuzulassung.Rebind();
            CheckOpenedOrders();
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
        // Updating Status 600 für Order und OrderItems
        protected void UpdateOrderAndItemsStatus()
        {
            if (RadGridNeuzulassung.SelectedItems.Count > 0)
            {
                ZulassungErrLabel.Visible = false;
                ZulassungErfolgtLabel.Visible = false;
                foreach (GridDataItem item in RadGridNeuzulassung.SelectedItems)
                {
                    // Vorbereitung für Update
                    int customerID;
                    if (!String.IsNullOrEmpty(CustomerDropDownListZulassungsstelle.SelectedValue.ToString()))
                        customerID = Int32.Parse(CustomerDropDownListZulassungsstelle.SelectedValue);
                    else
                        customerID = Int32.Parse(item["customerID"].Text);
                    
                    var orderId = Int32.Parse(item["OrderId"].Text);
                    
                    smallCustomerOrderHiddenField.Value = orderId.ToString();
                    CustomerIdHiddenField.Value = customerID.ToString();
                    try
                    {
                        DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                        var newOrder = dbContext.Order.Single(q => q.CustomerId == customerID && q.Id == orderId);
                        if (newOrder != null)
                        {
                            if (RadComboBoxCustomerZulassungsstelle.SelectedIndex == 1) // small
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
                    catch (Exception ex)
                    {
                        ZulassungErrLabel.Text = "Fehler: " + ex.Message;
                        ZulassungErrLabel.Visible = true;
                    }
                }
                // erfolgreich
                RadGridNeuzulassung.DataBind();
                ZulassungErfolgtLabel.Visible = true;
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
            LocationLabelWindow.Text = "Fügen Sie bitte die Adresse für " + CustomerDropDownListZulassungsstelle.Text + " hinzu";
            ZusatzlicheInfoLabel.Visible = false;
            if (RadComboBoxCustomerZulassungsstelle.SelectedIndex == 1) // small
            {
                ZusatzlicheInfoLabel.Visible = true;
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
            if (!String.IsNullOrEmpty(CustomerDropDownListZulassungsstelle.SelectedValue))
            {
                customerId = Int32.Parse(CustomerDropDownListZulassungsstelle.SelectedValue);
            }
            else if (!String.IsNullOrEmpty(CustomerIdHiddenField.Value))
            {
                customerId = Int32.Parse(CustomerIdHiddenField.Value);
            }

            street = StreetTextBox.Text;
            streetNumber = StreetNumberTextBox.Text;
            zipcode = ZipcodeTextBox.Text;
            city = CityTextBox.Text;
            country = CountryTextBox.Text;
            invoiceRecipient = InvoiceRecipient.Text;
            int itemCount = 0;
            TransactionScope scope = null;
            ZulassungErrLabel.Visible = false;
            try
            {
                using (DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())))
                {
                    using (scope = new TransactionScope())
                    {
                        var newAdress = Adress.CreateAdress(street, streetNumber, zipcode, city, country, dbContext);
                        var myCustomer = dbContext.Customer.FirstOrDefault(q => q.Id == customerId);
                        var newInvoice = Invoice.CreateInvoice(dbContext, Int32.Parse(Session["CurrentUserId"].ToString()), invoiceRecipient, newAdress, customerId, 
                            txbDiscount.Value, "Einzelrechnung");
                        InvoiceIdHidden.Value = newInvoice.Id.ToString();
                        //Submiting new Invoice and Adress
                        dbContext.SubmitChanges();
                        var orderQuery = dbContext.Order.SingleOrDefault(q => q.Id == Int32.Parse(smallCustomerOrderHiddenField.Value));
                        foreach (OrderItem ordItem in orderQuery.OrderItem)
                        {
                            ProductName = ordItem.ProductName;
                            Amount = ordItem.Amount;

                            CostCenter costCenter = null;
                            if (ordItem.CostCenterId.HasValue)//TODO && ordItem.CostCenterId.ToString().Length > 8)
                            {
                                costCenter = dbContext.CostCenter.FirstOrDefault(o => o.Id == ordItem.CostCenterId.Value);
                            }

                            itemCount = ordItem.Count;
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
                    RadGridNeuzulassung.Rebind();
                }
            }
            catch (Exception ex)
            {
                ZulassungErrLabel.Text = " Fehler:" + ex.Message;
                ZulassungErrLabel.Visible = true;
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
        //Prüfen ob alle Werte da sind um den Auftrag auf "Zulassungstelle" zu setzen
        private bool CheckIfAllExistsToUpdate()
        {
            bool shouldBeUpdated = true;
            ZulassungErrLabel.Visible = false;

            foreach (GridDataItem item in RadGridNeuzulassung.SelectedItems)
            {
                if (String.IsNullOrEmpty(item["VIN"].Text))
                {
                    shouldBeUpdated = false;
                    ZulassungErrLabel.Text = "Bitte fügen Sie FIN ein";
                    ZulassungErrLabel.Visible = true;
                }
                if (String.IsNullOrEmpty(item["TSN"].Text))
                {
                    shouldBeUpdated = false;
                    ZulassungErrLabel.Text = "Bitte fügen Sie TSN ein";
                    ZulassungErrLabel.Visible = true;
                }
                if (String.IsNullOrEmpty(item["HSN"].Text))
                {
                    shouldBeUpdated = false;
                    ZulassungErrLabel.Text = "Bitte fügen Sie HSN ein";
                    ZulassungErrLabel.Visible = true;
                }
                if (String.IsNullOrEmpty(item["CustomerLocation"].Text))
                {
                    shouldBeUpdated = false;
                    ZulassungErrLabel.Text = "Bitte fügen Sie Standort ein";
                    ZulassungErrLabel.Visible = true;
                }
            }
            return shouldBeUpdated;
        }
        protected void NewPositionButton_Clicked(object sender, EventArgs e)
        {
            if (RadGridNeuzulassung.SelectedItems.Count > 0)
            {
                try
                {
                    ZulassungErrLabel.Visible = false;
                    DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                    Button button = sender as Button;

                    Price newPrice = null;
                    OrderItem newOrderItem1 = null;
                    var productDropDown = button.NamingContainer.FindControl("NewProductDropDownList") as RadComboBox;
                    var costCenterDropDown = button.NamingContainer.FindControl("CostCenterDropDownList") as DropDownList;
                    var product = Int32.Parse(productDropDown.SelectedValue);
                    foreach (GridDataItem item in RadGridNeuzulassung.SelectedItems)
                    {
                        var orderId = Int32.Parse(item["OrderId"].Text);
                        KVSCommon.Database.Product newProduct = dbContext.Product.SingleOrDefault(q => q.Id == Int32.Parse(productDropDown.SelectedValue));
                        
                        //TODO newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == locationId);
                        
                        if (!String.IsNullOrEmpty(item["locationId"].Text))//TODO && item["locationId"].Text.Length > 6)
                        {
                            var locationId = Int32.Parse(item["locationId"].Text);
                            newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == locationId);
                        }
                        
                        if (newPrice == null || String.IsNullOrEmpty(item["locationId"].Text))
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

                            newOrderItem1 = orderToUpdate.AddOrderItem(newProduct.Id, newPrice.Amount, 1, (orderItemCostCenter != null) ? orderItemCostCenter.CostCenter : null, null, false, dbContext);
                            if (newPrice.AuthorativeCharge.HasValue)
                            {
                                orderToUpdate.AddOrderItem(newProduct.Id, newPrice.AuthorativeCharge.Value, 1, (orderItemCostCenter != null) ? orderItemCostCenter.CostCenter : null,
                                    newOrderItem1.Id, newPrice.AuthorativeCharge.HasValue, dbContext);
                            }
                            dbContext.SubmitChanges();
                        }
                    }
                    RadGridNeuzulassung.Rebind();
                }
                catch (Exception ex)
                {
                    ZulassungErrLabel.Visible = true;
                    ZulassungErrLabel.Text = "Fehler: " + ex.Message;
                }
            }
            else
            {
                ZulassungErrLabel.Visible = true;
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
                                  orderby cost.Name
                                  select new
                                  {
                                      Name = cost.Name,
                                      Value = cost.Id
                                  };
            e.Result = costCenterQuery;
        }
        //Row Index wird in hiddenfield gespeichert
        protected void Edit_Command(object sender, GridCommandEventArgs e)
        {
            var button = sender as RadButton;
            GridDataItem dataItem = e.Item as GridDataItem;
            dataItem.Selected = true;
            itemIndexHiddenField.Value = dataItem.ItemIndex.ToString();
        }
        // Updating Order und setzen Status 600 - Fertig
        protected void updateDataBase(string kennzeichen, string vin, string tsn, string hsn, int orderId, int customerId)
        {
            using (DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())))
            {
                var orderUpdateQuery = dbContext.Order.Single(q => q.Id == orderId && q.CustomerId == customerId);
                orderUpdateQuery.LogDBContext = dbContext;
                orderUpdateQuery.RegistrationOrder.LogDBContext = dbContext;
                orderUpdateQuery.RegistrationOrder.Registration.LogDBContext = dbContext;
                orderUpdateQuery.RegistrationOrder.Vehicle.LogDBContext = dbContext;
                orderUpdateQuery.RegistrationOrder.Registration.Licencenumber = kennzeichen;
                orderUpdateQuery.RegistrationOrder.Vehicle.VIN = vin;
                orderUpdateQuery.RegistrationOrder.Vehicle.TSN = tsn;
                orderUpdateQuery.RegistrationOrder.Vehicle.HSN = hsn;
                dbContext.SubmitChanges();
            }
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
            if (RadGridNeuzulassung.SelectedItems.Count > 0)
            {
                ZulassungErrLabel.Visible = false;
                StornierungErfolgLabel.Visible = false;
                foreach (GridDataItem item in RadGridNeuzulassung.SelectedItems)
                {
                    var orderId = Int32.Parse(item["OrderId"].Text);
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
                        RadGridNeuzulassung.Rebind();
                        StornierungErfolgLabel.Visible = true;
                    }
                    catch
                    {
                        ZulassungErrLabel.Text = "Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator";
                        ZulassungErrLabel.Visible = true;
                    }
                }
            }
            else
            {
                ZulassungErrLabel.Visible = true;
            }
        }
    }
}