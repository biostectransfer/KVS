using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse 
    /// </summary>
    public partial class CostCenter : ILogging
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
        private KVSEntities myDbContext;
        public KVSEntities _dbContext
        {
            get { return myDbContext; }
            set { myDbContext = value; }
        }
        /// <summary>
        /// Löscht die angegebene Kostenstelle aus der Datenbank.
        /// </summary>
        /// <param name="adressId">Id der Kostenstelle.</param>
        /// <param name="dbContext"></param>
        public static void RemoveCostCenter(int costCenterId, KVSEntities dbContext)
        {
            CostCenter cc = dbContext.CostCenter.SingleOrDefault(q => q.Id == costCenterId);
            if (cc != null)
            {
                if (cc.OrderItem != null && cc.OrderItem.Count > 0)
                    throw new Exception("Die Kostenstelle kann nicht gelöscht werden, diese ist mit Auftragspositionen verknüft");

                if (cc.InvoiceItem != null && cc.InvoiceItem.Count > 0)
                    throw new Exception("Die Kostenstelle kann nicht gelöscht werden, diese ist mit Rechnungspostionen verknüft");

                if(cc.BankAccountId.HasValue)
                BankAccount.DeleteBank(cc.BankAccountId.Value, dbContext);

                dbContext.WriteLogItem("CostCenter mit der Id:"+cc.Id+ " und dem Namen: " +cc.Name +  " gelöscht.", LogTypes.DELETE, costCenterId, "CostCenter");
                dbContext.CostCenter.DeleteOnSubmit(cc);
            }
        }
        /// <summary>
        /// Validierungs Methode um falsche Eingaben zu verhindern
        /// </summary>
        /// <param name="action"></param>
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            if (action == System.Data.Linq.ChangeAction.Insert)
            {
                if (this.myDbContext == null)
                {
                    using (KVSEntities dbContext = new KVSEntities())
                    {
                        if (dbContext.CostCenter.Any(q => q.CustomerId == this.CustomerId && q.Name == this.Name))
                        {
                            var customerName = dbContext.Customer.Single(q => q.Id == this.CustomerId).Name;
                            throw new Exception("Der Kunde " + customerName + " besitzt bereits eine Kostenstelle mit Namen " + this.Name + ".");
                        }
                    }
                }
                else
                {
                    if (myDbContext.CostCenter.Any(q => q.CustomerId == this.CustomerId && q.Name == this.Name))
                    {
                        var customerName = myDbContext.Customer.Single(q => q.Id == this.CustomerId).Name;
                        throw new Exception("Der Kunde " + customerName + " besitzt bereits eine Kostenstelle mit Namen " + this.Name + ".");
                    }
                }
            }
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
        partial void OnNameChanging(string value)
        {
            this.WriteUpdateLogItem("Name", this.Name, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnBankAccountIdChanging(int? value)
        {
            if (this.EntityState == Database.EntityState.Loaded)
            {
                if (value.HasValue)
                {
                    this.LogDBContext.WriteLogItem("Der Kostenstelle " + this.Name + " wurde eine neue Bankverbindung zugewiesen.", LogTypes.UPDATE, this.Id, "CostCenter", value.Value);
                }
                else
                {
                    this.LogDBContext.WriteLogItem("Bankverbindung der Kostenstelle " + this.Name + " wurde entfernt.", LogTypes.DELETE, this.Id, "CostCenter", this.BankAccountId.Value);
                }
            }
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCostcenterNumberChanging(string value)
        {
            this.WriteUpdateLogItem("Kostenstellennummer", this.CostcenterNumber, value);
        }
    }
}
