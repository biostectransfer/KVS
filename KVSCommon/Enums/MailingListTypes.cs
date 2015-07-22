using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Enums
{
    /// <summary>
    /// Benachrichtigungsart
    /// </summary>
    public enum MailingListTypes
    {
        /// <summary>
        /// Rechnung 
        /// </summary>
        Invoice = 1,

        /// <summary>
        /// Lieferschein 
        /// </summary>
        PackagingList = 2,

        /// <summary>
        /// Laufzettel 
        /// </summary>
        DocketList = 3,

        /// <summary>
        /// Auftragserledigung 
        /// </summary>
        OrderComplete = 4,
    }
}
