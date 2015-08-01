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

        /// <summary>
        ///  Update order item amount
        /// </summary>
        /// <param name="orderItemId"></param>
        /// <param name="amount"></param>
        public void UpdateOrderItemAmount(int orderItemId, decimal amount)
        {
            var orderItem = DataContext.GetSet<OrderItem>().FirstOrDefault(o => o.Id == orderItemId);
            orderItem.Amount = amount;
            SaveChanges();
        }

        /// <summary>
        /// Erstellt die amtlichen Gebuehren
        /// </summary>
        /// <param name="authId">Amtliche Gebuehr ID</param>
        /// <param name="itemId">Auftragspositionen Id</param>
        /// <param name="amount">Betrag</param>
        /// <returns>bool</returns>
        public bool GenerateAuthCharge(int? authId, int itemId, string amount)
        {
            if (amount == string.Empty)
                amount = "0";

            if (amount != "kein Preis")
            {
                if (!EmptyStringIfNull.IsNumber(amount))
                    throw new Exception("Achtung, Sie haben keinen gültigen Preis eingegeben");

                string amoutToSave = amount;
                if (amoutToSave.Contains("."))
                    amoutToSave = amoutToSave.Replace(".", ",");


                if (!authId.HasValue)
                {
                    var orderItem = DataContext.GetSet<OrderItem>().FirstOrDefault(o => o.Id == itemId);

                    AddOrderItem(orderItem.Order, orderItem.ProductId, Convert.ToDecimal(amoutToSave), orderItem.Count, orderItem.CostCenter, orderItem.Id, true);
                }
                else
                {
                    var orderItem = DataContext.GetSet<OrderItem>().FirstOrDefault(q => q.Id == authId);
                    orderItem.Amount = Convert.ToDecimal(amoutToSave);
                    SaveChanges();
                }

                return true;
            }
            else
            {

                return false;
            }
        }

        /// <summary>
        /// Get order items
        /// </summary>
        /// <param name="orderNumber">Order number</param>
        /// <returns></returns>
        public IQueryable<OrderItem> GetOrderItems(int orderNumber)
        {
            return DataContext.GetSet<OrderItem>().Where(o => o.OrderNumber == orderNumber);
        }

        /// <summary>
        /// Get order items
        /// </summary>
        /// <returns></returns>
        public IQueryable<OrderItem> GetOrderItems()
        {
            return DataContext.GetSet<OrderItem>();
        }

        /// <summary>
        /// Loescht eine Auftragsposition und ggf. die Amtlichen Gebuehren dazu
        /// </summary>
        /// <param name="orderItemId">AuftragspositionID</param>
        public void RemoveOrderItem(int orderItemId)
        {
            var orderItemToDelete = DataContext.GetSet<OrderItem>().FirstOrDefault(q => q.Id == orderItemId);
            if (orderItemToDelete != null)
            {
                if (orderItemToDelete.Status > (int)OrderItemStatusTypes.Open)
                    throw new Exception("Der Auftragsstatus ist nicht mehr Offen, löschen nicht möglich");

                if (orderItemToDelete.Order.DocketList != null)
                    throw new Exception("Laufzettel wurde bereits erstellt, löschen nicht möglich");

                if (orderItemToDelete.Order.PackingList != null)
                    throw new Exception("Lieferschein wurde bereits erstellt, löschen nicht möglich");

                var itemsAnzahl = DataContext.GetSet<OrderItem>().Count(q => q.Id != orderItemId && q.SuperOrderItemId != orderItemId && q.OrderNumber == orderItemToDelete.OrderNumber);
                if (itemsAnzahl == 0)
                    throw new Exception("Mind. eine Position muss pro Auftrag verfügbar sein");

                var hasChildItems = DataContext.GetSet<OrderItem>().FirstOrDefault(q => q.SuperOrderItemId == orderItemToDelete.Id);
                DataContext.DeleteObject(hasChildItems);
                                
                if (orderItemToDelete.SuperOrderItemId.HasValue == true)
                {
                    RemoveOrderItem(orderItemToDelete.SuperOrderItemId.Value);
                }

                DataContext.DeleteObject(orderItemToDelete);
                DataContext.WriteLogItem("Auftragsposition " + orderItemToDelete.ProductName + " mit der Auftragsnummer " + orderItemToDelete.Order.OrderNumber + " wurde gelöscht.",
                    LogTypes.DELETE, orderItemToDelete.Id, "OrderItem");
            }
        }
    }
}
