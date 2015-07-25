using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSCommon.Entities
{
    /// <summary>
    /// Interface entity with id  
    /// </summary>
    public interface IHasId<TId>
    {
        /// <summary>
        /// Entity id  
        /// </summary>
        TId Id { get; set; }
    }
}
