using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{

    /// <summary>
    /// Erweiterungsklasse zur DB Adress
    /// </summary>
    public partial class Adress : ILogging
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
        /// Erstellt eine neue Adresse.
        /// </summary>
        /// <param name="street">Straße.</param>
        /// <param name="streetNumber">Hausnummer.</param>
        /// <param name="zipcode">Postleitzahl.</param>
        /// <param name="city">Ort.</param>
        /// <param name="country">Land.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Die neue Adresse.</returns>
        public static Adress CreateAdress(string street, string streetNumber, string zipcode, string city, string country, DataClasses1DataContext dbContext)
        {
            Adress adress = new Adress()
            {
                Id = Guid.NewGuid(),
                Street = street,
                StreetNumber = streetNumber,
                Zipcode = zipcode,
                City = city,
                Country = country
            };

            dbContext.WriteLogItem("Adresse angelegt.", LogTypes.INSERT, adress.Id, "Adress");
            dbContext.Adress.InsertOnSubmit(adress);
            return adress;
        }

        /// <summary>
        /// Löscht die angegebene Adresse aus der Datenbank.
        /// </summary>
        /// <param name="adressId">Id der Adresse.</param>
        /// <param name="dbContext"></param>
        public static void DeleteAdress(Guid adressId, DataClasses1DataContext dbContext)
        {
            Adress adress = dbContext.Adress.SingleOrDefault(q => q.Id == adressId);
            if (adress != null)
            {
                dbContext.WriteLogItem("Adresse gelöscht.", LogTypes.DELETE, adressId, "Adress");
                dbContext.Adress.DeleteOnSubmit(adress);
            }
        }
        /// <summary>
        /// Erstelle eine Kopie der Adress
        /// </summary>
        /// <param name="adress">Zu kopierende Adress</param>
        /// <param name="dbContext">Datenbank Kontext</param>
        /// <returns>Adress</returns>
        public static Adress CreateCopy(Adress adress, DataClasses1DataContext dbContext)
        {
            Adress copy = new Adress()
            {
                Id = Guid.NewGuid(),
                Street = adress.Street,
                StreetNumber = adress.StreetNumber,
                Zipcode = adress.Zipcode,
                City = adress.City,
                Country = adress.Country
            };

            dbContext.WriteLogItem("Adresse kopiert.", LogTypes.INSERT, copy.Id, "Adress", adress.Id);
            dbContext.Adress.InsertOnSubmit(copy);
            return copy;
        }

        /// <summary>
        /// Vergleiche zwei Adressen zu einander
        /// </summary>
        /// <param name="toCompare">Zu vergleichende Adresse</param>
        /// <returns>bool</returns>
        public bool ContentEquals(Adress toCompare)
        {
            return this.Street == toCompare.Street
                && this.StreetNumber == toCompare.StreetNumber
                && this.Zipcode == toCompare.Zipcode
                && this.City == toCompare.City
                && this.Country == toCompare.Country;
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
        partial void OnCityChanging(string value)
        {
            this.WriteUpdateLogItem("Ort", this.City, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCountryChanging(string value)
        {
            this.WriteUpdateLogItem("Land", this.Country, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnStreetChanging(string value)
        {
            this.WriteUpdateLogItem("Straße", this.Street, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnStreetNumberChanging(string value)
        {
            this.WriteUpdateLogItem("Hausnummer", this.StreetNumber, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnZipcodeChanging(string value)
        {
            this.WriteUpdateLogItem("Postleitzahl", this.Zipcode, value);
        }
    }
}
