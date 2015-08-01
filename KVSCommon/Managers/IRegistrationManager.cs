using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IRegistrationManager : IEntityManager<Registration, int>
    {
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
        Registration CreateRegistration(CarOwner carOwner, Vehicle vehicle, string licenceNumber, string evbNumber, DateTime? generalInspectionDate,
            DateTime? registrationDate, string registrationDocumentNumber, string emissionCode);
    }
}
