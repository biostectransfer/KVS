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
namespace KVSWebApplication.Abrechnung
{
    /// <summary>
    /// Codebehind fuer die stornierten Rechnungen
    /// </summary>
    public partial class StornierteRechnungen : System.Web.UI.UserControl
    {       
        protected void Page_Load(object sender, EventArgs e)
        {
            Abrechnung abr = Page as Abrechnung;
            RadScriptManager script = abr.getScriptManager() as RadScriptManager;
            script.RegisterPostBackControl(btnRechnungsvorschau);    
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
        /// Event fuer den Button zum leeren der Kundenauswahl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void clearButton_Click(object sender, EventArgs e)
        {
            CustomerDropDownList.ClearSelection();
            RadGridAbrechnungErstellen.DataBind();
        }
        /// <summary>
        /// Prueft die Benutezerberechtigungen
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
        /// Event fuer die Vorschau der Rechnung
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RechnungVorschauButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (RadGridAbrechnungErstellen.SelectedItems.Count > 0)
                {
                    RechnungVorschauErrorLabel.Visible = false;
                    foreach (GridDataItem item in RadGridAbrechnungErstellen.SelectedItems)
                    {
                        var invoiceID = Int32.Parse(item["invoiceId"].Text);
                        if (invoiceID != null)
                        {
                            DataClasses1DataContext dbContext = new DataClasses1DataContext();
                            Invoice newInvoice = dbContext.Invoice.SingleOrDefault(q => q.Id == invoiceID);
                            if (newInvoice.InvoiceItem == null || newInvoice.InvoiceItem.Count == 0)
                                throw new Exception("Diese Rechnung enthält keine Positionen");
                            using (MemoryStream memS = new MemoryStream())
                            {
                                string fileName = "RechnungsVorschau_" + DateTime.Now.Ticks.ToString() + ".pdf";
                                string serverPath = ConfigurationManager.AppSettings["DataPath"] + "\\UserData";
                                newInvoice.PrintPreview(dbContext,memS, "");
                                if (!Directory.Exists(serverPath)) Directory.CreateDirectory(serverPath);
                                if (!Directory.Exists(serverPath + "\\" + Session["CurrentUserId"].ToString())) Directory.CreateDirectory(serverPath + "\\" + Session["CurrentUserId"].ToString());
                                serverPath = serverPath + "\\" + Session["CurrentUserId"].ToString();
                                File.WriteAllBytes(serverPath + "\\" + fileName, memS.ToArray());
                                OpenPrintfile(fileName);
                            }
                        }
                    }
                }
                else
                {
                    RechnungVorschauErrorLabel.Visible = true;
                    RechnungVorschauErrorLabel.Text = "Sie haben keine Rechnung ausgewählt";
                }
            }
            catch (Exception ex)
            {
                RechnungVorschauErrorLabel.Visible = true;
                RechnungVorschauErrorLabel.Text = "Fehler: " + ex.Message;
            }
        }
        /// <summary>
        /// Oeffne das erstellte Dokument
        /// </summary>
        /// <param name="myFile"></param>
        private void OpenPrintfile(string myFile)
        {
            string url = ConfigurationManager.AppSettings["BaseUrl"];
            string path = url + "/UserData/" + Session["CurrentUserId"].ToString() + "/" + myFile;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Invoice", "<script>openFile('" + path + "');</script>", false);
        }  
        #endregion
        #region Index Changed
        protected void Cell_Selected(object sender, EventArgs e)
        {          
            if (RadGridAbrechnungErstellen.SelectedItems.Count == 0)
            {
                btnRechnungsvorschau.Enabled = false;
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
        /// Datasource fuer die Detailstabelle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DetailTable_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            var invoiceId = Int32.Parse(e.WhereParameters["InvoiceId"].ToString());

            var invoiceAccounts = Accounts.generateAccountNumber(dbContext, invoiceId).ToList();
                  var invoiceItemsQuery = (from invitem in dbContext.InvoiceItem
                                     join inv in dbContext.Invoice on invitem.InvoiceId equals inv.Id
                                           where invitem.InvoiceId == invoiceId && inv.canceled == true
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
        /// DataSource fuer den Kunden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var customerQuery = from cust in con.Customer
                                select new
                                {   Name = cust.SmallCustomer != null && cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name, 
                                    Value = cust.Id, Matchcode = cust.MatchCode, Kundennummer = cust.CustomerNumber };
            e.Result = customerQuery;
        }
       /// <summary>
       /// Datasource fuer die Abrechnung
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
                                  && inv.IsPrinted == false && inv.canceled == true
                                   orderby inv.CreateDate descending
                                   select new
                                   {
                                       invoiceId = inv.Id,
                                       //customerId = ord.CustomerId,
                                       customerNumber = inv.Customer.CustomerNumber,
                                       Matchcode = inv.Customer.MatchCode,
                                       createDate = inv.CreateDate,
                                       isPrinted = (inv.IsPrinted) ? "Gedruckt/Gebucht" : "Offen",
                                       recipient = inv.InvoiceRecipient,
                                       invoiceNumber = inv.InvoiceNumber != null ? inv.InvoiceNumber.Number.ToString() : String.Empty
                                   };
                e.Result = invoiceQuery;
            }
            else
            {
                var invoiceQuery = from inv in con.Invoice
                                   where  inv.IsPrinted == false && inv.canceled == true
                                   orderby inv.CreateDate descending
                                   select new
                                   {
                                       invoiceId = inv.Id,
                                       //customerId = ord.CustomerId,
                                       customerNumber = inv.Customer.CustomerNumber,
                                       Matchcode = inv.Customer.MatchCode,
                                       customerName = //inv.Customer.Name,
                                         inv.Customer.SmallCustomer != null &&
                                        inv.Customer.SmallCustomer.Person != null ?
                                        inv.Customer.SmallCustomer.Person.FirstName + " " +
                                        inv.Customer.SmallCustomer.Person.Name : inv.Customer.Name, 
                                       createDate = inv.CreateDate,
                                       isPrinted = (inv.IsPrinted) ? "Gedruckt/Gebucht" : "Offen",
                                       recipient = inv.InvoiceRecipient,
                                       invoiceNumber = inv.InvoiceNumber != null ? inv.InvoiceNumber.Number.ToString() : String.Empty
                                   };
                e.Result = invoiceQuery;
            }           
        }
        #endregion   
        #region Button Clicked
        protected void AddButton_Clicked(object sender, EventArgs e)
        {

        }
        #endregion    
    }
}