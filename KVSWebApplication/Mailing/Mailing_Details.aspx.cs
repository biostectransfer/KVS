using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;

namespace KVSWebApplication.Mailing
{
    /// <summary>
    /// Codebehind fuer die Email Verwaltung
    /// </summary>
    public partial class Mailing_Details : System.Web.UI.Page
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
            rbtCustomerMail_Checked(this, new EventArgs());
            thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(Int32.Parse(Session["CurrentUserId"].ToString())));
            if (!thisUserPermissions.Contains("EMAIL_UEBERSICHT") && !thisUserPermissions.Contains("EMAIL_BEARBEITEN") && !thisUserPermissions.Contains("EMAIL_LOESCHEN") && !thisUserPermissions.Contains("EMAIL_ANLEGEN"))
            {
                Response.Redirect("../AccessDenied.aspx");
            }
            if (!thisUserPermissions.Contains("EMAIL_BEARBEITEN"))
            {
                RadGridMailEditor.Columns[0].Visible = false;            
            }
        
            if (!thisUserPermissions.Contains("EMAIL_LOESCHEN"))
            {
                RadGridMailEditor.Columns[1].Visible = false;            
            }
      
            if (!thisUserPermissions.Contains("EMAIL_ANLEGEN"))
            {
                foreach (GridItem item in RadGridMailEditor.MasterTableView.GetItems(GridItemType.CommandItem))
                {
                    ImageButton CustomerProductsAddButton = item.FindControl("add") as ImageButton;
                    CustomerProductsAddButton.Visible = false;
                    CustomerProductsAddButton.Enabled = false;
                }         
            }
        }
        protected void RadGridMailEditor_Init(object sender, System.EventArgs e)
        {
            GridFilterMenu menu = RadGridMailEditor.FilterMenu;
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
        protected void rbtCustomerMail_Checked(object sender, EventArgs e)
        {
         if(rbtCustomerMail.Checked==true)
            cmbLocations.Enabled = false;
        }
        protected void rbtLocationMail_Checked(object sender, EventArgs e)
        {
            if (AllCustomer.SelectedValue != string.Empty &&rbtLocationMail.Checked == true)
            {
                cmbLocations.Enabled = true;
                DataClasses1DataContext dbContext = new DataClasses1DataContext();
                if (AllCustomer.SelectedValue != string.Empty)
                {
                    var query = from customer in dbContext.Customer
                                join largeCustomer in dbContext.LargeCustomer on customer.Id equals largeCustomer.CustomerId
                                join _location in dbContext.Location on customer.Id equals _location.CustomerId
                                where customer.Id == Int32.Parse(AllCustomer.SelectedValue)
                                orderby _location.Name
                                select new
                                {
                                    _location.Id,
                                    _location.Name
                                };
                    cmbLocations.Items.Clear();
                    foreach (var location in query)
                    {
                        if (location.Id.ToString() != string.Empty)
                        {
                            cmbLocations.Items.Add(new RadComboBoxItem(location.Name, location.Id.ToString()));
                        }
                    }
                 }             
            }
        }
        protected void GetMailAdresses_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {            
            DataClasses1DataContext dbContext = new DataClasses1DataContext();

            if (rbtCustomerMail.Checked == true && !String.IsNullOrEmpty(AllCustomer.SelectedValue))
            {
               var mails = from emails in dbContext.Mailinglist
                           where emails.CustomerId == Int32.Parse(AllCustomer.SelectedValue)
                        select new { emails.Id, emails.CustomerId, emails.LocationId, EmailAdresse = emails.Email, EmailType = emails.MailinglistType.Name };
                e.Result = mails;
            }
            else if (rbtLocationMail.Checked == true && String.IsNullOrEmpty(cmbLocations.SelectedValue))
            {
                var mails = from emails in dbContext.Mailinglist
                            where emails.LocationId == Int32.Parse(cmbLocations.SelectedValue)
                            select new { emails.Id, emails.CustomerId, emails.LocationId, EmailAdresse = emails.Email, EmailType = emails.MailinglistType.Name };
                e.Result = mails;
            }
            else
            {
                var mails = from emails in dbContext.Mailinglist
                            where 1 == 0
                            group emails by emails.Id into t
                            select new { t.Key };
                e.Result = mails;
            }
            if (String.IsNullOrEmpty(AllCustomer.SelectedValue))
            {
                RadGridMailEditor.Enabled = false;
                RadGridMailEditor.Visible = false;
            }
            else if (rbtLocationMail.Checked == true)
            {
                if (String.IsNullOrEmpty(cmbLocations.SelectedValue))
                {
                    RadGridMailEditor.Enabled = false;
                    RadGridMailEditor.Visible = false;
                }
                else
                {
                    RadGridMailEditor.Enabled = true;
                    RadGridMailEditor.Visible = true;
                }
            }
            else
            {
                RadGridMailEditor.Enabled = true;
                RadGridMailEditor.Visible = true;
            }
        }
        protected void bSchow_Click(object sender, EventArgs e)
        {
            RadGridMailEditor.Enabled = true;
            RadGridMailEditor.Visible = true;
            if (rbtLocationMail.Checked == true)
            {
                if (cmbLocations.SelectedValue == string.Empty)
                {
                    RadGridMailEditor.Enabled = false;
                    RadGridMailEditor.Visible = false;
                    return;
                }
                if (String.IsNullOrEmpty(cmbLocations.SelectedValue))
                {
                    RadGridMailEditor.Enabled = false;
                    RadGridMailEditor.Visible = false;
                    return;
                }
            }
            if (AllCustomer.SelectedValue == string.Empty)
            {
                RadGridMailEditor.Enabled = false;
                RadGridMailEditor.Visible = false;
                return;
            }
            if (String.IsNullOrEmpty(AllCustomer.SelectedValue))
            {
                RadGridMailEditor.Enabled = false;
                RadGridMailEditor.Visible = false;
                return;
            }        
            RadGridMailEditor.EditIndexes.Clear();
            RadGridMailEditor.MasterTableView.IsItemInserted = false;
            RadGridMailEditor.MasterTableView.Rebind();
            if (!thisUserPermissions.Contains("EMAIL_ANLEGEN"))
            {
                foreach (GridItem item in RadGridMailEditor.MasterTableView.GetItems(GridItemType.CommandItem))
                {
                    ImageButton CustomerProductsAddButton = item.FindControl("add") as ImageButton;
                    CustomerProductsAddButton.Enabled = false;
                }
            }
        }
        protected void btnSaveMail_Click(object sender, EventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())); // hier kommt die Loggingid
            Button myButton = ((Button)sender);
            Label errorMessage = ((Label)myButton.FindControl("SchowErrorMessages"));
            TextBox email = ((TextBox)myButton.FindControl("EmailAdress"));
            RadComboBox cmbSelectedType = ((RadComboBox)myButton.FindControl("cmbMailTyp"));
            try
            {
                errorMessage.Text = "";
                TextBox Id = ((TextBox)myButton.FindControl("EditMailId"));
             
                TextBox customerId = ((TextBox)myButton.FindControl("EditCustomerId"));
                TextBox locationId = ((TextBox)myButton.FindControl("EditLocationId"));
               
                Id.Text = Id.Text.Trim();
                customerId.Text = customerId.Text.Trim();
                locationId.Text = locationId.Text.Trim();

                if (cmbSelectedType.SelectedValue == string.Empty || email.Text == null || email.Text == string.Empty)
                {
                    throw new Exception("Bitte geben Sie einen Emailtyp und die Emailadresse ein");
                }
                if ((Id.Text == string.Empty && customerId.Text == string.Empty && locationId.Text == string.Empty) )
                {
                    if (cmbSelectedType.SelectedValue != string.Empty)
                    {
                        if (rbtCustomerMail.Checked == true && AllCustomer.SelectedValue != string.Empty)
                        {
                            var cust = dbContext.LargeCustomer.SingleOrDefault(q => q.CustomerId == Int32.Parse(AllCustomer.SelectedValue));
                            if (cust != null)
                            {
                                cust.AddNewMailinglistItem(EmptyStringIfNull.ReturnEmptyStringIfNull(email.Text), Int32.Parse(cmbSelectedType.SelectedValue), dbContext);
                            }                         
                        }
                        if (rbtLocationMail.Checked == true && AllCustomer.SelectedValue != "" && cmbLocations.SelectedValue != "")
                        {
                            var loc = dbContext.Location.SingleOrDefault(q => q.Id == Int32.Parse(cmbLocations.SelectedValue));
                            if (loc != null)
                            {
                                loc.AddNewMailinglistItem(EmptyStringIfNull.ReturnEmptyStringIfNull(email.Text), Int32.Parse(cmbSelectedType.SelectedValue), dbContext);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Bitte geben Sie einen Emailtyp und die Emailadresse ein");
                    }
                }
                else if (Id.Text != string.Empty || email.Text == string.Empty)
                {
                    var Mlist = dbContext.Mailinglist.SingleOrDefault(q => q.Id == Int32.Parse(Id.Text));
                    if (Mlist != null)
                    {
                        Mlist.LogDBContext = dbContext;
                        Mlist.Email = EmptyStringIfNull.ReturnEmptyStringIfNull(email.Text);
                        if (cmbSelectedType.SelectedValue != string.Empty)
                        {
                            Mlist.MailinglistTypeId = Int32.Parse(cmbSelectedType.SelectedValue);
                        }                         
                    }
                 }
                else
                {
                    throw new Exception("Bitte geben Sie einen Emailtyp und die Emailadresse ein");
                }
                dbContext.SubmitChanges();
                RadGridMailEditor.EditIndexes.Clear();
                RadGridMailEditor.MasterTableView.IsItemInserted = false;
                RadGridMailEditor.MasterTableView.Rebind();
           } 
                catch(Exception ex)
            {
                cmbSelectedType.Text = "";
                errorMessage.Text = "Fehler:" + ex.Message;
                dbContext.WriteLogItem("Email List Update Error " + ex.Message, LogTypes.ERROR, "Mailinglist");
            }
        }
        protected void btnAbortMail_Click(object sender, EventArgs e)
        {
            RadGridMailEditor.EditIndexes.Clear();
            RadGridMailEditor.MasterTableView.IsItemInserted = false;
            RadGridMailEditor.MasterTableView.Rebind();
        }
        protected void RadGridMailEditor_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandSource is ImageButton)
            {
                if (((ImageButton)e.CommandSource).CommandName == "Delete")
                {
                    if (e.Item is GridDataItem)
                    {
                        GridDataItem deletedItem = e.Item as GridDataItem;
                        using (DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())))
                        {
                            try
                            {
                                if (rbtCustomerMail.Checked == true && AllCustomer.SelectedValue != string.Empty)
                                {
                                    var cust = dbContext.LargeCustomer.SingleOrDefault(q => q.CustomerId == Int32.Parse(AllCustomer.SelectedValue));
                                    if (cust != null && deletedItem["Id"].Text != string.Empty)
                                    {
                                        cust.RemoveMailinglistItem(Int32.Parse(deletedItem["Id"].Text), dbContext);
                                    }
                                }
                                if (rbtLocationMail.Checked == true)
                                {
                                    var loc = dbContext.Location.SingleOrDefault(q => q.Id == Int32.Parse(cmbLocations.SelectedValue));
                                    if (loc != null && deletedItem["Id"].Text != string.Empty)
                                    {
                                        loc.RemoveMailinglistItem(Int32.Parse(deletedItem["Id"].Text), dbContext);
                                    }
                                }
                                dbContext.SubmitChanges();
                            }
                            catch (Exception ex)
                            {
                                RadWindowManagerEditMail.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                                try
                                {
                                    dbContext.WriteLogItem("Email List Delete Error " + ex.Message, LogTypes.ERROR, "Mailinglist");
                                    dbContext.SubmitChanges();
                                }
                                catch { }
                            }
                        }        
                    }
                }
                else if (((ImageButton)e.CommandSource).CommandName == "InsertItem")
                {
                    e.Item.OwnerTableView.IsItemInserted = true;
                    e.Item.OwnerTableView.InsertItem();
                }
            }
        }
        protected void CustomerCombobox_Init(object sender, EventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            var largeCustomer = from cust in dbContext.LargeCustomer
                                select new
                                {
                                    Name = cust.Customer.Name,
                                    Value = cust.Customer.Id,
                                    Matchcode = cust.Customer.MatchCode,
                                    Kundennummer = cust.Customer.CustomerNumber
                                };
            AllCustomer.Items.Clear();
            AllCustomer.DataSource = largeCustomer;
            AllCustomer.DataBind();
        }
        protected void MailType_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            RadComboBox types = ((RadComboBox)sender);
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            var mailType = from mailT in dbContext.MailinglistType
                           select new { id = mailT.Id, typeName = mailT.Name };
            AllCustomer.Items.Clear();
            foreach (var myTypes in mailType)
            {
                types.Items.Add(new RadComboBoxItem(myTypes.typeName, myTypes.id.ToString()));
            }
        }
        protected void CustomerCombobox_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (AllCustomer.SelectedValue != string.Empty && rbtLocationMail.Checked == true)
            {
                DataClasses1DataContext dbContext = new DataClasses1DataContext();
                if (AllCustomer.SelectedValue != string.Empty)
                {
                    var query = from customer in dbContext.Customer
                                join largeCustomer in dbContext.LargeCustomer on customer.Id equals largeCustomer.CustomerId
                                join _location in dbContext.Location on customer.Id equals _location.CustomerId
                                where customer.Id == Int32.Parse(e.Value)
                                select new
                                {
                                    _location.Id,
                                    _location.Name
                                };
                    cmbLocations.Items.Clear();
                    foreach (var location in query)
                    {
                        if (location.Id.ToString() != string.Empty)
                        {
                            cmbLocations.Items.Add(new RadComboBoxItem(location.Name, location.Id.ToString()));
                        }
                    }
                }
            }
        }
    }
}