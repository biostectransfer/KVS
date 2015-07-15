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
        public static Registration CreateRegistration(Guid carOwnerId, Guid vehicleId, string licencenumber, string evbNumber, DateTime? generalInspectionDate, DateTime? registrationDate, string registrationDocumentNumber, string emissionCode, DataClasses1DataContext dbContext)
        {
            Registration registration = new Registration()
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicleId,
                CarOwnerId = carOwnerId,
                Licencenumber = licencenumber,
                GeneralInspectionDate = generalInspectionDate,
                RegistrationDate = registrationDate,
                RegistrationDocumentNumber = registrationDocumentNumber,
                eVBNumber = evbNumber,
                EmissionCode = emissionCode
            };

            //   var vehicleVIN = dbContext.Vehicle.Single(q => q.Id == vehicleId).VIN;
            dbContext.WriteLogItem("Zulassung wurde angelegt.", LogTypes.INSERT, registration.Id, "Registration", vehicleId);
            dbContext.Registration.InsertOnSubmit(registration);
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
