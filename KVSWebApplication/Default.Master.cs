using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Security;

namespace KVSWebApplication
{
    /// <summary>
    /// Default Maske
    /// </summary>
    public partial class Default : System.Web.UI.MasterPage
    {

        private string CurrentPostionName
        {
            get
            {
                object pn = ViewState["PosName"];
                if (pn != null)
                {
                    return pn.ToString();
                }
                else
                {
                    return string.Empty;
                }

            }
            set
            {
                if (ViewState["PosName"] == null || ViewState["PosName"] == string.Empty)
                    ViewState["PosName"] = KVSCommon.Database.PathPosition.GetPathPostions(new KVSCommon.Database.KVSEntities());
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentPostionName = "";
            if (ViewState["PosName"] != null || ViewState["PosName"] != string.Empty)
            {
                Dictionary<string, string> dict = ViewState["PosName"] as Dictionary<string, string>;
                string path = Request.Path;
                if (dict.ContainsKey(path))
                {
                    lblPostionPath.Text = dict[path];
                    lblPosName.Visible = true;
                }
                else
                    lblPosName.Visible = false;
            }
            if (Session["CurrentUserName"] != null)
            {



                if (Session["CurrentUserName"].ToString() != string.Empty)
                {
                    lblLoginUsername.Text = Session["CurrentUserName"].ToString();
                    FormsAuthentication.SetAuthCookie("newdirectionAdmin", false);
                }
                else
                    lblLoginUsername.Text = "Gast";

            }

        }
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Request.Url.OriginalString.ToLower().Contains("login.aspx"))
            {
                check();
            }

        }
        public void check()
        {

            string basePath = ConfigurationManager.AppSettings["BaseUrl"];

            if (Session["CurrentUserId"] == null || Session["CurrentUserId"].ToString() == "" || Session["CurrentUserId"] == null)
            {
                Session["CurrentUserId"] = "";
                Session["CurrentUserName"] = "";

                Response.Redirect(basePath + "login.aspx");
                if (FormsAuthentication.Authenticate("", ""))
                {
                    FormsAuthentication.RedirectFromLoginPage("", false);
                }
            }
            else
            {
                Session["CurrentUserId"] = Session["CurrentUserId"];
                Session["CurrentUserName"] = Session["CurrentUserName"];
            }
        }
    }
}