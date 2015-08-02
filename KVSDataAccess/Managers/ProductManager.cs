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
    public partial class ProductManager : EntityManager<Product, int>, IProductManager
    {
        public ProductManager(IKVSEntities context) : base(context) { }

        /// <summary>
        /// Löscht das angegebene Produkt
        /// </summary>
        /// <param name="productId">Id des Products.</param>
        /// <param name="dbContext"></param>
        public void RemoveProduct(int productId)
        {
            var prices = DataContext.GetSet<Price>().Where(q => q.ProductId == productId).ToList();
            var product = DataContext.GetSet<Product>().SingleOrDefault(q => q.Id == productId);
            RemovePrice(prices, true);

            DataContext.DeleteObject(product);
            SaveChanges();
            DataContext.WriteLogItem("Product mit der Id:" + product.Id + " und dem Namen: " + product.Name + " gelöscht.", LogTypes.DELETE, product.Id, "Product");
        }

        /// <summary>
        /// Loescht Eintraege aus der Preisliste
        /// </summary>
        /// <param name="Price[]">Preisliste</param>
        ///  <param name="callFromProduct"></param>
        public void RemovePrice(IEnumerable<Price> prices, bool callFromProduct = true)
        {
            foreach (var price in prices)
            {
                if (!callFromProduct && price.LocationId == null)
                {
                    throw new Exception("Aus der Preisliste darf kein Standardpreis gelöscht werden!");
                }

                var orderCount = DataContext.GetSet<OrderItem>().Where(o => o.ProductId == price.ProductId).Count();

                if (orderCount > 0)
                {
                    throw new Exception("Löschen nicht möglich! Zu diesem Produkt: " + price.Product.Name + " mit dem Preis: " + price.Amount + "  bereits Auftragspositionen vorhanden!");
                }

                var priceAccounts = DataContext.GetSet<PriceAccount>().Where(q => q.PriceId == price.Id).ToList();
                foreach (var priceAccount in priceAccounts)
                    DataContext.DeleteObject(priceAccount);

                SaveChanges();
                DataContext.WriteLogItem("PriceAccounts mit der Id:" + price.Id + " wurden gelöscht.", LogTypes.DELETE, price.Id, "PriceAccounts");

                var customerProducts = DataContext.GetSet<CustomerProduct>().Where(q => q.ProductId == price.ProductId).ToList();
                foreach (var customerProduct in customerProducts)
                    DataContext.DeleteObject(customerProduct);

                SaveChanges();
                DataContext.WriteLogItem("CustomerProduct mit der Id:" + price.ProductId + " wurden gelöscht.", LogTypes.DELETE, price.Id, "CustomerProduct");

                DataContext.DeleteObject(price);
                SaveChanges();
                DataContext.WriteLogItem("Price mit der Id:" + price.Id + " wurde gelöscht.", LogTypes.DELETE, price.Id, "Id");
            }
        }

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
        public Price CreateProduct(string name, int? productCategoryId, decimal priceAmount, decimal? authorativeCharge, string itemNumber,
            int orderTypeId, int? registrationOrderTypeId, bool needsVAT)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Der Name darf nicht leer sein.");
            }

            if (string.IsNullOrEmpty(itemNumber))
            {
                throw new ArgumentException("Die Artikelnummer darf nicht leer sein.");
            }

            var product = new Product()
            {
                Name = name,
                ProductCategoryId = productCategoryId,
                ItemNumber = itemNumber,
                OrderTypeId = orderTypeId,
                RegistrationOrderTypeId = registrationOrderTypeId,
                NeedsVAT = needsVAT,
                IsLocked = false,
            };

            var price = new Price()
            {
                Amount = priceAmount,
                AuthorativeCharge = authorativeCharge,
            };

            product.Price.Add(price);
            DataContext.AddObject(price);
            DataContext.AddObject(product);
            SaveChanges();
            DataContext.WriteLogItem("Produkt " + name + " wurde angelegt.", LogTypes.INSERT, product.Id, "Product");

            return price;
        }

        /// <summary>
        /// Return customer products by product id
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public IQueryable<CustomerProduct> GetCustomerProducts(int customerId, int productId)
        {
            return DataContext.GetSet<CustomerProduct>().Where(o => o.CustomerId == customerId && o.ProductId == productId);
        }

        /// <summary>
        /// Update customer product
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="productId"></param>
        /// <param name="isChecked"></param>
        public void UpdateCustomerProducts(int customerId, int productId, bool isChecked)
        {
            var customerProducts = GetCustomerProducts(customerId, productId);

            if (customerProducts != null) // exists
            {
                if (!isChecked)// exits, but not checked - delete
                {
                    DataContext.DeleteObject(customerProducts);
                }
            }
            else //not exists
            {
                if (isChecked) //checked and not exists - insert
                {
                    var newCustProd = new CustomerProduct
                    {
                        CustomerId = customerId,
                        ProductId = productId
                    };

                    DataContext.AddObject(newCustProd);
                }
            }

            SaveChanges();
        }

        /// <summary>
        /// Return accounts
        /// </summary>
        /// <returns></returns>
        public IQueryable<Accounts> GetAccounts()
        {
            return DataContext.GetSet<Accounts>();
        }
    }
}
