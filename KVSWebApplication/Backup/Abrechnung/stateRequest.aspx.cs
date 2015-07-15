using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
namespace KVSWebApplication.Abrechnung
{
    public partial class stateRequest : System.Web.UI.Page
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
            if (EmptyStringIfNull.IsGuid(id))
            {
                using (DataClasses1DataContext dbContext = new DataClasses1DataContext())
                {
                    write = dbContext.InvoiceRunReport.FirstOrDefault(q => q.Id == new Guid(id)).InvoiceRunProgress;
                    if (write == null)
                        write = null;
                    if (write > 100)
                        write = 100;
                }
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