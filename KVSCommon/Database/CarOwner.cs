using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse zur DB CarOwner
    /// </summary>
    public partial class CarOwner : ILogging
    {
        public DataClasses1DataContext LogDBContext
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
        /// Gibt den vollständigen Namen zurück (Fahrzeughalter)
        /// </summary>
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.FirstName))
                {
                    return this.FirstName + "  " + this.Name;
                }

                return this.Name;
            }
        }
        /// <summary>
        /// Erstelle einen neuen Fahrzeughalter
        /// </summary>
        /// <param name="name">Nachname</param>
        /// <param name="firstName">Vorname</param>
        /// <param name="bankAccountId">ID der Bankverbindung</param>
        /// <param name="contactId">ID des Kontaktdatensatzes</param>
        /// <param name="adressId">ID der Adresse</param>
        /// <param name="dbContext">DB Kontext</param>
        /// <returns>CarOwner</returns>
        public static CarOwner CreateCarOwner(string name, string firstName, Guid? bankAccountId, Guid? contactId, Guid? adressId, DataClasses1DataContext dbContext)
        {
            if (string.IsNullOrEmpty(name))
            {
                ////throw new Exception("Der Name des Halters darf nicht leer sein.");
            }

            CarOwner owner = new CarOwner()
            {
                Id = Guid.NewGuid(),
                Name = name,
                FirstName = firstName,
                BankAccountId = bankAccountId,
                ContactId = contactId,
                AdressId = adressId
            };

            dbContext.WriteLogItem("Halter " + name + " wurde angelegt.", LogTypes.INSERT, owner.Id, "CarOwner");
            dbContext.CarOwner.InsertOnSubmit(owner);
            return owner;
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
        partial void OnNameChanging(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                ////throw new Exception("Der Name des Halters darf nicht leer sein.");
            }

            this.WriteUpdateLogItem("Name", this.Name, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnFirstNameChanging(string value)
        {
            this.WriteUpdateLogItem("Vorname", this.FirstName, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnAdressIdChanging(Guid? value)
        {
            this.WriteUpdateLogItem("Adresse", this.AdressId, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnBankAccountIdChanging(Guid? value)
        {
            this.WriteUpdateLogItem("Bankverbindung", this.BankAccountId, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnContactIdChanging(Guid? value)
        {
            this.WriteUpdateLogItem("Kontaktdaten", this.ContactId, value);
        }
    }
}
