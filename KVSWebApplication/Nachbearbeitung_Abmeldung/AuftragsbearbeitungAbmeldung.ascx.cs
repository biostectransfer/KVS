using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Configuration;
using System.Transactions;
using KVSCommon.Enums;
using KVSWebApplication.BasePages;

namespace KVSWebApplication.Nachbearbeitung_Abmeldung
{
    /// <summary>
    /// Codebehind fuer die Abmeldung Maske
    /// </summary>
    public partial class AuftragsbearbeitungAbmeldung : EditOrdersBase
    {
        #region Members  

        protected override RadGrid OrderGrid { get { return this.RadGridAbmeldung; } }
        protected override RadDatePicker RegistrationDatePicker { get { return this.ZulassungsDatumPicker; } }
        protected override RadComboBox CustomerTypeDropDown { get { return this.RadComboBoxCustomerAbmeldungOffen; } }
        protected override RadComboBox CustomerDropDown { get { return this.CustomerDropDownListAbmeldungOffen; } }
        protected override PermissionTypes PagePermission { get { return PermissionTypes.LOESCHEN_AUFTRAGSPOSITION; } }
        protected override OrderTypes OrderType { get { return OrderTypes.Cancellation; } }
        protected override OrderStatusTypes OrderStatusType { get { return OrderStatusTypes.Open; } }
        protected override string OrderStatusSearch { get { return "Offen"; } }
        #endregion

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserManager.CheckPermissionsForUser(Session["UserPermissions"], PagePermission))
            {
                foreach (var table in RadGridAbmeldung.MasterTableView.DetailTables)
                {
                    if (table.GetColumn("RemoveOrderItem") != null)
                        table.GetColumn("RemoveOrderItem").Visible = false;
                }
            }

            var auftragNeu = Page as NachbearbeitungAbmeldung;
            script = auftragNeu.getScriptManager() as RadScriptManager;
            script.RegisterPostBackControl(ZulassungsstelleLieferscheineButton);
            string target = Request["__EVENTTARGET"] == null ? "" : Request["__EVENTTARGET"];

            if (String.IsNullOrEmpty(target) && Session["orderNumberSearch"] != null && !String.IsNullOrEmpty(Session["orderNumberSearch"].ToString()))
                target = "IamFromSearch";

            StornierungErfolgLabel.Visible = false;

