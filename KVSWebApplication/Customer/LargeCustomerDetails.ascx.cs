using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.Data;
using System.Collections;
using System.Transactions;
namespace KVSWebApplication.Customer
{
    /// <summary>
    /// Codebehind fuer die Grosskundenverwaltung
    /// </summary>
    public partial class LargeCustomerDetails : System.Web.UI.UserControl
    {
        List<string> thisUserPermissions = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(Int32.Parse(Session["CurrentUserId"].ToString())));
            if (!thisUserPermissions.Contains("KUNDEN_BEARBEITEN"))
            {
                getAllCustomer.Columns[0].Visible = false;
                getAllCustomer.MasterTableView.DetailTables[0].Columns[0].Visible = false;
            }
            if (!thisUserPermissions.Contains("LOESCHEN"))
            {
                getAllCustomer.Columns[1].Visible = false;
            }
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
        protected void GetAllInvoiceTypes_Selecting(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            RadComboBox rcbInvoiceType = ((RadComboBox)sender);
            rcbInvoiceType.Items.Clear();
            var types = from tp in dbContext.InvoiceTypes
                        select new { Key = tp.ID, Value = tp.InvoiceTypeName };
            rcbInvoiceType.DataSource = types;
            rcbInvoiceType.DataBind();
        }
        /// <summary>
        /// Gibt die Mastergrid zurück (Informationen zur allen Kunden)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GetAllCustomerDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from cust in dbContext.Customer
                        join cost in dbContext.LargeCustomer on cust.Id equals cost.CustomerId
                        orderby cust.Name
                        select new
                        {
                            TableId = "Outer",
                            cust.Id,
                            cust.Name,
                            cust.ContactId,
                            cust.AdressId,
                            cust.Adress.Street,
                            cust.Adress.StreetNumber,
                            cust.Adress.Zipcode,
                            cust.Adress.City,
                            cust.Adress.Country,
                            PersonId = EmptyStringIfNull.ReturnEmptyStringIfNull(cost.PersonId),
                            Show = cust.Id.ToString() == cust.LargeCustomer.CustomerId.ToString() ? "true" : "false",
                            SameAsAdress = cust.AdressId == cust.InvoiceAdressId ? "true" : "false",
                            SameAsInvoice = cust.InvoiceAdressId == cust.InvoiceDispatchAdressId ? "true" : "false",
                            Zahlungsziel = cust.TermOfCredit,
                            Vat = EmptyStringIfNull.ReturnEmptyStringIfNull(cust.VAT),
                            SendInvoiceToMainLocation = cost.SendInvoiceByEmail != null ? cost.SendInvoiceByEmail : false,
                            SendInvoiceByEmail = cost.SendInvoiceToMainLocation != null ? cost.SendInvoiceToMainLocation : false,
                            Kundennummer = cust.CustomerNumber,
                            InvoiceStreet = cust.InvoiceAdress.Street,
                            InvoiceStreeetNumber = cust.InvoiceAdress.StreetNumber,
                            InvoiceZipCode = cust.InvoiceAdress.Zipcode,
                            InvoiceCity = cust.InvoiceAdress.City,
                            InvoiceCountry = cust.InvoiceAdress.Country,
                            SendStreet = cust.InvoiceDispatchAdress.Street,
                            SendStreeetNumber = cust.InvoiceDispatchAdress.StreetNumber,
                            SendZipCode = cust.InvoiceDispatchAdress.Zipcode,
                            SendCity = cust.InvoiceDispatchAdress.City,
                            SendCountry = cust.InvoiceDispatchAdress.Country,
                            InvoiceType = cost.InvoiceTypes != null ? cost.InvoiceTypes.InvoiceTypeName : "Hinzufügen",
                            VersandadresseKunde = cust.LargeCustomer.SendOrderFinishedNoteToCustomer == null || cust.LargeCustomer.SendOrderFinishedNoteToCustomer == false ? "false" : "true",
                            VersandadresseStandort = cust.LargeCustomer.SendOrderFinishedNoteToLocation == null || cust.LargeCustomer.SendOrderFinishedNoteToLocation == false ? "false" : "true",
                            VersandadresseTimeNothing = cust.LargeCustomer.OrderFinishedNoteSendType == 0 ? "true" : "false",
                            VersandadresseTimeNow = cust.LargeCustomer.OrderFinishedNoteSendType == 1 ? "true" : "false",
                            VersandadresseTimeHourly = cust.LargeCustomer.OrderFinishedNoteSendType == 2 ? "true" : "false",
                            VersandadresseTimeDayly = cust.LargeCustomer.OrderFinishedNoteSendType == 3 ? "true" : "false",
                            LieferscheinKunde = cust.LargeCustomer.SendPackingListToCustomer == null || cust.LargeCustomer.SendPackingListToCustomer == false ? "false" : "true",
                            LieferscheinStandort = cust.LargeCustomer.SendPackingListToLocation == null || cust.LargeCustomer.SendPackingListToLocation == false ? "false" : "true",
                            MatchCode = cust.LargeCustomer.Customer.MatchCode,
                            Debitornumber = cust.LargeCustomer.Customer.Debitornumber,
                            evbnumber = EmptyStringIfNull.ReturnEmptyStringIfNull(cust.eVB_Number),
                            InternalId = cust.InternalId
                        };
            e.Result = query;
        }
        protected void cmbErloeskonten_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            RadComboBox cmbErloeskonten = ((RadComboBox)sender);
            Label lbl = cmbErloeskonten.Parent.FindControl("lblId") as Label;
            cmbErloeskonten.Items.Clear();
            cmbErloeskonten.Text = string.Empty;
            var myErloeskonten = from _accounts in dbContext.Accounts
                                 where _accounts.CustomerId == Int32.Parse(lbl.Text)
                                 group _accounts by _accounts.AccountNumber into count
                                 select new { count.Key };
            foreach (var myItem in myErloeskonten)
            {
                cmbErloeskonten.Items.Add(new RadComboBoxItem(myItem.Key));
            }
        }
        protected void RemoveKonto_Click(object sender, EventArgs e)
        {
            RadButton rbtSender = ((RadButton)sender);
            Label lbl = rbtSender.Parent.FindControl("lblId") as Label;
            RadComboBox rbt = rbtSender.Parent.FindControl("cmbErloeskonten") as RadComboBox;
            using (TransactionScope ts = new TransactionScope())
            {
                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));

                try
                {

                    Accounts.DeleteAccount(Int32.Parse(lbl.Text), rbt.Text, dbContext);

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    if (ts != null)
                        ts.Dispose();
                    RadWindowManagerLargeCustomer.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                    try
                    {
                        dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                        dbContext.WriteLogItem("RemoveKonto_Click Error " + ex.Message, LogTypes.ERROR, "Customer");
                        dbContext.SubmitChanges();
                    }
                    catch { }
                }


            }
            cmbErloeskonten_ItemsRequested(rbt, new RadComboBoxItemsRequestedEventArgs());
        }
        protected void AddInvoiceType_Click(object sender, EventArgs e)
        {
            bool insertUpdateOk = true;
            using (TransactionScope ts = new TransactionScope())
            {
                KVSEntities dbContext = new KVSEntities();

                try
                {
                    RadButton AddInvoiceType = ((RadButton)sender);
                    GridDataItem myItem = AddInvoiceType.Parent.Parent.Parent.Parent as GridDataItem;
                    RadComboBox cmbSelectedType = AddInvoiceType.Parent.FindControl("cmbInvoiceTypes") as RadComboBox;

                    if (myItem != null && myItem["Id"].Text != string.Empty && !String.IsNullOrEmpty(myItem["Id"].Text) && cmbSelectedType != null
                        && cmbSelectedType.SelectedValue != string.Empty && !String.IsNullOrEmpty(cmbSelectedType.SelectedValue))
                    {
                        var customer = dbContext.LargeCustomer.FirstOrDefault(q => q.CustomerId == Int32.Parse(myItem["Id"].Text));
                        if (customer != null)
                        {
                            customer.InvoiceTypesID = Int32.Parse(cmbSelectedType.SelectedValue);
                            dbContext.SubmitChanges();
                        }
                        ts.Complete();

                    }
                }
                catch (Exception ex)
                {
                    insertUpdateOk = false;
                    if (ts != null)
                        ts.Dispose();
                    RadWindowManagerLargeCustomer.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                    try
                    {
                        dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                        dbContext.WriteLogItem("AddInvoiceType_Click Error " + ex.Message, LogTypes.ERROR, "Customer");
                        dbContext.SubmitChanges();
                    }
                    catch { }
                }

            }
            if (insertUpdateOk)
            {
                getAllCustomer.EditIndexes.Clear();
                getAllCustomer.MasterTableView.IsItemInserted = false;
                getAllCustomer.MasterTableView.Rebind();
            }
        }
        protected void AddKonto_Click(object sender, EventArgs e)
        {
            RadButton rbtSender = ((RadButton)sender);
            Label lbl = rbtSender.Parent.FindControl("lblId") as Label;
            RadComboBox rbt = rbtSender.Parent.FindControl("cmbErloeskonten") as RadComboBox;
            using (TransactionScope ts = new TransactionScope())
            {
                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));

                try
                {

                    if (rbt.Text == string.Empty)
                    {
                        throw new Exception("Bitte tragen Sie ein gültiges Erlöskonto ein! Leer ist nicht erlaubt");
                    }
                    Accounts.CreateAccount(Int32.Parse(lbl.Text), rbt.Text, dbContext);

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    if (ts != null)
                        ts.Dispose();
                    RadWindowManagerLargeCustomer.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                    try
                    {
                        dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                        dbContext.WriteLogItem("LargeCustomerDetails Error " + ex.Message, LogTypes.ERROR, "Customer");
                        dbContext.SubmitChanges();
                    }
                    catch { }
                }



            }
            cmbErloeskonten_ItemsRequested(rbt, new RadComboBoxItemsRequestedEventArgs());
        }
        /// <summary>
        /// Gibt die Kundenkontaktdaten zurück, die beim Kunden ausgewählt wurden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GetAllCustomerContactData_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            int? myPersonId = null;
            if (e.WhereParameters["PersonId"] != null)
            {
                myPersonId = Int32.Parse(e.WhereParameters["PersonId"].ToString());
            }
            var query = from contact in dbContext.Contact
                        from person in dbContext.LargeCustomer.Where(p => p.PersonId == myPersonId
                           && p.CustomerId == Int32.Parse(
                            (e.WhereParameters["CustomerId"].ToString()))).DefaultIfEmpty()
                        select new
                        {
                            TableId = "Inner",
                            ContactId = contact.Id,
                            contact.Phone,
                            contact.Fax,
                            contact.MobilePhone,
                            contact.Email,
                            CustomerId = e.WhereParameters["CustomerId"],
                            PersonId = person.Customer.LargeCustomer.PersonId != null ? person.Customer.LargeCustomer.PersonId.ToString() : "",
                            Title = person.Customer.LargeCustomer.PersonId != null ? EmptyStringIfNull.ReturnEmptyStringIfNull(
                            person.Customer.LargeCustomer.Person.Title) : "",
                            Name = person.Customer.LargeCustomer.PersonId != null ?
                            EmptyStringIfNull.ReturnEmptyStringIfNull(person.Customer.LargeCustomer.Person.Name) : "",
                            Vorname = person.Customer.LargeCustomer.PersonId != null ?
                            EmptyStringIfNull.ReturnEmptyStringIfNull(person.Customer.LargeCustomer.Person.FirstName) : "",
                            Extension = person.Customer.LargeCustomer.PersonId != null ?
                            EmptyStringIfNull.ReturnEmptyStringIfNull(person.Customer.LargeCustomer.Person.Extension) : ""
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
                    Label LargeEditCustomerSendStreetError = ((Label)myButton.Parent.FindControl("lblLargeEditCustomerSendStreetError"));
                    Label LargeCustomerEditInvoiceZipCodeError = ((Label)myButton.Parent.FindControl("lblLargeCustomerEditInvoiceZipCodeError"));
                    Label LargeEditCustomerInvoiceStreetError = ((Label)myButton.Parent.FindControl("lblLargeEditCustomerInvoiceStreetError"));
                    Label LargeCustomerEditSendZipCodeError = ((Label)myButton.Parent.FindControl("lblLargeCustomerEditSendZipCodeError"));
                    Label LargeCustomerEditInvoiceCityError = ((Label)myButton.Parent.FindControl("lblLargeCustomerEditInvoiceCityError"));
                    Label LargeCustomerEditSendCityError = ((Label)myButton.Parent.FindControl("lblLargeCustomerEditSendCityError"));
                    Label LargeCustomerEditInvoiceCountryError = ((Label)myButton.Parent.FindControl("lblLargeCustomerEditInvoiceCountryError"));
                    Label LargeCustomerSendCountryError = ((Label)myButton.Parent.FindControl("lblLargeCustomerSendCountryError"));
                    CheckBox SameAsAdress = ((CheckBox)myButton.Parent.FindControl("chbInvoiceSameAsAdressLC"));
                    CheckBox SameAsInvoice = ((CheckBox)myButton.Parent.FindControl("chbSameASInvoiceAdress"));
                    RadTextBox LargeEditCustomerInvoiceStreet = ((RadTextBox)myButton.Parent.FindControl("txbLargeEditCustomerInvoiceStreet"));
                    RadTextBox LargeCustomerInvoiceStreetNr = ((RadTextBox)myButton.Parent.FindControl("txbLargeCustomerInvoiceStreetNr"));
                    RadTextBox LargeEditCustomerSendStreet = ((RadTextBox)myButton.Parent.FindControl("txbLargeEditCustomerSendStreet"));
                    RadTextBox txbLargeEditCustomerSendStreetNr = ((RadTextBox)myButton.Parent.FindControl("txbLargeEditCustomerSendStreetNr"));
                    RadComboBox CustomerEditInvoiceZipCode = ((RadComboBox)myButton.Parent.FindControl("cmbLargeCustomerEditInvoiceZipCode"));
                    RadComboBox CustomerEditSendZipCode = ((RadComboBox)myButton.Parent.FindControl("cmbLargeCustomerEditSendZipCode"));
                    RadComboBox LargeCustomerEditInvoiceCity = ((RadComboBox)myButton.Parent.FindControl("cmbLargeCustomerEditInvoiceCity"));
                    RadComboBox LargeCustomerEditSendCity = ((RadComboBox)myButton.Parent.FindControl("LargeCustomerEditSendCity"));
                    RadComboBox LargeCustomerEditInvoiceCountry = ((RadComboBox)myButton.Parent.FindControl("cmbLargeCustomerEditInvoiceCountry"));
                    RadComboBox LargeCustomerSendCountry = ((RadComboBox)myButton.Parent.FindControl("cmbLargeCustomerSendCountry"));
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
                                var newAdress = Adress.CreateAdress(LargeEditCustomerInvoiceStreet.Text, LargeCustomerInvoiceStreetNr.Text, CustomerEditInvoiceZipCode.Text, LargeCustomerEditInvoiceCity.Text, LargeCustomerEditInvoiceCountry.Text, dbContext);
                                selectedCustomer.InvoiceAdress = newAdress;
                            }
                            else
                            {
                                selectedCustomer.InvoiceAdress.LogDBContext = dbContext;
                                selectedCustomer.InvoiceAdress.Street = LargeEditCustomerInvoiceStreet.Text;
                                selectedCustomer.InvoiceAdress.StreetNumber = LargeCustomerInvoiceStreetNr.Text;
                                selectedCustomer.InvoiceAdress.Zipcode = CustomerEditInvoiceZipCode.Text;
                                selectedCustomer.InvoiceAdress.City = LargeCustomerEditInvoiceCity.Text;
                                selectedCustomer.InvoiceAdress.Country = LargeCustomerEditInvoiceCountry.Text;
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
                            var newAdress = Adress.CreateAdress(LargeEditCustomerSendStreet.Text, txbLargeEditCustomerSendStreetNr.Text, CustomerEditSendZipCode.Text, LargeCustomerEditSendCity.Text, LargeCustomerSendCountry.Text, dbContext);
                            selectedCustomer.InvoiceDispatchAdress = newAdress;
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
                    RadWindowManagerLargeCustomer.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                    try
                    {
                        dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                        dbContext.WriteLogItem("Add/Edit LargeCustomer InvoiceAdress Error:  " + ex.Message, LogTypes.ERROR, "Customer");
                        dbContext.SubmitChanges();
                    }
                    catch { }
                }
            }
            if (insertUpdateOk)
            {
                getAllCustomer.EditIndexes.Clear();
                getAllCustomer.MasterTableView.IsItemInserted = false;
                getAllCustomer.MasterTableView.Rebind();
            }
        }
        protected void chbInvoiceSameAsAdress_Checked(object sender, EventArgs e)
        {
            CheckBox myCheckbox = ((CheckBox)sender);
            Label customerId = ((Label)myCheckbox.Parent.FindControl("lblId"));
            RadTextBox LargeEditCustomerInvoiceStreet = ((RadTextBox)myCheckbox.Parent.FindControl("txbLargeEditCustomerInvoiceStreet"));
            RadTextBox LargeCustomerInvoiceStreetNr = ((RadTextBox)myCheckbox.Parent.FindControl("txbLargeCustomerInvoiceStreetNr"));
            RadComboBox CustomerEditInvoiceZipCode = ((RadComboBox)myCheckbox.Parent.FindControl("cmbLargeCustomerEditInvoiceZipCode"));
            RadComboBox LargeCustomerEditInvoiceCity = ((RadComboBox)myCheckbox.Parent.FindControl("cmbLargeCustomerEditInvoiceCity"));
            RadComboBox LargeCustomerEditInvoiceCountry = ((RadComboBox)myCheckbox.Parent.FindControl("cmbLargeCustomerEditInvoiceCountry"));
            Control[] selCon = { LargeEditCustomerInvoiceStreet, LargeCustomerInvoiceStreetNr, CustomerEditInvoiceZipCode, LargeCustomerEditInvoiceCity, LargeCustomerEditInvoiceCountry };
            if (myCheckbox.Checked == true)
            {
                SetControlsEnabled(selCon, false);
            }
            if (myCheckbox.Checked == false)
            {
                SetControlsEnabled(selCon, true);
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
            RadTextBox LargeEditCustomerSendStreet = ((RadTextBox)myCheckbox.Parent.FindControl("txbLargeEditCustomerSendStreet"));
            RadTextBox txbLargeEditCustomerSendStreetNr = ((RadTextBox)myCheckbox.Parent.FindControl("txbLargeEditCustomerSendStreetNr"));
            RadComboBox CustomerEditSendZipCode = ((RadComboBox)myCheckbox.Parent.FindControl("cmbLargeCustomerEditSendZipCode"));
            RadComboBox LargeCustomerEditSendCity = ((RadComboBox)myCheckbox.Parent.FindControl("LargeCustomerEditSendCity"));
            RadComboBox LargeCustomerSendCountry = ((RadComboBox)myCheckbox.Parent.FindControl("cmbLargeCustomerSendCountry"));
            Control[] selCon = { LargeEditCustomerSendStreet, txbLargeEditCustomerSendStreetNr, CustomerEditSendZipCode, LargeCustomerEditSendCity, LargeCustomerSendCountry };
            if (myCheckbox.Checked == true)
            {
                SetControlsEnabled(selCon, false);
            }
            if (myCheckbox.Checked == false)
            {
                SetControlsEnabled(selCon, true);
            }
        }
        protected void getAllCustomer_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack && getAllCustomer.MasterTableView.Items.Count > 0)
            {
                getAllCustomer.MasterTableView.Items[1].Expanded = true;
            }
        }
        /// <summary>
        /// Speichert die Änderung der Kundendaten  in die Datenbank
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void getAllCustomer_EditCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            bool insertUpdateOk = true;
            using (TransactionScope ts = new TransactionScope())
            {
                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                Hashtable newValues = new Hashtable();
                ((GridEditableItem)e.Item).ExtractValues(newValues);

                try
                {
                    if (newValues["TableId"].ToString() == "Inner")
                    {
                        var contactUpdate = dbContext.Contact.SingleOrDefault(q => q.Id == Int32.Parse(newValues["ContactId"].ToString()));
                        contactUpdate.LogDBContext = dbContext;

                        contactUpdate.MobilePhone = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["MobilePhone"]);
                        contactUpdate.Phone = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Phone"]);
                        contactUpdate.Email = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Email"]);
                        contactUpdate.Fax = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Fax"]);
                        if (newValues["PersonId"] != null)
                        {
                            var personUpdate = dbContext.Person.SingleOrDefault(q => q.Id == Int32.Parse(newValues["PersonId"].ToString()));
                            personUpdate.LogDBContext = dbContext;
                            personUpdate.Title = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Title"]);
                            personUpdate.Name = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Name"]);
                            personUpdate.FirstName = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Vorname"]);
                            personUpdate.Extension = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Extension"]);
                        }
                        else
                        {
                            if (EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Name"]) != string.Empty ||
                                EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Vorname"]) != string.Empty ||
                                EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Extension"]) != string.Empty)
                            {
                                var myTempPerson = Person.CreatePerson(dbContext, EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Vorname"]),
                                     EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Name"]), EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Title"]),
                                      EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Extension"]));
                                var myTempCustomer = dbContext.LargeCustomer.SingleOrDefault(q => q.CustomerId == Int32.Parse(newValues["CustomerId"].ToString()));
                                myTempCustomer.PersonId = myTempPerson.Id;
                            }
                        }
                        dbContext.SubmitChanges();
                    }
                    else if (newValues["TableId"].ToString() == "Outer")
                    {
                        var contactUpdate = dbContext.Customer.SingleOrDefault(q => q.Id == Int32.Parse(newValues["Id"].ToString()));
                        contactUpdate._dbContext = dbContext;
                        contactUpdate.LogDBContext = dbContext;
                        if (newValues["Name"] == null || newValues["Street"] == null || newValues["StreetNumber"] == null ||
                            newValues["Zipcode"] == null || newValues["Kundennummer"] == null)
                        {
                            throw new Exception("Die Felder, Kundenname, Strasse, Hausnummer, Kundennummer dürfen nicht leer sein");
                        }
                        contactUpdate.Name = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Name"]);
                        contactUpdate.MatchCode = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["MatchCode"]);
                        contactUpdate.Debitornumber = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Debitornumber"]);
                        contactUpdate.CustomerNumber = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Kundennummer"]);
                        contactUpdate.eVB_Number = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["evbnumber"]);
                        contactUpdate.Adress.LogDBContext = dbContext;
                        contactUpdate.Adress.Street = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Street"]);
                        contactUpdate.Adress.StreetNumber = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["StreetNumber"]);
                        contactUpdate.Adress.Zipcode = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Zipcode"]);
                        contactUpdate.Adress.City = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["City"]);
                        contactUpdate.Adress.Country = EmptyStringIfNull.ReturnEmptyStringIfNull(newValues["Country"]);
                        contactUpdate.VAT = EmptyStringIfNull.ReturnZeroDecimalIfNullEditVat(newValues["Vat"]);
                        contactUpdate.LargeCustomer.LogDBContext = dbContext;
                        contactUpdate.LargeCustomer.SendInvoiceToMainLocation = (bool)newValues["SendInvoiceToMainLocation"];
                        contactUpdate.LargeCustomer.SendInvoiceByEmail = (bool)newValues["SendInvoiceByEmail"];
                        contactUpdate.TermOfCredit = EmptyStringIfNull.ReturnNullableInteger(newValues["Zahlungsziel"]);
                        dbContext.SubmitChanges();
                    }

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    insertUpdateOk = false;
                    if (ts != null)
                        ts.Dispose();
                    try
                    {
                        dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                        dbContext.WriteLogItem("LargeCustomerDetails Error " + ex.Message, LogTypes.ERROR, "Customer");
                        dbContext.SubmitChanges();
                    }
                    catch { }

                    RadWindowManagerLargeCustomer.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                }
            }
            if (insertUpdateOk)
            {
                getAllCustomer.EditIndexes.Clear();
                getAllCustomer.MasterTableView.IsItemInserted = false;
                getAllCustomer.MasterTableView.Rebind();
            }
        }
        protected void SaveSendOrder_Click(object sender, EventArgs e)
        {
            try
            {
                Button saveButton = sender as Button;
                Label lblIdkonfig = saveButton.Parent.FindControl("lblIdkonfig") as Label;
                CheckBox chbLCustomerAuftragKunde = saveButton.Parent.FindControl("chbLCustomerAuftragKunde") as CheckBox;
                CheckBox chbLCustomerAuftragStandort = saveButton.Parent.FindControl("chbLCustomerAuftragStandort") as CheckBox;
                RadioButton LCustomerAuftragHourly = saveButton.Parent.FindControl("LCustomerAuftragHourly") as RadioButton;
                RadioButton LCustomerAuftragDaily = saveButton.Parent.FindControl("LCustomerAuftragDaily") as RadioButton;
                RadioButton LCustomerAuftragNow = saveButton.Parent.FindControl("LCustomerAuftragNow") as RadioButton;
                RadioButton LCustomerAuftragNoInfo = saveButton.Parent.FindControl("LCustomerAuftragNoInfo") as RadioButton;
                CheckBox chbLieferscheinKunde = saveButton.Parent.FindControl("chbLieferscheinKunde") as CheckBox;
                CheckBox chbLieferscheinStandort = saveButton.Parent.FindControl("chbLieferscheinStandort") as CheckBox;
                using (KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())))
                {
                    var customer = dbContext.LargeCustomer.SingleOrDefault(q => q.CustomerId == Int32.Parse(lblIdkonfig.Text));
                    if (customer != null)
                    {
                        customer.LogDBContext = dbContext;
                        if (LCustomerAuftragNow.Checked == true)
                        {
                            customer.OrderFinishedNoteSendType = 1;
                        }
                        if (LCustomerAuftragHourly.Checked == true)
                        {
                            customer.OrderFinishedNoteSendType = 2;
                        }
                        if (LCustomerAuftragDaily.Checked == true)
                        {
                            customer.OrderFinishedNoteSendType = 3;
                        }
                        if (LCustomerAuftragNoInfo.Checked == true)
                        {
                            customer.OrderFinishedNoteSendType = 0;
                        }
                        customer.SendOrderFinishedNoteToCustomer = chbLCustomerAuftragKunde.Checked;
                        customer.SendOrderFinishedNoteToLocation = chbLCustomerAuftragStandort.Checked;
                        customer.SendPackingListToCustomer = chbLieferscheinKunde.Checked;
                        customer.SendPackingListToLocation = chbLieferscheinStandort.Checked;
                        dbContext.SubmitChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                RadWindowManagerLargeCustomer.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                using (KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())))
                {

                    dbContext.WriteLogItem("LargeCustomerDetails Error " + ex.Message, LogTypes.ERROR, "Customer");
                    dbContext.SubmitChanges();
                }
            }

        }
        protected void LargeCustomer_Init(object sender, System.EventArgs e)
        {
            GridFilterMenu menu = getAllCustomer.FilterMenu;
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
        protected void RemoveLargeCustomer_Click(object sender, EventArgs e)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));


                try
                {
                    RadButton rbtSender = ((RadButton)sender);
                    Label lbl = rbtSender.Parent.FindControl("lblLargeCustomerId") as Label;
                    if (!String.IsNullOrEmpty(lbl.Text))
                        LargeCustomer.RemoveLargeCutomer(dbContext, Int32.Parse(lbl.Text));

                    dbContext.SubmitChanges();
                    ts.Complete();
                    RadWindowManagerLargeCustomer.RadAlert("Kunde wurde erfolgreich gelöscht", 380, 180, "Info", "");


                }
                catch (Exception ex)
                {

                    if (ts != null)
                        ts.Dispose();
                    RadWindowManagerLargeCustomer.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                    try
                    {
                        dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                        dbContext.WriteLogItem("RemoveLargeCustomer_Click Error " + ex.Message, LogTypes.ERROR, "Customer");
                        dbContext.SubmitChanges();
                    }
                    catch { }

                }



            }
            getAllCustomer.EditIndexes.Clear();
            getAllCustomer.MasterTableView.IsItemInserted = false;
            getAllCustomer.MasterTableView.Rebind();
        }
    }
}