using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.Data.SqlClient;
using KVSCommon.Enums;
using KVSWebApplication.Auftragsbearbeitung_Neuzulassung;

namespace KVSWebApplication.Nachbearbeitung_Abmeldung
{
    /// <summary>
    /// Codebehind fuer den Reiter Lieferschein Abmeldung
    /// </summary>
    public partial class LieferscheinAbmeldung : EditOrdersBase
    {
        #region Members  

        protected override RadGrid OrderGrid { get { return this.RadGridLieferscheine; } }
        protected override RadDatePicker RegistrationDatePicker { get { return null; } }
        protected override RadComboBox CustomerTypeDropDown { get { return null; } }
        protected override RadComboBox CustomerDropDown { get { return null; } }
        protected override PermissionTypes PagePermission { get { return PermissionTypes.LOESCHEN_AUFTRAGSPOSITION; } }
        protected override OrderTypes OrderType { get { return OrderTypes.Cancellation; } }
        protected override OrderStatusTypes OrderStatusType { get { return OrderStatusTypes.Closed; } }
        protected override string OrderStatusSearch { get { return "Offen"; } }
        #endregion

        #region Event handlers     

        protected void Page_Load(object sender, EventArgs e)
        {
            string target = Request["__EVENTTARGET"];
            NachbearbeitungAbmeldung auftragNeu = Page as NachbearbeitungAbmeldung;
            script = auftragNeu.getScriptManager() as RadScriptManager;
            script.RegisterPostBackControl(Button1);
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
                }
            }
        }

        protected void LieferscheineLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetLargeCustomerOrders();
        }

        protected void LieferItems_Selected(object sender, EventArgs e)
        {
            if (RadGridLieferscheine.SelectedItems.Count > 0)
            {
                BitteTextBox.Visible = false;
                var itemToCheck = RadGridLieferscheine.SelectedItems[0] as GridDataItem;
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

        protected void OnAddAdressButton_Clicked(object sender, EventArgs e)
        {
            AllesIstOkeyLabelLieferschein.Visible = true;
            LieferscheinErstellen();
            RadGridLieferscheine.Rebind();
        }

        protected void LieferscheineOffeneLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            if (RadGridLieferscheine.SelectedItems.Count > 0)
            {
                var item = RadGridLieferscheine.SelectedItems[0] as GridDataItem;
                var locationId = Int32.Parse(item["locationId"].Text);

                e.Result = GetLargeCustomerOrders(OrderTypes.Cancellation, OrderStatusTypes.AdmissionPoint).Where(o => o.locationId == locationId);
            }
            else if (!String.IsNullOrEmpty(LocationIdHiddenField.Value))
            {
                e.Result = GetLargeCustomerOrders(OrderTypes.Cancellation, OrderStatusTypes.AdmissionPoint).
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

        protected void FertigstellenButton_Clicked(object sender, GridCommandEventArgs e)
        {
            AllesIsOkeyBeiOffene.Visible = false;
            if (e.Item is GridDataItem)
            {
                var fertigStellenItem = e.Item as GridDataItem;
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
                        AllesIstOkeyLabel.Text = "Lieferschein wurde erfolgreich erstellt.";
                    }
                    catch (SqlException ex)
                    {
                        ErrorOffeneLabel.Text = "Fehler: " + ex.Message; ;
                    }
                    RadGridLieferscheine.Rebind();
                    NochOffenAuftraegeRadGrid.Rebind();
                }
            }
        }

        #endregion

        #region Methods

        //Checked if amt.gebühr UND mind.eine Dienstleistung vorhanden ist
        protected bool CheckDienstleistungAndAmtGebuhr(int orderNumber)
        {
            bool DienstVorhanden = false;
            bool AmtGebuhVorhanden = false;

            var searchOrderQuery = OrderManager.GetById(orderNumber);
            if (searchOrderQuery != null)
            {
                foreach (OrderItem item in searchOrderQuery.OrderItem)
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

        //Prüfen ob alle Werte da sind um den Auftrag auf "Zulassungstelle" zu setzen
        private bool CheckIfAllExistsToUpdate(GridDataItem fertigStellenItem)
        {
            bool shouldBeUpdated = true;
            if (String.IsNullOrEmpty(fertigStellenItem["VIN"].Text))
            {
                shouldBeUpdated = false;
            }
            return shouldBeUpdated;
        }
        
        /// <summary>
        /// Prüft die Datenbank für Aufträge, die nicht geschlossen sind.
        /// </summary>
        /// <param name="location">Location des Kundes</param>
        /// <returns>Bool (Gefunden oder nicht)</returns>
        protected bool CheckForOpenValues(string location)
        {
            bool statusFromCheck = false;
            if (!String.IsNullOrEmpty(location))
            {
                var locationCount = GetUnfineshedOrders(OrderTypes.Cancellation, OrderStatusTypes.AdmissionPoint).Count(o => o.LocationId.HasValue);
                if (locationCount > 0)
                {
                    statusFromCheck = true;
                    LocationIdHiddenField.Value = location;
                }
            }
            return statusFromCheck;
        }

        //erstellt die Lieferscheine
        protected void LieferscheinErstellen()
        {
            OffenePanel.Visible = false;
            AllesIstOkeyLabelLieferschein.Visible = false;
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

                        // für alle, die selected sind - ins package list hinzufügen
                        foreach (var orders in gr)
                        {
                            PackingListManager.AddOrderById(packingList, orders.Order.OrderNumber);
                        }

                        RadGridLieferscheine.DataBind();
                        OffenePanel.Visible = false;
                        NochOffenAuftraegeRadGrid.Enabled = false;
                        AllesIstOkeyLabelLieferschein.Visible = true;
                    }
                }
                catch (SqlException ex)
                {
                    ErrorLabelLieferschein.Visible = true;
                    ErrorLabelLieferschein.Text = "Fehler: " + ex.Message;
                }
            }
        }

        #endregion
    }
}