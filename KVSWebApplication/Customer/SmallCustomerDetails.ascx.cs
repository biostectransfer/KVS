using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using System.Collections;
using Telerik.Web.UI;
using System.Transactions;
namespace KVSWebApplication.Customer
{
    /// <summary>
    /// Codebehind fuer die Laufkundenverwaltung
    /// </summary>
    public partial class SmallCustomerDetails : System.Web.UI.UserControl
    {
        List<string> thisUserPermissions = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(Int32.Parse(Session["CurrentUserId"].ToString())));
            if (!thisUserPermissions.Contains("KUNDEN_BEARBEITEN"))
            {
                getSmallCustomer.Columns[0].Visible = false;
                getSmallCustomer.MasterTableView.DetailTables[0].Columns[0].Visible = false;
            }
            if (!thisUserPermissions.Contains("LOESCHEN"))
            {
                getSmallCustomer.Columns[1].Visible = false;
            }
        }
        protected void RemoveSmallCustomer_Click(object sender, EventArgs e)
        {
            bool insertUpdateOk = true;

            using (TransactionScope ts = new TransactionScope())
            {
                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));

                try
                {
                    RadButton rbtSender = ((RadButton)sender);
                    Label lbl = rbtSender.Parent.FindControl("lblCustomerId") as Label;
                    if (!String.IsNullOrEmpty(lbl.Text))
                        SmallCustomer.RemoveSmallCutomer(dbContext, Int32.Parse(lbl.Text));

                    dbContext.SubmitChanges();
                    ts.Complete();

                    RadWindowManager1.RadAlert("Kunde wurde erfolgreich gelöscht", 380, 180, "Info", "");

                }
                catch (Exception ex)
                {
                    insertUpdateOk = false;
                    if (ts != null)
                        ts.Dispose();
                    RadWindowManager1.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                    try
                    {
                        dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                        dbContext.WriteLogItem("RemoveSmallCustomer_Click Error " + ex.Message, LogTypes.ERROR, "Customer");
                        dbContext.SubmitChanges();
                    }
                    catch { }


                }
            }
            if (insertUpdateOk)
            {
                getSmallCustomer.EditIndexes.Clear();
                getSmallCustomer.MasterTableView.IsItemInserted = false;
                getSmallCustomer.MasterTableView.Rebind();
            }
        }
        /// <summary>
        /// Gibt die Mastergrid zurück (Informationen zur allen Kunden)
        /// </summary>
        ///// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GetSmallCustomerDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from cust in dbContext.Customer
                        join cost in dbContext.SmallCustomer on cust.Id equals cost.CustomerId
                        orderby cust.Name
                        select new
                        {
                            TableId = "Outer",
                            cust.Id,
                            cust.Name,
                            cust.CustomerNumber,
                            cust.ContactId,
                            cust.AdressId,
                            cust.Adress.Street,
                            cust.Adress.StreetNumber,
                            cust.Adress.Zipcode,
                            cust.Adress.City,
                            cust.Adress.Country,
                            cust.Contact.Phone,
                            cust.Contact.Fax,
                            cust.Contact.MobilePhone,
                            cust.Contact.Email,
                            SameAsAdress = cust.AdressId == cust.InvoiceAdressId ? "true" : "false",
                            SameAsInvoice = cust.InvoiceAdressId == cust.InvoiceDispatchAdressId ? "true" : "false",
                            Zahlungsziel = cust.TermOfCredit,
                            Vat = EmptyStringIfNull.ReturnEmptyStringIfNull(cust.VAT),
                            InvoiceStreet = cust.InvoiceAdress.Street,
                            InvoiceStreeetNumber = cust.InvoiceAdress.StreetNumber,
                            InvoiceZipCode = cust.InvoiceAdress.Zipcode,
                            InvoiceCity = cust.InvoiceAdress.City,
                            InvoiceCountry = cust.InvoiceAdress.Country,
                            SendStreet = cust.InvoiceDispatchAdress.Street,
                            SendStreeetNumber = cust.InvoiceDispatchAdress.StreetNumber,
                            SendZipCode = cust.InvoiceDispatchAdress.Zipcode,
                            SendCity = cust.InvoiceDispatchAdress.City,
                            SendCountry = cust.InvoiceDispatchAdress.Country
                        };
            e.Result = query;
        }
        protected void ZipCodes_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            RadComboBox cmbZipCode = ((RadComboBox)sender);
            cmbZipCode.Items.Clear();
            var myzipCodes = from zipCodes in dbContext.Adress
                             group zipCodes by zipCodes.Zipcode into z
                             select new { z.Key };
            foreach (var myItem in myzipCodes)
            {
                cmbZipCode.Items.Add(new RadComboBoxItem(myItem.Key));
            }
        }
        protected void City_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            RadComboBox cmbCity = ((RadComboBox)sender);
            cmbCity.Items.Clear();
            var myCitys = from citys in dbContext.Adress
                          group citys by citys.City into c
                          select new { c.Key };
            foreach (var myItem in myCitys)
            {
                cmbCity.Items.Add(new RadComboBoxItem(myItem.Key));
            }
        }
        protected void Country_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            RadComboBox cmbCountry = ((RadComboBox)sender);
            cmbCountry.Items.Clear();
            var myCountrys = from countrys in dbContext.Adress
                             group countrys by countrys.Country into count
                             select new { count.Key };
            foreach (var myItem in myCountrys)
            {
                cmbCountry.Items.Add(new RadComboBoxItem(myItem.Key));
            }
        }
        protected void GetSmallCustomerBankData_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from bankData in dbContext.SmallCustomer
                        select new
                        {
                            TableId = "Inner",
                            bankData.CustomerId,
                            bankData.BankAccount.BankName,
                            bankData.BankAccount.Accountnumber,
                            bankData.BankAccount.BankCode,
                            bankData.BankAccount.IBAN,
                            bankData.BankAccount.BIC
                        };
            e.Result = query;
        }
        protected void bSaveAdressData_Click(object sender, EventArgs e)
        {
            bool insertUpdateOk = true;
            using (TransactionScope ts = new TransactionScope())
            {
                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())); // hier kommt die Loggingid
                try
                {
                    RadButton myButton = ((RadButton)sender);
                    Label customerId = ((Label)myButton.Parent.FindControl("lblId"));
                    CheckBox SameAsAdress = ((CheckBox)myButton.Parent.FindControl("chbInvoiceSameAsAdressLC"));
                    CheckBox SameAsInvoice = ((CheckBox)myButton.Parent.FindControl("chbSameASInvoiceAdress"));
                    RadTextBox smallEditCustomerInvoiceStreet = ((RadTextBox)myButton.Parent.FindControl("txbSmallEditCustomerInvoiceStreet"));
                    RadTextBox smallCustomerInvoiceStreetNr = ((RadTextBox)myButton.Parent.FindControl("txbSmallCustomerInvoiceStreetNr"));
                    RadTextBox smallEditCustomerSendStreet = ((RadTextBox)myButton.Parent.FindControl("txbSmallEditCustomerSendStreet"));
                    RadTextBox txbSmallEditCustomerSendStreetNr = ((RadTextBox)myButton.Parent.FindControl("txbSmallEditCustomerSendStreetNr"));
                    RadComboBox smallCustomerEditInvoiceZipCode = ((RadComboBox)myButton.Parent.FindControl("cmbSmallCustomerEditInvoiceZipCode"));
                    RadComboBox smallCustomerEditSendZipCode = ((RadComboBox)myButton.Parent.FindControl("cmbSmallCustomerEditSendZipCode"));
                    RadComboBox smallCustomerEditInvoiceCity = ((RadComboBox)myButton.Parent.FindControl("cmbSmallCustomerEditInvoiceCity"));
                    RadComboBox smallCustomerEditSendCity = ((RadComboBox)myButton.Parent.FindControl("SmallCustomerEditSendCity"));
                    RadComboBox smallCustomerEditInvoiceCountry = ((RadComboBox)myButton.Parent.FindControl("cmbSmallCustomerEditInvoiceCountry"));
                    RadComboBox smallCustomerSendCountry = ((RadComboBox)myButton.Parent.FindControl("cmbSmallCustomerSendCountry"));
                    var selectedCustomer = dbContext.Customer.SingleOrDefault(q => q.Id == Int32.Parse(customerId.Text));
                    selectedCustomer._dbContext = dbContext;
                    if (selectedCustomer != null)
                    {
                        if (SameAsAdress.Checked == true && selectedCustomer.InvoiceAdressId != selectedCustomer.AdressId)
                        {
                            Adress.DeleteAdress(selectedCustomer.InvoiceAdressId, dbContext);
                            selectedCustomer.InvoiceAdress = selectedCustomer.Adress;
                        }
                        else if (SameAsAdress.Checked == false)
                        {
                            if (selectedCustomer.AdressId == selectedCustomer.InvoiceAdressId)
                            {
                                var newAdress = Adress.CreateAdress(smallEditCustomerInvoiceStreet.Text, smallCustomerInvoiceStreetNr.Text, smallCustomerEditInvoiceZipCode.Text, smallCustomerEditInvoiceCity.Text, smallCustomerEditInvoiceCountry.Text, dbContext);
                                selectedCustomer.InvoiceAdress = newAdress;
                            }
                            else
                            {
                                selectedCustomer.InvoiceAdress.LogDBContext = dbContext;
                                selectedCustomer.InvoiceAdress.Street = smallEditCustomerInvoiceStreet.Text;
                                selectedCustomer.InvoiceAdress.StreetNumber = smallCustomerInvoiceStreetNr.Text;
                                selectedCustomer.InvoiceAdress.Zipcode = smallCustomerEditInvoiceZipCode.Text;
                                selectedCustomer.InvoiceAdress.City = smallCustomerEditInvoiceCity.Text;
                                selectedCustomer.InvoiceAdress.Country = smallCustomerEditInvoiceCountry.Text;
                            }
                        }
                        if (SameAsInvoice.Checked == true && selectedCustomer.InvoiceDispatchAdressId != selectedCustomer.InvoiceAdressId)
                        {
                            if (selectedCustomer.AdressId != selectedCustomer.InvoiceDispatchAdressId)
                                Adress.DeleteAdress(selectedCustomer.InvoiceDispatchAdressId, dbContext);
                            selectedCustomer.InvoiceDispatchAdress = selectedCustomer.InvoiceAdress;
                        }
                        else if (SameAsInvoice.Checked == false)
                        {
                            if (selectedCustomer.InvoiceAdressId == selectedCustomer.InvoiceDispatchAdressId)
                            {
                                var newAdress = Adress.CreateAdress(smallEditCustomerSendStreet.Text, txbSmallEditCustomerSendStreetNr.Text, smallCustomerEditSendZipCode.Text, smallCustomerEditSendCity.Text, smallCustomerSendCountry.Text, dbContext);
                                selectedCustomer.InvoiceDispatchAdress = newAdress;
                            }
                            else
                            {
                                selectedCustomer.InvoiceDispatchAdress.LogDBContext = dbContext;
                                selectedCustomer.InvoiceDispatchAdress.Street = smallEditCustomerSendStreet.Text;
                                selectedCustomer.InvoiceDispatchAdress.StreetNumber = txbSmallEditCustomerSendStreetNr.Text;
                                selectedCustomer.InvoiceDispatchAdress.Zipcode = smallCustomerEditSendZipCode.Text;
                                selectedCustomer.InvoiceDispatchAdress.City = smallCustomerEditSendCity.Text;
                                selectedCustomer.InvoiceDispatchAdress.Country = smallCustomerSendCountry.Text;
                            }
                        }
                        dbContext.SubmitChanges();
                    }
                    ts.Complete();

                }
                catch (Exception ex)
                {
                    insertUpdateOk = false;
                    if (ts != null)
                        ts.Dispose();
                    RadWindowManager1.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                    try
                    {
                        dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                        dbContext.WriteLogItem("Add/Edit SmallCustomer InvoiceAdress Error:  " + ex.Message, LogTypes.ERROR, "Customer");
                        dbContext.SubmitChanges();
                    }
                    catch { }
                }
            }
            if (insertUpdateOk)
            {
                getSmallCustomer.EditIndexes.Clear();
                getSmallCustomer.MasterTableView.IsItemInserted = false;
                getSmallCustomer.MasterTableView.Rebind();
            }
        }
        /// <summary>
        /// Speichert die Änderung der Kundendaten  in die Datenbank
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void getSmallCustomer_EditCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            bool insertUpdateOk = true;
            using (TransactionScope ts = new TransactionScope())
            {
                Hashtable newValues = new Hashtable();
                ((GridEditableItem)e.Item).ExtractValues(newValues);
                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())); // hier kommt die Loggingid
                try
                {
                    if (newValues["TableId"].ToString() == "Inner")
                    {
                        var customerBankUpdate = dbContext.SmallCustomer.SingleOrDefault(q => q.CustomerId == Int32.Parse(newValues["CustomerId"].ToString()));
                        if (customerBankUpdate != null && customerBankUpdate.BankAccountId != null)
                        {
                            customerBankUpdate.BankAccount.LogDBContext = dbContext;
                            customerBankUpdate.BankAccount.BankName = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["BankName"]);
                            customerBankUpdate.BankAccount.Accountnumber = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Accountnumber"]);
                            customerBankUpdate.BankAccount.BankCode = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["BankCode"]);
                            customerBankUpdate.BankAccount.IBAN = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["IBAN"]);
                            customerBankUpdate.BankAccount.BIC = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["BIC"]);
                            dbContext.SubmitChanges();
                            e.Canceled = true;
                        }
                        else if (customerBankUpdate != null && customerBankUpdate.BankAccountId == null)
                        {
                            if (EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["BankName"]) != string.Empty || EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Accountnumber"]) != string.Empty ||
                                EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["BankCode"]) != string.Empty || EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["IBAN"]) != string.Empty)
                            {
                                var addBank = BankAccount.CreateBankAccount(dbContext, newValues["BankName"].ToString(), newValues["Accountnumber"].ToString(), newValues["BankCode"].ToString(), newValues["IBAN"].ToString(), newValues["BIC"].ToString());
                                customerBankUpdate.BankAccountId = addBank.Id;
                                dbContext.SubmitChanges();
                                e.Canceled = true;
                            }
                        }
                    }
                    else if (newValues["TableId"].ToString() == "Outer")
                    {
                        var customerUpdate = dbContext.Customer.SingleOrDefault(q => q.Id == Int32.Parse(newValues["Id"].ToString()));
                        customerUpdate._dbContext = dbContext;
                        customerUpdate.LogDBContext = dbContext;
                        if (newValues["Name"] == null)
                            throw new Exception("Das Feld Kundenname ist ein Pflichtfeld und darf nicht leer sein");
                        if (newValues["Vat"] == null)
                            throw new Exception("Das Feld MwSt darf nicht leer sein");
                        if (newValues["CustomerNumber"] == null)
                            throw new Exception("Die Kundennummer darf nicht leer sein");
                        customerUpdate.Name = newValues["Name"].ToString();
                        customerUpdate.Adress.LogDBContext = dbContext;
                        customerUpdate.Adress.Street = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Street"]);
                        customerUpdate.Adress.StreetNumber = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["StreetNumber"]);
                        customerUpdate.Adress.Zipcode = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Zipcode"]);
                        customerUpdate.Adress.City = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["City"]);
                        customerUpdate.Adress.Country = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Country"]);
                        customerUpdate.Contact.LogDBContext = dbContext;
                        customerUpdate.Contact.Phone = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Phone"]);
                        customerUpdate.Contact.Fax = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Fax"]);
                        customerUpdate.Contact.MobilePhone = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["MobilePhone"]);
                        customerUpdate.Contact.Email = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Email"]);
                        customerUpdate.VAT = EmptyStringIfNull.ReturnZeroDecimalIfNullEditVat(newValues["Vat"]);
                        customerUpdate.TermOfCredit = EmptyStringIfNull.ReturnNullableInteger(newValues["Zahlungsziel"]);
                        customerUpdate.CustomerNumber = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["CustomerNumber"]);
                        dbContext.SubmitChanges();
                    }
                    ts.Complete();

                }
                catch (Exception ex)
                {
                    insertUpdateOk = false;
                    if (ts != null)
                        ts.Dispose();
                    RadWindowManager1.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                    try
                    {
                        dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                        dbContext.WriteLogItem("SmallCustomerDetails Error " + ex.Message, LogTypes.ERROR, "Customer");
                        dbContext.SubmitChanges();
                    }
                    catch { }
                }
            }
            if (insertUpdateOk)
            {
                getSmallCustomer.EditIndexes.Clear();
                getSmallCustomer.MasterTableView.IsItemInserted = false;
                getSmallCustomer.MasterTableView.Rebind();
            }
        }
        protected void SmallCustomer_Init(object sender, System.EventArgs e)
        {
            GridFilterMenu menu = getSmallCustomer.FilterMenu;
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
        protected void chbInvoiceSameAsAdress_Checked(object sender, EventArgs e)
        {
            CheckBox myCheckbox = ((CheckBox)sender);
            if (myCheckbox != null)
            {
                RadTextBox smallEditCustomerInvoiceStreet = ((RadTextBox)myCheckbox.Parent.FindControl("txbSmallEditCustomerInvoiceStreet"));
                RadTextBox smallCustomerInvoiceStreetNr = ((RadTextBox)myCheckbox.Parent.FindControl("txbSmallCustomerInvoiceStreetNr"));
                RadComboBox smallCustomerEditInvoiceZipCode = ((RadComboBox)myCheckbox.Parent.FindControl("cmbSmallCustomerEditInvoiceZipCode"));
                RadComboBox smallCustomerEditInvoiceCity = ((RadComboBox)myCheckbox.Parent.FindControl("cmbSmallCustomerEditInvoiceCity"));
                RadComboBox smallCustomerEditInvoiceCountry = ((RadComboBox)myCheckbox.Parent.FindControl("cmbSmallCustomerEditInvoiceCountry"));
                Control[] selCon = { smallEditCustomerInvoiceStreet, smallCustomerInvoiceStreetNr, smallCustomerEditInvoiceZipCode, smallCustomerEditInvoiceCity, smallCustomerEditInvoiceCountry };
                if (myCheckbox.Checked == true)
                {
                    SetControlsEnabled(selCon, false);
                }
                if (myCheckbox.Checked == false)
                {
                    SetControlsEnabled(selCon, true);
                }
            }
        }
        protected void SetControlsEnabled(Control[] controls, bool state)
        {
            foreach (Control con in controls)
            {
                if (con is RadTextBox)
                {
                    ((RadTextBox)con).Enabled = state;

                }
                if (con is RadComboBox)
                {
                    ((RadComboBox)con).Enabled = state;
                }
            }
        }
        protected void chbInvoiceSameAsInvoice_Checked(object sender, EventArgs e)
        {
            CheckBox myCheckbox = ((CheckBox)sender);
            if (myCheckbox != null)
            {
                RadTextBox smallEditCustomerSendStreet = ((RadTextBox)myCheckbox.Parent.FindControl("txbSmallEditCustomerSendStreet"));
                RadTextBox txbSmallEditCustomerSendStreetNr = ((RadTextBox)myCheckbox.Parent.FindControl("txbSmallEditCustomerSendStreetNr"));
                RadComboBox smallCustomerEditSendZipCode = ((RadComboBox)myCheckbox.Parent.FindControl("cmbSmallCustomerEditSendZipCode"));
                RadComboBox smallCustomerEditSendCity = ((RadComboBox)myCheckbox.Parent.FindControl("SmallCustomerEditSendCity"));
                RadComboBox smallCustomerSendCountry = ((RadComboBox)myCheckbox.Parent.FindControl("cmbSmallCustomerSendCountry"));
                Control[] selCon = { smallEditCustomerSendStreet, txbSmallEditCustomerSendStreetNr, smallCustomerEditSendZipCode, smallCustomerEditSendCity, smallCustomerSendCountry };
                if (myCheckbox.Checked == true)
                {
                    SetControlsEnabled(selCon, false);
                }
                if (myCheckbox.Checked == false)
                {
                    SetControlsEnabled(selCon, true);
                }
            }
        }
    }
}