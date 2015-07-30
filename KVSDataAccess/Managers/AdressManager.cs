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
    public partial class AdressManager : EntityManager<Adress, int>, IAdressManager
    {
        public AdressManager(IKVSEntities context) : base(context) { }

        /// <summary>
        /// Erstellt eine neue Adresse.
        /// </summary>
        /// <param name="street">Straße.</param>
        /// <param name="streetNumber">Hausnummer.</param>
        /// <param name="zipcode">Postleitzahl.</param>
        /// <param name="city">Ort.</param>
        /// <param name="country">Land.</param>
        /// <returns>Die neue Adresse.</returns>
        public Adress CreateAdress(string street, string streetNumber, string zipcode, string city, string country)
        {
            var adress = new Adress()
            {
                Street = street,
                StreetNumber = streetNumber,
                Zipcode = zipcode,
                City = city,
                Country = country
            };

            DataContext.AddObject(adress);
            SaveChanges();
            DataContext.WriteLogItem("Adresse angelegt.", LogTypes.INSERT, adress.Id, "Adress");
            return adress;
        }
    }
}
