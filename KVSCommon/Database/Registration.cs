using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse fuer die DB Tabelle Registration
    /// </summary>
    public partial class Registration : ILogging
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
        /// Erstellt eine neue Zulassung.
        /// </summary>
        /// <param name="carOwnerId">Id des Halters.</param>
        /// <param name="vehicleId">Id des Fahrzeugs.</param>
        /// <param name="licencenumber">Kennzeichen für die Zulassung.</param>
        /// <param name="evbNumber">eVB-Nummer der Fahrzeugversicherung.</param>
        /// <param name="generalInspectionDate">Datum der Hauptuntersuchung (HU).</param>
        /// <param name="registrationDate">Zulassungsdatum.</param>
        /// <param name="registrationDocumentNumber">Fahrzeugbriefnummer, falls vorhanden.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Die neue Zulassung.</returns>
        public static Registration CreateRegistration(CarOwner carOwner, Vehicle vehicle, string licencenumber, string evbNumber, DateTime? generalInspectionDate, 
            DateTime? registrationDate, string registrationDocumentNumber, string emissionCode, KVSEntities dbContext)
        {
            Registration registration = new Registration()
            {
                Vehicle = vehicle,
                CarOwner = carOwner,
                Licencenumber = licencenumber,
                GeneralInspectionDate = generalInspectionDate,
                RegistrationDate = registrationDate,
                RegistrationDocumentNumber = registrationDocumentNumber,
                eVBNumber = evbNumber,
                EmissionCode = emissionCode
            };

            dbContext.Registration.InsertOnSubmit(registration);
            dbContext.SubmitChanges();
            dbContext.WriteLogItem("Zulassung wurde angelegt.", LogTypes.INSERT, registration.Id, "Registration", vehicle.Id);

            return registration;
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
