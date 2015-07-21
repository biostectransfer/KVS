using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using KVSCommon.Database;

namespace KVSWebApplication.Abrechnung
{
    /// <summary>
    /// Codebehind fuer die Rechnungslauf Maske
    /// </summary>
    public partial class InvoiceRun : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Abrechnung abr = Page as Abrechnung;
            RadScriptManager script = abr.getScriptManager() as RadScriptManager;
            if (Session["CurrentUserId"] != null && CheckUserPermissions())
            {
                if (!Page.IsPostBack)
                {
                    RadGridInvoiceRun.Enabled = true;
                    RadGridInvoiceRun.DataBind();
                }
            }
            else
            {
                invoiceRunPanel.Visible = false;
            }
        }
        /// <summary>
        /// Pruefe der Benutzerrechte
        /// </summary>
        /// <returns></returns>
        protected bool CheckUserPermissions()
        {
            List<string> userPermissions = new List<string>();
            userPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(Int32.Parse(Session["CurrentUserId"].ToString())));
            if (userPermissions.Count > 0)
            {
                if (userPermissions.Contains("RECHNUNG_ERSTELLEN"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Datasource fuer die Kundenliste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();

            var customerQuery = from cust in con.Customer
                                where cust.Id == cust.LargeCustomer.CustomerId
                                select new { Name = cust.Name, Value = cust.Id, Matchcode = cust.MatchCode, Kundennummer = cust.CustomerNumber };
            e.Result = customerQuery;
        }
        protected void clearButton_Click(object sender, EventArgs e)
        {
            CustomerDropDownList.ClearSelection();
            RechnungsTypComboBox.Enabled = true;
        }
        /// <summary>
        /// DataSource fuer die Rechnungslauf Tabelle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvoiceRunLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var progressQUery = from run in con.InvoiceRunReport
                                orderby run.CreateDate descending
                                select new
                                {
                                    InvoiceSection = run.Id,
                                    CustomerId = run.CustomerId.HasValue ? run.CustomerId : (int?)null,
                                    CustomerName = (run.CustomerId.HasValue) ? con.Customer.FirstOrDefault(q => q.Id == run.CustomerId).Name : "Alle",
                                    InvoiceTypeId = (run.InvoiceTypeId.HasValue) ? con.InvoiceTypes.FirstOrDefault(q => q.ID == run.InvoiceTypeId).ID.ToString() : "Alle",
                                    InvoiceTypeName = (run.InvoiceTypeId.HasValue) ? con.InvoiceTypes.FirstOrDefault(q => q.ID == run.InvoiceTypeId).InvoiceTypeName : "Alle",
                                    CreateDate = run.CreateDate,
                                    FinishedDate = run.FinishedDate,
                                };
            e.Result = progressQUery;
        }
        /// <summary>
        /// DataSource fuer die Rechnungstypen Kombobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RechnungsTypComboBox_init(object sender, EventArgs e)
        {
            RadComboBox combo = sender as RadComboBox;
            IQueryable<InvoiceTypes> tp = null;
            if (combo != null)
            {
                DataClasses1DataContext dbContext = new DataClasses1DataContext();
                tp = dbContext.InvoiceTypes;
                combo.Items.Clear();
                combo.Items.Add(new RadComboBoxItem("Alle", ""));
                foreach (var items in tp)
                {
                    combo.Items.Add(new RadComboBoxItem(items.InvoiceTypeName, items.ID.ToString()));
                }
                combo.DataBind();
            }
        }
        /// <summary>
        /// Event fuer die Auswahl der Kunden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CustomerDropDownList_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            RechnungsTypComboBox.Enabled = false;
        }
        /// <summary>
        /// Erstellt einen neuen Rechnungslauf Datensatz
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GenerateInvoiceRunButton_Click(object sender, EventArgs e)
        {
            InvoiceRunError.Text = "";
            using (DataClasses1DataContext dbContext = new DataClasses1DataContext())
            {
                try
                {
                    int? customerId = null;
                    int? invoiceType = null;
                    
                    if (CustomerDropDownList.SelectedValue != string.Empty)
                        customerId = Int32.Parse(CustomerDropDownList.SelectedValue);
                    
                    if (RechnungsTypComboBox.SelectedValue != string.Empty && RechnungsTypComboBox.Enabled == true)
                        invoiceType = Int32.Parse(RechnungsTypComboBox.SelectedValue);
                    
                    InvoiceRunReport run = new InvoiceRunReport()
                    {
                        CustomerId = customerId,
                        InvoiceTypeId = invoiceType,
                        CreateDate = DateTime.Now
                    };
                    dbContext.InvoiceRunReport.InsertOnSubmit(run);
                    dbContext.SubmitChanges();
                    RadGridInvoiceRun.MasterTableView.ClearChildEditItems();
                    RadGridInvoiceRun.MasterTableView.ClearEditItems();
                    RadGridInvoiceRun.Rebind();
                }
                catch (Exception ex)
                {
                    InvoiceRunError.Visible = true;
                    InvoiceRunError.Text = "Rechnungslauf Fehler " + ex.Message;
                    dbContext.WriteLogItem("Rechnungslauf Error " + ex.Message, LogTypes.ERROR, "Rechnungslauf");
                }
            }
        }
    }
}