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
namespace KVSWebApplication.ImportExport
{
    /// <summary>
    /// Codebehind fuer den Rechnungsexport
    /// </summary>
    public partial class ExportInvoices : System.Web.UI.UserControl
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
            KVSEntities dbContext = new KVSEntities();
            var query = from cust in dbContext.Customer 
                        join inv in dbContext.Invoice on cust.Id equals  inv.CustomerId
                        join inn in dbContext.InvoiceNumber on inv.Id equals inn.InvoiceId
                        where inv.IsPrinted == true
                        select new
                        {
                            TableId = "Outer",
                            InvoiceNumber_ = inn.Number,
                            CustomerId=cust.Id,
                            CreateDate= inv.CreateDate,
                            Printed = (inv.IsPrinted)? "Ja": "Nein",
                            inv.PrintDate,
                            InvoiceRecipient = inv.InvoiceRecipient,
                            CustomerName = //cust.Name,
                               cust.SmallCustomer != null &&
                                        cust.SmallCustomer.Person != null ?
                                        cust.SmallCustomer.Person.FirstName + " " +
                                        cust.SmallCustomer.Person.Name : cust.Name, 
                            CustomerNumber = cust.CustomerNumber,
                            DebitorNumber = cust.Debitornumber,
                            InternalAccountNumber = cust.MatchCode,
                            NettoSum = inv.NetSum,
                            Discount = inv.discount                            
                        };
            if (CustomerNameBox.SelectedIndex > 0)
            {
                query = query.Where(q => q.CustomerId == Int32.Parse(CustomerNameBox.SelectedValue));
            }
            if (RechnungsnummerSearchBox.Text != string.Empty && isNumber(RechnungsnummerSearchBox.Text))
            {
                query = query.Where(q => q.InvoiceNumber_ == int.Parse(RechnungsnummerSearchBox.Text));
            }
            if (DebitorennummerSearchBox.Text != string.Empty)
            {
                query = query.Where(q => q.DebitorNumber == DebitorennummerSearchBox.Text);
            }
            if (RechnungsempfaengerSearchBox.Text != string.Empty)
            {
                query = query.Where(q => q.InvoiceRecipient == RechnungsempfaengerSearchBox.Text);
            }
            if (KontonummerSearchBox.Text != string.Empty)
            {
                query = query.Where(q => q.InternalAccountNumber == KontonummerSearchBox.Text);
            }
            if (FromDateSearchBox.SelectedDate != null)
            {
                query = query.Where(q => q.CreateDate >= FromDateSearchBox.SelectedDate);
            }
            if (ToDateSearchBox.SelectedDate != null)
            {
                query = query.Where(q => q.CreateDate <= ToDateSearchBox.SelectedDate);
            }      
            e.Result = query;
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
            KVSEntities con = new KVSEntities();
            var CustomerName = from cust in con.Customer
                               select new
                               {
                                   Name = cust.SmallCustomer != null && cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name, 
                                   Value = cust.Id, Matchcode = cust.MatchCode, Kundennummer = cust.CustomerNumber 
                               };
            e.Result = CustomerName;
        }
        protected void clearButton_Click(object sender, EventArgs e)
        {
         CustomerNameBox.ClearSelection();
        }
        protected void getAllInvoice_RowDataBound(Object sender, GridItemEventArgs e)
            {
            if (e.Item is GridDataItem)// to access a row 
            {              
                GridDataItem item = (GridDataItem)e.Item;
                TableCell cell = item["InvoiceNumber_"];
                if (cell != null && cell.Text != "")
                {
                    using (KVSEntities dbContext = new KVSEntities())
                    {
                        var myInvoice = dbContext.Invoice.FirstOrDefault(q => q.InvoiceNumber.Number == int.Parse(cell.Text));
                        //cell = item["NettoSum"];
                        //cell.Text = myInvoice.NetSum.ToString();
                        cell = item["BruttoBetrag"];
                        cell.Text = (myInvoice.GrandTotal).ToString("C2");
                    }
                }
                item = (GridDataItem)e.Item;
                CheckBox chk = (CheckBox)item["Exporting"].FindControl("ChbExporting");
                chk.Enabled = true;
                if (Session["SelectedInvoices"] != null)
                {
                    cell = item["InvoiceNumber_"];
                    Dictionary<string, bool> myHelper = (Dictionary<string, bool>)Session["SelectedInvoices"];
                    bool isInDic = false;
                    myHelper.TryGetValue(cell.Text, out isInDic);
                    if (isInDic)
                    {
                        chk.Checked = myHelper[cell.Text]; 
                    }
                }
            }
        }
        protected void CheckMyInvoice(object sender, EventArgs e)
        {
            CheckBox s = (CheckBox)sender;
            Label l = ((CheckBox)sender).Parent.FindControl("myInvoiceNumber") as Label;
            if (l != null && Session["SelectedInvoices"] != null)
            {
                Dictionary<string, bool> myHelper = (Dictionary<string, bool>)Session["SelectedInvoices"];
                bool isInDic = false;
                    //myHelper.TryGetValue(l.Text, out isInDic);
                if (myHelper.TryGetValue(l.Text, out isInDic))
                {
                    myHelper[l.Text] = s.Checked;
                }
                else
                {
                    myHelper.Add(l.Text,s.Checked);
                }
            }
        }
        protected void PrintSelectedInvoices_Click(object sender, EventArgs e)
        {
            if (Session["SelectedInvoices"] != null)
            {              
                using (KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())))
                {
                    try
                    {
                    Dictionary<string, bool> myHelper = (Dictionary<string, bool>)Session["SelectedInvoices"];
                    var myInvoices = dbContext.Invoice;
                    List<VirtualInvoice> myInvoiceList = new List<VirtualInvoice>();                             
                    foreach (var key in (from helper in myHelper where helper.Value == true select new { helper.Key }).ToArray())
                    {
                        IQueryable<VirtualInvoice> myInvoice = (from i in dbContext.Invoice
                                                                join invoiceitem in dbContext.InvoiceItem on i.Id equals invoiceitem.InvoiceId
                                                                join IIACI in dbContext.InvoiceItemAccountItem on invoiceitem.Id equals IIACI.InvoiceItemId
                                                                where i.InvoiceNumber.Number == int.Parse(key.Key)
                                                                group IIACI by new { IIACI.RevenueAccountText, invoiceitem.Invoice } into last
                                                                select new VirtualInvoice
                                                                {
                                                                    account = last.Key.RevenueAccountText,
                                                                    Invoice = last.Key.Invoice,
                                                                    accountSum = (from invCount in last.Key.Invoice.InvoiceItem
                                                                                    join myaccount in dbContext.InvoiceItemAccountItem on new { Id = invCount.Id, text = last.Key.RevenueAccountText }
                                                                                    equals
                                                                                    new { Id = myaccount.InvoiceItemId, text = myaccount.RevenueAccountText }
                                                                                    select new { invCount }).Sum(s => (s.invCount.Amount * s.invCount.Count * s.invCount.VAT / 100) + (s.invCount.Amount * s.invCount.Count))
                                                                });
                        foreach (VirtualInvoice ins in myInvoice)
                        {
                            myInvoiceList.Add(ins);
                        }
                    }
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Rechnungsexport r = new Rechnungsexport(myInvoiceList);
                    string result = r.CompleteByteString;
                    string appPath = ConfigurationManager.AppSettings["ExportPath"];
                    // string myFileName = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + "_" + DateTime.Now.Ticks;
                    string myFileName = "DE001";
                    string completeFileName = appPath + "\\export\\" + myFileName;
                    if (Directory.Exists(appPath))
                    {                        
                        ASCIIEncoding ascii = new ASCIIEncoding();
                        byte[] byteArray = Encoding.UTF8.GetBytes(result);
                        byte[] asciiArray = Encoding.Convert(Encoding.UTF8, Encoding.ASCII, byteArray);
                        string finalString = ascii.GetString(asciiArray);
                        if (!Directory.Exists(appPath + "\\export")) { Directory.CreateDirectory(appPath + "\\export"); }
                        StreamWriter wr = new StreamWriter(completeFileName);
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
                    dbContext.WriteLogItem("Fehler beim Erstellen des Rechnungsexports:" + ex.Message, LogTypes.ERROR);
                    dbContext.SubmitChanges();
                    Page.ClientScript.RegisterStartupScript(typeof(ExportInvoices), "alertError", "alert('Achtung, beim erstellen der Rechnungsexports ist Fehler aufgetretten. Bitte versuchen Sie es erneut. Fehler: "+ex.Message+ "');",true);
                    
                }
             }               
            }
        }
    }
}