using KVSCommon.Database;
using KVSCommon.Enums;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IRegistrationOrderManager : IEntityManager<RegistrationOrder, int>
    {
        /// <summary>
        /// Erstellt einen Zulassungsauftrag.
        /// </summary>
        /// <param name="customerId">Id des Kunden.</param>
        /// <param name="licencenumber">Kennzeichen für die Zulassung.</param>
        /// <param name="previousLicencenumber">Vorheriges Kennzeichen (nur bei Umkennzeichnung).</param>
        /// <param name="evbNumber">eVB-Nummer der Fahrzeugversicherung.</param>
        /// <param name="vehicleId">Id des Fahrzeugs.</param>
        /// <param name="registrationId">Id der Zulassung.</param>
        /// <param name="registrationOrderType">Id der Zulassungsart.</param>
        /// <param name="locationId">Id des Standorts (Pflicht bei Grosskunden, sonst null).</param>
        /// <param name="zulassungsstelleId"></param>
        /// <param name="freeText"></param>
        /// <returns>Den neuen Zulassungsauftrag.</returns>
        /// <remarks>Erstellt auch gleichzeitig den Order-Datensatz.</remarks>
        RegistrationOrder CreateRegistrationOrder(int customerId, string licencenumber, string previousLicencenumber, string evbNumber,
            Vehicle vehicle, Registration registration, RegistrationOrderTypes registrationOrderType, int? locationId, int zulassungsstelleId, string freeText);
    }
}
