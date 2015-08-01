using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IBankAccountManager : IEntityManager<BankAccount, int>
    {
        /// <summary>
        /// Erstellt eine neu Bankverbindung.
        /// </summary>
        /// <param name="name">Name der Bank.</param>
        /// <param name="accountnumber">Kontonummer der Bankverbindung.</param>
        /// <param name="bankcode">Bankleitzahl der Bank.</param>
        /// <param name="IBANNumber">Aktuelle IBAN Nummer</param>
        /// <returns>Die neue Bankverbindung.</returns>
        BankAccount CreateBankAccount(string name, string accountnumber, string bankcode, string IBANNumber, string BICNumber);
    }
}
