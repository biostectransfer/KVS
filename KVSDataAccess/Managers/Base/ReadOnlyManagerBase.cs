using KVSCommon.Entities;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSDataAccess.Managers.Base
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public abstract class ReadOnlyManagerBase<TEntity, TId> : Manager, IReadOnlyManagerBase<TEntity, TId>
        where TEntity : IHasId<TId>
        where TId : struct, IEquatable<TId>
    {
        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        protected ReadOnlyManagerBase(IEntities context)
            : base(context)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract TEntity GetById(TId id);

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<TEntity> GetEntities();

        /// <summary>
        /// </summary>
        /// <param name="whereStatement"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetEntities(Func<TEntity, bool> whereStatement)
        {
            return GetEntities().Where(whereStatement);
        }

        /// <summary>
        /// </summary>
        /// <param name="anyStatement"></param>
        /// <returns></returns>
        public bool Any(Func<TEntity, bool> anyStatement)
        {
            return GetEntities().Any(anyStatement);
        }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TDataContext"></typeparam>
    public abstract class ReadOnlyManagerBase<TEntity, TId, TDataContext> :
        ReadOnlyManagerBase<TEntity, TId>, IReadOnlyManagerBase<TEntity, TId, TDataContext>
        where TDataContext : class, IEntities
        where TEntity : IHasId<TId>
        where TId : struct, IEquatable<TId>
    {
        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        protected ReadOnlyManagerBase(TDataContext context)
            : base(context)
        {
        }

        /// <summary>
        /// </summary>
        public new TDataContext DataContext
        {
            get { return base.DataContext as TDataContext; }
            set { base.DataContext = value; }
        }
    }
}
