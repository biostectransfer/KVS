using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse fuer die DB Tabelle RegistrationOrder
    /// </summary>
    public partial class RegistrationOrder : ILogging
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
                return this.OrderNumber;
            }
        }

        public EntityState EntityState
        {
            get;
            set;
        }

        /// <summary>
        /// Erstellt einen Zulassungsauftrag.
        /// </summary>
        /// <param name="userId">Id des Benutzers.</param>
        /// <param name="customerId">Id des Kunden.</param>
        /// <param name="licencenumber">Kennzeichen für die Zulassung.</param>
        /// <param name="previousLicencenumber">Vorheriges Kennzeichen (nur bei Umkennzeichnung).</param>
        /// <param name="evbNumber">eVB-Nummer der Fahrzeugversicherung.</param>
        /// <param name="vehicleId">Id des Fahrzeugs.</param>
        /// <param name="registrationId">Id der Zulassung.</param>
        /// <param name="registrationOrderTypeId">Id der Zulassungsart.</param>
        /// <param name="locationId">Id des Standorts (Pflicht bei Grosskunden, sonst null).</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Den neuen Zulassungsauftrag.</returns>
        /// <remarks>Erstellt auch gleichzeitig den Order-Datensatz.</remarks>
        public static RegistrationOrder CreateRegistrationOrder(int userId, int customerId, string licencenumber, string previousLicencenumber, string evbNumber, 
            Vehicle vehicle, Registration registration, int registrationOrderTypeId, int? locationId, int zulassungsstelleId, DataClasses1DataContext dbContext)
        {
            var orderTypeId = dbContext.OrderType.Single(q => q.Name == "Zulassung").Id;
            Order order = Order.CreateOrder(userId, customerId, orderTypeId, zulassungsstelleId, dbContext);
            order.LocationId = locationId;

            RegistrationOrder registrationOrder = new RegistrationOrder()
            {
                Order = order,
                Licencenumber = licencenumber,
                Vehicle = vehicle,
                Registration = registration,
                PreviousLicencenumber = previousLicencenumber,
                RegistrationOrderTypeId = registrationOrderTypeId,
                eVBNumber = evbNumber
            };

            //var vehicleVIN = dbContext.Vehicle.Where(q => q.Id == vehicleId).Select(q => q.VIN).Single();
            dbContext.RegistrationOrder.InsertOnSubmit(registrationOrder);
            dbContext.WriteLogItem("Zulassungsauftrag wurde angelegt.", LogTypes.INSERT, registrationOrder.OrderNumber, "RegistrationOrder", vehicle.Id);
            return registrationOrder;
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
    }
}
