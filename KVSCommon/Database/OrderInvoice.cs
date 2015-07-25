using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterugnsklasse für die OrderInvoice Tabelle
    /// </summary>
    public partial class OrderInvoice : ILogging
    {
        public KVSEntities LogDBContext
        {
            get;
            set;
        }

        public object ItemId
        {
            get
            {
                return this.InvoiceId;
            }
        }

        public EntityState EntityState
        {
            get;
            set;
        }

        /// <summary>
        /// Erstellt eine Verknuepfung zwischen einem Auftrag und einer Rechnung.
        /// </summary>
        /// <param name="OrderNumber">Id des Auftrags.</param>
        /// <param name="invoiceId">Id der Rechnung.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Die neue Verknüpfung.</returns>
        public static OrderInvoice CreateOrderInvoice(Order order, Invoice invoice, KVSEntities dbContext)
        {
            OrderInvoice item = new OrderInvoice()
            {
                Order = order,
                Invoice = invoice
            };

            dbContext.OrderInvoice.InsertOnSubmit(item);
            ///  var invoiceNumber = dbContext.Invoice.Where(q => q.Id == invoiceId).Select(q => q.InvoiceNumber).Single();
            var orderNumber = dbContext.Order.Where(q => q.OrderNumber == order.OrderNumber).Select(q => q.OrderNumber).Single();
            dbContext.WriteLogItem("Rechnung wurde mit Auftrag " + orderNumber + " verknüpft.", LogTypes.INSERT, order.OrderNumber, "OrderInvoice", invoice.Id);
            return item;
        }
        /// <summary>
        /// Loescht eine Auftrags/Rechnungsverknuepfung
        /// </summary>
        /// <param name="OrderNumber">AuftragsID</param>
        /// <param name="invoiceId">RechnungsID</param>
        /// <param name="dbContext">DB Kontext</param>
        public static void DeleteOrderInvoice(int orderNumber, int invoiceId, KVSEntities dbContext)
        {
            var item = dbContext.OrderInvoice.SingleOrDefault(q => q.OrderNumber == orderNumber && q.InvoiceId == invoiceId);
            if (item == null)
            {
                var invoiceNumber = dbContext.Invoice.Where(q => q.Id == invoiceId).Select(q => q.InvoiceNumber.Number).SingleOrDefault();
                throw new Exception("Es existiert keine Verknüpfung zwischen dem Auftrag Nr. " + orderNumber + " und der Rechnung Nr. " + invoiceNumber);
            }

            dbContext.OrderInvoice.DeleteOnSubmit(item);
            dbContext.WriteLogItem("Verknüpfung zwischen Auftrag Nr. " + item.Order.OrderNumber + " und Rechnung Nr. " + 
                item.Invoice.InvoiceNumber.Number + " wurde gelöscht.", 
                LogTypes.DELETE, orderNumber, "OrderInvoice", invoiceId);
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
