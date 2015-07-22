using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.IO;
using System.Configuration;
using System.Transactions;
using KVSCommon.Enums;

namespace KVSWebApplication.Nachbearbeitung_Abmeldung
{
    /// <summary>
    /// Codebehind Reiter Versand Abmeldung
    /// </summary>
    public partial class Versand : System.Web.UI.UserControl
    {
        RadScriptManager script = null;
        protected void RadGridVersand_PreRender(object sender, EventArgs e)
        {
            HideExpandColumnRecursive(RadGridVersand.MasterTableView);
        }
        public void HideExpandColumnRecursive(GridTableView tableView)
        {
            GridItem[] nestedViewItems = tableView.GetItems(GridItemType.NestedView);
            foreach (GridNestedViewItem nestedViewItem in nestedViewItems)
            {
                foreach (GridTableView nestedView in nestedViewItem.NestedTableViews)
                {
                    nestedView.ParentItem.Expanded = true;
                    HideExpandColumnRecursive(nestedView);
                }
            }
        } 
        protected void Page_Load(object sender, EventArgs e)
        {
            string target = Request["__EVENTTARGET"];

            if(!String.IsNullOrEmpty(target))
                if (!target.Contains("LieferscheinDruckenButton") && !target.Contains("EditOffenColumn") && !target.Contains("DispatchOrderNumberBox") && !target.Contains("isSelfDispathCheckBox") && !target.Contains("EditButton")) 
                RadGridVersand.Rebind();
            NachbearbeitungAbmeldung auftragNeu = Page as NachbearbeitungAbmeldung;
            script = auftragNeu.getScriptManager() as RadScriptManager;
        }
        protected void VersandLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var versandQuery = from packList in con.PackingList
                               let CustomerNameLet = con.Order.Where(q => q.PackingListNumber == packList.PackingListNumber).FirstOrDefault()
                               orderby packList.PackingListNumber descending
                               select new
                               {
                                   listId = packList.PackingListNumber,
                                   OrderNumber = CustomerNameLet != null ? CustomerNameLet.OrderNumber : (int?)null,
                                   CustomerName = 
                                    CustomerNameLet.Customer.SmallCustomer != null &&
                                        CustomerNameLet.Customer.SmallCustomer.Person != null ?
                                        CustomerNameLet.Customer.SmallCustomer.Person.FirstName + " " +
                                        CustomerNameLet.Customer.SmallCustomer.Person.Name : CustomerNameLet.Customer.Name, 
                                   Order = CustomerNameLet,
                                   listNumber = packList.PackingListNumber,
                                   isPrinted = packList.IsPrinted == true ? "Ja" : "Nein",
                                   PostBackUrl = packList.Document.FileName == null ? "" : "<a href=" + '\u0022' + packList.Document.FileName + '\u0022' + " target=" + '\u0022' + "_blank" + '\u0022' + "> Lieferschein " + packList.PackingListNumber + " öffnen</a>",
                                   DispatchOrderNumber = (packList != null) ? packList.DispatchOrderNumber : "",
                                   IsSelf = (packList != null && packList.IsSelfDispatch.HasValue) ? packList.IsSelfDispatch.Value : false,                                   
                               };

            versandQuery = versandQuery.Where(q => q.Order.OrderType.Id != (int)OrderTypes.Admission && q.Order.Status == (int)OrderStatusTypes.Closed);

