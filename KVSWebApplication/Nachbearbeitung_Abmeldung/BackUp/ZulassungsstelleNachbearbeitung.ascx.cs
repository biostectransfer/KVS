using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;

namespace KVSWebApplication.Auftragsbearbeitung_Neuzulassung
{
    public partial class ZulassungsstelleNachbearbeitung : System.Web.UI.UserControl
    {
        //private string customer = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!String.IsNullOrEmpty((this.Page as AuftragsbearbeitungNeuzulassung).largeOrSmallCust) && (!String.IsNullOrEmpty((this.Page as AuftragsbearbeitungNeuzulassung).customerId)))
            //{
            //    //Values: 1-small, 2-large
            //    //RadComboCustomer.SelectedItem.Value = (this.Page as AuftragsbearbeitungNeuzulassung).largeOrSmallCust;
            //    //CustomerDropDownList.SelectedValue = (this.Page as AuftragsbearbeitungNeuzulassung).customerId;
            //    //RadGridAbmeldung.DataBind();
            //}
        }

        protected void AbmeldungenLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            //select all values for small customers
            if (CustomerZulDropDownList.SelectedValue != null && RadComboCustomer.SelectedValue == "1" && CustomerZulDropDownList.SelectedValue != "") // Small
            {
                Guid guid = new Guid(CustomerZulDropDownList.SelectedValue);
                DataClasses1DataContext con = new DataClasses1DataContext();
                RadGridAbmeldung.Columns.FindByUniqueName("Location").Visible = false;

                var smallCustomerQuery = from cust in con.Customer
                                         join ord in con.Order on cust.Id equals ord.CustomerId
                                         join smcust in con.SmallCustomer on cust.Id equals smcust.CustomerId
                                         join loc in con.Location on cust.Id equals loc.CustomerId
                                         join derOrd in con.DeregistrationOrder on ord.Id equals derOrd.OrderId
                                         join reg in con.Registration on derOrd.OrderId equals reg.Id
                                         join veh in con.Vehicle on derOrd.VehicleId equals veh.Id
                                         join mod in con.Model on veh.ModelId equals mod.Id
                                         join make in con.Make on veh.MakeId equals make.Id
                                         join pr in con.Price on loc.Id equals pr.LocationId
                                         join prod in con.Product on pr.ProductId equals prod.Id
                                         join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                         where cust.Id == guid && ord.Status != 600 && pr.LocationId == null
                                         select new
                                         {
                                             OrderId = ord.Id,
                                             CustomerName = cust.Name,
                                             OrderNumber = ord.Ordernumber,
                                             Status = ordst.Name,
                                             CreateDate = ord.CreateDate,
                                             //CustomerLocation = loc.Name,
                                             Kennzeichen = reg.Licencenumber,
                                             VIN = veh.VIN,
                                             Model = mod.Name,
                                             Make = make.Name,
                                             Amount = pr.Amount,
                                             Product = prod.Name
                                         };
                e.Result = smallCustomerQuery;
            }

