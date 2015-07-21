using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.Web.UI.WebControls;
namespace KVSWebApplication.Product
{
   /// <summary>
   /// Hilfklasse fuer die Erloeskonten
   /// </summary>
   public class PriceAccountHelper
    {
        public static void CreateAccount(int? AccountNumber, Price _object, DataClasses1DataContext dbContext, bool withLocation = false)
       {
           if (AccountNumber != null)
           {
               IQueryable<PriceAccount> myAccount = null;
               if (withLocation == false)
               {
                   myAccount = dbContext.PriceAccount.Where(q => q.PriceId == _object.Id && q.Price.LocationId == null);
               }
               else
               {
                   myAccount = dbContext.PriceAccount.Where(q => q.PriceId == _object.Id && q.Price.LocationId == _object.LocationId);
               }
               if (myAccount.Count() > 0)
               {
                   dbContext.PriceAccount.DeleteAllOnSubmit<PriceAccount>(myAccount);
                   dbContext.SubmitChanges();
               }
               if (myAccount.Count() == 0)
               {
                   var myNewAccount = new PriceAccount
                   {
                       PriceId = _object.Id,
                       AccountId = AccountNumber.Value
                   };
                   dbContext.PriceAccount.InsertOnSubmit(myNewAccount);
                   dbContext.SubmitChanges();
               }
           }           
       }
    }
}
