using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
   /// <summary>
   /// Enum fuer die Datenbank Statuse
   /// </summary>
    public enum EntityState
    {
        New = 0,
        Loaded = 1
    }
    /// <summary>
    /// Stellt die Schnitstelle zwischen der Datenbank und dem Code dar
    /// </summary>
    public interface ILogging
    {
        DataClasses1DataContext LogDBContext
        {
            get;
            set;
        }

        object ItemId
        {
            get;
        }

        EntityState EntityState
        {
            get;
            set;
        }
    }
    /// <summary>
    /// Schnittstelle fuer das Logging
    /// </summary>
    public static class ILoggingExtensions
    {
        /// <summary>
        /// Prueft ob ein Logging möglich ist
        /// </summary>
        /// <param name="item"></param>
        public static void CheckLoggingPossible(this ILogging item)
        {
            if (item.LogDBContext == null)
            {
                throw new Exception("DBContext für das Logging ist nicht gesetzt.");
            }
        }

        /// <summary>
        /// Schreibt einen Logeintrag für ein Update eines Properties.
        /// </summary>
        /// <param name="item">Datensatz, der geändert wird.</param>
        /// <param name="fieldName">Name des Properties, das geändert wird.</param>
        /// <param name="valueBefore">Wert vor der Änderung.</param>
        /// <param name="valueAfter">Wert nach der Änderung.</param>
        public static void WriteUpdateLogItem(this ILogging item, string fieldName, object valueBefore, object valueAfter)
        {
            if (item.EntityState == EntityState.New)
            {
                return;
            }

            item.CheckLoggingPossible();
            if (item.ItemId.GetType() != typeof(int))
            {
                throw new Exception("Fehler beim Erstellen des UpdateLog-Eintrags: Die ItemId ist keine Int.");
            }

            item.LogDBContext.WriteLogItem(fieldName + " von '" + valueBefore + "' auf '" + valueAfter + "' geändert.", LogTypes.UPDATE, (int)item.ItemId, item.GetType().Name, fieldName);
        }
    }
}
