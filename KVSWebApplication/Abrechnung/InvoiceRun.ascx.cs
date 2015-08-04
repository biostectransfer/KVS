using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using KVSCommon.Database;
using KVSWebApplication.BasePages;
using KVSCommon.Enums;
using KVSCommon.Managers;
using System.Web.Http;

namespace KVSWebApplication.Abrechnung
{
    /// <summary>
    /// Codebehind fuer die Rechnungslauf Maske
    /// </summary>
    public partial class InvoiceRun : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var abr = Page as Abrechnung;
            var script = abr.getScriptManager() as RadScriptManager;
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
            return UserManager.CheckPermissionsForUser(Session["UserPermissions"], PermissionTypes.RECHNUNG_ERSTELLEN);
        }

        /// <summary>
        /// Datasource fuer die Kundenliste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            var customerQuery = CustomerManager.GetEntities().
                    Select(cust => new
                    {
                        Name = cust.SmallCustomer != null && cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name,
                        Value = cust.Id,
                        Matchcode = cust.MatchCode,
                        Kundennummer = cust.CustomerNumber
                    });

            e.Result = customerQuery.ToList();
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
            var progressQUery = InvoiceManager.GetInvoiceRunReports().
                                Select(run => new
                                {
                                    InvoiceSection = run.Id,
                                    CustomerId = run.CustomerId.HasValue ? run.CustomerId : (int?)null,
                                    CustomerName = run.CustomerId.HasValue ? CustomerManager.GetEntities().FirstOrDefault(o => o.Id == run.CustomerId.Value).Name : "Alle",
                                    InvoiceTypeId = run.InvoiceTypeId.HasValue ? InvoiceTypesManager.GetEntities().FirstOrDefault(q => q.Id == run.InvoiceTypeId).Id.ToString() : "Alle",
                                    InvoiceTypeName = run.InvoiceTypeId.HasValue ? InvoiceTypesManager.GetEntities().FirstOrDefault(q => q.Id == run.InvoiceTypeId).InvoiceTypeName : "Alle",
                                    CreateDate = run.CreateDate,
                                    FinishedDate = run.FinishedDate,
                                });

            e.Result = progressQUery.OrderByDescending(o => o.CreateDate).ToList();
        }

        /// <summary>
        /// DataSource fuer die Rechnungstypen Kombobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RechnungsTypComboBox_init(object sender, EventArgs e)
        {
            var combo = sender as RadComboBox;
            if (combo != null)
            {
                combo.Items.Clear();
                combo.Items.Add(new RadComboBoxItem("Alle", ""));

                foreach (var items in InvoiceTypesManager.GetEntities().ToList())
                {
                    combo.Items.Add(new RadComboBoxItem(items.InvoiceTypeName, items.Id.ToString()));
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

            try
            {
                int? customerId = null;
                int? invoiceTypeId = null;

                if (CustomerDropDownList.SelectedValue != string.Empty)
                    customerId = Int32.Parse(CustomerDropDownList.SelectedValue);

                if (RechnungsTypComboBox.SelectedValue != string.Empty && RechnungsTypComboBox.Enabled == true)
                    invoiceTypeId = Int32.Parse(RechnungsTypComboBox.SelectedValue);

                InvoiceManager.AddRunReport(customerId, invoiceTypeId);

                RadGridInvoiceRun.MasterTableView.ClearChildEditItems();
                RadGridInvoiceRun.MasterTableView.ClearEditItems();
                RadGridInvoiceRun.Rebind();
            }
            catch (Exception ex)
            {
                InvoiceRunError.Visible = true;
                InvoiceRunError.Text = "Rechnungslauf Fehler " + ex.Message;
                //TODO WriteLogItem("Rechnungslauf Error " + ex.Message, LogTypes.ERROR, "Rechnungslauf");
            }
        }
    }
}