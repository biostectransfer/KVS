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
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Transactions;
using KVSCommon.Enums;
using KVSWebApplication.BasePages;
using KVSWebApplication.PrintServiceReference;

namespace KVSWebApplication.Auftragsbearbeitung_Neuzulassung
{

    /// <summary>
    /// Codebehind fuer den Reiter Offen Neuzulassung
    /// </summary>
    public partial class OffenNeuzulassung : EditOrdersBase
    {
        #region Members  

        protected override RadGrid OrderGrid { get { return this.RadGridOffNeuzulassung; } }
        protected override RadDatePicker RegistrationDatePicker { get { return this.ZulassungsDatumPicker; } }
        protected override RadComboBox CustomerTypeDropDown { get { return this.RadComboBoxCustomerOffenNeuzulassung; } }
        protected override RadComboBox CustomerDropDown { get { return this.CustomerDropDownListOffenNeuzulassung; } }
        protected override PermissionTypes PagePermission { get { return PermissionTypes.LOESCHEN_AUFTRAGSPOSITION; } }
        protected override OrderTypes OrderType { get { return OrderTypes.Admission; } }
        protected override OrderStatusTypes OrderStatusType { get { return OrderStatusTypes.Open; } }
        protected override string OrderStatusSearch { get { return "Offen"; } }
        #endregion

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserManager.CheckPermissionsForUser(Session["UserPermissions"], PagePermission))
            {
                foreach (var table in RadGridOffNeuzulassung.MasterTableView.DetailTables)
                {
                    if (table.GetColumn("RemoveOrderItem") != null)
                        table.GetColumn("RemoveOrderItem").Visible = false;
                }
            }

            var auftragNeu = Page as AuftragsbearbeitungNeuzulassung;
            script = auftragNeu.getScriptManager() as RadScriptManager;
            script.RegisterPostBackControl(ZulassungsstelleLieferscheineButton);

            CheckOpenedOrders();

            string target = Request["__EVENTTARGET"] == null ? "" : Request["__EVENTTARGET"];
            if (String.IsNullOrEmpty(target))
                if (Session["orderNumberSearch"] != null)
                    if (!String.IsNullOrEmpty(Session["orderNumberSearch"].ToString()))
                        target = "IamFromSearch";

            StornierungErfolgLabel.Visible = false;

