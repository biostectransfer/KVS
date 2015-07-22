using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Enums
{
    /// <summary>
    /// Auftag Position Status
    /// </summary>
    public enum OrderItemStatusTypes
    {
        /// <summary>
        /// Gelöscht 
        /// </summary>
        Deleted = -1,
        
        /// <summary>
        /// Storniert 
        /// </summary>
        Cancelled = 0,

        /// <summary>
        /// Offen 
        /// </summary>
        Open = 100,

        /// <summary>
        /// Bearbeitung 
        /// </summary>
        InProgress = 300,
        
        /// <summary>
        /// Abgeschlossen 
        /// </summary>
        Closed = 600,
        
        /// <summary>
        /// Abgerechnet 
        /// </summary>
        Payed = 900,
    }
}
