using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IDocketListManager : IEntityManager<DocketList, int>
    {
        /// <summary>
        /// Erstellt einen neuen Laufzetteldatensatz.
        /// </summary>
        /// <param name="recipient">Empfänger des Laufzettels.</param>
        /// <param name="recipientAdressId">Id der Adresse für den Laufzettel.</param>
        /// <returns>Den neuen Lieferscheindatensatz.</returns>
        DocketList CreateDocketList(string recipient, Adress recipientAdress);

        /// <summary>
        /// Fügt einen Laufzettel zum Auftrag anhand der AuftragsId hinzu.
        /// </summary>
        /// <param name="docketList">DocketList</param>
        /// <param name="OrderNumber">Id des Auftrags.</param>
        void AddOrderById(DocketList docketList, int orderNumber);

        /// <summary>
        /// Fügt einen Laufzettel zum Auftrag anhand der AuftragsId hinzu.
        /// </summary>
        /// <param name="docketList">DocketList</param>
        /// <param name="Order">Auftrag.</param>
        void AddOrder(DocketList docketList, Order orders);

        /// <summary>
        /// Erstellt den Laufzettel als PDF und schreibt ihn in den übergebenen MemoryStream.
        /// </summary>
        /// <param name="docketList">DocketList</param>
        /// <param name="ms">Der MemoryStream, in den das PDF geschrieben wird.</param>
        /// <param name="headerLogoPath">Der Pfad zum Logo für den Header.</param>
        /// <remarks>Setzt im Erfolgsfall im Lieferscheindatensatz das Merkmal "IsPrinted" auf "true".</remarks>
        void Print(DocketList docketList, MemoryStream ms, string headerLogoPath, string fileName, bool isZulassungsStelle);

        /// <summary>
        /// Versendet den Laufzettel per Email, falls erforderlich.
        /// </summary>
        /// <param name="docketList">DocketList</param>
        /// <param name="ms">MemoryStream mit dem Lieferschein-PDF.</param>
        /// <param name="fromAddress">Absenderemailadresse.</param>
        /// <param name="smtpServer">SMTP-Server für den Emailversand.</param>
        /// <returns>True, wenn eine Email versendet wurde, sonst false.</returns>
        void SendByEmail(DocketList docketList, MemoryStream ms, string fromAddress, string smtpServer);
    }
}
