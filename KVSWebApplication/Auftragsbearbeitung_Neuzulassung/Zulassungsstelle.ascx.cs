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
using KVSCommon.Enums;
using KVSWebApplication.Auftragseingang;
using KVSWebApplication.BasePages;

namespace KVSWebApplication.Auftragsbearbeitung_Neuzulassung
{
    public partial class Zulassungsstelle : EditOrdersBase
    {
        #region Members  
        
        public bool comeFromOrder { set; get; }

        protected override RadGrid OrderGrid { get { return this.RadGridNeuzulassung; } }
        protected override RadDatePicker RegistrationDatePicker { get { return null; } }
        protected override RadComboBox CustomerTypeDropDown { get { return this.RadComboBoxCustomerZulassungsstelle; } }
        protected override RadComboBox CustomerDropDown { get { return this.CustomerDropDownListZulassungsstelle; } }
        protected override PermissionTypes PagePermission { get { return PermissionTypes.LOESCHEN_AUFTRAGSPOSITION; } }
        protected override OrderTypes OrderType { get { return OrderTypes.Admission; } }
        protected override OrderStatusTypes OrderStatusType { get { return OrderStatusTypes.AdmissionPoint; } }
        protected override string OrderStatusSearch { get { return "Zulassungsstelle"; } }

        #endregion

        #region Event handlers

        protected void RadGridNeuzulassung_PreRender(object sender, EventArgs e)
        {
            HideExpandColumnRecursive(RadGridNeuzulassung.MasterTableView);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (comeFromOrder == true)
            {
                var auftrag = Page as ZulassungLaufkunde;
                var script = auftrag.getScriptManager() as RadScriptManager;
                script.RegisterPostBackControl(AddAdressButton);
            }
            else
            {
                var auftragNeu = Page as AuftragsbearbeitungNeuzulassung;
                var script = auftragNeu.getScriptManager() as RadScriptManager;
                script.RegisterPostBackControl(AddAdressButton);
            }

            CheckOpenedOrders();

            string target = Request["__EVENTTARGET"] == null ? "" : Request["__EVENTTARGET"];
            ZulassungErfolgtLabel.Visible = false;
            ZulassungErrLabel.Visible = false;

            if (String.IsNullOrEmpty(target) && Session["orderNumberSearch"] != null && !String.IsNullOrEmpty(Session["orderNumberSearch"].ToString()))
                target = "IamFromSearch";

            StornierungErfolgLabel.Visible = false;

            if (Session["CustomerIndex"] != null)
            {
                if (!target.Contains("RadComboBoxCustomerZulassungsstelle") &&
                    !target.Contains("CustomerDropDownListZulassungsstelle") &&
                    !target.Contains("NewPositionZulButton") &&
                    !target.Contains("StornierenButton"))
                {
                    if (RadComboBoxCustomerZulassungsstelle.Items.Count() > 0)
                        RadComboBoxCustomerZulassungsstelle.SelectedValue = Session["CustomerIndex"].ToString();

                    if (Session["CustomerId"] != null)
                    {
                        if (CustomerDropDownListZulassungsstelle.Items.Count() > 0)
                            CustomerDropDownListZulassungsstelle.SelectedValue = Session["CustomerId"].ToString();

                        RadGridNeuzulassung.Enabled = true;

                        if (target.Contains("OffenNeuzulassung") ||
                            target.Contains("RadTabStripNeuzulassung") ||
                            target.Contains("IamFromSearch") ||
                            target.Contains("ShowAll"))
                            RadGridNeuzulassung.DataBind();
                    }
                }
            }
        }

