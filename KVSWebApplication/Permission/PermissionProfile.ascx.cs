using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.Collections;
namespace KVSWebApplication.Permission
{
    /// <summary>
    /// Codebehind fuer die Maske Rechteprofile
    /// </summary>
    public partial class PermissionProfile : System.Web.UI.UserControl
    {
        List<string> thisUserPermissions = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(Int32.Parse(Session["CurrentUserId"].ToString())));
            if (!thisUserPermissions.Contains("RECHTEPROFIL_BEARBEITEN"))
            {
                getAllPermissionProfile.MasterTableView.CommandItemSettings.ShowAddNewRecordButton = false;
                getAllPermissionProfile.MasterTableView.Columns[0].Visible = false;
                getAllPermissionProfile.MasterTableView.Columns[4].Visible = false;
                getAllPermissionProfile.MasterTableView.Columns[5].Visible = false;
            }
        }

        protected void getAllPermissionProfile_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = (GridDataItem)e.Item;
                object myId = item.GetDataKeyValue("Id");

                if (myId != null)
                {
                    using (KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())))
                    {
                        try
                        {
                            var myProfileId = Int32.Parse(item.GetDataKeyValue("Id").ToString());

                            var permProfUsers = dbContext.UserPermissionProfile.Where(q => q.PermissionProfileId == myProfileId);
                            if (permProfUsers != null)
                            {
                                foreach (var permProfUser in permProfUsers)
                                {
                                    dbContext.UserPermissionProfile.DeleteOnSubmit(permProfUser);
                                    dbContext.SubmitChanges();
                                }
                            }

                            var permProfPermitions = dbContext.PermissionProfilePermission.Where(q => q.PermissionProfileId == myProfileId);
                            if (permProfPermitions != null)
                            {
                                foreach (var permProfPermition in permProfPermitions)
                                {
                                    dbContext.PermissionProfilePermission.DeleteOnSubmit(permProfPermition);
                                    dbContext.SubmitChanges();
                                }
                            }


                            var permProf = dbContext.PermissionProfile.SingleOrDefault(q => q.Id == myProfileId);
                            if (permProf != null) //kann gelöscht sein
                            {
                                dbContext.PermissionProfile.DeleteOnSubmit(permProf);
                                dbContext.SubmitChanges();
                            }
                        }

                        catch (Exception ex)
                        {
                            RadWindowManagerAllPermissionProfile.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                            try
                            {
                                dbContext.WriteLogItem("PermissionProfile Error " + ex.Message, LogTypes.ERROR, "PermissionProfile");
                                dbContext.SubmitChanges();
                            }
                            catch { }
                        }
                    }

                    getAllPermissionProfile.Rebind();
                }
            }
        }

        protected void getAllPermissionProfileDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from permission in dbContext.PermissionProfile
                        orderby permission.Name
                        select new
                        {
                            permission.Id,
                            permission.Name,
                            permission.Description
                        };
            e.Result = query;
        }
        protected void getAllPermissionProfile_Init(object sender, System.EventArgs e)
        {
            GridFilterMenu menu = getAllPermissionProfile.FilterMenu;
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
        protected void getAllPermissionProfile_EditCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
            Hashtable newValues = new Hashtable();
            ((GridEditableItem)e.Item).ExtractValues(newValues);
            try
            {
                var checkThisPermission = dbContext.PermissionProfile.SingleOrDefault(q => q.Id == Int32.Parse(newValues["Id"].ToString()));
                if (checkThisPermission != null)
                {
                    if (newValues["Description"] == null || newValues["Name"] == null)
                    {
                        throw new Exception("Die Rechtebeschreibung/Rechtename darf nicht leer sein!");
                    }
                    else
                    {
                        checkThisPermission.LogDBContext = dbContext;
                        checkThisPermission.Description = newValues["Description"].ToString();
                        dbContext.SubmitChanges();
                    }
                    getAllPermissionProfile.EditIndexes.Clear();
                    getAllPermissionProfile.MasterTableView.IsItemInserted = false;
                    getAllPermissionProfile.MasterTableView.Rebind();
                }
                else
                {
                    throw new Exception("Das Recht wurde in der Datenbank nicht gefunden, aktualisieren Sie die Seite und versuchen Sie erneut!");
                }
            }
            catch (Exception ex)
            {
                RadWindowManagerAllPermissionProfile.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                try
                {
                    dbContext.WriteLogItem("PermissionProfile Error " + ex.Message, LogTypes.ERROR, "PermissionProfile");
                    dbContext.SubmitChanges();
                }
                catch { }
            }
        }
        /// <summary>
        /// Gibt alle bekannten Versicherungen zurück
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddedPermissionListboxDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from permission in dbContext.PermissionProfile
                        join pprofile in dbContext.PermissionProfilePermission on permission.Id equals pprofile.PermissionProfileId
                        where permission.Id == Int32.Parse(e.WhereParameters["Id"].ToString())
                        orderby pprofile.Permission.Name
                        select new
                        {
                            Id = pprofile.Permission.Id,
                            Name = pprofile.Permission.Name,
                            pprofile.Permission.Description
                        };
            e.Result = query;
        }
        /// <summary>
        /// Gibt die Versicherungsdaten zurück, die dem gewählten Kunden entsprechen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PermissionsListBoxDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from permission in dbContext.Permission
                        where permission.PermissionProfilePermission.Any(q => q.PermissionProfileId == Int32.Parse(e.WhereParameters["Id"].ToString())) == false
                        orderby permission.Name
                        select new
                        {
                            permission.Id,
                            permission.Name,
                            permission.Description
                        };
            e.Result = query;
        }
        protected void PermissionsListBox_ItemDataBound(object sender, RadListBoxItemEventArgs e)
        {
            e.Item.ToolTip = (string)DataBinder.Eval(e.Item.DataItem, "Description");
        }
        protected void PermissionsAddedListBox_ItemDataBound(object sender, RadListBoxItemEventArgs e)
        {
            e.Item.ToolTip = (string)DataBinder.Eval(e.Item.DataItem, "Description");
        }
        protected void savePermissionPackageClick(object sender, EventArgs e)
        {
            KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())); // hier kommt die Loggingid
            try
            {
                RadListBoxItemCollection AddedPermission = ((RadListBox)((RadButton)sender).Parent.FindControl("AddedPermission")).Items;
                RadListBoxItemCollection AllPermissions = ((RadListBox)((RadButton)sender).Parent.FindControl("Permissions")).Items;
                var permissionProfileId = Int32.Parse(((RadButton)sender).CommandArgument.ToString());
                var permissionClear = dbContext.PermissionProfilePermission.Where(q => q.PermissionProfileId == permissionProfileId);
                var thisPermissionProfile = dbContext.PermissionProfile.SingleOrDefault(q => q.Id == permissionProfileId);
                foreach (RadListBoxItem permission in AllPermissions)
                {
                    if (permissionClear.SingleOrDefault(q => q.PermissionId == Int32.Parse(permission.Value) && q.PermissionProfileId == permissionProfileId) != null)
                    {
                        thisPermissionProfile.RemovePermission(Int32.Parse(permission.Value), dbContext);
                    }
                }
                foreach (RadListBoxItem addItem in AddedPermission)
                {
                    if (permissionClear.SingleOrDefault(q => q.PermissionId == Int32.Parse(addItem.Value) && q.PermissionProfileId == permissionProfileId) == null)
                    {
                        thisPermissionProfile.AddPermission(Int32.Parse(addItem.Value), dbContext);
                    }
                }
                dbContext.SubmitChanges();
                getAllPermissionProfile.EditIndexes.Clear();
                getAllPermissionProfile.MasterTableView.IsItemInserted = false;
                getAllPermissionProfile.MasterTableView.Rebind();
            }
            catch (Exception ex)
            {
                RadWindowManagerAllPermissionProfile.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                try
                {
                    dbContext.WriteLogItem("Fehler beim Versicherungsverweis (savePermissionPackageClick) in der Maske PermissionProfile.cs" + ex.Message, LogTypes.ERROR, "PermissionProfilePermission");
                }
                catch { }
            }
        }
        protected void Grid_InsertCommand(object source, GridCommandEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())); // hier kommt die Loggingid
            try
            {
                Hashtable newValues = new Hashtable();
                ((GridEditableItem)e.Item).ExtractValues(newValues);
                if (newValues["Description"] == null || newValues["Name"] == null)
                {
                    throw new Exception("Die Rechtebeschreibung/Rechtename darf nicht leer sein!");
                }
                var checkRightProfile = dbContext.PermissionProfile.SingleOrDefault(q => q.Name == newValues["Name"].ToString());
                if (checkRightProfile == null)
                {
                    KVSCommon.Database.PermissionProfile.CreatePermissionProfile(newValues["Name"].ToString(), newValues["Description"].ToString(), dbContext);
                    dbContext.SubmitChanges();
                }
                else
                    throw new Exception("Der Rechteprofilname existiert bereits!");
                e.Canceled = true;
                getAllPermissionProfile.EditIndexes.Clear();
                getAllPermissionProfile.MasterTableView.IsItemInserted = false;
                getAllPermissionProfile.MasterTableView.Rebind();
            }
            catch (Exception ex)
            {
                RadWindowManagerAllPermissionProfile.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                try
                {
                    dbContext.WriteLogItem("Fehler beim Versicherungsverweis (Grid_InsertCommand) in der Maske PermissionProfile.cs" + ex.Message, LogTypes.ERROR, "PermissionProfile");
                    dbContext.SubmitChanges();
                }
                catch { }
            }
        }
    }
}