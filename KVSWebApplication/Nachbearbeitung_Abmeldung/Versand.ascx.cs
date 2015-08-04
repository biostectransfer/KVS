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
using System.Collections;
using KVSWebApplication.BasePages;

namespace KVSWebApplication.Nachbearbeitung_Abmeldung
{
    /// <summary>
    /// Codebehind Reiter Versand Abmeldung
    /// </summary>
    public partial class Versand : EditOrdersBase
    {
        #region Members  

        protected override RadGrid OrderGrid { get { return this.RadGridVersand; } }
        protected override RadDatePicker RegistrationDatePicker { get { return null; } }
        protected override RadComboBox CustomerTypeDropDown { get { return null; } }
        protected override RadComboBox CustomerDropDown { get { return null; } }
        protected override PermissionTypes PagePermission { get { return PermissionTypes.LOESCHEN_AUFTRAGSPOSITION; } }
        protected override OrderTypes OrderType { get { return OrderTypes.Cancellation; } }
        protected override OrderStatusTypes OrderStatusType { get { return OrderStatusTypes.Closed; } }
        protected override string OrderStatusSearch { get { return "Offen"; } }
        #endregion

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            string target = Request["__EVENTTARGET"];

            if (!String.IsNullOrEmpty(target))
                if (!target.Contains("LieferscheinDruckenButton") &&
                    !target.Contains("EditOffenColumn") && !target.Contains("DispatchOrderNumberBox") &&
                    !target.Contains("isSelfDispathCheckBox") && !target.Contains("EditButton"))
                    RadGridVersand.Rebind();
            NachbearbeitungAbmeldung auftragNeu = Page as NachbearbeitungAbmeldung;
            script = auftragNeu.getScriptManager() as RadScriptManager;
        }

        protected void RadGridVersand_PreRender(object sender, EventArgs e)
        {
            HideExpandColumnRecursive(RadGridVersand.MasterTableView);
        }

        protected void VersandLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            var orders = OrderManager.GetEntities(o =>
                o.OrderTypeId == (int)OrderTypes.Cancellation &&
                o.Status == (int)OrderStatusTypes.Closed &&
                o.PackingListNumber.HasValue).OrderByDescending(o => o.PackingListNumber).Select(ord => new
                {
                    listId = ord.PackingListNumber,
                    CustomerName = ord.Customer.SmallCustomer != null &&
                        ord.Customer.SmallCustomer.Person != null ? ord.Customer.SmallCustomer.Person.FirstName + " " + ord.Customer.SmallCustomer.Person.Name :
                        ord.Customer.Name,
                    Order = ord,
                    listNumber = ord.PackingListNumber,
                    isPrinted = ord.PackingList.IsPrinted == true ? "Ja" : "Nein",
                    PostBackUrl = ord.PackingList.Document.FileName == null ? "" : "<a href=" + '\u0022' + ord.PackingList.Document.FileName + '\u0022' + " target=" + '\u0022' + "_blank" +
                        '\u0022' + "> Lieferschein " + ord.PackingList.Id + " öffnen</a>",
                    DispatchOrderNumber = ord.PackingList.DispatchOrderNumber,
                    IsSelf = ord.PackingList.IsSelfDispatch.HasValue ? ord.PackingList.IsSelfDispatch.Value : false,
                });

