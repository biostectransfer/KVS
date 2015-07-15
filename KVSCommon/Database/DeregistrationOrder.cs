using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse für die DeregistrationOrder Tabelle
    /// </summary>
    public partial class DeregistrationOrder : ILogging
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
                return this.OrderId;
            }
        }

        public EntityState EntityState
        {
            get;
            set;
        }

        /// <summary>
        /// Erstellt einen Abmeldeauftrag.
        /// </summary>
        /// <param name="userId">Id des Benutzers, der den Auftrag anlegt.</param>
        /// <param name="customerId">Id des Kunden.</param>
        /// <param name="vehicleId">Id des Fahrzeugs.</param>
        /// <param name="registrationId">Id der Zulassung.</param>
        /// <param name="locationId">Id des Kundenstandorts.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Den neuen Abmeldeauftrag.</returns>
        /// <remarks>Erstellt auch gleichzeitig den Order-Datensatz.</remarks>
        public static DeregistrationOrder CreateDeregistrationOrder(Guid userId, Guid customerId, Guid vehicleId, Guid registrationId, Guid? locationId, Guid zulassungsstelleId, DataClasses1DataContext dbContext)
        {
            Guid orderTypeId = dbContext.OrderType.Single(q => q.Name == "Abmeldung").Id;
            Order order = Order.CreateOrder(userId, customerId, orderTypeId, zulassungsstelleId, dbContext); //Guid.Empty ersetzen mit ZulassungstelleID
            order.LocationId = locationId;
            DeregistrationOrder deregistrationOrder = new DeregistrationOrder()
            {
                Order = order,
                VehicleId = vehicleId,
                RegistrationId = registrationId
            };

            //  var vehicleVIN = dbContext.Vehicle.Where(q => q.Id == vehicleId).Select(q => q.VIN).Single();
            dbContext.DeregistrationOrder.InsertOnSubmit(deregistrationOrder);
            dbContext.WriteLogItem("Abmeldeauftrag angelegt.", LogTypes.INSERT, deregistrationOrder.OrderId, "DeregistrationOrder", vehicleId);
            return deregistrationOrder;
        }
        /// <summary>
        /// Validierungs Methode um falsche Eingaben zu verhindern
        /// </summary>
        /// <param name="action"></param>
        partial void OnCreated()
        {
            this.EntityState = Database.EntityState.New;
        }
        /// <summary>
        /// Validierungs Methode um falsche Eingaben zu verhindern
        /// </summary>
        /// <param name="action"></param>
        partial void OnLoaded()
        {
            this.EntityState = Database.EntityState.Loaded;
        }
    }
}
