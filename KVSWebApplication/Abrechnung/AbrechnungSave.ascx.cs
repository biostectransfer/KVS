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
using KVSCommon.Enums;
using KVSWebApplication.BasePages;
using System.Collections;

namespace KVSWebApplication.Abrechnung
{
    public class OrderItemViewModel
    {
        public int OrderNumber { get; set; }
        public int OrderItemId { get; set; }
        public int? CostCenterId { get; set; }
        public string Location { get; set; }
        public int ItemCount { get; set; }
        public decimal Amount { get; set; }
        public string ProductName { get; set; }
        public string ItemStatus { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public int? OrderLocation { get; set; }
        public DateTime? OrderDate { get; set; }
        public int ItemStatusId { get; set; }
        public int OrderStatusId { get; set; }
    }

    public partial class AbrechnungSave : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var abr = Page as Abrechnung;
            var script = abr.getScriptManager() as RadScriptManager;
            script.RegisterPostBackControl(btnPreviewInvoice);
            if (!Page.IsPostBack)
            {
                Session["currentLocationIndex"] = 0;
            }
        }

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
            if (RadComboBoxCustomer.SelectedValue == "1") //Small Customers
            {
                var customerQuery = CustomerManager.GetEntities(o => o.SmallCustomer != null).
                    Select(cust => new
                    {
                        Name = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name,
                        Value = cust.Id,
                        Matchcode = cust.MatchCode,
                        Kundennummer = cust.CustomerNumber
                    });

                e.Result = customerQuery.ToList();
            }
            else if (RadComboBoxCustomer.SelectedValue == "2") //Large Customers
            {
                var customerQuery = CustomerManager.GetEntities(o => o.LargeCustomer != null).
                    Select(cust => new
                    {
                        Name = cust.Name,
                        Value = cust.Id,
                        Matchcode = cust.MatchCode,
                        Kundennummer = cust.CustomerNumber
                    });

                e.Result = customerQuery.ToList();
            }
        }

        /// <summary>
        /// Datasource fuer die Abrechnungsgrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AbrechnungLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            int customerId = 0;
            if (String.IsNullOrEmpty(StandortDropDown.SelectedValue))
            {
                StandortDropDown.DataBind();
            }
            else
            {
                customerId = Int32.Parse(CustomerDropDownList.SelectedValue);
            }

            //select all values for small customers
            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue) && RadComboBoxCustomer.SelectedValue == "1")
            {
                var orderItems = GetOrderItems(customerId).Where(o => o.ItemStatusId == (int)OrderItemStatusTypes.Closed);

                e.Result = orderItems.OrderByDescending(o => o.OrderNumber).ToList();
            }
            //select all values for large customers
            else if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue) && RadComboBoxCustomer.SelectedValue == "2")
            {
                if (RechnungsTypComboBox.SelectedValue != "Einzel")
                {
                    RadGridAbrechnung.AllowMultiRowSelection = true;
                }

                if (RechnungsTypComboBox.SelectedValue == "Einzel")
                {
                    var orderItems = GetOrderItems(customerId).Where(o => o.ItemStatusId == (int)OrderItemStatusTypes.Closed &&
                        (o.OrderStatusId == (int)OrderStatusTypes.Closed || o.OrderStatusId == (int)OrderStatusTypes.PartiallyPayed));

                    if (AllLocationsCheckBox.Checked == false)
                    {
                        if (!String.IsNullOrEmpty(StandortDropDown.SelectedValue.ToString()))
                        {
                            orderItems = orderItems.Where(q => q.OrderLocation == Int32.Parse(StandortDropDown.SelectedValue));
                        }
                    }

                    RadGridAbrechnung.AllowMultiRowSelection = false;

                    e.Result = orderItems.OrderByDescending(o => o.OrderNumber).ToList();
                }
                else if (RechnungsTypComboBox.SelectedValue == "Sammel")
                {
                    var orderItems = GetOrderItems(customerId).Where(o => o.ItemStatusId == (int)OrderItemStatusTypes.Closed &&
                        (o.OrderStatusId == (int)OrderStatusTypes.Closed || o.OrderStatusId == (int)OrderStatusTypes.PartiallyPayed));

                    if (AllLocationsCheckBox.Checked == false)
                    {
                        if (!String.IsNullOrEmpty(StandortDropDown.SelectedValue.ToString()))
                        {
                            orderItems = orderItems.Where(q => q.OrderLocation == Int32.Parse(StandortDropDown.SelectedValue));
                        }
                    }

                    e.Result = orderItems.OrderByDescending(o => o.OrderNumber).ToList();
                }
                else if (RechnungsTypComboBox.SelectedValue == "Woche")
                {
                    DayOfWeek weekStart = DayOfWeek.Monday;
                    DateTime startingDate = DateTime.Today;

                    while (startingDate.DayOfWeek != weekStart)
                        startingDate = startingDate.AddDays(-1);

                    DateTime endDate = startingDate.AddDays(7);

                    var orderItems = GetOrderItems(customerId).Where(o => o.ItemStatusId == (int)OrderItemStatusTypes.Closed &&
                        (o.OrderStatusId == (int)OrderStatusTypes.Closed || o.OrderStatusId == (int)OrderStatusTypes.PartiallyPayed) &&
                        o.ExecutionDate.Value > startingDate && o.ExecutionDate < endDate);

                    if (AllLocationsCheckBox.Checked == false)
                    {
                        if (!String.IsNullOrEmpty(StandortDropDown.SelectedValue.ToString()))
                        {
                            orderItems = orderItems.Where(q => q.OrderLocation == Int32.Parse(StandortDropDown.SelectedValue));
                        }
                    }

                    e.Result = orderItems.OrderByDescending(o => o.OrderNumber).ToList();
                }
                else if (RechnungsTypComboBox.SelectedValue == "Monat")
                {
                    var orderItems = GetOrderItems(customerId).Where(o => o.ItemStatusId == (int)OrderItemStatusTypes.Closed &&
                        (o.OrderStatusId == (int)OrderStatusTypes.Closed || o.OrderStatusId == (int)OrderStatusTypes.PartiallyPayed) &&
                        o.ExecutionDate.Value.Month == DateTime.Now.Month);

                    if (AllLocationsCheckBox.Checked == false)
                    {
                        if (!String.IsNullOrEmpty(StandortDropDown.SelectedValue.ToString()))
                        {
                            orderItems = orderItems.Where(q => q.OrderLocation == Int32.Parse(StandortDropDown.SelectedValue));
                        }
                    }

                    e.Result = orderItems.OrderByDescending(o => o.OrderNumber).ToList();
                }
                else
                {
                    var orderItems = GetOrderItems(customerId);

                    if (AllLocationsCheckBox.Checked == false)
                    {
                        orderItems = orderItems.Where(q => q.OrderLocation == Int32.Parse(StandortDropDown.SelectedValue));
                    }

                    e.Result = orderItems.OrderByDescending(o => o.OrderNumber).ToList();
                }
            }
            else
            {
                //empty query
                e.Result = new List<OrderItemViewModel>();
            }
        }

        protected IQueryable<OrderItemViewModel> GetOrderItems(int customerId)
        {
            return OrderManager.GetEntities(o => o.CustomerId == customerId).
                    SelectMany(o => o.OrderItem).
                    Select(ordItem => new OrderItemViewModel()
                    {
                        OrderNumber = ordItem.OrderNumber,
                        OrderItemId = ordItem.Id,
                        CostCenterId = ordItem.CostCenterId,
                        Location = "",
                        ItemCount = ordItem.Count,
                        Amount = ordItem.Amount,
                        ProductName = ordItem.ProductName,
                        ItemStatus = ordItem.OrderItemStatus.Name,
                        ExecutionDate = ordItem.Order.ExecutionDate,
                        OrderLocation = ordItem.Order.LocationId,
                        OrderDate = ordItem.Order.ExecutionDate,
                        ItemStatusId = ordItem.Status,
                        OrderStatusId = ordItem.Order.Status
                    }).AsQueryable();
        }

        #endregion

        #region Button Clicked

        protected bool SetValuesForAdressWindow()
        {
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
                var virtualItems = new List<SelectedInvoiceItems>();
                var currItem = new SelectedInvoiceItems();
                foreach (GridDataItem item in RadGridAbrechnung.SelectedItems)
                {
                    currItem = new SelectedInvoiceItems();
                    currItem.ProductName = item["ProductName"].Text;
                    currItem.Amount = decimal.Parse(item["Amount"].Text, NumberStyles.Currency).ToString();
                    currItem.OrderItemId = Int32.Parse(item["OrderItemId"].Text);
                    if (!String.IsNullOrEmpty(item["CostCenterId"].Text))
                    {
                        currItem.CostCenterId = Int32.Parse(item["CostCenterId"].Text);
                    }
                    currItem.ItemCount = Convert.ToInt32(item["ItemCount"].Text);
                    currItem.OrderLocationName = item["Location"].Text;
                    currItem.OrderNumber = Int32.Parse(item["OrderNumber"].Text);
                    currItem.OrderLocationId = String.IsNullOrEmpty(item["OrderLocation"].Text) ? (int?)null : Int32.Parse(item["OrderLocation"].Text);
                    virtualItems.Add(currItem);
                }

                var groupedInvoiceItems = virtualItems.GroupBy(q => q.OrderLocationId).ToList();
                if (groupedInvoiceItems.Count > 0 && Session["currentLocationIndex"] != null && EmptyStringIfNull.IsNumber(Session["currentLocationIndex"].ToString()))
                {
                    if (String.IsNullOrEmpty(groupedInvoiceItems[((int)Session["currentLocationIndex"])].Key.ToString()))
                    {
                        newAdress = InvoiceManager.GetInitialInvoiceAdress(Int32.Parse(CustomerDropDownList.SelectedValue),
                            Int32.Parse(groupedInvoiceItems[((int)Session["currentLocationIndex"])].Key.ToString()));
                    }
                    else
                    {
                        newAdress = InvoiceManager.GetInitialInvoiceAdress(Int32.Parse(CustomerDropDownList.SelectedValue), null);
                    }

                    StreetTextBox.Text = newAdress.Street;
                    StreetNumberTextBox.Text = newAdress.StreetNumber;
                    ZipcodeTextBox.Text = newAdress.Zipcode;
                    CityTextBox.Text = newAdress.City;
                    CountryTextBox.Text = newAdress.Country;

                    FooterTextBox.Text = InvoiceManager.GetDefaultInvoiceText(Int32.Parse(CustomerDropDownList.SelectedValue));

                    InvoiceRecipient.Text = groupedInvoiceItems[((int)Session["currentLocationIndex"])].First().OrderLocationName == "&nbsp;" ? "" :
                        groupedInvoiceItems[((int)Session["currentLocationIndex"])].First().OrderLocationName;
                    AllesIstOkeyLabel.Text = string.Empty;
                }
                else
                {
                    throw new Exception("Achtung die Session ist abgelaufen! Bitte loggen Sie sich erneut ein.");
                }
            }
            catch (Exception ex)
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

        private InvoiceType getFullInvoiceName(string selectCase)
        {
            InvoiceType result;
            switch (selectCase)
            {
                case "Einzel":
                    result = InvoiceType.Single;
                    break;
                case "Sammel":
                    result = InvoiceType.Collection;
                    break;
                case "Monat":
                    result = InvoiceType.Monthly;
                    break;
                case "Woche":
                    result = InvoiceType.Weekly;
                    break;
                case "Frei":
                    result = InvoiceType.Single;//TODO?
                    break;
                default:
                    throw new NotImplementedException();
            }
            return result;
        }

        // Create new Adress in der DatenBank
        protected void OnAddAdressButton_Clicked(object sender, EventArgs e)
        {
            GenerateInvoice(false);
        }

        protected void btnPreviewInvoice_Clicked(object sender, EventArgs e)
        {
            GenerateInvoice(true);
        }

        protected void Cell_Selected(object sender, EventArgs e)
        {
            var item = RadGridAbrechnung.SelectedItems[0] as GridDataItem;
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

        private void GenerateInvoice(bool preview)
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
            var customerId = Int32.Parse(CustomerDropDownList.SelectedValue);
            street = StreetTextBox.Text;
            streetNumber = StreetNumberTextBox.Text;
            zipcode = ZipcodeTextBox.Text;
            city = CityTextBox.Text;
            country = CountryTextBox.Text;
            invoiceRecipient = InvoiceRecipient.Text;
            footerText = FooterTextBox.Text;

            try
            {
                var newAdress = AdressManager.CreateAdress(street, streetNumber, zipcode, city, country);
                var newInvoice = InvoiceManager.CreateInvoice(invoiceRecipient,
                    newAdress, Int32.Parse(CustomerDropDownList.SelectedValue), null, getFullInvoiceName(RechnungsTypComboBox.SelectedValue));
                newInvoice.InvoiceText = footerText;

                //Submiting new Invoice and Adress
                if (!preview)
                {
                    InvoiceManager.SaveChanges();
                }
                else
                {
                    newInvoice.Adress = newAdress;
                    newInvoice.Customer = CustomerManager.GetById(Int32.Parse(CustomerDropDownList.SelectedValue));
                }

                var virtualItems = new List<SelectedInvoiceItems>();
                var currItem = new SelectedInvoiceItems();
                foreach (GridDataItem item in RadGridAbrechnung.SelectedItems)
                {
                    currItem = new SelectedInvoiceItems();
                    currItem.ProductName = item["ProductName"].Text;
                    currItem.Amount = decimal.Parse(item["Amount"].Text, NumberStyles.Currency).ToString();

                    currItem.OrderItemId = Int32.Parse(item["OrderItemId"].Text);

                    if (!String.IsNullOrEmpty(item["CostCenterId"].Text))
                    {
                        currItem.CostCenterId = Int32.Parse(item["CostCenterId"].Text);
                    }

                    currItem.ItemCount = Convert.ToInt32(item["ItemCount"].Text);
                    currItem.OrderLocationName = item["Location"].Text == "&nbsp;" ? "" : item["Location"].Text;
                    currItem.OrderNumber = Int32.Parse(item["OrderNumber"].Text);
                    currItem.OrderLocationId = String.IsNullOrEmpty(item["OrderLocation"].Text) ? (int?)null : Int32.Parse(item["OrderLocation"].Text);
                    virtualItems.Add(currItem);
                }

                var groupedInvoiceItems = virtualItems.GroupBy(q => q.OrderLocationId.ToString()).ToList();
                if (groupedInvoiceItems.Count > 0 && Session["currentLocationIndex"] != null && EmptyStringIfNull.IsNumber(Session["currentLocationIndex"].ToString()))
                {
                    var customer = CustomerManager.GetById(customerId);

                    foreach (var item in groupedInvoiceItems[((int)Session["currentLocationIndex"])])
                    {
                        CostCenter costCenter = null;
                        if (item.CostCenterId.HasValue)
                        {
                            costCenter = CostCenterManager.GetById(item.CostCenterId.Value);
                        }

                        var orderItem = OrderManager.GetOrderItems().FirstOrDefault(o => o.Id == item.OrderItemId);

                        var newInvoiceItem = InvoiceManager.AddInvoiceItem(newInvoice, item.ProductName, Convert.ToDecimal(item.Amount), item.ItemCount, orderItem, costCenter,
                            customer, OrderItemStatusTypes.Payed);
                        if (newInvoiceItem != null)
                        {
                            UpdateOrderStatus(item.OrderNumber);
                        }
                    }


                    if (preview && groupedInvoiceItems.Count > 0)
                    {
                        using (MemoryStream memS = new MemoryStream())
                        {
                            string fileName = "RechnungsVorschau_" + DateTime.Now.Ticks.ToString() + ".pdf";
                            string serverPath = ConfigurationManager.AppSettings["DataPath"] + "\\UserData";

                            InvoiceManager.PrintPreview(newInvoice, memS, "");

                            if (!Directory.Exists(serverPath))
                                Directory.CreateDirectory(serverPath);

                            if (!Directory.Exists(serverPath + "\\" + Session["CurrentUserId"].ToString()))
                                Directory.CreateDirectory(serverPath + "\\" + Session["CurrentUserId"].ToString());

                            serverPath = serverPath + "\\" + Session["CurrentUserId"].ToString();
                            File.WriteAllBytes(serverPath + "\\" + fileName, memS.ToArray());
                            OpenPrintfile(fileName);
                        }
                    }

                    AllesIstOkeyLabel.Text = "Rechnung für " + customer.Name + " wurde erfolgreich hinzugefügt";

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
                //try
                //{
                //    if (newInvoice != null)
                //    {

                //        dbContext.Invoice.DeleteOnSubmit(newInvoice);
                //        dbContext.SubmitChanges();
                //        if (newAdress != null)
                //            dbContext.Adress.DeleteOnSubmit(newAdress);
                //        dbContext.SubmitChanges();
                //    }
                //}
                //catch { AbrechnungSaveErrorLabel.Text = "Leider war es nicht möglich durch das System die defekte Rechnung zu löschen (Datenbankkonflikt), Sie können diese auch manuell  im Reiter 'Rechnung erstellen' stornieren"; }

                AbrechnungSaveErrorLabel.Visible = true;
                AbrechnungSaveErrorLabel.Text += "Fehler:" + ex.Message;
            }
        }

        private void OpenPrintfile(string myFile)
        {
            string url = ConfigurationManager.AppSettings["BaseUrl"];
            string path = url + "UserData/" + Session["CurrentUserId"].ToString() + "/" + myFile;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Invoice", "<script>openFile('" + path + "');</script>", false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Orders werden nach OrderItemStatus geprüft
        /// Falls nicht alle Items abgerechnet sind - Status 700 (Teilabgerechnet)
        /// Falls alle Items schon abgerechnet - Status 900 (Abgerechnet)
        /// </summary>
        /// <param name="OrderNumber"></param>
        protected void UpdateOrderStatus(int orderNumber)
        {
            bool shouldBeUpdated = false;
            bool hasAbgerechnetItem = false;
            var order = OrderManager.GetById(orderNumber);

            foreach (OrderItem item in order.OrderItem)
            {
                if (item.Status == (int)OrderItemStatusTypes.Closed)
                {
                    shouldBeUpdated = true;
                }

                if (item.Status == (int)OrderItemStatusTypes.Payed)
                {
                    hasAbgerechnetItem = true;
                }
            }

            if (shouldBeUpdated == true && hasAbgerechnetItem == true) // teil
            {
                order.Status = (int)OrderStatusTypes.PartiallyPayed;
            }
            else if (shouldBeUpdated == false && hasAbgerechnetItem == true) //komplet abgerechnet
            {
                order.Status = (int)OrderStatusTypes.Payed;
            }

            OrderManager.SaveChanges();
        }

        /// <summary>
        /// Datasource fuer den Standort
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void StandortLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            int? customerId = null;
            if (!String.IsNullOrEmpty(CustomerDropDownList.SelectedValue))
            {
                customerId = Int32.Parse(CustomerDropDownList.SelectedValue);
            }

            var locations = LocationManager.GetEntities(loc => loc.CustomerId == customerId).Select(loc => new
            {
                Value = loc.Id,
                Name = loc.Name
            }).ToList();

            e.Result = locations;
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
            if (AllLocationsCheckBox.Checked == true)
            {
                StandortDropDown.Enabled = false;
                GridGroupByExpression expression1 = GridGroupByExpression.Parse("Group By Location");
                GridGroupByField existing = expression1.SelectFields.FindByName("Location");
                if (existing == null) //field is not present
                {
                    //Construct and add a new aggregate field
                    var field = new GridGroupByField();
                    field.FieldName = "Location";
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

        #endregion
    }
}