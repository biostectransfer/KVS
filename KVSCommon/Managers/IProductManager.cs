using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IProductManager : IEntityManager<Product, int>
    {
        /// <summary>
        /// Löscht das angegebene Produkt
        /// </summary>
        /// <param name="productId">Id des Products.</param>
        void RemoveProduct(int productId);

        /// <summary>
        /// Loescht Eintraege aus der Preisliste
        /// </summary>
        /// <param name="Price[]">Preisliste</param>
        ///  <param name="callFromProduct"></param>
        void RemovePrice(IEnumerable<Price> prices, bool callFromProduct = true);

        /// <summary>
        /// Erstellt ein neues Produkt.
        /// </summary>
        /// <param name="name">Name des Produkts.</param>
        /// <param name="productCategoryId">Id der Produktkategorie, falls gewünscht.</param>
        /// <param name="priceAmount">Standardpreis des Produkts.</param>
        /// <param name="authorativeCharge">Standardmäßge behördliche Gebühr, falls gewünscht.</param>
        /// <param name="itemNumber">Artikelnummer des Produkts.</param>
        /// <param name="needsVAT">Gibt an, ob das Produkt mehrwertsteuerpflichtig ist oder nicht.</param>
        /// <param name="orderTypeId">Id der Auftragsart.</param>
        /// <param name="registrationOrderTypeId">Id der Zulassungsauftragsart, falls benötigt.</param>
        /// <returns>neues Produkt Preis.</returns>
        Price CreateProduct(string name, int? productCategoryId, decimal priceAmount, decimal? authorativeCharge, string itemNumber,
            int orderTypeId, int? registrationOrderTypeId, bool needsVAT);

        /// <summary>
        /// Return customer products by product id
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="productId"></param>
        /// <returns></returns>
        IQueryable<CustomerProduct> GetCustomerProducts(int customerId, int productId);

        /// <summary>
        /// Update customer product
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="productId"></param>
        /// <param name="isChecked"></param>
        void UpdateCustomerProducts(int customerId, int productId, bool isChecked);

        /// <summary>
        /// Return accounts
        /// </summary>
        /// <returns></returns>
        IQueryable<Accounts> GetAccounts();
    }
}
