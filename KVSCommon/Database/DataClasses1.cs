using System;
using System.Web;
using System.Configuration;

namespace KVSCommon.Database
{
    public partial class DataClasses1DataContext
    {
        public DataClasses1DataContext()
            : base(ConfigurationManager.ConnectionStrings["KVSConnectionString"].ConnectionString)
        {
        }

        public DataClasses1DataContext(int logUserId)
            : this()
        {
            this.LogUserId = logUserId;
        }

        internal int? LogUserId
        {
            get;
            set;
        }
                
        /// <summary>
        /// Fьgt dem Datenbankkontext einen Logeintrag hinzu.
        /// </summary>
        /// <param name="logText">Text fьr den Logeintrag.</param>
        /// <param name="type">Typ des Logeintrages.</param>
        /// <param name="tableName">Tabellenname des betroffenen Datensatzes.</param>
        public void WriteLogItem(string logText, LogTypes type, string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new Exception("Der Tabellenname darf nicht leer sein.");
            }

            //TODO
            this.Systemlog.InsertOnSubmit(this.GetLogItem(logText, type, (int?)null, tableName, null, null));
        }
        
        partial void OnCreated()
        {
            if (!this.LogUserId.HasValue)
            {
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Session["UserId"] != null)
                    {
                        int logUserId;
                        if (Int32.TryParse(HttpContext.Current.Session["UserId"].ToString(), out logUserId))
                        {
                            this.LogUserId = logUserId;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fьgt dem Datenbankkontext einen Logeintrag hinzu.
        /// </summary>
        /// <param name="logText">Text fьr den Logeintrag.</param>
        /// <param name="type">Typ des Logeintrages.</param>
        public void WriteLogItem(string logText, LogTypes type)
        {
            //TODO
            this.Systemlog.InsertOnSubmit(this.GetLogItem(logText, type, (int?)null, null, null, (int?)null));
        }

        /// <summary>
        /// Fьgt dem Datenbankkontext einen Logeintrag hinzu.
        /// </summary>
        /// <param name="logText">Text fьr den Logeintrag.</param>
        /// <param name="type">Typ des Logeintrages.</param>
        /// <param name="referenceId">Id des betroffenen Datensatzes.</param>
        /// <param name="tableName">Tabellenname des betroffenen Datensatzes.</param>
        /// <param name="propertyName">Name des betroffenen Feldes des Datensatzes.</param>
        /// <param name="childReferenceId">Id des untergeordneten Datensatzes.</param>
        public void WriteLogItem(string logText, LogTypes type, int referenceId, string tableName, string propertyName, int childReferenceId)
        {
            this.Systemlog.InsertOnSubmit(this.GetLogItem(logText, type, referenceId, tableName, propertyName, childReferenceId));
        }
        
        /// <summary>
        /// Fьgt dem Datenbankkontext einen Logeintrag hinzu.
        /// </summary>
        /// <param name="logText">Text fьr den Logeintrag.</param>
        /// <param name="type">Typ des Logeintrages.</param>
        /// <param name="referenceId">Id des betroffenen Datensatzes.</param>
        /// <param name="tableName">Tabellenname des betroffenen Datensatzes.</param>
        public void WriteLogItem(string logText, LogTypes type, int referenceId, string tableName)
        {
            if (referenceId == 0)
            {
                throw new Exception("Die ReferenceId darf nicht leer sein.");
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new Exception("Der Tabellenname darf nicht leer sein.");
            }

            //TODO this.Systemlog.InsertOnSubmit(this.GetLogItem(logText, type, referenceId, tableName, null, null));
        }

        /// <summary>
        /// Fьgt dem Datenbankkontext einen Logeintrag hinzu.
        /// </summary>
        /// <param name="logText">Text fьr den Logeintrag.</param>
        /// <param name="type">Typ des Logeintrages.</param>
        /// <param name="referenceId">Id des betroffenen Datensatzes.</param>
        /// <param name="tableName">Tabellenname des betroffenen Datensatzes.</param>
        /// <param name="childReferenceId">Id des untergeordneten Datensatzes.</param>
        public void WriteLogItem(string logText, LogTypes type, int referenceId, string tableName, int childReferenceId)
        {
            if (referenceId == 0)
            {
                throw new Exception("Die ReferenceId darf nicht leer sein.");
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new Exception("Der Tabellenname darf nicht leer sein.");
            }

            if (childReferenceId == 0)
            {
                throw new Exception("Die ChildReferenceId darf nicht leer sein.");
            }

            //TODO this.Systemlog.InsertOnSubmit(this.GetLogItem(logText, type, referenceId, tableName, null, childReferenceId));
        }

        /// <summary>
        /// Fьgt dem Datenbankkontext einen Logeintrag hinzu.
        /// </summary>
        /// <param name="logText">Text fьr den Logeintrag.</param>
        /// <param name="type">Typ des Logeintrages.</param>
        /// <param name="referenceId">Id des betroffenen Datensatzes.</param>
        /// <param name="tableName">Tabellenname des betroffenen Datensatzes.</param>
        /// <param name="propertyName">Name des betroffenen Feldes des Datensatzes.</param>
        public void WriteLogItem(string logText, LogTypes type, int referenceId, string tableName, string propertyName)
        {
            if (referenceId == 0)
            {
                throw new Exception("Die ReferenceId darf nicht leer sein.");
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new Exception("Der Tabellenname darf nicht leer sein.");
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new Exception("Der Feldname darf nicht leer sein.");
            }

            //TODO this.Systemlog.InsertOnSubmit(this.GetLogItem(logText, type, referenceId, tableName, propertyName, null));
        }
        
        /// <summary>
        /// Erstellt einen neuen Logeintrag.
        /// </summary>
        /// <param name="logText">Der Text des Logeintrags.</param>
        /// <param name="type">Der Typ des Logeintrages.</param>
        /// <param name="referenceId">Id des betroffenen Datensatzes.</param>
        /// <param name="tableName">Name der betroffenen Tabelle.</param>
        /// <param name="propertyName">Name des betroffenen Properties.</param>
        /// <param name="childReferenceId">Id des betroffenen untergeordneten Datensatzes.</param>
        /// <returns>Den Logeintrag.</returns>
        internal Systemlog GetLogItem(string logText, LogTypes type, int? referenceId, string tableName, string propertyName, int? childReferenceId)
        {
            if (this.LogUserId.HasValue == false)
            {
                throw new Exception("LogUserId wurde nicht gesetzt.");
            }

            if (string.IsNullOrEmpty(logText))
            {
                throw new Exception("Der LogText darf nicht leer sein.");
            }

            return new Systemlog()
            {
                LogUserId = this.LogUserId.Value,
                Date = DateTime.Now,
                Text = logText,
                LogTypeId = (int)type,
                TableName = tableName,
                TableProperty = propertyName,
                //TODO ReferenceId = referenceId,
                //ChildReferenceId = childReferenceId
            };
        }
    }
}
