using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse fuer die Mailinglist DB Tabelle
    /// </summary>
    public partial class Mailinglist : ILogging
    {
        public DataClasses1DataContext LogDBContext
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
        /// Erstellt die Mailing Liste
        /// </summary>
        /// <param name="email">Email Adresse</param>
        /// <param name="typeId">Email Typ (Rechnung, Laufzettel, Lieferschein)</param>
        /// <param name="customerId">Kundenid</param>
        /// <param name="locationId">StandortiD</param>
        /// <param name="dbContext">DB Kontext</param>
        /// <returns>Mailinglist Objekt</returns>
        internal static Mailinglist CreateMailinglistItem(string email, int typeId, int? customerId, int? locationId, DataClasses1DataContext dbContext)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("Die Emailadresse darf nicht leer sein.");
            }

            Mailinglist ml = new Mailinglist()
            {
                Email = email,
                MailinglistTypeId = typeId,
                CustomerId = customerId,
                LocationId = locationId
            };

            dbContext.Mailinglist.InsertOnSubmit(ml);
            return ml;
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
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnEmailChanging(string value)
        {
            this.WriteUpdateLogItem("Email", this.Email, value);
        }
    }
}
