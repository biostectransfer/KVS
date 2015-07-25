using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSCommon.Entities
{

    /// <summary>
    ///     Entities interface
    /// </summary>
    public interface IEntities
    {
        /// <summary>
        ///     Gets set of entities
        /// </summary>
        /// <typeparam name="TEntity">type of entities</typeparam>
        /// <returns></returns>
        IQueryable<TEntity> GetSet<TEntity>() where TEntity : class;

        /// <summary>
        ///     Add entity to context
        /// </summary>
        /// <typeparam name="TEntity">Type of adding entity</typeparam>
        /// <param name="entity">Adding entity</param>
        void AddObject<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        ///     Add entity to context
        /// </summary>
        /// <typeparam name="TEntity">Type of adding entity</typeparam>
        /// <param name="entity">Adding entity</param>
        void AddRange<TEntity>(IEnumerable<TEntity> entity) where TEntity : class;

        /// <summary>
        ///     Update entity in context
        /// </summary>
        /// <typeparam name="TEntity">Type of updating entity</typeparam>
        /// <param name="entity">Updating entity</param>
        void UpdateObject<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        ///     Create entity without adding it to context. Use <see cref="AddObject{TEntity}" /> to add object to context
        /// </summary>
        /// <typeparam name="TEntity">Type of creating entity</typeparam>
        /// <returns>Entity</returns>
        TEntity CreateObject<TEntity>() where TEntity : class;

        /// <summary>
        ///     Delete entity in context
        /// </summary>
        /// <typeparam name="TEntity">Type of deleting entity</typeparam>
        /// <param name="entity">Deleting entity</param>
        void DeleteObject<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        ///     Save all changes from underlying context to database
        /// </summary>
        void SaveChanges();
    }
}
