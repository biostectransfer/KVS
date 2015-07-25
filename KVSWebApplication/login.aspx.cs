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
using System.Web.Security;
using KVSCommon.Managers;
using System.Web.Http;

namespace KVSWebApplication
{
    /// <summary>
    /// Codebehind fuer die Login/Logout Maske
    /// </summary>
    public partial class login : Page
    {
        public login()
        {
            UserManager = (IUserManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IUserManager));
        }

        public IUserManager UserManager { get; set; }

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
                ClearSession();
                KillSession();
            }
            if (Session["CurrentUserId"] == null || String.IsNullOrEmpty(Session["CurrentUserId"].ToString()))
            {
                ClearSession();
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

        private void ClearSession()
        {
            Session["CurrentUserId"] = "";
            Session["CurrentUserName"] = "";
            Session["CurrentUser"] = null;
            Session["UserPermissions"] = null;
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
                var user = UserManager.Logon(
                    (Login2.FindControl("UserName") as TextBox).Text,
                    (Login2.FindControl("Password") as TextBox).Text);

                Session["CurrentUserId"] = user.Id;
                Session["CurrentUserName"] = user.Login;
                Session["CurrentUser"] = user;
                Session["UserPermissions"] = UserManager.GetAllPermissionsByID(user);

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
            RadMenu menu = (RadMenu)Master.FindControl("RadMenu1");
            HtmlGenericControl myLeftDiv = (HtmlGenericControl)Master.FindControl("leftcolumn");
            myLeftDiv.Visible = value;
            menu.Enabled = value;
            menu.Visible = value;
        }
    }
}