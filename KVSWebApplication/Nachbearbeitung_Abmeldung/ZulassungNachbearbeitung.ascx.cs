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
using KVSCommon.Enums;
using KVSWebApplication.BasePages;

namespace KVSWebApplication.Nachbearbeitung_Abmeldung
{
    /// <summary>
    ///Codebehind für die Maske (Reiter) Abmeldung Nachbearbeitung
    /// </summary>
    public partial class ZulassungNachbearbeitung : EditOrdersBase
    {
        #region Members  

        protected override RadGrid OrderGrid { get { return this.RadGridAbmeldung; } }
        protected override RadDatePicker RegistrationDatePicker { get { return null; } }
        protected override RadComboBox CustomerTypeDropDown { get { return this.RadComboCustomerAbmeldungZulassunsstelle; } }
        protected override RadComboBox CustomerDropDown { get { return this.CustomerDropDownListAbmeldungZulassunsstelle; } }
        protected override PermissionTypes PagePermission { get { return PermissionTypes.LOESCHEN_AUFTRAGSPOSITION; } }
        protected override OrderTypes OrderType { get { return OrderTypes.Cancellation; } }
        protected override OrderStatusTypes OrderStatusType { get { return OrderStatusTypes.AdmissionPoint; } }
        protected override string OrderStatusSearch { get { return "Zulassungsstelle"; } }
        #endregion

        #region Event handlers     

