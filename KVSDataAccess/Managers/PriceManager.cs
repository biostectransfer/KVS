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
    public partial class PriceManager : EntityManager<Price, int>, IPriceManager
    {
        public PriceManager(IKVSEntities context) : base(context) { }
        
        /// <summary>
        /// Erstellt einen neuen Preis.
        /// </summary>
        /// <param name="amount">Der Betrag des Preises.</param>
        /// <param name="authorativeCharge">Die behoerdliche Gebuehr.</param>
        /// <param name="productId">Id des Produkts, für den der Preis gilt.</param>
        /// <param name="locationId">Id des Standorts, falls benoetigt. </param>
        /// <returns>Den neuen Preis.</returns>
        public Price CreatePrice(decimal amount, decimal? authorativeCharge, int productId, int? locationId, int? accountId)
        {
            if (DataContext.GetSet<Price>().Any(q => q.ProductId == productId && q.LocationId == locationId))
            {
                throw new Exception("Für dieses Produkt und diesen Standort ist bereits ein Preis eingetragen.");
            }

            var productName = DataContext.GetSet<Product>().Single(q => q.Id == productId).Name;
            string standortText = string.Empty;
            if (locationId.HasValue)
            {
                standortText = " und Standort " + DataContext.GetSet<Location>().Single(q => q.Id == locationId).Name;
            }

            var price = new Price()
            {
                Amount = amount,
                AuthorativeCharge = authorativeCharge,
                LocationId = locationId,
                ProductId = productId,
            };

            DataContext.AddObject(price);
            SaveChanges();
            DataContext.WriteLogItem("Preis für Produkt " + productName + standortText + " eingetragen.", LogTypes.INSERT, price.Id, "Price");

            if (accountId.HasValue)
            {
                var account = new PriceAccount()
                {
                    Price = price,
                    AccountId = accountId.Value
                };

                DataContext.AddObject(account);
                SaveChanges();
                DataContext.WriteLogItem("Account für Produkt " + productName + standortText + " eingetragen.", LogTypes.INSERT, price.Id, "Price");
            }

            return price;
        }

        /// <summary>
        /// Create price account
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <param name="price"></param>
        /// <param name="withLocation"></param>
        public void CreateAccount(int? AccountNumber, Price price, bool withLocation = false)
        {
            if (AccountNumber != null)
            {
                IEnumerable<PriceAccount> accounts = null;
                if (withLocation == false)
                {
                    accounts = DataContext.GetSet<PriceAccount>().Where(q => q.PriceId == price.Id && q.Price.LocationId == null).ToList();
                }
                else
                {
                    accounts = DataContext.GetSet<PriceAccount>().Where(q => q.PriceId == price.Id && q.Price.LocationId == price.LocationId).ToList();
                }

                if (accounts.Count() > 0)
                {
                    foreach(var account in accounts)
                        DataContext.DeleteObject(account);
                }
                else
                {
                    var newAccount = new PriceAccount
                    {
                        PriceId = price.Id,
                        AccountId = AccountNumber.Value
                    };

                    DataContext.AddObject(newAccount);
                }

                SaveChanges();
            }
        }
    }
}
