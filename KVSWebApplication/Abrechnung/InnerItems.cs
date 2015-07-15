using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSWebApplication.Abrechnung
{
    /// <summary>
    /// Hilfklasse fuer die Rechnungspositionen
    /// </summary>
    class InnerItems
    {
        public Guid ItemId { get; set; }
        public Guid? OrderItemId { get; set; }
        public Guid InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public bool isPrinted { get; set; }
        public Guid InvoiceItemId { get; set; }
        public Guid AccountId { get; set; }
        public string AccountNumber { get; set; }
        public bool Active { get; set; }
    }
}
