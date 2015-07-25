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
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TDataContext"></typeparam>
    public class EntityManager<TEntity, TId, TDataContext> :
        ReadOnlyEntityManager<TEntity, TId, TDataContext>, IEntityManager<TEntity, TId, TDataContext>
        where TEntity : class, IHasId<TId>, IRemovable, ISystemFields
        where TDataContext : class, IEntities
        where TId : struct, IEquatable<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public EntityManager(TDataContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool SaveChangesAutomatically
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void AddEntity(TEntity entity)
        {
            DataContext.AddObject(entity);

            if (SaveChangesAutomatically)
                DataContext.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool RemoveEntity(TEntity entity)
        {
            if (entity != null)
            {
                DoRemove(entity);
                DoUpdate(entity);

                if (SaveChangesAutomatically)
                    DataContext.SaveChanges();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void DoRemove(TEntity entity)
        {
            entity.DeleteDate = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void DoUpdate(TEntity entity)
        {
            entity.ChangeDate = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveEntity(TId id)
        {
            var entity = GetById(id);

            return RemoveEntity(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TEntity CreateEntity()
        {
            return DataContext.CreateObject<TEntity>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public abstract class EntityManager<TEntity, TId> : EntityManager<TEntity, TId, IEntities>
        where TEntity : class, IHasId<TId>, IRemovable, ISystemFields
        where TId : struct, IEquatable<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        protected EntityManager(IEntities context) :
            base(context)
        {
        }
    }
}
