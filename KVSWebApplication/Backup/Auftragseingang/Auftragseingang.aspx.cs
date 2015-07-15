using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
namespace KVSWebApplication.Auftragseingang
{
    public partial class Auftragseingang : System.Web.UI.Page
    {
        PageStatePersister _pers;
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
            if (!Page.IsPostBack)
            {
                AddTab("Zulassung", true);
                RadPageView pageViewZulassung = new RadPageView();
                pageViewZulassung.ID = "AuftragseingangZulassung";
                RadMultiPageAuftragseingang.PageViews.Add(pageViewZulassung);
                AddTab("Abmeldung", true);
                RadPageView pageViewAbmeldung = new RadPageView();
                pageViewAbmeldung.ID = "AuftragseingangAbmeldung";
                RadMultiPageAuftragseingang.PageViews.Add(pageViewAbmeldung);                           
            }       
        }
        private void AddTab(string tabName, bool enabled)
        {
            RadTab tab = new RadTab(tabName);
            tab.Enabled = enabled;
            RadTabStripAuftragseingang.Tabs.Add(tab);
        }
        protected void RadMultiPageAuftragseingang_PageViewCreated(object sender, RadMultiPageEventArgs e)
        {
            Control pageViewContents = LoadControl(e.PageView.ID + ".ascx");
            pageViewContents.ID = e.PageView.ID + "userControl";

            e.PageView.Controls.Add(pageViewContents);
        }
        public RadScriptManager getScriptManager()
        {
            return RadScriptManager1;
        }
    }
}