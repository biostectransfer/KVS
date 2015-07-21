using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KVSCommon.Database;
using System.Text.RegularExpressions;
namespace KVSWebApplication.ChangeHistory
{
    /// <summary>
    /// Hilfsklasse fuer das Logging
    /// </summary>
    class OrderLogging
    {
        public int? OrderNumber{ get; set;}
        public string TableName { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string Login { get; set; }
        public DateTime Date { get; set; }
        public int? ReferenceId { get; set; }
        public long LogId { get; set; }
        public string TranslatedText { get; set; }
        public string Type { get; set; }
    }
}