            if (Session["CustomerIndex"] != null)
            {
                if (!target.Contains("RadComboBoxCustomerOffenNeuzulassung") && !target.Contains("StornierenButton") && !target.Contains("CustomerDropDownListOffenNeuzulassung") && !target.Contains("NewPositionButton"))
                {

                    //CustomerDropDownListOffenNeuzulassung.DataBind();
                    if (Session["CustomerId"] != null)
                    {
                        if (!Page.IsPostBack)
                        {
                            if (CustomerDropDownListOffenNeuzulassung.Items.Count > 0 && RadComboBoxCustomerOffenNeuzulassung.Items.Count() > 0)
                            {
                                CustomerDropDownListOffenNeuzulassung.SelectedValue = Session["CustomerId"].ToString();
                                RadComboBoxCustomerOffenNeuzulassung.SelectedValue = Session["CustomerIndex"].ToString();
                            }
                        }

                        if (Session["orderStatusSearch"] != null)
                            if (!Session["orderStatusSearch"].ToString().Contains("Zulassungsstelle"))
                                if (target.Contains("ZulassungNachbearbeitung") || target.Contains("RadTabStripNeuzulassung") || target.Contains("IamFromSearch"))
                                {
                                    RadGridOffNeuzulassung.Enabled = true;
                                    RadGridOffNeuzulassung.DataBind();
                                }
                    }
                }
            }
        }

        protected void RadGridRadGridOffNeuzulassung_PreRender(object sender, EventArgs e)
        {
            HideExpandColumnRecursive(RadGridOffNeuzulassung.MasterTableView);
        }

        /// <summary>
        /// Lilfsmethoden fuer das oeffnen der Grid
        /// </summary>
        /// <param name="tableView"></param>
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

        // Large oder small Customer
        protected void SmallLargeCustomerIndex_Changed(object sender, EventArgs e)
        {
            CustomerDropDownListOffenNeuzulassung.Enabled = true;
            this.CustomerDropDownListOffenNeuzulassung.DataBind();
            this.RadGridOffNeuzulassung.DataBind();
            Session["CustomerIndex"] = RadComboBoxCustomerOffenNeuzulassung.SelectedValue;
        }

        /// <summary>
        /// Kundenauswahl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CustomerIndex_Changed(object sender, EventArgs e)
        {
            RadGridOffNeuzulassung.Enabled = true;
            this.RadGridOffNeuzulassung.DataBind();
            Session["CustomerIndex"] = RadComboBoxCustomerOffenNeuzulassung.SelectedValue;
            Session["CustomerId"] = CustomerDropDownListOffenNeuzulassung.SelectedValue;
        }

        /// <summary>
        /// Datasource fuer die Detailgrid
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RadGridZulOffen_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            var item = (GridDataItem)e.DetailTableView.ParentItem;

            e.DetailTableView.DataSource = GetOrderPositions(item["OrderNumber"].Text);
        }

        /// <summary>
        ///  Neue freie Position hinzufügen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void NewPositionButton_Clicked(object sender, EventArgs e)
        {
            if (RadGridOffNeuzulassung.SelectedItems.Count > 0)
            {
                try
                {
                    ZulassungErrLabel.Visible = false;
                    Button button = sender as Button;
                    Price newPrice = null;

                    var productDropDown = button.NamingContainer.FindControl("NewProductDropDownList") as RadComboBox;
                    var productId = Int32.Parse(productDropDown.SelectedValue);

                    foreach (GridDataItem item in RadGridOffNeuzulassung.SelectedItems)
                    {
                        var orderNumber = Int32.Parse(item["OrderNumber"].Text);
                        var newProduct = ProductManager.GetById(productId);

                        if (!String.IsNullOrEmpty(item["locationId"].Text) && item["locationId"].Text.Length > 6)
                        {
                            var locationId = Int32.Parse(item["locationId"].Text);
                            newPrice = PriceManager.GetEntities(q => q.ProductId == newProduct.Id && q.LocationId == locationId).SingleOrDefault();
                        }

                        if (newPrice == null || String.IsNullOrEmpty(item["locationId"].Text))
                        {
                            newPrice = PriceManager.GetEntities(q => q.ProductId == newProduct.Id && q.LocationId == null).SingleOrDefault();
                        }

                        var orderToUpdate = OrderManager.GetById(orderNumber);

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

                    RadGridOffNeuzulassung.Rebind();
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

            CheckOpenedOrders();
        }

        /// <summary>
        /// Event fuer das Bearbeiten in der Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EditButton_Clicked(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "AmtGebuhrSetzen")
            {
                GridEditableItem editedItem = e.Item as GridEditableItem;
                RadTextBox tbEditPrice = editedItem["ColumnPrice"].FindControl("tbEditPrice") as RadTextBox;
            }
            else
            {
                var button = sender as RadButton;
                GridDataItem dataItem = e.Item as GridDataItem;
                dataItem.Selected = true;
                itemIndexHiddenField.Value = dataItem.ItemIndex.ToString();
            }
        }

        /// <summary>
        /// Command fuer das Bearbeiten in der Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                    if (OrderManager.GenerateAuthCharge(authChargeId, Int32.Parse(itemId), tbAuthPrice.Text))
                    {
                        tbAuthPrice.ForeColor = System.Drawing.Color.Green;
                    }

                    UpdatePosition(itemId, tbEditPrice.Text);
                    tbEditPrice.ForeColor = System.Drawing.Color.Green;
                }
                else if (e.CommandName == "RemoveOrderItem")
                {
                    try
                    {
                        GridEditableItem editedItem = e.Item as GridEditableItem;
                        string itemId = editedItem["ItemIdColumn"].Text;
                        OrderManager.RemoveOrderItem(Int32.Parse(itemId));
                    }
                    catch (Exception ex)
                    {
                        ZulassungErrLabel.Text = "Fehler: " + ex.Message;
                        ZulassungErrLabel.Visible = true;

                        //TODO WriteLogItem("Delete OrderItem Error " + ex.Message, LogTypes.ERROR, "OrderItem");
                    }

                    RadGridOffNeuzulassung.Rebind();
                }
                else if (e.CommandName == "Edit")
                {
                    if (e.Item is GridDataItem)
                    {
                        var button = sender as RadButton;
                        GridDataItem dataItem = e.Item as GridDataItem;
                        dataItem.Selected = true;
                        itemIndexHiddenField.Value = dataItem.ItemIndex.ToString();
                    }
                }
                else if(e.CommandName == "PrintColumn")
                {
                    //TODO
                    var printServiceClient = new PrintServiceClient();
                    printServiceClient.EmissionBadgePrint("lol");
                }

                CheckOpenedOrders();
            }
            catch (Exception ex)
            {
                ZulassungErrLabel.Text = "Fehler: " + ex.Message;
                ZulassungErrLabel.Visible = true;

            }
        }

        /// <summary>
        ///  Automatische Suche nach HSN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Event fuer den Lieferschein erstellen Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ZulassungsstelleLieferscheineButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(ZulassungsDatumPicker.SelectedDate.ToString()))
                {
                    LieferscheinePath.Text = "Wählen Sie bitte das Zulassungsdatum aus!";
                    LieferscheinePath.Visible = true;
                }
                else
                {
                    var laufzettel = new List<string>();
                    script.RegisterPostBackControl(ZulassungsstelleLieferscheineButton);

                    var orders = OrderManager.GetEntities(o =>
                                    o.Status == (int)OrderStatusTypes.Open &&
                                    o.OrderTypeId == (int)OrderTypes.Admission &&
                                    o.HasError.GetValueOrDefault(false) != true &&
                                    o.RegistrationOrder != null &&
                                    o.RegistrationOrder.Registration.RegistrationDate <= ZulassungsDatumPicker.SelectedDate).ToList();

                    foreach (var location in orders.GroupBy(q => q.Zulassungsstelle.Value))
                    {
                        var docketList = new DocketList();
                        var ms = new MemoryStream();

                        if (location.Count() > 0)
                        {
                            docketList = DocketListManager.CreateDocketList(
                                location.First().RegistrationLocation.RegistrationLocationName,
                                location.First().RegistrationLocation.Adress);

                            docketList.IsSelfDispatch = true;
                        }

                        foreach (var order in location)
                        {
                            if (order != null)
                            {
                                DocketListManager.AddOrder(docketList, order);
                                //updating order status
                                order.Status = (int)OrderStatusTypes.AdmissionPoint;

                                //updating orderitems status                          
                                foreach (OrderItem ordItem in order.OrderItem)
                                {
                                    if (ordItem.Status != (int)OrderItemStatusTypes.Cancelled)
                                    {
                                        ordItem.Status = (int)OrderItemStatusTypes.InProgress;
                                    }
                                }
                            }
                        }

                        DocketListManager.SaveChanges();

                        string myPackListFileName = EmptyStringIfNull.CheckIfFolderExistsAndReturnPathForPdf(Session["CurrentUserId"].ToString(), true);

                        DocketListManager.Print(docketList, ms, string.Empty, "/UserData/" + Session["CurrentUserId"].ToString() + "/" + Path.GetFileName(myPackListFileName), true);

                        string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
                        string host = ConfigurationManager.AppSettings["smtpHost"];

                        var pdfDocument = PdfReader.Open(new MemoryStream(ms.ToArray(), 0, Convert.ToInt32(ms.Length)));
                        pdfDocument.Save(myPackListFileName);

                        DocketListManager.SendByEmail(docketList, ms, fromEmail, host);
                        ms.Close();
                        ms = null;

                        laufzettel.Add(myPackListFileName);
                    }


                    RadGridOffNeuzulassung.MasterTableView.ClearChildEditItems();
                    RadGridOffNeuzulassung.MasterTableView.ClearEditItems();
                    RadGridOffNeuzulassung.Rebind();

                    CheckOpenedOrders();

                    if (laufzettel.Count > 1)
                    {
                        string myMergedFileName = EmptyStringIfNull.CheckIfFolderExistsAndReturnPathForPdf(Session["CurrentUserId"].ToString(), true);
                        PackingListManager.MergePackingLists(laufzettel.ToArray(), myMergedFileName);

                        myMergedFileName = myMergedFileName.Replace(ConfigurationManager.AppSettings["BasePath"], ConfigurationManager.AppSettings["BaseUrl"]);
                        myMergedFileName = myMergedFileName.Replace(@"\\", @"/");
                        myMergedFileName = myMergedFileName.Replace(@"\", @"/");
                        LieferscheinePath.Text = "<a href=" + '\u0022' + myMergedFileName + '\u0022' + " target=" + '\u0022' + "_blank" + '\u0022' + "> Laufzettel öffnen</a>";
                        LieferscheinePath.Visible = true;
                    }
                    else if (laufzettel.Count == 1)
                    {
                        string myMergedFileName = laufzettel[0];
                        myMergedFileName = myMergedFileName.Replace(ConfigurationManager.AppSettings["BasePath"], ConfigurationManager.AppSettings["BaseUrl"]);
                        myMergedFileName = myMergedFileName.Replace(@"\\", @"/");
                        myMergedFileName = myMergedFileName.Replace(@"\", @"/");

                        LieferscheinePath.Text = "<a href=" + '\u0022' + myMergedFileName + '\u0022' + " target=" + '\u0022' + "_blank" + '\u0022' + "> Laufzettel öffnen</a>";
                        LieferscheinePath.Visible = true;
                    }
                    else
                    {
                        LieferscheinePath.Text = "Keine Lieferscheine vorhanden!";
                        LieferscheinePath.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ZulassungErrLabel.Visible = true;
                ZulassungErrLabel.Text = "Fehler: " + ex.Message;
            }
        }

        /// <summary>
        /// Zeige alle Auftraege
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ShowAllButton1_Click(object sender, EventArgs e)
        {
            RadGridOffNeuzulassung.Enabled = true;

            Session["customerIndexSearch"] = null;
            Session["orderStatusSearch"] = null;
            Session["customerIdSearch"] = null;
            Session["orderNumberSearch"] = null;

            CustomerDropDownListOffenNeuzulassung.ClearSelection();
            RadGridOffNeuzulassung.Rebind();
        }

        /// <summary>
        /// Event um eine neue Zulassung zu erstellen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ZulassungZulassen_Command(object sender, EventArgs e)
        {
            if (itemIndexHiddenField.Value != null) // falls ausgewählte Row Index gesetz wurde
            {
                GridDataItem selectedItem = RadGridOffNeuzulassung.MasterTableView.Items[Convert.ToInt32(itemIndexHiddenField.Value)];
                selectedItem.Selected = true;

                string VIN = string.Empty,
                    kennzeichen = string.Empty,
                    HSN = string.Empty,
                    TSN = string.Empty;
                
                var editButton = sender as Button;
                var item = editButton.NamingContainer as GridEditFormItem;

                var vinBox = item.FindControl("VINBox") as TextBox;
                var orderIdBox = item.FindControl("orderIdBox") as TextBox;
                var kennzeichenBox = item.FindControl("KennzeichenBox") as TextBox;
                var errorCheckBox = item.FindControl("ErrorCheckBox") as CheckBox;
                var errorReasonTextBox = item.FindControl("ErrorReasonTextBox") as TextBox;
                var HSNBox = item.FindControl("HSNAbmBox") as TextBox;
                var TSNBox = item.FindControl("TSNAbmBox") as TextBox;

                var orderNumber = Int32.Parse(orderIdBox.Text);
                
                ZulassungErrLabel.Visible = false;

                if (errorCheckBox.Checked) // falls Auftrag als Fehler gemeldet sollte
                {
                    string errorReason = errorReasonTextBox.Text;

                    try
                    {
                        var orderToUpdate = OrderManager.GetById(orderNumber);
                        orderToUpdate.HasError = true;
                        orderToUpdate.ErrorReason = errorReason;
                        OrderManager.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ZulassungErrLabel.Text = "Fehler:" + ex.Message;
                        ZulassungErrLabel.Visible = true;
                    }
                }
                else // falls normales Update 
                {
                    VIN = vinBox.Text;
                    TSN = TSNBox.Text;
                    HSN = HSNBox.Text;
                    kennzeichen = kennzeichenBox.Text;

                    try
                    {
                        updateDataBase(VIN, TSN, HSN, orderNumber, kennzeichen);
                    }
                    catch (Exception ex)
                    {
                        ZulassungErrLabel.Text = "Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator. <br /> " + "Error: " + ex.Message;
                        ZulassungErrLabel.Visible = true;
                    }
                }

                if (Session["orderNumberSearch"] != null)
                    Session["orderNumberSearch"] = string.Empty; //after search should be empty

                RadGridOffNeuzulassung.MasterTableView.ClearChildEditItems();
                RadGridOffNeuzulassung.MasterTableView.ClearEditItems();
                RadGridOffNeuzulassung.Rebind();
            }
        }

        /// <summary>
        /// Ändert den Text von Button entweder auf Fehler oder Zulassung
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ErrorCheckBox_Clicked(object sender, EventArgs e)
        {
            CheckBox errorCheckBox = sender as CheckBox;
            Button saveButton = errorCheckBox.FindControl("ZulassenButton") as Button;

            if (errorCheckBox.Checked)
                saveButton.Text = "Als Fehler markieren";
            else
                saveButton.Text = "Speichern und zulassen";
        }

        /// <summary>
        /// Event Auftrag stornieren 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void StornierenButton_Clicked(object sender, EventArgs e)
        {
            if (RadGridOffNeuzulassung.SelectedItems.Count > 0)
            {
                ZulassungErrLabel.Visible = false;
                StornierungErfolgLabel.Visible = false;

                foreach (GridDataItem item in RadGridOffNeuzulassung.SelectedItems)
                {
                    var orderNumber = Int32.Parse(item["OrderNumber"].Text);

                    try
                    {
                        var newOrder = OrderManager.GetById(orderNumber);
                        newOrder.Status = (int)OrderStatusTypes.Cancelled;

                        //updating orderitems status                          
                        foreach (OrderItem ordItem in newOrder.OrderItem)
                        {
                            ordItem.Status = (int)OrderItemStatusTypes.Cancelled;
                        }

                        OrderManager.SaveChanges();
                        RadGridOffNeuzulassung.Rebind();
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
        
        protected override void CheckOpenedOrders()
        {
            var count = GetUnfineshedOrdersCount(OrderTypes.Admission, OrderStatusTypes.Open);
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

        /// <summary>
        ///  Updat vom gewaehlter Auftragsposition
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="amount"></param>
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
                    OrderManager.UpdateOrderItemAmount(Int32.Parse(itemId), Convert.ToDecimal(amoutToSave));
                }
                catch (Exception ex)
                {
                    throw new Exception("Die ausgewählte Position kann nicht aktualisiert werden <br /> Error: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Aktualisiere den Auftragsstatus
        /// </summary>
        /// <param name="customerIdToUpdate">KundeID</param>
        /// <param name="orderNumberToUpdate">Auftragsid</param>

        protected void UpdateOrderAfterZulassungsstelle(int customerIdToUpdate, int orderNumberToUpdate)
        {
            try
            {
                var newOrder = OrderManager.GetById(orderNumberToUpdate);

                if (newOrder != null)
                {
                    //updating order status
                    newOrder.Status = (int)OrderStatusTypes.AdmissionPoint;

                    //updating orderitems status                          
                    foreach (OrderItem ordItem in newOrder.OrderItem)
                    {
                        if (ordItem.Status != (int)OrderItemStatusTypes.Cancelled)
                        {
                            ordItem.Status = (int)OrderItemStatusTypes.InProgress;
                        }
                    }

                    OrderManager.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ZulassungErrLabel.Text = "Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator. <br />" + "Fehler: " + ex.Message;
                ZulassungErrLabel.Visible = true;
            }
        }

        /// <summary>
        ///  Aktualisiere Auftragsstatus auf 400 (Zulassungstelle)
        /// </summary>
        protected void UpdateOrderAndItemsStatus()
        {
            if (RadGridOffNeuzulassung.SelectedItems.Count > 0)
            {
                ZulassungErrLabel.Visible = false;
                ZulassungOkLabel.Visible = false;

                foreach (GridDataItem item in RadGridOffNeuzulassung.SelectedItems)
                {
                    var orderNumber = Int32.Parse(item["OrderNumber"].Text);

                    try
                    {
                        var newOrder = OrderManager.GetById(orderNumber);

                        if (newOrder != null)
                        {
                            //updating order status
                            newOrder.Status = (int)OrderStatusTypes.AdmissionPoint;

                            //updating orderitems status                          
                            foreach (OrderItem ordItem in newOrder.OrderItem)
                            {
                                if (ordItem.Status != (int)OrderItemStatusTypes.Cancelled)
                                {
                                    ordItem.Status = (int)OrderItemStatusTypes.InProgress;
                                }
                            }

                            OrderManager.SaveChanges();
                        }
                    }
                    catch
                    {
                        ZulassungErrLabel.Text = "Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator.";
                        ZulassungErrLabel.Visible = true;
                    }
                }

                // erfolgreich
                RadGridOffNeuzulassung.DataBind();
                ZulassungOkLabel.Visible = true;
            }
            else
            {
                ZulassungErrLabel.Text = "Sie haben keine Auftrag ausgewählt!";
                ZulassungErrLabel.Visible = true;
            }
        }

        /// <summary>
        ///  Aktualisiere DB Einträge vor der Zulassungsstelle
        /// </summary>
        /// <param name="vin">FIN</param>
        /// <param name="tsn">TSN</param>
        /// <param name="hsn">HSN</param>
        /// <param name="OrderNumber">AuftragsID</param>
        /// <param name="customerId">KundeID</param>
        /// <param name="kennzeichen">Kennzeichen</param>
        protected void updateDataBase(string vin, string tsn, string hsn, int orderNumber, string kennzeichen)
        {
            var orderToUpdate = RegistrationOrderManager.GetById(orderNumber);

            orderToUpdate.Vehicle.VIN = vin;
            orderToUpdate.Registration.Licencenumber = kennzeichen;
            orderToUpdate.Vehicle.TSN = tsn;
            orderToUpdate.Vehicle.HSN = hsn;

            RegistrationOrderManager.SaveChanges();
        }

        /// <summary>
        /// Prüfen ob alle Werte da sind um den Auftrag auf "Zulassungstelle" zu setzen
        /// </summary>
        /// <returns></returns>
        protected bool CheckIfAllExistsToUpdate()
        {
            bool shouldBeUpdated = true;
            ZulassungErrLabel.Visible = false;

            foreach (GridDataItem item in RadGridOffNeuzulassung.SelectedItems)
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

        #endregion
    }
}