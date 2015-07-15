using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
namespace KVSWebApplication.ImportExport
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        /// <summary>
        /// Gibt die Mastergrid zurück (Informationen zur allen Rechnungen)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void getAllInvoiceDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            var query = from cust in dbContext.Customer
                        join inv in dbContext.Invoice on cust.Id equals inv.CustomerId
                        join inn in dbContext.InvoiceNumber on inv.Id equals inn.InvoiceId
                        select new
                        {
                            TableId = "Outer",
                            InvoiceNumber_ = inn.Number,
                            CustomerId = cust.Id,
                            CreateDate = inv.CreateDate,
                            Printed = (inv.IsPrinted) ? "Ja" : "Nein",
                            inv.PrintDate,
                            InvoiceRecipient = inv.InvoiceRecipient,
                            CustomerName = cust.Name,
                            CustomerNumber = cust.CustomerNumber,
                            DebitorNumber = cust.Debitornumber,
                            InternalAccountNumber = cust.MatchCode
                        };
            e.Result = query;
        }
    }
}