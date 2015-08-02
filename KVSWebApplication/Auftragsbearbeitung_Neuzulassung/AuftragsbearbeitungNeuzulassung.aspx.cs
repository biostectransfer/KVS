using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using KVSCommon.Database;

namespace KVSWebApplication.Auftragsbearbeitung_Neuzulassung
{
    public partial class AuftragsbearbeitungNeuzulassung : System.Web.UI.Page
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
                AddTab("OffenNeuzulassung", "Offen");
                RadPageView offenPageView = new RadPageView();
                offenPageView.ID = "OffenNeuzulassung";
                RadMultiPageNeuzulassung.PageViews.Add(offenPageView);


                AddTab("Zulassungsstelle", "Zulassungsstelle");
                if (RadTabStripNeuzulassung.Tabs[0].Value == "Zulassungsstelle")
                    AddPageView(RadTabStripNeuzulassung.FindTabByValue("Zulassungsstelle"));

                AddTab("Lieferscheine", "Lieferscheine");
                if (RadTabStripNeuzulassung.Tabs[0].Value == "Lieferscheine")
                    AddPageView(RadTabStripNeuzulassung.FindTabByValue("Lieferscheine"));

                AddTab("VersandZulassung", "Versand");
                if (RadTabStripNeuzulassung.Tabs[0].Value == "Versand")
                    AddPageView(RadTabStripNeuzulassung.FindTabByValue("Versand"));

                AddTab("FehlerhaftZulassung", "Fehlerhaft");
                if (RadTabStripNeuzulassung.Tabs[0].Value == "Fehlerhaft")
                    AddPageView(RadTabStripNeuzulassung.FindTabByValue("Fehlerhaft"));
            }

            if (Session["orderStatusSearch"] != null && Session["customerIdSearch"] != null && Session["customerIndexSearch"] != null)
            {
                if (Session["orderStatusSearch"].ToString().Contains("Offen"))
                {
                    Session["CustomerId"] = Session["customerIdSearch"];
                    Session["CustomerIndex"] = Session["customerIndexSearch"];
                    RadTab offenTab = RadTabStripNeuzulassung.Tabs.FindTabByText("Offen");
                    offenTab.Selected = true;
                    Session["customerIdSearch"] = null;
                    RadMultiPageNeuzulassung.SelectedIndex = 0;
                }

                else if (Session["orderStatusSearch"].ToString().Contains("Zulassungsstelle"))
                {
                    Session["CustomerId"] = Session["customerIdSearch"];
                    Session["CustomerIndex"] = Session["customerIndexSearch"];
                    RadTab offenTab = RadTabStripNeuzulassung.Tabs.FindTabByText("Zulassungsstelle");
                    offenTab.Selected = true;

                    Session["customerIdSearch"] = null;
                    RadMultiPageNeuzulassung.SelectedIndex = 1;
                }

                else if(Session["orderStatusSearch"].ToString().Contains("Error"))
                {
                    Session["CustomerId"] = Session["customerIdSearch"];
                    Session["CustomerIndex"] = Session["customerIndexSearch"];
                    RadTab offenTab = RadTabStripNeuzulassung.Tabs.FindTabByText("Fehlerhaft");
                    offenTab.Selected = true;
                    Session["customerIdSearch"] = null;
                    RadMultiPageNeuzulassung.SelectedIndex = 4;
                }
            }
        }

        private void AddTab(string tabValue, string tabName)
        {
            RadTab tab = new RadTab(tabName);
            if (tab.Value == "OffenNeuzulassung")
            {
                tab.PostBack = false;
            }         
            tab.Text = tabName;
            tab.Value = tabValue;
            RadTabStripNeuzulassung.Tabs.Add(tab);
        }

        protected void RadMultiPageNeuzulassung_PageViewCreated(object sender, RadMultiPageEventArgs e)
        {         
            Control pageViewContents = LoadControl(e.PageView.ID + ".ascx");
            pageViewContents.ID = e.PageView.ID + "userControl";
            e.PageView.Controls.Add(pageViewContents);                                     
        }

        private void AddPageView(RadTab tab)
        {
            RadPageView pageView = new RadPageView();
            pageView.ID = tab.Value;
            RadMultiPageNeuzulassung.PageViews.Add(pageView);

            tab.PageViewID = pageView.ID;
        }
     

        protected void RadTabStrip1_TabClick(object sender, RadTabStripEventArgs e)
        {
            if (e.Tab.Value != "OffenNeuzulassung")
            {
                AddPageView(e.Tab);
                e.Tab.PageView.Selected = true;
            }          
        }

        public RadScriptManager getScriptManager()
        {
            return RadScriptManager1;
        }

        public RadTabStrip getRadTabStrip()
        {
            return RadTabStripNeuzulassung;
        }

        public RadAjaxManager getRadAjaxManager1()
        {
            return RadAjaxManager1;
        }
    }
}