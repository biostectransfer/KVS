using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    public class InvoiceHelper
    {
        /// <summary>
        /// Erstelle zu der jeweiligen Rechnung ein neues Erlöskonto
        /// </summary>
        /// <param name="dbContext">DB Kontext</param>
        /// <param name="inv">Rechnungsobjekt</param>
        public static void CreateAccounts(KVSEntities dbContext, Invoice inv)
        {
            List<_Accounts> acc = null;
             acc = Accounts.generateAccountNumber(dbContext,inv.Id).ToList();

             if (acc != null && acc.Count() == inv.InvoiceItem.Count)
             {
                 foreach (var thisItems in acc)
                 {
                     var myAccount = new InvoiceItemAccountItem
                     {
                         InvoiceItemId = thisItems.InvoiceItemId,
                         RevenueAccountText = thisItems.AccountNumber
                     };
                     var contains = dbContext.InvoiceItemAccountItem.FirstOrDefault(q => q.InvoiceItemId ==
                         thisItems.InvoiceItemId && q.RevenueAccountText == thisItems.AccountNumber.Trim());
                     if (contains != null)
                     {
                         contains.RevenueAccountText = thisItems.AccountNumber.Trim();
                     }
                     else
                     {
                         dbContext.InvoiceItemAccountItem.InsertOnSubmit(myAccount);
                     }
                     dbContext.SubmitChanges();
                 }
             }
             else
             {
                 throw new Exception("Die Rechnung konnte nicht gedruckt werden, da nicht alle Dienstleistungen ein Erlöskonto haben! Sie können die Erlöskonten im Reiter 'Rechnung erstellen' zuweisen und die Rechnung erneut drucken.");
             }
        }
    }
}
