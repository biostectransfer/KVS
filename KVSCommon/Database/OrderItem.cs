using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse für die OrderItem Tabelle
    /// </summary>
    public partial class OrderItem : ILogging
    {
        public DataClasses1DataContext LogDBContext
        {
            get;
            set;
        }

        public object ItemId
        {
            get
            {
                return this.Id;
            }
        }

        public EntityState EntityState
        {
            get;
            set;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCreated()
        {
            this.EntityState = Database.EntityState.New;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnLoaded()
        {
            this.EntityState = Database.EntityState.Loaded;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnNeedsVATChanging(bool value)
        {
            if (this.Status == (int)OrderItemState.Abgerechnet)
            {
                throw new Exception("Die Mehrwersteuerpflicht kann nicht geändert werden, da die Auftragsposition bereits abgerechnet ist.");
            }

            this.WriteUpdateLogItem("Mehrwertsteuer", this.NeedsVAT, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCostCenterIdChanging(int? value)
        {
            this.WriteUpdateLogItem("Kostenstelle", this.CostCenterId, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnProductNameChanging(string value)
        {
            this.WriteUpdateLogItem("Produktname", this.ProductName, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnStatusChanging(int value)
        {
            if (value == (int)OrderItemState.Storniert)
            {
                if (this.Status == (int)OrderItemState.Abgerechnet)
                {
                    throw new Exception("Die Auftragsposition kann nicht storniert werden, da sie bereits abgerechnet ist.");
                }
            }
            else if (value == (int)OrderItemState.Abgerechnet)
            {
                if (this.Status == (int)OrderItemState.Storniert)
                {
                    throw new Exception("Die Auftragsposition kann nicht abgerechnet werden, da sie storniert ist.");
                }

                if (this.Status != (int)OrderItemState.Abgeschlossen)
                {
                    throw new Exception("Die Auftragsposition kann nicht abgerechnet werden, da sie nicht abgeschlossen ist.");
                }
            }

            this.WriteUpdateLogItem("Status", this.Status, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnProductIdChanging(int value)
        {
            this.WriteUpdateLogItem("Produkt", this.ProductId, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnAmountChanging(decimal value)
        {
            if (this.Status == (int)OrderItemState.Abgerechnet)
            {
                throw new Exception("Der Betrag kann nicht geändert werden, da die Auftragsposition bereits abgerechnet ist.");
            }

            this.WriteUpdateLogItem("Betrag", this.Amount, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnIsAuthorativeChargeChanging(bool value)
        {
            if (this.Status == (int)OrderItemState.Abgerechnet)
            {
                throw new Exception("Die amtliche Gebühr kann nicht geändert werden, da die Auftragsposition bereits abgerechnet ist.");
            }

            this.WriteUpdateLogItem("IstAmtlicheGebühr", this.IsAuthorativeCharge, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCountChanging(int value)
        {
            if (this.Status == (int)OrderItemState.Abgerechnet)
            {
                throw new Exception("Die Anzahl kann nicht geändert werden, da die Auftragsposition bereits abgerechnet ist.");
            }

            this.WriteUpdateLogItem("Anzahl", this.Count, value);
        }
        /// <summary>
        /// Loescht eine Auftragsposition und ggf. die Amtlichen Gebuehren dazu
        /// </summary>
        /// <param name="dbContext">DB Kontext</param>
        /// <param name="orderItemId">AuftragspositionID</param>
        public static void RemoveOrderItem(DataClasses1DataContext dbContext, int orderItemId)
        {
            var orderItemToDelete = dbContext.OrderItem.FirstOrDefault(q => q.Id == orderItemId);
            if (orderItemToDelete != null)
            {

                if (orderItemToDelete.Status > 100)
                    throw new Exception("Der Auftragsstatus ist nicht mehr Offen, löschen nicht möglich");
                if (orderItemToDelete.Order.DocketList != null)
                    throw new Exception("Laufzettel wurde bereits erstellt, löschen nicht möglich");
                if (orderItemToDelete.Order.PackingList != null)
                    throw new Exception("Lieferschein wurde bereits erstellt, löschen nicht möglich");

                var itemsAnzahl = dbContext.OrderItem.Count(q => q.Id != orderItemId && q.SuperOrderItemId != orderItemId && q.OrderId==orderItemToDelete.OrderId);
                    if(itemsAnzahl==0)
                        throw new Exception("Mind. eine Position muss pro Auftrag verfügbar sein");

                 var hasChildItems = dbContext.OrderItem.FirstOrDefault(q => q.SuperOrderItemId == orderItemToDelete.Id);
                 dbContext.OrderItem.DeleteOnSubmit(hasChildItems);



                if (orderItemToDelete.SuperOrderItemId.HasValue == true)
                {
                    RemoveOrderItem(dbContext, orderItemToDelete.SuperOrderItemId.Value);
                }

                dbContext.OrderItem.DeleteOnSubmit(orderItemToDelete);
                dbContext.WriteLogItem("Auftragsposition " + orderItemToDelete.ProductName + " mit der Auftragsnummer " + orderItemToDelete.Order.Ordernumber+ " wurde gelöscht.", LogTypes.DELETE, orderItemToDelete.Id, "OrderItem");

            }
        }
    }
}
