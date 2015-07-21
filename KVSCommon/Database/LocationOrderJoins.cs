using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Hilfklasse für die Standort Auftrags Verknuepfungen
    /// </summary>
    public class LocationOrderJoins
    {
        public int LocationId { get; set; }
        public Order Order { get; set; }
    }  
}
