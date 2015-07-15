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
    /// Codebehind fuer die Maske Alle Rechte
    /// </summary>
    public partial class AllPermission : System.Web.UI.UserControl
    {
        List<string> thisUserPermissions = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
       
            thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(((Guid)Session["CurrentUserId"])));
            if (!thisUserPermissions.Contains("RECHTEPROFIL_BEARBEITEN"))
            {
                getAllPermission.MasterTableView.CommandItemSettings.ShowAddNewRecordButton = false;
                getAllPermission.MasterTableView.Columns[0].Visible = false;
            }
        }     
        protected void getAllPermissionDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            var query = from permission in dbContext.Permission
                        orderby permission.Name
                        select new
                        {
                            permission.Id,
                            permission.Name,
                            permission.Description
                        };
            e.Result = query;
        }
        protected void getAllPermission_Init(object sender, System.EventArgs e)
        {
            GridFilterMenu menu = getAllPermission.FilterMenu;
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
        protected void getAllPermission_EditCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext(((Guid)Session["CurrentUserId"]));
            Hashtable newValues = new Hashtable();
            ((GridEditableItem)e.Item).ExtractValues(newValues);
            try
            {
                var checkThisPermission = dbContext.Permission.SingleOrDefault(q => q.Id == new Guid(newValues["Id"].ToString()));
                if (checkThisPermission != null)
                {
                    if (newValues["Description"] == null)
                    {
                        throw new Exception("Die Rechtebeschreibung darf nicht leer sein!");
                    }
                    checkThisPermission.LogDBContext = dbContext;
                    checkThisPermission.Description = newValues["Description"].ToString();
                    dbContext.SubmitChanges();
                }
                else
                {
                    throw new Exception("Das Bearbeiten ist nicht möglich!");
                }
            }
            catch (Exception ex)
            {
                RadWindowManagerAllPermission.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                try
                {
                    dbContext.WriteLogItem("Permission Error " + ex.Message, LogTypes.ERROR, "Permission");
                    dbContext.SubmitChanges();
                }
                catch { }
            }
            finally
            {
                getAllPermission.EditIndexes.Clear();
                getAllPermission.MasterTableView.IsItemInserted = false;
                getAllPermission.MasterTableView.Rebind();
            }
        }
    }
}