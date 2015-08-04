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
    public partial class RegistrationOrderManager : EntityManager<RegistrationOrder, int>, IRegistrationOrderManager
    {
        public RegistrationOrderManager(IKVSEntities context) : base(context) { }

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
        /// <returns>Den neuen Zulassungsauftrag.</returns>
        /// <remarks>Erstellt auch gleichzeitig den Order-Datensatz.</remarks>
        public RegistrationOrder CreateRegistrationOrder(int customerId, string licencenumber, string previousLicencenumber, string evbNumber,
            Vehicle vehicle, Registration registration, RegistrationOrderTypes registrationOrderType, int? locationId, int zulassungsstelleId, string freeText)
        {
            var orderTypeId = (int)OrderTypes.Admission;

            var order = new Order()
            {
                CreateDate = DateTime.Now,
                CustomerId = customerId,
                Status = (int)OrderStatusTypes.Open,
                OrderTypeId = orderTypeId,
                UserId = DataContext.LogUserId.Value,
                Zulassungsstelle = zulassungsstelleId,
                LocationId = locationId,
                FreeText = freeText
            };

            DataContext.AddObject(order);
            SaveChanges();
            DataContext.WriteLogItem("Auftrag angelegt.", LogTypes.INSERT, order.Id, "Order");

            var registrationOrder = new RegistrationOrder()
            {
                Order = order,
                Licencenumber = licencenumber,
                Vehicle = vehicle,
                Registration = registration,
                PreviousLicencenumber = previousLicencenumber,
                RegistrationOrderTypeId = (int)registrationOrderType,
                eVBNumber = evbNumber
            };

            DataContext.AddObject(registrationOrder);
            SaveChanges();

            DataContext.WriteLogItem("Zulassungsauftrag wurde angelegt.", LogTypes.INSERT, registrationOrder.Id, "RegistrationOrder", vehicle.Id);
            return registrationOrder;
        }
    }
}
