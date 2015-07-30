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
    public partial class CustomerManager : EntityManager<Customer, int>, ICustomerManager
    {
        public CustomerManager(IKVSEntities context) : base(context) { }

        /// <summary>
        /// Erstellt einen neuen Kunden.
        /// </summary>
        /// <param name="name">Name des Kunden.</param>
        /// <param name="street">Straße.</param>
        /// <param name="streetnumber">Hausnummer.</param>
        /// <param name="zipcode">Postleitzahl.</param>
        /// <param name="city">Ort.</param>
        /// <param name="country">Land.</param>
        /// <param name="phone">Telefonnummer.</param>
        /// <param name="fax">Faxnummer.</param>
        /// <param name="mobilephone">Mobiltelefonnummer.</param>
        /// <param name="email">E-Mailadresse.</param>
        /// <param name="vat">Mehrwersteuersatz.</param>
        /// <param name="termOfCredit">Zahlungsziel (in Tagen).</param>
        /// <param name="customerNumber">Kundennummer.</param>
        /// <returns>Den Kunden.</returns>
        /// <remarks>Methode soll nur für interne Zwecke benutzt werden. Die public Create-Methoden befinden sich in den spezialisierten Kundenklassen.</remarks>
        public Customer CreateCustomer(string name, string street, string streetnumber, string zipcode, string city, string country, string phone, string fax, 
            string mobilephone, string email, decimal vat, int? termOfCredit, string customerNumber, string Matchcode = "", string Debitornumber = "", string eVB_Number = "")
        {
            if (string.IsNullOrEmpty(customerNumber))
            {
                throw new ArgumentNullException("Die Kundennummer darf nicht leer sein.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Der Kundenname darf nicht leer sein.");
            }

            var adress = new Adress()
            {
                Street = street,
                StreetNumber = streetnumber,
                City = city,
                Country = country,
                Zipcode = zipcode
            };

            var contact = new Contact()
            {
                Phone = phone,
                Fax = fax,
                MobilePhone = mobilephone,
                Email = email
            };

            var customer = new Customer()
            {
                Name = name,
                Adress = adress,
                Contact = contact,
                VAT = vat,
                InvoiceAdress = adress,
                InvoiceDispatchAdress = adress,
                TermOfCredit = termOfCredit,
                CustomerNumber = customerNumber,
                MatchCode = Matchcode,
                Debitornumber = Debitornumber,
                eVB_Number = eVB_Number
            };

            return customer;
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
        public SmallCustomer CreateSmallCustomer(string name, string firstName, string title, string gender, string street, string streetnumber, string zipcode, string city,
            string country, string phone, string fax, string mobilephone, string email, decimal vat, int? termOfCredit, string customerNumber)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Der Name darf nicht leer sein.");
            }

            var customer = CreateCustomer(name, street, streetnumber, zipcode, city, country, phone, fax, mobilephone, email, vat, termOfCredit, customerNumber);

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

            DataContext.AddObject(smallCustomer);
            SaveChanges();
            DataContext.WriteLogItem("Kunde " + firstName + " " + name + " wurde angelegt.", LogTypes.INSERT, customer.Id, "SmallCustomer");

            return smallCustomer;
        }
    }
}
