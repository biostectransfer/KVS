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
    public partial class InvoiceItemManager : EntityManager<InvoiceItem, int>, IInvoiceItemManager
    {
        public InvoiceItemManager(IKVSEntities context) : base(context) { }

        /// <summary>
        /// Fügt der Rechnung eine neue Rechnungsposition hinzu.
        /// </summary>
        /// <param name="invoice">Invoice</param>
        /// <param name="name">Bezeichnung für die Rechnungsposition.</param>
        /// <param name="amount">Betrag der Rechnungsposition.</param>
        /// <param name="count">Anzahl für die Position.</param>
        /// <param name="orderItemId">Id der Auftragsposition, falls vorhande.</param>
        /// <param name="costCenterId">Id der Kostenstelle, falls benötigt.</param>
        /// <returns>Die neue Rechnungsposition.</returns>
        public InvoiceItem AddInvoiceItem(Invoice invoice, string name, decimal amount, int count, OrderItem orderItem, CostCenter costCenter)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Die Bezeichnung der Rechnungsposition darf nicht leer sein.");
            }

            if (invoice.IsPrinted)
            {
                throw new Exception("Die Rechnungsposition kann nicht hinzugefügt werden: Die Rechnung ist bereits gedruckt.");
            }

            var customer = DataContext.GetSet<Customer>().Single(q => q.Id == invoice.CustomerId);

            var item = new InvoiceItem()
            {
                Amount = amount,
                Count = count,
                Name = name,
                OrderItem = orderItem,
                CostCenter = costCenter
            };

            invoice.InvoiceItem.Add(item);
            DataContext.AddObject(item);
            SaveChanges();
            DataContext.WriteLogItem("Rechnungsposition " + name + " zur Rechnung hinzugefügt.", LogTypes.INSERT, invoice.Id, "InvoiceItem", item.Id);

            if (orderItem != null)
            {
                if (orderItem.Status == (int)OrderItemStatusTypes.Payed)
                {
                    throw new Exception("Die Auftragsposition ist bereits abgerechnet.");
                }

                if (orderItem.Status != (int)OrderItemStatusTypes.Closed)
                {
                    throw new Exception("Die Auftragsposition ist nicht abgeschlossen.");
                }

                if (orderItem.Order.LocationId.HasValue && invoice.OrderInvoice.Any(q => q.Order.LocationId != orderItem.Order.LocationId))
                {
                    throw new Exception("Die Auftragsposition kann nicht zur Rechnung hinzugefügt werden, da der Standort des Auftrags nicht mit dem Standort der bisherigen Aufträge in der Rechnung übereinstimmt.");
                }

                if (orderItem.NeedsVAT)
                {
                    if (orderItem.Order.Location != null && orderItem.Order.Location.VAT.HasValue) //Großkunde
                    {
                        item.VAT = orderItem.Order.Location.VAT.Value;
                    }
                    else //SofortKunde
                    {
                        item.VAT = customer.VAT;
                    }
                }

                orderItem.Status = (int)OrderItemStatusTypes.Payed;
                var order = orderItem.Order;
                if (!DataContext.GetSet<OrderInvoice>().Any(q => q.OrderNumber == order.Id && q.InvoiceId == invoice.Id))
                {
                    var orderInvoice = new OrderInvoice()
                    {
                        Order = order,
                        Invoice = invoice
                    };

                    DataContext.AddObject(orderInvoice);
                    DataContext.WriteLogItem("Rechnung wurde mit Auftrag " + order.Id + " verknüpft.", LogTypes.INSERT, order.Id, "OrderInvoice", invoice.Id);
                }
            }
            else
            {
                item.VAT = customer.VAT;
            }

            return item;
        }
    }
}