        protected void Page_Load(object sender, EventArgs e)
        {
            string target = Request["__EVENTTARGET"] == null ? "" : Request["__EVENTTARGET"];
            if (String.IsNullOrEmpty(target) && Session["orderNumberSearch"] != null && !String.IsNullOrEmpty(Session["orderNumberSearch"].ToString()))
                target = "IamFromSearch";

            StornierungErfolgLabel.Visible = false;
            if (Session["CustomerIndex"] != null)
            {
                if (target == null || (!target.Contains("CustomerDropDownListAbmeldungZulassunsstelle") &&
                    !target.Contains("CustomerDropDownListAbmeldungZulassunsstelle") && !target.Contains("NewPositionZulButton") && !target.Contains("StornierenButton")))
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

                        if (target.Contains("AuftragsbearbeitungAbmeldung") && target.Contains("EditButton"))
                            target = "Editbutton";

                        if (target.Contains("AuftragsbearbeitungAbmeldung") || target.Contains("RadTabStrip1") || target.Contains("IamFromSearch"))
                            RadGridAbmeldung.DataBind();
                    }
                }
            }
        }

        protected void RadGridAbmeldung_PreRender(object sender, EventArgs e)
        {
            HideExpandColumnRecursive(RadGridAbmeldung.MasterTableView);
        }

        protected void ShowAllButton_Click(object sender, EventArgs e)
        {
            CustomerDropDownListAbmeldungZulassunsstelle.ClearSelection();
            RadGridAbmeldung.Enabled = true;
            RadGridAbmeldung.Rebind();
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

            var editButton = sender as Button;
            var item = editButton.Parent as Panel;

            //update prices
            UpdateAllOrderItems(((RadGrid)item.FindControl("RadGridNeuzulassungDetails")).MasterTableView);


            //update order data
            (((GridNestedViewItem)(((GridTableCell)(item.Parent.Parent)).Parent))).ParentItem.Selected = true;
            var picker = item.FindControl("CreateDateBox") as RadDateTimePicker;
            var kennzeichenBox = item.FindControl("KennzeichenBox") as TextBox;
            var vinBox = item.FindControl("VINBox") as TextBox;
            var orderIdBox = item.FindControl("orderIdBox") as TextBox;

            var errorCheckBox = item.FindControl("ErrorZulCheckBox") as CheckBox;
            var errorReasonTextBox = item.FindControl("ErrorReasonZulTextBox") as TextBox;
            var HSNBox = item.FindControl("HSNAbmBox") as TextBox;
            var TSNBox = item.FindControl("TSNAbmBox") as TextBox;
            var amtGebBox = item.FindControl("AmtGebTextBox") as TextBox;
            ErrorZulLabel.Visible = false;
            kennzeichen = kennzeichenBox.Text.ToUpper();
            VIN = vinBox.Text;
            var orderNumber = Int32.Parse(orderIdBox.Text);

            HSN = HSNBox.Text;
            TSN = TSNBox.Text;
            if (errorCheckBox.Checked) // falls Auftrag als Fehler gemeldet sollte
            {
                string errorReason = errorReasonTextBox.Text;
                try
                {
                    var orderToUpdate = OrderManager.GetById(orderNumber);
                    orderToUpdate.HasError = true;
                    orderToUpdate.ErrorReason = errorReason;

                    OrderManager.SaveChanges();
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
                var order = OrderManager.GetById(orderNumber);
                foreach (var orderItem in order.OrderItem)
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
                        updateDataBase(kennzeichen, VIN, TSN, HSN, orderNumber);
                        UpdateOrderAndItemsStatus();
                        AbmeldungErfolgtLabel.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        AbmeldungErrLabel.Text = "Fehler:" + ex.Message;
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

        protected void UpdateAllOrderItems(GridTableView pricesGrid)
        {
            foreach (var item in pricesGrid.Items)
            {
                SaveOrderItemPrices(item as GridEditableItem);
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

            int? costCenterId = null;
            street = StreetTextBox.Text;
            streetNumber = StreetNumberTextBox.Text;
            zipcode = ZipcodeTextBox.Text;
            city = CityTextBox.Text;
            country = CountryTextBox.Text;
            invoiceRecipient = InvoiceRecipient.Text;
            int itemCount = 0;

            AbmeldungErrLabel.Visible = false;
            try
            {
                var newAdress = AdressManager.CreateAdress(street, streetNumber, zipcode, city, country);
                var customer = CustomerManager.GetById(customerId);
                var newInvoice = InvoiceManager.CreateInvoice(invoiceRecipient, newAdress, customerId, txbDiscount.Value, InvoiceType.Single);

                InvoiceIdHidden.Value = newInvoice.Id.ToString();

                var order = OrderManager.GetById(Int32.Parse(smallCustomerOrderHiddenField.Value));
                foreach (OrderItem ordItem in order.OrderItem)
                {
                    ProductName = ordItem.ProductName;
                    Amount = ordItem.Amount;

                    itemCount = ordItem.Count;

                    CostCenter costCenter = null;
                    if (costCenterId.HasValue)
                    {
                        costCenter = CostCenterManager.GetById(costCenterId.Value);
                    }

                    var newInvoiceItem = InvoiceManager.AddInvoiceItem(newInvoice, ProductName, Convert.ToDecimal(Amount), itemCount, ordItem, costCenter,
                        customer, OrderItemStatusTypes.Payed);
                }

                // Closing RadWindow
                string script = "function f(){$find(\"" + AddAdressRadWindow.ClientID + "\").close(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);

                Print(newInvoice);

                order.Status = (int)OrderStatusTypes.Payed;
                OrderManager.SaveChanges();

                RadGridAbmeldung.Rebind();
            }
            catch (Exception ex)
            {
                AbmeldungErrLabel.Text = " Fehler:" + ex.Message;
                AbmeldungErrLabel.Visible = true;
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
                    SaveOrderItemPrices(e.Item as GridEditableItem);
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

        protected void SaveOrderItemPrices(GridEditableItem editedItem)
        {
            var tbEditPrice = editedItem["ColumnPrice"].FindControl("tbEditPrice") as RadTextBox;
            string itemId = editedItem["ItemIdColumn"].Text;
            var tbAuthPrice = editedItem["AuthCharge"].FindControl("tbAuthChargePrice") as RadTextBox;

            int? authChargeId = null;
            if (!String.IsNullOrEmpty(editedItem["AuthChargeId"].Text))
            {
                authChargeId = Int32.Parse(editedItem["AuthChargeId"].Text);
            }

            if (OrderManager.GenerateAuthCharge(authChargeId, Int32.Parse(itemId), tbAuthPrice.Text))
            {
                tbAuthPrice.ForeColor = System.Drawing.Color.Green;
            }

            UpdatePosition(itemId, tbEditPrice.Text);
            tbEditPrice.ForeColor = System.Drawing.Color.Green;
        }

        // Automatische Suche nach HSN
        protected void HSNBox_TextChanged(object sender, EventArgs e)
        {
            var hsnTextBox = sender as RadTextBox;
            var hsnLabel = hsnTextBox.Parent.FindControl("HSNSearchLabel") as Label;
            var tsnBox = hsnTextBox.Parent.FindControl("TSNAbmBox") as TextBox;
            hsnLabel.Text = "";
            if (!String.IsNullOrEmpty(hsnTextBox.Text))
            {
                hsnLabel.Visible = true;
                hsnLabel.Text = Make.GetMakeByHSN(hsnTextBox.Text);
            }
            tsnBox.Focus();
        }
               
        //Row Index wird in hiddenfield gespeichert
        protected void Edit_Command(object sender, GridCommandEventArgs e)
        {
            var button = sender as RadButton;
            var dataItem = e.Item as GridDataItem;
            dataItem.Selected = true;
            itemIndexHiddenField.Value = dataItem.ItemIndex.ToString();
        }

        protected void RadGridAbmeldung_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            var item = (GridDataItem)e.DetailTableView.ParentItem;
            var nestedItem = (GridNestedViewItem)item.ChildItem;
            var radGrdEnquiriesVarients = (RadGrid)nestedItem.FindControl("RadGridNeuzulassungDetails");
            radGrdEnquiriesVarients.DataSource = GetOrderPositions(item["OrderNumber"].Text);
            radGrdEnquiriesVarients.DataBind();
        }

        protected void NewPositionButton_Clicked(object sender, EventArgs e)
        {
            if (RadGridAbmeldung.SelectedItems.Count > 0)
            {
                try
                {
                    AbmeldungErrLabel.Visible = false;
                    Button button = sender as Button;
                    int locationId = 0;
                    Price newPrice = null;

                    var productDropDown = button.NamingContainer.FindControl("NewProductDropDownList") as RadComboBox;
                    var costCenterDropDown = button.NamingContainer.FindControl("CostCenterDropDownList") as DropDownList;
                    var product = Int32.Parse(productDropDown.SelectedValue);

                    foreach (GridDataItem item in RadGridAbmeldung.SelectedItems)
                    {
                        var orderNumber = Int32.Parse(item["OrderNumber"].Text);
                        var newProduct = ProductManager.GetById(Int32.Parse(productDropDown.SelectedValue));

                        if (CustomerDropDownListAbmeldungZulassunsstelle.SelectedValue == "2") // if small customer
                        {
                            locationId = Int32.Parse(item["locationId"].Text);
                            newPrice = PriceManager.GetEntities(q => q.ProductId == newProduct.Id && q.LocationId == locationId).SingleOrDefault();
                        }

                        if (newPrice == null)
                        {
                            newPrice = PriceManager.GetEntities(q => q.ProductId == newProduct.Id && q.LocationId == null).SingleOrDefault();
                        }

                        var orderToUpdate = OrderManager.GetById(orderNumber);

                        if (newPrice == null || newProduct == null || orderToUpdate == null)
                            throw new Exception("Achtung, die Position kann nicht hinzugefügt werden, es konnte entweder kein Preis, Produkt oder der Auftrag gefunden werden!");
                        
                        if (orderToUpdate != null)
                        {
                            var orderItemCostCenter = orderToUpdate.OrderItem.FirstOrDefault(q => q.CostCenter != null);

                            var newOrderItem = OrderManager.AddOrderItem(orderToUpdate, newProduct.Id, newPrice.Amount, 1, (orderItemCostCenter != null) ? orderItemCostCenter.CostCenter : null,
                                null, false);

                            if (newPrice.AuthorativeCharge.HasValue)
                            {
                                OrderManager.AddOrderItem(orderToUpdate, newProduct.Id, newPrice.AuthorativeCharge.Value, 1, (orderItemCostCenter != null) ? orderItemCostCenter.CostCenter : null,
                                    newOrderItem.Id, newPrice.AuthorativeCharge.HasValue);
                            }
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

        // Ändert das Text von Button entweder nach Fehler oder Zulassung
        protected void ErrorCheckBox_Clicked(object sender, EventArgs e)
        {
            var errorCheckBox = sender as CheckBox;
            var saveButton = errorCheckBox.FindControl("FertigStellenButton") as Button;
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
                    var orderNumber = Int32.Parse(item["OrderNumber"].Text);
                    try
                    {
                        var newOrder = OrderManager.GetById(orderNumber);
                        //updating order status
                        newOrder.Status = (int)OrderStatusTypes.Cancelled;
                        //updating orderitems status                          
                        foreach (OrderItem ordItem in newOrder.OrderItem)
                        {
                            ordItem.Status = (int)OrderItemStatusTypes.Cancelled;
                        }

                        OrderManager.SaveChanges();
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

        #endregion

        #region Methods

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

        protected override void CheckOpenedOrders()
        {
            var count = GetUnfineshedOrdersCount(OrderTypes.Cancellation, OrderStatusTypes.AdmissionPoint);
            ordersCount.Text = count.ToString();

            if (count == 0)
            {
                go.Visible = false;
            }
            else
            {
                go.Visible = true;
            }
        }

        protected override void SmallCustomerOrdersFunctions()
        {
            RadGridAbmeldung.Columns.FindByUniqueName("CustomerLocation").Visible = false;
        }

        // Updating Order und OrderItems auf Status 600 - Abgeschlossen
        protected void updateDataBase(string kennzeichen, string vin, string tsn, string hsn, int orderNumber)
        {
            var order = OrderManager.GetById(orderNumber);
            order.DeregistrationOrder.Registration.Licencenumber = kennzeichen;
            order.DeregistrationOrder.Vehicle.VIN = vin;
            order.DeregistrationOrder.Vehicle.TSN = tsn;
            order.DeregistrationOrder.Vehicle.HSN = hsn;

            OrderManager.SaveChanges();
        }

        //falls benötigt, wird der Status per Email gesendet
        protected void SendStatusByEmail(int customerId, Order orderToSend)
        {
            var customer = CustomerManager.GetById(customerId);
            string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
            string smtp = ConfigurationManager.AppSettings["smtpHost"];


            if (customer.LargeCustomer != null && customer.LargeCustomer.OrderFinishedNoteSendType == 1) //sofort gesendet
            {
                try
                {
                    OrderManager.SendOrderFinishedNote(orderToSend, fromEmail, smtp);
                }
                catch
                {
                    throw new Exception("Email mit OrderStatus konnte nicht gesendet werden.");
                }
            }
        }
        
        protected void Print(Invoice newInvoice)
        {
            using (MemoryStream memS = new MemoryStream())
            {
                string serverPath = ConfigurationManager.AppSettings["DataPath"] + "\\UserData";

                InvoiceItemAccountItemManager.CreateAccounts(newInvoice);
                InvoiceManager.Print(newInvoice, memS, "", "");

                string fileName = "Rechnung_" + newInvoice.InvoiceNumber.Number + "_" + newInvoice.CreateDate.Day + "_" + newInvoice.CreateDate.Month + "_" + newInvoice.CreateDate.Year + ".pdf";

                if (!Directory.Exists(serverPath))
                    Directory.CreateDirectory(serverPath);

                if (!Directory.Exists(serverPath + "\\" + Session["CurrentUserId"].ToString()))
                    Directory.CreateDirectory(serverPath + "\\" + Session["CurrentUserId"].ToString());

                serverPath = serverPath + "\\" + Session["CurrentUserId"].ToString();
                File.WriteAllBytes(serverPath + "\\" + fileName, memS.ToArray());
                string url = ConfigurationManager.AppSettings["BaseUrl"];
                string path = url + "UserData/" + Session["CurrentUserId"].ToString() + "/" + fileName;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Invoice", "<script>openFile('" + path + "');</script>", false);
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
                    OrderManager.UpdateOrderItemAmount(Int32.Parse(itemId), Convert.ToDecimal(amoutToSave));
                }
                catch (Exception ex)
                {
                    throw new Exception("Die ausgewählte Position kann nicht updatet werden <br /> Error: " + ex.Message);
                }
            }
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

                    var orderNumber = Int32.Parse(item["OrderNumber"].Text);
                    smallCustomerOrderHiddenField.Value = orderNumber.ToString();
                    CustomerIdHiddenField.Value = customerID.ToString();
                    try
                    {
                        var newOrder = OrderManager.GetById(orderNumber);
                        if (newOrder != null)
                        {
                            if (RadComboCustomerAbmeldungZulassunsstelle.SelectedIndex == 1) // small
                            {
                                //updating order status
                                newOrder.Status = (int)OrderStatusTypes.Closed;
                                newOrder.ExecutionDate = DateTime.Now;
                                //updating orderitems status                          
                                foreach (OrderItem ordItem in newOrder.OrderItem)
                                {
                                    if (ordItem.Status != (int)OrderItemStatusTypes.Cancelled)
                                    {
                                        ordItem.Status = (int)OrderItemStatusTypes.Closed;
                                    }
                                }

                                OrderManager.SaveChanges();

                                //opening window for adress
                                string script = "function f(){$find(\"" + AddAdressRadWindow.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
                                SetValuesForAdressWindow(customerID);
                            }
                            else //large
                            {
                                //updating order status
                                newOrder.Status = (int)OrderStatusTypes.Closed;
                                newOrder.ExecutionDate = DateTime.Now;
                                //updating orderitems status                          
                                foreach (OrderItem ordItem in newOrder.OrderItem)
                                {
                                    if (ordItem.Status != (int)OrderItemStatusTypes.Cancelled)
                                    {
                                        ordItem.Status = (int)OrderItemStatusTypes.Closed;
                                    }
                                }

                                OrderManager.SaveChanges();
                                SendStatusByEmail(customerID, newOrder);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AbmeldungErrLabel.Text = "Fehler: " + ex.Message;
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
            var location = CustomerManager.GetEntities(o => o.Id == customerId).
                Select(o => o.InvoiceAdress).FirstOrDefault();

            StreetTextBox.Text = location.Street;
            StreetNumberTextBox.Text = location.StreetNumber;
            ZipcodeTextBox.Text = location.Zipcode;
            CityTextBox.Text = location.City;
            CountryTextBox.Text = location.Country;
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

        #endregion
    }
}