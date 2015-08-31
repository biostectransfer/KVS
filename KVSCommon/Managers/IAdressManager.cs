using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IAdressManager : IEntityManager<Adress, int>
    {
        /// <summary>
        /// Erstellt eine neue Adresse.
        /// </summary>
        /// <param name="street">Straße.</param>
        /// <param name="streetNumber">Hausnummer.</param>
        /// <param name="zipcode">Postleitzahl.</param>
        /// <param name="city">Ort.</param>
        /// <param name="country">Land.</param>
        /// <returns>Die neue Adresse.</returns>
        Adress CreateAdress(string street, string streetNumber, string zipcode, string city, string country, bool saveChanges = true);
    }
}
