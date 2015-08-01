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
    public partial class CarOwnerManager : EntityManager<CarOwner, int>, ICarOwnerManager
    {
        public CarOwnerManager(IKVSEntities context) : base(context) { }

        /// <summary>
        /// Erstelle einen neuen Fahrzeughalter
        /// </summary>
        /// <param name="name">Nachname</param>
        /// <param name="firstName">Vorname</param>
        /// <param name="bankAccountId">ID der Bankverbindung</param>
        /// <param name="contactId">ID des Kontaktdatensatzes</param>
        /// <param name="adressId">ID der Adresse</param>
        /// <returns>CarOwner</returns>
        public CarOwner CreateCarOwner(string name, string firstName, BankAccount bankAccount, Contact contact, Adress adress)
        {
            if (string.IsNullOrEmpty(name))
            {
                ////throw new Exception("Der Name des Halters darf nicht leer sein.");
            }

            var owner = new CarOwner()
            {
                Name = name,
                FirstName = firstName,
                BankAccount = bankAccount,
                Contact = contact,
                Adress = adress
            };

            DataContext.AddObject(owner);
            SaveChanges();
            DataContext.WriteLogItem("Halter " + name + " wurde angelegt.", LogTypes.INSERT, owner.Id, "CarOwner");
            return owner;
        }
    }
}
