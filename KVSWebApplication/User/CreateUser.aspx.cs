using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.Collections;
namespace KVSWebApplication.User
{    
    /// <summary>
    /// Codebehind fuer das erstellen eines neuen Users
    /// </summary>
    public partial class CreateUser : System.Web.UI.Page
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
            thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(Int32.Parse(Session["CurrentUserId"].ToString())));
            if (!thisUserPermissions.Contains("BENUTZER_ANLEGEN"))
            {
                rbtcreateUser.Enabled = false;
            }
            if (!thisUserPermissions.Contains("BENUTZER_BEARBEITEN"))
            {
                getAllUser.Columns[0].Visible = false;
            }
            if (!thisUserPermissions.Contains("ADMIN_PASSWORT_AENDERN"))
            {
                getAllUser.Columns[1].Visible = false;
            }
        }
        protected void getAllUser_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandSource is ImageButton)
            {
                if (((ImageButton)e.CommandSource).CommandName == "reversePWD" && ((ImageButton)e.CommandSource).ID == "gbcDeleteColumn")
                {
                    RadWindow_ChangePWD.VisibleOnPageLoad = true;
                    RadWindow_ChangePWD.Width = Unit.Parse(350.ToString());
                    RadWindow_ChangePWD.Height = Unit.Parse(180.ToString());
                    if (e.Item is GridDataItem)
                    {
                        GridDataItem newPWD = e.Item as GridDataItem;
                        if (newPWD["Id"].Text != string.Empty)
                        {
                            ViewState["userToChange"] = newPWD["Id"].Text;
                            RadWindow_ChangePWD.Title = "Passwort zurücksetzen: " + newPWD["Login"].Text;
                        }
                    }
                }
                else
                {
                    RadWindow_ChangePWD.VisibleOnPageLoad = false;
                }
            }
        }
        protected void ChangeSaveBtn_Click(object sender, EventArgs e)
        {
            KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())); // hier kommt die Loggingid
            try
            {
                if (txbNewPassword.Text == txbRepeatPWD.Text && ViewState["userToChange"] != null)
                {
                    if (ViewState["userToChange"].ToString() != string.Empty)
                    {
                        var thisUser = dbContext.User.SingleOrDefault(q => q.Id == Int32.Parse(ViewState["userToChange"].ToString()));
                        if (thisUser != null)
                        {
                            thisUser.ChangePassword(txbNewPassword.Text, dbContext);
                            dbContext.SubmitChanges();
                            RadWindow_ChangePWD.Title = "Passwort zurücksetzen";
                            RadWindow_ChangePWD.VisibleOnPageLoad = false;
                            RadWindowManagerCreateUser.RadAlert("Das Passwort wurde erfolgreich zurückgesetzt", 380, 180, "Info", "");
                        }
                        else
                        {
                            Response.Redirect("../login.aspx");
                        }
                    }
                    else
                    {
                        throw new Exception("Es ist ein Fehler unterlaufen, bitte aktualisieren Sie die Seite");
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
        protected void bSaveClick(object sender, EventArgs e)
        {
            ResetErrorLabels();
            if (checkFields() == true)
            {
                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                try
                {                   
                    var createUser = KVSCommon.Database.User.CreateUser(txbUserLogin.Text, txbUserPassword1.Text, txbUserNachname.Text, txbUserVorname.Text, txbUserTitle.Text, dbContext);
                    if (hasContactData() == true)
                    {
                        var createContact = Contact.CreateContact(txbUserPhone.Text, txbUserFax.Text, txbUserMobile.Text, txbUserEmail.Text, dbContext);
                        createUser.Contact = createContact;
                    }
                    dbContext.SubmitChanges();
                    RadWindowManagerCreateUser.RadAlert("Der Benutzer wurde erfolgreich angelegt!", 380, 180, "Info", "");
                    txbUserLogin.Text=""; txbUserPassword1.Text=""; txbUserNachname.Text="";
                    txbUserVorname.Text = ""; txbUserTitle.Text = "";
                    txbUserPhone.Text = ""; txbUserFax.Text = ""; txbUserMobile.Text = ""; txbUserEmail.Text = "";
                }
                catch (Exception ex)
                {
                    RadWindowManagerCreateUser.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                    try
                    {
                        dbContext.WriteLogItem("Create User Error " + ex.Message, LogTypes.ERROR, "User");
                        dbContext.SubmitChanges();
                    }
                    catch { }
                }
            }
        }
        protected void ShowUserTab(object sender, EventArgs e)
        {
            ShowUserPanel.Visible = true;
            CreateUserPanel.Visible = false;
        }
        protected void CreateUserTab(object sender, EventArgs e)
        {
            ShowUserPanel.Visible = false;
            CreateUserPanel.Visible = true;
        }
        /// <summary>
        /// Gibt die Mastergrid zurück (Informationen zur allen Benutzern)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void getAllUserDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from user in dbContext.User
                        select new
                        {
                            user.Id,
                            user.PersonId,
                            user.ContactId,
                            user.Login,
                            user.Person.Name,
                            user.Person.FirstName,
                            user.Person.Title,
                            user.Contact.Phone,
                            user.Contact.Fax,
                            user.Contact.MobilePhone,
                            user.Contact.Email,
                            Locked = user.IsLocked
                        };
            e.Result = query;
        }
        protected void getAllUser_EditCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            Hashtable newValues = new Hashtable();
            ((GridEditableItem)e.Item).ExtractValues(newValues);
            KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())); // hier kommt die Loggingid
            try
            {
                var userDataUpdate = dbContext.User.SingleOrDefault(q => q.Id == Int32.Parse(newValues["Id"].ToString()));              
                    if (userDataUpdate != null && userDataUpdate.Contact == null)
                    {
                        var addContact = Contact.CreateContact(EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Phone"]), EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Fax"]),
                            EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["MobilePhone"]), EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Email"]), dbContext);
                        userDataUpdate.Contact = addContact;
                        dbContext.SubmitChanges();
                    }
                    else if (userDataUpdate != null && userDataUpdate.Person != null && userDataUpdate.Contact != null)
                    {
                        userDataUpdate.LogDBContext = dbContext;
                        if (newValues["Login"].ToString() == null || newValues["Login"].ToString() == string.Empty)
                            throw new Exception("Das Login Feld darf nicht leer sein!");                        
                        userDataUpdate.Login = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Login"]);
                        userDataUpdate.Person.LogDBContext = dbContext;
                        if(newValues["Name"] == null || newValues["FirstName"] == null)
                            throw new Exception("Das Feld Vor/Nachname darf nicht leer sein!");
                        userDataUpdate.Person.Name = newValues["Name"].ToString();
                        userDataUpdate.Person.FirstName = newValues["FirstName"].ToString();
                        userDataUpdate.Person.Title = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Title"]);
                        userDataUpdate.Contact.LogDBContext = dbContext;
                        userDataUpdate.Contact.Phone = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Phone"]);
                        userDataUpdate.Contact.Fax = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Fax"]);
                        userDataUpdate.Contact.MobilePhone = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["MobilePhone"]);
                        userDataUpdate.Contact.Email = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Email"]);
                        userDataUpdate.IsLocked = (bool)newValues["Locked"];
                        dbContext.SubmitChanges();
                        e.Canceled = true;
                    }
                    else
                    {
                        throw new Exception("Fehler beim ändern der Userdaten, bitte Informieren Sie den Support!"); 
                    }
                    getAllUser.EditIndexes.Clear();
                    getAllUser.MasterTableView.IsItemInserted = false;
                    getAllUser.MasterTableView.Rebind();              
            }
            catch (Exception ex)
            {
                RadWindowManagerCreateUser.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                try
                {
                    dbContext.WriteLogItem("Update User Error " + ex.Message, LogTypes.ERROR, "User");
                    dbContext.SubmitChanges();
                }
                catch { }
            }
        }
        protected void getAllUser_Init(object sender, System.EventArgs e)
        {
            GridFilterMenu menu = getAllUser.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "Ist gleich" || menu.Items[i].Text == "Ist ungleich" || menu.Items[i].Text == "Ist größer als" || menu.Items[i].Text == "Ist kleiner als" || menu.Items[i].Text == "Ist größer gleich" || menu.Items[i].Text == "Ist kleiner gleich" || menu.Items[i].Text == "Enthält" || menu.Items[i].Text == "Kein Filter")
                {
                    i++;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }
        protected void ResetErrorLabels()
        {
            lblUserEmailError.Text = "";
            lblUserFaxError.Text = "";
            lblUserLoginError.Text = "";
            lblUserMobileError.Text = "";
            lblUserNachnameError.Text = "";
            lblUserPassword1Error.Text = "";
            lblUserPassword2Error.Text = "";
            lblUserPhoneError.Text = "";
            lblUserVornameError.Text = "";
            lblUserLoginError.Text = "";
        }
        protected bool checkFields()
        {
            bool check = true;
            if (rbtcreateUser.Checked == true)
            {
                if (txbUserLogin.Text == string.Empty)
                {
                    lblUserLoginError.Text = "Bitte den Benutzernamen festlegen";
                    check = false;
                }
                if (txbUserPassword1.Text == string.Empty )
                {
                    lblUserPassword1Error.Text = "Bitte das Passwort eintragen";
                    check = false;
                }
                if (txbUserPassword2.Text == string.Empty)
                {
                    lblUserPassword2Error.Text = "Bitte das Passwort widerholen eintragen";
                    check = false;
                }
                if ((txbUserPassword1.Text != string.Empty || txbUserPassword2.Text != string.Empty) && (txbUserPassword1.Text != txbUserPassword2.Text))
                {
                    lblUserPassword1Error.Text = "Die Passwörter stimmen nicht überein!";
                    lblUserPassword2Error.Text = "Die Passwörter stimmen nicht überein!";
                }
                if (txbUserNachname.Text == string.Empty)
                {
                    lblUserNachnameError.Text = "Bitte den Nachnamen eintragen";
                    check = false;
                }
                if (txbUserVorname.Text == string.Empty)
                {
                    lblUserVornameError.Text = "Bitte den Vornamen eintragen";
                    check = false;
                } 
            }
            return check;
        }
        protected bool hasContactData()
        {
            bool check = false;
            if (txbUserPhone.Text != string.Empty || txbUserFax.Text != string.Empty || txbUserMobile.Text != string.Empty || txbUserEmail.Text != string.Empty)
            {
                check = true;
            }
            return check;            
        }
    }
}