            //select all values for large customers
            else if (CustomerZulDropDownList.SelectedValue != null && CustomerZulDropDownList.SelectedValue != "" && RadComboCustomer.SelectedValue == "2") // Large
            {
                DataClasses1DataContext con = new DataClasses1DataContext();
                Guid guid = new Guid(CustomerZulDropDownList.SelectedValue);
                RadGridAbmeldung.Columns.FindByUniqueName("Location").Visible = true;

                var smallCustomerQuery = from cust in con.Customer
                                         join ord in con.Order on cust.Id equals ord.CustomerId
                                         join lrcust in con.LargeCustomer on cust.Id equals lrcust.CustomerId
                                         join loc in con.Location on cust.Id equals loc.CustomerId
                                         join derOrd in con.DeregistrationOrder on ord.Id equals derOrd.OrderId
                                         join reg in con.Registration on derOrd.OrderId equals reg.Id
                                         join veh in con.Vehicle on derOrd.VehicleId equals veh.Id
                                         join mod in con.Model on veh.ModelId equals mod.Id
                                         join make in con.Make on veh.MakeId equals make.Id
                                         join pr in con.Price on loc.Id equals pr.LocationId
                                         join prod in con.Product on pr.ProductId equals prod.Id
                                         join ordst in con.OrderStatus on ord.Status equals ordst.Id
                                         where cust.Id == guid && ord.Status != 600
                                         select new
                                         {
                                             OrderId = ord.Id,
                                             CustomerName = cust.Name,
                                             OrderNumber = ord.Ordernumber,
                                             Status = ordst.Name,
                                             CreateDate = ord.CreateDate,
                                             CustomerLocation = loc.Name,
                                             Kennzeichen = reg.Licencenumber,
                                             VIN = veh.VIN,
                                             Model = mod.Name,
                                             Make = make.Name,
                                             Amount = pr.Amount,
                                             Product = prod.Name
                                         };
                e.Result = smallCustomerQuery;
            }
        }

        // Small oder Large -> Auswahl der KundenName
        protected void CustomerZulLinq_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();

            if (RadComboBoxCustomer.SelectedValue == "1") //Small Customers
            {
                var customerQuery = from cust in con.Customer
                                    where cust.Id == cust.SmallCustomer.CustomerId
                                    select new { Name = cust.Name, Value = cust.Id };

                e.Result = customerQuery;
            }

            else if (RadComboBoxCustomer.SelectedValue == "2") //Large Customers
            {
                var customerQuery = from cust in con.Customer
                                    where cust.Id == cust.LargeCustomer.CustomerId
                                    select new { Name = cust.Name, Value = cust.Id };

                e.Result = customerQuery;
            }
        }

        // Large oder small Customer
        protected void SmLrCustomerIndex_Changed(object sender, EventArgs e)
        {
            CustomerZulDropDownList.Enabled = true;
            this.CustomerZulDropDownList.DataBind();
            this.RadGridAbmeldung.DataBind();
        }

        // Auswahl von Kunde
        protected void CustomerZulassungIndex_Changed(object sender, EventArgs e)
        {
            RadGridAbmeldung.Enabled = true;
            this.RadGridAbmeldung.DataBind();

        }

        protected void AuftragFertig_Command(object sender, EventArgs e)
        {
            string CustomerName = string.Empty,
                OrderNumber = string.Empty,
                Status = string.Empty,
                CustomerLocation = string.Empty,
                Kennzeichen = string.Empty,
                VIN = string.Empty,
                Make = string.Empty,
                Model = string.Empty,
                Amount = string.Empty,
                Product = string.Empty;

            DateTime createDate;
            Guid orderId;

            Button editButton = sender as Button;
            GridEditFormItem item = editButton.NamingContainer as GridEditFormItem;

            TextBox customerName = item.FindControl("CustomerNameBox") as TextBox;
            TextBox orderNumber = item.FindControl("OrderNumberBox") as TextBox;
            TextBox status = item.FindControl("StatusBox") as TextBox;
            RadDateTimePicker picker = item.FindControl("CreateDateBox") as RadDateTimePicker;
            TextBox location = item.FindControl("LocationBox") as TextBox;
            TextBox kennzeichen = item.FindControl("KennzeichenBox") as TextBox;
            TextBox vin = item.FindControl("VINBox") as TextBox;
            TextBox make = item.FindControl("MakeBox") as TextBox;
            TextBox model = item.FindControl("ModelBox") as TextBox;
            TextBox amount = item.FindControl("AmountBox") as TextBox;
            TextBox product = item.FindControl("ProductBox") as TextBox;
            TextBox orderIdBox = item.FindControl("orderIdBox") as TextBox;

            // if small customer - hide location field
            if (RadComboBoxCustomer.SelectedValue == "1")
            {
                location.Visible = false;
            }

            else
            {
                CustomerLocation = location.Text;
            }



            CustomerName = customerName.Text;
            OrderNumber = orderNumber.Text;
            Status = status.Text;
            createDate = picker.SelectedDate.Value;
            Kennzeichen = kennzeichen.Text;
            VIN = vin.Text;
            Make = make.Text;
            Model = model.Text;
            Amount = amount.Text;
            Product = product.Text;
            orderId = new Guid(orderIdBox.Text);

            // should be one more time checkt if small or large
            if (RadComboBoxCustomer.SelectedValue == "1") //if small
            {
                //save to archive and show PDF
                Response.Write("Das ist Rechnung für SofortKunde");

            }

            else // if large customer
            {
                //update Status mit "Fertig"
                using (DataClasses1DataContext DbContext = new DataClasses1DataContext(new Guid("7732DF6B-C1EF-4CA5-BCBE-A21D5828A53F")))
                {
                    var Order1 = DbContext.Order.Where(q => q.Id == orderId);

                    var order = from ord in DbContext.Order
                                where ord.Id == orderId
                                select ord;

                }

            }



            //DataClasses1DataContext con = new DataClasses1DataContext(" USER ID");


            //if ( CustomerZulDropDownList.SelectedValue != null)
            //{
            //    try
            //    {
            //      Guid guid = new Guid(CustomerZulDropDownList.SelectedValue);

            //      var OrderUpd = con.Order.Single(c => c.CustomerId == guid);
            //      OrderUpd.LOGDataContext = con;
            //      //OrderUpd.Status = 300;
            //      //con.SubmitChanges();
            //    }

            //    catch
            //    {}
            //}



        }



        protected void Edit_Command(object sender, GridCommandEventArgs e)
        {

            if (e.CommandName == RadGrid.EditCommandName)
            {
                //RadTextBox radText = new RadTextBox();
                //radText.Text = "Hallo";

            }

        }


    }
    
}