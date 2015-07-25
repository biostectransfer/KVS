using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse fuer die DB Tabelle Vehicle
    /// </summary>
    public partial class Vehicle : ILogging
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
        /// Erstellt ein neues Fahrzeug.
        /// </summary>
        /// <param name="vin">Fahrgestellnummer des Fahrzeugs.</param>
        /// <param name="hsn">HSN des Fahrzeugs..</param>
        /// <param name="tsn">TSN des Fahrzeugs..</param>
        /// <param name="variant">Variante / Edition des Fahrzeugs.</param>
        /// <param name="firstRegistrationDate">Erstzulassungsdatum.</param>
        /// <param name="colorCode">Farbcode.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Das neue Fahrzeug.</returns>
        public static Vehicle CreateVehicle(string vin, string hsn, string tsn, string variant, DateTime? firstRegistrationDate, int? colorCode, KVSEntities dbContext)
        {
            if (string.IsNullOrEmpty(vin))
            {
                ////throw new Exception("Fehler beim Anlegen des Fahrzeugs: Die FIN darf nicht leer sein.");
            }
            else if (vin.Length != 17 && vin.Length != 8)
            {
                throw new Exception("Fehler beim Anlegen des Fahrzeugs: Die FIN muss 8- oder 17-stellig sein.");
            }

            if (string.IsNullOrEmpty(hsn))
            {
                ////throw new Exception("Fehler beim Anlegen des Fahrzeugs: Der Herstellerschlüssel (HSN) darf nicht leer sein.");
            }

            if (string.IsNullOrEmpty(tsn))
            {
                ////throw new Exception("Fehler beim Anlegen des Fahrzeugs: Der Typschlüssel (TSN) darf nicht leer sein.");
            }

            Vehicle vehicle = new Vehicle()
            {
                VIN = vin,
                HSN = hsn,
                TSN = tsn,
                Variant = variant,
                ColorCode = colorCode,
                FirstRegistrationDate = firstRegistrationDate
            };
                        
            dbContext.Vehicle.InsertOnSubmit(vehicle);
            dbContext.SubmitChanges();
            dbContext.WriteLogItem("Fahrzeug " + vin + " angelegt.", LogTypes.INSERT, vehicle.Id, "Vehicle");

            return vehicle;
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