        protected void ShowAllButton_Click(object sender, EventArgs e)
        {
            CustomerDropDownListZulassungsstelle.ClearSelection();
            RadGridNeuzulassung.Enabled = true;
            RadGridNeuzulassung.Rebind();
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
            var amtGebCheckBox = sender as CheckBox;

            var orderIdBox = amtGebCheckBox.FindControl("orderIdBox") as TextBox;
            var dienstleistungLabel = amtGebCheckBox.FindControl("DienstleisungZulLabel") as Label;
            dienstleistungLabel.Text = "";
            var dienstleistungVorhandenCheckBox = amtGebCheckBox.FindControl("DiensleistungVorhandenZulCheckBox") as CheckBox;
            dienstleistungVorhandenCheckBox.Checked = false;
            var amtGebBox = amtGebCheckBox.FindControl("AmtGebTextBox") as TextBox;
            var amtGebLabel = amtGebCheckBox.FindControl("AmtGebLabel") as Label;
            amtGebBox.Visible = false;
            amtGebLabel.Visible = false;


            var order = OrderManager.GetById(Int32.Parse(orderIdBox.Text));
            if (order != null)
            {
                foreach (OrderItem item in order.OrderItem)
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

        protected void OnItemCommand_Fired(object sender, GridCommandEventArgs e)
        {
            try
            {
                ZulassungErrLabel.Text = "";
                ZulassungErrLabel.Visible = false;
                if (e.CommandName == "AmtGebuhrSetzen")
                {
                    var editedItem = e.Item as GridEditableItem;
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

        protected void RadGridNeuzulassung_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            var item = (GridDataItem)e.DetailTableView.ParentItem;
            var nestedItem = (GridNestedViewItem)item.ChildItem;
            var radGrdEnquiriesVarients = (RadGrid)nestedItem.FindControl("RadGridNeuzulassungDetails");
            radGrdEnquiriesVarients.DataSource = GetOrderPositions(item["OrderNumber"].Text);
            radGrdEnquiriesVarients.DataBind();
        }

        protected void AuftragFertigStellen_Command(object sender, EventArgs e)
        {
            string kennzeichen = string.Empty,
                VIN = string.Empty,
                hsn = string.Empty,
                tsn = string.Empty;

            var editButton = sender as Button;
            var item = editButton.Parent as Panel;

            ((GridNestedViewItem)((GridTableCell)item.Parent.Parent).Parent).ParentItem.Selected = true;

            var kennzeichenBox = item.FindControl("KennzeichenBox") as TextBox;
            var vinBox = item.FindControl("VINBox") as TextBox;
            var orderIdBox = item.FindControl("orderIdBox") as TextBox;
            var TSNBox = item.FindControl("TSNAbmBox") as TextBox;
            var HSNBox = item.FindControl("HSNAbmBox") as TextBox;
            var errorCheckBox = item.FindControl("ErrorZulCheckBox") as CheckBox;
            var errorReasonTextBox = item.FindControl("ErrorReasonZulTextBox") as TextBox;
            var amtGebBox = item.FindControl("AmtGebTextBox") as TextBox;
            kennzeichen = kennzeichenBox.Text.ToUpper();
            VIN = vinBox.Text;
            var orderNumber = Int32.Parse(orderIdBox.Text);

            tsn = TSNBox.Text;
            hsn = HSNBox.Text;
            ZulassungErfolgtLabel.Visible = false;

            if (errorCheckBox.Checked) // falls Auftrag als Fehler gemeldet sollte
            {
                string errorReason = errorReasonTextBox.Text;
                try
                {
                    var orderToUpdate = OrderManager.GetById(orderNumber);
                    orderToUpdate.HasError = true;
                    orderToUpdate.ErrorReason = errorReason;
                    OrderManager.SaveChanges();

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
                var order = OrderManager.GetById(orderNumber);

                foreach (var orderItem in order.OrderItem)
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
                        updateDataBase(kennzeichen, VIN, tsn, hsn, orderNumber);
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

            ZulassungErrLabel.Visible = false;
            try
            {
                var newAdress = AdressManager.CreateAdress(street, streetNumber, zipcode, city, country);
                var customer = CustomerManager.GetById(customerId);
                var newInvoice = InvoiceManager.CreateInvoice(invoiceRecipient, newAdress, customerId, txbDiscount.Value, InvoiceType.Single);

                InvoiceIdHidden.Value = newInvoice.Id.ToString();

                var order = OrderManager.GetById(Int32.Parse(smallCustomerOrderHiddenField.Value));
                foreach (var ordItem in order.OrderItem)
                {
                    ProductName = ordItem.ProductName;
                    Amount = ordItem.Amount;

                    CostCenter costCenter = null;
                    if (ordItem.CostCenterId.HasValue)
                    {
                        costCenter = CostCenterManager.GetById(ordItem.CostCenterId.Value);
                    }

                    itemCount = ordItem.Count;
                    var newInvoiceItem = InvoiceManager.AddInvoiceItem(newInvoice, ProductName, Convert.ToDecimal(Amount), itemCount, ordItem, costCenter,
                        customer, OrderItemStatusTypes.Payed);
                }
                
                // Closing RadWindow
                string script = "function f(){$find(\"" + AddAdressRadWindow.ClientID + "\").close(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);

                Print(newInvoice);

                order.Status = (int)OrderStatusTypes.Payed;
                OrderManager.SaveChanges();
 
                RadGridNeuzulassung.Rebind();
            }
            catch (Exception ex)
            {
                ZulassungErrLabel.Text = " Fehler:" + ex.Message;
                ZulassungErrLabel.Visible = true;
            }
        }

        protected void NewPositionButton_Clicked(object sender, EventArgs e)
        {
            if (RadGridNeuzulassung.SelectedItems.Count > 0)
            {
                try
                {
                    ZulassungErrLabel.Visible = false;
                    var button = sender as Button;

                    Price newPrice = null;
                    var productDropDown = button.NamingContainer.FindControl("NewProductDropDownList") as RadComboBox;
                    var costCenterDropDown = button.NamingContainer.FindControl("CostCenterDropDownList") as DropDownList;

                    foreach (GridDataItem item in RadGridNeuzulassung.SelectedItems)
                    {
                        var newProduct = ProductManager.GetById(Int32.Parse(productDropDown.SelectedValue));

                        if (!String.IsNullOrEmpty(item["locationId"].Text))
                        {
                            var locationId = Int32.Parse(item["locationId"].Text);
                            newPrice = PriceManager.GetEntities(q => q.ProductId == newProduct.Id && q.LocationId == locationId).SingleOrDefault();
                        }

                        if (newPrice == null || String.IsNullOrEmpty(item["locationId"].Text))
                        {
                            newPrice = PriceManager.GetEntities(q => q.ProductId == newProduct.Id && q.LocationId == null).SingleOrDefault();
                        }

                        var orderToUpdate = OrderManager.GetById(Int32.Parse(item["OrderNumber"].Text));

                        if (newPrice == null || newProduct == null || orderToUpdate == null)
                            throw new Exception("Achtung, die Position kann nicht hinzugefügt werden, es konnte entweder kein Preis, Produkt oder der Auftrag gefunden werden!");

                        if (orderToUpdate != null)
                        {
                            var orderItemCostCenter = orderToUpdate.OrderItem.FirstOrDefault(q => q.CostCenter != null);

                            var newOrderItem = OrderManager.AddOrderItem(orderToUpdate, newProduct.Id, newPrice.Amount, 1, 
                                (orderItemCostCenter != null) ? orderItemCostCenter.CostCenter : null, null, false);

                            if (newPrice.AuthorativeCharge.HasValue)
                            {
                                OrderManager.AddOrderItem(orderToUpdate, newProduct.Id, newPrice.AuthorativeCharge.Value, 1, 
                                    (orderItemCostCenter != null) ? orderItemCostCenter.CostCenter : null,
                                    newOrderItem.Id, newPrice.AuthorativeCharge.HasValue);
                            }
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

        protected void CostCenterDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = CostCenterManager.GetEntities().OrderBy(o => o.Name).Select(cost => new
            {
                Name = cost.Name,
                Value = cost.Id
            }).ToList();
        }

        //Row Index wird in hiddenfield gespeichert
        protected void Edit_Command(object sender, GridCommandEventArgs e)
        {
            var button = sender as RadButton;
            GridDataItem dataItem = e.Item as GridDataItem;
            dataItem.Selected = true;
            itemIndexHiddenField.Value = dataItem.ItemIndex.ToString();
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
            var count = GetUnfineshedOrdersCount(OrderTypes.Admission, OrderStatusTypes.AdmissionPoint);
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
            RadGridNeuzulassung.Columns.FindByUniqueName("CustomerLocation").Visible = false;
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

        //falls benötigt, wird der Status per Email gesendet
        protected void SendStatusByEmail(int customerId, Order orderToSend)
        {
            var customer = CustomerManager.GetById(customerId);
            string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
            string smtp = ConfigurationManager.AppSettings["smtpHost"];

            if (customer.LargeCustomer != null &&
                customer.LargeCustomer.OrderFinishedNoteSendType == 1) //sofort gesendet
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

                    var orderNumber = Int32.Parse(item["OrderNumber"].Text);

                    smallCustomerOrderHiddenField.Value = orderNumber.ToString();
                    CustomerIdHiddenField.Value = customerID.ToString();

                    try
                    {
                        var newOrder = OrderManager.GetById(orderNumber);
                        if (newOrder != null)
                        {
                            if (RadComboBoxCustomerZulassungsstelle.SelectedIndex == 1) // small
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
            var location = CustomerManager.GetEntities(o => o.Id == customerId).
                Select(o => o.InvoiceAdress).FirstOrDefault();

            StreetTextBox.Text = location.Street;
            StreetNumberTextBox.Text = location.StreetNumber;
            ZipcodeTextBox.Text = location.Zipcode;
            CityTextBox.Text = location.City;
            CountryTextBox.Text = location.Country;

            LocationLabelWindow.Text = "Fügen Sie bitte die Adresse für " + CustomerDropDownListZulassungsstelle.Text + " hinzu";
            ZusatzlicheInfoLabel.Visible = false;
            if (RadComboBoxCustomerZulassungsstelle.SelectedIndex == 1) // small
            {
                ZusatzlicheInfoLabel.Visible = true;
            }
        }
        
        protected void Print(Invoice newInvoice)
        {
            using (MemoryStream memS = new MemoryStream())
            {
                string serverPath = ConfigurationManager.AppSettings["DataPath"] + "\\UserData";
                InvoiceItemAccountItemManager.CreateAccounts(newInvoice);
                InvoiceManager.Print(newInvoice, memS, "", ""/*TODO check last para, */);

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
        
        // Updating Order und setzen Status 600 - Fertig
        protected void updateDataBase(string kennzeichen, string vin, string tsn, string hsn, int orderNumber)
        {
            var order = OrderManager.GetById(orderNumber);
            order.RegistrationOrder.Registration.Licencenumber = kennzeichen;
            order.RegistrationOrder.Vehicle.VIN = vin;
            order.RegistrationOrder.Vehicle.TSN = tsn;
            order.RegistrationOrder.Vehicle.HSN = hsn;
            OrderManager.SaveChanges();
        }

        #endregion
    }
}