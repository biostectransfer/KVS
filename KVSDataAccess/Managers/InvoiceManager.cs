using KVSCommon.Database;
using KVSCommon.Entities;
using KVSCommon.Enums;
using KVSCommon.Managers;
using KVSDataAccess.Managers.Base;
using KVSDataAccess.PDF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSDataAccess.Managers
{
    public partial class InvoiceManager : EntityManager<Invoice, int>, IInvoiceManager
    {
        public InvoiceManager(IKVSEntities context) : base(context) { }

        /// <summary>
        /// Druckt die Rechnung im Original als PDF erstellt einen Datensatz zum Speichern des PDF in der Datenbank.
        /// </summary>
        /// <param name="invoice">Invoice</param>
        /// <param name="ms">MemoryStream, in den das PDF geschrieben wird.</param>
        /// <param name="logoFilePath">Dateipfad zum Logo für das PDF.</param>
        ///  <param name="defaultAccountNumber">Das Standard definierte Konto</param>
        public void Print(Invoice invoice, MemoryStream ms, string logoFilePath, string newAccountNumber, bool defaultAccountNumber = true)
        {
            if (invoice.IsPrinted)
            {
                throw new Exception("Die Rechnung wurde bereits gedruckt.");
            }

            var invoiceNumber = invoice.InvoiceNumber;
            if (invoiceNumber == null)
            {
                invoiceNumber = new InvoiceNumber()
                {
                    InvoiceId = invoice.Id
                };
                DataContext.AddObject(invoice);
                SaveChanges();
                invoice.InvoiceNumber = invoiceNumber;
            }

            var pdf = new InvoicePDF(DataContext, invoice, logoFilePath);
            pdf.WritePDF(ms);
            var doc = new Document()
            {
                Data = ms.ToArray(),
                DokumentTypeId = (int)DocumentTypes.Invoice,
                FileName = "Rechnung_" + invoiceNumber.Number.ToString() + ".pdf",
                MimeType = "application/pdf"
            };

            invoice.Document = doc;
            SaveChanges();
            DataContext.WriteLogItem("Rechnung " + invoiceNumber.Number.ToString() + " wurde gedruckt.", LogTypes.UPDATE, invoice.Id, "Invoice", doc.Id);
            invoice.IsPrinted = true;
            invoice.PrintDate = DateTime.Now;

            if (defaultAccountNumber)
            {
                UpdateAuthorativeAccounts(invoice,  newAccountNumber);
            }
        }

        /// <summary>
        /// Ändert das Erloeskonto für alle Amtlichen Gebühren
        /// </summary>
        /// <param name="dbContext">DB Kontext</param>
        /// <param name="invoice">Rechnungsobjekt</param>
        /// <param name="newAccountNumber">neues Erloeskonto</param>
        public void UpdateAuthorativeAccounts(Invoice invoice, string newAccountNumber)
        {
            if (newAccountNumber == string.Empty)
                throw new Exception("Es wurde kein Standard Erlös-Konto in der Konfiguration gefunden");

            var accountsToChange = DataContext.GetSet<AuthorativeChargeAccounts>().Where(q => q.InvoiceId == invoice.Id).ToList();
            if (accountsToChange.Count() > 0)
            {
                foreach (var atc in accountsToChange)
                {
                    var invItemAccountItem = DataContext.GetSet<InvoiceItemAccountItem>().SingleOrDefault(q => q.IIACCID == atc.InvoiceItemAccountItemId);
                    invItemAccountItem.RevenueAccountText = newAccountNumber;
                    DataContext.WriteLogItem("Rechnungsposition mit der ID: " + invItemAccountItem.IIACCID + " und der Rechnungsid: " + invoice.Id + " wurde auf das Standard Konto " + 
                        newAccountNumber + " geändert.",
                        LogTypes.UPDATE, invItemAccountItem.IIACCID, "InvoiceItemAccountItem", invItemAccountItem.IIACCID);

                    SaveChanges();
                }
            }
        }

        /// <summary>
        /// Erstellt eine neue Rechnung.
        /// </summary>
        /// <param name="userId">Id des Benutzers.</param>
        /// <param name="invoiceRecipient">Rechnungsempfänger.</param>
        /// <param name="invoiceRecipientAdressId">Adresse des Rechnungsempfängers.</param>
        /// <param name="customerId">Id des Kunden.</param>
        /// <returns>Die neue Rechnung.</returns>
        public Invoice CreateInvoice(int userId, string invoiceRecipient, Adress invoiceRecipientAdress, int customerId, double? discount, InvoiceType invType)
        {
            //if (string.IsNullOrEmpty(invoiceRecipient))
            //{
            //    throw new ArgumentNullException("Der Rechnungsempfänger darf nicht leer sein.");
            //}

            decimal? helper = null;
            var invoice = new Invoice()
            {
                CreateDate = DateTime.Now,
                IsPrinted = false,
                UserId = userId,
                InvoiceRecipient = invoiceRecipient,
                Adress = invoiceRecipientAdress,
                CustomerId = customerId,
                discount = ((discount.HasValue) ? decimal.Parse(discount.Value.ToString()) : helper),
                InvoiceType = (int)invType
            };

            DataContext.AddObject(invoice);
            SaveChanges();
            DataContext.WriteLogItem("Rechnung wurde angelegt.", LogTypes.INSERT, invoice.Id, "Invoice");
            return invoice;
        }
    }
}
