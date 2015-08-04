using KVSCommon.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse fuer die DB Tabelle Registration
    /// </summary>
    public partial class Registration : ILogging, IHasId<int>, IRemovable, ISystemFields
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
        partial void OnLicencenumberChanging(string value)
        {
            this.WriteUpdateLogItem("Kennzeichen", this.Licencenumber, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnGeneralInspectionDateChanging(DateTime? value)
        {
            this.WriteUpdateLogItem("HU-Datum", this.GeneralInspectionDate, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnRegistrationDateChanging(DateTime? value)
        {
            this.WriteUpdateLogItem("Zulassungsdatum", this.RegistrationDate, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnRegistrationDocumentNumberChanging(string value)
        {
            this.WriteUpdateLogItem("Fahrzeugbriefnummer", this.RegistrationDocumentNumber, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OneVBNumberChanging(string value)
        {
            this.WriteUpdateLogItem("eVB-Nummer", this.eVBNumber, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnEmissionCodeChanging(string value)
        {
            this.WriteUpdateLogItem("Emissionscode", this.EmissionCode, value);
        }
    }
}
