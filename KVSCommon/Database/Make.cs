using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse fue die DB Tabelle Make
    /// </summary>
    public partial class Make : ILogging
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
        /// Erstellt einen neuen Fahrzeughersteller.
        /// </summary>
        /// <param name="name">Name des Herstellers.</param>
        /// <param name="hsn">HSN des Herstellers.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Den neuen Hersteller.</returns>
        public static Make CreateMake(string name, string hsn, DataClasses1DataContext dbContext)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Der Name des Fahrzeugherstellers darf nicht leer sein.");
            }

            Make make = new Make()
            {
                Name = name,
                HSN = hsn
            };

            dbContext.Make.InsertOnSubmit(make);
            dbContext.WriteLogItem("Fahrzeughersteller " + name + " wurde angelegt.", LogTypes.INSERT, make.Id, "Make");
            return make;
        }

        /// <summary>
        /// Gibt alle Herstellern by HSN zurück
        /// </summary>
        /// <param name="hsn">HSN des Herstellers.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Herstellers</returns>
        public static string GetMakeByHSN(string hsn)
        {
            string hsnResult = string.Empty;
            bool atLeastOneHSNFound = false;

            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            var hsnQuery = from make in dbContext.Make
                           where make.HSN == hsn
                           select make.Name;

            foreach (string hsnName in hsnQuery)
            {
                atLeastOneHSNFound = true;
                hsnResult += hsnName + " ";
            }

            hsnResult = "Hersteller: " + hsnResult;

            if (atLeastOneHSNFound == false)
            {
                hsnResult = "Wir haben kein Hersteller mit dem Nummer " + hsn + " gefunden";
            }

            return hsnResult;
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
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            if (action == System.Data.Linq.ChangeAction.Insert || action == System.Data.Linq.ChangeAction.Update)
            {
                using (DataClasses1DataContext dbContext = new DataClasses1DataContext())
                {
                    if (dbContext.Make.Any(q => q.HSN == this.HSN))
                    {
                        throw new Exception("Es existiert bereits ein Fahrzeughersteller mit der HSN " + this.HSN + ".");
                    }
                }
            }
        }
    }
}
