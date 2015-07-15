using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
namespace KVSWebApplication.Product
{    
    /// <summary>
    /// Verwaltngsmaske fuer die Produkte
    /// </summary>
    public partial class Product_Details : System.Web.UI.Page
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
        protected void Page_Load(object sender, System.EventArgs e)
        {
            thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(((Guid)Session["CurrentUserId"])));
            if (!Page.IsPostBack)
            {
                if (thisUserPermissions.Contains("PRODUKTE_UEBERSICHT") || thisUserPermissions.Contains("PRODUKTE_SPERREN") || thisUserPermissions.Contains("PRODUKTE_ANLEGEN") || thisUserPermissions.Contains("PRODUKTE_BEARBEITEN"))
                {
                    AddTab("Produktübersicht", "AllProducts");
                    AddPageView(RadTabStrip1.FindTabByValue("AllProducts"));
                    AddTab("Kundenpreisliste", "CustomerProducts");
                    AddPageView(RadTabStrip1.FindTabByValue("CustomerProducts"));
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