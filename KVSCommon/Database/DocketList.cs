using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;
using System.Net.Mail;
using KVSCommon.Enums;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse für die DocketList Tabelle (Laufzettelverwaltung)
    /// </summary>
    public partial class DocketList : ILogging
    {
        public KVSEntities LogDBContext
        {
            get;
            set;
        }

        public object ItemId
        {
            get
            {
                return this.DocketListNumber;
            }
        }

        public EntityState EntityState
        {
            get;
            set;
        }

        /// <summary>
        /// Erstellt einen neuen Laufzetteldatensatz.
        /// </summary>
        /// <param name="recipient">Empfänger des Laufzettels.</param>
        /// <param name="recipientAdressId">Id der Adresse für den Laufzettel.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Den neuen Lieferscheindatensatz.</returns>
        public static DocketList CreateDocketList(string recipient, Adress recipientAdress, KVSEntities dbContext)
        {
            if (recipient == null)
            {
                throw new Exception("Der Empfänger des Lieferscheins darf nicht leer sein.");
            }

            DocketList docketList = new DocketList()
            {
                Adress = recipientAdress,
                Recipient = recipient
            };
                        
            dbContext.DocketList.InsertOnSubmit(docketList);
            dbContext.SubmitChanges();
            dbContext.WriteLogItem("Laufzettel erstellt.", LogTypes.INSERT, docketList.DocketListNumber, "DocketList");
            return docketList;
        }

        /// <summary>
        /// Fügt einen Laufzettel zum Auftrag anhand der AuftragsId hinzu.
        /// </summary>
        /// <param name="OrderNumber">Id des Auftrags.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        public void AddOrderById(int orderNumber, KVSEntities dbContext)
        {
            if (this.IsPrinted.GetValueOrDefault(false))
            {
                throw new Exception("Der Auftrag kann zum Lieferschein nicht hinzugefügt werden. Der Lieferschein wurde bereits gedruckt.");
            }

            var order = dbContext.Order.Single(q => q.OrderNumber == orderNumber);
            order.LogDBContext = dbContext;
            order.DocketListNumber = this.DocketListNumber;
            this.Order.Add(order);

            dbContext.WriteLogItem("Auftrag zum Laufzettel hinzugefügt.", LogTypes.UPDATE, this.DocketListNumber, "DocketList", orderNumber);
        }
        /// <summary>
        /// Merged PDFs
        /// </summary>
        /// <param name="OrderNumber">File Array</param>
        /// <param name="return"> Gemerged PDF</param>
        public static void MergeDocketLists(string[] files, string mergedFileName)
        {
            if (files.Length > 0)
            {
                PdfDocument outputDocument = new PdfDocument();
                foreach (string file in files)
                {
                    PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                    int count = inputDocument.PageCount;
                    for (int idx = 0; idx < count; idx++)
                    {
                        PdfPage page = inputDocument.Pages[idx];
                        outputDocument.AddPage(page);
                    }
                }

                outputDocument.Save(mergedFileName);
            }
        }

        /// <summary>
        /// Entfernt einen Laufzettel  aus dem Auftrag.
        /// </summary>
        /// <param name="OrderNumber">Id des Auftrags, der entfernt werden soll.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        public void RemoveOrderById(int orderNumber, KVSEntities dbContext)
        {
            if (this.IsPrinted.GetValueOrDefault(false))
            {
                throw new Exception("Der Auftrag kann nicht aus dem Laufzettel entfernt werden. Der Laufzettel wurde bereits gedruckt.");
            }

            Order order = dbContext.Order.Single(q => q.OrderNumber == orderNumber);
            order.LogDBContext = dbContext;
            order.DocketListNumber = null;
            dbContext.WriteLogItem("Auftrag aus Laufzettel entfernt.", LogTypes.UPDATE, this.DocketListNumber, "DocketList", orderNumber);
        }

        /// <summary>
        /// Erstellt den Laufzettel als PDF und schreibt ihn in den übergebenen MemoryStream.
        /// </summary>
        /// <param name="ms">Der MemoryStream, in den das PDF geschrieben wird.</param>
        /// <param name="headerLogoPath">Der Pfad zum Logo für den Header.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <remarks>Setzt im Erfolgsfall im Lieferscheindatensatz das Merkmal "IsPrinted" auf "true".</remarks>
        public void Print(MemoryStream ms, string headerLogoPath, KVSEntities dbContext, string fileName, bool isZulassungsStelle)
        {
            if (headerLogoPath == string.Empty)
            {
                headerLogoPath = dbContext.DocumentConfiguration.FirstOrDefault(q => q.Id == "LOGOPATH").Text;
            }

            if (!this.IsSelfDispatch.HasValue)
            {
                throw new Exception("Der Laufzettel kann nicht gedruckt werden. Es wurde nicht angegeben, ob es sich um eine Eigenverbringung handelt oder nicht.");
            }

            if (!this.IsSelfDispatch.Value && string.IsNullOrEmpty(this.DispatchOrderNumber))
            {
                throw new Exception("Der Laufzettel kann nicht gedruckt werden. Es wurde keine Versandauftragsnummer angegeben.");
            }
            KVSCommon.PDF.PackingListPDF_Zulassungsstelle pdf = new PDF.PackingListPDF_Zulassungsstelle(this, headerLogoPath, dbContext);
            pdf.WritePDF(ms);


            Document doc = new Document()
            {
                Data = ms.ToArray(),
                DocumentType = dbContext.DocumentType.Where(q => q.Id == (int)DocumentTypes.DocketList).Single(),
                FileName = fileName,
                MimeType = "application/pdf"
            };        

            dbContext.Document.InsertOnSubmit(doc);
            dbContext.SubmitChanges();

            this.DocumentId = doc.Id;
            dbContext.WriteLogItem("Laufzettel " + fileName + " wurde gedruckt.", LogTypes.UPDATE, this.DocketListNumber, "Versand", doc.Id);


            if (this.LogDBContext == null)
            {
                this.LogDBContext = dbContext;
            }

            this.IsPrinted = true;
            dbContext.WriteLogItem("Laufzettel wurde gedruckt.", LogTypes.UPDATE, this.DocketListNumber, "PackingList");
        }
        /// <summary>
        /// Versendet den Laufzettel per Email, falls erforderlich.
        /// </summary>
        /// <param name="ms">MemoryStream mit dem Lieferschein-PDF.</param>
        /// <param name="fromAddress">Absenderemailadresse.</param>
        /// <param name="smtpServer">SMTP-Server für den Emailversand.</param>
        /// <returns>True, wenn eine Email versendet wurde, sonst false.</returns>
        public void SendByEmail(MemoryStream ms, string fromAddress, string smtpServer)
        {
            if (ms == null || ms.Length == 0)
            {
                throw new ArgumentException("Es wurde kein Anhang übergeben.");
            }

            if (this.Order.Count == 0)
            {
                throw new Exception("Dem Laufzettel sind keine Aufträge zugeordnet.");
            }

            var largeCustomer = this.Order.First().Customer.LargeCustomer;
            List<string> emails = new List<string>();
            if (largeCustomer != null)
            {

                if (largeCustomer.SendPackingListToCustomer.GetValueOrDefault(false))
                {
                    emails.AddRange(largeCustomer.Mailinglist.Where(q => q.MailinglistType.Id == (int)MailingListTypes.DocketList && 
                        q.Location == null).Select(q => q.Email).ToList());
                }

                if (largeCustomer.SendPackingListToLocation.GetValueOrDefault(false))
                {
                    emails.AddRange(largeCustomer.Mailinglist.Where(q => q.MailinglistType.Id == (int)MailingListTypes.DocketList && 
                        q.Location == this.Order.First().Location).Select(q => q.Email).ToList());
                }



            }
            if (emails.Count != 0)
            {

                List<Attachment> attachments = new List<Attachment>();
                attachments.Add(new Attachment(ms, "Laufzettel.pdf", "application/pdf"));
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<p>Sehr geehrte Damen und Herren, </p>");
                sb.AppendLine("<br/>");
                sb.AppendLine("<p>im Anhang finden Sie einen Laufzettel zu abgeschlossenen Aufträgen.</p>");
                sb.AppendLine("<br/>");
                sb.AppendLine("<p>Mit freundlichen Grüßen,<br/>");
                sb.AppendLine("Ihr CASE-Team</p>");
                Utility.Email.SendMail(fromAddress, emails, "Laufzettel " + this.DocketListNumber.ToString(), sb.ToString(), null, null, smtpServer, attachments);

            }
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnDispatchOrderNumberChanging(string value)
        {
            this.WriteUpdateLogItem("Versandauftragsnummer", this.DispatchOrderNumber, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnIsSelfDispatchChanging(bool? value)
        {
            this.WriteUpdateLogItem("IstEigenverbringung", this.IsSelfDispatch, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnRecipientChanging(string value)
        {
            this.WriteUpdateLogItem("Empfänger", this.Recipient, value);
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

    }
}
