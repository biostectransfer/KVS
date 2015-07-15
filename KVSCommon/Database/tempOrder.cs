using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Temporaere Klasse fuer Auftrag und Auftragspositionen
    /// </summary>
    class tempOrder
    {
        public KVSCommon.Database.Order order { get; set; }
        public KVSCommon.Database.OrderItem orderItem { get; set; }
    }
}
