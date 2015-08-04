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
