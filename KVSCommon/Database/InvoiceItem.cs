using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KVSCommon.Database;
using System.Data.Linq;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse für die Rechnungspositionen
    /// </summary>
    public partial class InvoiceItem : ILogging
    {
        public KVSEntities LogDBContext
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
        partial void OnVATChanging(decimal value)
        {
            this.WriteUpdateLogItem("Mehrwertsteuersatz", this.VAT, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnAmountChanging(decimal value)
        {
            this.WriteUpdateLogItem("Betrag", this.Amount, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCostcenterIdChanging(int? value)
        {
            this.WriteUpdateLogItem("Kostenstelle", this.CostcenterId, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnNameChanging(string value)
        {
            this.WriteUpdateLogItem("Bezeichnung", this.Name, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnOrderItemIdChanging(int? value)
        {
            this.WriteUpdateLogItem("Auftragsposition", this.OrderItemId, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCountChanging(int value)
        {
            this.WriteUpdateLogItem("Anzahl", this.Count, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnAccountNumberChanging(string value)
        {
            this.WriteUpdateLogItem("AccountNumber", this.AccountNumber, value);
        }
    }
}
