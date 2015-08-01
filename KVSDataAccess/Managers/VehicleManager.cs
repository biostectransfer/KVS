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
    public partial class VehicleManager : EntityManager<Vehicle, int>, IVehicleManager
    {
        public VehicleManager(IKVSEntities context) : base(context) { }

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
        public Vehicle CreateVehicle(string vin, string hsn, string tsn, string variant, DateTime? firstRegistrationDate, int? colorCode)
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

            var vehicle = new Vehicle()
            {
                VIN = vin,
                HSN = hsn,
                TSN = tsn,
                Variant = variant,
                ColorCode = colorCode,
                FirstRegistrationDate = firstRegistrationDate
            };

            DataContext.AddObject(vehicle);
            SaveChanges();
            DataContext.WriteLogItem("Fahrzeug " + vin + " angelegt.", LogTypes.INSERT, vehicle.Id, "Vehicle");

            return vehicle;
        }
    }
}
