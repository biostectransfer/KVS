using KVSCommon.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    public partial class BankAccount : ILogging, IHasId<int>, IRemovable, ISystemFields
    {
        /// <summary>
        /// Erweiterungsklasse zur DB BankAccount
        /// </summary>
        public KVSEntities LogDBContext
        {
            get;
            set;
        }

        public object ItemId
        {
            get
            {
                return this.Id;
            }
        }

        public EntityState EntityState
        {
            get;
            set;
        }
        /// <summary>
        /// Löscht die angegebene Bank aus der Datenbank.
        /// </summary>
        /// <param name="adressId">Id der Bank.</param>
        /// <param name="dbContext"></param>
        public static void DeleteBank(int bankId, KVSEntities dbContext)
        {
            BankAccount bank = dbContext.BankAccount.SingleOrDefault(q => q.Id == bankId);
            if (bank != null)
            {
                dbContext.WriteLogItem("bank gelöscht.", LogTypes.DELETE, bankId, "BankAccount");
                dbContext.BankAccount.DeleteOnSubmit(bank);
            }
        }
        /// <summary>
        /// Erstellt eine neu Bankverbindung.
        /// </summary>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <param name="name">Name der Bank.</param>
        /// <param name="accountnumber">Kontonummer der Bankverbindung.</param>
        /// <param name="bankcode">Bankleitzahl der Bank.</param>
        /// <param name="IBANNumber">Aktuelle IBAN Nummer</param>
        /// <returns>Die neue Bankverbindung.</returns>
        public static BankAccount CreateBankAccount(KVSEntities dbContext, string name, string accountnumber, string bankcode, string IBANNumber, string BICNumber)
        {
            if (string.IsNullOrEmpty(name))
            {
                ////throw new Exception("Der Name der Bank darf nicht leer sein.");
            }

            var bankaccount = new BankAccount()
            {
                BankName = name,
                BankCode = bankcode,
                Accountnumber = accountnumber,
                IBAN = IBANNumber,
                BIC = BICNumber
            };

            dbContext.BankAccount.InsertOnSubmit(bankaccount);
            dbContext.SubmitChanges();
            dbContext.WriteLogItem("Bankverbindung angelegt.", LogTypes.INSERT, bankaccount.Id, "BankAccount");
            return bankaccount;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnLoaded()
        {
            this.EntityState = Database.EntityState.Loaded;
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
        partial void OnAccountnumberChanging(string value)
        {
            this.WriteUpdateLogItem("Kontonummer", this.Accountnumber, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnBankCodeChanging(string value)
        {
            this.WriteUpdateLogItem("Bankleitzahl", this.BankCode, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnBankNameChanging(string value)
        {
            this.WriteUpdateLogItem("Name", this.BankName, value);
        }
    }
}
