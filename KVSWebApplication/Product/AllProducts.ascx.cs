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
namespace KVSWebApplication.Product
{
    /// <summary>
    /// Codebehind fuer die Maske Alle Produkte
    /// </summary>
    public partial class AllProducts : System.Web.UI.UserControl
    {
        List<string> thisUserPermissions = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(Int32.Parse(Session["CurrentUserId"].ToString())));
                if (!thisUserPermissions.Contains("PRODUKTE_BEARBEITEN"))
                {
                    getAllProducts.Columns[0].Visible = false;
                }
                if (!thisUserPermissions.Contains("PRODUKTE_ANLEGEN"))
                {
                    foreach (GridItem item in getAllProducts.MasterTableView.GetItems(GridItemType.CommandItem))
                    {

                        ImageButton AllProductsAddButton = item.FindControl("add") as ImageButton;
                        AllProductsAddButton.Visible = false;
                    }
                }
                if (!thisUserPermissions.Contains("PRODUKTE_SPERREN"))
                {
                    //Implementation
                }
                if (!thisUserPermissions.Contains("LOESCHEN"))
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
                    GridEditableItem edititem = (GridEditableItem)e.Item;
                    string productId = edititem["Id"].Text;
                    Session["editableProductId"] = productId;

                    RadComboBox cmbCustProd = e.Item.FindControl("cmbCustomerProducts") as RadComboBox;
                    cmbCustomerProducts_OnLoad(cmbCustProd);
                }
                else if ((e.Item is GridEditableItem) && (e.Item.IsInEditMode) && (e.Item.ItemIndex == -1))
                {
                    RadComboBox cmbCustProd = e.Item.FindControl("cmbCustomerProducts") as RadComboBox;
                    Label lbKundenProd = e.Item.FindControl("lbKundenProd") as Label;

                    if (cmbCustProd != null && lbKundenProd != null)
                    {
                        cmbCustProd.Visible = false;
                        lbKundenProd.Visible = false;
                    }
                }

                if (!thisUserPermissions.Contains("PRODUKTE_ANLEGEN"))
                {
                    foreach (GridItem item in getAllProducts.MasterTableView.GetItems(GridItemType.CommandItem))
                    {
                        ImageButton AllProductsAddButton = item.FindControl("add") as ImageButton;
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
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
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
                            Vat = products.NeedsVAT != null && products.NeedsVAT == true ? "true" : "false",
                            AccountNumber = (from price_ in dbContext.Price
                                             join _priceAccounts in dbContext.PriceAccount on price_.Id equals _priceAccounts.PriceId
                                             where price_.Id == price && price_.LocationId == null
                                             select _priceAccounts.Accounts.AccountNumber).SingleOrDefault()
                        };
            try
            {
                var productCategorys = from pCategorys in dbContext.ProductCategory
                                       select new { pCategorys.Id, pCategorys.Name };
                myProductCategorys.Clear();


                foreach (var Category in productCategorys)
                {
                    if (Category.Id.ToString() != string.Empty)
                    {
                        if (myProductCategorys.ContainsKey(Category.Id) == false)
                            myProductCategorys.Add(Category.Id, Category.Name);
                    }
                }
                myOrderTypeNames.Clear();
                var orderTypeNames = from oTNames in dbContext.OrderType
                                     select new { oTNames.Id, oTNames.Name };

                foreach (var orderTypeName in orderTypeNames)
                {
                    if (orderTypeName.Id.ToString() != string.Empty)
                    {
                        if (myOrderTypeNames.ContainsKey(orderTypeName.Id) == false)
                            myOrderTypeNames.Add(orderTypeName.Id, orderTypeName.Name);
                    }
                }
                myRegistrationOrderTypeNames.Clear();
                var RegistrationOrderTypeNames = from ROTNames in dbContext.RegistrationOrderType
                                                 select new { ROTNames.Id, ROTNames.Name };

                foreach (var RegistrationOrderTypeName in RegistrationOrderTypeNames)
                {
                    if (RegistrationOrderTypeName.Id.ToString() != string.Empty)
                    {
                        if (myOrderTypeNames.ContainsKey(RegistrationOrderTypeName.Id) == false)
                            myRegistrationOrderTypeNames.Add(RegistrationOrderTypeName.Id, RegistrationOrderTypeName.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                RadWindowManagerAllProducts.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                try
                {
                    dbContext.WriteLogItem("Product Error " + ex.Message, LogTypes.ERROR, "Product");
                }
                catch { }
            }
            e.Result = query;
        }


        protected void Grid_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            try
            {
                if (e.Item is GridEditableItem && e.Item.IsInEditMode)
                {
                    GridEditableItem item = e.Item as GridEditableItem;
                    string Employee = (item["ctlColumnGridWorkflow"].Controls[0] as DropDownList).SelectedItem.Text;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void cmbCustomerProducts_OnLoad(RadComboBox Box)// public void cmbCustomerProducts_OnLoad(object sender, EventArgs e)
        {
            using (DataClasses1DataContext dbContext = new DataClasses1DataContext())
            {
                try
                {
                    if (Session["editableProductId"] != null && !String.IsNullOrEmpty(Session["editableProductId"].ToString()))
                    {
                        string myProductId = Session["editableProductId"].ToString();
                        Box.Visible = true;

                        var myCustomers = from cust in dbContext.Customer
                                          join lCust in dbContext.LargeCustomer on cust.Id equals lCust.CustomerId
                                          orderby cust.Name ascending
                                          select new
                                          {
                                              CustomerId = cust.Id,
                                              CustomerName = String.IsNullOrEmpty(cust.MatchCode) ? cust.Name : cust.Name + "(" + cust.MatchCode + ")",
                                              IsChecked = dbContext.CustomerProduct.SingleOrDefault(q => q.ProductId == Int32.Parse(myProductId) && q.CustomerId == cust.Id) != null ? true : false
                                          };

                        Box.DataSource = myCustomers;
                        Box.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    dbContext.WriteLogItem("Product Error " + ex.Message, LogTypes.ERROR, "Product");
                    throw new Exception(ex.Message);
                }
            }
        }


        protected void cmbCategory_OnLoad(object sender, EventArgs e)
        {
            try
            {
                var myProductCategorys = ((Dictionary<int, string>)Session["myProductCategorys"]);
                RadComboBox cmbCategory = ((RadComboBox)sender);
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
                RadComboBox cmbOrderTypeName = ((RadComboBox)sender);
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
                RadComboBox cmbRegistrationOrderTypeName = ((RadComboBox)sender);
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
            RadComboBox myCombobox = (RadComboBox)((RadComboBox)o).Parent.FindControl("cmbRegistrationOrderTypeName");
            try
            {
                if (Int32.Parse(e.Value) == (int)OrderTypes.Admission)
                    myCombobox.Enabled = true;
                else
                    myCombobox.Enabled = false;
            }
            catch
            {
                myCombobox.Enabled = false;
            }
        }

        protected void cmbErloeskonten_OnInit(object sender, EventArgs e)
        {
            using (DataClasses1DataContext dbContext = new DataClasses1DataContext())
            {
                try
                {

                    RadComboBox cmbErloeskonten = ((RadComboBox)sender);
                    if (Session["selectedProductId"] != null && !String.IsNullOrEmpty(Session["selectedProductId"].ToString()))
                    {
                        string lblPriceId = Session["selectedProductId"].ToString();
                        cmbErloeskonten.Items.Clear();
                        cmbErloeskonten.Text = string.Empty;
                        var myErloeskonten = from _accounts in dbContext.Accounts
                                             select new
                                             {
                                                 AccountId = _accounts.AccountId,
                                                 AccountNumber = _accounts.AccountNumber,
                                                 AccountSelected = (dbContext.PriceAccount.SingleOrDefault(s => s.AccountId == _accounts.AccountId &&
                                                  s.PriceId == Int32.Parse(lblPriceId)) == null ? false : true)
                                             };
                        cmbErloeskonten.DataSource = myErloeskonten;
                        cmbErloeskonten.Text = "";
                        cmbErloeskonten.DataBind();
                        var selectedItem = myErloeskonten.SingleOrDefault(q => q.AccountSelected == true);
                        if (selectedItem != null)
                            cmbErloeskonten.FindItemByValue(Convert.ToString(selectedItem.AccountId)).Selected = true;
                    }
                    else
                    {
                        cmbErloeskonten.Items.Clear();
                        cmbErloeskonten.Text = string.Empty;
                        IQueryable myErloeskonten = from _accounts in dbContext.Accounts
                                                    select new
                                                    {
                                                        AccountId = _accounts.AccountId,
                                                        AccountNumber = _accounts.AccountNumber,
                                                        AccountSelected = false,
                                                    };
                        cmbErloeskonten.DataSource = myErloeskonten;
                        cmbErloeskonten.Text = "";
                        cmbErloeskonten.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    dbContext.WriteLogItem("Product Error " + ex.Message, LogTypes.ERROR, "Product");
                    throw new Exception(ex.Message);
                }
            }
        }
        protected void btnSaveProduct_Click(object sender, EventArgs e)
        {
            bool insertUpdateOk = true;
            using (TransactionScope ts = new TransactionScope())
            {

                DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())); // hier kommt die Loggingid
                Button myButton = ((Button)sender);
                Label errorMessage = ((Label)myButton.FindControl("SchowErrorMessages"));
                try
                {
                    errorMessage.Text = "";
                    TextBox productId = ((TextBox)myButton.FindControl("ProductId"));
                    TextBox productName = ((TextBox)myButton.FindControl("ProductNameBox"));
                    TextBox productNumber = ((TextBox)myButton.FindControl("ProductNumberBox"));
                    RadComboBox Category = ((RadComboBox)myButton.FindControl("cmbCategory"));
                    RadComboBox comboCmbOrderTypeName = ((RadComboBox)myButton.FindControl("cmbOrderTypeName"));
                    RadComboBox comboRegisOrTypeName = ((RadComboBox)myButton.FindControl("cmbRegistrationOrderTypeName"));
                    TextBox textAmount = ((TextBox)myButton.FindControl("txbAmount"));
                    TextBox txtACharge = ((TextBox)myButton.FindControl("txbAuthorativeCharge"));
                    CheckBox chbVat = ((CheckBox)myButton.FindControl("chbVAT"));
                    RadComboBox cmbErloeskonten = ((RadComboBox)myButton.FindControl("cmbErloeskonten"));
                    int? erloesKonto = null;

                    RadComboBox cmbCustomerProduct = ((RadComboBox)myButton.FindControl("cmbCustomerProducts"));

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
                            var editProductData = dbContext.Product.SingleOrDefault(q => q.Id == Int32.Parse(productId.Text));
                            if (editProductData == null)
                            {
                                throw new Exception("Es ist ein Fehler aufgetretten, bitte aktualisieren Sie die Seite und versuchen Sie es erneut!");
                            }
                            else
                            {
                                editProductData.LogDBContext = dbContext;
                                editProductData.Name = productName.Text;
                                editProductData.ItemNumber = productNumber.Text;
                                editProductData.NeedsVAT = chbVat.Checked;
                                if (dbContext.Product.FirstOrDefault(q => q.ItemNumber == productNumber.Text && q.Id != Int32.Parse(productId.Text)) != null)
                                {
                                    throw new Exception("Diese Produktnummer ist bereits an ein anderes Produkt vergeben!");
                                }

                                if (Category.SelectedIndex != -1)
                                    editProductData.ProductCategoryId = Int32.Parse(Category.SelectedValue);
                                if (comboCmbOrderTypeName.SelectedIndex != -1)
                                    editProductData.OrderTypeId = Int32.Parse(comboCmbOrderTypeName.SelectedValue);
                                if (comboRegisOrTypeName.SelectedIndex != -1)
                                    editProductData.RegistrationOrderTypeId = Int32.Parse(comboRegisOrTypeName.SelectedValue);

                                var editPrice = dbContext.Price.SingleOrDefault(q => q.LocationId == null && q.ProductId == Int32.Parse(productId.Text));
                                if (editPrice == null)
                                {
                                    if (textAmount.Text != string.Empty)
                                    {
                                        var newPrice = KVSCommon.Database.Price.CreatePrice(price, autCharge, Int32.Parse(productId.Text), null, null, dbContext);
                                        PriceAccountHelper.CreateAccount(erloesKonto, newPrice, dbContext);
                                    }
                                }
                                else
                                {
                                    editPrice.LogDBContext = dbContext;
                                    editPrice.Amount = price;
                                    editPrice.AuthorativeCharge = autCharge;
                                    PriceAccountHelper.CreateAccount(erloesKonto, editPrice, dbContext);
                                }
                                dbContext.SubmitChanges();
                                Session["selectedProductId"] = null;
                            }
                        }
                        else if ((bool)Session["InsertEdit"] == true)
                        {
                            if (dbContext.Product.FirstOrDefault(q => q.ItemNumber == productNumber.Text) != null)
                            {
                                throw new Exception("Diese Produktnummer ist bereits an ein anderes Produkt vergeben!");
                            }
                            Price newPrice = KVSCommon.Database.Product.CreateProduct(productName.Text, Int32.Parse(Category.SelectedValue), price, autCharge, productNumber.Text,
                                Int32.Parse(comboCmbOrderTypeName.SelectedValue),
                                ((comboRegisOrTypeName.SelectedValue != string.Empty || comboRegisOrTypeName.SelectedIndex != -1) ? Int32.Parse(comboRegisOrTypeName.SelectedValue) :
                                (int?)null), chbVat.Checked, true, dbContext);

                            PriceAccountHelper.CreateAccount(erloesKonto, newPrice, dbContext);

                            Session["selectedProductId"] = null;
                        }
                    }
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    insertUpdateOk = false;
                    if (ts != null)
                        ts.Dispose();
                    errorMessage.Text = "Fehler:" + ex.Message;
                    //RadWindowManagerAllProducts.RadAlert(ex.Message, 380, 180, "Fehler", "");
                    dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                    dbContext.WriteLogItem("Product Error " + ex.Message, LogTypes.ERROR, "Product");
                    dbContext.SubmitChanges();
                }

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
            using (DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString())))
            {
                foreach (RadComboBoxItem Item in cmbCustomerProduct.Items)
                {
                    if (!String.IsNullOrEmpty(Item.Value))
                    {
                        var CustProdRelation = dbContext.CustomerProduct.SingleOrDefault(q => q.CustomerId == Int32.Parse(Item.Value) &&
                            q.ProductId == Int32.Parse(productId));

                        if (CustProdRelation != null) // exists
                        {
                            if (Item.Checked) //checked and exists - nothing
                            {

                            }
                            else // exits, but not checked - delete
                            {
                                dbContext.CustomerProduct.DeleteOnSubmit(CustProdRelation);
                            }
                        }
                        else //not exists
                        {
                            if (Item.Checked) //checked and not exists - insert
                            {
                                CustomerProduct newCustProd = new CustomerProduct
                                {
                                    CustomerId = Int32.Parse(Item.Value),
                                    ProductId = Int32.Parse(productId)
                                };

                                dbContext.CustomerProduct.InsertOnSubmit(newCustProd);
                            }
                            else // not exists and not checked - nothing
                            {
                                //  dbContext.CustomerProduct.DeleteOnSubmit(CustProdRelation);
                            }
                        }

                        dbContext.SubmitChanges();
                    }
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
                        GridDataItem item = (GridDataItem)e.Item;
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
            using (TransactionScope ts = new TransactionScope())
            {
                DataClasses1DataContext dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));


                try
                {
                    RadButton rbtSender = ((RadButton)sender);
                    Label lblProductId = rbtSender.Parent.FindControl("lblProductId") as Label;
                    if (!String.IsNullOrEmpty(lblProductId.Text))
                    {
                        KVSCommon.Database.Product.RemoveProduct(Int32.Parse(lblProductId.Text), dbContext);
                        dbContext.SubmitChanges();
                    }
                    ts.Complete();
                }
                catch (Exception ex)
                {

                    if (ts != null)
                        ts.Dispose();
                    RadWindowManagerAllProducts.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                    try
                    {
                        dbContext = new DataClasses1DataContext(Int32.Parse(Session["CurrentUserId"].ToString()));
                        dbContext.WriteLogItem("Product Error " + ex.Message, LogTypes.ERROR, "Product");
                        dbContext.SubmitChanges();
                    }
                    catch { }
                }

            }
            getAllProducts.EditIndexes.Clear();
            getAllProducts.MasterTableView.IsItemInserted = false;
            getAllProducts.MasterTableView.Rebind();
        }
    }
}