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

namespace KVSWebApplication.Auftragsbearbeitung_Neuzulassung
{
    /// <summary>
    /// Codebehind fuer den Reiter Offen Neuzulassung
    /// </summary>
    public partial class OffenNeuzulassung : System.Web.UI.UserControl
    {

        private string customer = string.Empty;

        RadScriptManager script = null;

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
        List<string> thisUserPermissions = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(Int32.Parse(Session["CurrentUserId"].ToString())));
            bool canDeleteItem = thisUserPermissions.Contains("LOESCHEN_AUFTRAGSPOSITION");
            if (canDeleteItem == false)
            {
                foreach (var table in RadGridOffNeuzulassung.MasterTableView.DetailTables)
                {
                    if (table.GetColumn("RemoveOrderItem") != null)
                        table.GetColumn("RemoveOrderItem").Visible = false;
                }
            }
            AuftragsbearbeitungNeuzulassung auftragNeu = Page as AuftragsbearbeitungNeuzulassung;
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

            //else
            //{
            //    RadGridOffNeuzulassung.Enabled = true;
            //    RadGridOffNeuzulassung.DataBind();
            //}
        }
        protected void CheckOpenedOrders()
        {
            ordersCount.Text = Order.getUnfineshedOrdersCount(new DataClasses1DataContext(), "Zulassung", 100).ToString();
            if (ordersCount.Text == "" || ordersCount.Text == "0")
            {
                go.Visible = false;
            }
            else
            {
                go.Visible = true;
            }
        }
        /// <summary>
        /// Datasource fuer die Uebersichtsgrud
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AbmeldungenLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {

            if (RadComboBoxCustomerOffenNeuzulassung.SelectedValue == "1")
            {

                DataClasses1DataContext con = new DataClasses1DataContext();

                var smallCustomerQuery = from ord in con.Order
                                         join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                         join cust in con.Customer on ord.CustomerId equals cust.Id
                                         join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                         join regord in con.RegistrationOrder on ord.OrderNumber equals regord.OrderNumber
                                         join reg in con.Registration on regord.RegistrationId equals reg.Id
                                         join veh in con.Vehicle on regord.VehicleId equals veh.Id
                                         join smc in con.SmallCustomer on cust.Id equals smc.CustomerId
                                         orderby ord.OrderNumber descending
                                         where ord.Status == 100 && ordtype.Name == "Zulassung" && ord.HasError.GetValueOrDefault(false) != true
                                         select new
                                         {
                                             OrderNumber = ord.OrderNumber,
                                             customerId = cust.Id,
                                             customerID = ord.CustomerId,
                                             CreateDate = ord.CreateDate,
                                             Status = ordst.Name,
                                             CustomerName = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + "  " + cust.SmallCustomer.Person.Name : cust.Name,
                                             CustomerLocation = "",
                                             Kennzeichen = reg.Licencenumber,
                                             VIN = veh.VIN,
                                             TSN = veh.TSN,
                                             HSN = veh.HSN,
                                             OrderTyp = ordtype.Name,
                                             Freitext = ord.FreeText,
                                             Geprueft = ord.Geprueft == null ? "Nein" : "Ja",
                                             Datum = ord.RegistrationOrder.Registration.RegistrationDate
                                         };

                if (CustomerDropDownListOffenNeuzulassung.SelectedValue != string.Empty)
                {
                    var custId = Int32.Parse(CustomerDropDownListOffenNeuzulassung.SelectedValue);
                    smallCustomerQuery = smallCustomerQuery.Where(q => q.customerID == custId);
                }

                e.Result = smallCustomerQuery;
            }

            //select all values for large customers
            else if (RadComboBoxCustomerOffenNeuzulassung.SelectedValue == "2")
            {
                DataClasses1DataContext con = new DataClasses1DataContext();


                var zulassungQuery = from ord in con.Order
                                     join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                     join cust in con.Customer on ord.CustomerId equals cust.Id
                                     join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                     join loc in con.Location on ord.LocationId equals loc.Id
                                     join regord in con.RegistrationOrder on ord.OrderNumber equals regord.OrderNumber
                                     join reg in con.Registration on regord.RegistrationId equals reg.Id
                                     join veh in con.Vehicle on regord.VehicleId equals veh.Id
                                     join lmc in con.LargeCustomer on cust.Id equals lmc.CustomerId
                                     orderby ord.OrderNumber descending
                                     where ord.Status == 100 && ordtype.Name == "Zulassung" && ord.HasError.GetValueOrDefault(false) != true
                                     select new
                                     {
                                         OrderNumber = ord.OrderNumber,
                                         locationId = loc.Id,
                                         customerID = cust.Id,
                                         CreateDate = ord.CreateDate,
                                         Status = ordst.Name,
                                         CustomerName = cust.Name,
                                         Kennzeichen = reg.Licencenumber,
                                         VIN = veh.VIN,
                                         TSN = veh.TSN,
                                         HSN = veh.HSN,
                                         CustomerLocation = loc.Name,
                                         OrderTyp = ordtype.Name,
                                         Freitext = ord.FreeText,
                                         Geprueft = ord.Geprueft == null ? "Nein" : "Ja",
                                         Datum = ord.RegistrationOrder.Registration.RegistrationDate
                                     };

                if (CustomerDropDownListOffenNeuzulassung.SelectedValue != string.Empty)
                {
                    var custId = Int32.Parse(CustomerDropDownListOffenNeuzulassung.SelectedValue);
                    zulassungQuery = zulassungQuery.Where(q => q.customerID == custId);
                }



                if (Session["orderNumberSearch"] != null && Session["orderStatusSearch"] != null)
                {
                    if (!String.IsNullOrEmpty(Session["orderNumberSearch"].ToString()))
                    {
                        if (Session["orderStatusSearch"].ToString().Contains("Offen"))
                        {
                            int orderNumber = Convert.ToInt32(Session["orderNumberSearch"].ToString());
                            zulassungQuery = zulassungQuery.Where(q => q.OrderNumber == orderNumber);

                        }
                    }
                }
                e.Result = zulassungQuery;
            }
        }
        /// <summary>
        /// Small oder Large -> Auswahl der KundenName
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();

            if (RadComboBoxCustomerOffenNeuzulassung.SelectedValue == "1") //Small Customers
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

            else if (RadComboBoxCustomerOffenNeuzulassung.SelectedValue == "2") //Large Customers
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
        /// Datasource fuer die Dienstleistungen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        //protected void CostCenterDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        //{
        //    DataClasses1DataContext con = new DataClasses1DataContext();

        //    var costCenterQuery = from cost in con.CostCenter
        //                          orderby cost.Name 
        //                          select new
        //                          {
        //                              Name = cost.Name,
        //                              Value = cost.Id
        //                          };

        //    e.Result = costCenterQuery;
        //}
        /// <summary>
        /// Datasource fuer die Detailgrid
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RadGridZulOffen_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            var dbContext = new DataClasses1DataContext();
            var item = (GridDataItem)e.DetailTableView.ParentItem;
            var orderNumber = Int32.Parse(item["OrderNumber"].Text);
            
            var positionQuery = from ord in dbContext.Order
                                join orditem in dbContext.OrderItem on ord.OrderNumber equals orditem.OrderNumber
                                let authCharge = dbContext.OrderItem.FirstOrDefault(s => s.SuperOrderItemId == orditem.Id)
                                where ord.OrderNumber == orderNumber && (orditem.SuperOrderItemId == null)
                                select new
                                {
                                    OrderItemId = orditem.Id,
                                    Amount = orditem.Amount == null ? "kein Preis" : (Math.Round(orditem.Amount, 2, MidpointRounding.AwayFromZero)).ToString(),
                                    ProductName = orditem.IsAuthorativeCharge ? orditem.ProductName + " (Amtl.Gebühr)" : orditem.ProductName,
                                    AmtGebuhr = authCharge == null ? false : true,
                                    //AmtGebuhr = orditem.IsAuthorativeCharge,
                                    AuthCharge = authCharge == null || authCharge.Amount == null ? "kein Preis" : (Math.Round(authCharge.Amount, 2, MidpointRounding.AwayFromZero)).ToString(),
                                    AuthChargeId = authCharge == null ? (int?)null : authCharge.Id,
                                    //AmtGebuhr2 = orditem.IsAuthorativeCharge ? "Ja" : "Nein"
                                };
            e.DetailTableView.DataSource = positionQuery;


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

                    DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                    Button button = sender as Button;

                    Price newPrice = null;
                    OrderItem newOrderItem1 = null;

                    RadComboBox productDropDown = button.NamingContainer.FindControl("NewProductDropDownList") as RadComboBox;
                    //DropDownList costCenterDropDown = button.NamingContainer.FindControl("CostCenterDropDownList") as DropDownList;

                    var productId = Int32.Parse(productDropDown.SelectedValue);

                    foreach (GridDataItem item in RadGridOffNeuzulassung.SelectedItems)
                    {
                        var orderNumber = Int32.Parse(item["OrderNumber"].Text);

                        KVSCommon.Database.Product newProduct = dbContext.Product.SingleOrDefault(q => q.Id == productId);

                        if (!String.IsNullOrEmpty(item["locationId"].Text) && item["locationId"].Text.Length > 6)
                        {
                            var locationId = Int32.Parse(item["locationId"].Text);
                            newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == locationId);
                        }

                        if (newPrice == null || String.IsNullOrEmpty(item["locationId"].Text))
                        {
                            newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == null);
                        }

                        var orderToUpdate = dbContext.Order.SingleOrDefault(q => q.OrderNumber == orderNumber);

                        if (newPrice == null || newProduct == null || orderToUpdate == null)
                            throw new Exception("Achtung, die Position kann nicht hinzugefügt werden, es konnte entweder kein Preis, Produkt oder der Auftrag gefunden werden!");


                        if (orderToUpdate != null)
                        {
                            orderToUpdate.LogDBContext = dbContext;
                            var orderItemCostCenter = orderToUpdate.OrderItem.FirstOrDefault(q => q.CostCenter != null);

                            newOrderItem1 = orderToUpdate.AddOrderItem(newProduct.Id, newPrice.Amount, 1,
                                (orderItemCostCenter != null) ? orderItemCostCenter.CostCenter : null,
                            null, false, dbContext);

                            if (newPrice.AuthorativeCharge.HasValue)
                            {
                                orderToUpdate.AddOrderItem(newProduct.Id, newPrice.AuthorativeCharge.Value, 1,
                                    (orderItemCostCenter != null) ? orderItemCostCenter.CostCenter : null,
                                    newOrderItem1.Id, newPrice.AuthorativeCharge.HasValue, dbContext);
                            }
                            dbContext.SubmitChanges();

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
                else if (e.CommandName == "RemoveOrderItem")
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));

                        try
                        {

                            GridEditableItem editedItem = e.Item as GridEditableItem;
                            string itemId = editedItem["ItemIdColumn"].Text;
                            OrderItem.RemoveOrderItem(dbContext, Int32.Parse(itemId));
                            dbContext.SubmitChanges();
                            ts.Complete();

                        }
                        catch (Exception ex)
                        {
                            if (ts != null)
                                ts.Dispose();

                            ZulassungErrLabel.Text = "Fehler: " + ex.Message;
                            ZulassungErrLabel.Visible = true;
                            dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                            dbContext.WriteLogItem("Delete OrderItem Error " + ex.Message, LogTypes.ERROR, "OrderItem");
                            dbContext.SubmitChanges();
                        }


                    }

                    RadGridOffNeuzulassung.Rebind();

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
                    DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));

                    var positionUpdateQuery = dbContext.OrderItem.SingleOrDefault(q => q.Id == orderItemId);
                    positionUpdateQuery.LogDBContext = dbContext;

                    positionUpdateQuery.Amount = Convert.ToDecimal(amoutToSave);
                    // positionUpdateQuery.ProductName = prodNameBox.Text;
                    // positionUpdateQuery.IsAuthorativeCharge = Convert.ToBoolean(amtGeb.Checked);

                    dbContext.SubmitChanges();

                    //RadGridOffNeuzulassung.MasterTableView.ClearEditItems();
                    // RadGridOffNeuzulassung.MasterTableView.ClearChildEditItems();
                    // RadGridOffNeuzulassung.MasterTableView.ClearSelectedItems();
                }

                catch (Exception ex)
                {
                    throw new Exception("Die ausgewählte Position kann nicht updatet werden <br /> Error: " + ex.Message);
                }
            }

            // RadGridOffNeuzulassung.Rebind();
        }
        /// <summary>
        /// Event fuer den Lieferschein erstellen Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ZulassungsstelleLieferscheineButton_Clicked(object sender, EventArgs e)
        {
            TransactionScope ts = null;
            try
            {

                if (String.IsNullOrEmpty(ZulassungsDatumPicker.SelectedDate.ToString()))
                {
                    LieferscheinePath.Text = "Wählen Sie bitte das Zulassungsdatum aus!";
                    LieferscheinePath.Visible = true;
                }
                else
                {


                    List<string> laufzettel = new List<string>();
                    script.RegisterPostBackControl(ZulassungsstelleLieferscheineButton);
                    using (DataClasses1DataContext con = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())))
                    {
                        using (ts = new TransactionScope())
                        {

                            var zulQuery2 = from ord in con.Order
                                            join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                            join regLoc in con.RegistrationLocation on ord.Zulassungsstelle equals regLoc.ID
                                            join regord in con.RegistrationOrder on ord.OrderNumber equals regord.OrderNumber
                                            where ord.Status == 100 && ordtype.Name == "Zulassung" &&
                                            ord.HasError.GetValueOrDefault(false) != true &&
                                            ord.RegistrationOrder.Registration.RegistrationDate <=
                                            ZulassungsDatumPicker.SelectedDate
                                            select ord;

                            var grouptedOrders = zulQuery2.GroupBy(q => q.Zulassungsstelle.Value);

                            foreach (var location in grouptedOrders)
                            {
                                DocketList docketList = new DocketList();
                                MemoryStream ms = new MemoryStream();
                                if (location.Count() > 0)
                                {

                                    docketList = DocketList.CreateDocketList(
                                        location.First().RegistrationLocation.RegistrationLocationName,
                                        location.First().RegistrationLocation.Adress, con);
                                    docketList.LogDBContext = con;
                                    docketList.IsSelfDispatch = true;
                                }
                                foreach (var order in location)
                                {
                                    foreach (var order2 in location)
                                    {
                                        if (order2 != null)
                                        {
                                            docketList.AddOrderById(order2.OrderNumber, con);
                                            //updating order status
                                            order2.LogDBContext = con;
                                            order2.Status = 400;

                                            //updating orderitems status                          
                                            foreach (OrderItem ordItem in order2.OrderItem)
                                            {
                                                ordItem.LogDBContext = con;
                                                if (ordItem.Status != (int)OrderItemState.Storniert)
                                                {
                                                    ordItem.Status = 300;
                                                }
                                            }
                                        }
                                    }

                                    con.SubmitChanges();

                                }

                                string myPackListFileName = EmptyStringIfNull.CheckIfFolderExistsAndReturnPathForPdf(Session["CurrentUserId"].ToString(), true);

                                docketList.Print(ms, string.Empty, con, "/UserData/" + Session["CurrentUserId"].ToString() + "/" + Path.GetFileName(myPackListFileName), true);
                                con.SubmitChanges();
                                string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
                                string host = ConfigurationManager.AppSettings["smtpHost"];
                                //File.WriteAllBytes(myPackListFileName, ms.ToArray());
                                PdfDocument d = PdfReader.Open(new MemoryStream(ms.ToArray(), 0, Convert.ToInt32(ms.Length)));
                                d.Save(myPackListFileName);
                                docketList.SendByEmail(ms, fromEmail, host);
                                docketList = null;
                                d = null;
                                ms.Close();
                                ms = null;

                                laufzettel.Add(myPackListFileName);
                                // break;
                            }
                            ts.Complete();
                        }
                        RadGridOffNeuzulassung.MasterTableView.ClearChildEditItems();
                        RadGridOffNeuzulassung.MasterTableView.ClearEditItems();
                        RadGridOffNeuzulassung.Rebind();
                        CheckOpenedOrders();
                        if (laufzettel.Count > 1)
                        {
                            string myMergedFileName = EmptyStringIfNull.CheckIfFolderExistsAndReturnPathForPdf(Session["CurrentUserId"].ToString(), true);
                            PackingList.MergePackingLists(laufzettel.ToArray(), myMergedFileName);

                            myMergedFileName = myMergedFileName.Replace(ConfigurationManager.AppSettings["BasePath"], ConfigurationManager.AppSettings["BaseUrl"]);
                            // myInvoiceListFileName = myInvoiceListFileName.Replace("//", "/");
                            //myMergedFileName = myMergedFileName.Replace(@"\", "");
                            //myMergedFileName = myMergedFileName.Replace(@"/\", @"/");
                            //myMergedFileName = myMergedFileName.Replace(@"\", @"/");
                            myMergedFileName = myMergedFileName.Replace(@"\\", @"/");
                            myMergedFileName = myMergedFileName.Replace(@"\", @"/");
                            LieferscheinePath.Text = "<a href=" + '\u0022' + myMergedFileName + '\u0022' + " target=" + '\u0022' + "_blank" + '\u0022' + "> Laufzettel öffnen</a>";
                            LieferscheinePath.Visible = true;
                        }
                        else if (laufzettel.Count == 1)
                        {
                            string myMergedFileName = laufzettel[0];
                            //PackingList.MergePackingLists(lieferscheine.ToArray(), myMergedFileName);

                            myMergedFileName = myMergedFileName.Replace(ConfigurationManager.AppSettings["BasePath"], ConfigurationManager.AppSettings["BaseUrl"]);
                            // myInvoiceListFileName = myInvoiceListFileName.Replace("//", "/");
                            //myMergedFileName = myMergedFileName.Replace(@"\", "");
                            //myMergedFileName = myMergedFileName.Replace(@"/\", @"/");
                            //myMergedFileName = myMergedFileName.Replace(@"\", @"/");
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
            }
            catch (Exception ex)
            {
                if (ts != null)
                    ts.Dispose();

                ZulassungErrLabel.Visible = true;
                ZulassungErrLabel.Text = "Fehler: " + ex.Message;

            }
        }
        /// <summary>
        /// Aktualisiere den Auftragsstatus
        /// </summary>
        /// <param name="customerIdToUpdate">KundeID</param>
        /// <param name="orderNumberToUpdate">Auftragsid</param>
        private void UpdateOrderAfterZulassungsstelle(int customerIdToUpdate, int orderNumberToUpdate)
        {
            var customerID = customerIdToUpdate;
            var orderNumber = orderNumberToUpdate;

            try
            {
                DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));

                var newOrder = dbContext.Order.Single(q => q.CustomerId == customerID && q.OrderNumber == orderNumber);

                if (newOrder != null)
                {
                    //updating order status
                    newOrder.LogDBContext = dbContext;
                    newOrder.Status = 400;

                    //updating orderitems status                          
                    foreach (OrderItem ordItem in newOrder.OrderItem)
                    {
                        ordItem.LogDBContext = dbContext;
                        if (ordItem.Status != (int)OrderItemState.Storniert)
                        {
                            ordItem.Status = 300;
                        }
                    }

                    dbContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                ZulassungErrLabel.Text = "Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator. <br />" + "Fehler: " + ex.Message;
                ZulassungErrLabel.Visible = true;
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

                int customerId = 0;

                Button editButton = sender as Button;
                GridEditFormItem item = editButton.NamingContainer as GridEditFormItem;

                TextBox vinBox = item.FindControl("VINBox") as TextBox;
                TextBox orderIdBox = item.FindControl("orderIdBox") as TextBox;
                TextBox kennzeichenBox = item.FindControl("KennzeichenBox") as TextBox;
                CheckBox errorCheckBox = item.FindControl("ErrorCheckBox") as CheckBox;
                TextBox errorReasonTextBox = item.FindControl("ErrorReasonTextBox") as TextBox;
                TextBox HSNBox = item.FindControl("HSNAbmBox") as TextBox;
                TextBox TSNBox = item.FindControl("TSNAbmBox") as TextBox;

                var orderNumber = Int32.Parse(orderIdBox.Text);

                if (!String.IsNullOrEmpty(CustomerDropDownListOffenNeuzulassung.SelectedValue.ToString()))
                    customerId = Int32.Parse(CustomerDropDownListOffenNeuzulassung.SelectedValue);
                else
                    customerId = Int32.Parse(item["customerID"].Text);

                ZulassungErrLabel.Visible = false;

                if (errorCheckBox.Checked) // falls Auftrag als Fehler gemeldet sollte
                {
                    string errorReason = errorReasonTextBox.Text;

                    try
                    {
                        DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));

                        var OrderToUpdate = dbContext.Order.SingleOrDefault(q => q.OrderNumber == orderNumber && q.CustomerId == customerId);
                        OrderToUpdate.LogDBContext = dbContext;
                        OrderToUpdate.HasError = true;
                        OrderToUpdate.ErrorReason = errorReason;
                        dbContext.SubmitChanges();

                        //RadGridOffNeuzulassung.MasterTableView.ClearChildEditItems();
                        //RadGridOffNeuzulassung.MasterTableView.ClearEditItems();
                        //RadGridOffNeuzulassung.Rebind();
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
                        updateDataBase(VIN, TSN, HSN, orderNumber, customerId, kennzeichen);

                        //UpdateOrderAndItemsStatus();


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
        ///  Aktualisiere Auftragsstatus auf 400 (Zulassungstelle)
        /// </summary>
        protected void UpdateOrderAndItemsStatus()
        {
            if (RadGridOffNeuzulassung.SelectedItems.Count > 0)
            {
                ZulassungErrLabel.Visible = false;
                ZulassungOkLabel.Visible = false;

                //if (CheckIfAllExistsToUpdate())
                //{
                foreach (GridDataItem item in RadGridOffNeuzulassung.SelectedItems)
                {
                    // Vorbereitung für Update
                    int customerID = 0;
                    if (!String.IsNullOrEmpty(CustomerDropDownListOffenNeuzulassung.SelectedValue.ToString()))
                        customerID = Int32.Parse(CustomerDropDownListOffenNeuzulassung.SelectedValue);
                    else
                        customerID = Int32.Parse(item["customerID"].Text);
                    var orderNumber = Int32.Parse(item["OrderNumber"].Text);

                    try
                    {
                        DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));

                        var newOrder = dbContext.Order.Single(q => q.CustomerId == customerID && q.OrderNumber == orderNumber);

                        if (newOrder != null)
                        {
                            //updating order status
                            newOrder.LogDBContext = dbContext;
                            newOrder.Status = 400;

                            //updating orderitems status                          
                            foreach (OrderItem ordItem in newOrder.OrderItem)
                            {
                                ordItem.LogDBContext = dbContext;
                                if (ordItem.Status != (int)OrderItemState.Storniert)
                                {
                                    ordItem.Status = 300;
                                }
                            }

                            dbContext.SubmitChanges();
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
                //}
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
        protected void updateDataBase(string vin, string tsn, string hsn, int orderNumber, int customerId, string kennzeichen)
        {
            using (DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())))
            {
                var orderToUpdate = dbContext.RegistrationOrder.Single(q => q.OrderNumber == orderNumber);

                orderToUpdate.LogDBContext = dbContext;
                orderToUpdate.Vehicle.LogDBContext = dbContext;
                orderToUpdate.Registration.LogDBContext = dbContext;
                orderToUpdate.Order.LogDBContext = dbContext;

                orderToUpdate.Vehicle.VIN = vin;
                orderToUpdate.Registration.Licencenumber = kennzeichen;
                orderToUpdate.Vehicle.TSN = tsn;
                orderToUpdate.Vehicle.HSN = hsn;
                //orderToUpdate.Order.Geprueft = true;

                dbContext.SubmitChanges();
            }
        }

        /// <summary>
        /// Prüfen ob alle Werte da sind um den Auftrag auf "Zulassungstelle" zu setzen
        /// </summary>
        /// <returns></returns>
        private bool CheckIfAllExistsToUpdate()
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
                        DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                        var newOrder = dbContext.Order.SingleOrDefault(q => q.OrderNumber == orderNumber);

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
    }
}