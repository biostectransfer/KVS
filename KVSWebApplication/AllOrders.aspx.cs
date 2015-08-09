using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using KVSWebApplication.BasePages;
using KVSCommon.Enums;

namespace KVSWebApplication
{
    /// <summary>
    /// Codebehind fuer alle Auftraege
    /// </summary>
    public partial class AllOrders : BasePage
    {        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GridSortExpression sortExpr = new GridSortExpression();
                sortExpr.FieldName = "CreateDate";
                sortExpr.SortOrder = GridSortOrder.Descending;
                RadGridAllOrders.MasterTableView.SortExpressions.AddSortExpression(sortExpr);
            }
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
            e.Result = ProductManager.GetEntities().
                               Select(prod => new
                               {
                                   ItemNumber = prod.ItemNumber,
                                   Name = prod.Name,
                                   Value = prod.Id,
                                   Category = prod.ProductCategory.Name
                               }).ToList();
        }

        protected void CostCenterDataSourceLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = CostCenterManager.GetEntities().OrderBy(o => o.Name).Select(cost => new
            {
                Name = cost.Name,
                Value = cost.Id
            }).ToList();
        }

        protected void CustomerLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities con = new KVSEntities();
            if (RadComboBoxCustomerOffenNeuzulassung.SelectedValue == "1") //Small Customers
            {
                var customerQuery = CustomerManager.GetEntities(o => o.SmallCustomer != null).
                    Select(cust => new
                    {
                        Name = cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name,
                        Value = cust.Id,
                        Matchcode = cust.MatchCode,
                        Kundennummer = cust.CustomerNumber
                    });

                e.Result = customerQuery.ToList();
            }
            else if (RadComboBoxCustomerOffenNeuzulassung.SelectedValue == "2") //Large Customers
            {
                var customerQuery = CustomerManager.GetEntities(o => o.LargeCustomer != null).
                    Select(cust => new
                    {
                        Name = cust.Name,
                        Value = cust.Id,
                        Matchcode = cust.MatchCode,
                        Kundennummer = cust.CustomerNumber
                    });

                e.Result = customerQuery.ToList();
            }
        }

        protected void RadGridAllOrders_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            var item = (GridDataItem)e.DetailTableView.ParentItem;
            e.DetailTableView.DataSource = GetOrderPositions(item["OrderNumber"].Text);            
        }

        protected void AllOrdersLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {       
            //select all values for small customers
            if (RadComboBoxCustomerOffenNeuzulassung.SelectedValue == "1")
            {
                var smallCustomerOrders = GetSmallCustomerOrders().Where(o => o.OrderStatusId >= (int)OrderStatusTypes.Closed &&
                    o.HasError.GetValueOrDefault(false) != true);

                if (CustomerDropDownListOffenNeuzulassung.SelectedValue != string.Empty)
                {
                    var custId = Int32.Parse(CustomerDropDownListOffenNeuzulassung.SelectedValue);
                    smallCustomerOrders = smallCustomerOrders.Where(q => q.customerId == custId);
                }

                e.Result = smallCustomerOrders.OrderByDescending(o => o.OrderNumber).ToList();
            }
            //select all values for large customers
            else if (RadComboBoxCustomerOffenNeuzulassung.SelectedValue == "2")
            {
                var largeCustomerOrders = GetLargeCustomerOrders().Where(o => o.OrderStatusId >= (int)OrderStatusTypes.Closed &&
                    o.HasError.GetValueOrDefault(false) != true &&
                    o.ReadyToSend.GetValueOrDefault(false) == true);

                if (CustomerDropDownListOffenNeuzulassung.SelectedValue != string.Empty)
                {
                    var custId = Int32.Parse(CustomerDropDownListOffenNeuzulassung.SelectedValue);
                    largeCustomerOrders = largeCustomerOrders.Where(q => q.customerId == custId);
                }
                e.Result = largeCustomerOrders.OrderByDescending(o => o.OrderNumber).ToList();
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