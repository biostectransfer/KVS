using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IOrderManager : IEntityManager<Order, int>
    {
        /// <summary>
        /// Fügt dem Auftrag eine neue Position hinzu.
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="productId">Id des Produkts.</param>
        /// <param name="priceAmount">Preis für die Position.</param>
        /// <param name="count">Anzahl für die Position.</param>
        /// <param name="costCenterId">Id der Kostenstelle, falls benötigt.</param>
        /// <param name="superOrderItemId">Id der übergeordneten Auftragsposition, falls benoetigt.</param>
        /// <param name="isAuthorativeCharge">Gibt an, ob es sich um eine behoerdliche Gebühr handelt oder nicht.</param>
        /// <returns>Die neue Auftragsposition.</returns>
        OrderItem AddOrderItem(Order order, int productId, decimal priceAmount, int count, CostCenter costCenter, int? superOrderItemId, bool isAuthorativeCharge);
    }
}
