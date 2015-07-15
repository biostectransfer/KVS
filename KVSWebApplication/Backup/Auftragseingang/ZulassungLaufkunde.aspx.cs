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
namespace KVSWebApplication.Auftragseingang
{
    /// <summary>
    /// Neuzulassung Laufkunde Hauptmaske
    /// </summary>
    public partial class ZulassungLaufkunde : System.Web.UI.Page
    {
        PageStatePersister _pers;
        List<Control> controls = new List<Control>();
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                if (_pers == null)
                {
                    _pers = new SessionPageStatePersister(Page);
                }
                return _pers;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        public RadScriptManager getScriptManager()
        {
            return RadScriptManager1;
        }
    }
}