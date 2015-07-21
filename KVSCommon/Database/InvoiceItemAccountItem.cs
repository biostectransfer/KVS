using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse für die Rechnugspositionen/Erloeskonten
    /// </summary>
    partial class InvoiceItemAccountItem : ILogging
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
                return this.IIACCID;
            }
        }

        public EntityState EntityState
        {
            get;
            set;
        }

        /// <summary>
        /// Ändert das Erloeskonto für alle Amtlichen Gebühren
        /// </summary>
        /// <param name="dbContext">DB Kontext</param>
        /// <param name="inv">Rechnungsobjekt</param>
        /// <param name="newAccountNumber">neues Erloeskonto</param>
        public static void UpdateAuthorativeAccounts(DataClasses1DataContext dbContext, Invoice inv, string newAccountNumber)
        {
            if (newAccountNumber == string.Empty)
                throw new Exception("Es wurde kein Standard Erlös-Konto in der Konfiguration gefunden");

            var accountsToChange = dbContext.AuthorativeChargeAccounts.Where(q => q.InvoiceId == inv.Id);
            if (accountsToChange.Count() > 0)
            {
                foreach (var atc in accountsToChange)
                {
                    var invItemAccountItem = dbContext.InvoiceItemAccountItem.SingleOrDefault(q => q.IIACCID == atc.InvoiceItemAccountItemId);
                    invItemAccountItem.RevenueAccountText = newAccountNumber;
                    dbContext.WriteLogItem("Rechnungsposition mit der ID: " + invItemAccountItem.IIACCID + " und der Rechnungsid: " + inv.Id+ " wurde auf das Standard Konto " + newAccountNumber + " geändert.", 
                        LogTypes.UPDATE, invItemAccountItem.IIACCID, "InvoiceItemAccountItem", invItemAccountItem.IIACCID);

                    dbContext.SubmitChanges();
                }

            }
          
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnIIACCIDChanging(int value)
        {
            this.WriteUpdateLogItem("IIACCID", this.IIACCID, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnRevenueAccountTextChanging(string value)
        {
            this.WriteUpdateLogItem("RevenueAccountText", this.RevenueAccountText, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnInvoiceItemIdChanging(int value)
        {
            this.WriteUpdateLogItem("InvoiceItemId", this.InvoiceItemId, value);
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


    }
}
