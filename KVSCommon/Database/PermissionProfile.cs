using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse fuer die DB Tabelle PermissionProfile
    /// </summary>
    public partial class PermissionProfile : ILogging
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
        /// Erstellt ein neues Rechteprofil.
        /// </summary>
        /// <param name="name">Name des Rechteprofils.</param>
        /// <param name="description">Beschreibung des Rechteprofils.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Das neue Rechteprofil.</returns>
        public static PermissionProfile CreatePermissionProfile(string name, string description, DataClasses1DataContext dbContext)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Der Name darf nicht leer sein.");
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException("Die Beschreibung darf nicht leer sein.");
            }

            var profile = new PermissionProfile()
            {
                Name = name,
                Description = description
            };

            dbContext.PermissionProfile.InsertOnSubmit(profile);
            dbContext.SubmitChanges();
            dbContext.WriteLogItem("Rechteprofil " + name + " angelegt.", LogTypes.INSERT, profile.Id, "PermissionProfile");
            return profile;
        }

        /// <summary>
        /// Loescht das Rechteprofil mit der uebergebenen Id.
        /// </summary>
        /// <param name="id">Id des Rechteprofils.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        public static void DeletePermissionProfileById(int id, DataClasses1DataContext dbContext)
        {
            var item = dbContext.PermissionProfile.SingleOrDefault(q => q.Id == id);
            if (item == null)
            {
                throw new Exception("Es ist kein Rechteprofil mit der id " + id.ToString() + " vorhanden.");
            }

            foreach (var userProfile in item.UserPermissionProfile)
            {
                dbContext.WriteLogItem("Rechteprofil " + item.Name + " wurde dem Benutzer " + userProfile.User.Login + " entzogen, da das Rechteprofil gelöscht wurde.", LogTypes.UPDATE, userProfile.User.Id, "User");
                item.UserPermissionProfile.Remove(userProfile);
            }

            dbContext.WriteLogItem("Rechteprofil " + item.Name + " wurde gelöscht.", LogTypes.DELETE, "PermissionProfile");
            dbContext.PermissionProfile.DeleteOnSubmit(item);
        }

        /// <summary>
        /// Fuegt dem Rechteprofil das uebergebene Recht hinzu.
        /// </summary>
        /// <param name="permissionId">Id des Rechts.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        public void AddPermission(int permissionId, DataClasses1DataContext dbContext)
        {
            Permission permission = dbContext.Permission.SingleOrDefault(q => q.Id == permissionId);
            if (permission == null)
            {
                throw new Exception("Es gibt kein Recht mit der Id " + permission.Id + ".");
            }

            if (this.PermissionProfilePermission.Any(q => q.PermissionId == permission.Id))
            {
                throw new Exception(String.Format("Das Recht {0} gehört bereits zum Profil {1}.", permission.Name, this.Name));
            }

            dbContext.WriteLogItem("Recht " + permission.Name + " wurde Rechteprofil " + this.Name + " zugewiesen.", LogTypes.UPDATE, this.Id, "PermissionProfilePermission", permission.Id);
            this.PermissionProfilePermission.Add(new PermissionProfilePermission()
            {
                PermissionId = permission.Id
            });
        }

        /// <summary>
        /// Entfernt das übergebene Recht aus dem Rechteprofil.
        /// </summary>
        /// <param name="permissionId">Id des Rechts.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        public void RemovePermission(int permissionId, DataClasses1DataContext dbContext)
        {
            Permission permission = dbContext.Permission.SingleOrDefault(q => q.Id == permissionId);
            if (permission == null)
            {
                throw new Exception("Es gibt kein Recht mit der Id " + permissionId + ".");
            }

            PermissionProfilePermission profilePermission = this.PermissionProfilePermission.SingleOrDefault(q => q.PermissionId == permission.Id);
            if (profilePermission == null)
            {
                throw new Exception(String.Format("Das Recht {0} ist nicht im Profil {1} enthalten.", permission.Name, this.Name));
            }

            dbContext.WriteLogItem("Recht " + permission.Name + " wurde aus Rechteprofil " + this.Name + " entfernt.", LogTypes.UPDATE, this.Id, "PermissionProfilePermission", permission.Id);
            dbContext.PermissionProfilePermission.DeleteOnSubmit(profilePermission);
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
            if (action == System.Data.Linq.ChangeAction.Insert)
            {
                if (string.IsNullOrEmpty(this.Name))
                {
                    throw new ArgumentException("Der Name darf nicht leer sein.");
                }

                if (string.IsNullOrEmpty(this.Description))
                {
                    throw new ArgumentException("Die Beschreibung darf nicht leer sein.");
                }
            }
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnNameChanging(string value)
        {
            this.WriteUpdateLogItem("Name", this.Name, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnDescriptionChanging(string value)
        {
            this.WriteUpdateLogItem("Beschreibung", this.Description, value);
        }
    }
}
