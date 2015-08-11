using KVSCommon.Database;
using KVSCommon.Entities;
using KVSCommon.Managers;
using KVSDataAccess.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSDataAccess.Managers
{
    public partial class InvoiceItemAccountItemManager : EntityManager<InvoiceItemAccountItem, int>, IInvoiceItemAccountItemManager
    {
        public InvoiceItemAccountItemManager(IKVSEntities context) : base(context) { }

        /// <summary>
        /// Erstelle zu der jeweiligen Rechnung ein neues Erlöskonto
        /// </summary>
        /// <param name="invoice">Rechnungsobjekt</param>
        public void CreateAccounts(Invoice invoice)
        {
            var accountNumbers = GetAccountNumbers(invoice.Id).ToList();

            if (accountNumbers != null && accountNumbers.Count() == invoice.InvoiceItem.Count)
            {
                foreach (var item in accountNumbers)
                {
                    var myAccount = new InvoiceItemAccountItem
                    {
                        InvoiceItemId = item.InvoiceItemId,
                        RevenueAccountText = item.AccountNumber
                    };
                    var accountInDb = GetEntities(q => q.InvoiceItemId ==
                        item.InvoiceItemId && q.RevenueAccountText == item.AccountNumber.Trim()).FirstOrDefault();

                    if (accountInDb != null)
                    {
                        accountInDb.RevenueAccountText = item.AccountNumber.Trim();
                    }
                    else
                    {
                        DataContext.AddObject(accountInDb);
                    }
                    DataContext.SaveChanges();
                }
            }
            else
            {
                throw new Exception("Die Rechnung konnte nicht gedruckt werden, da nicht alle Dienstleistungen ein Erlöskonto haben! Sie können die Erlöskonten im Reiter 'Rechnung erstellen' zuweisen und die Rechnung erneut drucken.");
            }
        }

        /// <summary>
        /// Gibt Erloeskonten als Liste zurück
        /// </summary>
        /// <param name="invoiceId">Rechnungspositionsid</param>
        /// <param name="isPrinted">Ist Gedruckt</param>
        /// <returns>IQueryable<_Accounts></returns>
        public IEnumerable<_Accounts> GetAccountNumbers(int invoiceId, bool isPrinted = false)
        {
            IEnumerable<_Accounts> result = null;
            if (isPrinted)
            {
                result = DataContext.GetSet<InvoiceItemAccountItem>().Where(o => o.InvoiceItem.InvoiceId == invoiceId).Select(o => new _Accounts
                            {
                                InvoiceItemId = o.InvoiceItemId,
                                AccountId = o.Id,
                                AccountNumber = o.RevenueAccountText
                            });
            }
            else
            {
                if (!isPrinted)
                {
                    result = DataContext.GetSet<GetAccountNumber>().Where(o => o.InvoiceId == invoiceId).
                      Select(o => new _Accounts
                      {
                          InvoiceItemId = o.Id,
                          AccountId = o.AccountId,
                          AccountNumber = o.AccountNumber
                      }).ToList();
                }

                if (result.Count() == 0)
                {
                    result = DataContext.GetSet<GetAccountNumber>().Where(o => o.InvoiceId == invoiceId && !o.LocationId.HasValue).
                      Select(o => new _Accounts
                      {
                          InvoiceItemId = o.Id,
                          AccountId = o.AccountId,
                          AccountNumber = o.AccountNumber
                      }).ToList();
                }
            }
            return result;
        }
    }
}
