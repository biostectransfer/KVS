using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse für die Erlöskonten
    /// </summary>
    public partial class Accounts : ILogging
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
                return this.AccountId;
            }
        }

        public EntityState EntityState
        {
            get;
            set;
        }
        /// <summary>
        /// Erstelle ein neues Erloeskonte
        /// </summary>
        /// <param name="CustomerId">Kundenid</param>
        /// <param name="AccountNumber">Kontonummer</param>
        /// <param name="dbContext">Datenbank Kontext</param>
        /// <returns>Erloeskonto</returns>
       public static Accounts CreateAccount(int? CustomerId, string AccountNumber, KVSEntities dbContext)
       {
           if (dbContext.Accounts.Any(q => q.CustomerId == CustomerId && q.AccountNumber == AccountNumber))
           {
               throw new Exception("Für diesen Kunden ist bereits dieses Konto eingetragen");
           }
           var account = new Accounts
           {
               AccountNumber = AccountNumber,
               CustomerId = CustomerId
           };
           dbContext.Accounts.InsertOnSubmit(account);
           dbContext.SubmitChanges();
           return account;
         
       }
        /// <summary>
        /// Lösche ein bestehendes Erlöskonto
        /// </summary>
       /// <param name="CustomerId">Kundenid</param>
       /// <param name="AccountNumber">Kontonummer</param>
       /// <param name="dbContext">Datenbank Kontext</param>
       public static void DeleteAccount(int CustomerId, string AccountNumber, KVSEntities dbContext)
       {
           var myAcount = dbContext.Accounts.FirstOrDefault(q => q.CustomerId == CustomerId && q.AccountNumber == AccountNumber);
           if (myAcount == null)
           {
               throw new Exception("Dieses Buchungskonto wurde im System nicht gefunden.");
           }
           else
           {
               dbContext.Accounts.DeleteOnSubmit(myAcount);
               dbContext.SubmitChanges();
           }
       }
       /// <summary>
       /// Gibt Erloeskonten als Liste zurück
       /// </summary>
       /// <param name="dbContext">Datenbank Kontext</param>
       /// <param name="itemId">Rechnungspositionsid</param>
       /// <param name="isPrinted">Ist Gedruckt</param>
       /// <returns>IQueryable<_Accounts></returns>
        public static IQueryable<_Accounts> generateAccountNumber(KVSEntities dbContext, int itemId, bool isPrinted = false)
       {
        
           IQueryable<_Accounts> _accounts = null;
           if (isPrinted)
           {
               _accounts = from invItem in dbContext.InvoiceItem
                           join iai in dbContext.InvoiceItemAccountItem on invItem.Id equals iai.InvoiceItemId
                           where invItem.InvoiceId == itemId
                           select new _Accounts
                           {
                               InvoiceItemId = invItem.Id,
                               AccountId = iai.IIACCID,
                               AccountNumber = iai.RevenueAccountText
                           };


           }
           else
           {
               if (!isPrinted)
               {
                   _accounts = from inv in dbContext.Invoice
                               join invItem in dbContext.InvoiceItem on inv.Id equals invItem.InvoiceId
                               join ordItem in dbContext.OrderItem on invItem.OrderItemId equals ordItem.Id
                               join order in dbContext.Order on ordItem.OrderNumber equals order.OrderNumber
                               join pr in dbContext.Price on new { ordItem.ProductId, order.LocationId } equals new { pr.ProductId, pr.LocationId }
                               join priceAccount in dbContext.PriceAccount on pr.Id equals priceAccount.PriceId
                               join Account in dbContext.Accounts on priceAccount.AccountId equals Account.AccountId
                               where invItem.InvoiceId == itemId
                               select new _Accounts
                               {
                                   InvoiceItemId = invItem.Id,
                                   AccountId = Account.AccountId,
                                   AccountNumber = Account.AccountNumber

                               };
               }
               if (_accounts.Count() == 0)
               {
                   _accounts = from inv in dbContext.Invoice
                               join invItem in dbContext.InvoiceItem on inv.Id equals invItem.InvoiceId
                               join ordItem in dbContext.OrderItem on invItem.OrderItemId equals ordItem.Id
                               join order in dbContext.Order on ordItem.OrderNumber equals order.OrderNumber
                               join pr in dbContext.Price on new { ordItem.ProductId } equals new { pr.ProductId }
                               join priceAccount in dbContext.PriceAccount on pr.Id equals priceAccount.PriceId
                               join Account in dbContext.Accounts on priceAccount.AccountId equals Account.AccountId
                               where invItem.InvoiceId == itemId && pr.LocationId == null
                               select new _Accounts
                               {
                                   InvoiceItemId = invItem.Id,
                                   AccountId = Account.AccountId,
                                   AccountNumber = Account.AccountNumber,

                               };

               }
           }
           return _accounts;
       }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
       partial void OnAccountIdChanging(int value)
        {
            this.WriteUpdateLogItem("AccountId", this.AccountId, value);
        }
       /// <summary>
       /// Aenderungsevents für die Historie
       /// </summary>
       /// <param name="value"></param>
       partial void OnCustomerIdChanging(int? value)
       {
           this.WriteUpdateLogItem("CustomerId", this.CustomerId, value);
       }
       /// <summary>
       /// Aenderungsevents für die Historie
       /// </summary>
       /// <param name="value"></param>
       partial void OnAccountNumberChanging(string value)
       {
           this.WriteUpdateLogItem("AccountNumber", this.AccountNumber, value);
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
