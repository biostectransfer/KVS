using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Mail;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using KVSCommon.Enums;
using KVSCommon.Entities;

namespace KVSCommon.Database
{

    /// <summary>
    /// Erweiterungsklasse für die Datenbanktabelle PackingList (Lieferscheintabelle)
    /// </summary>
    partial class PackingList : ILogging, IHasId<int>, IRemovable, ISystemFields
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
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnDispatchOrderNumberChanging(string value)
        {
            this.WriteUpdateLogItem("Versandauftragsnummer", this.DispatchOrderNumber, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnIsSelfDispatchChanging(bool? value)
        {
            this.WriteUpdateLogItem("IstEigenverbringung", this.IsSelfDispatch, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnRecipientChanging(string value)
        {
            this.WriteUpdateLogItem("Empfänger", this.Recipient, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCreated()
        {
            this.EntityState = Database.EntityState.New;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnLoaded()
        {
            this.EntityState = Database.EntityState.Loaded;
        }
    }
}
