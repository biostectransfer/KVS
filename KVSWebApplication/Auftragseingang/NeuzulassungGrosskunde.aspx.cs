using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.IO;
using System.Configuration;
using KVSWebApplication.BasePages;
using System.Web.Http;
using KVSCommon.Managers;

namespace KVSWebApplication.Auftragseingang
{
    /// <summary>
    /// Hauptmaske Neuzulassung Grosskunde
    /// </summary>
    public partial class NeuzulassungGrosskunde : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public RadScriptManager getScriptManager()
        {
            return RadScriptManager1;
        }
    }
}