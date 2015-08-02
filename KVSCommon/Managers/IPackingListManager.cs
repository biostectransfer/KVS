using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IPackingListManager : IEntityManager<PackingList, int>
    {
        /// <summary>
        /// Erstellt einen neuen Lieferscheindatensatz.
        /// </summary>
        /// <param name="recipient">Empfänger des Lieferscheins.</param>
        /// <param name="recipientAdressId">Id der Adresse für den Lieferschein.</param>
        /// <returns>Den neuen Lieferscheindatensatz.</returns>
        PackingList CreatePackingList(string recipient, Adress recipientAdress);

        /// <summary>
        /// Fügt dem Lieferschein einen Auftrag anhand der AuftragsId hinzu.
        /// </summary>
        /// <param name="packingList">packingList</param>
        /// <param name="OrderNumber">Id des Auftrags.</param>
        void AddOrderById(PackingList packingList, int orderNumber);

        /// <summary>
        /// Erstellt den Lieferschein als PDF und schreibt ihn in den übergebenen MemoryStream.
        /// </summary>
        /// <param name="packingList">packingList</param>
        /// <param name="ms">Der MemoryStream, in den das PDF geschrieben wird.</param>
        /// <param name="headerLogoPath">Der Pfad zum Logo für den Header.</param>
        /// <remarks>Setzt im Erfolgsfall im Lieferscheindatensatz das Merkmal "IsPrinted" auf "true".</remarks>
        void Print(PackingList packingList, MemoryStream ms, string headerLogoPath, string fileName, bool isZulassungsStelle);

        /// <summary>
        /// Versendet den Lieferschein per Email, falls erforderlich.
        /// </summary>
        /// <param name="packingList">packingList</param>
        /// <param name="ms">MemoryStream mit dem Lieferschein-PDF.</param>
        /// <param name="fromAddress">Absenderemailadresse.</param>
        /// <param name="smtpServer">SMTP-Server für den Emailversand.</param>
        /// <returns>True, wenn eine Email versendet wurde, sonst false.</returns>
        void SendByEmail(PackingList packingList, MemoryStream ms, string fromAddress, string smtpServer);

        /// <summary>
        /// Merged PDFs
        /// </summary>
        /// <param name="OrderNumber">File Array</param>
        /// <param name="return"> Gemerged PDF</param>
        void MergePackingLists(string[] files, string mergedFileName);
    }
}
