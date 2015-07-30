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
    public partial class OrderManager : EntityManager<Order, int>, IOrderManager
    {
        public OrderManager(IKVSEntities context) : base(context) { }

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
        public OrderItem AddOrderItem(Order order, int productId, decimal priceAmount, int count, CostCenter costCenter, int? superOrderItemId, bool isAuthorativeCharge)
        {
            var product = DataContext.GetSet<Product>().Where(q => q.Id == productId).Single();

            var item = new OrderItem()
            {
                Amount = priceAmount,
                CostCenter = costCenter,
                ProductId = productId,
                Status = (int)OrderItemStatusTypes.Open,
                ProductName = product.Name,
                SuperOrderItemId = superOrderItemId,
                Count = count,
                IsAuthorativeCharge = isAuthorativeCharge,
                NeedsVAT = isAuthorativeCharge ? false : product.NeedsVAT
            };

            order.OrderItem.Add(item);
            SaveChanges();
            DataContext.WriteLogItem("Auftragsposition " + product.Name + " für Auftrag " + order.OrderNumber + " angelegt.", LogTypes.INSERT, item.Id, "OrderItem");

            return item;
        }
    }
}
