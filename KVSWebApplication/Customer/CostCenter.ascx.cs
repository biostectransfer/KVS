using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using System.Collections;
using Telerik.Web.UI;
using System.Transactions;
namespace KVSWebApplication.Customer
{
    /// <summary>
    /// Codebehind fuer das Verwalten der Kostenstellen
    /// </summary>
    public partial class CostCenter : System.Web.UI.UserControl
    {
        List<string> thisUserPermissions = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(Int32.Parse(Session["CurrentUserId"].ToString())));
            if (!thisUserPermissions.Contains("KOSTENSTELLEN_BEARBEITEN"))
            {
                rbtCreateCostCenter.Enabled = false;
            }
            if (!thisUserPermissions.Contains("KOSTENSTELLEN_ANLEGEN"))
            {
                getAllCostCenter.MasterTableView.DetailTables[0].Columns[0].Visible = false;
            }
            if (!thisUserPermissions.Contains("KOSTENSTELLEN_ANSICHT"))
            {
                getAllCostCenter.MasterTableView.DetailTables[0].Columns[0].Visible = false;
                rbtCreateCostCenter.Enabled = false;
            }
            if (!thisUserPermissions.Contains("LOESCHEN"))
            {
                getAllCostCenter.MasterTableView.Columns[getAllCostCenter.MasterTableView.Columns.Count - 1].Visible = false;
            }
            if (!Page.IsPostBack)
            {
                using (KVSEntities dbContext = new KVSEntities())
                {                   
                    txbCostCenterNumber.Text = EmptyStringIfNull.generateIndividualNumber(dbContext.CostCenter.Max(q => q.CostcenterNumber));
                }
            }
        }      
        /// <summary>
        /// Gibt die Mastergrid zurück (Informationen zur allen Kunden)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void getAllCostCenterDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from cust in dbContext.Customer
                        join cost in dbContext.CostCenter on cust.Id equals cost.CustomerId 
                        orderby cost.Name
                        select new
                        {
                            TableId = "Outer",
                            CustomerId= cust.Id,
                            cust.Name,
                            CostCenterId = cost.Id,
                            CostCenterNumber = cost.CostcenterNumber,
                            CustomerNumber = cust.CustomerNumber,
                            cust.ContactId,
                            cust.AdressId,
                            cust.Adress.Street,
                            cust.Adress.StreetNumber,
                            cust.Adress.Zipcode,
                            cust.Adress.City,
                            cust.Adress.Country,
                            CostCenterName= cost.Name,                   
                        };
            e.Result = query;
        }
        /// <summary>
        /// Gibt die KostenstellenDaten zurück, die beim Kunden ausgewählt wurden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GetCustomerCostCenter_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {         
            KVSEntities dbContext = new KVSEntities();
            var query = from cust in dbContext.CostCenter
                        where cust.CustomerId == Int32.Parse(e.WhereParameters["CustomerId"].ToString()) && cust.Id == Int32.Parse(e.WhereParameters["CostCenterId"].ToString())
                        orderby cust.BankAccount.BankName
                        select new
                        {
                            TableId = "Inner",
                            CustomerId = cust.CustomerId,
                            CostCenterId = cust.Id,
                            CostCenterName = cust.Name,
                            BankId = cust.BankAccount == null ? (int?)null : cust.BankAccount.Id,
                            BankName = cust.BankAccount == null ? EmptyStringIfNull.ReturnEmptyStringIfNull(null) : cust.BankAccount.BankName,
                            Accountnumber = cust.BankAccount == null ? EmptyStringIfNull.ReturnEmptyStringIfNull(null) : cust.BankAccount.Accountnumber,
                            BankCode = cust.BankAccount == null ? EmptyStringIfNull.ReturnEmptyStringIfNull(null) : cust.BankAccount.BankCode,
                            IBAN = cust.BankAccount == null ? EmptyStringIfNull.ReturnEmptyStringIfNull(null) : cust.BankAccount.IBAN,
                            BIC = cust.BankAccount == null ? EmptyStringIfNull.ReturnEmptyStringIfNull(null) : cust.BankAccount.BIC
                        };    
            e.Result = query;
        }
        protected void getAllCustomer_EditCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
           bool insertUpdateOk = true;
            using (TransactionScope ts = new TransactionScope())
            {
                Hashtable newValues = new Hashtable();
                ((GridEditableItem)e.Item).ExtractValues(newValues);
                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())); // hier kommt die Loggingid
                try
                {
                    if (newValues["TableId"].ToString() == "Inner")
                    {
                        var contactUpdate = dbContext.CostCenter.SingleOrDefault(q => q.Id == Int32.Parse(newValues["CostCenterId"].ToString()) &&
                            q.CustomerId == Int32.Parse(newValues["CustomerId"].ToString()));
                        contactUpdate._dbContext = dbContext;
                        if (contactUpdate != null)
                        {
                            if (newValues["BankId"] == null || String.IsNullOrEmpty(newValues["BankId"].ToString()))
                            {
                                if (newValues["BankName"] != null || newValues["IBAN"] != null)
                                {
                                    var newBankAccount = KVSCommon.Database.BankAccount.CreateBankAccount(dbContext, EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["BankName"]),
                                        EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Accountnumber"]),
                                        EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["BankCode"]),
                                        EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["IBAN"]),
                                        EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["BIC"]));
                                    contactUpdate.LogDBContext = dbContext;
                                    contactUpdate.BankAccountId = newBankAccount.Id;
                                    dbContext.SubmitChanges();
                                }
                            }
                            else
                            {
                                contactUpdate.LogDBContext = dbContext;
                                contactUpdate.Name = newValues["CostCenterName"].ToString();
                                contactUpdate.BankAccount.LogDBContext = dbContext;
                                contactUpdate.BankAccount.BankName = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["BankName"]);
                                contactUpdate.BankAccount.Accountnumber = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Accountnumber"]);
                                contactUpdate.BankAccount.BankCode = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["BankCode"]);
                                contactUpdate.BankAccount.IBAN = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["IBAN"]);
                                contactUpdate.BankAccount.BIC = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["BIC"]);
                                dbContext.SubmitChanges();
                            }
                            ts.Complete();
                        

                        }
                    }
                }
                catch (Exception ex)
                {
                    insertUpdateOk = false;
                    if (ts != null)
                        ts.Dispose();
                    RadWindowManagerCostCenter.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                    try
                    {
                        dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                        dbContext.WriteLogItem("CostCenter Edit Error " + ex.Message, LogTypes.ERROR, "CostCenter");
                        dbContext.SubmitChanges();
                    }
                    catch { }
                }
            }
            if (insertUpdateOk)
            {
                getAllCostCenter.EditIndexes.Clear();
                getAllCostCenter.MasterTableView.IsItemInserted = false;
                getAllCostCenter.MasterTableView.Rebind();
            }
        }
        protected void getAllCostCenter_Init(object sender, System.EventArgs e)
        {
            GridFilterMenu menu = getAllCostCenter.FilterMenu;
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
        protected void AllCostCenterChecked(object sender, EventArgs e)
        {
            AllCostCenterTable.Visible = false;
            getAllCostCenter.Visible = true;
        }
        protected void CreateCostcenterChecked(object sender, EventArgs e)
        {
            AllCostCenterTable.Visible = true;
            getAllCostCenter.Visible = false;
        }
        protected void CustomerCombobox_ItemsRequested(object sender, EventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from customer in dbContext.Customer
                        join lCustomer in dbContext.LargeCustomer on customer.Id equals lCustomer.Id
                        select new
                        {
                            Name = customer.Name,
                            Value = customer.Id,
                            Matchcode = customer.MatchCode,
                            Kundennummer = customer.CustomerNumber
                        };
            CustomerCostCenter.Items.Clear();
            CustomerCostCenter.DataSource = query;
            CustomerCostCenter.DataBind();
        } 
        protected bool checkFields()
        {
            bool check = true;
            if (CustomerCostCenter.SelectedValue == string.Empty)
            {
                lblCustomerNameCostCenterError.Text = "Bitte wählen Sie einen Kunden aus";
                check = false;
            }
            if (CostCenterName.Text == string.Empty)
            {
                check = false;
                lblCostCenterNameError.Text="Bitte geben Sie den Kostenstellennamen an";
            }
            if (txbCostCenterNumber.Text == string.Empty)
            {
                check = false;
                lblCostCenterNumberError.Text = "Bitte geben Sie die Kostenstellenummer an";
            }
            return check;
        }
        protected void genIban_Click(object sender, EventArgs e)
        {
            if (cmbCostCenterAccountNumber.Text != string.Empty && rcbCostCenterBankCode.Text != string.Empty
                && EmptyStringIfNull.IsNumber(cmbCostCenterAccountNumber.Text) && EmptyStringIfNull.IsNumber(rcbCostCenterBankCode.Text))
            {
                txbLargeCustomerIBAN.Text = "DE" + (98 - ((62 * ((1 + long.Parse(rcbCostCenterBankCode.Text) % 97)) +
                    27 * (long.Parse(cmbCostCenterAccountNumber.Text) % 97)) % 97)).ToString("D2");
                txbLargeCustomerIBAN.Text += long.Parse(rcbCostCenterBankCode.Text).ToString("00000000").Substring(0, 4);
                txbLargeCustomerIBAN.Text += long.Parse(rcbCostCenterBankCode.Text).ToString("00000000").Substring(4, 4);
                txbLargeCustomerIBAN.Text += long.Parse(cmbCostCenterAccountNumber.Text).ToString("0000000000").Substring(0, 4);
                txbLargeCustomerIBAN.Text += long.Parse(cmbCostCenterAccountNumber.Text).ToString("0000000000").Substring(4, 4);
                txbLargeCustomerIBAN.Text += long.Parse(cmbCostCenterAccountNumber.Text).ToString("0000000000").Substring(8, 2);
                using (KVSEntities dataContext = new KVSEntities())
                {
                    var bicNr = dataContext.BIC_DE.FirstOrDefault(q => q.Bankleitzahl.Contains(rcbCostCenterBankCode.Text) && (q.Bezeichnung.Contains(cmbBankNameCostCenter.Text) || q.Kurzbezeichnung.Contains(cmbBankNameCostCenter.Text)));
                    if (bicNr != null)
                    {
                        if (!String.IsNullOrEmpty(bicNr.BIC.ToString()))
                            txbLargeCustomerBIC.Text = bicNr.BIC.ToString();
                    }
                }
            }
        }
        protected void RemoveCostCenter_Click(object sender, EventArgs e)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                

                    try
                    {
                        RadButton rbtSender = ((RadButton)sender);
                        Label lbl = rbtSender.Parent.FindControl("lblCostCenterId") as Label;
                        if (!String.IsNullOrEmpty(lbl.Text))
                           KVSCommon.Database.CostCenter.RemoveCostCenter(Int32.Parse(lbl.Text),dbContext);

                        dbContext.SubmitChanges();
                      
                        ts.Complete();
                        RadWindowManagerCostCenter.RadAlert("Kostenstelle wurde erfolgreich gelöscht", 380, 180, "Info", "");
                       

                    }
                    catch (Exception ex)
                    {

                        if (ts != null)
                            ts.Dispose();
                        RadWindowManagerCostCenter.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                        try
                        {
                            dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                            dbContext.WriteLogItem("CostCenter Remove Error " + ex.Message, LogTypes.ERROR, "CostCenter");
                            dbContext.SubmitChanges();
                        }
                        catch { }

                    }
                
            }
            getAllCostCenter.EditIndexes.Clear();
            getAllCostCenter.MasterTableView.IsItemInserted = false;
            getAllCostCenter.MasterTableView.Rebind();
        }
        protected void rbtSaveCostCenter_Click(object sender, EventArgs e)
        {
            if (checkFields())
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())); // hier kommt die Loggingid
                    try
                    {
                        var contactUpdate = dbContext.Customer.SingleOrDefault(q => q.Id == Int32.Parse(CustomerCostCenter.SelectedValue));
                        if (contactUpdate != null)
                        {
                            var newCostCenter = dbContext.LargeCustomer.SingleOrDefault(q => q.Id == Int32.Parse(CustomerCostCenter.SelectedValue));
                            var createdCostCenter = newCostCenter.AddNewCostCenter(CostCenterName.Text, txbCostCenterNumber.Text, dbContext);
                            if (cmbBankNameCostCenter.Text != string.Empty || txbLargeCustomerIBAN.Text != string.Empty)
                            {
                                var createBank = KVSCommon.Database.BankAccount.CreateBankAccount(dbContext, cmbBankNameCostCenter.Text, cmbCostCenterAccountNumber.Text, rcbCostCenterBankCode.Text, txbLargeCustomerIBAN.Text, txbLargeCustomerBIC.Text);
                                createdCostCenter.BankAccountId = createBank.Id;
                            }
                            dbContext.SubmitChanges();
                            ts.Complete();
                            RadWindowManagerCostCenter.RadAlert("Die Kostenstelle wurde erfolgreich angelegt.", 380, 180, "Information", "");
                        }
                        else
                            throw new Exception("Bitte wählen Sie einen Kunden aus.");
                    }
                    catch (Exception ex)
                    {
                        if (ts != null)
                            ts.Dispose();
                        RadWindowManagerCostCenter.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                        try
                        {
                            dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                            dbContext.WriteLogItem("rbtSaveCostCenter_Click Error " + ex.Message, LogTypes.ERROR, "CostCenter");
                            dbContext.SubmitChanges();
                        }
                        catch { }
                    }
                }
            }
        }
    }
}