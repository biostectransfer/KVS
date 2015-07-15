using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace KVSWebApplication.ImportExport
{
    /// <summary>
    /// Hilfklasse um viruelle Rechnungen fuer den Export zu erstellen
    /// </summary>
    public class VirtualInvoice
    {
        public string account { get; set; }
        public KVSCommon.Database.Invoice Invoice { get; set; }
        public decimal accountSum { get; set; }
    }
}