            e.Result = orders;
        }

        protected void OrdersDetailedTabel_DetailTable(object source, GridDetailTableDataBindEventArgs e)
        {
            var item = (GridDataItem)e.DetailTableView.ParentItem;
            var nestedItem = (GridNestedViewItem)item.ChildItem;
            var radGrdEnquiriesVarients = (RadGrid)nestedItem.FindControl("RadGridVersandDetails");
            radGrdEnquiriesVarients.DataSource = GetOrders(item["listId"].Text);
            radGrdEnquiriesVarients.DataBind();
        }

        protected void OrdersDetailedTabel_DetailTableDataBind(object source, GridNeedDataSourceEventArgs e)
        {
            var sender = source as RadGrid;
            var item = sender.Parent as Panel;
            var packagingListNumberStr = item.FindControl("listIdBox") as TextBox;

            if (!String.IsNullOrEmpty(packagingListNumberStr.Text))
            {
                sender.DataSource = GetOrders(packagingListNumberStr.Text);
            }
        }

        protected void PruefenButton_Clicked(object sender, GridCommandEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var item = e.Item as GridDataItem;
                item.Selected = true;
                itemIndexHiddenField.Value = item.ItemIndex.ToString();
                var packagingListNumber = Int32.Parse(item["listId"].Text);

                var myVerbringung = PackingListManager.GetById(packagingListNumber);
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
            try
            {
                var btnsender = sender as Button;
                Label lblPackingListId = null;
                if (btnsender != null)
                {
                    lblPackingListId = btnsender.Parent.FindControl("lbllistId") as Label;
                }
                if (lblPackingListId != null && !String.IsNullOrEmpty(lblPackingListId.Text))
                {
                    OrderManager.TryToRemovePackingListIdAndSetStateToRegistration(Int32.Parse(lblPackingListId.Text));
                }
                else
                {
                    throw new Exception("Achtung, die Auftragsnummer konnte nicht identifiziert werden. Bitte aktualisieren Sie die Seite oder wenden Sie sich an den Administrator");
                }
            }
            catch (Exception ex)
            {
                ErrorVersandLabel.Text = ex.Message;
                ErrorVersandLabel.Visible = true;

                //TODO WriteLogItem("btnRemovePackingList_Click Error " + ex.Message, LogTypes.ERROR, "Order");
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
            var listId = item.FindControl("listIdBox") as TextBox;
            var packagingListNumber = Int32.Parse(listId.Text);

            var packList = PackingListManager.GetById(packagingListNumber);
            if (packList.IsPrinted == true)
            {
                ErrorVersandGedrucktLabel.Visible = true;
            }
            else
            {
                ErrorVersandGedrucktLabel.Visible = false;

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

                    PackingListManager.SaveChanges();

                    string myPackListFileName = EmptyStringIfNull.CheckIfFolderExistsAndReturnPathForPdf(Session["CurrentUserId"].ToString());
                    string myPathToSave = myPackListFileName;

                    PackingListManager.Print(packList, ms, string.Empty, "/UserData/" + Session["CurrentUserId"].ToString() + "/" + Path.GetFileName(myPackListFileName), false);
                    string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
                    string host = ConfigurationManager.AppSettings["smtpHost"];

                    PackingListManager.SendByEmail(packList, ms, fromEmail, host);
                    File.WriteAllBytes(myPathToSave, ms.ToArray());

                    RadGridVersand.DataBind();
                }
                catch (Exception ex)
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
            var dispachBox = sender as RadTextBox;
            var isSelf = dispachBox.Parent.FindControl("isSelfDispathCheckBox") as CheckBox;
            var printButton = dispachBox.Parent.FindControl("LieferscheinDruckenButton") as Button;
            script.RegisterPostBackControl(printButton);
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
            var isSelf = sender as CheckBox;
            var dispachBox = isSelf.Parent.FindControl("DispatchOrderNumberBox") as RadTextBox;
            var printButton = dispachBox.Parent.FindControl("LieferscheinDruckenButton") as Button;
            script.RegisterPostBackControl(printButton);
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

        #endregion

        #region Methods

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

        protected IEnumerable GetOrders(string number)
        {
            var packagingListNumber = Int32.Parse(number);

            return OrderManager.GetEntities(o =>
                  o.PackingListNumber == packagingListNumber &&
                  o.Status == (int)OrderStatusTypes.Closed &&
                  o.HasError.GetValueOrDefault(false) != true).Select(ord => new
                  {
                      OrderNumber = ord.Id,
                      CustomerName = ord.Customer.SmallCustomer != null &&
                          ord.Customer.SmallCustomer.Person != null ? ord.Customer.SmallCustomer.Person.FirstName + " " + ord.Customer.SmallCustomer.Person.Name :
                          ord.Customer.Name,
                      OrderLocation = ord.Location.Name,
                      Status = ord.OrderStatus.Name,
                      OrderType = ord.OrderType.Name,
                      OrderError = ord.HasError == true ? "Ja" : "Nein"
                  });
        }

        #endregion
    }
}