using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IVehicleManager : IEntityManager<Vehicle, int>
    {
        /// <summary>
        /// Erstellt ein neues Fahrzeug.
        /// </summary>
        /// <param name="vin">Fahrgestellnummer des Fahrzeugs.</param>
        /// <param name="hsn">HSN des Fahrzeugs..</param>
        /// <param name="tsn">TSN des Fahrzeugs..</param>
        /// <param name="variant">Variante / Edition des Fahrzeugs.</param>
        /// <param name="firstRegistrationDate">Erstzulassungsdatum.</param>
        /// <param name="colorCode">Farbcode.</param>
        /// <returns>Das neue Fahrzeug.</returns>
        Vehicle CreateVehicle(string vin, string hsn, string tsn, string variant, DateTime? firstRegistrationDate, int? colorCode);
    }
}
