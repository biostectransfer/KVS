using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;

namespace KVSWebApplication.Auftragsbearbeitung_Neuzulassung
{
    public partial class ShowOrder : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void AuftragsDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            var query = dbContext.Order.Where(q => q.RegistrationOrder != null && q.Status==300);
            var query2 = from or in dbContext.Order
                         where or.Status == 300
                         select new
                         {
                             or.Status,
                             or.User
                         };
            e.Result = query;
        }
    }
}