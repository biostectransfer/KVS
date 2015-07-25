using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    public partial class Product : ILogging
    {
        public KVSEntities LogDBContext
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
        private KVSEntities myDbContext;
        public KVSEntities _dbContext
        {
            get { return myDbContext; }
            set { myDbContext = value; }
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
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Das neue Produkt.</returns>
        public static Product CreateProduct(string name, int? productCategoryId, decimal priceAmount, decimal? authorativeCharge, string itemNumber, 
            int orderTypeId, int? registrationOrderTypeId, bool needsVAT, KVSEntities dbContext)
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
                _dbContext = dbContext
            };

            var price = new Price()
            {
                Amount = priceAmount,
                AuthorativeCharge = authorativeCharge,
                
            };

            product.Price.Add(price);
            dbContext.Product.InsertOnSubmit(product);
            dbContext.SubmitChanges();
            dbContext.WriteLogItem("Produkt " + name + " wurde angelegt.", LogTypes.INSERT, product.Id, "Product");

            return product;
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
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Den neuen Preis.</returns>
        public static Price CreateProduct(string name, int? productCategoryId, decimal priceAmount, decimal? authorativeCharge, string itemNumber,
            int orderTypeId, int? registrationOrderTypeId, bool needsVAT, bool accountId,  KVSEntities dbContext)
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
                _dbContext = dbContext
            };

            var price = new Price()
            {
                Amount = priceAmount,
                AuthorativeCharge = authorativeCharge,
                
            };
            product.Price.Add(price);
            dbContext.Product.InsertOnSubmit(product);
            dbContext.SubmitChanges();
            dbContext.WriteLogItem("Produkt " + name + " wurde angelegt.", LogTypes.INSERT, product.Id, "Product");

            return price;
        }
         /// <summary>
        /// Löscht das angegebene Produkt
        /// </summary>
        /// <param name="productId">Id des Products.</param>
        /// <param name="dbContext"></param>
        public static void RemoveProduct(int productId, KVSEntities dbContext)
        {
            Price[] pr = dbContext.Price.Where(q => q.ProductId == productId).ToArray();
            Product prod = dbContext.Product.SingleOrDefault(q => q.Id == productId);
            KVSCommon.Database.Price.RemovePrice(pr, dbContext,true);
            dbContext.WriteLogItem("Product mit der Id:" + prod.Id + " und dem Namen: " + prod.Name + " gelöscht.", LogTypes.DELETE, prod.Id, "Product");
            dbContext.Product.DeleteOnSubmit(prod);
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
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            if (action == System.Data.Linq.ChangeAction.Insert)
            {
                KVSEntities dbContext;
                if (_dbContext != null)
                {
                    dbContext = _dbContext;
                }
                else
                {
                    dbContext = new KVSEntities();
                }
             
                    if (dbContext.Product.Any(q => q.Name == this.Name && q.ItemNumber == this.ItemNumber))
                    {
                        throw new Exception("Es existiert bereits ein Produkt mit Namen " + this.Name + " und der Artikelnummer " + this.ItemNumber + " . ");
                    }
            }
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnNameChanging(string value)
        {
            this.WriteUpdateLogItem("Name", this.Name, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnProductCategoryIdChanging(int? value)
        {
            if (this.EntityState != Database.EntityState.New)
            {
                this.CheckLoggingPossible();
                if (this.ProductCategoryId.HasValue)
                {
                    if (value.HasValue)
                    {
                        this.LogDBContext.WriteLogItem("Zuordnung des Produkts " + this.Name + " von Kategorie " + this.ProductCategory.Name + " zu  " + this.LogDBContext.ProductCategory.Single(q => q.Id == value.Value).Name + " geändert.", LogTypes.UPDATE, this.Id, "Product", value.Value);
                    }
                    else
                    {
                        this.LogDBContext.WriteLogItem("Zuordnung des Produkts " + this.Name + " zu Kategorie " + this.ProductCategory.Name + " aufgehoben.", LogTypes.UPDATE, this.Id, "Product", this.ProductCategoryId.Value);
                    }
                }
                else
                {
                    ProductCategory category = this.LogDBContext.ProductCategory.Single(q => q.Id == value.Value);
                    this.LogDBContext.WriteLogItem("Produkt wurde der Kategorie " + category.Name + " zugeordnet.", LogTypes.UPDATE, this.Id, "Product", category.Id);
                }
            }
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnItemNumberChanging(string value)
        {
            this.WriteUpdateLogItem("Artikelnummer", this.ItemNumber, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnOrderTypeIdChanging(int value)
        {
            this.WriteUpdateLogItem("Auftragsart", this.OrderTypeId, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnRegistrationOrderTypeIdChanging(int? value)
        {
            this.WriteUpdateLogItem("Zulassungsauftragsart", this.RegistrationOrderTypeId, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnNeedsVATChanging(bool value)
        {
            this.WriteUpdateLogItem("Mehrwertsteuerpflicht", this.NeedsVAT, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnIsLockedChanging(bool value)
        {
            this.WriteUpdateLogItem("Gesperrt", this.IsLocked, value);
        }
   
    }
}
