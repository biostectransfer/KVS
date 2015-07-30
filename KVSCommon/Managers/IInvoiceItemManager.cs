using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IInvoiceItemManager : IEntityManager<InvoiceItem, int>
    {
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
        InvoiceItem AddInvoiceItem(Invoice invoice, string name, decimal amount, int count, OrderItem orderItem, CostCenter costCenter);
    }
}
