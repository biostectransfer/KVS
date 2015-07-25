using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.Data.SqlClient;
using System.Data;
namespace KVSWebApplication
{
    /// <summary>
    /// Codebehind fuer die Maske ChangeLog Technisch
    /// </summary>
    public partial class ChangeLog : System.Web.UI.Page
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

            if (!thisUserPermissions.Contains("CHANGELOG"))
            {
                Response.Redirect("../AccessDenied.aspx");
            }
        }
        protected void AllChangesLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities con = new KVSEntities();
            var changes = from chang in con.ChangeLog orderby chang.Expr1 descending
                          select chang;
            e.Result = changes;
        }
        protected void RadGridAllChanges_DetailTableDataBind(object source, GridNeedDataSourceEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            try
            {
                RadGrid sender = source as RadGrid;
                Panel item = sender.Parent as Panel;
                Label lblReferenceId = item.FindControl("lblReferenceId") as Label;
                Label lblTableName = item.FindControl("lblTableName") as Label;
                DataTable t = new DataTable();
                if (lblReferenceId.Text != string.Empty || lblTableName.Text != string.Empty)
                {
                    var tableColumns = dbContext.ChangeLogColumNames.SingleOrDefault(q => q.TableName == lblTableName.Text);
                    string thisSelect = "";
                    if (tableColumns != null)
                    {
                        string idName = tableColumns.IdColumnName;
                        foreach (var col in tableColumns.ColumnNames.Split(';'))
                        {
                            if (thisSelect == string.Empty)
                            {
                                thisSelect = "[" + col + "]";
                            }
                            else
                            {
                                thisSelect += ",[" + col + "]";
                            }
                        }
                        dbContext.Connection.Close();
                        string myQuery = "";
                        if (thisSelect != string.Empty && idName != string.Empty && lblTableName.Text != string.Empty)
                        {
                            myQuery = "select " + thisSelect + " from [" + lblTableName.Text + "] where " + idName + " ='" + lblReferenceId.Text + "'";
                            using (SqlConnection c = new SqlConnection(dbContext.Connection.ConnectionString))
                            {
                                c.Open();
                                using (SqlDataAdapter a = new SqlDataAdapter(myQuery, c))
                                {
                                    a.Fill(t);
                                }
                            }
                            sender.DataSource = t;
                        }
                    }
                    else
                    {
                        setEmptyDataSet(((RadGrid)sender));
                    }
                }
                else
                {
                    setEmptyDataSet(((RadGrid)sender));
                }
            }
            catch (Exception ex)
            {
                dbContext.WriteLogItem("Fehler beim generieren des ChangeLogs Details. " + ex.Message, LogTypes.ERROR);
                dbContext.SubmitChanges();
            }         
        }
        protected void RadGridAllChanges_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            try
            {
                GridDataItem item = (GridDataItem)e.DetailTableView.ParentItem;
                string TableName = item["TableName"].Text.ToString();
                string ReferenceId = item["ReferenceId"].Text.ToString();
                GridDataItem dataItem = item;
                GridNestedViewItem nestedItem = (GridNestedViewItem)dataItem.ChildItem;
                RadGrid radGrdEnquiriesVarients = (RadGrid)nestedItem.FindControl("radGridDetailInfo");
                DataTable t = new DataTable();
                if (ReferenceId != string.Empty || TableName != string.Empty)
                {
                    var tableColumns = dbContext.ChangeLogColumNames.SingleOrDefault(q => q.TableName == TableName);
                    string thisSelect = "";
                    if (tableColumns != null)
                    {
                        string idName = tableColumns.IdColumnName;
                        foreach (var col in tableColumns.ColumnNames.Split(';'))
                        {
                            if (thisSelect == string.Empty)
                            {
                                thisSelect = "[" + col + "]";
                            }
                            else
                            {
                                thisSelect += ",[" + col + "]";
                            }
                        }
                        dbContext.Connection.Close();
                        string myQuery = "";
                        if (thisSelect != string.Empty && idName != string.Empty && TableName != string.Empty)
                        {
                            myQuery = "select " + thisSelect + " from [" + TableName + "] where " + idName + " ='" + ReferenceId + "'";
                            using (SqlConnection c = new SqlConnection(dbContext.Connection.ConnectionString))
                            {
                                c.Open();
                                using (SqlDataAdapter a = new SqlDataAdapter(myQuery, c))
                                {
                                    a.Fill(t);
                                }
                            }
                            radGrdEnquiriesVarients.DataSource = t;
                            radGrdEnquiriesVarients.DataBind();
                        }
                    }
                    else
                    {
                        setEmptyDataSet(radGrdEnquiriesVarients, true);
                    }
                }
                else
                {
                    setEmptyDataSet(radGrdEnquiriesVarients, true);
                }
            }
            catch (Exception ex)
            {
                dbContext.WriteLogItem("Fehler beim generieren des ChangeLogs Details. " + ex.Message, LogTypes.ERROR);
                dbContext.SubmitChanges();
            }          
        }
        protected void setEmptyDataSet(RadGrid grid, bool isNotNeedEvent=false)
        {
            KVSEntities dbContext = new KVSEntities();
            DataTable t = new DataTable();
            string  myQuery = "select  'Keine Datensätze vorhanden' as Info";
            using (SqlConnection c = new SqlConnection(dbContext.Connection.ConnectionString))
            {
                c.Open();
                using (SqlDataAdapter a = new SqlDataAdapter(myQuery, c))
                {
                    a.Fill(t);
                }
            }
            grid.DataSource = t;
            if (isNotNeedEvent)
                grid.DataBind();           
        }
    }
}