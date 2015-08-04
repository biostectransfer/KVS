using KVSCommon.Entities;
using KVSCommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse fuer die DB Tabelle RegistrationLocation
    /// </summary>
    public partial class RegistrationLocation : IHasId<int>, IRemovable, ISystemFields
    {
    }
}
