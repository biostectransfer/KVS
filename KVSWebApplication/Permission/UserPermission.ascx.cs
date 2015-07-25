using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using KVSCommon.Database;
namespace KVSWebApplication.Permission
{
    /// <summary>
    /// Codebehind fuer die Maske Benutzerrechte
    /// </summary>
    public partial class UserPermission : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void getAllUserPermissionDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from user in dbContext.User
                        orderby user.Person.Name
                        select new
                        {
                            user.Id,
                            user.Login,
                            user.Person.Name,
                            user.Person.FirstName,
                            user.Contact.Email
                        };
            e.Result = query;
        }
        protected void GetAllPermissionDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from permissions in dbContext.Permission
                        where permissions.UserPermission.Any(q => q.UserId == Int32.Parse(e.WhereParameters["Id"].ToString())) == false
                        orderby permissions.Name
                        select new { permissions.Id, permissions.Name, permissions.Description };
            e.Result = query;
        }
        protected void UserPermissionDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from userPermissions in dbContext.UserPermission
                        join per in dbContext.Permission on userPermissions.PermissionId equals per.Id
                        where userPermissions.UserId == Int32.Parse(e.WhereParameters["Id"].ToString())
                        orderby per.Name
                        select new { per.Id, per.Name, per.Description };
            e.Result = query;
        }
        protected void GetAllPermissionProfileDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from permissions in dbContext.PermissionProfile
                        where permissions.UserPermissionProfile.Any(q => q.UserId == Int32.Parse(e.WhereParameters["Id"].ToString())) == false
                        orderby permissions.Name
                        select new { permissions.Id, permissions.Name, permissions.Description };
            e.Result = query;
        }
        protected void UserPermissionProfileDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from userPermissions in dbContext.UserPermissionProfile
                        join per in dbContext.PermissionProfile on userPermissions.PermissionProfileId equals per.Id
                        where userPermissions.UserId == Int32.Parse(e.WhereParameters["Id"].ToString())
                        orderby per.Name
                        select new { per.Id, per.Name, per.Description };
            e.Result = query;
        }
        protected void lbsUserPermissions_ItemDataBound(object sender, RadListBoxItemEventArgs e)
        {
            e.Item.ToolTip = (string)DataBinder.Eval(e.Item.DataItem, "Description");
        }
        protected void AddedUserPermission_ItemDataBound(object sender, RadListBoxItemEventArgs e)
        {
            e.Item.ToolTip = (string)DataBinder.Eval(e.Item.DataItem, "Description");
        }
        protected void lbsUserPermissionsProfile_ItemDataBound(object sender, RadListBoxItemEventArgs e)
        {
            e.Item.ToolTip = (string)DataBinder.Eval(e.Item.DataItem, "Description");
        }
        protected void AddedUserPermissionsProfile_ItemDataBound(object sender, RadListBoxItemEventArgs e)
        {
            e.Item.ToolTip = (string)DataBinder.Eval(e.Item.DataItem, "Description");
        }
        protected void rbtSavePermissions_Click(object sender, EventArgs e)
        {
            KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
            try
            {
                RadListBoxItemCollection AddedPermission = ((RadListBox)((RadButton)sender).Parent.FindControl("AddedUserPermission")).Items;
                RadListBoxItemCollection AllPermissions = ((RadListBox)((RadButton)sender).Parent.FindControl("lbsUserPermissions")).Items;
                var userId = Int32.Parse(((RadButton)sender).CommandArgument.ToString());
                var myUserPermissions = dbContext.User.SingleOrDefault(q => q.Id == userId);
                foreach (RadListBoxItem permission in AllPermissions)
                {
                    if (myUserPermissions.UserPermission.SingleOrDefault(q => q.PermissionId == Int32.Parse(permission.Value) && q.UserId == userId) != null)
                    {
                        myUserPermissions.RemovePermission(Int32.Parse(permission.Value), dbContext);
                    }
                }
                foreach (RadListBoxItem addItem in AddedPermission)
                {
                    if (myUserPermissions.UserPermission.SingleOrDefault(q => q.PermissionId == Int32.Parse(addItem.Value) && q.UserId == userId) == null)
                    {
                        myUserPermissions.AddPermission(Int32.Parse(addItem.Value), dbContext);
                    }
                }
                dbContext.SubmitChanges();
                getAllUserPermission.EditIndexes.Clear();
                getAllUserPermission.MasterTableView.IsItemInserted = false;
                getAllUserPermission.MasterTableView.Rebind();
            }
            catch (Exception ex)
            {
                RadWindowManagerAllUserPermission.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                try
                {
                    dbContext.WriteLogItem("UserPermission Error " + ex.Message, LogTypes.ERROR, "UserPermission");
                    dbContext.SubmitChanges();
                }
                catch { }
            }
        }
        protected void rbtSavePermissionProfile_Click(object sender, EventArgs e)
        {
            KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
            try
            {
                RadListBoxItemCollection AddedPermission = ((RadListBox)((RadButton)sender).Parent.FindControl("lsbPermissionProfilePermission")).Items;
                RadListBoxItemCollection AllPermissions = ((RadListBox)((RadButton)sender).Parent.FindControl("lsbAllPermissionProfile")).Items;
                var userId = Int32.Parse(((RadButton)sender).CommandArgument.ToString());
                var myUserPermissions = dbContext.User.SingleOrDefault(q => q.Id == userId);
                foreach (RadListBoxItem permission in AllPermissions)
                {
                    if (myUserPermissions.UserPermissionProfile.SingleOrDefault(q => q.PermissionProfileId == Int32.Parse(permission.Value) && q.UserId == userId) != null)
                    {
                        myUserPermissions.RemovePermissionProfile(Int32.Parse(permission.Value), dbContext);
                    }
                }
                foreach (RadListBoxItem addItem in AddedPermission)
                {
                    if (myUserPermissions.UserPermissionProfile.SingleOrDefault(q => q.PermissionProfileId == Int32.Parse(addItem.Value) && q.UserId == userId) == null)
                    {
                        myUserPermissions.AddPermissionProfile(Int32.Parse(addItem.Value), dbContext);
                    }
                }
                dbContext.SubmitChanges();
                getAllUserPermission.EditIndexes.Clear();
                getAllUserPermission.MasterTableView.IsItemInserted = false;
                getAllUserPermission.MasterTableView.Rebind();
            }
            catch (Exception ex)
            {
                RadWindowManagerAllUserPermission.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                try
                {
                    dbContext.WriteLogItem("UserPermission Error " + ex.Message, LogTypes.ERROR, "UserPermission");
                    dbContext.SubmitChanges();
                }
                catch { }
            }
        }
    }
}