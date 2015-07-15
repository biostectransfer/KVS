using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Auftragspositionsstatus
    /// </summary>
    public enum OrderItemState
    {
        Offen = 100,
        Storniert = 0,
        InBearbeitung = 300,
        Abgeschlossen = 600,
        Abgerechnet = 900
    }
}

