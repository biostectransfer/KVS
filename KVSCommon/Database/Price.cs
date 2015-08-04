using KVSCommon.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse fuer die DB Tabelle Price
    /// </summary>
    public partial class Price : ILogging, IHasId<int>, IRemovable, ISystemFields
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
    

        partial void OnCreated()
        {
            this.EntityState = Database.EntityState.New;
        }

        partial void OnLoaded()
        {
            this.EntityState = Database.EntityState.Loaded;
        }

        partial void OnAuthorativeChargeChanging(decimal? value)
        {
            this.WriteUpdateLogItem("Behördliche Gebühr", this.AuthorativeCharge, value);
        }

        partial void OnAmountChanging(decimal value)
        {
            this.WriteUpdateLogItem("Betrag", this.Amount, value);
        }
     
        partial void OnLocationIdChanging(int? value)
        {
            if (this.EntityState != Database.EntityState.New)
            {
                throw new Exception("Der Standort eines Preises kann nicht geändert werden.");
            }
        }

        partial void OnProductIdChanging(int value)
        {
            if (this.EntityState != Database.EntityState.New)
            {
                throw new Exception("Das Produkt eines Preises kann nicht geändert werden.");
            }
        }
    }
}
