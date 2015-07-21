using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse für die Kontaktdaten
    /// </summary>
    public partial class Contact : ILogging
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
        /// Erstelle einene neuen Kontakt 
        /// </summary>
        /// <param name="phone">Telefon</param>
        /// <param name="fax">Fax</param>
        /// <param name="mobilePhone">Mobil</param>
        /// <param name="email">Mail</param>
        /// <param name="dbContext">Datenbankkontext</param>
        /// <returns>Den neue erstellten Kontakt</returns>
        public static Contact CreateContact(string phone, string fax, string mobilePhone, string email, DataClasses1DataContext dbContext)
        {
            Contact contact = new Contact()
            {
                Phone = phone,
                Fax = fax,
                MobilePhone = mobilePhone,
                Email = email
            };

            dbContext.WriteLogItem("Kontakt angelegt.", LogTypes.INSERT, contact.Id, "Contact");
            dbContext.Contact.InsertOnSubmit(contact);
            return contact;
        }
        /// <summary>
        /// Löscht den angegeben Kontakt aus der Datenbank
        /// </summary>
        /// <param name="adressId">Id des Kontakts.</param>
        /// <param name="dbContext"></param>
        public static void DeleteContact(int contactId, DataClasses1DataContext dbContext)
        {
            Contact contact = dbContext.Contact.SingleOrDefault(q => q.Id == contactId);
            if (contact != null)
            {
                dbContext.WriteLogItem("Contact gelöscht.", LogTypes.DELETE, contactId, "Contact");
                dbContext.Contact.DeleteOnSubmit(contact);
            }
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
        partial void OnEmailChanging(string value)
        {
            this.WriteUpdateLogItem("Email", this.Email, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnFaxChanging(string value)
        {
            this.WriteUpdateLogItem("Fax", this.Fax, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnPhoneChanging(string value)
        {
            this.WriteUpdateLogItem("Telefon", this.Phone, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnMobilePhoneChanging(string value)
        {
            this.WriteUpdateLogItem("Mobiltelefon", this.MobilePhone, value);
        }
    }
}
