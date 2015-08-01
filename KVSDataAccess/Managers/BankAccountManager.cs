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
    public partial class BankAccountManager : EntityManager<BankAccount, int>, IBankAccountManager
    {
        public BankAccountManager(IKVSEntities context) : base(context) { }

        /// <summary>
        /// Erstellt eine neu Bankverbindung.
        /// </summary>
        /// <param name="name">Name der Bank.</param>
        /// <param name="accountnumber">Kontonummer der Bankverbindung.</param>
        /// <param name="bankcode">Bankleitzahl der Bank.</param>
        /// <param name="IBANNumber">Aktuelle IBAN Nummer</param>
        /// <returns>Die neue Bankverbindung.</returns>
        public BankAccount CreateBankAccount(string name, string accountnumber, string bankcode, string IBANNumber, string BICNumber)
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

            DataContext.AddObject(bankaccount);
            SaveChanges();
            DataContext.WriteLogItem("Bankverbindung angelegt.", LogTypes.INSERT, bankaccount.Id, "BankAccount");
            return bankaccount;
        }
    }
}
