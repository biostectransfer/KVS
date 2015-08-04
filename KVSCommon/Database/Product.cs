using KVSCommon.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    public partial class Product : ILogging, IHasId<int>, IRemovable, ISystemFields
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
