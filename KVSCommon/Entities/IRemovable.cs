using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSCommon.Entities
{
    /// <summary>
    /// Interface entity with Delete Data
    /// </summary>
    public interface IRemovable
    {
        /// <summary>
        /// Delete date entity 
        /// </summary>
        DateTime? DeleteDate { get; set; }
    }
}
