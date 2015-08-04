using KVSCommon.Database;
using KVSCommon.Entities;
using KVSCommon.Enums;
using KVSCommon.Managers;
using KVSDataAccess.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSDataAccess.Managers
{
    public partial class DeregistrationOrderManager : EntityManager<DeregistrationOrder, int>, IDeregistrationOrderManager
    {
        public DeregistrationOrderManager(IKVSEntities context) : base(context) { }

        /// <summary>
        /// Erstellt einen Abmeldeauftrag.
        /// </summary>
        /// <param name="customerId">Id des Kunden.</param>
        /// <param name="vehicleId">Id des Fahrzeugs.</param>
        /// <param name="registrationId">Id der Zulassung.</param>
        /// <param name="locationId">Id des Kundenstandorts.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Den neuen Abmeldeauftrag.</returns>
        /// <remarks>Erstellt auch gleichzeitig den Order-Datensatz.</remarks>
        public DeregistrationOrder CreateDeregistrationOrder(int customerId, Vehicle vehicle, Registration registration, int? locationId, int zulassungsstelleId)
        {
            var orderTypeId = (int)OrderTypes.Cancellation;

            var order = new Order()
            {
                CreateDate = DateTime.Now,
                CustomerId = customerId,
                Status = (int)OrderStatusTypes.Open,
                OrderTypeId = orderTypeId,
                UserId = DataContext.LogUserId.Value,
                Zulassungsstelle = zulassungsstelleId,
                LocationId = locationId,
                //FreeText = freeText
            };

            DataContext.AddObject(order);
            SaveChanges();

            var deregistrationOrder = new DeregistrationOrder()
            {
                Order = order,
                Vehicle = vehicle,
                Registration = registration
            };

            DataContext.AddObject(deregistrationOrder);
            SaveChanges();
            DataContext.WriteLogItem("Abmeldeauftrag angelegt.", LogTypes.INSERT, deregistrationOrder.Id, "DeregistrationOrder", vehicle.Id);
            return deregistrationOrder;
        }
    }
}
