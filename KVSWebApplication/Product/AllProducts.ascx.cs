using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.Transactions;
using KVSCommon.Enums;
using KVSWebApplication.BasePages;

namespace KVSWebApplication.Product
{
    /// <summary>
    /// Codebehind fuer die Maske Alle Produkte
    /// </summary>
    public partial class AllProducts : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!UserManager.CheckPermissionsForUser(Session["UserPermissions"], PermissionTypes.PRODUKTE_BEARBEITEN))
                {
                    getAllProducts.Columns[0].Visible = false;
                }

                if (!UserManager.CheckPermissionsForUser(Session["UserPermissions"], PermissionTypes.PRODUKTE_ANLEGEN))
                {
                    foreach (GridItem item in getAllProducts.MasterTableView.GetItems(GridItemType.CommandItem))
                    {

                        ImageButton AllProductsAddButton = item.FindControl("add") as ImageButton;
                        AllProductsAddButton.Visible = false;
                    }
                }

                if (!UserManager.CheckPermissionsForUser(Session["UserPermissions"], PermissionTypes.PRODUKTE_SPERREN))
                {
                    //Implementation
                }

                if (!UserManager.CheckPermissionsForUser(Session["UserPermissions"], PermissionTypes.LOESCHEN))
                {
                    getAllProducts.MasterTableView.Columns[getAllProducts.MasterTableView.Columns.Count - 1].Visible = false;
                }

                if (!Page.IsPostBack)
                {
                    Session["myProductCategorys"] = new Dictionary<int, string>();
                    Session["myOrderTypeNames"] = new Dictionary<int, string>();
                    Session["myRegistrationOrderTypeNames"] = new Dictionary<int, string>();
                    Session["InsertEdit"] = false;
                    Session["selectedProductId"] = null;
                    Session["editableProductId"] = null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        protected void getAllProducts_ItemDataBound(object sender, GridItemEventArgs e)
        {
            try
            {
                if ((e.Item is GridEditableItem) && (e.Item.IsInEditMode) && (e.Item.ItemIndex != -1))
                {
                    var edititem = (GridEditableItem)e.Item;
                    string productId = edititem["Id"].Text;
                    Session["editableProductId"] = productId;

                    var cmbCustProd = e.Item.FindControl("cmbCustomerProducts") as RadComboBox;
                    cmbCustomerProducts_OnLoad(cmbCustProd);
                }
                else if ((e.Item is GridEditableItem) && (e.Item.IsInEditMode) && (e.Item.ItemIndex == -1))
                {
                    var cmbCustProd = e.Item.FindControl("cmbCustomerProducts") as RadComboBox;
                    var lbKundenProd = e.Item.FindControl("lbKundenProd") as Label;

                    if (cmbCustProd != null && lbKundenProd != null)
                    {
                        cmbCustProd.Visible = false;
                        lbKundenProd.Visible = false;
                    }
                }

                if (!UserManager.CheckPermissionsForUser(Session["UserPermissions"], PermissionTypes.PRODUKTE_ANLEGEN))
                {
                    foreach (GridItem item in getAllProducts.MasterTableView.GetItems(GridItemType.CommandItem))
                    {
                        var AllProductsAddButton = item.FindControl("add") as ImageButton;
                        AllProductsAddButton.Enabled = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        protected void GetAllProductsDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            Dictionary<int, string> myProductCategorys = ((Dictionary<int, string>)Session["myProductCategorys"]);
            Dictionary<int, string> myOrderTypeNames = ((Dictionary<int, string>)Session["myOrderTypeNames"]);
            Dictionary<int, string> myRegistrationOrderTypeNames = ((Dictionary<int, string>)Session["myRegistrationOrderTypeNames"]);

            KVSEntities dbContext = new KVSEntities();
            var query = from products in dbContext.Product
                        let PriceId = products.Price.FirstOrDefault(p => p.ProductId == products.Id && p.LocationId == null)
                        let price = (PriceId == null) ? (int?)null : PriceId.Id
                        orderby products.ItemNumber
                        select new
                        {
                            products.Id,
                            PriceId = price,
                            OrderTypeId = products.OrderTypeId,
                            ProductCategoryId = products.ProductCategoryId,
                            RegistrationOrderTypeId = products.RegistrationOrderTypeId,
                            ProductName = products.Name,
                            ItemNumber = products.ItemNumber != null ? products.ItemNumber : "",
                            OrderTypeName = products.OrderType != null ? products.OrderType.Name : "",
                            ProductCategorieName = products.ProductCategory != null ? products.ProductCategory.Name : "",
                            RegistrationOrderTypeName = products.RegistrationOrderType != null ? products.RegistrationOrderType.Name : "",
                            EnableDropDown = products.OrderType.Id == (int)OrderTypes.Admission ? "true" : "false",
                            Amount = EmptyStringIfNull.ReturnEmptyStringIfNull(products.Price.SingleOrDefault(q => q.ProductId == products.Id && q.LocationId == null).Amount.ToString()),
                            AutoCharge = EmptyStringIfNull.ReturnEmptyStringIfNull(products.Price.SingleOrDefault(q => q.ProductId == products.Id && q.LocationId == null).AuthorativeCharge.ToString()),
                            Vat = products.NeedsVAT == true ? "true" : "false",
                            AccountNumber = (from price_ in dbContext.Price
                                             join _priceAccounts in dbContext.PriceAccount on price_.Id equals _priceAccounts.PriceId
                                             where price_.Id == price && price_.LocationId == null
                                             select _priceAccounts.Accounts.AccountNumber).SingleOrDefault()
                        };
            try
            {
                myProductCategorys.Clear();
                foreach (var Category in ProductCategoryCollection)
                {
                    if (!myProductCategorys.ContainsKey(Category.Id))
                        myProductCategorys.Add(Category.Id, Category.Name);
                }
                

                myOrderTypeNames.Clear();
                foreach (var orderTypeName in OrderTypesCollection)
                {
                    if (!myOrderTypeNames.ContainsKey(orderTypeName.Id))
                        myOrderTypeNames.Add(orderTypeName.Id, orderTypeName.Name);
                }

                myRegistrationOrderTypeNames.Clear();
                foreach (var RegistrationOrderTypeName in RegistrationOrderTypeCollection)
                {
                    if (myRegistrationOrderTypeNames.ContainsKey(RegistrationOrderTypeName.Id) == false)
                        myRegistrationOrderTypeNames.Add(RegistrationOrderTypeName.Id, RegistrationOrderTypeName.Name);
                }
            }
            catch (Exception ex)
            {
                RadWindowManagerAllProducts.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                //TODO WriteLogItem("Product Error " + ex.Message, LogTypes.ERROR, "Product");
            }
            e.Result = query;
        }

        protected void Grid_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            try
            {
                if (e.Item is GridEditableItem && e.Item.IsInEditMode)
                {
                    var item = e.Item as GridEditableItem;
                    string Employee = (item["ctlColumnGridWorkflow"].Controls[0] as DropDownList).SelectedItem.Text;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void cmbCustomerProducts_OnLoad(RadComboBox Box)
        {
            try
            {
                if (Session["editableProductId"] != null && !String.IsNullOrEmpty(Session["editableProductId"].ToString()))
                {
                    string myProductId = Session["editableProductId"].ToString();
                    Box.Visible = true;

                    var customers = CustomerManager.GetEntities(o => o.LargeCustomer != null).Select(cust => new
                    {
                        CustomerId = cust.Id,
                        CustomerName = String.IsNullOrEmpty(cust.MatchCode) ? cust.Name : cust.Name + "(" + cust.MatchCode + ")",
                        IsChecked = cust.CustomerProduct.Any(p => p.ProductId == Int32.Parse(myProductId)) ? true : false
                    }).ToList();

                    Box.DataSource = customers.OrderBy(o => o.CustomerName);
                    Box.DataBind();
                }
            }
            catch (Exception ex)
            {
                //TODO WriteLogItem("Product Error " + ex.Message, LogTypes.ERROR, "Product");
                throw new Exception(ex.Message);
            }
        }
        
        protected void cmbCategory_OnLoad(object sender, EventArgs e)
        {
            try
            {
                var myProductCategorys = ((Dictionary<int, string>)Session["myProductCategorys"]);
                var cmbCategory = ((RadComboBox)sender);
                cmbCategory.Items.Clear();
                foreach (var myItem in myProductCategorys)
                {
                    cmbCategory.Items.Add(new RadComboBoxItem(myItem.Value, myItem.Key.ToString()));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        protected void cmbOrderTypeName_OnLoad(object sender, EventArgs e)
        {
            try
            {
                var myOrderTypeNames = ((Dictionary<int, string>)Session["myOrderTypeNames"]);
                var cmbOrderTypeName = ((RadComboBox)sender);
                cmbOrderTypeName.Items.Clear();
                foreach (var myItem in myOrderTypeNames)
                {
                    cmbOrderTypeName.Items.Add(new RadComboBoxItem(myItem.Value, myItem.Key.ToString()));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        protected void cmbRegistrationOrderTypeName_OnLoad(object sender, EventArgs e)
        {
            try
            {
                var myRegistrationOrderTypeNames = ((Dictionary<int, string>)Session["myRegistrationOrderTypeNames"]);
                var cmbRegistrationOrderTypeName = ((RadComboBox)sender);
                cmbRegistrationOrderTypeName.Items.Clear();
                foreach (var myItem in myRegistrationOrderTypeNames)
                {
                    cmbRegistrationOrderTypeName.Items.Add(new RadComboBoxItem(myItem.Value, myItem.Key.ToString()));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        protected void cmbOrderTypeName_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var myCombobox = (RadComboBox)((RadComboBox)o).Parent.FindControl("cmbRegistrationOrderTypeName");

            if (Int32.Parse(e.Value) == (int)OrderTypes.Admission)
                myCombobox.Enabled = true;
            else
                myCombobox.Enabled = false;
        }

        protected void cmbErloeskonten_OnInit(object sender, EventArgs e)
        {
            try
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
                        AccountNumber = o.AccountNumber,
                        AccountSelected = o.PriceAccount.Any(p => p.PriceId == Int32.Parse(lblPriceId)) ? false : true
                    }).ToList();

                    cmbErloeskonten.DataSource = accounts;
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
                        AccountNumber = o.AccountNumber,
                        AccountSelected = false
                    }).ToList();

                    cmbErloeskonten.DataSource = accounts;
                    cmbErloeskonten.Text = "";
                    cmbErloeskonten.DataBind();
                }
            }
            catch (Exception ex)
            {
                //TODO WriteLogItem("Product Error " + ex.Message, LogTypes.ERROR, "Product");
                throw new Exception(ex.Message);
            }
        }

        protected void btnSaveProduct_Click(object sender, EventArgs e)
        {
            bool insertUpdateOk = true;

            Button myButton = ((Button)sender);
            var errorMessage = ((Label)myButton.FindControl("SchowErrorMessages"));
            try
            {
                errorMessage.Text = "";
                var productId = ((TextBox)myButton.FindControl("ProductId"));
                var productName = ((TextBox)myButton.FindControl("ProductNameBox"));
                var productNumber = ((TextBox)myButton.FindControl("ProductNumberBox"));
                var Category = ((RadComboBox)myButton.FindControl("cmbCategory"));
                var comboCmbOrderTypeName = ((RadComboBox)myButton.FindControl("cmbOrderTypeName"));
                var comboRegisOrTypeName = ((RadComboBox)myButton.FindControl("cmbRegistrationOrderTypeName"));
                var textAmount = ((TextBox)myButton.FindControl("txbAmount"));
                var txtACharge = ((TextBox)myButton.FindControl("txbAuthorativeCharge"));
                var chbVat = ((CheckBox)myButton.FindControl("chbVAT"));
                var cmbErloeskonten = ((RadComboBox)myButton.FindControl("cmbErloeskonten"));
                int? erloesKonto = null;

                var cmbCustomerProduct = ((RadComboBox)myButton.FindControl("cmbCustomerProducts"));

                UpdateCustomerProductsTabel(cmbCustomerProduct, productId.Text);

                if (!String.IsNullOrEmpty(cmbErloeskonten.SelectedValue))
                {
                    erloesKonto = Int32.Parse(cmbErloeskonten.SelectedValue);
                }

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

                if (productName.Text == string.Empty || productNumber.Text == string.Empty || Category.Text == string.Empty ||
                    comboCmbOrderTypeName.Text == string.Empty || (comboRegisOrTypeName.Text == string.Empty && comboRegisOrTypeName.Enabled == true) || textAmount.Text == string.Empty)
                {
                    throw new Exception("Alle Felder, bis auf die behördliche Gebühr müssen ausgefüllt sein!");
                }
                else
                {
                    if ((bool)Session["InsertEdit"] == false)
                    {
                        var editProductData = ProductManager.GetById(Int32.Parse(productId.Text));
                        if (editProductData == null)
                        {
                            throw new Exception("Es ist ein Fehler aufgetretten, bitte aktualisieren Sie die Seite und versuchen Sie es erneut!");
                        }
                        else
                        {
                            editProductData.Name = productName.Text;
                            editProductData.ItemNumber = productNumber.Text;
                            editProductData.NeedsVAT = chbVat.Checked;

                            ProductManager.SaveChanges();

                            if (ProductManager.GetEntities(q => q.ItemNumber == productNumber.Text && q.Id != Int32.Parse(productId.Text)).Count() != 0)
                            {
                                throw new Exception("Diese Produktnummer ist bereits an ein anderes Produkt vergeben!");
                            }

                            if (Category.SelectedIndex != -1)
                                editProductData.ProductCategoryId = Int32.Parse(Category.SelectedValue);
                            if (comboCmbOrderTypeName.SelectedIndex != -1)
                                editProductData.OrderTypeId = Int32.Parse(comboCmbOrderTypeName.SelectedValue);
                            if (comboRegisOrTypeName.SelectedIndex != -1)
                                editProductData.RegistrationOrderTypeId = Int32.Parse(comboRegisOrTypeName.SelectedValue);

                            var editPrice = PriceManager.GetEntities(q => q.LocationId == null && q.ProductId == Int32.Parse(productId.Text)).SingleOrDefault();
                            if (editPrice == null)
                            {
                                if (textAmount.Text != string.Empty)
                                {
                                    var newPrice = PriceManager.CreatePrice(price, autCharge, Int32.Parse(productId.Text), null, null);
                                    PriceManager.CreateAccount(erloesKonto, newPrice);
                                }
                            }
                            else
                            {
                                editPrice.Amount = price;
                                editPrice.AuthorativeCharge = autCharge;
                                PriceManager.CreateAccount(erloesKonto, editPrice);
                            }

                            Session["selectedProductId"] = null;
                        }
                    }
                    else if ((bool)Session["InsertEdit"] == true)
                    {
                        if (ProductManager.GetEntities(q => q.ItemNumber == productNumber.Text).FirstOrDefault() != null)
                        {
                            throw new Exception("Diese Produktnummer ist bereits an ein anderes Produkt vergeben!");
                        }

                        var newPrice = ProductManager.CreateProduct(productName.Text, Int32.Parse(Category.SelectedValue), price, autCharge, productNumber.Text,
                            Int32.Parse(comboCmbOrderTypeName.SelectedValue),
                            ((comboRegisOrTypeName.SelectedValue != string.Empty || comboRegisOrTypeName.SelectedIndex != -1) ? Int32.Parse(comboRegisOrTypeName.SelectedValue) :
                            (int?)null), chbVat.Checked);

                        PriceManager.CreateAccount(erloesKonto, newPrice);

                        Session["selectedProductId"] = null;
                    }
                }
            }
            catch (Exception ex)
            {
                insertUpdateOk = false;
                errorMessage.Text = "Fehler:" + ex.Message;

                //TODO WriteLogItem("Product Error " + ex.Message, LogTypes.ERROR, "Product");
            }


            if (insertUpdateOk)
            {
                getAllProducts.EditIndexes.Clear();
                getAllProducts.MasterTableView.IsItemInserted = false;
                getAllProducts.MasterTableView.Rebind();
            }
        }

        protected void UpdateCustomerProductsTabel(RadComboBox cmbCustomerProduct, string productId)
        {
            foreach (RadComboBoxItem item in cmbCustomerProduct.Items)
            {
                if (!String.IsNullOrEmpty(item.Value))
                {
                    ProductManager.UpdateCustomerProducts(Int32.Parse(item.Value), Int32.Parse(productId), item.Checked);                    
                }
            }
        }

        protected void btnAbort_Click(object sender, EventArgs e)
        {
            try
            {
                Session["selectedProductId"] = null;
                getAllProducts.EditIndexes.Clear();
                getAllProducts.MasterTableView.IsItemInserted = false;
                getAllProducts.MasterTableView.Rebind();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        protected void getAllProducts_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            try
            {
                if (e.CommandSource is ImageButton)
                {
                    if (e.Item is GridDataItem)
                    {
                        var item = (GridDataItem)e.Item;
                        Session["selectedProductId"] = item["Id"].Text;
                    }
                    if (((ImageButton)e.CommandSource).CommandName == "Edit")
                    {
                        Session["InsertEdit"] = false;
                    }
                    else if (((ImageButton)e.CommandSource).CommandName == "InsertItem")
                    {
                        Session["InsertEdit"] = true;
                        e.Item.OwnerTableView.IsItemInserted = true;
                        e.Item.OwnerTableView.InsertItem();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        protected void getAllProducts_Init(object sender, System.EventArgs e)
        {
            try
            {
                GridFilterMenu menu = getAllProducts.FilterMenu;
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        protected void RemoveProduct_Click(object sender, EventArgs e)
        {
            try
            {
                var rbtSender = ((RadButton)sender);
                var lblProductId = rbtSender.Parent.FindControl("lblProductId") as Label;
                if (!String.IsNullOrEmpty(lblProductId.Text))
                {
                    ProductManager.RemoveProduct(Int32.Parse(lblProductId.Text));
                }
            }
            catch (Exception ex)
            {
                RadWindowManagerAllProducts.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                //TODO WriteLogItem("Product Error " + ex.Message, LogTypes.ERROR, "Product");
            }

            getAllProducts.EditIndexes.Clear();
            getAllProducts.MasterTableView.IsItemInserted = false;
            getAllProducts.MasterTableView.Rebind();
        }
    }
}