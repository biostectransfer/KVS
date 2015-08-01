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
    public partial class ContactManager : EntityManager<Contact, int>, IContactManager
    {
        public ContactManager(IKVSEntities context) : base(context) { }

        /// <summary>
        /// Erstelle einene neuen Kontakt 
        /// </summary>
        /// <param name="phone">Telefon</param>
        /// <param name="fax">Fax</param>
        /// <param name="mobilePhone">Mobil</param>
        /// <param name="email">Mail</param>
        /// <returns>Den neue erstellten Kontakt</returns>
        public Contact CreateContact(string phone, string fax, string mobilePhone, string email)
        {
            Contact contact = new Contact()
            {
                Phone = phone,
                Fax = fax,
                MobilePhone = mobilePhone,
                Email = email
            };

            DataContext.AddObject(contact);
            SaveChanges();
            DataContext.WriteLogItem("Kontakt angelegt.", LogTypes.INSERT, contact.Id, "Contact");
            return contact;
        }
    }
}
