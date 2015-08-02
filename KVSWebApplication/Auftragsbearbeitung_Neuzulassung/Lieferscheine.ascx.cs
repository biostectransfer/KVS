using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.Data;
using System.Data.SqlClient;
using KVSCommon.Enums;
using KVSWebApplication.BasePages;

namespace KVSWebApplication.Auftragsbearbeitung_Neuzulassung
{
    /// <summary>
    /// Codebehind fuer die Lieferschein Maske
    /// </summary>
    public partial class Lieferscheine : EditOrdersBase
    {
        #region Members  

        protected override RadGrid OrderGrid { get { return this.RadGridLieferscheine; } }
        protected override RadDatePicker RegistrationDatePicker { get { return null; } }
        protected override RadComboBox CustomerTypeDropDown { get { return null; } }
        protected override RadComboBox CustomerDropDown { get { return null; } }
        protected override PermissionTypes PagePermission { get { return PermissionTypes.LOESCHEN_AUFTRAGSPOSITION; } }
        protected override OrderTypes OrderType { get { return OrderTypes.Admission; } }
        protected override OrderStatusTypes OrderStatusType { get { return OrderStatusTypes.Closed; } }
        protected override string OrderStatusSearch { get { return "Offen"; } }
        #endregion

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            string target = Request["__EVENTTARGET"];
            var auftragNeu = Page as AuftragsbearbeitungNeuzulassung;
            var script = auftragNeu.getScriptManager() as RadScriptManager;

            if (!String.IsNullOrEmpty(target))
            {
                if (target.Equals("UserValueConfirmLieferscheine") || target.Equals("UserValueDontConfirmLieferscheine"))
                {
                    UserValueConfirm.Value = null;
                    userAuswahl.Value = target;
                    if (!string.IsNullOrEmpty(userAuswahl.Value) && userAuswahl.Value.Equals("UserValueDontConfirmLieferscheine"))
                    {
                        OffenePanel.Visible = true;
                        NochOffenAuftraegeRadGrid.Enabled = true;
                        NochOffenAuftraegeRadGrid.Rebind();
                    }
                    else if (target.Equals("UserValueConfirmLieferscheine"))
                    {
                        LieferscheinErstellen();
                    }
                }

                if (!target.Contains("LieferungButton") && !target.Contains("Button1"))
                {
                    //RadGridLieferscheine.Rebind();
                }
            }
        }

