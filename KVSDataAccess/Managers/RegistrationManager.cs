using KVSCommon.Database;
using KVSCommon.Entities;
using KVSCommon.Managers;
using KVSDataAccess.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSDataAccess.Managers
{
    public partial class RegistrationManager : EntityManager<Registration, int>, IRegistrationManager
    {
        public RegistrationManager(IKVSEntities context) : base(context) { }

        /// <summary>
        /// Erstellt eine neue Zulassung.
        /// </summary>
        /// <param name="carOwnerId">Id des Halters.</param>
        /// <param name="vehicleId">Id des Fahrzeugs.</param>
        /// <param name="licenceNumber">Kennzeichen für die Zulassung.</param>
        /// <param name="evbNumber">eVB-Nummer der Fahrzeugversicherung.</param>
        /// <param name="generalInspectionDate">Datum der Hauptuntersuchung (HU).</param>
        /// <param name="registrationDate">Zulassungsdatum.</param>
        /// <param name="registrationDocumentNumber">Fahrzeugbriefnummer, falls vorhanden.</param>
        /// <returns>Die neue Zulassung.</returns>
        public Registration CreateRegistration(CarOwner carOwner, Vehicle vehicle, string licenceNumber, string evbNumber, DateTime? generalInspectionDate,
            DateTime? registrationDate, string registrationDocumentNumber, string emissionCode)
        {
            var registration = new Registration()
            {
                Vehicle = vehicle,
                CarOwner = carOwner,
                Licencenumber = licenceNumber,
                GeneralInspectionDate = generalInspectionDate,
                RegistrationDate = registrationDate,
                RegistrationDocumentNumber = registrationDocumentNumber,
                eVBNumber = evbNumber,
                EmissionCode = emissionCode
            };

            DataContext.AddObject(registration);
            SaveChanges();
            DataContext.WriteLogItem("Zulassung wurde angelegt.", LogTypes.INSERT, registration.Id, "Registration", vehicle.Id);

            return registration;
        }
    }
}
