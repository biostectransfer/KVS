using KVSCommon.Enums;
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
                return this.OrderNumber;
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
        public static DeregistrationOrder CreateDeregistrationOrder(int userId, int customerId, Vehicle vehicle, Registration registration, int? locationId, 
            int zulassungsstelleId, DataClasses1DataContext dbContext)
        {
            var orderTypeId = dbContext.OrderType.Single(q => q.Id == (int)OrderTypes.Cancellation).Id;
            Order order = Order.CreateOrder(userId, customerId, orderTypeId, zulassungsstelleId, dbContext); 
            order.LocationId = locationId;
            DeregistrationOrder deregistrationOrder = new DeregistrationOrder()
            {
                Order = order,
                Vehicle = vehicle,
                Registration = registration
            };

            dbContext.DeregistrationOrder.InsertOnSubmit(deregistrationOrder);
            dbContext.SubmitChanges();
            dbContext.WriteLogItem("Abmeldeauftrag angelegt.", LogTypes.INSERT, deregistrationOrder.OrderNumber, "DeregistrationOrder", vehicle.Id);
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
