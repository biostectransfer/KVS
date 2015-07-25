using KVSCommon.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSCommon.Managers.Base
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IReadOnlyManagerBase<out TEntity, in TId> : IManager
        where TEntity : IHasId<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetById(TId id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetEntities();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereStatement"></param>
        /// <returns></returns>
        IEnumerable<TEntity> GetEntities(Func<TEntity, bool> whereStatement);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anyStatement"></param>
        /// <returns></returns>
        bool Any(Func<TEntity, bool> anyStatement);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TDataContext"></typeparam>
    public interface IReadOnlyManagerBase<TEntity, TId, TDataContext> : IReadOnlyManagerBase<TEntity, TId>
        where TDataContext : IEntities
        where TEntity : IHasId<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        new TDataContext DataContext
        {
            get;
            set;
        }
    }
}