        /// <summary>
        /// Datasource fuer die Lieferscheine Tabelle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LieferscheineLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetLargeCustomerOrders();
        }

        /// <summary>
        /// Event fuer das selektieren der Lieferscheine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LieferItems_Selected(object sender, EventArgs e)
        {
            if (RadGridLieferscheine.SelectedItems.Count > 0)
            {
                BitteTextBox.Visible = false;
                GridDataItem itemToCheck = RadGridLieferscheine.SelectedItems[0] as GridDataItem;
                bool statusFromCheck = CheckForOpenValues(itemToCheck["CustomerLocation"].Text);
                //Falls es gibt noch values - start javascript und raus
                if (statusFromCheck == true)
                {
                    ScriptManager.RegisterStartupScript(this.Page, typeof(Page), "UserResponce", "Closewindow()", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, typeof(Page), "UserResponce2", "CreatePacking()", true);
                }
            }
            else
            {
                BitteTextBox.Visible = true;
            }
        }

        /// <summary>
        /// Event fuer das hinzufuegen der Adresse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnAddAdressButton_Clicked(object sender, EventArgs e)
        {
            LieferscheinErstellen();
            RadGridLieferscheine.DataBind();
            AllesOkLieferscheine.Visible = true;
        }

        /// <summary>
        /// Event fuer das fertigstellen der Lieferscheine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FertigstellenButton_Clicked(object sender, GridCommandEventArgs e)
        {
            AllesIsOkeyBeiOffene.Visible = false;
            if (e.Item is GridDataItem)
            {
                GridDataItem fertigStellenItem = e.Item as GridDataItem;
                var orderNumber = Int32.Parse(fertigStellenItem["OrderNumber"].Text);
                var customerID = Int32.Parse(fertigStellenItem["customerID"].Text);

                if (!CheckDienstleistungAndAmtGebuhr(orderNumber))
                {
                    ErrorOffeneLabel.Text = "Bei den ausgewählten Auftrag fehlt noch die Dienstleistung und/oder amtliche Gebühr! In dem Reiter 'Zulassungstelle' können Sie den Auftrag bearbeiten. ";
                }
                else if (!CheckIfAllExistsToUpdate(fertigStellenItem))
                {
                    ErrorOffeneLabel.Text = "Fahrzeugdaten sind nicht vollständig! In dem Reiter 'Zulassungstelle' können Sie den Auftrag bearbeiten. ";
                }
                else
                {
                    try
                    {
                        var newOrder = OrderManager.GetById(orderNumber);
                        if (newOrder != null)
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
                            AllesIsOkeyBeiOffene.Visible = true;
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorOffeneLabel.Visible = true;
                        ErrorOffeneLabel.Text = "Fehler: " + ex.Message;
                        //TODO WriteLogItem(ex.Message, LogTypes.ERROR, "Order");
                    }
                    NochOffenAuftraegeRadGrid.Rebind();
                    RadGridLieferscheine.Rebind();
                }
            }
        }

        /// <summary>
        /// Datasource fuer die offenen Lieferscheine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LieferscheineOffeneLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            if (RadGridLieferscheine.SelectedItems.Count > 0)
            {
                var item = RadGridLieferscheine.SelectedItems[0] as GridDataItem;
                var locationId = Int32.Parse(item["locationId"].Text);
                
                e.Result = GetLargeCustomerOrders(OrderTypes.Admission, OrderStatusTypes.AdmissionPoint).Where(o => o.locationId == locationId);
            }
            else if (!String.IsNullOrEmpty(LocationIdHiddenField.Value))
            {
                e.Result = GetLargeCustomerOrders(OrderTypes.Admission, OrderStatusTypes.AdmissionPoint).
                    Where(o => o.CustomerLocation.Equals(LocationIdHiddenField.Value, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                e.Result = new List<string>();

                OffenePanel.Visible = false;
                NochOffenAuftraegeRadGrid.Enabled = false;
                AllesIsOkeyBeiOffene.Visible = false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prüft die Datenbank für Auftraege, die nicht geschlossen sind.
        /// </summary>
        /// <param name="location">Location des Kundes</param>
        /// <returns>Bool (Gefunden oder nicht)</returns>
        protected bool CheckForOpenValues(string location)
        {
            bool statusFromCheck = false;
            if (!String.IsNullOrEmpty(location))
            {
                var locationCount = GetUnfineshedOrders(OrderTypes.Admission, OrderStatusTypes.AdmissionPoint).Count(o => o.LocationId.HasValue);
                if (locationCount > 0)
                {
                    statusFromCheck = true;
                    LocationIdHiddenField.Value = location;
                }
            }
            return statusFromCheck;
        }

        /// <summary>
        /// Lieferschein erstellen
        /// </summary>
        protected void LieferscheinErstellen()
        {
            OffenePanel.Visible = false;
            AllesOkLieferscheine.Visible = false;
            ErrorLabelLieferschein.Visible = false;
            if (RadGridLieferscheine.SelectedItems.Count > 0)
            {
                try
                {
                    var locationIdList = new List<LocationOrderJoins>();
                    foreach (GridDataItem item in RadGridLieferscheine.SelectedItems)
                    {
                        var order = OrderManager.GetById(Int32.Parse(item["OrderNumber"].Text));
                        var join = new LocationOrderJoins();
                        join.LocationId = Int32.Parse(item["locationId"].Text);
                        join.Order = order;
                        locationIdList.Add(join);
                    }

                    var groupedOrder = locationIdList.GroupBy(q => q.LocationId);
                    foreach (var gr in groupedOrder)
                    {
                        var location = LocationManager.GetById(gr.First().LocationId);
                        var packingList = PackingListManager.CreatePackingList(location.Name, location.Adress);

                        // für alle, die selected sind - daten zu packing list
                        foreach (var orders in gr)
                        {
                            PackingListManager.AddOrderById(packingList, orders.Order.OrderNumber);
                        }
                    }

                    RadGridLieferscheine.DataBind();
                    OffenePanel.Visible = false;
                    NochOffenAuftraegeRadGrid.Enabled = false;
                    AllesOkLieferscheine.Visible = true;
                }
                catch (SqlException ex)
                {
                    ErrorLabelLieferschein.Visible = true;
                    ErrorLabelLieferschein.Text = "Fehler: " + ex.Message;
                    //TODO WriteLogItem(ex.Message, LogTypes.ERROR, "Order");
                }
            }
        }

        /// <summary>
        /// Checked if amt.gebühr UND mind.eine Dienstleistung vorhanden ist
        /// </summary>
        /// <param name="OrderNumber"></param>
        /// <returns></returns>
        protected bool CheckDienstleistungAndAmtGebuhr(int orderNumber)
        {
            bool DienstVorhanden = false;
            bool AmtGebuhVorhanden = false;

            var order = OrderManager.GetById(orderNumber);
            if (order != null)
            {
                foreach (OrderItem item in order.OrderItem)
                {
                    if (item.IsAuthorativeCharge == true)
                    {
                        AmtGebuhVorhanden = true;
                    }
                    else if (item.IsAuthorativeCharge == false)
                    {
                        DienstVorhanden = true;
                    }
                }
            }

            if (AmtGebuhVorhanden == true && DienstVorhanden == true)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Prüfen ob alle Werte da sind um den Auftrag auf "Zulassungstelle" zu setzen
        /// </summary>
        /// <param name="fertigStellenItem"></param>
        /// <returns></returns>
        private bool CheckIfAllExistsToUpdate(GridDataItem fertigStellenItem)
        {
            bool shouldBeUpdated = true;
            if (String.IsNullOrEmpty(fertigStellenItem["VIN"].Text))
            {
                shouldBeUpdated = false;
            }
            return shouldBeUpdated;
        }

        #endregion
    }
}