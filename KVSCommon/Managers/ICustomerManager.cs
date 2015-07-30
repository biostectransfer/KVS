using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface ICustomerManager : IEntityManager<Customer, int>
    {
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
        Customer CreateCustomer(string name, string street, string streetnumber, string zipcode, string city, string country, string phone,
            string fax, string mobilephone, string email, decimal vat, int? termOfCredit, string customerNumber, string Matchcode = "", string Debitornumber = "", string eVB_Number = "");

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
        /// <returns>Den neuen Laufkunden.</returns>
        SmallCustomer CreateSmallCustomer(string name, string firstName, string title, string gender, string street, string streetnumber, string zipcode, string city,
            string country, string phone, string fax, string mobilephone, string email, decimal vat, int? termOfCredit, string customerNumber);
    }
}
