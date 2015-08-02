using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IPriceManager : IEntityManager<Price, int>
    {
        /// <summary>
        /// Erstellt einen neuen Preis.
        /// </summary>
        /// <param name="amount">Der Betrag des Preises.</param>
        /// <param name="authorativeCharge">Die behoerdliche Gebuehr.</param>
        /// <param name="productId">Id des Produkts, für den der Preis gilt.</param>
        /// <param name="locationId">Id des Standorts, falls benoetigt. </param>
        /// <returns>Den neuen Preis.</returns>
        Price CreatePrice(decimal amount, decimal? authorativeCharge, int productId, int? locationId, int? accountId);

        /// <summary>
        /// Create price account
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <param name="price"></param>
        /// <param name="withLocation"></param>
        void CreateAccount(int? AccountNumber, Price price, bool withLocation = false);
    }
}
