using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using KVSCommon.Database;

namespace KVSWebApplication.RequiredField
{
    /// <summary>
    /// Codebehind fuer die Pflichtfeldermaske
    /// </summary>
    public partial class RequiredField_Details : System.Web.UI.Page
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
        protected void Page_Load(object sender, System.EventArgs e)
        {
            thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(Int32.Parse(Session["CurrentUserId"].ToString())));          
        }
        protected void getAllCustomerRequiredDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from cust in dbContext.Customer
                        join cost in dbContext.LargeCustomer on cust.Id equals cost.CustomerId
                        orderby cust.Name
                        select new
                        {
                            TableId = "Outer",
                            cust.Id,
                            cust.Name,
                            cust.ContactId,
                            CustomerNumber = cust.CustomerNumber,
                            cust.AdressId,
                            cust.Adress.Street,
                            cust.Adress.StreetNumber,
                            cust.Adress.Zipcode,
                            cust.Adress.City,
                            cust.Adress.Country,
                            Show = cust.Id.ToString() == cust.LargeCustomer.CustomerId.ToString() ? "true" : "false",                       
                        };
            e.Result = query;
        }
        protected void RequiredListBox_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from reqField in dbContext.RequiredField
                        join orType in dbContext.OrderType on reqField.OrderTypeId equals orType.Id
                        where !(from l in dbContext.LargeCustomerRequiredField where
                                l.LargeCustomerId == Int32.Parse(e.WhereParameters["LargeCustomerId"].ToString())
                                select l.RequiredFieldId).Contains(reqField.Id)
                        select new
                        {
                            reqField.Id,
                            Name = "(" + orType.Name + ") " + reqField.RequiredFieldTranslation.Name,
                            OrName=reqField.OrderType.Name,
                           FName= reqField.Name
                        };
            query = query.OrderByDescending(q => q.OrName).ThenBy(q => q.Name);
            e.Result = query;
        }
        protected void CustomerRequiredListbox_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from reqField in dbContext.RequiredField
                        join orType in dbContext.OrderType on reqField.OrderTypeId equals orType.Id
                        where (from l in dbContext.LargeCustomerRequiredField
                                where
                                    l.LargeCustomerId == Int32.Parse(e.WhereParameters["LargeCustomerId"].ToString())
                                select l.RequiredFieldId).Contains(reqField.Id)
                        select new
                        {
                            reqField.Id,
                            Name = "(" + orType.Name + ") " + reqField.RequiredFieldTranslation.Name,
                            OrName = reqField.OrderType.Name,
                            FName = reqField.Name
                        };
            query = query.OrderByDescending(q => q.OrName).ThenBy(q => q.Name);
            e.Result = query;
        }
        protected void btnSaveRequired_Click(object sender, EventArgs e)
        {
            KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
            try
            {
                RadListBoxItemCollection AddedRequired = ((RadListBox)((RadButton)sender).Parent.FindControl("CustomerRequired")).Items;
                RadListBoxItemCollection AllRequired = ((RadListBox)((RadButton)sender).Parent.FindControl("AllRequired")).Items;
                var customerId = Int32.Parse(((RadButton)sender).CommandArgument.ToString());
                var myCustomer = dbContext.LargeCustomer.SingleOrDefault(q => q.CustomerId== customerId);
                if (myCustomer != null)
                {
                    foreach (RadListBoxItem required in AllRequired)
                    {
                        if (myCustomer.LargeCustomerRequiredField.SingleOrDefault(q => q.RequiredFieldId == Int32.Parse(required.Value)) != null)
                        {
                            myCustomer.RemoveRequiredField(Int32.Parse(required.Value), dbContext);
                        }
                    }
                    foreach (RadListBoxItem addItem in AddedRequired)
                    {
                        if (myCustomer.LargeCustomerRequiredField.SingleOrDefault(q => q.RequiredFieldId == Int32.Parse(addItem.Value)) == null)
                        {
                            myCustomer.AddRequiredField(Int32.Parse(addItem.Value), dbContext);
                        }
                    }
                    dbContext.SubmitChanges();
                }
                getAllCustomerRequired.EditIndexes.Clear();
                getAllCustomerRequired.MasterTableView.IsItemInserted = false;
                getAllCustomerRequired.MasterTableView.Rebind();
            }
            catch (Exception ex)
            {
                RadWindowManagerLargeCustomerRequired.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                try
                {
                    dbContext.WriteLogItem("LargeCustomerRequiredField Error " + ex.Message, LogTypes.ERROR, "LargeCustomerRequiredField");
                    dbContext.SubmitChanges();
                }
                catch { }
            }
        }
        protected void getAllCustomerRequired_Init(object sender, System.EventArgs e)
        {
            GridFilterMenu menu = getAllCustomerRequired.FilterMenu;
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
    }
}