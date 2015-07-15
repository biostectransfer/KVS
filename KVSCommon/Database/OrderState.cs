using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Auftragsstatus
    /// </summary>
    public enum OrderState
    {
        Offen = 100,
        Storniert = 0,
        InBearbeitung = 300,
        Zulassungsstelle = 400,
        Überarbeitet = 500,
        Abgeschlossen = 600,
        Teilabgerechnet = 700,
        Abgerechnet = 900
    }
}