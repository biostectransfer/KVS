using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.Transactions;
using KVSWebApplication.BasePages;
using KVSCommon.Enums;

namespace KVSWebApplication.Product
{
    /// <summary>
    /// Codebehind fuer die Kundenprodukte
    /// </summary>
    public partial class CustomerProducts : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserManager.CheckPermissionsForUser(Session["UserPermissions"], PermissionTypes.PRODUKTE_BEARBEITEN))
            {
                getCustomerPrice.Columns[0].Visible = false;
            }

            if (!UserManager.CheckPermissionsForUser(Session["UserPermissions"], PermissionTypes.PRODUKTE_ANLEGEN))
            {
                foreach (GridItem item in getCustomerPrice.MasterTableView.GetItems(GridItemType.CommandItem))
                {
                    ImageButton CustomerProductsAddButton = item.FindControl("add") as ImageButton;
                    CustomerProductsAddButton.Visible = false;
                    CustomerProductsAddButton.Enabled = false;
                }
            }

            if (!UserManager.CheckPermissionsForUser(Session["UserPermissions"], PermissionTypes.PRODUKTE_SPERREN))
            {
                //Implementation
            }

            if (!UserManager.CheckPermissionsForUser(Session["UserPermissions"], PermissionTypes.LOESCHEN))
            {
                getCustomerPrice.MasterTableView.Columns[getCustomerPrice.MasterTableView.Columns.Count - 1].Visible = false;
            }

            if (!Page.IsPostBack)
            {
                Session["selectedProductId"] = null;
            }
        }

        protected void GetCustomerProductsDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            int? customerId = null;
            if (!String.IsNullOrEmpty(AllCustomer.SelectedValue))
            {
                customerId = Int32.Parse(AllCustomer.SelectedValue);
            }

            int? locationId = null;
            if (!String.IsNullOrEmpty(cmbLocations.SelectedValue))
            {
                locationId = Int32.Parse(cmbLocations.SelectedValue);
            }


            var products = ProductManager.GetEntities(o => o.Price.Any(p =>
                    (locationId.HasValue && p.LocationId == locationId && p.Location.CustomerId == customerId) ||
                    (!p.LocationId.HasValue && !locationId.HasValue))).
                Select(prod =>
            {
                var price = prod.Price.FirstOrDefault(p => (locationId.HasValue && p.LocationId == locationId && p.Location.CustomerId == customerId) ||
                    (!p.LocationId.HasValue && !locationId.HasValue));

                string accountNumber = String.Empty;
                var account = price.PriceAccount.FirstOrDefault();
                if (account != null)
                {
                    accountNumber = account.Accounts.AccountNumber;
                }

                return new
                {
                    Id = prod.Id,
                    CustomerId = price.Location.CustomerId,
                    LocationIdCustomer = price.LocationId,
                    PriceId = price.Id,
                    OrderTypeId = prod.OrderTypeId,
                    ProductCategoryId = prod.ProductCategoryId,
                    RegistrationOrderTypeId = prod.RegistrationOrderTypeId,
                    ProductName = prod.Name,
                    ItemNumber = prod.ItemNumber,
                    OrderTypeName = OrderTypesCollection.FirstOrDefault(o => o.Id == prod.OrderTypeId).Name,
                    ProductCategorieName = prod.ProductCategoryId.HasValue ?
                            ProductCategoryCollection.FirstOrDefault(o => o.Id == prod.ProductCategoryId.Value).Name : String.Empty,
                    RegistrationOrderTypeName = prod.RegistrationOrderTypeId.HasValue ?
                            RegistrationOrderTypeCollection.FirstOrDefault(o => o.Id == prod.RegistrationOrderTypeId.Value).Name : String.Empty,
                    AccountNumber = accountNumber,
                    Amount = Math.Round(price.Amount, 2, MidpointRounding.AwayFromZero).ToString(),
                    AutoCharge = price.AuthorativeCharge != null ? Math.Round(price.AuthorativeCharge.Value, 2, MidpointRounding.AwayFromZero).ToString() : ""
                };
            });


            e.Result = products.OrderBy(o => o.ItemNumber).ToList();


            if (String.IsNullOrEmpty(cmbLocations.SelectedValue) || String.IsNullOrEmpty(AllCustomer.SelectedValue))
            {
                getCustomerPrice.Enabled = false;
                getCustomerPrice.Visible = false;
            }
            else
            {
                getCustomerPrice.Enabled = true;
                getCustomerPrice.Visible = true;
            }
        }

        protected void bSchow_Click(object sender, EventArgs e)
        {
            getCustomerPrice.Enabled = true;
            getCustomerPrice.Visible = true;
            if (cmbLocations.SelectedValue == string.Empty)
            {
                getCustomerPrice.Enabled = false;
                getCustomerPrice.Visible = false;
                return;
            }
            if (String.IsNullOrEmpty(cmbLocations.SelectedValue))
            {
                getCustomerPrice.Enabled = false;
                getCustomerPrice.Visible = false;
                return;
            }
            if (AllCustomer.SelectedValue == string.Empty)
            {
                getCustomerPrice.Enabled = false;
                getCustomerPrice.Visible = false;
                return;
            }
            if (String.IsNullOrEmpty(AllCustomer.SelectedValue))
            {
                getCustomerPrice.Enabled = false;
                getCustomerPrice.Visible = false;
                return;
            }
            else
            {
                getCustomerPrice.Enabled = true;
                getCustomerPrice.Visible = true;
            }

            getCustomerPrice.EditIndexes.Clear();
            getCustomerPrice.MasterTableView.IsItemInserted = false;
            getCustomerPrice.MasterTableView.Rebind();

            if (!UserManager.CheckPermissionsForUser(Session["UserPermissions"], PermissionTypes.PRODUKTE_ANLEGEN))
            {
                foreach (GridItem item in getCustomerPrice.MasterTableView.GetItems(GridItemType.CommandItem))
                {
                    ImageButton CustomerProductsAddButton = item.FindControl("add") as ImageButton;
                    CustomerProductsAddButton.Enabled = false;
                    break;
                }
            }
        }

        protected void CustomerCombobox_Init(object sender, EventArgs e)
        {
            var customerQuery = CustomerManager.GetEntities().
                    Select(cust => new
                    {
                        Name = cust.SmallCustomer != null && cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name,
                        Value = cust.Id,
                        Matchcode = cust.MatchCode,
                        Kundennummer = cust.CustomerNumber
                    });

            AllCustomer.Items.Clear();
            AllCustomer.DataSource = customerQuery.ToList();
            AllCustomer.DataBind();
        }

        protected void cmdProductNames_OnInit(object sender, EventArgs e)
        {
            var locationId = Int32.Parse(cmbLocations.SelectedValue.ToString());

            var products = ProductManager.GetEntities(o => !o.Price.Any(p => p.LocationId == locationId)).
                Select(prod => new
                {
                    ItemNumber = prod.ItemNumber,
                    Name = prod.Name,
                    Value = prod.Id,
                    Category = prod.ProductCategoryId.HasValue ?
                                ProductCategoryCollection.FirstOrDefault(o => o.Id == prod.ProductCategoryId.Value).Name : String.Empty
                });


            var myCustomerProducts = ((RadComboBox)sender);
            myCustomerProducts.Items.Clear();
            myCustomerProducts.DataSource = products.ToList();
            myCustomerProducts.DataBind();
        }

        protected void CustomerCombobox_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (AllCustomer.SelectedValue != string.Empty)
            {
                var locations = LocationManager.GetEntities(loc => loc.CustomerId != Int32.Parse(e.Value)).Select(loc => new
                {
                    Value = loc.Id,
                    Name = loc.Name
                }).ToList();

                cmbLocations.Items.Clear();

                foreach (var location in locations)
                {
                    cmbLocations.Items.Add(new RadComboBoxItem(location.Name, location.Value.ToString()));
                }

                if (cmbLocations.Items.Count == 0)
                {
                    getCustomerPrice.Enabled = false;
                    getCustomerPrice.Visible = false;
                }
            }
        }

        protected void getAllCustomerProducts_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandSource is ImageButton)
            {
                if (e.Item is GridDataItem)
                {
                    GridDataItem item = (GridDataItem)e.Item;
                    Session["selectedProductId"] = item["Id"].Text;
                }
                if (((ImageButton)e.CommandSource).CommandName == "Edit")
                {
                    Session["InsertCustomerProduktEdit"] = false;
                }
                else if (((ImageButton)e.CommandSource).CommandName == "InsertItem")
                {
                    Session["InsertCustomerProduktEdit"] = true;
                    e.Item.OwnerTableView.IsItemInserted = true;
                    e.Item.OwnerTableView.InsertItem();
                }
            }
        }

        protected void cmbErloeskonten_OnInit(object sender, EventArgs e)
        {
            var cmbErloeskonten = ((RadComboBox)sender);
            if (Session["selectedProductId"] != null && !String.IsNullOrEmpty(Session["selectedProductId"].ToString()))
            {
                string lblPriceId = Session["selectedProductId"].ToString();
                cmbErloeskonten.Items.Clear();
                cmbErloeskonten.Text = string.Empty;

                var accounts = ProductManager.GetAccounts().Select(o => new
                {
                    AccountId = o.AccountId,
                    CustomerId = o.CustomerId,
                    AccountNumber = o.AccountNumber,
                    AccountSelected = o.PriceAccount.Any(p => p.PriceId == Int32.Parse(lblPriceId)) ? false : true
                });


                if (!String.IsNullOrEmpty(AllCustomer.SelectedValue))
                {
                    accounts = accounts.Where(q => q.CustomerId == Int32.Parse(AllCustomer.SelectedValue));
                }

                cmbErloeskonten.DataSource = accounts.ToList();
                cmbErloeskonten.Text = "";
                cmbErloeskonten.DataBind();

                var selectedItem = accounts.SingleOrDefault(q => q.AccountSelected == true);

                if (selectedItem != null)
                    cmbErloeskonten.FindItemByValue(Convert.ToString(selectedItem.AccountId)).Selected = true;
            }
            else
            {
                cmbErloeskonten.Items.Clear();
                cmbErloeskonten.Text = string.Empty;

                var accounts = ProductManager.GetAccounts().Select(o => new
                {
                    AccountId = o.AccountId,
                    CustomerId = o.CustomerId,
                    AccountNumber = o.AccountNumber,
                    AccountSelected = false
                });

                if (!String.IsNullOrEmpty(AllCustomer.SelectedValue))
                {
                    accounts = accounts.Where(q => q.CustomerId == Int32.Parse(AllCustomer.SelectedValue));
                }

                cmbErloeskonten.DataSource = accounts.ToList();
                cmbErloeskonten.Text = "";
                cmbErloeskonten.DataBind();
            }
        }

        protected void btnSaveProduct_Click(object sender, EventArgs e)
        {
            bool insertUpdateOk = true;

            var senderButton = ((Button)sender);
            var errorMessage = ((Label)senderButton.FindControl("SchowErrorMessages"));
            errorMessage.Text = "";
            try
            {
                var productId = ((TextBox)senderButton.FindControl("ProductIdCustomer"));
                var locationId = ((TextBox)senderButton.FindControl("LocationIdCustomer"));
                var priceId = ((TextBox)senderButton.FindControl("txbPriceId"));
                var textAmount = ((TextBox)senderButton.FindControl("txbAmountCustomer"));
                var txtACharge = ((TextBox)senderButton.FindControl("txbAuthorativeChargeCustomer"));
                var cmbErloeskonten = ((RadComboBox)senderButton.FindControl("cmbErloeskonten"));

                int? erloesKonto = null;
                if (!String.IsNullOrEmpty(cmbErloeskonten.SelectedValue))
                {
                    erloesKonto = Int32.Parse(cmbErloeskonten.SelectedValue);
                }

                var cmbSelectedProduct = ((RadComboBox)senderButton.FindControl("cmdProductNames"));
                decimal? autCharge = null;
                if (txtACharge.Text == string.Empty)
                {
                    autCharge = null;
                }
                else
                {
                    if (EmptyStringIfNull.IsNumber(txtACharge.Text))
                    {
                        autCharge = EmptyStringIfNull.ReturnZeroDecimalIfNullEditVat(txtACharge.Text);
                    }
                    else
                    {
                        throw new Exception("Die amtlichen Gebühren müssen eine Dezimalzahl darstellen!");
                    }
                }


                decimal price = 0;
                if (EmptyStringIfNull.IsNumber(textAmount.Text))
                {
                    price = EmptyStringIfNull.ReturnZeroDecimalIfNullEditVat(textAmount.Text);
                }
                else
                {
                    throw new Exception("Der Preis muss eine Dezimalzahl darstellen!");
                }

                if (Session["InsertCustomerProduktEdit"] == null)
                {
                    throw new Exception("Die Session ist abgelaufen, bitte wiederholen Sie den Vorgang!");
                }

                if ((bool)Session["InsertCustomerProduktEdit"] == false)
                {
                    var newPrice = PriceManager.GetEntities(q => q.ProductId == Int32.Parse(productId.Text) &&
                        q.Id == Int32.Parse(priceId.Text)).SingleOrDefault();

                    if (newPrice != null)
                    {
                        newPrice.Amount = price;
                        newPrice.AuthorativeCharge = autCharge;
                        PriceManager.CreateAccount(erloesKonto, newPrice);
                    }

                    Session["selectedProductId"] = null;
                }
                else if ((bool)Session["InsertCustomerProduktEdit"] == true)
                {
                    if (cmbSelectedProduct.SelectedValue != "")
                    {
                        var newPrice = PriceManager.CreatePrice(price, autCharge, Int32.Parse(cmbSelectedProduct.SelectedValue), Int32.Parse(cmbLocations.SelectedValue), null);
                        PriceManager.CreateAccount(erloesKonto, newPrice, true);

                        Session["selectedProductId"] = null;
                    }
                    else
                    {
                        throw new Exception("Bitte wählen Sie ein Produkt aus");
                    }
                }
            }
            catch (Exception ex)
            {
                insertUpdateOk = false;

                errorMessage.Text = "Fehler:" + ex.Message;
                //TODO WriteLogItem("btnSaveProduct_Click Error " + ex.Message, LogTypes.ERROR, "Product");
            }


            if (insertUpdateOk)
            {
                getCustomerPrice.EditIndexes.Clear();
                getCustomerPrice.MasterTableView.IsItemInserted = false;
                getCustomerPrice.MasterTableView.Rebind();
            }
        }

        protected void btnAbortProduct_Click(object sender, EventArgs e)
        {
            Session["selectedProductId"] = null;
            getCustomerPrice.EditIndexes.Clear();
            getCustomerPrice.MasterTableView.IsItemInserted = false;
            getCustomerPrice.MasterTableView.Rebind();
        }

        protected void getAllCustomerProducts_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode)
            {
                var editForm = e.Item as GridEditFormItem;
                var productName = editForm.FindControl("ProductNameBoxCustomer") as TextBox;
                productName.Visible = true;
                var productNumber = editForm.FindControl("ProductNumberBoxCustomer") as TextBox;
                productNumber.Enabled = true;
                var productNames = editForm.FindControl("cmdProductNames") as RadComboBox;
                productNames.Visible = false;
            }

            if (e.Item is GridEditFormInsertItem && e.Item.OwnerTableView.IsItemInserted)
            {
                var editForm = e.Item as GridEditFormItem;
                var productName = editForm.FindControl("ProductNameBoxCustomer") as TextBox;
                productName.Visible = false;
                var productNumber = editForm.FindControl("ProductNumberBoxCustomer") as TextBox;
                productNumber.Enabled = false;
                productNumber.Text = "Automatisch";
                var productNames = editForm.FindControl("cmdProductNames") as RadComboBox;
                productNames.Visible = true;
            }
        }

        protected void getCustomerPrice_Init(object sender, System.EventArgs e)
        {
            GridFilterMenu menu = getCustomerPrice.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "Ist gleich" || menu.Items[i].Text == "Ist ungleich" || menu.Items[i].Text == "Ist größer als" || menu.Items[i].Text == "Ist kleiner als" || menu.Items[i].Text == "Ist größer gleich" || menu.Items[i].Text == "Ist kleiner gleich" || menu.Items[i].Text == "Enthält" || menu.Items[i].Text == "Kein Filter")
                {
                    i++;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }

        protected void RemovePrice_Click(object sender, EventArgs e)
        {
            try
            {
                var rbtSender = ((RadButton)sender);
                var lblPriceId = rbtSender.Parent.FindControl("lblPriceId") as Label;
                if (!String.IsNullOrEmpty(lblPriceId.Text))
                {
                    var selectedPrice = PriceManager.GetById(Int32.Parse(lblPriceId.Text));
                    if (selectedPrice != null)
                    {
                        ProductManager.RemovePrice(new Price[] { selectedPrice });
                    }
                    else
                    {
                        throw new Exception("Achtung, Fehler beim löschen (Preis wurde nicht gefunden). Bitte aktualisieren Sie die Webseite oder kontaktieren Sie den Administrator");
                    }
                }
            }
            catch (Exception ex)
            {
                RadWindowManagerCustomerPrice.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");

                //TODO WriteLogItem("RemovePrice_Click Error " + ex.Message, LogTypes.ERROR, "Price");
            }

            getCustomerPrice.EditIndexes.Clear();
            getCustomerPrice.MasterTableView.IsItemInserted = false;
            getCustomerPrice.MasterTableView.Rebind();
        }
    }
}