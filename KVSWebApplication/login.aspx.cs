using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using KVSCommon.Database;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Configuration;
using KVSCommon.Database;
using System.Web.Security;
namespace KVSWebApplication
{
    /// <summary>
    /// Codebehind fuer die Login/Logout Maske
    /// </summary>
    public partial class login : System.Web.UI.Page
    {
        private void KillSession()
        {
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie 
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            FormsAuthentication.SignOut();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["logout"] != null)
            {
                Session["CurrentUserId"] = "";
                Session["CurrentUserName"] = "";
                KillSession();
            }
            if (Session["CurrentUserId"] == null || String.IsNullOrEmpty(Session["CurrentUserId"].ToString()))
            {
                Session["CurrentUserId"] = "";
                Session["CurrentUserName"] = "";
                SetMenuItems(false);
            }
            else
            {
                if (FormsAuthentication.Authenticate("newdirectionAdmin", "CaseDirectory"))
                {
                    FormsAuthentication.RedirectFromLoginPage("newdirectionAdmin", true);
                }
                SetMenuItems(true);
                RadAjaxPanel1.Redirect("Search/search.aspx");
            }
        }

        protected void OnLoggedIn(object sender, EventArgs e)
        {
            if (Session["CurrentUserId"] == null || String.IsNullOrEmpty(Session["CurrentUserId"].ToString()))
            {
                SetMenuItems(true);
                Response.Redirect("Search/search.aspx");
            }
            else
            {
                SetMenuItems(false);
                RadAjaxPanel1.Redirect(Request.RawUrl);
            }
        }

        protected void OnAuthenticate(object sender, AuthenticateEventArgs e)
        {
            bool Authenticated = false;
            try
            {
                using (KVSEntities dbContext = new KVSEntities())
                {
                    var myId = KVSCommon.Database.User.Logon((Login2.FindControl("UserName") as TextBox).Text, (Login2.FindControl("Password") as TextBox).Text);
                    Session["CurrentUserId"] = myId;
                    Session["CurrentUserName"] = KVSCommon.Database.User.GetUserNamebyId(myId);

                }
                Authenticated = true;
            }
            catch (Exception ex)
            {
                StreamWriter myFile = new StreamWriter(@ConfigurationManager.AppSettings["LogFilePath"] + "loginError.txt", true);
                myFile.Write(ex.Message);
                myFile.Close();
                Authenticated = false;
            }
            e.Authenticated = Authenticated;
        }
        private void SetMenuItems(bool value)
        {
            RadMenu myMene = (RadMenu)Master.FindControl("RadMenu1");
            HtmlGenericControl myLeftDiv = (HtmlGenericControl)Master.FindControl("leftcolumn");
            myLeftDiv.Visible = value;
            myMene.Enabled = value;
            myMene.Visible = value;
        }
    }
}