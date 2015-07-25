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
    /// <typeparam name="TDataContext"></typeparam>
    public interface IEntityManager<TEntity, TId, TDataContext> :
        IManagerBase<TEntity, TId, TDataContext>
        where TEntity : class, IHasId<TId>, IRemovable
        where TDataContext : class, IEntities
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IEntityManager<TEntity, TId> : IEntityManager<TEntity, TId, IEntities>
        where TEntity : class, IHasId<TId>, IRemovable
    {
    }
}
