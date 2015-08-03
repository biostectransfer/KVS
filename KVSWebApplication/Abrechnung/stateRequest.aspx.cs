using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using KVSWebApplication.BasePages;

namespace KVSWebApplication.Abrechnung
{
    public partial class stateRequest : BasePage
    {   
        /// <summary>
        /// Fragt den aktuellen Status zum Rechnungslauf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string id = Request.Form["invoiceRunId"];
            int? write = 0;

            if (!String.IsNullOrEmpty(id))
            {
                write = InvoiceManager.GetInvoiceRunReports().FirstOrDefault(q => q.Id == Int32.Parse(id)).InvoiceRunProgress;
                if (write == null)
                    write = null;
                if (write > 100)
                    write = 100;
            }

            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Write(write);
            Response.Flush();
            Response.End();
        }
    }
}