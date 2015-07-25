using KVSCommon.Database;
using KVSCommon.Enums;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IUserManager: IEntityManager<User, int>
    {
        User Logon(string login, string password);
        
        /// <summary>
        /// Gibt zurueck alle Berechtigungen für den angegebenen Benutzer.
        /// </summary>
        /// <param name="userId">ID des Benutzers.</param>
        /// <returns>Eine Liste mit Berechtigungen.</returns>
        List<int> GetAllPermissionsByID(int userId);

        /// <summary>
        /// Gibt zurueck alle Berechtigungen für den angegebenen Benutzer.
        /// </summary>
        /// <param name="user">Benutzer.</param>
        /// <returns>Eine Liste mit Berechtigungen.</returns>
        List<int> GetAllPermissionsByID(User user);
        
        /// <summary>
        /// Überprüft Berechtigungen für den angegebenen Benutzer.
        /// </summary>
        bool CheckPermissionsForUser(User user, PermissionTypes permission);

        /// <summary>
        /// Überprüft Berechtigungen für den angegebenen Benutzer.
        /// </summary>
        bool CheckPermissionsForUser(object userPermission, PermissionTypes permission);
    }
}
