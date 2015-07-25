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
    /// <typeparam name="TDataContext"></typeparam>
    public class ReadOnlyEntityManager<TEntity, TId, TDataContext> :
        ReadOnlyManagerBase<TEntity, TId, TDataContext>, IReadOnlyEntityManager<TEntity, TId, TDataContext>
        where TEntity : class, IHasId<TId>, ISystemFields
        where TId : struct, IEquatable<TId>
        where TDataContext : class, IEntities
    {
        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        public ReadOnlyEntityManager(TDataContext context)
            : base(context)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override TEntity GetById(TId id)
        {
            return DataContext.GetSet<TEntity>().SingleOrDefault(o => o.Id.Equals(id));
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<TEntity> GetEntities()
        {
            return DataContext.GetSet<TEntity>();
        }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public abstract class ReadOnlyEntityManager<TEntity, TId> : ReadOnlyEntityManager<TEntity, TId, IEntities>
        where TEntity : class, IHasId<TId>, ISystemFields
        where TId : struct, IEquatable<TId>
    {
        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        protected ReadOnlyEntityManager(IEntities context)
            : base(context)
        {
        }
    }
}
