using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IInvoiceItemAccountItemManager : IEntityManager<InvoiceItemAccountItem, int>
    {
        /// <summary>
        /// C
        /// </summary>
        /// <param name="invoice"></param>
        void CreateAccounts(Invoice invoice);

        /// <summary>
        /// Erstelle zu der jeweiligen Rechnung ein neues Erlöskonto
        /// </summary>
        /// <param name="invoice">Rechnungsobjekt</param>
        void CreateAccountsForSmallCustomers(Invoice invoice, string accountNumber);

        /// <summary>
        /// Gibt Erloeskonten als Liste zurück
        /// </summary>
        /// <param name="invoiceId">Rechnungspositionsid</param>
        /// <param name="isPrinted">Ist Gedruckt</param>
        /// <returns>IQueryable<_Accounts></returns>
        IEnumerable<_Accounts> GetAccountNumbers(int invoiceId, bool isPrinted = false);
    }
}
