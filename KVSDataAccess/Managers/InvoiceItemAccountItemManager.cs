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
        public IQueryable<_Accounts> GetAccountNumbers(int invoiceId, bool isPrinted = false)
        {
            IQueryable<_Accounts> result = null;
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
                    var pairs = DataContext.GetSet<Invoice>().FirstOrDefault(o => o.Id == invoiceId).InvoiceItem.Select(o =>
                        new
                        {
                            ProductId = o.OrderItem.ProductId,
                            LocationId = o.OrderItem.Order.LocationId
                        }).ToList();


                    //TODO TODO Make VIEWs


                    //DataContext.GetSet<Product>().Where(prod => pairs.Any(pair => pair.ProductId == prod.Id)).
                    //    Select(o => new {
                    //        Product = o,
                    //        Prices = o.Price.Where(price => pairs.Any(pair => pair.LocationId == price.LocationId))
                    //    }).
                    //    SelectMany(o => o.Prices).
                    //    Select(o => new _Accounts
                    //    {
                    //        InvoiceItemId = o.PriceAccount.FirstOrDefault()..InvoiceItemId,
                    //        AccountId = o.IIACCID,
                    //        AccountNumber = o.Product.RevenueAccountText
                    //    });

                    //result = from inv in dbContext.Invoice
                    //            join invItem in dbContext.InvoiceItem on inv.Id equals invItem.InvoiceId
                    //            join ordItem in dbContext.OrderItem on invItem.OrderItemId equals ordItem.Id
                    //            join order in dbContext.Order on ordItem.OrderNumber equals order.OrderNumber
                    //            join pr in dbContext.Price on new { ordItem.ProductId, order.LocationId } equals new { pr.ProductId, pr.LocationId }
                    //            join priceAccount in dbContext.PriceAccount on pr.Id equals priceAccount.PriceId
                    //            join Account in dbContext.Accounts on priceAccount.AccountId equals Account.AccountId
                    //            where invItem.InvoiceId == itemId
                    //            select new _Accounts
                    //            {
                    //                InvoiceItemId = invItem.Id,
                    //                AccountId = Account.AccountId,
                    //                AccountNumber = Account.AccountNumber

                    //            };
                }

                if (result.Count() == 0)
                {
                    //TODO TODO

                    //result = from inv in dbContext.Invoice
                    //            join invItem in dbContext.InvoiceItem on inv.Id equals invItem.InvoiceId
                    //            join ordItem in dbContext.OrderItem on invItem.OrderItemId equals ordItem.Id
                    //            join order in dbContext.Order on ordItem.OrderNumber equals order.OrderNumber
                    //            join pr in dbContext.Price on new { ordItem.ProductId } equals new { pr.ProductId }
                    //            join priceAccount in dbContext.PriceAccount on pr.Id equals priceAccount.PriceId
                    //            join Account in dbContext.Accounts on priceAccount.AccountId equals Account.AccountId
                    //            where invItem.InvoiceId == itemId && pr.LocationId == null
                    //            select new _Accounts
                    //            {
                    //                InvoiceItemId = invItem.Id,
                    //                AccountId = Account.AccountId,
                    //                AccountNumber = Account.AccountNumber,

                    //            };
                }
            }
            return result;
        }
    }
}
