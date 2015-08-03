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
using KVSWebApplication.BasePages;
using KVSCommon.Enums;

namespace KVSWebApplication.Abrechnung
{
    /// <summary>
    /// Codebehind fuer die stornierten Rechnungen
    /// </summary>
    public partial class StornierteRechnungen : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var abr = Page as Abrechnung;
            var script = abr.getScriptManager() as RadScriptManager;
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
            if (UserManager.CheckPermissionsForUser(Session["UserPermissions"], PermissionTypes.RECHNUNG_ERSTELLEN))
            {
                AllButtonsPanel.Visible = true;
            }
            else
            {
                AllButtonsPanel.Visible = false;
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

                        var newInvoice = InvoiceManager.GetById(invoiceID);

                        if (newInvoice.InvoiceItem == null || newInvoice.InvoiceItem.Count == 0)
                            throw new Exception("Diese Rechnung enthält keine Positionen");

                        using (MemoryStream memS = new MemoryStream())
                        {
                            string fileName = "RechnungsVorschau_" + DateTime.Now.Ticks.ToString() + ".pdf";
                            string serverPath = ConfigurationManager.AppSettings["DataPath"] + "\\UserData";

                            InvoiceManager.PrintPreview(newInvoice, memS, "");

                            if (!Directory.Exists(serverPath))
                                Directory.CreateDirectory(serverPath);

                            if (!Directory.Exists(serverPath + "\\" + Session["CurrentUserId"].ToString()))
                                Directory.CreateDirectory(serverPath + "\\" + Session["CurrentUserId"].ToString());

                            serverPath = serverPath + "\\" + Session["CurrentUserId"].ToString();
                            File.WriteAllBytes(serverPath + "\\" + fileName, memS.ToArray());
                            OpenPrintfile(fileName);
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
            var invoiceId = Int32.Parse(e.WhereParameters["InvoiceId"].ToString());
            var invoiceAccounts = InvoiceItemAccountItemManager.GetAccountNumbers(invoiceId).ToList();

            var invoiceItems = InvoiceManager.GetEntities(o => o.Id == invoiceId && o.canceled == true).
                SelectMany(o => o.InvoiceItem).
                Select(invitem => new InnerItems
                {
                    ItemId = invitem.Id,
                    OrderItemId = invitem.OrderItemId,
                    InvoiceId = invitem.InvoiceId,
                    Amount = invitem.Amount,
                    Count = invitem.Count,
                    Name = invitem.Name,
                    isPrinted = invitem.Invoice.IsPrinted,
                    AccountId = 0,
                    AccountNumber = "",
                    InvoiceItemId = invitem.Id,
                }).ToList();

            foreach (var invItem in invoiceItems)
            {
                var account = invoiceAccounts.FirstOrDefault(o => o.InvoiceItemId == invItem.ItemId);

                if (account != null)
                {
                    invItem.AccountId = account.AccountId;
                    invItem.AccountNumber = account.AccountNumber;
                    invItem.InvoiceItemId = account.InvoiceItemId;
                }
            }

            e.Result = invoiceItems;
        }

        /// <summary>
        /// DataSource fuer den Kunden
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

        /// <summary>
        /// Datasource fuer die Abrechnung
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AbrechnungLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            var customerId = 0;
            if (CustomerDropDownList.SelectedValue != string.Empty)
                customerId = Int32.Parse(CustomerDropDownList.SelectedValue);

            if (customerId != 0)
            {
                var invoices = InvoiceManager.GetEntities(inv => inv.CustomerId == customerId
                                  && inv.IsPrinted == false && inv.canceled == true).Select(inv => new
                                  {
                                      invoiceId = inv.Id,
                                      customerNumber = inv.Customer.CustomerNumber,
                                      Matchcode = inv.Customer.MatchCode,
                                      createDate = inv.CreateDate,
                                      isPrinted = (inv.IsPrinted) ? "Gedruckt/Gebucht" : "Offen",
                                      recipient = inv.InvoiceRecipient,
                                      invoiceNumber = inv.InvoiceNumber != null ? inv.InvoiceNumber.Number.ToString() : String.Empty
                                  });

                e.Result = invoices.OrderByDescending(o => o.createDate).ToList();
            }
            else
            {
                var invoices = InvoiceManager.GetEntities(inv => inv.IsPrinted == false && inv.canceled == true).
                    Select(inv => new
                    {
                        invoiceId = inv.Id,
                        customerNumber = inv.Customer.CustomerNumber,
                        Matchcode = inv.Customer.MatchCode,
                        createDate = inv.CreateDate,
                        isPrinted = (inv.IsPrinted) ? "Gedruckt/Gebucht" : "Offen",
                        recipient = inv.InvoiceRecipient,
                        invoiceNumber = inv.InvoiceNumber != null ? inv.InvoiceNumber.Number.ToString() : String.Empty,
                        customerName = inv.Customer.SmallCustomer != null && inv.Customer.SmallCustomer.Person != null ?
                                        inv.Customer.SmallCustomer.Person.FirstName + " " +
                                        inv.Customer.SmallCustomer.Person.Name : 
                                        inv.Customer.Name,
                    });

                e.Result = invoices.OrderByDescending(o => o.createDate).ToList();
            }
        }

        #endregion   
    }
}