            if (Session["CustomerIndex"] != null)
            {
                if (!target.Contains("RadComboBoxCustomerAbmeldungOffen") && !target.Contains("CustomerDropDownListAbmeldungOffen") &&
                    !target.Contains("StornierenButton") && !target.Contains("NewPositionButton"))
                {
                    if (Session["CustomerId"] != null)
                    {
                        if (!Page.IsPostBack)
                        {
                            if (CustomerDropDownListAbmeldungOffen.Items.Count > 0 && RadComboBoxCustomerAbmeldungOffen.Items.Count() > 0)
                            {
                                RadComboBoxCustomerAbmeldungOffen.SelectedValue = Session["CustomerIndex"].ToString();
                                CustomerDropDownListAbmeldungOffen.SelectedValue = Session["CustomerId"].ToString();
                            }
                        }

                        if (Session["orderStatusSearch"] != null && !Session["orderStatusSearch"].ToString().Contains("Zulassungsstelle") &&
                            (target.Contains("ZulassungNachbearbeitung") || target.Contains("RadTabStrip1") || target.Contains("IamFromSearch")))
                        {
                            RadGridAbmeldung.Enabled = true;
                            RadGridAbmeldung.Rebind();
                        }
                    }
                }
            }
        }

        protected void ShowAllButton_Click(object sender, EventArgs e)
        {
            Session["customerIndexSearch"] = null;
            Session["orderStatusSearch"] = null;
            Session["customerIdSearch"] = null;
            Session["orderNumberSearch"] = null;

            CustomerDropDownListAbmeldungOffen.ClearSelection();
            RadGridAbmeldung.Enabled = true;
            RadGridAbmeldung.Rebind();
        }

        // Large oder small Customer
        protected void SmallLargeCustomerIndex_Changed(object sender, EventArgs e)
        {
            CustomerDropDownListAbmeldungOffen.Enabled = true;
            this.CustomerDropDownListAbmeldungOffen.DataBind();
            this.RadGridAbmeldung.DataBind();
            Session["CustomerIndex"] = RadComboBoxCustomerAbmeldungOffen.SelectedValue;
        }

        // Auswahl von Kunde
        protected void CustomerIndex_Changed(object sender, EventArgs e)
        {
            RadGridAbmeldung.Enabled = true;
            this.RadGridAbmeldung.DataBind();
            Session["CustomerIndex"] = RadComboBoxCustomerAbmeldungOffen.SelectedValue;
            Session["CustomerId"] = CustomerDropDownListAbmeldungOffen.SelectedValue;
        }

        // Automatische Suche nach HSN
        protected void HSNBox_TextChanged(object sender, EventArgs e)
        {
            var hsnTextBox = sender as TextBox;
            var hsnLabel = hsnTextBox.Parent.FindControl("HSNSearchLabel") as Label;
            var tsnBox = hsnTextBox.Parent.FindControl("TSNAbmFormBox") as TextBox;
            hsnLabel.Text = "";

            if (!String.IsNullOrEmpty(hsnTextBox.Text))
            {
                hsnLabel.Visible = true;
                hsnLabel.Text = Make.GetMakeByHSN(hsnTextBox.Text);
            }

            tsnBox.Focus();
        }

        protected void OnItemCommand_Fired(object sender, GridCommandEventArgs e)
        {
            try
            {
                AbmeldungErrLabel.Text = "";
                AbmeldungErrLabel.Visible = false;
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
                else if (e.CommandName == "RemoveOrderItem")
                {
                    try
                    {

                        var editedItem = e.Item as GridEditableItem;
                        string itemId = editedItem["ItemIdColumn"].Text;
                        OrderManager.RemoveOrderItem(Int32.Parse(itemId));
                    }
                    catch (Exception ex)
                    {
                        AbmeldungErrLabel.Text = "Fehler: " + ex.Message;
                        AbmeldungErrLabel.Visible = true;

                        //TODO WriteLogItem("Delete OrderItem Error " + ex.Message, LogTypes.ERROR, "OrderItem");
                    }

                    RadGridAbmeldung.Rebind();
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

        // Neue freie Position wird hinzugefügt
        protected void NewPositionButton_Clicked(object sender, EventArgs e)
        {
            if (RadGridAbmeldung.SelectedItems.Count > 0)
            {
                try
                {
                    AbmeldungErrLabel.Visible = false;
                    Button button = sender as Button;

                    Price newPrice = null;
                    var productDropDown = button.NamingContainer.FindControl("NewProductDropDownList") as RadComboBox;
                    var costCenterDropDown = button.NamingContainer.FindControl("CostCenterDropDownList") as DropDownList;
                    var product = Int32.Parse(productDropDown.SelectedValue);

                    foreach (GridDataItem item in RadGridAbmeldung.SelectedItems)
                    {
                        var newProduct = ProductManager.GetById(Int32.Parse(productDropDown.SelectedValue));

                        var orderNumber = Int32.Parse(item["OrderNumber"].Text);
                        if (!String.IsNullOrEmpty(item["locationId"].Text))
                        {
                            var locationId = Int32.Parse(item["locationId"].Text);
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

                            var newOrderItem = OrderManager.AddOrderItem(orderToUpdate, newProduct.Id, newPrice.Amount, 1,
                                 (orderItemCostCenter != null) ? orderItemCostCenter.CostCenter : null,
                                 null, false);

                            if (newPrice.AuthorativeCharge.HasValue)
                            {
                                OrderManager.AddOrderItem(orderToUpdate, newProduct.Id, newPrice.AuthorativeCharge.Value, 1,
                                    (orderItemCostCenter != null) ? orderItemCostCenter.CostCenter : null,
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

        protected void RadGridOffen_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            var item = (GridDataItem)e.DetailTableView.ParentItem;

            e.DetailTableView.DataSource = GetOrderPositions(item["OrderNumber"].Text);
        }

        //Row Index wird in hiddenfield gespeichert
        protected void EditButton_Clicked(object sender, GridCommandEventArgs e)
        {
            var button = sender as RadButton;
            GridDataItem dataItem = e.Item as GridDataItem;
            dataItem.Selected = true;
            itemIndexHiddenField.Value = dataItem.ItemIndex.ToString();
        }

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

                    var orders = OrderManager.GetEntities(o =>
                           o.Status == (int)OrderStatusTypes.Open &&
                           o.OrderTypeId == (int)OrderTypes.Cancellation &&
                           o.HasError.GetValueOrDefault(false) != true &&
                           o.DeregistrationOrder != null &&
                           o.DeregistrationOrder.Registration.RegistrationDate <= ZulassungsDatumPicker.SelectedDate).ToList();

                    var grouptedOrders = orders.GroupBy(q => q.Zulassungsstelle.Value);
                    foreach (var location in grouptedOrders)
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
                                DocketListManager.AddOrderById(docketList, order.Id);
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


                    RadGridAbmeldung.MasterTableView.ClearChildEditItems();
                    RadGridAbmeldung.MasterTableView.ClearEditItems();
                    RadGridAbmeldung.Rebind();

                    if (laufzettel.Count > 1)
                    {
                        string myMergedFileName = EmptyStringIfNull.CheckIfFolderExistsAndReturnPathForPdf(Session["CurrentUserId"].ToString(), true);
                        DocketListManager.MergeDocketLists(laufzettel.ToArray(), myMergedFileName);
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
                        LieferscheinePath.Text = "Keine Laufzettel vorhanden!";
                        LieferscheinePath.Visible = true;
                    }
                }

                CheckOpenedOrders();
            }
            catch (Exception ex)
            {
                AbmeldungErrLabel.Visible = true;
                AbmeldungErrLabel.Text = "Fehler: " + ex.Message;
            }
        }

        private void UpdateOrderAfterZulassungsstelle(int customerIdToUpdate, int orderIdToUpdate)
        {
            var customerID = customerIdToUpdate;
            var orderNumber = orderIdToUpdate;
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
            catch (Exception ex)
            {
                AbmeldungErrLabel.Text = "Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator. <br />" + "Error: " + ex.Message;
                AbmeldungErrLabel.Visible = true;
            }
        }

        protected void AbmeldungZulassen_Command(object sender, EventArgs e)
        {
            if (itemIndexHiddenField.Value != null) // falls ausgewählte Row Index gesetz wurde
            {
                GridDataItem selectedItem = RadGridAbmeldung.MasterTableView.Items[Convert.ToInt32(itemIndexHiddenField.Value)];
                selectedItem.Selected = true;
                string VIN = string.Empty,
                    kennzeichen = string.Empty,
                    HSN = string.Empty,
                    TSN = string.Empty;
                int customerId = 0;
                var editButton = sender as Button;
                var item = editButton.NamingContainer as GridEditFormItem;
                var vinBox = item.FindControl("VINBox") as TextBox;
                var orderIdBox = item.FindControl("orderIdBox") as TextBox;
                var kennzeichenBox = item.FindControl("KennzeichenBox") as TextBox;
                var errorCheckBox = item.FindControl("ErrorCheckBox") as CheckBox;
                var errorReasonTextBox = item.FindControl("ErrorReasonTextBox") as TextBox;
                var HSNBox = item.FindControl("HSNAbmFormBox") as TextBox;
                var TSNBox = item.FindControl("TSNAbmFormBox") as TextBox;
                var orderNumber = Int32.Parse(orderIdBox.Text);

                if (!String.IsNullOrEmpty(CustomerDropDownListAbmeldungOffen.SelectedValue.ToString()))
                    customerId = Int32.Parse(CustomerDropDownListAbmeldungOffen.SelectedValue);
                else
                {
                    TextBox customerid = item.FindControl("customerIdBox") as TextBox;
                    customerId = Int32.Parse(customerid.Text);
                }

                AbmeldungErrLabel.Visible = false;
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
                        AbmeldungErrLabel.Text = "Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator. <br /> " + "Error: " + ex.Message;
                        AbmeldungErrLabel.Visible = true;
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
                        updateDataBase(VIN, TSN, HSN, orderNumber, customerId, kennzeichen);
                    }
                    catch (Exception ex)
                    {
                        AbmeldungErrLabel.Text = "Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator. <br /> " + "Error: " + ex.Message;
                        AbmeldungErrLabel.Visible = true;
                    }
                }

                if (Session["orderNumberSearch"] != null)
                    Session["orderNumberSearch"] = string.Empty; //after search should be empty

                RadGridAbmeldung.MasterTableView.ClearChildEditItems();
                RadGridAbmeldung.MasterTableView.ClearEditItems();
                RadGridAbmeldung.Rebind();
            }
        }

        // Ändert das Text von Button entweder nach Fehler oder Zulassung
        protected void ErrorCheckBox_Clicked(object sender, EventArgs e)
        {
            var errorCheckBox = sender as CheckBox;
            var saveButton = errorCheckBox.FindControl("ZulassenButton") as Button;
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
                    catch
                    {
                        AbmeldungErrLabel.Text = "Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator";
                        AbmeldungErrLabel.Visible = true;
                    }
                }
            }
            else
            {
                AbmeldungErrLabel.Visible = true;
            }
        }

        protected void RadGridAbmeldung_PreRender(object sender, EventArgs e)
        {
            HideExpandColumnRecursive(RadGridAbmeldung.MasterTableView);
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
            var count = GetUnfineshedOrdersCount(OrderTypes.Cancellation, OrderStatusTypes.Open);
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
                    OrderManager.UpdateOrderItemAmount(Int32.Parse(itemId), Convert.ToDecimal(amoutToSave));
                }
                catch (Exception ex)
                {
                    throw new Exception("Die ausgewählte Position kann nicht updatet werden <br /> Error: " + ex.Message);
                }
            }
        }
        
        // Order wird updated mit Status 400 (Zulassungstelle)
        protected void UpdateOrderAndItemsStatus()
        {
            if (RadGridAbmeldung.SelectedItems.Count > 0)
            {
                AbmeldungErrLabel.Visible = false;
                AbmeldungOkLabel.Visible = false;

                foreach (GridDataItem item in RadGridAbmeldung.SelectedItems)
                {
                    // Vorbereitung für Update
                    int customerID = 0;

                    if (!String.IsNullOrEmpty(CustomerDropDownListAbmeldungOffen.SelectedValue.ToString()))
                        customerID = Int32.Parse(CustomerDropDownListAbmeldungOffen.SelectedValue);
                    else
                        customerID = Int32.Parse(item["customerID"].Text);

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
                    catch (Exception ex)
                    {
                        AbmeldungErrLabel.Text = "Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator. <br /> " + "Error: " + ex.Message;
                        AbmeldungErrLabel.Visible = true;
                    }
                }
                // erfolgreich
                RadGridAbmeldung.DataBind();
                AbmeldungOkLabel.Visible = true;
            }
            else
            {
                AbmeldungErrLabel.Text = "Sie haben keinen Auftrag ausgewählt!";
                AbmeldungErrLabel.Visible = true;
            }
        }

        // Updating Order before Zulassungstelle
        protected void updateDataBase(string vin, string tsn, string hsn, int orderNumber, int customerId, string kennzeichen)
        {
            var order = DeregistrationOrderManager.GetById(orderNumber);
            order.Vehicle.VIN = vin;
            order.Registration.Licencenumber = kennzeichen;
            order.Vehicle.TSN = tsn;
            order.Vehicle.HSN = hsn;
            DeregistrationOrderManager.SaveChanges();
        }

        //Prüfen ob alle Werte da sind um den Auftrag auf "Zulassungstelle" zu setzen
        private bool CheckIfAllExistsToUpdate()
        {
            bool shouldBeUpdated = true;
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