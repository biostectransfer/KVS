using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Enums
{
    /// <summary>
    /// Auftagsart
    /// </summary>
    public enum OrderTypes
    {
        /// <summary>
        /// Allgemein 
        /// </summary>
        Common = 1,
        
        /// <summary>
        /// Abmeldung 
        /// </summary>
        Cancellation = 2,

        /// <summary>
        /// Zulassung 
        /// </summary>
        Admission = 3,
    }
}