            e.Result = versandQuery;
        }
        protected void OrdersDetailedTabel_DetailTable(object source, GridDetailTableDataBindEventArgs e)
        {
            var dbContext = new DataClasses1DataContext();
            var _item = (GridDataItem)e.DetailTableView.ParentItem;
            var listId = Int32.Parse(_item["listId"].Text);
            var orderQuery = from ord in dbContext.Order
                             where ord.PackingListNumber == listId && ord.Status == (int)OrderStatusTypes.Closed && 
                             ord.HasError.GetValueOrDefault(false) != true
                             select new
                             {
                                 OrderNumber = ord.OrderNumber,
                                 CustomerName = 
                                 ord.Customer.SmallCustomer != null &&
                                        ord.Customer.SmallCustomer.Person != null ?
                                        ord.Customer.SmallCustomer.Person.FirstName + " " +
                                        ord.Customer.SmallCustomer.Person.Name : ord.Customer.Name, 
                                 OrderLocation = ord.Location.Name,
                                 Status = ord.OrderStatus.Name,
                                 OrderType = ord.OrderType.Name,
                                 OrderError = ord.HasError == true ? "Ja" : "Nein"
                             };
            var item = (GridDataItem)e.DetailTableView.ParentItem;
            var nestedItem = (GridNestedViewItem)item.ChildItem;
            var radGrdEnquiriesVarients = (RadGrid)nestedItem.FindControl("RadGridVersandDetails");
            radGrdEnquiriesVarients.DataSource = orderQuery;
            radGrdEnquiriesVarients.DataBind();
        }
        protected void OrdersDetailedTabel_DetailTableDataBind(object source, GridNeedDataSourceEventArgs e)
        {
            var dbContext = new DataClasses1DataContext();
            var sender = source as RadGrid;
            var item = sender.Parent as Panel;
            var mylistId = item.FindControl("listIdBox") as TextBox;
            if (!String.IsNullOrEmpty(mylistId.Text))
            {
                var listId = Int32.Parse(mylistId.Text);
                var orderQuery = from ord in dbContext.Order
                                 where ord.PackingListNumber == listId && ord.Status == (int)OrderStatusTypes.Closed && ord.HasError.GetValueOrDefault(false) != true
                                 select new
                                 {
                                     OrderNumber = ord.OrderNumber,
                                     CustomerName = 
                                      ord.Customer.SmallCustomer != null &&
                                        ord.Customer.SmallCustomer.Person != null ?
                                        ord.Customer.SmallCustomer.Person.FirstName + " " +
                                        ord.Customer.SmallCustomer.Person.Name : ord.Customer.Name, 
                                     OrderLocation = ord.Location.Name,
                                     Status = ord.OrderStatus.Name,
                                     OrderType = ord.OrderType.Name,
                                     OrderError = ord.HasError == true ? "Ja" : "Nein"
                                 };
                sender.DataSource = orderQuery;
            }
        }
        protected void PruefenButton_Clicked(object sender, GridCommandEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var item = e.Item as GridDataItem;
                item.Selected = true;
                itemIndexHiddenField.Value = item.ItemIndex.ToString();
                var myListId = Int32.Parse(item["listId"].Text);
                var dbContext = new DataClasses1DataContext();
                var myVerbringung = dbContext.PackingList.SingleOrDefault(q => q.PackingListNumber == myListId);
                if (myVerbringung.IsSelfDispatch == true)
                {
                    myDispathHiddenField.Value = "true";
                }
                else
                {
                    myDispathHiddenField.Value = myVerbringung.DispatchOrderNumber;
                }
            }
        }
        protected void btnRemovePackingList_Click(object sender, EventArgs e)
        {
            using (var ts = new TransactionScope())
            {
                var dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                try
                {
                    var btnsender = sender as Button;
                    Label lblPackingListId = null;
                    if(btnsender != null)
                    {
                        lblPackingListId = btnsender.Parent.FindControl("lbllistId") as Label;
                    }
                    if (lblPackingListId != null && !String.IsNullOrEmpty(lblPackingListId.Text))
                    {
                        Order.TryToRemovePackingListIdAndSetStateToRegistration(dbContext, Int32.Parse(lblPackingListId.Text));
                        ts.Complete();
                    }
                    else
                    {
                        throw new Exception("Achtung, die Auftragsnummer konnte nicht identifiziert werden. Bitte aktualisieren Sie die Seite oder wenden Sie sich an den Administrator");
                    }
                }
                catch (Exception ex)
                {
                    if (ts != null)
                        ts.Dispose();

                    ErrorVersandLabel.Text = ex.Message;
                    ErrorVersandLabel.Visible = true;
                        try
                        {
                            dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                            dbContext.WriteLogItem("btnRemovePackingList_Click Error " + ex.Message, LogTypes.ERROR, "Order");
                            dbContext.SubmitChanges();
                        }
                        catch { }

                }
            }
            RadGridVersand.Rebind();

        }
        protected void DrueckenButton_Clicked(object sender, EventArgs e)
        {
            ErrorVersandLabel.Visible = false;
                var ms = new MemoryStream();
                var editButton = sender as Button;
                var item = editButton.Parent as Panel;
                var isSelfDispathCheckBox = item.FindControl("isSelfDispathCheckBox") as CheckBox;
                var DispatchOrderNumberBox = item.FindControl("DispatchOrderNumberBox") as RadTextBox;
                var ErrorVersandGedrucktLabel = item.FindControl("ErrorVersandGedrucktLabel") as Label;
                var _listId = item.FindControl("listIdBox") as TextBox;
                var listId = Int32.Parse(_listId.Text);
                var dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                var packList = dbContext.PackingList.SingleOrDefault(q => q.PackingListNumber == listId);
                if (packList.IsPrinted == true)
                {
                    ErrorVersandGedrucktLabel.Visible = true;
                }
                else
                {
                    ErrorVersandGedrucktLabel.Visible = false;
                    packList.LogDBContext = dbContext;
                    try
                    {
                        if (isSelfDispathCheckBox.Checked == true && String.IsNullOrEmpty(DispatchOrderNumberBox.Text))
                        {
                            packList.IsSelfDispatch = true;
                        }
                        else if (isSelfDispathCheckBox.Checked == false && !String.IsNullOrEmpty(DispatchOrderNumberBox.Text))
                        {
                            packList.DispatchOrderNumber = DispatchOrderNumberBox.Text;
                            packList.IsSelfDispatch = false;
                        }
                        else
                        {
                            packList.DispatchOrderNumber = DispatchOrderNumberBox.Text;
                            packList.IsSelfDispatch = false;
                        }
                        dbContext.SubmitChanges();
                    }
                    catch(Exception ex)
                    {
                        ErrorVersandLabel.Text = ex.Message;
                        ErrorVersandLabel.Visible = true;
                    }
                    try
                    {
                        string myPackListFileName = EmptyStringIfNull.CheckIfFolderExistsAndReturnPathForPdf(Session["CurrentUserId"].ToString());
                        string myPathToSave = myPackListFileName;
                        packList.Print(ms, string.Empty, dbContext, "/UserData/" + Session["CurrentUserId"].ToString() + "/" + Path.GetFileName(myPackListFileName), false);
                        string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
                        string host = ConfigurationManager.AppSettings["smtpHost"];
                        packList.SendByEmail(ms, fromEmail, host);
                        File.WriteAllBytes(myPathToSave, ms.ToArray());
                        dbContext.SubmitChanges();
                        RadGridVersand.DataBind();    
                    }
                    catch(Exception ex)
                    {
                        ErrorVersandLabel.Text = ex.Message;
                        ErrorVersandLabel.Visible = true;
                    }   
            }
        }
        protected void RadAjaxManager1_AjaxSettingCreated(object sender, AjaxSettingCreatedEventArgs e)
        {
            if (e.Updated.ClientID == panel11.ClientID)
            {
                e.UpdatePanel.ChildrenAsTriggers = false;
            }
        }
        protected void DispatchOrderNumberBoxText_Changed(object sender, EventArgs e)
        {
            RadTextBox dispachBox = sender as RadTextBox;
            CheckBox isSelf = dispachBox.Parent.FindControl("isSelfDispathCheckBox") as CheckBox;
            Button drucke = dispachBox.Parent.FindControl("LieferscheinDruckenButton") as Button;
            script.RegisterPostBackControl(drucke);
            if (!String.IsNullOrEmpty(dispachBox.Text))
            {
                isSelf.Checked = false;
                isSelf.Enabled = false;
                dispachBox.Enabled = true;
            }
            else
            {
                isSelf.Checked = true;
                isSelf.Enabled = true;
                dispachBox.Enabled = false;
            }
        }
        protected void isSelfDispathCheckBox_Checked(object sender, EventArgs e)
        {
            CheckBox isSelf = sender as CheckBox;
            RadTextBox dispachBox = isSelf.Parent.FindControl("DispatchOrderNumberBox") as RadTextBox;
            Button drucke = dispachBox.Parent.FindControl("LieferscheinDruckenButton") as Button;
            script.RegisterPostBackControl(drucke);
            if (isSelf.Checked == true)
            {
                dispachBox.Text = "";
                dispachBox.Enabled = false;
            }
            else
            {
                dispachBox.Text = "";
                dispachBox.Enabled = true;
            }
        }
    }
}