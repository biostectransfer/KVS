using KVSCommon.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse fuer die DB Tabelle Vehicle
    /// </summary>
    public partial class Vehicle : ILogging, IHasId<int>, IRemovable, ISystemFields
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
        partial void OnCreated()
        {
            this.EntityState = Database.EntityState.New;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        partial void OnLoaded()
        {
            this.EntityState = Database.EntityState.Loaded;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        partial void OnCurrentRegistrationIdChanging(int? value)
        {
            this.WriteUpdateLogItem("Zulassung", this.CurrentRegistrationId, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        partial void OnTSNChanging(string value)
        {
            this.WriteUpdateLogItem("TSN", this.TSN, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        partial void OnVariantChanging(string value)
        {
            this.WriteUpdateLogItem("Variante", this.Variant, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        partial void OnVINChanging(string value)
        {
            this.WriteUpdateLogItem("Fahrgestellnummer", this.VIN, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        partial void OnHSNChanging(string value)
        {
            this.WriteUpdateLogItem("HSN", this.HSN, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        partial void OnFirstRegistrationDateChanging(DateTime? value)
        {
            this.WriteUpdateLogItem("Erstzulassungsdatum", this.FirstRegistrationDate, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        partial void OnColorCodeChanging(int? value)
        {
            this.WriteUpdateLogItem("Farbcode", this.ColorCode, value);
        }
    }
}
