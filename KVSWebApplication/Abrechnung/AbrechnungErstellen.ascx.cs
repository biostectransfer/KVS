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

namespace KVSWebApplication.Abrechnung
{
    /// <summary>
    /// Codebehind fuer den Reiter Abrechnung Erstellen
    /// </summary>
    public partial class AbrechnungErstellen : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string target = Request["__EVENTTARGET"];
            if (target == "New_Param")
            {
                UpdateInvoiceWithNewParam();
            }
            Abrechnung abr = Page as Abrechnung;
            RadScriptManager script = abr.getScriptManager() as RadScriptManager;
            script.RegisterPostBackControl(RechnungErstellenButton);
            script.RegisterPostBackControl(PrintCopyButton);
            script.RegisterPostBackControl(btnShowInvoice);
            if (Session["CurrentUserId"] != null)
            {
                if (!String.IsNullOrEmpty(Session["CurrentUserId"].ToString()))
                {
                    CheckUserPermissions();
                }
                if (!Page.IsPostBack)
                {
                    RadGridAbrechnungErstellen.Enabled = true;
                    RadGridAbrechnungErstellen.DataBind();
                }
            }
        }
        /// <summary>
        /// Pruefe die Benutzerrechte
        /// </summary>
        protected void CheckUserPermissions()
        {
            List<string> userPermissions = new List<string>();
            userPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(Int32.Parse(Session["CurrentUserId"].ToString())));
            if (userPermissions.Count > 0)
            {
                if (!userPermissions.Contains("RECHNUNG_ERSTELLEN"))
                {
                    AllButtonsPanel.Visible = false;
                }
                else
                {
                    AllButtonsPanel.Visible = true;
                }
            }
        }
        #region Methods
        /// <summary>
        /// Event fuer den Stornieren Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Stornieren_Clicked(object sender, EventArgs e)
        {
            if (RadGridAbrechnungErstellen.SelectedItems.Count > 0)
            {
                RechnungVorschauErrorLabel.Visible = false;
                foreach (GridDataItem item in RadGridAbrechnungErstellen.SelectedItems)
                {
                    var invoiceID = Int32.Parse(item["invoiceId"].Text);
                    if (invoiceID != null)
                    {
                        DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                        Invoice newInvoice = dbContext.Invoice.SingleOrDefault(q => q.Id == invoiceID);
                        if (newInvoice.IsPrinted == false)
                        {
                            newInvoice.canceled = true;
                            newInvoice.Cancel(dbContext);
                            dbContext.SubmitChanges();
                        }
                        else
                        {
                            RechnungVorschauErrorLabel.Visible = true;
                            RechnungVorschauErrorLabel.Text = "Sie können keine Rechnung stornieren, die bereits gedruckt/gebucht ist";
                        }
                    }
                }
                RadGridAbrechnungErstellen.MasterTableView.ClearChildEditItems();
                RadGridAbrechnungErstellen.MasterTableView.ClearEditItems();
                RadGridAbrechnungErstellen.Rebind();
            }
            else
            {
                RechnungVorschauErrorLabel.Visible = true;
                RechnungVorschauErrorLabel.Text = "Sie haben keine Rechnung zum stornieren ausgewählt";
            }
        }
        /// <summary>
        /// Event fuer den Clear Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void clearButton_Click(object sender, EventArgs e)
        {
            CustomerDropDownList.ClearSelection();
            RadGridAbrechnungErstellen.DataBind();
        }
        /// <summary>
        /// Event um eine Mail zu senden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EmailSendButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (DataClasses1DataContext dbCotenxt = new DataClasses1DataContext())
                {
                    EmailOkeyLabel.Visible = false;
                    var item = RadGridAbrechnungErstellen.SelectedItems[0] as GridDataItem;
                    var invoiceId = Int32.Parse(item["invoiceId"].Text);
                    string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
                    string host = ConfigurationManager.AppSettings["smtpHost"];
                    Invoice.SendByMail(dbCotenxt, invoiceId, host, fromEmail);
                    EmailOkeyLabel.ForeColor = System.Drawing.Color.Green;
                    EmailOkeyLabel.Text = "Email wurde erfolgreich gesendet!";
                    EmailOkeyLabel.Visible = true;
                }
            }
            catch (Exception ex)
            {
                EmailOkeyLabel.ForeColor = System.Drawing.Color.Red;
                EmailOkeyLabel.Text = "Fehler beim Emailversand:" + ex.Message;
                EmailOkeyLabel.Visible = true;
            }
        }
        /// <summary>
        /// Update Invoice mit neue Positionen
        /// </summary>
        protected void UpdateInvoiceWithNewParam()
        {
            try
            {
                if (!EmptyStringIfNull.IsNumber(AmountField.Value))
                    throw new Exception("Sie müssen beim Preis eine nummerische Zahl eintragen");
                decimal newAmount = Convert.ToDecimal(AmountField.Value);
                string newInvoiceItemName = NameField.Value;
                if (RadGridAbrechnungErstellen.SelectedItems.Count == 1)
                {
                    foreach (GridDataItem item in RadGridAbrechnungErstellen.SelectedItems)
                    {
                        var invoiceId = Int32.Parse(item["invoiceId"].Text);
                        var dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                        var invoiceToAdd = dbContext.Invoice.SingleOrDefault(q => q.Id == invoiceId);
                        if (invoiceToAdd != null)
                        {
                            invoiceToAdd.AddInvoiceItem(newInvoiceItemName, newAmount, 1, null, null, dbContext);
                            dbContext.SubmitChanges();
                        }
                    }
                }
                RadGridAbrechnungErstellen.Rebind();
            }
            catch (Exception ex)
            {
                RechnungVorschauErrorLabel.Visible = true;
                RechnungVorschauErrorLabel.Text = "Fehler: " + ex.Message;
            }
        }
        #endregion
        #region Index Changed
        protected void Cell_Selected(object sender, EventArgs e)
        {
            if (RadGridAbrechnungErstellen.SelectedItems.Count == 0)
            {
                AddButton.Enabled = false;
                RechnungErstellenButton.Enabled = false;
                PrintCopyButton.Enabled = false;
            }
            else
            {
                try
                {
                    GridDataItem item = RadGridAbrechnungErstellen.SelectedItems[0] as GridDataItem;
                    AddButton.Enabled = true;
                    if (Convert.ToBoolean(item["isPrinted"].Text))
                    {
                        PrintCopyButton.Enabled = true;
                        RechnungErstellenButton.Enabled = false;
                        EmailSendButton.Enabled = true;
                        StornierenButton.Enabled = false;
                        AddButton.Enabled = false;
                        btnShowInvoice.Enabled = true;
                    }
                    else
                    {
                        PrintCopyButton.Enabled = false;
                        RechnungErstellenButton.Enabled = true;
                        EmailSendButton.Enabled = false;
                        StornierenButton.Enabled = true;
                        AddButton.Enabled = true;
                        btnShowInvoice.Enabled = false;
                    }
                }
                catch { }
            }
        }
        protected void PrintCopyButton_Clicked(object sender, EventArgs e)
        {
            GridDataItem item = RadGridAbrechnungErstellen.SelectedItems[0] as GridDataItem;
            if (Convert.ToBoolean(item["isPrinted"].Text))
            {
                PrintCopyErrorLabel.Visible = false;
                try
                {
                    DataClasses1DataContext dbContext = new DataClasses1DataContext();
                    var invoiceId = Int32.Parse(item["invoiceId"].Text);
                    Invoice newInvoice = dbContext.Invoice.SingleOrDefault(q => q.Id == invoiceId);
                    string fileName = "RechnungsCopy_" + newInvoice.CreateDate.Day + "_" + newInvoice.CreateDate.Month + "_" + newInvoice.CreateDate.Year + "_" + DateTime.Now.Ticks + ".pdf";
                    using (MemoryStream memS = new MemoryStream())
                    {
                        string serverPath = ConfigurationManager.AppSettings["DataPath"] + "\\UserData";
                        newInvoice.PrintCopy(memS);
                        if (!Directory.Exists(serverPath)) Directory.CreateDirectory(serverPath);
                        if (!Directory.Exists(serverPath + "\\" + Session["CurrentUserId"].ToString())) Directory.CreateDirectory(serverPath + "\\" + Session["CurrentUserId"].ToString());
                        serverPath = serverPath + "\\" + Session["CurrentUserId"].ToString();
                        File.WriteAllBytes(serverPath + "\\" + fileName, memS.ToArray());
                        OpenPrintfile(fileName);
                        dbContext.SubmitChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                PrintCopyErrorLabel.Visible = true;
            }
        }
        protected void CustomerIndex_Changed(object sender, EventArgs e)
        {
            RadGridAbrechnungErstellen.Enabled = true;
            RadGridAbrechnungErstellen.DataBind();
        }
        #endregion
        #region Linq Data Sources
        /// <summary>
        /// Detail Tabelle Datasource
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DetailTable_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();

            bool isPrinted = false;
            //TODO if (e.WhereParameters["InvoiceId"] != null)
            //{
            var invoiceId = Int32.Parse(e.WhereParameters["InvoiceId"].ToString());
            //}
            if (e.WhereParameters["isPrinted"] != null)
            {
                isPrinted = Convert.ToBoolean(e.WhereParameters["isPrinted"].ToString());
            }
            var invoiceAccounts = Accounts.generateAccountNumber(dbContext, invoiceId, isPrinted).ToList();
            var invoiceItemsQuery = (from invitem in dbContext.InvoiceItem
                                     join inv in dbContext.Invoice on invitem.InvoiceId equals inv.Id
                                     where invitem.InvoiceId == invoiceId
                                     select new InnerItems
                                     {
                                         ItemId = invitem.Id,
                                         OrderItemId = invitem.OrderItemId,
                                         InvoiceId = inv.Id,
                                         Amount = invitem.Amount,
                                         Count = invitem.Count,
                                         Name = invitem.Name,
                                         isPrinted = inv.IsPrinted,
                                         AccountId = 0,
                                         AccountNumber = "",
                                         InvoiceItemId = invitem.Id,
                                         Active = (isPrinted) ? false : true
                                     }).ToList();
            foreach (var invItem in invoiceItemsQuery)
            {
                var thisItem = (from i in invoiceAccounts
                                where i.InvoiceItemId == invItem.ItemId
                                select new _Accounts { InvoiceItemId = i.InvoiceItemId, AccountId = i.AccountId, AccountNumber = i.AccountNumber, }).SingleOrDefault();
                if (thisItem != null)
                {
                    invItem.AccountId = thisItem.AccountId;
                    invItem.AccountNumber = thisItem.AccountNumber;
                    invItem.InvoiceItemId = thisItem.InvoiceItemId;
                }
            }
            e.Result = invoiceItemsQuery;
        }
        /// <summary>
        ///  Auswahl der KundenNamen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var customerQuery = from cust in con.Customer
                                select new
                                {
                                    Name = cust.SmallCustomer != null && cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name,
                                    Value = cust.Id,
                                    Matchcode = cust.MatchCode,
                                    Kundennummer = cust.CustomerNumber
                                };
            e.Result = customerQuery;
        }
        /// <summary>
        /// Gibt alle Daten zum gewaehlten Kunden zurueck
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AbrechnungLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var customerId = 0;
            if (CustomerDropDownList.SelectedValue != string.Empty)
                customerId = Int32.Parse(CustomerDropDownList.SelectedValue);
            if (customerId != 0)
            {
                var invoiceQuery = from inv in con.Invoice
                                   where inv.CustomerId == customerId
                                   && (inv.canceled == null || inv.canceled == false)
                                   orderby inv.CreateDate descending
                                   select new
                                   {
                                       invoiceId = inv.Id,
                                       customerNumber = inv.Customer.CustomerNumber,
                                       Matchcode = inv.Customer.MatchCode,
                                       customerName = //inv.Customer.Name,
                                        inv.Customer.SmallCustomer != null &&
                                        inv.Customer.SmallCustomer.Person != null ?
                                        inv.Customer.SmallCustomer.Person.FirstName + " " +
                                        inv.Customer.SmallCustomer.Person.Name : inv.Customer.Name,
                                       createDate = inv.CreateDate,
                                       isPrinted = inv.IsPrinted,
                                       isPrintedMEssage = (inv.IsPrinted) ? "Gedruckt/Gebucht" : "Offen",
                                       recipient = inv.InvoiceRecipient,
                                       invoiceNumber = inv.InvoiceNumber != null ? inv.InvoiceNumber.Number.ToString() : String.Empty
                                   };
                e.Result = invoiceQuery;
            }
            else
            {
                var invoiceQuery = from inv in con.Invoice
                                   where (inv.canceled == null || inv.canceled == false)
                                   orderby inv.CreateDate descending
                                   select new
                                   {
                                       invoiceId = inv.Id,
                                       customerNumber = inv.Customer.CustomerNumber,
                                       Matchcode = inv.Customer.MatchCode,
                                       customerName = //inv.Customer.Name,     
                                      inv.Customer.SmallCustomer != null &&
                                        inv.Customer.SmallCustomer.Person != null ?
                                        inv.Customer.SmallCustomer.Person.FirstName + " " +
                                        inv.Customer.SmallCustomer.Person.Name : inv.Customer.Name,
                                       createDate = inv.CreateDate,
                                       isPrinted = inv.IsPrinted,
                                       isPrintedMEssage = (inv.IsPrinted) ? "Gedruckt/Gebucht" : "Offen",
                                       recipient = inv.InvoiceRecipient,
                                       invoiceNumber = inv.InvoiceNumber != null ? inv.InvoiceNumber.Number.ToString() : String.Empty
                                   };
                e.Result = invoiceQuery;
            }
        }
        #endregion
        #region Button Clicked
        /// <summary>
        /// Erstellt eine neue Rechnung
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RechnungErstellen_Click(object sender, EventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
            try
            {
                if (RadGridAbrechnungErstellen.SelectedItems.Count > 0)
                {
                    RechnungVorschauErrorLabel.Visible = false;
                    foreach (GridDataItem item in RadGridAbrechnungErstellen.SelectedItems)
                    {
                        var invoiceID = Int32.Parse(item["invoiceId"].Text);
                        TransactionScope ts = null;
                        if (invoiceID != null)
                        {
                            var myErloeskonten = Accounts.generateAccountNumber(dbContext, invoiceID).ToList();
                            using (ts = new TransactionScope())
                            {
                                if (item.ChildItem.NestedTableViews[0].Items.Count > 0)
                                {
                                    foreach (GridDataItem myHelperItem in item.ChildItem.NestedTableViews[0].Items)
                                    {
                                        RadTextBox helperTextbox = (myHelperItem.FindControl("AccountText") as RadTextBox);
                                        Label itemId = ((Label)myHelperItem.FindControl("lblItemId")) as Label;
                                        if (helperTextbox == null || itemId == null)
                                        {
                                            throw new Exception("Fehler, bitte wiederholen Sie den Vorgang");
                                        }
                                        if (helperTextbox.Text == string.Empty && myErloeskonten.FirstOrDefault(s => s.InvoiceItemId == Int32.Parse(itemId.Text)) == null)
                                        {
                                            throw new Exception("Alle Rechnungspositionen müssen mind. einem Erlöskonto zugewiesen sein");
                                        }
                                        if (helperTextbox.Text != string.Empty)
                                        {
                                            var Contains = myErloeskonten.FirstOrDefault(q => q.InvoiceItemId == Int32.Parse(itemId.Text));
                                            if (Contains != null && Contains.AccountNumber != helperTextbox.Text)
                                            {
                                                Contains.AccountNumber = helperTextbox.Text;
                                            }
                                            if (Contains == null)
                                            {
                                                _Accounts acc = new _Accounts
                                                {
                                                    AccountNumber = helperTextbox.Text,
                                                    InvoiceItemId = Int32.Parse(itemId.Text)
                                                };
                                                myErloeskonten.Add(acc);
                                            }
                                        }
                                    }
                                }
                                var invoiceItems = from invitem in dbContext.InvoiceItem
                                                   join inv in dbContext.Invoice on invitem.InvoiceId equals inv.Id
                                                   where inv.Id == invoiceID
                                                   select new
                                                   {
                                                       ItemId = invitem.Id,
                                                   };
                                if (invoiceItems.Count() == 0)
                                {
                                    throw new Exception("Es gibt keine Rechnungspositionen, deshalb kann die Rechnung nicht erstellt werden");
                                }
                                if (!((myErloeskonten.Count() > 0 && invoiceItems.Count() > 0) && (myErloeskonten.Count() == invoiceItems.Count())))
                                {
                                    throw new Exception("Aller Rechnungspositionen müssen mind. einem Erlöskonto zugewiesen sein");
                                }
                                foreach (var thisItems in myErloeskonten)
                                {
                                    var myAccount = new InvoiceItemAccountItem
                                    {
                                        InvoiceItemId = thisItems.InvoiceItemId,
                                        RevenueAccountText = thisItems.AccountNumber
                                    };
                                    var contains = dbContext.InvoiceItemAccountItem.FirstOrDefault(q => q.InvoiceItemId == thisItems.InvoiceItemId && 
                                        q.RevenueAccountText == thisItems.AccountNumber.Trim());
                                    if (contains == null)
                                    {
                                        dbContext.InvoiceItemAccountItem.InsertOnSubmit(myAccount);
                                    }
                                    else
                                    {
                                        contains.RevenueAccountText = thisItems.AccountNumber;
                                    }

                                    dbContext.SubmitChanges();



                                }
                                Invoice newInvoice = dbContext.Invoice.SingleOrDefault(q => q.Id == invoiceID);

                                if (newInvoice.InvoiceItem.Count == 0)
                                {
                                    throw new Exception("Die Rechnung konnte nicht erstellt werden, da für diese Rechnung keine Rechnungspositionen verbucht wurden");
                                }
                                if (newInvoice.IsPrinted == false)
                                {
                                    using (MemoryStream memS = new MemoryStream())
                                    {
                                        string serverPath = ConfigurationManager.AppSettings["DataPath"] + "\\UserData";
                                        newInvoice.Print(dbContext, memS, "", defaultAccountNumber.Checked);
                                        string fileName = "Rechnung_" + newInvoice.InvoiceNumber.Number + "_" + newInvoice.CreateDate.Day + "_" + 
                                            newInvoice.CreateDate.Month + "_" + newInvoice.CreateDate.Year + ".pdf";
                                        if (!Directory.Exists(serverPath)) Directory.CreateDirectory(serverPath);
                                        if (!Directory.Exists(serverPath + "\\" + Session["CurrentUserId"].ToString())) 
                                            Directory.CreateDirectory(serverPath + "\\" + Session["CurrentUserId"].ToString());
                                        
                                        serverPath = serverPath + "\\" + Session["CurrentUserId"].ToString();
                                        File.WriteAllBytes(serverPath + "\\" + fileName, memS.ToArray());
                                        OpenPrintfile(fileName);
                                        dbContext.SubmitChanges();
                                    }
                                }
                                ts.Complete();
                            }
                        }
                    }
                }
                else
                {
                    RechnungVorschauErrorLabel.Visible = true;
                }
            }
            catch (Exception ex)
            {
                RechnungVorschauErrorLabel.Visible = true;
                RechnungVorschauErrorLabel.Text = "Fehler bei der Rechnungserstellung: " + ex.Message;
                dbContext.WriteLogItem("Abrechnung Error " + ex.Message, LogTypes.ERROR, "Abrechnung");
            }
            finally
            {
                RadGridAbrechnungErstellen.MasterTableView.ClearChildEditItems();
                RadGridAbrechnungErstellen.MasterTableView.ClearEditItems();
                RadGridAbrechnungErstellen.Rebind();
            }
        }
        /// <summary>
        /// Zeige die erstellte Rechnung
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ShowInvoiceButton_Clicked(object sender, EventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
            try
            {
                if (RadGridAbrechnungErstellen.SelectedItems.Count > 0)
                {
                    RechnungVorschauErrorLabel.Visible = false;
                    foreach (GridDataItem item in RadGridAbrechnungErstellen.SelectedItems)
                    {
                        if (!String.IsNullOrEmpty(item["invoiceId"].Text))
                        {
                            var invoiceID = Int32.Parse(item["invoiceId"].Text);
                            Invoice newInvoice = dbContext.Invoice.SingleOrDefault(q => q.Id == invoiceID);
                            if (newInvoice.DocumentId != null)
                            {
                                using (MemoryStream memS = new MemoryStream(newInvoice.Document.Data.ToArray()))
                                {
                                    string serverPath = ConfigurationManager.AppSettings["DataPath"] + "\\UserData";
                                    if (!Directory.Exists(serverPath)) Directory.CreateDirectory(serverPath);
                                    if (!Directory.Exists(serverPath + "\\" + Session["CurrentUserId"].ToString())) Directory.CreateDirectory(serverPath + "\\" + Session["CurrentUserId"].ToString());
                                    serverPath = serverPath + "\\" + Session["CurrentUserId"].ToString();
                                    File.WriteAllBytes(serverPath + "\\" + newInvoice.Document.FileName, memS.ToArray());
                                    OpenPrintfile(newInvoice.Document.FileName);
                                }
                            }
                            else
                            {
                                throw new Exception("Die Rechnung befindet sich nicht in der Datenbank");
                            }
                        }
                    }
                }
                else
                {
                    RechnungVorschauErrorLabel.Visible = true;
                }
            }
            catch (Exception ex)
            {
                RechnungVorschauErrorLabel.Visible = true;
                RechnungVorschauErrorLabel.Text = "Fehler bei der Rechnungsvorschau: " + ex.Message;
                dbContext.WriteLogItem("Abrechnung Error " + ex.Message, LogTypes.ERROR, "Abrechnung");
            }
            finally
            {
                RadGridAbrechnungErstellen.MasterTableView.ClearChildEditItems();
                RadGridAbrechnungErstellen.MasterTableView.ClearEditItems();
                RadGridAbrechnungErstellen.Rebind();
            }
        }
        /// <summary>
        /// Oeffne das erstellte Dokument
        /// </summary>
        /// <param name="myFile"></param>
        private void OpenPrintfile(string myFile)
        {
            string url = ConfigurationManager.AppSettings["BaseUrl"];
            string path = url + "UserData/" + Session["CurrentUserId"].ToString() + "/" + myFile;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Invoice", "<script>openFile('" + path + "');</script>", false);
        }
        /// <summary>
        /// Zeige alle Erloeskonten zur gewaehlten Dienstleistung
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmbErloeskontenThisProducts_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            RadComboBox cmbErloeskonten = ((RadComboBox)sender);
            Label itemId = cmbErloeskonten.Parent.FindControl("lblItemId") as Label;
            bool printed = false;
            if (cmbErloeskonten.Parent.Parent is GridDataItem)
            {
                GridDataItem s = ((GridDataItem)cmbErloeskonten.Parent.Parent);
                if (s != null)
                {
                    printed = bool.Parse(s["IsPrinted"].Text);
                }
            }
            cmbErloeskonten.Items.Clear();
            cmbErloeskonten.Text = string.Empty;
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            cmbErloeskonten.DataSource = Accounts.generateAccountNumber(dbContext, Int32.Parse(itemId.Text), printed);
            cmbErloeskonten.Text = "";
            cmbErloeskonten.DataBind();
        }
        protected void AddButton_Clicked(object sender, EventArgs e)
        {
        }
        #endregion
    }
}