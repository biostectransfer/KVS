using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;

namespace KVSWebApplication.Customer
{
    public partial class CustomerDetails : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void GetAllCustomerDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();

            var query = from cust in dbContext.Customer
                        select cust;
            e.Result = query;
        }
    }
}