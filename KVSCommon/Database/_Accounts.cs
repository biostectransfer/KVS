using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Hilfsklasse für die Erlöskonten
    /// </summary>
    public class _Accounts
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public int AccountId { get; set; }
        /// <summary>
        /// Verweis auf die Rechnung
        /// </summary>
        public int InvoiceItemId { get; set; }
        /// <summary>
        /// Erlöskonto
        /// </summary>
        public string AccountNumber { get; set; }
    }
}
