using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KVSCommon.Utility;
using System.IO;
using KVSCommon.Entities;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse fuer die DB Tabelle User
    /// </summary>
    public partial class User : ILogging, IHasId<int>, IRemovable, ISystemFields
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
        /// Legt einen Benutzer mit den angebenen Daten an.
        /// </summary>
        /// <param name="login">Login des Benutzers.</param>
        /// <param name="password">Passwort des Benutzers.</param>
        /// <param name="name">Nachname des Benutzers.</param>
        /// <param name="firstname">Vorname des Benutzers.</param>
        /// <param name="title">Anrede des Benutzers.</param>
        /// <param name="dbContext">Datenbankkontext, mit dem die Transaktion durchgefuehrt werden soll.</param>
        /// <returns>Den neu erstellten Benutzer.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static User CreateUser(string login, string password, string name, string firstname, string title, KVSEntities dbContext)
        {
            if (string.IsNullOrEmpty(login))
            {
                throw new ArgumentNullException("Der Benutzername darf nicht leer sein.");
            }

            if (login.Length < 3)
            {
                throw new ArgumentException("Der Benutzername muss mindestens 3 Zeichen lang sein.");
            }

            CheckPassword(password);

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Der Name darf nicht leer sein.");
            }

            // Salt und Passwort-Hash berechnen.
            SaltedHash sh = new SaltedHash();
            string passwordHash = string.Empty;
            string salt = string.Empty;
            sh.GetHashAndSaltString(password, out passwordHash, out salt);
            var user = new User()
            {
                LogDBContext = dbContext,
                Login = login,
                Password = passwordHash,
                Salt = salt,
                IsLocked = false,
                Person = new Person()
                {
                    LogDBContext = dbContext,
                    FirstName = firstname,
                    Name = name,
                    Title = title
                }
            };

            dbContext.User.InsertOnSubmit(user);
            dbContext.SubmitChanges();
            dbContext.WriteLogItem("Benutzer " + login + " wurde angelegt.", LogTypes.INSERT, user.Id, "User", user.PersonId);
            return user;
        }
        
        /// <summary>
        /// Ermoeglicht es, das Passwort eines Benutzers zu setzen. Der im dbContext uebergebene Benutzer muss dafür das Administratorrecht "ADMIN_PASSWORT_AENDERN" besitzen.
        /// </summary>
        /// <param name="newPassword">Das neue Passwort.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        public void ChangePassword(string newPassword, KVSEntities dbContext)
        {
            CheckPassword(Password);

            var adminUser = dbContext.User.SingleOrDefault(q => q.Id == dbContext.LogUserId);
            if (adminUser == null)
            {
                throw new Exception("Der Administrator-Benutzer konnte nicht gefunden werden.");
            }

            if (!adminUser.HasPermission("ADMIN_PASSWORT_AENDERN"))
            {
                throw new Exception("Sie besitzen nicht die Berechtigung, Passwörter von anderen Benutzern zu ändern.");
            }

            SaltedHash sh = new SaltedHash();
            string passwordHash = string.Empty;
            string salt = string.Empty;
            sh.GetHashAndSaltString(newPassword, out passwordHash, out salt);
            if (this.LogDBContext == null)
            {
                this.LogDBContext = dbContext;
            }

            this.Password = passwordHash;
            this.Salt = salt;
        }
        /// <summary>
        /// Prueft ob das Passwort leer oder kleiner 8 Zeichen lang ist
        /// </summary>
        /// <param name="newPassword">Neues Passwort</param>
        private static void CheckPassword(string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentNullException("Das Passwort darf nicht leer sein.");
            }

            if (newPassword.Length < 8)
            {
                throw new ArgumentException("Das Passwort muss mindestens 8 Zeichen lang sein.");
            }
        }

        /// <summary>
        /// Gibt zurueck, ob der Benutzer die uebergebene Berechtigung besitzt.
        /// </summary>
        /// <param name="permissionName">Name der Berechtigung.</param>
        /// <returns>True, wenn der Benutzer die Berechtigung besitzt, sonst false.</returns>
        public bool HasPermission(string permissionName)
        {
            if (this.UserPermission.Any(q => q.Permission.Name == permissionName))
            {
                return true;
            }
            else
            {
                if (this.UserPermissionProfile.Any(q => q.PermissionProfile.PermissionProfilePermission.Any(p => p.Permission.Name == permissionName)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gibt zurueck alle Berechtigungen für den angegebenen Benutzer.
        /// </summary>
        /// <param name="userId">ID des Benutzers.</param>
        /// <returns>Eine Liste mit Berechtigungen.</returns>
        public static List<string> GetAllPermissionsByID(int userId)
        {
            List<string> userPermissions = new List<string>();

            using (KVSEntities dbContext = new KVSEntities())
            {
                var Permissions = from perm in dbContext.Permission
                                  join userper in dbContext.UserPermission on perm.Id equals userper.PermissionId
                                  join user in dbContext.User on userper.UserId equals user.Id
                                  where user.Id == userId
                                  select perm.Name;

                if (Permissions != null)
                {
                    foreach (var permission in Permissions)
                    {
                        userPermissions.Add(permission);
                    }
                }

                var permissionsFromProfile = from upprof in dbContext.UserPermissionProfile
                                             join pprof in dbContext.PermissionProfile on upprof.PermissionProfileId equals pprof.Id
                                             join ppp in dbContext.PermissionProfilePermission on pprof.Id equals ppp.PermissionProfileId
                                             join per in dbContext.Permission on ppp.PermissionId equals per.Id
                                             where upprof.UserId == userId
                                             select per.Name;
                if (permissionsFromProfile != null)
                {
                    foreach (var permission in permissionsFromProfile)
                    {
                        if (userPermissions.Contains(permission) == false)
                        {
                            userPermissions.Add(permission);
                        }
                    }
                }
            }

            return userPermissions;
        }
        
        /// <summary>
        /// Weist dem Benutzer das uebergebene Recht zu.
        /// </summary>
        /// <param name="permissionId">Id des Rechts, das zugewiesen werden soll.</param>
        /// <param name="dbContext">Datenbank-Context, mit dem das Recht zugewiesen wird.</param>
        public void AddPermission(int permissionId, KVSEntities dbContext)
        {
            var permission = dbContext.Permission.Single(q => q.Id == permissionId);
            if (this.UserPermission.Any(q => q.PermissionId == permission.Id))
            {
                throw new Exception("Der Benutzer " + this.Login + " besitzt das Recht " + permission.Name + " bereits.");
            }

            dbContext.WriteLogItem("Recht " + permission.Name + " wurde dem Benutzer " + this.Login + " zugewiesen", LogTypes.UPDATE, this.Id, "UserPermission", permission.Id);
            this.UserPermission.Add(new UserPermission()
            {
                PermissionId = permission.Id
            });
        }

        /// <summary>
        /// Entzieht dem Benutzer das übergebene Recht.
        /// </summary>
        /// <param name="permissionId">Id des Rechts, das entzogen werden soll.</param>
        /// <param name="dbContext">Datenbank-Context, mit dem das Recht entzogen wird.</param>
        public void RemovePermission(int permissionId, KVSEntities dbContext)
        {
            var permission = dbContext.Permission.Single(q => q.Id == permissionId);
            UserPermission userPermission = this.UserPermission.SingleOrDefault(q => q.PermissionId == permission.Id);
            if (userPermission == null)
            {
                throw new Exception("Der Benutzer " + this.Login + " besitzt das Recht " + permission.Name + " nicht.");
            }
           
            dbContext.WriteLogItem("Recht " + permission.Name + " wurde dem Benutzer " + this.Login + " entzogen.", LogTypes.UPDATE, "UserPermission");
            dbContext.UserPermission.DeleteOnSubmit(userPermission);
        }

        /// <summary>
        /// Weist dem Benutzer das uebergebene Rechteprofil zu.
        /// </summary>
        /// <param name="profileId">Id des Rechteprofils, das zugewiesen werden soll.</param>
        /// <param name="dbContext">Datenbank-Context, mit dem das Rechteprofil zugewiesen wird.</param>
        public void AddPermissionProfile(int profileId, KVSEntities dbContext)
        {
            var profile = dbContext.PermissionProfile.Single(q => q.Id == profileId);
            if (this.UserPermissionProfile.Any(q => q.PermissionProfileId == profile.Id))
            {
                throw new Exception("Der Benutzer " + this.Login + " besitzt das Rechteprofil " + profile.Name + " bereits.");
            }

            dbContext.WriteLogItem("Rechteprofil " + profile.Name + " wurde dem Benutzer " + this.Login + " zugewiesen", LogTypes.UPDATE, this.Id, "UserPermissionProfile", profile.Id);
            this.UserPermissionProfile.Add(new UserPermissionProfile()
            {
                PermissionProfileId = profile.Id
            });
        }

        /// <summary>
        /// Entzieht dem Benutzer das uebergebene Rechteprofil.
        /// </summary>
        /// <param name="profileId">Id des Rechteprofils, das entzogen werden soll.</param>
        /// <param name="dbContext">Datenbank-Context, mit dem das Rechteprofil entzogen wird.</param>
        public void RemovePermissionProfile(int profileId, KVSEntities dbContext)
        {
            var profile = dbContext.PermissionProfile.Single(q => q.Id == profileId);
            UserPermissionProfile userProfile = this.UserPermissionProfile.SingleOrDefault(q => q.PermissionProfileId == profile.Id);
            if (userProfile == null)
            {
                throw new Exception("Der Benutzer " + this.Login + " besitzt das Rechteprofil " + profile.Name + " nicht.");
            }

            dbContext.WriteLogItem("Rechteprofil " + profile.Name + " wurde dem Benutzer " + this.Login + " entzogen.", LogTypes.UPDATE, "UserPermissionProfile");
            dbContext.UserPermissionProfile.DeleteOnSubmit(userProfile);
        }

        /// <summary>
        /// Weist dem Benutzer die übergebenen Kontaktinformationen zu.
        /// </summary>
        /// <param name="contact">Kontakt, der dem Benutzer zugewiesen werden soll.</param>
        /// <param name="dbContext">Datenbankkontext.</param>
        public void AddContact(Contact contact, KVSEntities dbContext)
        {
            if (this.Contact != null)
            {
                throw new Exception("Der Benutzer besitzt bereits Kontaktinformationen.");
            }

            dbContext.WriteLogItem("Kontaktinformationen zu Benutzer " + this.Login + " hinzugefügt.", LogTypes.INSERT, this.Id, "User", contact.Id);
            this.Contact = contact;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        partial void OnCreated()
        {
            this.EntityState = Database.EntityState.New;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        partial void OnLoaded()
        {
            this.EntityState = Database.EntityState.Loaded;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            if (action == System.Data.Linq.ChangeAction.Insert)
            {
                using (KVSEntities dbContext = new KVSEntities())
                {
                    if (dbContext.User.Any(q => q.Login == this.Login))
                    {
                        throw new Exception("Dieser Login ist bereits vergeben.");
                    }
                }
            }
            else if (action == System.Data.Linq.ChangeAction.Delete)
            {
                throw new Exception("Benutzer können nicht gelöscht werden.");
            }
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        partial void OnPasswordChanging(string value)
        {
            if (this.Id != 0)
            {
                this.LogDBContext.WriteLogItem("Passwort wurde geändert.", LogTypes.UPDATE, this.Id, "User");
            }
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        partial void OnIsLockedChanging(bool value)
        {
            this.WriteUpdateLogItem("Gesperrt", this.IsLocked, value);
        }
    }
}
