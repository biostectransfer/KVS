using KVSCommon.Entities;
using KVSCommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse für die DeregistrationOrder Tabelle
    /// </summary>
    public partial class DeregistrationOrder : ILogging, IHasId<int>, IRemovable, ISystemFields
    {
        public KVSEntities LogDBContext
        {
            get;
            set;
        }

        public object ItemId
        {
            get
            {
                return this.Id;
            }
        }

        public EntityState EntityState
        {
            get;
            set;
        }

        /// <summary>
        /// Validierungs Methode um falsche Eingaben zu verhindern
        /// </summary>
        /// <param name="action"></param>
        partial void OnCreated()
        {
            this.EntityState = Database.EntityState.New;
        }
        /// <summary>
        /// Validierungs Methode um falsche Eingaben zu verhindern
        /// </summary>
        /// <param name="action"></param>
        partial void OnLoaded()
        {
            this.EntityState = Database.EntityState.Loaded;
        }
    }
}
