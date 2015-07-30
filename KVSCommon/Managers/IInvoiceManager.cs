using KVSCommon.Database;
using KVSCommon.Enums;
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

        /// <summary>
        /// Erstellt eine neue Rechnung.
        /// </summary>
        /// <param name="userId">Id des Benutzers.</param>
        /// <param name="invoiceRecipient">Rechnungsempfänger.</param>
        /// <param name="invoiceRecipientAdressId">Adresse des Rechnungsempfängers.</param>
        /// <param name="customerId">Id des Kunden.</param>
        /// <returns>Die neue Rechnung.</returns>
        Invoice CreateInvoice(int userId, string invoiceRecipient, Adress invoiceRecipientAdress, int customerId, double? discount, InvoiceType invType);
    }
}
