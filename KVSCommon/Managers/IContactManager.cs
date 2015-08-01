using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IContactManager : IEntityManager<Contact, int>
    {
        /// <summary>
        /// Erstelle einene neuen Kontakt 
        /// </summary>
        /// <param name="phone">Telefon</param>
        /// <param name="fax">Fax</param>
        /// <param name="mobilePhone">Mobil</param>
        /// <param name="email">Mail</param>
        /// <returns>Den neue erstellten Kontakt</returns>
        Contact CreateContact(string phone, string fax, string mobilePhone, string email);
    }
}
