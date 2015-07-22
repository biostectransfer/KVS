using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using KVSCommon.Enums;
namespace KVSWebApplication.Statistic
{
    /// <summary>
    /// Codebehind fuer die Statistik Maske
    /// </summary>
    public partial class statistic : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                MakeChartAnzahl();
                MakeChartUmsatz();   
            }
            MakeChartProKunde();
        }
        protected void RadGridStatistic_NeedDataSource_Linq(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetSourceForRadGrid();
        }
        protected object GetSourceForRadGrid()
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var newQuery = from ord in con.Order
                           let registration = ord.RegistrationOrder != null ? ord.RegistrationOrder.Registration : ord.DeregistrationOrder.Registration
                           where ord.Status == (int)OrderStatusTypes.Payed
                           select new
                           {
                               OrderNumber = ord.OrderNumber,
                               CustomerId = ord.CustomerId,
                               CreateDate = ord.CreateDate,
                               Status = ord.OrderStatus.Name,
                               CustomerName = ord.Customer.Name,
                               Kennzeichen = registration.Licencenumber,
                               VIN = registration.Vehicle.VIN,
                               CustomerLocation = ord.Location.Name,
                               OrderTyp = ord.OrderType.Name,
                               Haltername = registration.CarOwner.Name,
                               reg = registration
                           };
            if (!String.IsNullOrEmpty(CustomerNameBox.SelectedValue))
            {
                try
                {
                    newQuery = newQuery.Where(q => q.CustomerId == Int32.Parse(CustomerNameBox.SelectedValue));
                }

                catch
                {
                }
            }
            if (!String.IsNullOrEmpty(AuftragstypBox.SelectedValue))
            {
                try
                {
                    newQuery = newQuery.Where(q => q.OrderTyp == AuftragstypBox.Text);
                }

                catch
                {
                }
            }
            if (BisPicker.SelectedDate != null)
            {
                try
                {
                    newQuery = newQuery.Where(q => q.CreateDate < BisPicker.SelectedDate);
                }

                catch
                {
                }
            }
            if (VonPicker.SelectedDate != null)
            {
                try
                {
                    newQuery = newQuery.Where(q => q.CreateDate > VonPicker.SelectedDate);
                }

                catch
                {
                }
            }
            // Allgemein
            AuftragsCounterLabel.Text = newQuery.Count().ToString();            
            decimal gebuehren = 0;
            decimal umsatz = 0;
            //Amtliche Gebühr
            foreach (var newOrder in newQuery)
            {
                var order = con.Order.SingleOrDefault(q => q.OrderNumber == newOrder.OrderNumber);
                if (order != null)
                { 
                    foreach (OrderItem orderItem in order.OrderItem)
                    {
                        if (orderItem.IsAuthorativeCharge && orderItem.Status == (int)OrderItemStatusTypes.Payed)
                            gebuehren = gebuehren + orderItem.Amount;
                        else if (!orderItem.IsAuthorativeCharge && orderItem.Status == (int)OrderItemStatusTypes.Payed)
                            umsatz = umsatz + orderItem.Amount;
                    }
                }
            }
            SummeAmtGebuhrLabel.Text = gebuehren.ToString("C2");
            UmsatzLabel.Text = umsatz.ToString("C2");
            return newQuery;
        }
        protected void MakeChartProKunde()
        {
            int anzAll = 0, anzAbm = 0, anzZul = 0;
            if (!String.IsNullOrEmpty(CustomerNameBox.SelectedValue))
            {
                //making pie
                DataClasses1DataContext con = new DataClasses1DataContext();
                var customerQuery = con.Customer.SingleOrDefault(q => q.Id == Int32.Parse(CustomerNameBox.SelectedValue));
                if (customerQuery != null)
                {
                    List<Order> orderList = con.Order.Where(q => q.CustomerId == customerQuery.Id && q.Status == (int)OrderStatusTypes.Payed).ToList();
                    foreach (Order order in orderList)
                    {
                        if (order.OrderType.Id == (int)OrderTypes.Cancellation)
                        {
                            anzAbm++;
                        }
                        if (order.OrderType.Id == (int)OrderTypes.Admission)
                        {
                            anzZul++;
                        }
                    }
                    PieChart1.PlotArea.Series[0].Items[0].YValue = Convert.ToDecimal(anzZul);
                    PieChart1.PlotArea.Series[0].Items[1].YValue = Convert.ToDecimal(anzAbm);
                }
            }
            else
            {
                DataClasses1DataContext con = new DataClasses1DataContext();
                List<Order> orderList = con.Order.Where(q => q.Status == (int)OrderStatusTypes.Payed).ToList();
                foreach (Order order in orderList)
                {
                    if (order.OrderType.Id == (int)OrderTypes.Cancellation)
                    {
                        anzAbm++;
                    }

                    if (order.OrderType.Id == (int)OrderTypes.Admission)
                    {
                        anzZul++;
                    }
                }
                PieChart1.PlotArea.Series[0].Items[0].YValue = Convert.ToDecimal(anzZul);
                PieChart1.PlotArea.Series[0].Items[1].YValue = Convert.ToDecimal(anzAbm);
            }
        }
        protected void MakeChartAnzahl()
        {
            //making pie
            DataClasses1DataContext con = new DataClasses1DataContext();
            var Customer1 = from cust in con.Customer
                            select cust.Id; 
            foreach (var custId in Customer1)
            {
                var order = con.Order.Count(q => q.CustomerId == custId && q.Status == (int)OrderStatusTypes.Payed);
                if (order != 0)
                {
                    var customerName = con.Customer.SingleOrDefault(q => q.Id == custId).Name;
                    Telerik.Charting.ChartSeries chart = new Telerik.Charting.ChartSeries(customerName);
                    chart.AddItem(order);
                    anzahlChart.Series.Add(chart);
                }            
            }
        }
        protected void MakeChartUmsatz()
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var Customer1 = from cust in con.Customer
                            select cust.Id;
            foreach (var custId in Customer1)
            {
                var orders = (from ord in con.Order
                              where ord.CustomerId == custId && ord.Status == (int)OrderStatusTypes.Payed
                             select ord).ToList();
                var customerName = con.Customer.SingleOrDefault(q => q.Id == custId).Name;
                double sum = 0;
                if (orders.Count > 0)
                {
                    Telerik.Charting.ChartSeries chartUmsatz = new Telerik.Charting.ChartSeries(customerName);
                    foreach (Order order in orders)
                    {
                        foreach (OrderItem item in order.OrderItem)
                        {
                            if (!item.IsAuthorativeCharge && item.Status == (int)OrderItemStatusTypes.Payed)
                            {
                                sum = sum + Convert.ToDouble(item.Amount);                               
                            }
                        }
                    }
                    chartUmsatz.AddItem(sum);
                    umsatzProKundeChart.Series.Add(chartUmsatz);
                }
            }
        }
        protected void CustomerName_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var CustomerName = from cust in con.Customer
                               select new
                               {
                                   Name = cust.SmallCustomer != null && cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name,  
                                       Value = cust.Id, Matchcode = cust.MatchCode, Kundennummer = cust.CustomerNumber };                             
            e.Result = CustomerName;
        }
        protected void searchButton_Clicked(object sender, EventArgs e)
        {
            RadGridStatistic.Rebind();           
        }
        protected void LinqDataSourceAuftragstyp_Linq(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var auftragsTyp = from typ in con.OrderType
                               select new
                               {
                                   Name = typ.Name,
                                   Value = typ.Id
                               };
            e.Result = auftragsTyp;
        }
        protected void AllgemeinButton_Clicked(object sender, EventArgs e)
        {
            DateTime? DateNull = null;
            CustomerNameBox.SelectedIndex = -1;
            CustomerNameBox.SelectedValue = string.Empty;
            CustomerNameBox.ClearCheckedItems();
            CustomerNameBox.ClearSelection();
            CustomerNameBox.Text = string.Empty;
            AuftragstypBox.SelectedIndex = -1;
            AuftragstypBox.SelectedValue = string.Empty;
            AuftragstypBox.ClearCheckedItems();
            AuftragstypBox.ClearSelection();
            AuftragstypBox.Text = string.Empty;
            VonPicker.SelectedDate = DateNull;
            BisPicker.SelectedDate = DateNull;
            RadGridStatistic.Rebind();
            MakeChartProKunde();
        }
    }
}