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
    public interface IManager
    {
        /// <summary>
        /// 
        /// </summary>
        IEntities DataContext
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        void SaveChanges();
    }    
}
