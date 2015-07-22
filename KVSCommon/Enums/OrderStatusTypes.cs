using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Enums
{
    /// <summary>
    /// Auftag Status
    /// </summary>
    public enum OrderStatusTypes
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
        /// Zulassungsstelle 
        /// </summary>
        AdmissionPoint = 400,

        /// <summary>
        /// Überarbeitet 
        /// </summary>
        Processed = 500,

        /// <summary>
        /// Abgeschlossen 
        /// </summary>
        Closed = 600,

        /// <summary>
        /// Teilabgerechnet 
        /// </summary>
        PartiallyPayed = 700,

        /// <summary>
        /// Abgerechnet 
        /// </summary>
        Payed = 900,
    }
}
