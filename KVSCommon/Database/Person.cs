using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse fuer die DB Tabelle Person
    /// </summary>
    public partial class Person : ILogging
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
        /// Gibt den vollständigen Benutzernamen zurueck
        /// </summary>
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.FirstName))
                {
                    return this.FirstName + " " + this.Name;
                }
                else
                {
                    return this.Title + " " + this.Name;
                }
            }
        }
        /// <summary>
        /// Loescht die angegebene Person aus der Datenbank.
        /// </summary>
        /// <param name="adressId">Id der Person.</param>
        /// <param name="dbContext">DB Kontext</param>
        public static void DeletePerson(int personId, KVSEntities dbContext)
        {
            Person person = dbContext.Person.SingleOrDefault(q => q.Id == personId);
            if (person != null)
            {
                dbContext.WriteLogItem("Person gelöscht.", LogTypes.DELETE, personId, "Person");
                dbContext.Person.DeleteOnSubmit(person);
            }
        }
        /// <summary>
        /// Erstellt einen neunen Person Datensatz
        /// </summary>
        /// <param name="dbContext">DB Kontext</param>
        /// <param name="firstname">Vorname</param>
        /// <param name="name">Name</param>
        /// <param name="title">Titel</param>
        /// <param name="extension">Zusatz</param>
        /// <returns>Person</returns>
       public static Person CreatePerson(KVSEntities dbContext, string firstname, string name, string title, string extension)
        {
            var person = new Person()
            {
                FirstName = firstname,
                Name = name,
                Gender = null,
                Title = title,
                Extension = extension,
                
            };
            
            dbContext.Person.InsertOnSubmit(person);
            dbContext.SubmitChanges();
            dbContext.WriteLogItem("Kontaktperson " + firstname + " " + name + " wurde angelegt.", LogTypes.INSERT, person.Id, "LargeCustomer");

            return person;

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
        partial void OnFirstNameChanging(string value)
        {
            this.WriteUpdateLogItem("Vorname", this.FirstName, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnNameChanging(string value)
        {
            this.WriteUpdateLogItem("Nachname", this.Name, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnGenderChanging(string value)
        {
            this.WriteUpdateLogItem("Geschlecht", this.Gender, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnTitleChanging(string value)
        {
            this.WriteUpdateLogItem("Anrede", this.Title, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnExtensionChanging(string value)
        {
            this.WriteUpdateLogItem("Zusatz", this.Extension, value);
        }
    }
}
