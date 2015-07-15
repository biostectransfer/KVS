using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse fuer die DB Tabelle Price
    /// </summary>
    public partial class Price : ILogging
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
        /// Erstellt einen neuen Preis.
        /// </summary>
        /// <param name="amount">Der Betrag des Preises.</param>
        /// <param name="authorativeCharge">Die behoerdliche Gebuehr.</param>
        /// <param name="productId">Id des Produkts, für den der Preis gilt.</param>
        /// <param name="locationId">Id des Standorts, falls benoetigt. </param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Den neuen Preis.</returns>
        public static Price CreatePrice(decimal amount, decimal? authorativeCharge, Guid productId, Guid? locationId, Guid? accountId, DataClasses1DataContext dbContext)
        {
            if (dbContext.Price.Any(q => q.ProductId == productId && q.LocationId == locationId))
            {
                throw new Exception("Für dieses Produkt und diesen Standort ist bereits ein Preis eingetragen.");
            }

            var productName = dbContext.Product.Single(q => q.Id == productId).Name;
            string standortText = string.Empty;
            if (locationId.HasValue)
            {
                standortText = " und Standort " + dbContext.Location.Single(q => q.Id == locationId).Name;
            }

            var price = new Price()
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                AuthorativeCharge = authorativeCharge,
                LocationId = locationId,
                ProductId = productId,
               
            };
            dbContext.WriteLogItem("Preis für Produkt " + productName + standortText + " eingetragen.", LogTypes.INSERT, price.Id, "Price");
            dbContext.Price.InsertOnSubmit(price);
            if (accountId.HasValue)
            {
                
                var account = new PriceAccount()
                {
                    PriceId = price.Id,
                    AccountId =accountId.Value

                };
                dbContext.PriceAccount.InsertOnSubmit(account);
                dbContext.WriteLogItem("Account für Produkt " + productName + standortText + " eingetragen.", LogTypes.INSERT, price.Id, "Price");
            }
          
            
            return price;
        }
        /// <summary>
        /// Loescht Eintraege aus der Preisliste
        /// </summary>
        /// <param name="Price[]">Preisliste</param>
        ///  <param name="callFromProduct"></param>
        /// <param name="dbContext"></param>
        public static void RemovePrice(Price[] pr, DataClasses1DataContext dbContext, bool callFromProduct = true)
        {

            foreach (var price in pr)
            {
                if (!callFromProduct && price.LocationId == null)
                {
                    throw new Exception("Aus der Preisliste darf kein Standardpreis gelöscht werden!");
                }
                var orders = (from order in dbContext.Order
                              join orderserivce in dbContext.OrderItem on order.Id equals orderserivce.OrderId
                              where (price.LocationId.HasValue ? order.LocationId == price.LocationId : order.LocationId == null) && orderserivce.ProductId == price.ProductId
                              select new { order });//.ToList();
                    if (orders.ToList().Count() > 0)
                    {
                        throw new Exception("Löschen nicht möglich! Zu diesem Produkt: "+price.Product.Name+" mit dem Preis: "+price.Amount+ "  bereits Auftragspositionen vorhanden!");
                    }
                    var priceAccount = dbContext.PriceAccount.Where(q => q.PriceId == price.Id);
                    dbContext.PriceAccount.DeleteAllOnSubmit(priceAccount);
                    dbContext.WriteLogItem("PriceAccounts mit der Id:" + price.Id + " wurden gelöscht.", LogTypes.DELETE, price.Id, "PriceAccounts");
                    var customerProducts = dbContext.CustomerProduct.Where(q => q.ProductId == price.ProductId);
                    dbContext.CustomerProduct.DeleteAllOnSubmit(customerProducts);
                    dbContext.WriteLogItem("CustomerProduct mit der Id:" + price.ProductId + " wurden gelöscht.", LogTypes.DELETE, price.Id, "CustomerProduct");
                    dbContext.Price.DeleteOnSubmit(price);
                    dbContext.WriteLogItem("Price mit der Id:" + price.Id + " wurde gelöscht.", LogTypes.DELETE, price.Id, "Id");
             }
            
            //if (callFromProduct)
            //{
            //    var price = dbContext.Price.Where(q => q.ProductId == productId && q.LocationId == locationId);
            //    var orders = (from order in dbContext.Order
            //                  join orderserivce in dbContext.OrderItem on order.Id equals orderserivce.OrderId
            //                  where order.LocationId == locationId && orderserivce.ProductId == productId
            //                  select new { order }).ToList();
            //    if (orders.Count() > 0)
            //    {
            //        throw new Exception("Löschen nicht möglich! Zu diesem Preis sind bereits Auftragspositionen vorhanden!");
            //    }
            //    //var priceAccount = dbContext.PriceAccount.Where(q => q.PriceId == price.Id);
            //    //dbContext.PriceAccount.DeleteAllOnSubmit(priceAccount);
            //    //dbContext.WriteLogItem("PriceAccounts mit der Id:" + price.Id + " wurden gelöscht.", LogTypes.DELETE, price.Id, "PriceAccounts");

            //    //var customerProducts = dbContext.CustomerProduct.Where(q => q.ProductId == price.ProductId);
            //    //dbContext.CustomerProduct.DeleteAllOnSubmit(customerProducts);
            //    //dbContext.WriteLogItem("CustomerProduct mit der Id:" + price.ProductId + " wurden gelöscht.", LogTypes.DELETE, price.Id, "CustomerProduct");
            //    //dbContext.Price.DeleteOnSubmit(price);
            //    //dbContext.WriteLogItem("Price mit der Id:" + price.Id + " wurde gelöscht.", LogTypes.DELETE, price.Id, "Id");
            //}
        }
        partial void OnCreated()
        {
            this.EntityState = Database.EntityState.New;
        }

        partial void OnLoaded()
        {
            this.EntityState = Database.EntityState.Loaded;
        }

        partial void OnAuthorativeChargeChanging(decimal? value)
        {
            this.WriteUpdateLogItem("Behördliche Gebühr", this.AuthorativeCharge, value);
        }

        partial void OnAmountChanging(decimal value)
        {
            this.WriteUpdateLogItem("Betrag", this.Amount, value);
        }
     
        partial void OnLocationIdChanging(Guid? value)
        {
            if (this.EntityState != Database.EntityState.New)
            {
                throw new Exception("Der Standort eines Preises kann nicht geändert werden.");
            }
        }

        partial void OnProductIdChanging(Guid value)
        {
            if (this.EntityState != Database.EntityState.New)
            {
                throw new Exception("Das Produkt eines Preises kann nicht geändert werden.");
            }
        }
    }
}
