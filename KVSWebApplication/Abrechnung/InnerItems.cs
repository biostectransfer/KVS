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
        public int ItemId { get; set; }
        public int? OrderItemId { get; set; }
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public bool isPrinted { get; set; }
        public int InvoiceItemId { get; set; }
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public bool Active { get; set; }
    }
}
