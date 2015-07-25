using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Klasse für die Pfadverwaltung
    /// </summary>
    public partial class PathPosition
    {
        /// <summary>
        /// Gibt alle Pfadpositionen zurueck
        /// </summary>
        /// <param name="dbContext">Datenbank Kontext</param>
        /// <returns>Dictionary<string, string></returns>
        public static Dictionary<string, string> GetPathPostions(KVSEntities dbContext)
        {
            Dictionary<string, string> positions = new Dictionary<string, string>();
            var pos = from pp in dbContext.PathPosition select pp;
            foreach (var items in pos)
            {
                if (!positions.ContainsKey(items.Path))
                {
                    positions.Add(items.Path, items.PostionName);
                }
            }
            return positions;

        }

    }
}
