using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
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
            DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())); // hier kommt die Loggingid
            try
            {
                if (txbNewPassword.Text == txbRepeatPWD.Text)
                {
                    var thisUser = dbContext.User.SingleOrDefault(q => q.Id == Int32.Parse(Session["CurrentUserId"].ToString()));
                    if (thisUser != null)
                    {
                        thisUser.ChangePassword(txbNewPassword.Text, txbOldPWD.Text, dbContext);
                        dbContext.SubmitChanges();
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
                dbContext.WriteLogItem("ChangePassowrd Error:  " + ex.Message, LogTypes.ERROR, "User");
            }
        }
    }
}