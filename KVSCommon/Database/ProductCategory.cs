using KVSCommon.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse fuer die DB Tabelle ProductCategory
    /// </summary>
    public partial class ProductCategory : ILogging, IHasId<int>, IRemovable, ISystemFields
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

        /// <summary>
        /// Erstellt eine neue Produktkategorie mit uebergebenem Namen.
        /// </summary>
        /// <param name="name">Name der Kategorie.</param>
        /// <param name="dbContext">Datenbankkontext, mit dem die Kategorie erstellt wird.</param>
        /// <returns>Die neu erstellte Produktkategorie.</returns>
        public static ProductCategory CreateProductCategory(string name, KVSEntities dbContext)
        {
            var item = new ProductCategory()
            {
                LogDBContext = dbContext,
                Name = name
            };

            dbContext.ProductCategory.InsertOnSubmit(item);
            dbContext.SubmitChanges();
            dbContext.WriteLogItem("Produktkategorie " + item.Name + " angelegt.", LogTypes.INSERT, item.Id, "ProductCategory");
            return item;
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
        partial void OnNameChanging(string value)
        {
            this.WriteUpdateLogItem("Name", this.Name, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            if (action == System.Data.Linq.ChangeAction.Insert)
            {
                if ((new KVSEntities()).ProductCategory.Any(q => q.Name == this.Name && q.Id != this.Id))
                {
                    throw new Exception("Es existiert bereits eine Produktkategorie mit Namen " + this.Name + ".");
                }
            }
        }
    }
}
