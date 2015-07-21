using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using System.Data.Linq;
using System.Text.RegularExpressions;
namespace KVSWebApplication.ChangeHistory
{
    /// <summary>
    /// Maske fuer die Auftragshistorie 
    /// </summary>
    public partial class OrderHistory : System.Web.UI.Page
    {
        PageStatePersister _pers;
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                if (_pers == null)
                {
                    _pers = new SessionPageStatePersister(Page);
                }
                return _pers;
            }
        }
        List<string> thisUserPermissions = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(Int32.Parse(Session["CurrentUserId"].ToString())));
            if (!thisUserPermissions.Contains("CHANGELOG"))
            {
                Response.Redirect("../AccessDenied.aspx");
            }
        }
        protected void AllChangesLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var changes = from chang in con.ChangeLog
                          let tableName = chang.TableName
                          let status = chang.TableProperty
                          let allStates = con.OrderStatus
                          let myOrder = con.Order
                          let myOrderItem=con.OrderItem
                          let Ref = (!chang.ReferenceId.HasValue)? (int?)null : chang.ReferenceId.Value
                          where chang.TableName == "Order" || chang.TableName == "OrderItem"
                          select new OrderLogging
                          {
                              LogId = chang.Expr1,
                              TableName = tableName,
                              Name = chang.Name,
                              FirstName = chang.FirstName,
                              Login = chang.Login,
                              Type= tableName=="Order" ? "Auftrag":"Auftragsposition",
                              Date = chang.Date,
                              ReferenceId = (!chang.ReferenceId.HasValue)? (int?)null : chang.ReferenceId.Value,
                              TranslatedText = TranslatedText(status,chang.Text, allStates),
                              OrderNumber = tableName == "Order" ? myOrder.FirstOrDefault(q => q.Id == Ref).OrderNumber : myOrderItem.FirstOrDefault(q => q.Id == Ref).Order.OrderNumber
                          };
            changes = changes.OrderByDescending(q=>q.OrderNumber);
            e.Result = changes;
        }
        protected string TranslatedText(string state, string TrText, Table<OrderStatus> PossibleOrderStates)
        {
            if (state == "Status")
            {
                string[] states = Regex.Split(TrText, @"\D+");
                foreach (string number in states)
                {
                    if (number != string.Empty)
                    {
                        int n = int.Parse(number);
                        TrText = TrText.Replace(number, PossibleOrderStates.FirstOrDefault(q => q.Id == n).Name);
                    }
                }
            }
            return TrText;            
        }
    }
}