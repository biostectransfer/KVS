using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using KVSCommon.Database;
using System.Data;
namespace KVSWebApplication.Nachbearbeitung_Abmeldung
{
    /// <summary>
    /// Reiterverwaltung fuer die Abmeldung
    /// </summary>
    public partial class NachbearbeitungAbmeldung : System.Web.UI.Page
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
        public string largeOrSmallCust { get; set; }
        public string customerId { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                AddTab("AuftragsbearbeitungAbmeldung", "Offen");
                RadPageView pageViewOffen = new RadPageView();
                pageViewOffen.ID = "AuftragsbearbeitungAbmeldung";
                RadMultiPage1.PageViews.Add(pageViewOffen);
                AddTab("ZulassungNachbearbeitung", "Zulassungsstelle");
                if (RadTabStrip1.Tabs[0].Value == "Zulassungsstelle")
                    AddPageView(RadTabStrip1.FindTabByValue("Zulassungsstelle"));
                AddTab("LieferscheinAbmeldung", "Lieferschein");
                if (RadTabStrip1.Tabs[0].Value == "Lieferschein")
                    AddPageView(RadTabStrip1.FindTabByValue("Lieferschein"));
                AddTab("Versand", "Versand");
                if (RadTabStrip1.Tabs[0].Value == "Versand")
                    AddPageView(RadTabStrip1.FindTabByValue("Versand"));
                AddTab("Fehlerhaft", "Fehlerhaft");
                if (RadTabStrip1.Tabs[0].Value == "Fehlerhaft")
                    AddPageView(RadTabStrip1.FindTabByValue("Fehlerhaft"));      
            }
            if (Session["orderStatusSearch"] != null && Session["customerIdSearch"] != null && Session["customerIndexSearch"] != null)
            { 
                if(Session["orderStatusSearch"].ToString().Contains("Offen"))
                {
                    Session["CustomerId"] = Session["customerIdSearch"];
                    Session["CustomerIndex"] = Session["customerIndexSearch"];
                    RadTab offenTab = RadTabStrip1.Tabs.FindTabByText("Offen");
                    offenTab.Selected = true;
                    Session["customerIdSearch"] = null;
                    RadMultiPage1.SelectedIndex = 0;
                }
                else if (Session["orderStatusSearch"].ToString().Contains("Zulassungsstelle"))
                {
                    Session["CustomerId"] = Session["customerIdSearch"];
                    Session["CustomerIndex"] = Session["customerIndexSearch"];
                    RadTab offenTab = RadTabStrip1.Tabs.FindTabByText("Zulassungsstelle");
                    offenTab.Selected = true;
                    Session["customerIdSearch"] = null;
                    RadMultiPage1.SelectedIndex = 1;
                }
                else if (Session["orderStatusSearch"].ToString().Contains("Error"))
                {
                    Session["CustomerId"] = Session["customerIdSearch"];
                    Session["CustomerIndex"] = Session["customerIndexSearch"];
                    RadTab offenTab = RadTabStrip1.Tabs.FindTabByText("Fehlerhaft");
                    offenTab.Selected = true;
                    Session["customerIdSearch"] = null;
                    RadMultiPage1.SelectedIndex = 4;
                }
            }
        }
        private void AddTab(string tabValue, string tabName)
        {
            RadTab tab = new RadTab(tabName);
            if (tab.Value == "AuftragsbearbeitungAbmeldung")
            {
                tab.PostBack = false;
            }
            tab.Text = tabName;
            tab.Value = tabValue;
            RadTabStrip1.Tabs.Add(tab);
        }
        protected void RadMultiPage1_PageViewCreated(object sender, RadMultiPageEventArgs e)
        {
            Control pageViewContents = LoadControl(e.PageView.ID + ".ascx");
            pageViewContents.ID = e.PageView.ID + "userControl";
            e.PageView.Controls.Add(pageViewContents);
        }
        private void AddPageView(RadTab tab)
        {
            RadPageView pageView = new RadPageView();
            pageView.ID = tab.Value;
            RadMultiPage1.PageViews.Add(pageView);
            tab.PageViewID = pageView.ID;
        }
        protected void RadTabStrip1_TabClick(object sender, RadTabStripEventArgs e)
        {
            if (e.Tab.Value != "AuftragsbearbeitungAbmeldung")
            {
                AddPageView(e.Tab);
                e.Tab.PageView.Selected = true;
            }
        }
        public RadScriptManager getScriptManager()
        {
            return RadScriptManager1;
        }
    }
}