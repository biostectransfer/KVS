using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Enums
{
    /// <summary>
    /// Invoice types
    /// </summary>
    public enum InvoiceType
    {
        /// <summary>
        /// Einzelrechnung 
        /// </summary>
        Single = 1,

        /// <summary>
        /// Sammelrechnung 
        /// </summary>
        Collection = 2,

        /// <summary>
        /// Monatsrechnung 
        /// </summary>
        Monthly = 3,

        /// <summary>
        /// Wochenrechnung 
        /// </summary>
        Weekly = 4,
    }    
}
