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
using System.Text;
using KVSWebApplication.BasePages;

namespace KVSWebApplication.ImportExport
{
    /// <summary>
    /// Codebehind fuer den Rechnungsexport
    /// </summary>
    public partial class ExportInvoices : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Session["SelectedInvoices"] = new Dictionary<string, bool>();
            }
            ImportExport abr = Page as ImportExport;
            ScriptManager script = abr.getScriptManager() as ScriptManager;
            script.RegisterPostBackControl(ExportSelected);
        }

        /// <summary>
        /// Gibt die Mastergrid zurück (Informationen zur allen Rechnungen)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void getAllInvoiceDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var invoices = InvoiceManager.GetEntities(o => o.IsPrinted).Select(inv => new
            {
                TableId = "Outer",
                InvoiceNumber = inv.InvoiceNumber.Number,
                InvoiceId = inv.Id,
                CustomerId = inv.CustomerId,
                CreateDate = inv.CreateDate,
                Printed = (inv.IsPrinted) ? "Ja" : "Nein",
                inv.PrintDate,
                InvoiceRecipient = inv.InvoiceRecipient,
                CustomerName = inv.Customer.SmallCustomer != null && inv.Customer.SmallCustomer.Person != null ?
                                                    inv.Customer.SmallCustomer.Person.FirstName + " " + inv.Customer.SmallCustomer.Person.Name :
                                                    inv.Customer.Name,
                CustomerNumber = inv.Customer.CustomerNumber,
                DebitorNumber = inv.Customer.Debitornumber,
                InternalAccountNumber = inv.Customer.MatchCode,
                NettoSum = inv.NetSum,
                Discount = inv.discount
            });

            if (CustomerNameBox.SelectedIndex > 0)
            {
                invoices = invoices.Where(q => q.CustomerId == Int32.Parse(CustomerNameBox.SelectedValue));
            }
            if (RechnungsnummerSearchBox.Text != string.Empty && isNumber(RechnungsnummerSearchBox.Text))
            {
                invoices = invoices.Where(q => q.InvoiceNumber == int.Parse(RechnungsnummerSearchBox.Text));
            }
            if (DebitorennummerSearchBox.Text != string.Empty)
            {
                invoices = invoices.Where(q => q.DebitorNumber == DebitorennummerSearchBox.Text);
            }
            if (RechnungsempfaengerSearchBox.Text != string.Empty)
            {
                invoices = invoices.Where(q => q.InvoiceRecipient == RechnungsempfaengerSearchBox.Text);
            }
            if (KontonummerSearchBox.Text != string.Empty)
            {
                invoices = invoices.Where(q => q.InternalAccountNumber == KontonummerSearchBox.Text);
            }
            if (FromDateSearchBox.SelectedDate != null)
            {
                invoices = invoices.Where(q => q.CreateDate >= FromDateSearchBox.SelectedDate);
            }
            if (ToDateSearchBox.SelectedDate != null)
            {
                invoices = invoices.Where(q => q.CreateDate <= ToDateSearchBox.SelectedDate);
            }

            e.Result = invoices.ToList();
        }

        private bool isNumber(string text)
        {
            try
            {
                long.Parse(text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected void searchButton_Click(object sender, EventArgs e)
        {
            getAllInvoices.DataBind();
        }

        protected void CustomerName_Selected(object sender, LinqDataSourceSelectEventArgs e)
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
            CustomerNameBox.ClearSelection();
        }

        protected void getAllInvoice_RowDataBound(Object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)// to access a row 
            {
                var item = (GridDataItem)e.Item;
                var cell = item["InvoiceId"];
                if (cell != null && cell.Text != "")
                {
                    var invoice = InvoiceManager.GetById(int.Parse(cell.Text));
                    cell = item["BruttoBetrag"];
                    cell.Text = (invoice.GrandTotal).ToString("C2");
                }

                item = (GridDataItem)e.Item;
                var chk = (CheckBox)item["Exporting"].FindControl("ChbExporting");
                chk.Enabled = true;

                if (Session["SelectedInvoices"] != null)
                {
                    cell = item["InvoiceId"];
                    var helper = (Dictionary<string, bool>)Session["SelectedInvoices"];
                    bool isInDic = false;
                    helper.TryGetValue(cell.Text, out isInDic);
                    if (isInDic)
                    {
                        chk.Checked = helper[cell.Text];
                    }
                }
            }
        }

        protected void CheckMyInvoice(object sender, EventArgs e)
        {
            var chekInvoice = (CheckBox)sender;
            var invoiceId = ((CheckBox)sender).Parent.FindControl("InvoiceId") as Label;
            if (invoiceId != null && Session["SelectedInvoices"] != null)
            {
                var helper = (Dictionary<string, bool>)Session["SelectedInvoices"];
                bool isInDic = false;
                if (helper.TryGetValue(invoiceId.Text, out isInDic))
                {
                    helper[invoiceId.Text] = chekInvoice.Checked;
                }
                else
                {
                    helper.Add(invoiceId.Text, chekInvoice.Checked);
                }
            }
        }

        protected void PrintSelectedInvoices_Click(object sender, EventArgs e)
        {
            if (Session["SelectedInvoices"] != null)
            {
                try
                {
                    var selectedInvoices = (Dictionary<string, bool>)Session["SelectedInvoices"];
                    var invoiceList = new List<VirtualInvoice>();

                    foreach (var key in selectedInvoices.Where(o => o.Value).Select(o => o.Key))
                    {
                        var invoice = InvoiceManager.GetById(Int32.Parse(key));

                        foreach (var groupedItems in invoice.InvoiceItem.GroupBy(o => o.InvoiceItemAccountItem.FirstOrDefault().RevenueAccountText))
                        {
                            var totalSum = groupedItems.Sum(o => { return ((o.Amount * (decimal)o.Count) / (decimal)100) * o.VAT + (o.Amount * (decimal)o.Count); });

                            invoiceList.Add(new VirtualInvoice
                            {
                                account = groupedItems.Key,
                                Invoice = invoice,
                                accountSum = totalSum
                            });

                            //IQueryable<VirtualInvoice> myInvoice = (from i in dbContext.Invoice
                            //                                        join invoiceitem in dbContext.InvoiceItem on i.Id equals invoiceitem.InvoiceId
                            //                                        join IIACI in dbContext.InvoiceItemAccountItem on invoiceitem.Id equals IIACI.InvoiceItemId
                            //                                        where i.InvoiceNumber.Number == int.Parse(key)
                            //                                        group IIACI by new { IIACI.RevenueAccountText, invoiceitem.Invoice } into last
                            //                                        select new VirtualInvoice
                            //                                        {
                            //                                            account = last.Key.RevenueAccountText,
                            //                                            Invoice = last.Key.Invoice,
                            //                                            accountSum = (from invCount in last.Key.Invoice.InvoiceItem
                            //                                                          join myaccount in dbContext.InvoiceItemAccountItem on new { Id = invCount.Id, text = last.Key.RevenueAccountText }
                            //                                                          equals
                            //                                                          new { Id = myaccount.InvoiceItemId, text = myaccount.RevenueAccountText }
                            //                                                          select new { invCount }).Sum(s => (s.invCount.Amount * s.invCount.Count * s.invCount.VAT / 100) + (s.invCount.Amount * s.invCount.Count))
                            //                                        });
                            //foreach (var item in myInvoice)
                            //{
                            //    invoiceList.Add(item);
                            //}
                        }
                    }


                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();

                    //process DateV export
                    string result = DateVExport.CreateExport(invoiceList);


                    string appPath = ConfigurationManager.AppSettings["ExportPath"];
                    string myFileName = "DE001";
                    string completeFileName = appPath + "\\export\\" + myFileName;
                    if (Directory.Exists(appPath))
                    {
                        var ascii = new ASCIIEncoding();
                        var byteArray = Encoding.UTF8.GetBytes(result);
                        var asciiArray = Encoding.Convert(Encoding.UTF8, Encoding.ASCII, byteArray);
                        var finalString = ascii.GetString(asciiArray);

                        if (!Directory.Exists(appPath + "\\export"))
                            Directory.CreateDirectory(appPath + "\\export");

                        var wr = new StreamWriter(completeFileName);
                        wr.Write(finalString);
                        wr.Close();
                        wr.Dispose();

                        Response.AddHeader("Content-Disposition", "attachment; filename=" + myFileName);
                        Response.AddHeader("Content-Length", new FileInfo(completeFileName).Length.ToString());
                        Response.ContentType = "text/plain";
                        Response.TransmitFile(new FileInfo(completeFileName).FullName);
                        Response.Flush();
                        Response.End();
                    }
                    else
                    {
                        throw new Exception("Der Application Path existiert nicht!");
                    }
                }
                catch (Exception ex)
                {
                    //TODO WriteLogItem("Fehler beim Erstellen des Rechnungsexports:" + ex.Message, LogTypes.ERROR);
                    Page.ClientScript.RegisterStartupScript(typeof(ExportInvoices), "alertError", "alert('Achtung, beim erstellen der Rechnungsexports ist Fehler aufgetretten. Bitte versuchen Sie es erneut. Fehler: " + ex.Message + "');", true);
                }
            }
        }
    }
}