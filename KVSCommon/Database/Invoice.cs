using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    using System.IO;
    using KVSCommon.PDF;
    using PdfSharp.Pdf;
    using PdfSharp.Drawing;
    using System.Net.Mail;
    using System.Configuration;
    /// <summary>
    /// Erweiterungsklasse für die Tabelle Invoice
    /// </summary>
    public partial class Invoice : ILogging
    {
        public DataClasses1DataContext LogDBContext
        {
            get;
            set;
        }

        public object ItemId
        {
            get
            {
                return this.Id;
            }
        }

        public EntityState EntityState
        {
            get;
            set;
        }

        /// <summary>
        /// Erstellt eine neue Rechnung.
        /// </summary>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <param name="userId">Id des Benutzers.</param>
        /// <param name="invoiceRecipient">Rechnungsempfänger.</param>
        /// <param name="invoiceRecipientAdressId">Adresse des Rechnungsempfängers.</param>
        /// <param name="customerId">Id des Kunden.</param>
        /// <returns>Die neue Rechnung.</returns>
        public static Invoice CreateInvoice(DataClasses1DataContext dbContext, int userId, string invoiceRecipient, Adress invoiceRecipientAdress, int customerId, double? discount, string invTypeId)
        {
            //if (string.IsNullOrEmpty(invoiceRecipient))
            //{
            //    throw new ArgumentNullException("Der Rechnungsempfänger darf nicht leer sein.");
            //}

            decimal? helper = null;
            Invoice invoice = new Invoice()
            {
                CreateDate = DateTime.Now,
                IsPrinted = false,
                UserId = userId,
                InvoiceRecipient = invoiceRecipient,
                Adress = invoiceRecipientAdress,
                CustomerId = customerId,
                discount = ((discount.HasValue) ? decimal.Parse(discount.Value.ToString()) : helper),
                InvoiceType = GetInvoiceTypeId(dbContext, invTypeId)
            };

            dbContext.Invoice.InsertOnSubmit(invoice);
            dbContext.WriteLogItem("Rechnung wurde angelegt.", LogTypes.INSERT, invoice.Id, "Invoice");
            return invoice;
        }
        /// <summary>
        /// Gibt die id des Rechnungstyps zurück
        /// </summary>
        /// <param name="dbContext">Datenbank Kontext</param>
        /// <param name="myWhere">Rechnungstyp</param>
        /// <returns>Rechnungstyp ID</returns>
        public static int? GetInvoiceTypeId(DataClasses1DataContext dbContext, string myWhere)
        {
            int? type = null;

            var helperTypes = dbContext.InvoiceTypes.FirstOrDefault(q => q.InvoiceTypeName == myWhere);
            if (helperTypes == null)
                return null;
            else
                type = helperTypes.ID;

            return type;
        }

        /// <summary>
        /// Gibt die initiale Rechnungsadresse anhand der Kundendaten und des Standorts der Aufträge in der Rechnung zurück.
        /// </summary>
        /// <param name="customerId">Id des Kunden.</param>
        /// <param name="locationId">Id des Standorts, falls vorhanden.</param>
        /// <param name="dbContext">Datenbankkontext für die Abfrage.</param>
        /// <returns>Die Rechnungsadresse.</returns>
        public static Adress GetInitialInvoiceAdress(int customerId, int? locationId, DataClasses1DataContext dbContext)
        {
            var customer = dbContext.Customer.Single(q => q.Id == customerId);
            LargeCustomer largeCustomer = customer.LargeCustomer;
            if (largeCustomer != null)
            {
                if (largeCustomer.SendInvoiceToMainLocation)
                {
                    if (largeCustomer.MainLocation.InvoiceAdress != null)
                    {
                        return largeCustomer.MainLocation.InvoiceAdress;
                    }
                }
                else if (locationId.HasValue)
                {
                    Location location = dbContext.Location.Single(q => q.Id == locationId.Value);
                    if (location.InvoiceAdress != null)
                    {
                        return location.InvoiceAdress;
                    }
                }
            }

            return customer.InvoiceAdress;
        }

        /// <summary>
        /// Gibt die Rechnungsversandadresse anhand der Kundendaten und des Standorts der Aufträge in der Rechnung zurück.
        /// </summary>
        /// <returns>Die Rechnungsversandadresse.</returns>
        public static Adress GetInvoiceDispatchAdress(int customerId, int? locationId, DataClasses1DataContext dbContext)
        {
            var customer = dbContext.Customer.Single(q => q.Id == customerId);
            LargeCustomer largeCustomer = customer.LargeCustomer;
            if (largeCustomer != null)
            {
                if (largeCustomer.SendInvoiceToMainLocation)
                {
                    if (largeCustomer.MainLocation.InvoiceDispatchAdress != null)
                    {
                        return largeCustomer.MainLocation.InvoiceDispatchAdress;
                    }
                }
                else if (locationId.HasValue)
                {
                    Location location = dbContext.Location.Single(q => q.Id == locationId.Value);
                    if (location.InvoiceDispatchAdress != null)
                    {
                        return location.InvoiceDispatchAdress;
                    }
                }
            }

            return customer.InvoiceDispatchAdress;
        }
        /// <summary>
        /// Generiert und versendet die Rechnung per Email
        /// </summary>
        /// <param name="dbContext">Dastenbank Kontext</param>
        /// <param name="invoiceId">Rechnungsid</param>
        /// <param name="smtpServer">SMTP</param>
        /// <param name="fromAddress">Absender</param>
        /// <param name="toAdresses">Empfaenger</param>
        public static void SendByMail(DataClasses1DataContext dbContext, int invoiceId, string smtpServer, string fromAddress, List<string> toAdresses = null)
        {

            var invoice = dbContext.Invoice.Single(q => q.Id == invoiceId);
            if (invoice.Document != null)
            {
                MemoryStream ms = new MemoryStream();
                ms.Write(invoice.Document.Data.ToArray(), 0, invoice.Document.Data.Length);
                ms.Position = 0;
                Attachment att = new Attachment(ms, invoice.Document.FileName, "application/pdf");
                int? locationId = null;

                if (invoice.OrderInvoice.Any())
                {
                    locationId = invoice.OrderInvoice.First().Order.LocationId;
                }
                if (toAdresses == null)
                {
                    toAdresses = invoice.Customer.LargeCustomer.GetMailinglistAdresses(dbContext, locationId, "Rechnung");
                }
                var mailBody = dbContext.DocumentConfiguration.FirstOrDefault(q => q.Id == "MAILBODY");

                KVSCommon.Utility.Email.SendMail(fromAddress, toAdresses, "Rechnung #" + invoice.InvoiceNumber.Number, ((mailBody != null) ? mailBody.Text : "No Mailbody found"), new List<string>(), new List<string>(), smtpServer, new List<Attachment>() { att });
            }
            else
            {
                throw new Exception("Zu der Rechnung ist kein Rechnungs-PDF gespeichert.");
            }

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
        public InvoiceItem AddInvoiceItem(string name, decimal amount, int count, OrderItem orderItem, CostCenter costCenter, DataClasses1DataContext dbContext)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Die Bezeichnung der Rechnungsposition darf nicht leer sein.");
            }

            if (this.IsPrinted)
            {
                throw new Exception("Die Rechnungsposition kann nicht hinzugefügt werden: Die Rechnung ist bereits gedruckt.");
            }

            Customer customer = dbContext.Customer.Single(q => q.Id == this.CustomerId);
            InvoiceItem item = new InvoiceItem()
            {
                Amount = amount,
                Count = count,
                Name = name,
                OrderItem = orderItem,
                CostCenter = costCenter
            };

            this.InvoiceItem.Add(item);
            dbContext.WriteLogItem("Rechnungsposition " + name + " zur Rechnung hinzugefügt.", LogTypes.INSERT, this.Id, "InvoiceItem", item.Id);
            if (orderItem != null)
            {
                if (orderItem.Status == (int)OrderItemState.Abgerechnet)
                {
                    throw new Exception("Die Auftragsposition ist bereits abgerechnet.");
                }

                if (orderItem.Status != (int)OrderItemState.Abgeschlossen)
                {
                    throw new Exception("Die Auftragsposition ist nicht abgeschlossen.");
                }

                if (orderItem.Order.LocationId.HasValue && this.OrderInvoice.Any(q => q.Order.LocationId != orderItem.Order.LocationId))
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

                orderItem.LogDBContext = dbContext;
                orderItem.Status = (int)OrderItemState.Abgerechnet;
                var order = orderItem.Order;
                if (!dbContext.OrderInvoice.Any(q => q.OrderNumber == order.OrderNumber && q.InvoiceId == this.Id))
                {
                    Database.OrderInvoice.CreateOrderInvoice(order, this, dbContext);
                }
            }
            else
            {
                item.VAT = customer.VAT;
            }

            return item;
        }

        /// <summary>
        /// Erstellt eine Vorschau des Rechnungs-PDF.
        /// </summary>
        /// <param name="ms">MemoryStream, in den die PDF-Daten geschrieben werden.</param>
        /// <param name="logoFilePath">Dateipfad zum Logo für das PDF.</param>
        public void PrintPreview(DataClasses1DataContext dbContext, MemoryStream ms, string logoFilePath)
        {
            InvoicePDF pdf = new InvoicePDF(dbContext, this, logoFilePath);
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
        /// <param name="ms">MemoryStream, in den die PDF-Daten geschrieben werden.</param>
        public void PrintCopy(MemoryStream ms)
        {
            if (this.IsPrinted == false)
            {
                throw new Exception("Kopie kann nicht erstellt werden, da Original nicht gedruckt ist.");
            }

            //string watermarkText = "Kopie";
            using (MemoryStream readerStream = new MemoryStream())
            {
                try
                {
                    readerStream.Write(this.Document.Data.ToArray(), 0, this.Document.Data.Length);
                }
                catch (Exception ex)
                {
                    throw new Exception("Fehler Erstellen der Kopie: Original-PDF konnte nicht gelesen werden.", ex);
                }

                PDFSharpUtil.WriteWatermark("Kopie", readerStream, ms);
            }
        }

        /// <summary>
        /// Druckt die Rechnung im Original als PDF erstellt einen Datensatz zum Speichern des PDF in der Datenbank.
        /// </summary>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <param name="ms">MemoryStream, in den das PDF geschrieben wird.</param>
        /// <param name="logoFilePath">Dateipfad zum Logo für das PDF.</param>
        ///  <param name="defaultAccountNumber">Das Standard definierte Konto</param>
        public void Print(DataClasses1DataContext dbContext, MemoryStream ms, string logoFilePath, bool defaultAccountNumber = true)
        {
            if (this.IsPrinted)
            {
                throw new Exception("Die Rechnung wurde bereits gedruckt.");
            }

            this.LogDBContext = dbContext;

            InvoiceNumber num = this.InvoiceNumber;
            if (num == null)
            {
                num = new InvoiceNumber()
                            {
                                InvoiceId = this.Id
                            };
                dbContext.InvoiceNumber.InsertOnSubmit(num);
                dbContext.SubmitChanges();
                this.InvoiceNumber = num;

            }

            InvoicePDF pdf = new InvoicePDF(dbContext, this, logoFilePath);
            pdf.WritePDF(ms);
            Document doc = new Document()
            {
                Data = ms.ToArray(),
                DocumentType = dbContext.DocumentType.Where(q => q.Name == "Rechnung").Single(),
                FileName = "Rechnung_" + num.Number.ToString() + ".pdf",
                MimeType = "application/pdf"
            };

            this.Document = doc;
            dbContext.Document.InsertOnSubmit(doc);
            dbContext.WriteLogItem("Rechnung " + num.Number.ToString() + " wurde gedruckt.", LogTypes.UPDATE, this.Id, "Invoice", doc.Id);
            this.IsPrinted = true;
            this.PrintDate = DateTime.Now;

            if (defaultAccountNumber)
            {
                InvoiceItemAccountItem.UpdateAuthorativeAccounts(dbContext, this, Convert.ToString(ConfigurationManager.AppSettings["DefaultAccountNumber"]));
            }
        }

        /// <summary>
        /// Storniert eine Rechnung.
        /// </summary>
        /// <param name="dbContext"></param>
        public void Cancel(DataClasses1DataContext dbContext)
        {
            if (this.IsPrinted)
            {
                throw new Exception("Die Rechnung kann nicht storniert werden, da sie bereits gedruckt ist.");
            }

            foreach (var item in this.InvoiceItem)
            {
                if (item.OrderItem != null)
                {
                    item.OrderItem.LogDBContext = dbContext;
                    item.OrderItem.Status = (int)OrderItemState.Abgeschlossen;
                }
            }

            foreach (var item in this.OrderInvoice)
            {
                if (item.Order.OrderItem.All(q => q.Status == (int)OrderItemState.Abgeschlossen))
                {
                    item.Order.LogDBContext = dbContext;
                    item.Order.Status = (int)OrderState.Abgeschlossen;
                }
            }

            dbContext.OrderInvoice.DeleteAllOnSubmit(this.OrderInvoice);

        }
        /// <summary>
        /// Nettobetrag
        /// </summary>
        public decimal NetSum
        {
            get
            {

                var query = this.InvoiceItem.Where(q => q.OrderItem == null || q.OrderItem.IsAuthorativeCharge == false).Where(q => q.VAT > 0);
                decimal? discount = null;
                if (this.InvoiceItem.FirstOrDefault() != null)
                {
                    discount = this.InvoiceItem.FirstOrDefault().Invoice.discount;
                }
                if (query.Count() > 0)
                {
                    if (discount.HasValue && discount.Value != 0)
                    {
                        return query.Sum(q => q.Amount * q.Count) * ((100 - discount.Value) / 100);
                    }
                    else
                    {
                        return query.Sum(q => q.Amount * q.Count);
                    }
                }

                return 0;
            }
        }
        /// <summary>
        /// MwSt
        /// </summary>
        public decimal TaxValue
        {
            get
            {
                var query = this.InvoiceItem.Where(q => q.OrderItem == null || q.OrderItem.IsAuthorativeCharge == false).Where(q => q.VAT > 0);
                decimal? discount = null;
                if (this.InvoiceItem.FirstOrDefault() != null)
                {
                    discount = this.InvoiceItem.FirstOrDefault().Invoice.discount;
                }
                if (query.Count() > 0)
                {
                    if (discount.HasValue && discount.Value != 0)
                    {

                        return query.Sum(q => (q.Amount * q.Count) * ((100 - discount.Value) / 100) * q.VAT / 100);
                    }
                    else
                    {
                        return query.Sum(q => q.Amount * q.Count * q.VAT / 100);

                    }
                }

                return 0;
            }
        }
        /// <summary>
        /// Gruppe der Steuerfreien Rechnungspositionen
        /// </summary>
        public List<KeyValuePair<string, decimal>> TaxFreeItemsGrouped
        {
            get
            {
                var query = this.InvoiceItem.Where(q => q.OrderItem == null || q.OrderItem.IsAuthorativeCharge == false).Where(q => q.VAT == 0);
                if (query.Count() > 0)
                {
                    return query.GroupBy(q => q.Name).Select(q => new KeyValuePair<string, decimal>(q.Key, q.Sum(p => p.Amount * p.Count))).ToList();
                }

                return new List<KeyValuePair<string, decimal>>();
            }
        }
        /// <summary>
        /// Summe der amtlichen Gebuehren
        /// </summary>
        public decimal AuthorativeChargeSum
        {
            get
            {
                var query = this.InvoiceItem.Where(q => q.OrderItem != null && q.OrderItem.IsAuthorativeCharge);
                if (query.Count() > 0)
                {
                    return query.Sum(q => q.Amount * q.Count);
                }

                return 0;
            }
        }
        /// <summary>
        /// Endbetrag
        /// </summary>
        public decimal GrandTotal
        {
            get
            {
                return this.NetSum + this.TaxValue + this.AuthorativeChargeSum + this.TaxFreeItemsGrouped.Sum(q => q.Value);
            }
        }
        /// <summary>
        /// Gibt den Standard Rechnungstext zurück, abhaengig vom Kunden
        /// </summary>
        /// <param name="dbContext">Dastenbank Kontext</param>
        /// <param name="customerId">Kundenid</param>
        /// <param name="userId">Benutzerid</param>
        /// <returns>Rechnungstext</returns>
        public static string GetDefaultInvoiceText(DataClasses1DataContext dbContext, int customerId, int userId)
        {

            Customer customer = dbContext.Customer.Single(q => q.Id == customerId);
            //User user = dbContext.User.Single(q => q.Id == userId);
            string defaultText = dbContext.DocumentConfiguration.Where(q => q.Id == "INVOICE_TEXT").Select(q => q.Text).SingleOrDefault();
            if (!string.IsNullOrEmpty(defaultText))
            {
                //return string.Format(defaultText, DateTime.Now.AddDays(customer.TermOfCredit.GetValueOrDefault(30)).ToShortDateString(), user.Person.FullName);
                return string.Format(defaultText, DateTime.Now.AddDays(customer.TermOfCredit.GetValueOrDefault(30)).ToShortDateString());
            }

            return string.Empty;

        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCreated()
        {
            this.EntityState = Database.EntityState.New;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnLoaded()
        {
            this.EntityState = Database.EntityState.Loaded;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCreateDateChanging(DateTime value)
        {
            if (this.EntityState != EntityState.New)
            {
                throw new Exception("Das Rechnungserstelldatum darf nicht geändert werden.");
            }
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnInvoiceRecipientChanging(string value)
        {
            this.WriteUpdateLogItem("Rechnungsempfänger", this.InvoiceRecipient, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnIsPrintedChanging(bool value)
        {
            this.WriteUpdateLogItem("IstGedruckt", this.IsPrinted, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnPrintDateChanging(DateTime? value)
        {
            this.WriteUpdateLogItem("Druckdatum", this.PrintDate, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCustomerIdChanging(int value)
        {
            if (this.EntityState != Database.EntityState.New)
            {
                throw new Exception("Der Kunde einer Rechnung kann nicht geändert werden.");
            }
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnInvoiceTextChanging(string value)
        {
            this.WriteUpdateLogItem("Rechnungstext", this.InvoiceText, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnInvoiceTypeChanging(int? value)
        {
            this.WriteUpdateLogItem("Rechnungstype", this.InvoiceType, value);
        }
    }
}
