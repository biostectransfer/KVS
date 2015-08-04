using KVSCommon.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse zur DB CarOwner
    /// </summary>
    public partial class CarOwner : ILogging, IHasId<int>, IRemovable, ISystemFields
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
        /// Gibt den vollständigen Namen zurück (Fahrzeughalter)
        /// </summary>
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.FirstName))
                {
                    return this.FirstName + "  " + this.Name;
                }

                return this.Name;
            }
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
        partial void OnCreated()
        {
            this.EntityState = Database.EntityState.New;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnNameChanging(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                ////throw new Exception("Der Name des Halters darf nicht leer sein.");
            }

            this.WriteUpdateLogItem("Name", this.Name, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnFirstNameChanging(string value)
        {
            this.WriteUpdateLogItem("Vorname", this.FirstName, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnAdressIdChanging(int? value)
        {
            this.WriteUpdateLogItem("Adresse", this.AdressId, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnBankAccountIdChanging(int? value)
        {
            this.WriteUpdateLogItem("Bankverbindung", this.BankAccountId, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnContactIdChanging(int? value)
        {
            this.WriteUpdateLogItem("Kontaktdaten", this.ContactId, value);
        }
    }
}
