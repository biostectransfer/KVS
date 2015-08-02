using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using KVSCommon.Managers;
using System.Web.Http;

namespace KVSWebApplication
{
    /// <summary>
    /// Codebehind fuer die Passwort Ändern Maske
    /// </summary>
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ChangeSaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (txbNewPassword.Text == txbRepeatPWD.Text)
                {
                    var userManager = (IUserManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IUserManager));
                    var user = userManager.GetById(Int32.Parse(Session["CurrentUserId"].ToString()));
                    if (user != null)
                    {
                        userManager.ChangePassword(user, txbNewPassword.Text, txbOldPWD.Text);
                        userManager.SaveChanges();
                        RadWindowManagerChangePassword.RadAlert("Das Passwort wurde erfolgreich geändert", 380, 180, "Info", "");
                    }
                    else
                    {
                        Response.Redirect("../login.aspx");
                    }
                }
                else
                {
                    FailureText.Text = "Die neuen Passwörter stimmen nicht überein";
                }
            }
            catch (Exception ex)
            {
                FailureText.Text = ex.Message;
                //TODO WriteLogItem("ChangePassowrd Error:  " + ex.Message, LogTypes.ERROR, "User");
            }
        }
    }
}