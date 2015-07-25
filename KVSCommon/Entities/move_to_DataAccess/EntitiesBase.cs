using KVSCommon.Database;
using KVSCommon.SaveActors;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSCommon.Entities.move_to_DataAccess
{
    /// <summary>
    ///     Database context for ContainerStar
    /// </summary>
    public abstract class EntitiesBase : DataContext
    {
        private readonly ISaveActorManagerBase saveActorManager;
        /// <summary>
        /// Constructs a new context instance using conventions to create the name of the database to
        ///             which a connection will be made.  The by-convention name is the full name (namespace + class name)
        ///             of the derived context class.
        ///             See the class remarks for how this is used to create a connection.
        /// </summary>
        //protected EntitiesBase()
        //{
        //}

        /// <summary>
        /// Constructs a new context instance using the given string as the name or connection string for the
        ///             database to which a connection will be made.
        ///             See the class remarks for how this is used to create a connection.
        /// </summary>
        /// <param name="saveActorManager">Manage all save actors inside</param>
        /// <param name="nameOrConnectionString">Either the database name or a connection string. </param>
        protected EntitiesBase(ISaveActorManagerBase saveActorManager, string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            this.saveActorManager = saveActorManager;
        }

        /// <summary>
        /// Constructs a new context instance using the given string as the name or connection string for the
        ///             database to which a connection will be made.
        ///             See the class remarks for how this is used to create a connection.
        /// </summary>
        protected EntitiesBase(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        /// <summary>
        /// Constructs a new context instance using the given string as the name or connection string for the
        ///             database to which a connection will be made.
        ///             See the class remarks for how this is used to create a connection.
        /// </summary>
        protected EntitiesBase(string nameOrConnectionString, System.Data.Linq.Mapping.MappingSource mappingSource)
            : base(nameOrConnectionString, mappingSource)
        {
        }

        /// <summary>
        /// Constructs a new context instance using the given string as the name or connection string for the
        ///             database to which a connection will be made.
        ///             See the class remarks for how this is used to create a connection.
        /// </summary>
        protected EntitiesBase(System.Data.IDbConnection connection)
            : base(connection)
        {
        }

        /// <summary>
        /// Constructs a new context instance using the given string as the name or connection string for the
        ///             database to which a connection will be made.
        ///             See the class remarks for how this is used to create a connection.
        /// </summary>
        protected EntitiesBase(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource)
            : base(connection, mappingSource)
        {
        }

        /// <summary>
        ///     Gets set of entities
        /// </summary>
        /// <typeparam name="TEntity">type of entities</typeparam>
        /// <returns></returns>
        public IQueryable<TEntity> GetSet<TEntity>()
            where TEntity : class
        {
            return GetTable<TEntity>();
        }

        /// <summary>
        ///     Add entity in context
        /// </summary>
        /// <typeparam name="TEntity">Type of adding entity</typeparam>
        /// <param name="entity">Adding entity</param>
        public void AddObject<TEntity>(TEntity entity)
            where TEntity : class
        {
            GetTable<TEntity>().InsertOnSubmit(entity);
        }

        /// <summary>
        ///     Create entity without adding it to context. Use <see cref="IEntities.AddObject{TEntity}" /> to add object to
        ///     context
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public TEntity CreateObject<TEntity>() where TEntity : class
        {
            return Activator.CreateInstance<TEntity>();
        }

        /// <summary>
        ///     Update entity in context
        /// </summary>
        /// <typeparam name="TEntity">Type of updating entity</typeparam>
        /// <param name="entity">Updating entity</param>
        public void UpdateObject<TEntity>(TEntity entity)
            where TEntity : class
        {
            //var entry = Entry(entity);
            //entry.State = EntityState.Modified;
        }

        /// <summary>
        ///     Delete entity in context
        /// </summary>
        /// <typeparam name="TEntity">Type of deleting entity</typeparam>
        /// <param name="entity">Deleting entity</param>
        public void DeleteObject<TEntity>(TEntity entity)
            where TEntity : class
        {
            GetTable<TEntity>().DeleteOnSubmit(entity);
        }

        /// <summary>
        ///     Saves changes only
        /// </summary>
        protected void SaveChangesInternal()
        {
            try
            {
                var changeSet = GetChangeSet();

                foreach (var dbEntityEntry in changeSet.Inserts)
                {
                    saveActorManager.DoBeforeSaveAction(dbEntityEntry, EntityState.New);
                }


                foreach (var dbEntityEntry in changeSet.Updates)
                {
                    saveActorManager.DoBeforeSaveAction(dbEntityEntry, EntityState.Loaded);
                }


                foreach (var dbEntityEntry in changeSet.Deletes)
                {
                    saveActorManager.DoBeforeSaveAction(dbEntityEntry, EntityState.Deleted);
                }

                SubmitChanges();
            }
            catch (ChangeConflictException e)
            {
#if DEBUG
                //foreach (var eve in e.EntityValidationErrors)
                //{
                    //Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    //    eve.Entry.Entity.GetType().Name,
                    //    eve.Entry.State);
                    //foreach (var ve in eve.ValidationErrors)
                    //{
                    //    Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                    //        ve.PropertyName,
                    //        ve.ErrorMessage);
                    //}
                //}
#endif //DEBUG
                throw;
            }
        }

        /// <summary>
        ///     Add entity to context
        /// </summary>
        /// <typeparam name="TEntity">Type of adding entity</typeparam>
        /// <param name="entities">Adding entities</param>
        public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            GetTable<TEntity>().InsertAllOnSubmit(entities);
        }

        /// <summary>
        ///     Save all changes from underlying context to database
        /// </summary>
        public new virtual void SaveChanges()
        {
            SaveChangesInternal();
        }

        /// <summary>
        ///     Object context
        /// </summary>
        //public ObjectContext ObjectContext
        //{
        //    get { return ((IObjectContextAdapter)this).ObjectContext; }
        //}

        /// <summary> Throws InvalidOperationException if there is any change in DbContext after the last changes saving. </summary>
        //public void ThrowIfHasChange()
        //{
        //    foreach (var item in ChangeTracker.Entries())
        //    {
        //        if (item.State != EntityState.Unchanged)
        //        {
        //            throw new InvalidOperationException(
        //                string.Format("Unexpected FeEntities' changes are detected for entity of type: {0}",
        //                    item.Entity == null ? null : item.Entity.GetType().FullName));
        //        }
        //    }
        //}

        //protected virtual void RegisterCustomMappings(DbModelBuilder modelBuilder)
        //{

        //}

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    RegisterCustomMappings(modelBuilder);
        //}
    }
}
