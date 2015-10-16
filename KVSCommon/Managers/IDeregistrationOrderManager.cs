using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IDeregistrationOrderManager : IEntityManager<DeregistrationOrder, int>
    {
        /// <summary>
        /// Erstellt einen Abmeldeauftrag.
        /// </summary>
        /// <param name="customerId">Id des Kunden.</param>
        /// <param name="vehicleId">Id des Fahrzeugs.</param>
        /// <param name="registrationId">Id der Zulassung.</param>
        /// <param name="locationId">Id des Kundenstandorts.</param>
        /// <returns>Den neuen Abmeldeauftrag.</returns>
        /// <remarks>Erstellt auch gleichzeitig den Order-Datensatz.</remarks>
        DeregistrationOrder CreateDeregistrationOrder(int customerId, Vehicle vehicle, Registration registration, int? locationId, int? zulassungsstelleId);
    }
}
