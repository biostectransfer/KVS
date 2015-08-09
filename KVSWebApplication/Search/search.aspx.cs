using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using KVSCommon.Database;
using KVSWebApplication.BasePages;

namespace KVSWebApplication.Search
{
    /// <summary>
    ///Suchmaske fuer die einzelnen Auftraege
    /// </summary>
    public partial class search : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var sortExpr = new GridSortExpression();
                sortExpr.FieldName = "CreateDate";
                sortExpr.SortOrder = GridSortOrder.Descending;
                RadGridSearch.MasterTableView.SortExpressions.AddSortExpression(sortExpr);
            }
        }

        protected void RadGridSearch_NeedDataSource_Linq(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetSourceForRadGrid();
        }

        protected object GetSourceForRadGrid()
        {
            SearchErrorLabel.Visible = false;

            var smallCustomerOrders = GetSmallCustomerOrders();
            var largeCustomerOrders = GetLargeCustomerOrders();


            if (!String.IsNullOrEmpty(CustomerNameBox.SelectedValue))
            {
                try
                {
                    smallCustomerOrders = smallCustomerOrders.Where(q => q.customerId == Int32.Parse(CustomerNameBox.SelectedValue));
                    largeCustomerOrders = largeCustomerOrders.Where(q => q.customerId == Int32.Parse(CustomerNameBox.SelectedValue));
                }

                catch
                {
                    SearchErrorLabel.Visible = true;
                    SearchErrorLabel.Text = "Für " + CustomerNameBox.Text + " wurde keine Aufträge gefunden";
                }
            }

            if (!String.IsNullOrEmpty(KennzeichenSearchBox.Text))
            {
                try
                {
                    smallCustomerOrders = smallCustomerOrders.Where(q => q.Kennzeichen.Contains(KennzeichenSearchBox.Text));
                    largeCustomerOrders = largeCustomerOrders.Where(q => q.Kennzeichen.Contains(KennzeichenSearchBox.Text));
                }

                catch
                {
                    SearchErrorLabel.Visible = true;
                    SearchErrorLabel.Text = "Für " + KennzeichenSearchBox.Text + " wurde keine Aufträge gefunden";
                }
            }

            if (!String.IsNullOrEmpty(HalterNameBox.Text))
            {
                try
                {
                    smallCustomerOrders = smallCustomerOrders.Where(q => q.Haltername.Contains(HalterNameBox.Text));
                    largeCustomerOrders = largeCustomerOrders.Where(q => q.Haltername.Contains(HalterNameBox.Text));
                }

                catch
                {
                    SearchErrorLabel.Visible = true;
                    SearchErrorLabel.Text = "Für " + HalterNameBox.Text + " wurde keine Aufträge gefunden";
                }
            }

            if (!String.IsNullOrEmpty(FINBox.Text))
            {
                try
                {
                    smallCustomerOrders = smallCustomerOrders.Where(q => q.VIN.Contains(FINBox.Text));
                    largeCustomerOrders = largeCustomerOrders.Where(q => q.VIN.Contains(FINBox.Text));
                }

                catch
                {
                    SearchErrorLabel.Visible = true;
                    SearchErrorLabel.Text = "Für " + FINBox.Text + " wurde keine Aufträge gefunden";
                }
            }

            var result = new List<OrderViewModel>(smallCustomerOrders.ToList());
            result.AddRange(largeCustomerOrders.ToList());

            return result;
        }

        protected void searchButton_Clicked(object sender, EventArgs e)
        {
            RadGridSearch.Rebind();
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

        protected void NeueSucheButton_Clicked(object sender, EventArgs e)
        {
            CustomerNameBox.SelectedIndex = -1;
            CustomerNameBox.SelectedValue = string.Empty;
            CustomerNameBox.Text = string.Empty;

            CustomerNameBox.ClearCheckedItems();
            CustomerNameBox.ClearSelection();

            KennzeichenSearchBox.Text = string.Empty;
            HalterNameBox.Text = string.Empty;
            FINBox.Text = string.Empty;

            RadGridSearch.Rebind();
        }

        protected void RadGridSearch_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e is Telerik.Web.UI.GridPageChangedEventArgs || e is Telerik.Web.UI.GridFilterCommandEventArgs || e is GridPageSizeChangedEventArgs || e.CommandName == "Cancel" || e.CommandName == "Edit")
            {
                //muss nicht bearbeitet werden, da diese Events aus dem RadGrid kommen und mit der Suchfunktion nicht zu tun hat
            }
            else if (e.Item is GridDataItem)
            {
                SearchErrorLabel.Visible = false;
                bool showErrorLabel = false;
                GridDataItem item = (GridDataItem)e.Item;

                var customerId = Int32.Parse(item["CustomerId"].Text);
                string status = item["Status"].Text;


                if (!String.IsNullOrEmpty(item["OrderTyp"].Text))
                {
                    if (item["OrderTyp"].Text.Contains("Zulassung")) // *** ZULASSUNG ***
                    {
                        if (item["HasErrorAsString"].Text.Contains("Nein")) // falls kein Fehler
                        {
                            if (status.Contains("Offen"))// || status.Contains("Zulassungsstelle"))
                            {
                                Session["customerIndexSearch"] = String.IsNullOrEmpty(item["CustomerLocation"].Text) ? 1 : 2;
                                Session["orderStatusSearch"] = status;
                                Session["customerIdSearch"] = customerId;
                                Session["orderNumberSearch"] = item["OrderNumber"].Text;
                                RadAjaxPanel1.Redirect("../Auftragsbearbeitung_Neuzulassung/AuftragsbearbeitungNeuzulassung.aspx");
                            }

                            else if (status.Contains("Abgeschlossen"))
                                RadAjaxPanel1.Redirect("../Abrechnung/Abrechnung.aspx");
                            else
                                showErrorLabel = true;
                        }

                        else if (item["HasErrorAsString"].Text.Contains("Ja")) // soll zu Fehlerhaft redirect werden
                        {
                            Session["customerIndexSearch"] = String.IsNullOrEmpty(item["CustomerLocation"].Text) ? 1 : 2;
                            Session["orderStatusSearch"] = "Error";
                            Session["customerIdSearch"] = customerId;
                            Session["orderNumberSearch"] = item["OrderNumber"].Text;
                            RadAjaxPanel1.Redirect("../Auftragsbearbeitung_Neuzulassung/AuftragsbearbeitungNeuzulassung.aspx");
                        }
                    }

                    else if (item["OrderTyp"].Text.Contains("Abmeldung")) // *** ABMELDUNG ***
                    {
                        if (item["HasErrorAsString"].Text.Contains("Nein")) // falls kein Fehler
                        {
                            if (status.Contains("Offen"))//|| status.Contains("Zulassungsstelle"))
                            {
                                Session["customerIndexSearch"] = String.IsNullOrEmpty(item["CustomerLocation"].Text) ? 1 : 2;
                                Session["orderStatusSearch"] = status;
                                Session["customerIdSearch"] = customerId;
                                Session["orderNumberSearch"] = item["OrderNumber"].Text;
                                RadAjaxPanel1.Redirect("../Nachbearbeitung_Abmeldung/NachbearbeitungAbmeldung.aspx");
                            }

                            else if (status.Contains("Abgeschlossen"))
                                RadAjaxPanel1.Redirect("../Abrechnung/Abrechnung.aspx");

                            else
                                showErrorLabel = true;
                        }

                        else if (item["HasErrorAsString"].Text.Contains("Ja")) // soll zu Fehlerhaft redirect werden
                        {
                            Session["customerIndexSearch"] = String.IsNullOrEmpty(item["CustomerLocation"].Text) ? 1 : 2;
                            Session["orderStatusSearch"] = "Error";
                            Session["customerIdSearch"] = customerId;
                            Session["orderNumberSearch"] = item["OrderNumber"].Text;
                            RadAjaxPanel1.Redirect("../Nachbearbeitung_Abmeldung/NachbearbeitungAbmeldung.aspx");
                        }
                    }

                    if (showErrorLabel == true)
                    {
                        SearchErrorLabel.Visible = true;
                        SearchErrorLabel.Text = "Auftrag mit dem Status " + item["Status"].Text + " kann nicht angezeigt werden";
                    }
                }
            }
        }
    }
}