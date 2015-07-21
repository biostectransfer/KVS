using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
namespace KVSWebApplication
{
    /// <summary>
    /// Codebehind fuer alle Auftraege
    /// </summary>
    public partial class AllOrders : System.Web.UI.Page
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
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void ShowAllButton1_Click(object sender, EventArgs e)
        {
            RadGridAllOrders.Enabled = true;
            CustomerDropDownListOffenNeuzulassung.ClearSelection();
            RadGridAllOrders.Rebind();
        }
        protected void CustomerIndex_Changed(object sender, EventArgs e)
        {
            RadGridAllOrders.Enabled = true;
            this.RadGridAllOrders.DataBind();
        }
        protected void SmallLargeCustomerIndex_Changed(object sender, EventArgs e)
        {
            CustomerDropDownListOffenNeuzulassung.Enabled = true;
            this.CustomerDropDownListOffenNeuzulassung.DataBind();
            this.RadGridAllOrders.DataBind();
        }
        protected void ProductLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var productQuery = from prod in con.Product
                               select new
                               {
                                   ItemNumber = prod.ItemNumber,
                                   Name = prod.Name,
                                   Value = prod.Id,
                                   Category = prod.ProductCategory.Name
                               };
            e.Result = productQuery;
        }
        protected void CostCenterDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var costCenterQuery = from cost in con.CostCenter
                                  orderby cost.Name
                                  select new
                                  {
                                      Name = cost.Name,
                                      Value = cost.Id
                                  };
            e.Result = costCenterQuery;
        }
        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            if (RadComboBoxCustomerOffenNeuzulassung.SelectedValue == "1") //Small Customers
            {
                var customerQuery = from cust in con.Customer
                                    where cust.Id == cust.SmallCustomer.CustomerId
                                    select new
                                    {
                                        Name = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name,
                                        Value = cust.Id,
                                        Matchcode = cust.MatchCode,
                                        Kundennummer = cust.CustomerNumber
                                    };
                e.Result = customerQuery;
            }
            else if (RadComboBoxCustomerOffenNeuzulassung.SelectedValue == "2") //Large Customers
            {
                var customerQuery = from cust in con.Customer
                                    where cust.Id == cust.LargeCustomer.CustomerId
                                    select new { Name = cust.Name, Value = cust.Id, Matchcode = cust.MatchCode, Kundennummer = cust.CustomerNumber };
                e.Result = customerQuery;
            }
        }
        protected void RadGridAllOrders_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            var dbContext = new DataClasses1DataContext();
            var item = (GridDataItem)e.DetailTableView.ParentItem;
            var orderId = Int32.Parse(item["OrderNumber"].Text);
            var positionQuery = from ord in dbContext.Order
                                join orditem in dbContext.OrderItem on ord.OrderNumber equals orditem.OrderNumber
                                let authCharge = dbContext.OrderItem.FirstOrDefault(s => s.SuperOrderItemId == orditem.Id)
                                where ord.Id == orderId && (orditem.SuperOrderItemId == null)
                                select new
                                {
                                    OrderItemId = orditem.Id,
                                    Amount = orditem.Amount == null ? "kein Preis" : (Math.Round(orditem.Amount, 2, MidpointRounding.AwayFromZero)).ToString(),
                                    ProductName = orditem.IsAuthorativeCharge ? orditem.ProductName + " (Amtl.Gebühr)" : orditem.ProductName,
                                    AmtGebuhr = authCharge == null ? false : true,
                                    AuthCharge = authCharge == null || authCharge.Amount == null ? "kein Preis" : (Math.Round(authCharge.Amount, 2, MidpointRounding.AwayFromZero)).ToString(),
                                    AuthChargeId = authCharge == null ? (int?)null : authCharge.Id
                                };
            e.DetailTableView.DataSource = positionQuery;
        }
        protected void AllOrdersLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {       
            //select all values for small customers
            if (RadComboBoxCustomerOffenNeuzulassung.SelectedValue == "1")
            {
                DataClasses1DataContext con = new DataClasses1DataContext();
                var smallCustomerQuery = from ord in con.Order
                                         join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                         join cust in con.Customer on ord.CustomerId equals cust.Id
                                         join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                         join regord in con.RegistrationOrder on ord.OrderNumber equals regord.OrderNumber
                                         join reg in con.Registration on regord.RegistrationId equals reg.Id
                                         join veh in con.Vehicle on regord.VehicleId equals veh.Id
                                         join smc in con.SmallCustomer on cust.Id equals smc.CustomerId
                                         orderby ord.OrderNumber descending
                                         where ord.Status >= 600  && ord.HasError.GetValueOrDefault(false) != true
                                         select new
                                         {
                                             OrderId = ord.Id,
                                             customerId = cust.Id,
                                             OrderNumber = ord.OrderNumber,
                                             customerID = ord.CustomerId,
                                             CreateDate = ord.CreateDate,
                                             Status = ordst.Name,
                                             CustomerName = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + "  " + cust.SmallCustomer.Person.Name : cust.Name,
                                             CustomerLocation = "",
                                             Kennzeichen = reg.Licencenumber,
                                             VIN = veh.VIN,
                                             TSN = veh.TSN,
                                             HSN = veh.HSN,
                                             OrderTyp = ordtype.Name,
                                             Freitext = ord.FreeText,
                                             Geprueft = ord.Geprueft == null ? "Nein" : "Ja",
                                             Datum = ord.RegistrationOrder.Registration.RegistrationDate
                                         };
                if (CustomerDropDownListOffenNeuzulassung.SelectedValue != string.Empty)
                {
                    var custId = Int32.Parse(CustomerDropDownListOffenNeuzulassung.SelectedValue);
                    smallCustomerQuery = smallCustomerQuery.Where(q => q.customerID == custId);
                }
                e.Result = smallCustomerQuery;
            }
            //select all values for large customers
            else if (RadComboBoxCustomerOffenNeuzulassung.SelectedValue == "2")
            {
                DataClasses1DataContext con = new DataClasses1DataContext();
                var zulassungQuery = from ord in con.Order
                                     join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                     join cust in con.Customer on ord.CustomerId equals cust.Id
                                     join ordtype in con.OrderType on ord.OrderTypeId equals ordtype.Id
                                     join loc in con.Location on ord.LocationId equals loc.Id
                                     join regord in con.RegistrationOrder on ord.OrderNumber equals regord.OrderNumber                          
                                     join veh in con.Vehicle on regord.VehicleId equals veh.Id
                                     join lmc in con.LargeCustomer on cust.Id equals lmc.CustomerId
                                     orderby ord.OrderNumber descending
                                     where ord.Status >= 600 && ord.ReadyToSend.GetValueOrDefault(false) == true && ord.HasError.GetValueOrDefault(false) != true
                                     select new
                                     {
                                         OrderId = ord.Id,
                                         locationId = loc.Id,
                                         customerID = cust.Id,
                                         OrderNumber = ord.OrderNumber,
                                         CreateDate = ord.CreateDate,
                                         Status = ordst.Name,
                                         CustomerName = cust.Name,                                       
                                         VIN = veh.VIN,
                                         TSN = veh.TSN,
                                         HSN = veh.HSN,
                                         CustomerLocation = loc.Name,
                                         OrderTyp = ordtype.Name,
                                         Freitext = ord.FreeText,
                                         Geprueft = ord.Geprueft == null ? "Nein" : "Ja",
                                         Datum = ord.RegistrationOrder.Registration.RegistrationDate
                                     };
                if (CustomerDropDownListOffenNeuzulassung.SelectedValue != string.Empty)
                {
                    var custId = Int32.Parse(CustomerDropDownListOffenNeuzulassung.SelectedValue);
                    zulassungQuery = zulassungQuery.Where(q => q.customerID == custId);
                }
                e.Result = zulassungQuery;
            }
        }   
        protected void RadGridAllOrders_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == Telerik.Web.UI.RadGrid.ExportToExcelCommandName )
            {
                RadGridAllOrders.MasterTableView.AllowPaging = false;
                RadGridAllOrders.PageSize = RadGridAllOrders.Items.Count + 1;
                RadGridAllOrders.ExportSettings.ExportOnlyData = true;
                RadGridAllOrders.ExportSettings.IgnorePaging = true;
                RadGridAllOrders.ExportSettings.OpenInNewWindow = true;               
                RadGridAllOrders.MasterTableView.ExportToExcel();
            }
        }
        protected void RadGridAllOrders_PreRender(object sender, EventArgs e)
        {
            HideExpandColumnRecursive(RadGridAllOrders.MasterTableView);
        }
        public void HideExpandColumnRecursive(GridTableView tableView)
        {
            GridItem[] nestedViewItems = tableView.GetItems(GridItemType.NestedView);
            foreach (GridNestedViewItem nestedViewItem in nestedViewItems)
            {
                foreach (GridTableView nestedView in nestedViewItem.NestedTableViews)
                {
                    nestedView.ParentItem.Expanded = true;
                    HideExpandColumnRecursive(nestedView);
                }
            }
        }      
    }
}