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
    public abstract class ManagerBase<TEntity, TId> : ReadOnlyManagerBase<TEntity, TId>, IManagerBase<TEntity, TId>
        where TEntity : IHasId<TId>, IRemovable
        where TId : struct, IEquatable<TId>
    {
        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        protected ManagerBase(IEntities context)
            : base(context)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="entity"></param>
        public abstract void AddEntity(TEntity entity);

        /// <summary>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract bool RemoveEntity(TEntity entity);

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract bool RemoveEntity(TId id);

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public abstract TEntity CreateEntity();
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TDataContext"></typeparam>
    public abstract class ManagerBase<TEntity, TId, TDataContext> : ManagerBase<TEntity, TId>,
        IManagerBase<TEntity, TId, TDataContext>
        where TDataContext : class, IEntities
        where TEntity : IHasId<TId>, IRemovable
        where TId : struct, IEquatable<TId>
    {
        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        protected ManagerBase(TDataContext context)
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
