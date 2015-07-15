using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.PDF
{
    /// <summary>
    /// Hilfsklasse fuer die Zusatzdokumente (Anhang)
    /// </summary>
    public class GroupedAppendix
    {
        public Guid? OrderItemId;
      
        public int PosNumber;
        public string Bezeichnung;
        public string Halter;
        public string Vin;
        public string Kennzeichen;
        public string Zulassungsdatum;
        public string Farbe;
        public string OrderNumber;
        public string Amount;
        public string AuthorativeCharge;
        public bool IsAuthorativeCharge; 
    }
}
