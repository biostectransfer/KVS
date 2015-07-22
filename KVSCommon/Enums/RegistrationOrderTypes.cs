using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Enums
{
    /// <summary>
    /// Zulassungsart
    /// </summary>
    public enum RegistrationOrderTypes
    {
        /// <summary>
        /// Neuzulassung
        /// </summary>
        NewAdmission = 1,
        
        /// <summary>
        /// Tageszulassung
        /// </summary>
        DailyAdmission = 2,

        /// <summary>
        /// Umkennzeichnung
        /// </summary>
        Renumbering = 3,

        /// <summary>
        /// Wiederzulassung
        /// </summary>
        Readmission = 4,
    }
}
