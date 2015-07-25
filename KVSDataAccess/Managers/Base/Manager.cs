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
    public abstract class Manager : IManager
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        protected Manager(IEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            DataContext = context;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public IEntities DataContext
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public virtual void SaveChanges()
        {
            DataContext.SaveChanges();
        }

        #endregion
    }
}
