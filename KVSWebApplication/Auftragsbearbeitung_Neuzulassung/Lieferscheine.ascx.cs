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

namespace KVSWebApplication.Auftragsbearbeitung_Neuzulassung
{
    /// <summary>
    /// Codebehind fuer die Lieferschein Maske
    /// </summary>
    public partial class Lieferscheine : System.Web.UI.UserControl
    {
        RadScriptManager script = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            string target = Request["__EVENTTARGET"];
            AuftragsbearbeitungNeuzulassung auftragNeu = Page as AuftragsbearbeitungNeuzulassung;
            script = auftragNeu.getScriptManager() as RadScriptManager;
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
            DataClasses1DataContext con = new DataClasses1DataContext();
            var largeCustomerQuery = from ord in con.Order
                                     join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                     join cust in con.Customer on ord.CustomerId equals cust.Id
                                     join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                     join loc in con.Location on ord.LocationId equals loc.Id
                                     join regord in con.RegistrationOrder on ord.OrderNumber equals regord.OrderNumber
                                     join reg in con.Registration on regord.RegistrationId equals reg.Id
                                     join veh in con.Vehicle on regord.VehicleId equals veh.Id
                                     where ord.Status == (int)OrderStatusTypes.Closed && ordtype.Id == (int)OrderTypes.Admission && 
                                     (ord.ReadyToSend == false || ord.ReadyToSend == null)
                                     select new
                                     {
                                         OrderNumber = ord.OrderNumber,
                                         locationId = loc.Id,
                                         CreateDate = ord.CreateDate,
                                         Status = ordst.Name,
                                         CustomerName = cust.Name,
                                         Kennzeichen = reg.Licencenumber,
                                         VIN = veh.VIN,
                                         TSN = veh.TSN,
                                         HSN = veh.HSN,
                                         CustomerLocation = loc.Name,
                                         CustomerLocationId = loc.Id,
                                         CustomerId = ord.CustomerId,
                                         Kundenname = cust.Name,
                                         Standort = loc.Name,
                                         OrderTyp = ordtype.Name
                                     };
            e.Result = largeCustomerQuery;
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
        /// Prüft die Datenbank für Auftraege, die nicht geschlossen sind.
        /// </summary>
        /// <param name="location">Location des Kundes</param>
        /// <returns>Bool (Gefunden oder nicht)</returns>
        protected bool CheckForOpenValues(string location)
        {
            bool statusFromCheck = false;
            if (!String.IsNullOrEmpty(location))
            {
                DataClasses1DataContext con = new DataClasses1DataContext();
                var values = (from ord in con.Order
                              join loc in con.Location on ord.LocationId equals loc.Id
                              join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                              where loc.Name == location && ord.Status == (int)OrderStatusTypes.AdmissionPoint && ordtype.Id == (int)OrderTypes.Admission
                              select ord.LocationId).ToList();
                if (values.Count > 0)
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
                DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                try
                {
                    List<LocationOrderJoins> locationIdList = new List<LocationOrderJoins>();
                    foreach (GridDataItem item in RadGridLieferscheine.SelectedItems)
                    {
                        var myOrder = dbContext.Order.FirstOrDefault(q => q.OrderNumber == Int32.Parse(item["OrderNumber"].Text));
                        LocationOrderJoins orJ = new LocationOrderJoins();
                        orJ.LocationId = Int32.Parse(item["locationId"].Text);
                        orJ.Order = myOrder;
                        locationIdList.Add(orJ);
                    }
                    groupedOrder = locationIdList.GroupBy(q => q.LocationId);
                    foreach (var gr in groupedOrder)
                    {
                        var locationQuery = dbContext.Location.SingleOrDefault(q => q.Id == gr.First().LocationId);
                        var packingList = PackingList.CreatePackingList(locationQuery.Name, locationQuery.Adress, dbContext);
                        packingList.LogDBContext = dbContext;
                        // für alle, die selected sind - daten zu packing list
                        foreach (var orders in gr)
                        {
                            packingList.AddOrderById(orders.Order.OrderNumber, dbContext);
                            orders.Order.LogDBContext = dbContext;
                            orders.Order.PackingListNumber = packingList.PackingListNumber;
                            orders.Order.ReadyToSend = true;
                        }
                    }
                    dbContext.SubmitChanges();
                    RadGridLieferscheine.DataBind();
                    OffenePanel.Visible = false;
                    NochOffenAuftraegeRadGrid.Enabled = false;
                    AllesOkLieferscheine.Visible = true;
                }
                catch (SqlException ex)
                {
                    ErrorLabelLieferschein.Visible = true;
                    ErrorLabelLieferschein.Text = "Fehler: " + ex.Message;
                    dbContext.WriteLogItem(ex.Message, LogTypes.ERROR, "Order");
                }
            }
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
                    DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                    try
                    {
                        var newOrder = dbContext.Order.Single(q => q.CustomerId == customerID && q.OrderNumber == orderNumber);
                        if (newOrder != null)
                        {
                            //updating order status
                            newOrder.LogDBContext = dbContext;
                            newOrder.Status = (int)OrderStatusTypes.Closed;
                            newOrder.ExecutionDate = DateTime.Now;
                            //updating orderitems status                          
                            foreach (OrderItem ordItem in newOrder.OrderItem)
                            {
                                ordItem.LogDBContext = dbContext;
                                if (ordItem.Status != (int)OrderItemStatusTypes.Cancelled)
                                {
                                    ordItem.Status = (int)OrderItemStatusTypes.Closed;
                                }
                            }
                            dbContext.SubmitChanges();
                            AllesIsOkeyBeiOffene.Visible = true;
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorOffeneLabel.Visible = true;
                        ErrorOffeneLabel.Text = "Fehler: " + ex.Message;
                        dbContext.WriteLogItem(ex.Message, LogTypes.ERROR, "Order");
                    }
                    NochOffenAuftraegeRadGrid.Rebind();
                    RadGridLieferscheine.Rebind();
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
            using (DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())))
            {
                var searchOrderQuery = dbContext.Order.SingleOrDefault(q => q.OrderNumber == orderNumber);
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
                var con = new DataClasses1DataContext();
                var largeCustomerQuery = from ord in con.Order
                                         join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                         join cust in con.Customer on ord.CustomerId equals cust.Id
                                         join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                         join loc in con.Location on ord.LocationId equals loc.Id
                                         join regord in con.RegistrationOrder on ord.OrderNumber equals regord.OrderNumber
                                         join reg in con.Registration on regord.RegistrationId equals reg.Id
                                         join veh in con.Vehicle on regord.VehicleId equals veh.Id
                                         where ord.Status == (int)OrderStatusTypes.AdmissionPoint && ordtype.Id == (int)OrderTypes.Admission && loc.Id == locationId
                                         select new
                                         {
                                             OrderNumber = ord.OrderNumber,
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
                                             HasError = ord.HasError,
                                             ErrorReason = ord.ErrorReason
                                         };
                e.Result = largeCustomerQuery;
            }
            else if (!String.IsNullOrEmpty(LocationIdHiddenField.Value))
            {
                DataClasses1DataContext con = new DataClasses1DataContext();
                var largeCustomerQuery = from ord in con.Order
                                         join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                         join cust in con.Customer on ord.CustomerId equals cust.Id
                                         join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                         join loc in con.Location on ord.LocationId equals loc.Id
                                         join regord in con.RegistrationOrder on ord.OrderNumber equals regord.OrderNumber
                                         join reg in con.Registration on regord.RegistrationId equals reg.Id
                                         join veh in con.Vehicle on regord.VehicleId equals veh.Id
                                         where ord.Status == (int)OrderStatusTypes.AdmissionPoint && ordtype.Id == (int)OrderTypes.Admission && loc.Name == LocationIdHiddenField.Value
                                         select new
                                         {
                                             OrderNumber = ord.OrderNumber,
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
                                             HasError = ord.HasError,
                                             ErrorReason = ord.ErrorReason
                                         };
                e.Result = largeCustomerQuery;
            }
            else
            {
                OffenePanel.Visible = false;
                NochOffenAuftraegeRadGrid.Enabled = false;
                AllesIsOkeyBeiOffene.Visible = false;
            }
        }
        /// <summary>
        /// Hilfsproperty fuer die Gruppierten Lieferscheine
        /// </summary>
        public IEnumerable<IGrouping<int, LocationOrderJoins>> groupedOrder { get; set; }
    }
}