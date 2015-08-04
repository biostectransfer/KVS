using KVSCommon.Database;
using KVSCommon.Entities;
using KVSCommon.Enums;
using KVSCommon.Managers;
using KVSDataAccess.Managers.Base;
using KVSDataAccess.PDF;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace KVSDataAccess.Managers
{
    public partial class PackingListManager : EntityManager<PackingList, int>, IPackingListManager
    {
        public PackingListManager(IKVSEntities context) : base(context) { }

        /// <summary>
        /// Erstellt einen neuen Lieferscheindatensatz.
        /// </summary>
        /// <param name="recipient">Empfänger des Lieferscheins.</param>
        /// <param name="recipientAdressId">Id der Adresse für den Lieferschein.</param>
        /// <returns>Den neuen Lieferscheindatensatz.</returns>
        public PackingList CreatePackingList(string recipient, Adress recipientAdress)
        {
            if (recipient == null)
            {
                throw new Exception("Der Empfänger des Lieferscheins darf nicht leer sein.");
            }

            var packingList = new PackingList()
            {
                Adress = recipientAdress,
                Recipient = recipient
            };

            DataContext.AddObject(packingList);
            SaveChanges();
            DataContext.WriteLogItem("Lieferschein erstellt.", LogTypes.INSERT, packingList.Id, "PackingList");

            return packingList;
        }

        /// <summary>
        /// Fügt dem Lieferschein einen Auftrag anhand der AuftragsId hinzu.
        /// </summary>
        /// <param name="packingList">packingList</param>
        /// <param name="OrderNumber">Id des Auftrags.</param>
        public void AddOrderById(PackingList packingList, int orderNumber)
        {
            if (packingList.IsPrinted.GetValueOrDefault(false))
            {
                throw new Exception("Der Auftrag kann zum Lieferschein nicht hinzugefügt werden. Der Lieferschein wurde bereits gedruckt.");
            }

            var order = DataContext.GetSet<Order>().FirstOrDefault(q => q.Id == orderNumber);
            order.PackingListNumber = packingList.Id;
            order.ReadyToSend = true;
            SaveChanges();
            DataContext.WriteLogItem("Auftrag zum Lieferschein hinzugefügt.", LogTypes.UPDATE, packingList.Id, "PackingList", orderNumber);
        }

        /// <summary>
        /// Erstellt den Lieferschein als PDF und schreibt ihn in den übergebenen MemoryStream.
        /// </summary>
        /// <param name="packingList">packingList</param>
        /// <param name="ms">Der MemoryStream, in den das PDF geschrieben wird.</param>
        /// <param name="headerLogoPath">Der Pfad zum Logo für den Header.</param>
        /// <remarks>Setzt im Erfolgsfall im Lieferscheindatensatz das Merkmal "IsPrinted" auf "true".</remarks>
        public void Print(PackingList packingList, MemoryStream ms, string headerLogoPath, string fileName, bool isZulassungsStelle)
        {
            if (headerLogoPath == string.Empty)
            {
                headerLogoPath = DataContext.GetSet<DocumentConfiguration>().FirstOrDefault(q => q.Id == "LOGOPATH").Text;
            }

            if (!packingList.IsSelfDispatch.HasValue)
            {
                throw new Exception("Der Lieferschein kann nicht gedruckt werden. Es wurde nicht angegeben, ob es sich um eine Eigenverbringung handelt oder nicht.");
            }

            if (!packingList.IsSelfDispatch.Value && string.IsNullOrEmpty(packingList.DispatchOrderNumber))
            {
                throw new Exception("Der Lieferschein kann nicht gedruckt werden. Es wurde keine Versandauftragsnummer angegeben.");
            }

            //if (isZulassungsStelle)
            //{
            //    KVSCommon.PDF.PackingListPDF_Zulassungsstelle pdf = new PDF.PackingListPDF_Zulassungsstelle(this, headerLogoPath, dbContext);
            //    pdf.WritePDF(ms);
            //}
            //else
            //{
                var pdf = new PackingListPDF(DataContext, packingList, headerLogoPath);
                pdf.WritePDF(ms);
            //}


            var doc = new Document()
            {
                Data = ms.ToArray(),
                DokumentTypeId = (int)DocumentTypes.PackagingList,
                FileName = fileName,
                MimeType = "application/pdf"
            };

            DataContext.AddObject(doc);
            packingList.IsPrinted = true;
            packingList.Document = doc;
            SaveChanges();

            DataContext.WriteLogItem("Lieferschein " + fileName + " wurde gedruckt.", LogTypes.UPDATE, packingList.Id, "Versand", doc.Id);
            DataContext.WriteLogItem("Lieferschein wurde gedruckt.", LogTypes.UPDATE, packingList.Id, "PackingList");
        }

        /// <summary>
        /// Versendet den Lieferschein per Email, falls erforderlich.
        /// </summary>
        /// <param name="packingList">packingList</param>
        /// <param name="ms">MemoryStream mit dem Lieferschein-PDF.</param>
        /// <param name="fromAddress">Absenderemailadresse.</param>
        /// <param name="smtpServer">SMTP-Server für den Emailversand.</param>
        /// <returns>True, wenn eine Email versendet wurde, sonst false.</returns>
        public void SendByEmail(PackingList packingList, MemoryStream ms, string fromAddress, string smtpServer)
        {
            if (ms == null || ms.Length == 0)
            {
                throw new ArgumentException("Es wurde kein Anhang übergeben.");
            }

            if (packingList.Order.Count == 0)
            {
                throw new Exception("Dem Lieferschein sind keine Aufträge zugeordnet.");
            }

            var order = packingList.Order.First();
            var largeCustomer = order.Customer.LargeCustomer;
            List<string> emails = new List<string>();
            if (largeCustomer != null)
            {

                if (largeCustomer.SendPackingListToCustomer.GetValueOrDefault(false))
                {
                    emails.AddRange(largeCustomer.Mailinglist.Where(q => q.MailinglistType.Id == (int)MailingListTypes.PackagingList && q.Location == null).
                        Select(q => q.Email).ToList());
                }

                if (largeCustomer.SendPackingListToLocation.GetValueOrDefault(false))
                {
                    emails.AddRange(largeCustomer.Mailinglist.Where(q => q.MailinglistType.Id == (int)MailingListTypes.PackagingList &&
                        q.Location == order.Location).Select(q => q.Email).ToList());
                }
            }
            if (emails.Count != 0)
            {
                List<Attachment> attachments = new List<Attachment>();
                attachments.Add(new Attachment(ms, "Lieferschein.pdf", "application/pdf"));
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<p>Sehr geehrte Damen und Herren, </p>");
                sb.AppendLine("<br/>");
                sb.AppendLine("<p>im Anhang finden Sie einen Lieferschein zu abgeschlossenen Aufträgen.</p>");
                sb.AppendLine("<br/>");
                sb.AppendLine("<p>Mit freundlichen Grüßen,<br/>");
                sb.AppendLine("Ihr CASE-Team</p>");
                KVSCommon.Utility.Email.SendMail(fromAddress, emails, "Lieferschein " + packingList.Id.ToString(), sb.ToString(), null, null, smtpServer, attachments);

            }
        }

        /// <summary>
        /// Merged PDFs
        /// </summary>
        /// <param name="OrderNumber">File Array</param>
        /// <param name="return"> Gemerged PDF</param>
        public void MergePackingLists(string[] files, string mergedFileName)
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
    }
}
