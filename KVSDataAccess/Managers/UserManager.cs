using KVSCommon.Database;
using KVSCommon.Entities;
using KVSCommon.Enums;
using KVSCommon.Managers;
using KVSCommon.Utility;
using KVSDataAccess.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSDataAccess.Managers
{
    public partial class UserManager : EntityManager<User, int>, IUserManager
    {
        public UserManager(IKVSEntities context) : base(context) { }
        
        /// <summary>
        /// Versucht, sich mit den gegebenen Login-Daten im System einuzloggen.
        /// </summary>
        /// <param name="login">Benutzername des Benutzers.</param>
        /// <param name="password">Passwort des Benutzers.</param>
        /// <returns>Datenbank-Id des Benutzers, falls Logon erfolgreich.</returns>
        public User Logon(string login, string password)
        {
            var user = DataContext.GetSet<User>().FirstOrDefault(q => q.Login == login);
            if (user == null)
            {
                throw new Exception("Benutzer / Passwort konnte nicht verifiziert werden.");
            }

            if (user.IsLocked)
            {
                throw new Exception("Der Benutzerzugang ist gesperrt.");
            }

            SaltedHash sh = new SaltedHash();
            if (sh.VerifyHashString(password, user.Password, user.Salt))
            {
                DataContext.LogUserId = user.Id;
                user.LastLogin = DateTime.Now;
                DataContext.WriteLogItem("Benutzer " + login + " eingeloggt.", LogTypes.INFO, user.Id, "User");
                SaveChanges();
                return user;
            }
            else
            {
                throw new Exception("Benutzer / Passwort konnte nicht verifiziert werden.");
            }
        }
        
        /// <summary>
        /// Gibt zurueck alle Berechtigungen für den angegebenen Benutzer.
        /// </summary>
        /// <param name="userId">ID des Benutzers.</param>
        /// <returns>Eine Liste mit Berechtigungen.</returns>
        public List<int> GetAllPermissionsByID(int userId)
        {            
            var userPermissions = new List<int>();
            var user = DataContext.GetSet<User>().FirstOrDefault(o => o.Id == userId);

            if (user != null)
            {
                userPermissions = GetAllPermissionsByID(user);
            }

            return userPermissions;
        }

        /// <summary>
        /// Gibt zurueck alle Berechtigungen für den angegebenen Benutzer.
        /// </summary>
        /// <param name="user">Benutzer.</param>
        /// <returns>Eine Liste mit Berechtigungen.</returns>
        public List<int> GetAllPermissionsByID(User user)
        {
            var userPermissions = new List<int>();

            foreach (var permission in user.UserPermission)
            {
                userPermissions.Add(permission.PermissionId);
            }

            if (user.UserPermissionProfile != null)
            {
                foreach (var profile in user.UserPermissionProfile)
                {
                    foreach (var permission in profile.PermissionProfile.PermissionProfilePermission.Select(o => o.PermissionId))
                        if (userPermissions.Contains(permission) == false)
                        {
                            userPermissions.Add(permission);
                        }
                }
            }

            return userPermissions;
        }
        
        /// <summary>
        /// Überprüft Berechtigungen für den angegebenen Benutzer.
        /// </summary>
        public bool CheckPermissionsForUser(User user, PermissionTypes permission)
        {
            var permissions = GetAllPermissionsByID(user);

            return permissions.Contains((int)permission);
        }
        
        /// <summary>
        /// Überprüft Berechtigungen für den angegebenen Benutzer.
        /// </summary>
        public bool CheckPermissionsForUser(object userPermission, PermissionTypes permission)
        {
            var result = false;

            if(userPermission != null && userPermission is IEnumerable<int>)
            {
                return ((IEnumerable<int>)userPermission).Contains((int)permission);
            }

            return result;
        }
    }
}
