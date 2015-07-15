using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
namespace KVSWebApplication.Customer
{
    /// <summary>
    /// Codebehind fuer die Reiterverwaltungsmaske
    /// </summary>
    public partial class CreateCustomer : System.Web.UI.Page
    {
        List<string> thisUserPermissions = new List<string>();
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
        protected void Page_Load(object sender, System.EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(((Guid)Session["CurrentUserId"])));
                if (thisUserPermissions.Contains("KUNDEN_ANLEGEN"))
                {
                    AddTab("Kunden erstellen", "AddCustomer");
                    if (RadTabStrip1.Tabs[0].Value == "AddCustomer")
                        AddPageView(RadTabStrip1.FindTabByValue("AddCustomer"));
                }
                if (thisUserPermissions.Contains("KUNDEN_UEBERSICHT") || thisUserPermissions.Contains("KUNDEN_BEARBEITEN"))
                {
                    AddTab("Großkundenübersicht", "LargeCustomerDetails");
                 if (RadTabStrip1.Tabs[0].Value == "LargeCustomerDetails")
                    AddPageView(RadTabStrip1.FindTabByValue("LargeCustomerDetails"));      
                    AddTab("Laufkundschaft", "SmallCustomerDetails");
                 if(RadTabStrip1.Tabs[0].Value == "SmallCustomerDetails")
                        AddPageView(RadTabStrip1.FindTabByValue("SmallCustomerDetails"));
                }
                if (thisUserPermissions.Contains("KOSTENSTELLEN_ANSICHT") || thisUserPermissions.Contains("KOSTENSTELLEN_BEARBEITEN") || thisUserPermissions.Contains("KOSTENSTELLEN_ANLEGEN"))
                {                  
                    AddTab("Kostenstelle", "CostCenter");
                     if(RadTabStrip1.Tabs[0].Value == "CostCenter")
                        AddPageView(RadTabStrip1.FindTabByValue("CostCenter"));                   
                }
                if (thisUserPermissions.Contains("STANDORT_UEBERSICHT") || thisUserPermissions.Contains("STANDORT_BEARBEITEN") || thisUserPermissions.Contains("STANDORT_ANLAGE"))
                {
                    AddTab("Standorte", "Location_Details");
                        if(RadTabStrip1.Tabs[0].Value == "Location_Details")
                            AddPageView(RadTabStrip1.FindTabByValue("Location_Details"));                    
                }
            }
        }    
        private void AddTab(string tabName, string tabValue)
        {
            RadTab tab = new RadTab();
            tab.Text = tabName;
            tab.Value = tabValue;          
            RadTabStrip1.Tabs.Add(tab);
        }
        protected void RadMultiPage1_PageViewCreated(object sender, RadMultiPageEventArgs e)
        {
            string userControlName = e.PageView.ID + ".ascx";
            Control userControl = Page.LoadControl(userControlName);
            userControl.ID = e.PageView.ID + "_userControl";
            e.PageView.Controls.Add(userControl);
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
            AddPageView(e.Tab);
            e.Tab.PageView.Selected = true;
        }
    }
}