using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;

namespace KVSWebApplication.Abrechnung
{
    /// <summary>
    /// Codebehind fuer Hauptmaske  Reiter Abrechung
    /// </summary>
    public partial class Abrechnung : System.Web.UI.Page
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
        List<string> thisUserPermissions = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(((Guid)Session["CurrentUserId"])));

                if (thisUserPermissions.Contains("RECHNUNG_BEARBEITEN"))
                {
                    AddTab("Rechnung Speichern", true);
                    RadPageView pageViewAbrechnungSave = new RadPageView();
                    pageViewAbrechnungSave.ID = "AbrechnungSave";
                    RadMultiPageAbrechnung.PageViews.Add(pageViewAbrechnungSave);
                }
                if (thisUserPermissions.Contains("RECHNUNG_ERSTELLEN"))
                {   
                    AddTab("Rechnung Erstellen", true);
                    RadPageView pageViewErstellen = new RadPageView();
                    pageViewErstellen.ID = "AbrechnungErstellen";
                    RadMultiPageAbrechnung.PageViews.Add(pageViewErstellen); 
                }
                if (thisUserPermissions.Contains("RECHNUNG_BEARBEITEN"))
                {
                    AddTab("Stornierte Rechnungen", true);
                    RadPageView CanceledInvoice = new RadPageView();
                    CanceledInvoice.ID = "StornierteRechnungen";
                    RadMultiPageAbrechnung.PageViews.Add(CanceledInvoice);
                }
                if (thisUserPermissions.Contains("RECHNUNGSLAUF"))
                {
                    AddTab("Rechnungslauf", true);
                    RadPageView InvoiceRun = new RadPageView();
                    InvoiceRun.ID = "InvoiceRun";
                    RadMultiPageAbrechnung.PageViews.Add(InvoiceRun);
                }
            }
        }
        /// <summary>
        /// Fuegt einen neuen Tab hinzu
        /// </summary>
        /// <param name="tabName">Tab Name</param>
        /// <param name="enabled">Aktiv</param>
        private void AddTab(string tabName, bool enabled)
        {
            RadTab tab = new RadTab(tabName);
            tab.Enabled = enabled;
            RadTabStripAbrechnung.Tabs.Add(tab);
        }
        /// <summary>
        /// Event fuer die Seiten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadMultiPageAbrechnung_PageViewCreated(object sender, RadMultiPageEventArgs e)
        {
            Control pageViewContents = LoadControl(e.PageView.ID + ".ascx");
            pageViewContents.ID = e.PageView.ID + "userControl";
            e.PageView.Controls.Add(pageViewContents);
        }

        public RadScriptManager getScriptManager()
        {
            return RadScriptManager1;
        }

        public RadAjaxPanel getRadAjaxPanel()
        {
            return RadAjaxPanel1;
        }
    }
}