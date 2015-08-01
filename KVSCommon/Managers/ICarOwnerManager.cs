using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface ICarOwnerManager : IEntityManager<CarOwner, int>
    {
        /// <summary>
        /// Erstelle einen neuen Fahrzeughalter
        /// </summary>
        /// <param name="name">Nachname</param>
        /// <param name="firstName">Vorname</param>
        /// <param name="bankAccountId">ID der Bankverbindung</param>
        /// <param name="contactId">ID des Kontaktdatensatzes</param>
        /// <param name="adressId">ID der Adresse</param>
        /// <returns>CarOwner</returns>
        CarOwner CreateCarOwner(string name, string firstName, BankAccount bankAccount, Contact contact, Adress adress);
    }
}
