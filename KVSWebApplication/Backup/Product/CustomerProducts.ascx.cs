using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.Transactions;
namespace KVSWebApplication.Product
{
    /// <summary>
    /// Codebehind fuer die Kundenprodukte
    /// </summary>
    public partial class CustomerProducts : System.Web.UI.UserControl
    {       
        List<string> thisUserPermissions = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(((Guid)Session["CurrentUserId"])));
            if (!thisUserPermissions.Contains("PRODUKTE_BEARBEITEN"))
            {
                getCustomerPrice.Columns[0].Visible = false;
            }
            if (!thisUserPermissions.Contains("PRODUKTE_ANLEGEN"))
            {
                foreach (GridItem item in getCustomerPrice.MasterTableView.GetItems(GridItemType.CommandItem))
                {
                    ImageButton CustomerProductsAddButton = item.FindControl("add") as ImageButton;
                    CustomerProductsAddButton.Visible = false;
                    CustomerProductsAddButton.Enabled = false;
                }              
            }
            if (!thisUserPermissions.Contains("PRODUKTE_SPERREN"))
            {
                //Implementation
            }
            if (!thisUserPermissions.Contains("LOESCHEN"))
            {
                getCustomerPrice.MasterTableView.Columns[getCustomerPrice.MasterTableView.Columns.Count - 1].Visible = false;
            }
            if (!Page.IsPostBack)
            {
                Session["selectedProductId"] = "";
            }
        }
        protected void GetCustomerProductsDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            if (cmbLocations.SelectedValue == "")
                cmbLocations.SelectedValue = Guid.Empty.ToString();
            if (AllCustomer.SelectedValue == "")
                AllCustomer.SelectedValue = Guid.Empty.ToString();
            var query = from price in dbContext.Price
                        join products in dbContext.Product on price.ProductId equals products.Id
                        where price.Location.CustomerId == new Guid(AllCustomer.SelectedValue.ToString()) &&
                        price.LocationId == new Guid(cmbLocations.SelectedValue.ToString())
                        orderby products.ItemNumber
                        select new
                        {
                            Id = products.Id,
                            CustomerId =  price.Location.CustomerId ,
                            LocationIdCustomer = price.LocationId,
                            PriceId = price.Id,
                            OrderTypeId = products.OrderTypeId,
                            ProductCategoryId = products.ProductCategoryId,
                            RegistrationOrderTypeId = products.RegistrationOrderTypeId,
                            ProductName = products.Name,
                            products.ItemNumber,
                            OrderTypeName = products.OrderType != null ? products.OrderType.Name : "",
                            ProductCategorieName = products.ProductCategory != null ? products.ProductCategory.Name : "",
                            RegistrationOrderTypeName = products.RegistrationOrderType != null ? products.RegistrationOrderType.Name : "",
                            AccountNumber = EmptyStringIfNull.ReturnEmptyStringIfNull(dbContext.PriceAccount.SingleOrDefault(f => f.PriceId == price.Id).Accounts.AccountNumber),
                            Amount = Math.Round(price.Amount, 2, MidpointRounding.AwayFromZero).ToString(),
                            AutoCharge = price.AuthorativeCharge != null ? Math.Round(price.AuthorativeCharge.Value, 2, MidpointRounding.AwayFromZero).ToString() : ""
                        };
            e.Result = query;
            if (new Guid(cmbLocations.SelectedValue) == Guid.Empty || new Guid(AllCustomer.SelectedValue) == Guid.Empty)
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
            if (new Guid(cmbLocations.SelectedValue) == Guid.Empty)
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
            if (new Guid(AllCustomer.SelectedValue) == Guid.Empty)
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
                if (!thisUserPermissions.Contains("PRODUKTE_ANLEGEN"))
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
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            var query =     from customer in dbContext.Customer 
                        select new
                        {
                            Name = customer.SmallCustomer != null && customer.SmallCustomer.Person != null ? customer.SmallCustomer.Person.FirstName + " " + customer.SmallCustomer.Person.Name : customer.Name, 
                            Value = customer.Id,
                            Matchcode = customer.MatchCode,
                            Kundennummer = customer.CustomerNumber
                        };
            AllCustomer.Items.Clear();
            AllCustomer.DataSource = query;
            AllCustomer.DataBind();
        }
        protected void cmdProductNames_OnInit(object sender, EventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            RadComboBox myCustomerProducts = ((RadComboBox)sender);
            var query = from product in  dbContext.Product.Where(q => !q.Price.Any(p => p.LocationId == new Guid(cmbLocations.SelectedValue.ToString())))
                        select new
                        {
                            ItemNumber = product.ItemNumber,
                            Name = product.Name,
                            Value = product.Id,
                            Category = product.ProductCategory.Name
                        };
            myCustomerProducts.Items.Clear();
            myCustomerProducts.DataSource = query;
            myCustomerProducts.DataBind();
        }
        protected void CustomerCombobox_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            if (AllCustomer.SelectedValue != string.Empty)
            {
                var query = from customer in dbContext.Customer 
                            join largeCustomer in dbContext.LargeCustomer on customer.Id equals largeCustomer.CustomerId
                            join _location in dbContext.Location on customer.Id equals _location.CustomerId
                            where customer.Id == new Guid(e.Value)
                            select new
                            {
                                _location.Id,
                                _location.Name
                            };
                cmbLocations.Items.Clear();
                foreach (var location in query)
                {
                    if (location.Id.ToString() != string.Empty)
                    {
                        cmbLocations.Items.Add(new RadComboBoxItem(location.Name , location.Id.ToString()));
                    }
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
                    Session["selectedProductId"] = item["PriceId"].Text;
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
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            RadComboBox cmbErloeskonten = ((RadComboBox)sender);
            if (Session["selectedProductId"] != null && Session["selectedProductId"].ToString() != string.Empty)
            {
                string lblPriceId = Session["selectedProductId"].ToString();
                cmbErloeskonten.Items.Clear();
                cmbErloeskonten.Text = string.Empty;
                var myErloeskonten = from _accounts in dbContext.Accounts                                     
                                     select new
                                     {
                                         AccountId = _accounts.AccountId,
                                         CustomerId = _accounts.CustomerId,
                                         AccountNumber = _accounts.AccountNumber,
                                         AccountSelected = (dbContext.PriceAccount.SingleOrDefault(s => s.AccountId == _accounts.AccountId &&
                                                         s.PriceId == new Guid(lblPriceId)) == null ? false : true)
                                     };
                if (!String.IsNullOrEmpty(AllCustomer.SelectedValue))
                {
                    myErloeskonten = myErloeskonten.Where(q=>q.CustomerId==new Guid(AllCustomer.SelectedValue));
                }
                cmbErloeskonten.DataSource = myErloeskonten;
                cmbErloeskonten.Text = "";
                cmbErloeskonten.DataBind();
                var selectedItem = myErloeskonten.SingleOrDefault(q => q.AccountSelected == true);
                if(selectedItem!=null)
                    cmbErloeskonten.FindItemByValue(Convert.ToString(selectedItem.AccountId)).Selected = true;
            }
            else
            {
                cmbErloeskonten.Items.Clear();
                cmbErloeskonten.Text = string.Empty;
                var myErloeskonten = from _accounts in dbContext.Accounts
                                            select new
                                            {
                                                AccountId = _accounts.AccountId,
                                                CustomerId = _accounts.CustomerId,
                                                AccountNumber = _accounts.AccountNumber,
                                                AccountSelected = false,
                                            };
                if (!String.IsNullOrEmpty(AllCustomer.SelectedValue))
                {
                    myErloeskonten = myErloeskonten.Where(q => q.CustomerId == new Guid(AllCustomer.SelectedValue));
                }
                cmbErloeskonten.DataSource = myErloeskonten;
                cmbErloeskonten.Text = "";
                cmbErloeskonten.DataBind();
            }
        }
        protected void btnSaveProduct_Click(object sender, EventArgs e)
        {
            bool insertUpdateOk = true;
            using (TransactionScope ts = new TransactionScope())
            {
                DataClasses1DataContext dbContext = new DataClasses1DataContext(((Guid)Session["CurrentUserId"])); // hier kommt die Loggingid
                Button myButton = ((Button)sender);
                Label errorMessage = ((Label)myButton.FindControl("SchowErrorMessages"));
                errorMessage.Text = "";
                try
                {
                    TextBox productId = ((TextBox)myButton.FindControl("ProductIdCustomer"));
                    TextBox locationId = ((TextBox)myButton.FindControl("LocationIdCustomer"));
                    TextBox priceId = ((TextBox)myButton.FindControl("txbPriceId"));
                    TextBox textAmount = ((TextBox)myButton.FindControl("txbAmountCustomer"));
                    TextBox txtACharge = ((TextBox)myButton.FindControl("txbAuthorativeChargeCustomer"));
                    RadComboBox cmbErloeskonten = ((RadComboBox)myButton.FindControl("cmbErloeskonten"));
                    Guid? erloesKonto = null;
                    if (EmptyStringIfNull.IsGuid(cmbErloeskonten.SelectedValue))
                    {
                        erloesKonto = new Guid(cmbErloeskonten.SelectedValue);
                    }
                    RadComboBox cmbSelectedProduct = ((RadComboBox)myButton.FindControl("cmdProductNames"));
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
                        var myPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == new Guid(productId.Text) &&
                            q.Id == new Guid(priceId.Text));
                        if (myPrice != null)
                        {
                            myPrice.LogDBContext = dbContext;
                            myPrice.Amount = price;
                            myPrice.AuthorativeCharge = autCharge;
                            PriceAccountHelper.CreateAccount(erloesKonto, myPrice, dbContext, true);
                        }
                        dbContext.SubmitChanges();
                        Session["selectedProductId"] = "";
                        //getCustomerPrice.EditIndexes.Clear();
                        //getCustomerPrice.MasterTableView.IsItemInserted = false;
                        //getCustomerPrice.MasterTableView.Rebind();
                    }
                    else if ((bool)Session["InsertCustomerProduktEdit"] == true)
                    {
                        if (cmbSelectedProduct.SelectedValue != "")
                        {
                            var newPrice = Price.CreatePrice(price, autCharge, new Guid(cmbSelectedProduct.SelectedValue), new Guid(cmbLocations.SelectedValue), null, dbContext);
                            PriceAccountHelper.CreateAccount(erloesKonto, newPrice, dbContext, true);
                            dbContext.SubmitChanges();
                            Session["selectedProductId"] = "";
                            //getCustomerPrice.EditIndexes.Clear();
                            //getCustomerPrice.MasterTableView.IsItemInserted = false;
                            //getCustomerPrice.MasterTableView.Rebind();
                        }
                        else
                        {
                            throw new Exception("Bitte wählen Sie ein Produkt aus");
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
                    dbContext = new DataClasses1DataContext(((Guid)Session["CurrentUserId"]));
                    dbContext.WriteLogItem("btnSaveProduct_Click Error " + ex.Message, LogTypes.ERROR, "Product");
                    dbContext.SubmitChanges();
                }
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
            Session["selectedProductId"] = "";
            getCustomerPrice.EditIndexes.Clear();
            getCustomerPrice.MasterTableView.IsItemInserted = false;
            getCustomerPrice.MasterTableView.Rebind();
        }
        protected void getAllCustomerProducts_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem &&  e.Item.IsInEditMode)
            {
                GridEditFormItem editForm = e.Item as GridEditFormItem;
                TextBox productName = editForm.FindControl("ProductNameBoxCustomer") as TextBox;
                productName.Visible = true;
                TextBox productNumber = editForm.FindControl("ProductNumberBoxCustomer") as TextBox;
                productNumber.Enabled = true;              
                RadComboBox productNames = editForm.FindControl("cmdProductNames") as RadComboBox;
                productNames.Visible = false;               
            }
            if (e.Item is GridEditFormInsertItem && e.Item.OwnerTableView.IsItemInserted)
            {
                GridEditFormItem editForm = e.Item as GridEditFormItem;
                TextBox productName = editForm.FindControl("ProductNameBoxCustomer") as TextBox;
                productName.Visible = false;
                TextBox productNumber = editForm.FindControl("ProductNumberBoxCustomer") as TextBox;
                productNumber.Enabled = false;
                productNumber.Text = "Automatisch";
                RadComboBox productNames = editForm.FindControl("cmdProductNames") as RadComboBox;
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
            using (TransactionScope ts = new TransactionScope())
            {
                DataClasses1DataContext dbContext = new DataClasses1DataContext(((Guid)Session["CurrentUserId"]));
                

                    try
                    {
                        RadButton rbtSender = ((RadButton)sender);
                        Label lblPriceId = rbtSender.Parent.FindControl("lblPriceId") as Label;
                        if (EmptyStringIfNull.IsGuid(lblPriceId.Text))
                        {
                            var selectedPrice = dbContext.Price.FirstOrDefault(q => q.Id == new Guid(lblPriceId.Text));
                            if (selectedPrice != null)
                            {
                                KVSCommon.Database.Price.RemovePrice(new KVSCommon.Database.Price[] { selectedPrice }, dbContext);
                                dbContext.SubmitChanges();
                            }
                            else
                            {
                                throw new Exception("Achtung, Fehler beim löschen (Preis wurde nicht gefunden). Bitte aktualisieren Sie die Webseite oder kontaktieren Sie den Administrator");
                            }
                        }
                        ts.Complete();
                    }
                    catch (Exception ex)
                    {

                        if (ts != null)
                            ts.Dispose();
                        RadWindowManagerCustomerPrice.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                        try
                        {
                            dbContext = new DataClasses1DataContext(((Guid)Session["CurrentUserId"]));
                            dbContext.WriteLogItem("RemovePrice_Click Error " + ex.Message, LogTypes.ERROR, "Price");
                            dbContext.SubmitChanges();
                        }
                        catch { }
                    }
                
            }
            getCustomerPrice.EditIndexes.Clear();
            getCustomerPrice.MasterTableView.IsItemInserted = false;
            getCustomerPrice.MasterTableView.Rebind();
        }
    }
}