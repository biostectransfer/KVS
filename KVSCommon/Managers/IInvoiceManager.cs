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
        Invoice CreateInvoice(string invoiceRecipient, Adress invoiceRecipientAdress, int customerId, double? discount, InvoiceType invType);

        /// <summary>
        /// Fügt der Rechnung eine neue Rechnungsposition hinzu.
        /// </summary>
        /// <param name="name">Bezeichnung für die Rechnungsposition.</param>
        /// <param name="amount">Betrag der Rechnungsposition.</param>
        /// <param name="count">Anzahl für die Position.</param>
        /// <param name="orderItemId">Id der Auftragsposition, falls vorhande.</param>
        /// <param name="costCenterId">Id der Kostenstelle, falls benötigt.</param>
        /// <returns>Die neue Rechnungsposition.</returns>
        InvoiceItem AddInvoiceItem(Invoice invoice, string name, decimal amount, int count, OrderItem orderItem, CostCenter costCenter,
            Customer customer, OrderItemStatusTypes orderItemStatusType);

        /// <summary>
        /// Get InvoiceRunReports
        /// </summary>
        /// <returns></returns>
        IQueryable<InvoiceRunReport> GetInvoiceRunReports();

        /// <summary>
        /// Erstellt eine Vorschau des Rechnungs-PDF.
        /// </summary>
        /// <param name="invoice">invoice</param>
        /// <param name="ms">MemoryStream, in den die PDF-Daten geschrieben werden.</param>
        /// <param name="logoFilePath">Dateipfad zum Logo für das PDF.</param>
        void PrintPreview(Invoice invoice, MemoryStream ms, string logoFilePath);

        /// <summary>
        /// Erstellt eine Kopie der Original-Rechnung, falls diese bereits gedruckt wurde.
        /// </summary>
        /// <param name="invoice">invoice</param>
        /// <param name="ms">MemoryStream, in den die PDF-Daten geschrieben werden.</param>
        void PrintCopy(Invoice invoice, MemoryStream ms);

        /// <summary>
        /// Add run report
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="invoiceTypeId"></param>
        void AddRunReport(int? customerId, int? invoiceTypeId);

        /// <summary>
        /// Gibt die initiale Rechnungsadresse anhand der Kundendaten und des Standorts der Aufträge in der Rechnung zurück.
        /// </summary>
        /// <param name="customerId">Id des Kunden.</param>
        /// <param name="locationId">Id des Standorts, falls vorhanden.</param>
        /// <returns>Die Rechnungsadresse.</returns>
        Adress GetInitialInvoiceAdress(int customerId, int? locationId);

        /// <summary>
        /// Gibt den Standard Rechnungstext zurück, abhaengig vom Kunden
        /// </summary>
        /// <param name="customerId">Kundenid</param>
        /// <returns>Rechnungstext</returns>
        string GetDefaultInvoiceText(int customerId);

        /// <summary>
        /// Storniert eine Rechnung.
        /// </summary>
        /// <param name="invoice">invoice</param>
        void Cancel(Invoice invoice);

        /// <summary>
        /// Generiert und versendet die Rechnung per Email
        /// </summary>
        /// <param name="invoiceId">Rechnungsid</param>
        /// <param name="smtpServer">SMTP</param>
        /// <param name="fromAddress">Absender</param>
        /// <param name="toAdresses">Empfaenger</param>
        void SendByMail(int invoiceId, string smtpServer, string fromAddress, List<string> toAdresses = null);
    }
}
