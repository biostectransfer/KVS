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
        /// <param name="invoiceRecipient">Rechnungsempfänger.</param>
        /// <param name="invoiceRecipientAdressId">Adresse des Rechnungsempfängers.</param>
        /// <param name="customerId">Id des Kunden.</param>
        /// <returns>Die neue Rechnung.</returns>
        public Invoice CreateInvoice(string invoiceRecipient, Adress invoiceRecipientAdress, int customerId, double? discount, InvoiceType invType)
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
                UserId = DataContext.LogUserId,
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

        /// <summary>
        /// Fügt der Rechnung eine neue Rechnungsposition hinzu.
        /// </summary>
        /// <param name="name">Bezeichnung für die Rechnungsposition.</param>
        /// <param name="amount">Betrag der Rechnungsposition.</param>
        /// <param name="count">Anzahl für die Position.</param>
        /// <param name="orderItemId">Id der Auftragsposition, falls vorhande.</param>
        /// <param name="costCenterId">Id der Kostenstelle, falls benötigt.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Die neue Rechnungsposition.</returns>
        public InvoiceItem AddInvoiceItem(Invoice invoice, string name, decimal amount, int count, OrderItem orderItem, CostCenter costCenter,
            Customer customer, OrderItemStatusTypes orderItemStatusType)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Die Bezeichnung der Rechnungsposition darf nicht leer sein.");
            }

            if (invoice.IsPrinted)
            {
                throw new Exception("Die Rechnungsposition kann nicht hinzugefügt werden: Die Rechnung ist bereits gedruckt.");
            }

            var item = new InvoiceItem()
            {
                Amount = amount,
                Count = count,
                Name = name,
                OrderItem = orderItem,
                CostCenter = costCenter
            };

            invoice.InvoiceItem.Add(item);
            SaveChanges();
            DataContext.WriteLogItem("Rechnungsposition " + name + " zur Rechnung hinzugefügt.", LogTypes.INSERT, invoice.Id, "InvoiceItem", item.Id);

            if (orderItem != null)
            {
                if (orderItem.Status == (int)OrderItemStatusTypes.Payed)
                {
                    throw new Exception("Die Auftragsposition ist bereits abgerechnet.");
                }

                if (orderItem.Status != (int)OrderItemStatusTypes.Closed)
                {
                    throw new Exception("Die Auftragsposition ist nicht abgeschlossen.");
                }

                if (orderItem.Order.LocationId.HasValue && invoice.OrderInvoice.Any(q => q.Order.LocationId != orderItem.Order.LocationId))
                {
                    throw new Exception("Die Auftragsposition kann nicht zur Rechnung hinzugefügt werden, da der Standort des Auftrags nicht mit dem Standort der bisherigen Aufträge in der Rechnung übereinstimmt.");
                }

                if (orderItem.NeedsVAT)
                {
                    if (orderItem.Order.Location != null && orderItem.Order.Location.VAT.HasValue) //Großkunde
                    {
                        item.VAT = orderItem.Order.Location.VAT.Value;
                    }
                    else //SofortKunde
                    {
                        item.VAT = customer.VAT;
                    }
                }

                orderItem.Status = (int)orderItemStatusType;
                var order = orderItem.Order;
                if (!DataContext.GetSet<OrderInvoice>().Any(q => q.OrderNumber == order.OrderNumber && q.InvoiceId == invoice.Id))
                {
                    CreateOrderInvoice(order, invoice);
                }
            }
            else
            {
                item.VAT = customer.VAT;
            }

            return item;
        }

        /// <summary>
        /// Erstellt eine Verknuepfung zwischen einem Auftrag und einer Rechnung.
        /// </summary>
        /// <param name="OrderNumber">Id des Auftrags.</param>
        /// <param name="invoiceId">Id der Rechnung.</param>
        /// <returns>Die neue Verknüpfung.</returns>
        public OrderInvoice CreateOrderInvoice(Order order, Invoice invoice)
        {
            var item = new OrderInvoice()
            {
                Order = order,
                Invoice = invoice
            };

            DataContext.AddObject(item);
            DataContext.WriteLogItem("Rechnung wurde mit Auftrag " + order.OrderNumber + " verknüpft.", LogTypes.INSERT, order.OrderNumber, "OrderInvoice", invoice.Id);
            return item;
        }

        /// <summary>
        /// Get InvoiceRunReports
        /// </summary>
        /// <returns></returns>
        public IQueryable<InvoiceRunReport> GetInvoiceRunReports()
        {
            return DataContext.GetSet<InvoiceRunReport>();
        }

        /// <summary>
        /// Erstellt eine Vorschau des Rechnungs-PDF.
        /// </summary>
        /// <param name="invoice">invoice</param>
        /// <param name="ms">MemoryStream, in den die PDF-Daten geschrieben werden.</param>
        /// <param name="logoFilePath">Dateipfad zum Logo für das PDF.</param>
        public void PrintPreview(Invoice invoice, MemoryStream ms, string logoFilePath)
        {
            var pdf = new InvoicePDF(DataContext, invoice, logoFilePath);
            using (MemoryStream tempStream = new MemoryStream())
            {
                pdf.Preview = true;
                pdf.WritePDF(tempStream);
                PDFSharpUtil.WriteWatermark("", tempStream, ms);
            }
        }

        /// <summary>
        /// Erstellt eine Kopie der Original-Rechnung, falls diese bereits gedruckt wurde.
        /// </summary>
        /// <param name="invoice">invoice</param>
        /// <param name="ms">MemoryStream, in den die PDF-Daten geschrieben werden.</param>
        public void PrintCopy(Invoice invoice, MemoryStream ms)
        {
            if (invoice.IsPrinted == false)
            {
                throw new Exception("Kopie kann nicht erstellt werden, da Original nicht gedruckt ist.");
            }

            //string watermarkText = "Kopie";
            using (MemoryStream readerStream = new MemoryStream())
            {
                try
                {
                    readerStream.Write(invoice.Document.Data.ToArray(), 0, invoice.Document.Data.Length);
                }
                catch (Exception ex)
                {
                    throw new Exception("Fehler Erstellen der Kopie: Original-PDF konnte nicht gelesen werden.", ex);
                }

                PDFSharpUtil.WriteWatermark("Kopie", readerStream, ms);
            }
        }

        /// <summary>
        /// Add run report
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="invoiceTypeId"></param>
        public void AddRunReport(int? customerId, int? invoiceTypeId)
        {
            var run = new InvoiceRunReport()
            {
                CustomerId = customerId,
                InvoiceTypeId = invoiceTypeId,
                CreateDate = DateTime.Now
            };

            DataContext.AddObject(run);
            SaveChanges();
        }
    }
}
