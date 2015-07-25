using KVSCommon.Database;
using KVSCommon.Enums;
using KVSCommon.Managers;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace KVSWebApplication.Auftragseingang
{
    /// <summary>
    ///  Incoming Orders base class
    /// </summary>
    public abstract class IncomingOrdersBase : UserControl
    {
        #region Members

        public IncomingOrdersBase()
        {
            BicManager = (IBicManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IBicManager));
            UserManager = (IUserManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IUserManager));
        }

        protected abstract PermissionTypes PagePermission { get; }

        protected List<Control> controls = new List<Control>();
        protected abstract Panel Panel { get; }
        protected abstract RadTextBox AccountNumberTextBox { get; }
        protected abstract RadTextBox BankCodeTextBox { get; }
        protected abstract RadTextBox BankNameTextBox { get;  }
        protected abstract RadTextBox IBANTextBox { get; }
        protected abstract RadTextBox BICTextBox { get; }
        protected abstract Label CustomerHistoryLabel { get; }
        protected abstract RadComboBox CustomerDropDown { get; }

        public IBicManager BicManager { get; set; }
        public IUserManager UserManager { get; set; }

        #endregion

        #region Methods

        protected void CheckUserPermissions()
        {
            if (UserManager.CheckPermissionsForUser(Session["UserPermissions"], PagePermission))
            {
                Panel.Enabled = true;
            }
        }

        protected void genIban_Click(object sender, EventArgs e)
        {
            if (EmptyStringIfNull.IsNumber(AccountNumberTextBox.Text) &&
                !String.IsNullOrEmpty(BankNameTextBox.Text) &&
                EmptyStringIfNull.IsNumber(BankCodeTextBox.Text))
            {
                IBANTextBox.Text = "DE" + (98 - ((62 * ((1 + long.Parse(BankCodeTextBox.Text) % 97)) +
                    27 * (long.Parse(AccountNumberTextBox.Text) % 97)) % 97)).ToString("D2");
                IBANTextBox.Text += long.Parse(BankCodeTextBox.Text).ToString("00000000").Substring(0, 4);
                IBANTextBox.Text += long.Parse(BankCodeTextBox.Text).ToString("00000000").Substring(4, 4);
                IBANTextBox.Text += long.Parse(AccountNumberTextBox.Text).ToString("0000000000").Substring(0, 4);
                IBANTextBox.Text += long.Parse(AccountNumberTextBox.Text).ToString("0000000000").Substring(4, 4);
                IBANTextBox.Text += long.Parse(AccountNumberTextBox.Text).ToString("0000000000").Substring(8, 2);

                if (BICTextBox != null)
                {
                    var bicNr = BicManager.GetBicByCodeAndName(BankCodeTextBox.Text, BankNameTextBox.Text);
                    if (bicNr != null && !String.IsNullOrEmpty(bicNr.BIC))
                    {
                        BICTextBox.Text = bicNr.BIC.ToString();
                    }
                }
            }
        }

        protected void CheckUmsatzForSmallCustomer()
        {
            CustomerHistoryLabel.Visible = true;
            KVSEntities con = new KVSEntities();
            var newQuery = from ord in con.Order
                           let registration = ord.DeregistrationOrder != null ? ord.DeregistrationOrder.Registration : ord.DeregistrationOrder.Registration
                           where ord.Status == (int)OrderStatusTypes.Payed
                           select new
                           {
                               CustomerId = ord.CustomerId,
                               OrderNumber = ord.OrderNumber,
                           };
            if (!String.IsNullOrEmpty(CustomerDropDown.SelectedValue))
            {
                newQuery = newQuery.Where(q => q.CustomerId == Int32.Parse(CustomerDropDown.SelectedValue));
            }

            // Allgemein
            string countAuftrag = newQuery.Count().ToString();
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
            CustomerHistoryLabel.Text = "Historie: <br/> Gesamt: " + countAuftrag + " <br/> Umsatz: " + umsatz.ToString("C2") + "<br/> Gebühren: " + gebuehren.ToString("C2");
        }

        #endregion
    }
}