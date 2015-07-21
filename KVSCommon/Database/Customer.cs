using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse für die Kundentabelle
    /// </summary>
    public partial class Customer : ILogging
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
        private DataClasses1DataContext myDbContext = null;
        public DataClasses1DataContext _dbContext
        {
            get { return myDbContext; }
            set { myDbContext = value; }
        } 
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
        internal static Customer CreateCustomer(string name, string street, string streetnumber, string zipcode, string city, string country, string phone, string fax, string mobilephone, string email, decimal vat, int? termOfCredit, string customerNumber, string Matchcode="", string Debitornumber="", string eVB_Number="")
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
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnNameChanging(string value)
        {
            this.WriteUpdateLogItem("Name", this.Name, value);
        }
        /// <summary>
        /// Validierungs Methode um falsche Eingaben zu verhindern
        /// </summary>
        /// <param name="action"></param>
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            
            if (action == System.Data.Linq.ChangeAction.Insert || action == System.Data.Linq.ChangeAction.Update)
            {
                if (this.myDbContext == null)
                {
                    using (DataClasses1DataContext dbContext = new DataClasses1DataContext())
                    {

                        if (dbContext.Customer.Count(q => q.Name == this.Name && q.Id != this.Id && q.CustomerNumber == this.CustomerNumber) > 0)
                        {
                            throw new Exception("Es existiert bereits ein Kunde mit Namen " + this.Name + " und der CustomerNumber:" + this.CustomerNumber);
                        }

                        if (dbContext.Customer.Count(q => q.CustomerNumber == this.CustomerNumber && q.Id != this.Id) > 0)
                        {
                            throw new Exception("Es existiert bereits ein Kunde mit der Kundennummer " + this.CustomerNumber + ".");
                        }
                    }
                }
                else
                {
                    if (myDbContext.Customer.Count(q => q.Name == this.Name && q.Id != this.Id && q.CustomerNumber == this.CustomerNumber) > 0)
                    {
                        throw new Exception("Es existiert bereits ein Kunde mit Namen " + this.Name + " und der CustomerNumber:" + this.CustomerNumber);
                    }

                    if (myDbContext.Customer.Count(q => q.CustomerNumber == this.CustomerNumber && q.Id != this.Id) > 0)
                    {
                        throw new Exception("Es existiert bereits ein Kunde mit der Kundennummer " + this.CustomerNumber + ".");
                    }
                }
            }
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnTermOfCreditChanging(int? value)
        {
            this.WriteUpdateLogItem("Zahlungsziel", this.TermOfCredit, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnVATChanging(decimal value)
        {
            this.WriteUpdateLogItem("Mehrwertsteuersatz", this.VAT, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCustomerNumberChanging(string value)
        {
            this.WriteUpdateLogItem("Kundennummer", this.CustomerNumber, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnDebitornumberChanging(string value)
        {
            this.WriteUpdateLogItem("Debitornumber", this.Debitornumber, value);
        }
        partial void OnMatchCodeChanging(string value)
        {
            this.WriteUpdateLogItem("MatchCode", this.MatchCode, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OneVB_NumberChanging(string value)
        {
            this.WriteUpdateLogItem("eVB_Number", this.eVB_Number, value);
        }
    }
}
