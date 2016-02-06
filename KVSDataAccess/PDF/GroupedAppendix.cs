using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSDataAccess.PDF
{
    /// <summary>
    /// Hilfsklasse fuer die Zusatzdokumente (Anhang)
    /// </summary>
    public class GroupedAppendix
    {
        public int? OrderItemId;
      
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
        public string Comment;
    }
}
