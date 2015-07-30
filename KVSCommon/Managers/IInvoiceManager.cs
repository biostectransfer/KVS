using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IInvoiceManager : IEntityManager<Invoice, int>
    {
        /// <summary>
        /// Druckt die Rechnung im Original als PDF erstellt einen Datensatz zum Speichern des PDF in der Datenbank.
        /// </summary>
        /// <param name="invoice">Invoice</param>
        /// <param name="ms">MemoryStream, in den das PDF geschrieben wird.</param>
        /// <param name="logoFilePath">Dateipfad zum Logo für das PDF.</param>
        ///  <param name="defaultAccountNumber">Das Standard definierte Konto</param>
        void Print(Invoice invoice, MemoryStream ms, string logoFilePath, string newAccountNumber, bool defaultAccountNumber = true);
    }
}
