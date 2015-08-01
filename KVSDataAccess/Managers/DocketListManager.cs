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
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace KVSDataAccess.Managers
{
    public partial class DocketListManager : EntityManager<DocketList, int>, IDocketListManager
    {
        public DocketListManager(IKVSEntities context) : base(context) { }

        /// <summary>
        /// Erstellt einen neuen Laufzetteldatensatz.
        /// </summary>
        /// <param name="recipient">Empfänger des Laufzettels.</param>
        /// <param name="recipientAdressId">Id der Adresse für den Laufzettel.</param>
        /// <returns>Den neuen Lieferscheindatensatz.</returns>
        public DocketList CreateDocketList(string recipient, Adress recipientAdress)
        {
            if (recipient == null)
            {
                throw new Exception("Der Empfänger des Lieferscheins darf nicht leer sein.");
            }

            var docketList = new DocketList()
            {
                Adress = recipientAdress,
                Recipient = recipient
            };

            DataContext.AddObject(docketList);
            SaveChanges();
            DataContext.WriteLogItem("Laufzettel erstellt.", LogTypes.INSERT, docketList.DocketListNumber, "DocketList");
            return docketList;
        }

        /// <summary>
        /// Fügt einen Laufzettel zum Auftrag anhand der AuftragsId hinzu.
        /// </summary>
        /// <param name="docketList">DocketList</param>
        /// <param name="orderNumber">Id des Auftrags.</param>
        public void AddOrderById(DocketList docketList, int orderNumber)
        {
            if (docketList.IsPrinted.GetValueOrDefault(false))
            {
                throw new Exception("Der Auftrag kann zum Lieferschein nicht hinzugefügt werden. Der Lieferschein wurde bereits gedruckt.");
            }

            var order = DataContext.GetSet<Order>().Single(q => q.OrderNumber == orderNumber);
            order.DocketListNumber = docketList.DocketListNumber;
            docketList.Order.Add(order);

            SaveChanges();

            DataContext.WriteLogItem("Auftrag zum Laufzettel hinzugefügt.", LogTypes.UPDATE, docketList.DocketListNumber, "DocketList", orderNumber);
        }

        /// <summary>
        /// Fügt einen Laufzettel zum Auftrag anhand der AuftragsId hinzu.
        /// </summary>
        /// <param name="docketList">DocketList</param>
        /// <param name="order">Auftrag.</param>
        public void AddOrder(DocketList docketList, Order order)
        {
            if (docketList.IsPrinted.GetValueOrDefault(false))
            {
                throw new Exception("Der Auftrag kann zum Lieferschein nicht hinzugefügt werden. Der Lieferschein wurde bereits gedruckt.");
            }

            order.DocketListNumber = docketList.DocketListNumber;
            docketList.Order.Add(order);

            SaveChanges();

            DataContext.WriteLogItem("Auftrag zum Laufzettel hinzugefügt.", LogTypes.UPDATE, docketList.DocketListNumber, "DocketList", order.OrderNumber);
        }

        /// <summary>
        /// Erstellt den Laufzettel als PDF und schreibt ihn in den übergebenen MemoryStream.
        /// </summary>
        /// <param name="docketList">DocketList</param>
        /// <param name="ms">Der MemoryStream, in den das PDF geschrieben wird.</param>
        /// <param name="headerLogoPath">Der Pfad zum Logo für den Header.</param>
        /// <remarks>Setzt im Erfolgsfall im Lieferscheindatensatz das Merkmal "IsPrinted" auf "true".</remarks>
        public void Print(DocketList docketList, MemoryStream ms, string headerLogoPath, string fileName, bool isZulassungsStelle)
        {
            if (headerLogoPath == string.Empty)
            {
                headerLogoPath = DataContext.GetSet<DocumentConfiguration>().FirstOrDefault(q => q.Id == "LOGOPATH").Text;
            }

            if (!docketList.IsSelfDispatch.HasValue)
            {
                throw new Exception("Der Laufzettel kann nicht gedruckt werden. Es wurde nicht angegeben, ob es sich um eine Eigenverbringung handelt oder nicht.");
            }

            if (!docketList.IsSelfDispatch.Value && string.IsNullOrEmpty(docketList.DispatchOrderNumber))
            {
                throw new Exception("Der Laufzettel kann nicht gedruckt werden. Es wurde keine Versandauftragsnummer angegeben.");
            }

            var pdf = new PackingListPDF_Zulassungsstelle(docketList, headerLogoPath, DataContext);
            pdf.WritePDF(ms);

            var doc = new Document()
            {
                Data = ms.ToArray(),
                DokumentTypeId = (int)DocumentTypes.DocketList,
                FileName = fileName,
                MimeType = "application/pdf"
            };

            DataContext.AddObject(doc);
            docketList.DocumentId = doc.Id;
            docketList.IsPrinted = true;
            SaveChanges();

            DataContext.WriteLogItem("Laufzettel " + fileName + " wurde gedruckt.", LogTypes.UPDATE, docketList.DocketListNumber, "Versand", doc.Id);
            DataContext.WriteLogItem("Laufzettel wurde gedruckt.", LogTypes.UPDATE, docketList.DocketListNumber, "PackingList");
        }

        /// <summary>
        /// Versendet den Laufzettel per Email, falls erforderlich.
        /// </summary>
        /// <param name="docketList">DocketList</param>
        /// <param name="ms">MemoryStream mit dem Lieferschein-PDF.</param>
        /// <param name="fromAddress">Absenderemailadresse.</param>
        /// <param name="smtpServer">SMTP-Server für den Emailversand.</param>
        /// <returns>True, wenn eine Email versendet wurde, sonst false.</returns>
        public void SendByEmail(DocketList docketList, MemoryStream ms, string fromAddress, string smtpServer)
        {
            if (ms == null || ms.Length == 0)
            {
                throw new ArgumentException("Es wurde kein Anhang übergeben.");
            }

            if (docketList.Order.Count == 0)
            {
                throw new Exception("Dem Laufzettel sind keine Aufträge zugeordnet.");
            }

            var largeCustomer = docketList.Order.First().Customer.LargeCustomer;
            var emails = new List<string>();
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
                        q.Location == docketList.Order.First().Location).Select(q => q.Email).ToList());
                }
            }

            if (emails.Count != 0)
            {
                var attachments = new List<Attachment>();
                attachments.Add(new Attachment(ms, "Laufzettel.pdf", "application/pdf"));
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<p>Sehr geehrte Damen und Herren, </p>");
                sb.AppendLine("<br/>");
                sb.AppendLine("<p>im Anhang finden Sie einen Laufzettel zu abgeschlossenen Aufträgen.</p>");
                sb.AppendLine("<br/>");
                sb.AppendLine("<p>Mit freundlichen Grüßen,<br/>");
                sb.AppendLine("Ihr CASE-Team</p>");
                KVSCommon.Utility.Email.SendMail(fromAddress, emails, "Laufzettel " + docketList.DocketListNumber.ToString(), sb.ToString(), null, null, smtpServer, attachments);
            }
        }
    }
}
