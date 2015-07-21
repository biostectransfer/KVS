using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse fuer die DB Tabelle SmallCustomer
    /// </summary>
    public partial class SmallCustomer : ILogging
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
                return this.CustomerId;
            }
        }

        public EntityState EntityState
        {
            get;
            set;
        }
      /// <summary>
      /// Loescht einen Laufkunden, aber nur wenn der Kunde mit keinen Auftraegen und Rechnungen verknuepft ist
      /// </summary>
      /// <param name="dbContext">DB Kontext</param>
      /// <param name="smallCustomerId">Laufkunden ID</param>
        public static void RemoveSmallCutomer(DataClasses1DataContext dbContext, int smallCustomerId)
        {
            var smCustomer = dbContext.SmallCustomer.FirstOrDefault(q => q.CustomerId == smallCustomerId);
            if (smCustomer == null)
                throw new Exception("Der Kunde wurde in der Datenbank nicht gefunden");
            if (smCustomer.Customer.Order != null && smCustomer.Customer.Order.Count >0 )
                throw new Exception("Das löschen ist nicht möglich, da der Kunde bereits mit Aufträgen verknüpft ist");
            if (smCustomer.Customer.Invoice != null && smCustomer.Customer.Invoice.Count > 0)
                throw new Exception("Das löschen ist nicht möglich, da der Kunde bereits mit Rechnungen verknüpft ist");
            if (smCustomer.Customer.CustomerProduct != null && smCustomer.Customer.CustomerProduct.Count > 0)
                throw new Exception("Das löschen ist nicht möglich, da der Kunde bereits mit Produkten verknüpft ist");

            if(smCustomer.Customer.ContactId!=null)
            Contact.DeleteContact(smCustomer.Customer.ContactId.Value, dbContext);

            Adress.DeleteAdress(smCustomer.Customer.AdressId, dbContext);
            Adress.DeleteAdress(smCustomer.Customer.InvoiceAdressId, dbContext);
            Adress.DeleteAdress(smCustomer.Customer.InvoiceDispatchAdressId, dbContext);
            Person.DeletePerson(smCustomer.PersonId, dbContext);

            if (smCustomer.BankAccountId != null)
                BankAccount.DeleteBank(smCustomer.BankAccountId.Value, dbContext);

            dbContext.SmallCustomer.DeleteOnSubmit(smCustomer);
            dbContext.Customer.DeleteOnSubmit(smCustomer.Customer);

            dbContext.WriteLogItem("Kunde " + smCustomer.CustomerId + " mit dem Namen: "+smCustomer.Customer.Name + " wurde gelöscht.", LogTypes.DELETE, smCustomer.CustomerId, "SmallCustomer");


        }
        /// <summary>
        /// Erstellt einen Laufkunden.
        /// </summary>
        /// <param name="name">Name des Kunden.</param>
        /// <param name="firstName">Vorname des Kunden.</param>
        /// <param name="title">Anrede.</param>
        /// <param name="gender">Geschlecht.</param>
        /// <param name="street">Straße.</param>
        /// <param name="streetnumber">Hausnummer.</param>
        /// <param name="zipcode">Postleitzahl.</param>
        /// <param name="city">Straße.</param>
        /// <param name="country">Land.</param>
        /// <param name="phone">Telefonnummer.</param>
        /// <param name="fax">Faxnummer.</param>
        /// <param name="mobilephone">Mobiltelefonnummer.</param>
        /// <param name="email">Emailaddresse.</param>
        /// <param name="vat">Mehrwersteuersatz für den Kunden.</param>
        /// <param name="termOfCredit">Zahlungsziel (in Tagen).</param>
        /// <param name="customerNumber">Kundennummer.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Den neuen Laufkunden.</returns>
        public static SmallCustomer CreateSmallCustomer(string name, string firstName, string title, string gender, string street, string streetnumber, string zipcode, string city, 
            string country, string phone, string fax, string mobilephone, string email, decimal vat, int? termOfCredit, string customerNumber, DataClasses1DataContext dbContext)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Der Name darf nicht leer sein.");
            }

            var customer = Customer.CreateCustomer(name, street, streetnumber, zipcode, city, country, phone, fax, mobilephone, email, vat, termOfCredit, customerNumber);
            customer._dbContext = dbContext;
            var person = new Person()
            {
                FirstName = firstName,
                Name = name,
                Gender = gender,
                Title = title
            };

            var smallCustomer = new SmallCustomer()
            {
                Customer = customer,
                Person = person
            };

            dbContext.WriteLogItem("Kunde " + firstName + " " + name + " wurde angelegt.", LogTypes.INSERT, customer.Id, "SmallCustomer");
            dbContext.SmallCustomer.InsertOnSubmit(smallCustomer);
            return smallCustomer;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        partial void OnLoaded()
        {
            this.EntityState = Database.EntityState.Loaded;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        partial void OnCreated()
        {
            this.EntityState = Database.EntityState.New;
        }
    }
}
