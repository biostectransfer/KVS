using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.Transactions;
namespace KVSWebApplication.Customer
{
    /// <summary>
    /// Codebehind fuer die Standortverwaltung
    /// </summary>
    public partial class Location_Details : System.Web.UI.UserControl
    {
        List<string> thisUserPermissions = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            thisUserPermissions.AddRange(KVSCommon.Database.User.GetAllPermissionsByID(Int32.Parse(Session["CurrentUserId"].ToString())));
            if (!thisUserPermissions.Contains("STANDORT_ANLAGE"))
            {
                rbtCreateLocation.Enabled = false;
            }
            if (!thisUserPermissions.Contains("STANDORT_BEARBEITEN"))
            {
                getAllCustomerLocations.MasterTableView.DetailTables[0].Columns[0].Visible = false;
            }
            if (!thisUserPermissions.Contains("STANDORT_UEBERSICHT"))
            {
                getAllCustomerLocations.MasterTableView.DetailTables[0].Columns[0].Visible = false;
                rbtCreateLocation.Enabled = false;
            }
            if (!thisUserPermissions.Contains("LOESCHEN"))
            {
                getAllCustomerLocations.MasterTableView.DetailTables[0].Columns[getAllCustomerLocations.MasterTableView.DetailTables[0].Columns.Count - 1].Visible = false;
            }
            setDefaultValues();
        }
        protected void setDefaultValues()
        {
            chbLocationRechnungsadresse_Checked(this, new EventArgs());
            chbLocationVersandadresse_Checked(this, new EventArgs());
        }
        protected void chbLocationRechnungsadresse_Checked(object sender, EventArgs e)
        {
            if (chbLocationRechnungsaderesse.Checked == true)
            {
                txbLocationInvoiceAdressStreet.Enabled = false;
                txbLocationInvoiceAdressStreetNr.Enabled = false;
                cmbLocationCityInvoice.Enabled = false;
                cmbLocationInvoiceZip.Enabled = false;
                cmbLocationInvoiceCountry.Enabled = false;
            }
            else
            {
                txbLocationInvoiceAdressStreet.Enabled = true;
                txbLocationInvoiceAdressStreetNr.Enabled = true;
                cmbLocationCityInvoice.Enabled = true;
                cmbLocationInvoiceZip.Enabled = true;
                cmbLocationInvoiceCountry.Enabled = true;
            }
        }
        protected void chbLocationVersandadresse_Checked(object sender, EventArgs e)
        {
            if (chbLocationVersandadresse.Checked == true)
            {
                txbLocationSendAdressStreet.Enabled = false;
                txbLocationSendAdressStreetNr.Enabled = false;
                cmbLocationCitySend.Enabled = false;
                cmbLocationSendZip.Enabled = false;
                cmbLocationSendCountry.Enabled = false;
            }
            else
            {
                txbLocationSendAdressStreet.Enabled = true;
                txbLocationSendAdressStreetNr.Enabled = true;
                cmbLocationCitySend.Enabled = true;
                cmbLocationSendZip.Enabled = true;
                cmbLocationSendCountry.Enabled = true;
            }
        }
        protected void ZipCodes_ItemsRequested(object sender, EventArgs e)
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
        protected void Citys_ItemsRequested(object sender, EventArgs e)
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
        protected void Countrys_ItemsRequested(object sender, EventArgs e)
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
        protected void SuperLocation_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            RadComboBox cmbSuperLocation = ((RadComboBox)sender);
            TextBox txbCustomerId = ((RadComboBox)sender).Parent.FindControl("txbCustomerId") as TextBox;
            KVSEntities dbContext = new KVSEntities();
            var mySuperLocations = from _locations in dbContext.Location
                                   where _locations.CustomerId == Int32.Parse(txbCustomerId.Text)
                                   select new { _locations.Name, _locations.Id };
            foreach (var mySuperLocation in mySuperLocations)
            {
                cmbSuperLocation.Items.Add(new RadComboBoxItem(mySuperLocation.Name, mySuperLocation.Id.ToString()));
            }
        }
        protected void ResetErrorLabels()
        {
            lblCustomerNameLocationError.Text = "";
            lblLocationNameError.Text = "";
            lblLocationStreetNrError.Text = "";
            lblLocationFaxError.Text = "";
            lblLocationZipCodeError.Text = "";
            lblLocationInvoiceAdressError.Text = "";
            lblLocationSendAdressError.Text = "";
            lblLocationPhoneError.Text = "";
            lblLocationCityError.Text = "";
            lblLocationInvoiceZipError.Text = "";
            lblLocationSendZipInfoError.Text = "";
            lblLocationCountryError.Text = "";
            lblLocationCityInvoiceInfoError.Text = "";
            lblLocationCitySendInfoError.Text = "";
            lblLocationMobilePhoneNummerError.Text = "";
            lblOverLocationError.Text = "";
            lblLocationInvoiceCountryInfoError.Text = "";
            lblLocationSendCountryInfoError.Text = "";
            lblLocationEmailError.Text = "";
            lblVatError.Text = "";
            resultMessage.Text = "";
        }
        protected bool CheckFields()
        {
            bool check = true;
            if (txbLocationName.Text == string.Empty)
            {
                lblLocationNameError.Text = "Das Feld Standort Name darf nicht leer sein";
                check = false;
            }
            if (CustomerLocation.Text == string.Empty)
            {
                lblCustomerNameLocationError.Text = "Es wurde kein Kunde ausgewält";
                check = false;
            }
            if (txbLocationStreet.Text == string.Empty || txbLocationNr.Text == string.Empty)
            {
                lblLocationStreetNrError.Text = "Bitte Strasse/Nr. eintragen";
                check = false;
            }
            if (txbLocationZipCode.Text == string.Empty)
            {
                lblLocationZipCodeError.Text = "Bitte die PLZ eintragen";
                check = false;
            }
            if (cmbLocationCity.Text == string.Empty)
            {
                lblLocationCityError.Text = "Bitte die Stadt eintragen";
                check = false;
            }
            if (cmbLocationCountry.Text == string.Empty)
            {
                lblLocationCountryError.Text = "Bitte das Land eintragen";
                check = false;
            }
            if (chbLocationRechnungsaderesse.Checked == false)
            {
                if (txbLocationInvoiceAdressStreet.Text == string.Empty || txbLocationInvoiceAdressStreetNr.Text == string.Empty)
                {
                    lblLocationInvoiceAdressInfo.Text = "Bitte Strasse/Nr. eintragen";
                    check = false;
                }
                if (cmbLocationCityInvoice.Text == string.Empty)
                {
                    lblLocationCityInvoiceInfoError.Text = "Bitte die Stadt eintragen";
                    check = false;
                }
                if (cmbLocationInvoiceZip.Text == string.Empty)
                {
                    lblLocationInvoiceZipError.Text = "Bitte die PLZ eintragen";
                    check = false;
                }
                if (cmbLocationInvoiceCountry.Text == string.Empty)
                {
                    lblLocationInvoiceCountryInfoError.Text = "Bitte das Land eintragen";
                    check = false;
                }
            }
            if (chbLocationVersandadresse.Checked == false)
            {
                if (txbLocationSendAdressStreet.Text == string.Empty || txbLocationSendAdressStreetNr.Text == string.Empty)
                {
                    lblLocationSendAdressError.Text = "Bitte Strasse/Nr. eintragen";
                    check = false;
                }
                if (cmbLocationCitySend.Text == string.Empty)
                {
                    lblLocationCitySendInfoError.Text = "Bitte die Stadt eintragen";
                    check = false;
                }
                if (cmbLocationSendZip.Text == string.Empty)
                {
                    lblLocationSendZipInfoError.Text = "Bitte die PLZ eintragen";
                    check = false;
                }
                if (cmbLocationSendCountry.Text == string.Empty)
                {
                    lblLocationSendCountryInfoError.Text = "Bitte das Land eintragen";
                    check = false;
                }
            }
            return check;
        }
        protected void SaveLocationAdress(object sender, EventArgs e)
        {

            using (TransactionScope ts = new TransactionScope())
            {
                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())); // hier kommt die Loggingid
                try
                {
                    RadButton myButton = ((RadButton)sender);
                    Label locationId = ((Label)myButton.Parent.FindControl("txbLocationInvoiceId"));
                    RadTextBox LocationEditCustomerInvoiceStreet = ((RadTextBox)myButton.Parent.FindControl("txbLocationEditCustomerInvoiceStreet"));
                    RadTextBox LocationInvoiceStreetNr = ((RadTextBox)myButton.Parent.FindControl("txbLocationInvoiceStreetNr"));
                    RadTextBox LocationEditCustomerSendStreet = ((RadTextBox)myButton.Parent.FindControl("txbLocationEditCustomerSendStreet"));
                    RadTextBox LocationEditCustomerSendStreetNr = ((RadTextBox)myButton.Parent.FindControl("txbLocationEditCustomerSendStreetNr"));
                    RadComboBox LocationEditInvoiceZipCode = ((RadComboBox)myButton.Parent.FindControl("cmbLocationEditInvoiceZipCode"));
                    RadComboBox LocationEditSendZipCode = ((RadComboBox)myButton.Parent.FindControl("cmbLocationEditSendZipCode"));
                    RadComboBox LocationEditInvoiceCity = ((RadComboBox)myButton.Parent.FindControl("cmbLocationEditInvoiceCity"));
                    RadComboBox LocationEditSendCity = ((RadComboBox)myButton.Parent.FindControl("LocationEditSendCity"));
                    RadComboBox LocationEditInvoiceCountry = ((RadComboBox)myButton.Parent.FindControl("cmbLocationEditInvoiceCountry"));
                    RadComboBox LocationSendCountry = ((RadComboBox)myButton.Parent.FindControl("cmbLocationSendCountry"));
                    var selectedLocation = dbContext.Location.SingleOrDefault(q => q.Id == Int32.Parse(locationId.Text));
                    selectedLocation._dbContext = dbContext;
                    if (selectedLocation != null)
                    {
                        if (selectedLocation.InvoiceDispatchAdress != null)
                        {
                            if ((selectedLocation.InvoiceDispatchAdressId == selectedLocation.InvoiceAdressId ||
                                 selectedLocation.InvoiceDispatchAdressId == selectedLocation.AdressId) &&
                                (selectedLocation.InvoiceDispatchAdress.Street != LocationEditCustomerSendStreet.Text ||
                                selectedLocation.InvoiceDispatchAdress.StreetNumber != LocationEditCustomerSendStreetNr.Text ||
                                selectedLocation.InvoiceDispatchAdress.Zipcode != LocationEditSendZipCode.Text ||
                                selectedLocation.InvoiceDispatchAdress.City != LocationEditSendCity.Text ||
                                selectedLocation.InvoiceDispatchAdress.Country != LocationSendCountry.Text))
                            {
                                var newAdress = Adress.CreateAdress(LocationEditCustomerSendStreet.Text,
                                    LocationEditCustomerSendStreetNr.Text, LocationEditSendZipCode.Text, LocationEditSendCity.Text,
                                    LocationSendCountry.Text, dbContext);
                                selectedLocation.InvoiceDispatchAdress = newAdress;
                            }
                            else if (selectedLocation.InvoiceDispatchAdressId != selectedLocation.InvoiceAdressId &&
                                selectedLocation.InvoiceDispatchAdressId != selectedLocation.AdressId)
                            {
                                selectedLocation.InvoiceDispatchAdress.LogDBContext = dbContext;
                                selectedLocation.InvoiceDispatchAdress.Street = LocationEditCustomerSendStreet.Text;
                                selectedLocation.InvoiceDispatchAdress.StreetNumber = LocationEditCustomerSendStreetNr.Text;
                                selectedLocation.InvoiceDispatchAdress.Zipcode = LocationEditSendZipCode.Text;
                                selectedLocation.InvoiceDispatchAdress.City = LocationEditSendCity.Text;
                                selectedLocation.InvoiceDispatchAdress.Country = LocationSendCountry.Text;
                            }
                        }
                        else
                        {
                            if (LocationEditCustomerSendStreet.Text != string.Empty || LocationEditCustomerSendStreetNr.Text != string.Empty || LocationEditSendZipCode.Text != string.Empty || LocationEditSendCity.Text != string.Empty ||
                              LocationSendCountry.Text != string.Empty)
                            {
                                var newAdress = Adress.CreateAdress(LocationEditCustomerSendStreet.Text,
                                    LocationEditCustomerSendStreetNr.Text, LocationEditSendZipCode.Text, LocationEditSendCity.Text,
                                    LocationSendCountry.Text, dbContext);
                                selectedLocation.InvoiceDispatchAdress = newAdress;
                            }
                        }
                        if (selectedLocation.InvoiceAdress != null)
                        {
                            if ((selectedLocation.InvoiceAdressId == selectedLocation.AdressId) &&
                                (selectedLocation.InvoiceAdress.Street != LocationEditCustomerInvoiceStreet.Text ||
                                selectedLocation.InvoiceAdress.StreetNumber != LocationInvoiceStreetNr.Text ||
                                selectedLocation.InvoiceAdress.Zipcode != LocationEditInvoiceZipCode.Text ||
                                selectedLocation.InvoiceAdress.City != LocationEditInvoiceCity.Text ||
                                selectedLocation.InvoiceAdress.Country != LocationEditInvoiceCountry.Text))
                            {
                                var newAdress = Adress.CreateAdress(LocationEditCustomerInvoiceStreet.Text, LocationInvoiceStreetNr.Text,
                                    LocationEditInvoiceZipCode.Text, LocationEditInvoiceCity.Text, LocationEditInvoiceCountry.Text,
                                    dbContext);
                                selectedLocation.InvoiceAdress = newAdress;
                            }
                            else if (selectedLocation.InvoiceAdressId != selectedLocation.AdressId)
                            {
                                if (selectedLocation.InvoiceAdressId == selectedLocation.InvoiceDispatchAdressId)
                                {
                                    var newAdress = Adress.CreateAdress(LocationEditCustomerInvoiceStreet.Text, LocationInvoiceStreetNr.Text,
                                      LocationEditInvoiceZipCode.Text, LocationEditInvoiceCity.Text, LocationEditInvoiceCountry.Text,
                                      dbContext);
                                    selectedLocation.InvoiceAdress = newAdress;
                                }
                                else
                                {
                                    selectedLocation.InvoiceAdress.LogDBContext = dbContext;
                                    selectedLocation.InvoiceAdress.Street = LocationEditCustomerInvoiceStreet.Text;
                                    selectedLocation.InvoiceAdress.StreetNumber = LocationInvoiceStreetNr.Text;
                                    selectedLocation.InvoiceAdress.Zipcode = LocationEditInvoiceZipCode.Text;
                                    selectedLocation.InvoiceAdress.City = LocationEditInvoiceCity.Text;
                                    selectedLocation.InvoiceAdress.Country = LocationEditInvoiceCountry.Text;
                                }
                            }
                        }
                        else
                        {
                            if (LocationEditCustomerInvoiceStreet.Text != string.Empty || LocationInvoiceStreetNr.Text != string.Empty ||
                                    LocationEditInvoiceZipCode.Text != string.Empty || LocationEditInvoiceCity.Text != string.Empty || LocationEditInvoiceCountry.Text != string.Empty)
                            {
                                var newAdress = Adress.CreateAdress(LocationEditCustomerInvoiceStreet.Text, LocationInvoiceStreetNr.Text,
                                        LocationEditInvoiceZipCode.Text, LocationEditInvoiceCity.Text, LocationEditInvoiceCountry.Text,
                                        dbContext);
                                selectedLocation.InvoiceAdress = newAdress;
                            }
                        }
                        dbContext.SubmitChanges();
                    }
                    ts.Complete();
                    RadWindowManagerLocationDetails.RadAlert("Standort erfolgreich bearbeitet", 380, 180, "Info", "");
                }
                catch (Exception ex)
                {

                    if (ts != null)
                        ts.Dispose();
                    RadWindowManagerLocationDetails.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                    try
                    {
                        dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                        dbContext.WriteLogItem("Add/Edit Location InvoiceAdress Error:  " + ex.Message, LogTypes.ERROR, "Customer");
                        dbContext.SubmitChanges();
                    }
                    catch { }
                }
            }
            getAllCustomerLocations.EditIndexes.Clear();
            getAllCustomerLocations.MasterTableView.IsItemInserted = false;
            getAllCustomerLocations.MasterTableView.Rebind();
        }
        protected void getAllCustomerLocationsDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from cust in dbContext.Customer
                        join cost in dbContext.LargeCustomer on cust.Id equals cost.Id
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
                            CustomerNumber = cust.CustomerNumber
                        };
            e.Result = query;
        }
        protected void GetCustomerLocations_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            try
            {
                if (e.WhereParameters["Id"].ToString() != string.Empty)
                {
                    var query = from customer in dbContext.Customer
                                join location in dbContext.Location
                                on customer.Id equals location.CustomerId
                                join large in dbContext.LargeCustomer
                                on customer.Id equals large.Id
                                where customer.Id == Int32.Parse(e.WhereParameters["Id"].ToString())
                                orderby location.Name
                                select new
                                {
                                    TableId = "Inner",
                                    Id = customer.Id == null ? "0" : customer.Id.ToString(),
                                    LocationId = location.Id == null ? "0" : location.Id.ToString(),
                                    ContactId = location != null && location.ContactId != null ? location.ContactId : (int?)null,
                                    AdressId = location != null && location.AdressId != null ? location.AdressId : 0,
                                    SuperLocationId = location != null && location.SuperLocationId != null ? location.SuperLocationId : (int?)null,
                                    Name = location != null ? location.Name : null,
                                    Phone = location != null && location.Contact != null ? location.Contact.Phone : null,
                                    Fax = location != null && location.Contact != null ? location.Contact.Fax : null,
                                    MobilePhone = location != null && location.Contact != null ? location.Contact.MobilePhone : null,
                                    Email = location != null && location.Contact != null ? location.Contact.Email : null,
                                    Street = location != null && location.Adress != null ? location.Adress.Street : null,
                                    HouseNumber = location != null && location.Adress != null ? location.Adress.StreetNumber : null,
                                    Zipcode = location != null && location.Adress != null ? location.Adress.Zipcode : null,
                                    City = location != null && location.Adress != null ? location.Adress.City : null,
                                    Country = location != null && location.Adress != null ? location.Adress.Country : null,
                                    SuperLocation = location != null ? location.SuperLocationId : null,
                                    DefaulLocation = large.MainLocationId != null && large.MainLocationId == location.Id ? "true" : "false",
                                    Vat = location != null ? EmptyStringIfNull.ReturnEmptyStringIfNull(location.VAT) : null,
                                    InvoiceStreet = location != null && location.InvoiceAdress != null ? location.InvoiceAdress.Street : null,
                                    InvoiceStreeetNumber = location != null && location.InvoiceAdress != null ? location.InvoiceAdress.StreetNumber : null,
                                    InvoiceZipCode = location != null && location.InvoiceAdress != null ? location.InvoiceAdress.Zipcode : null,
                                    InvoiceCity = location != null && location.InvoiceAdress != null ? location.InvoiceAdress.City : null,
                                    InvoiceCountry = location != null && location.InvoiceAdress != null ? location.InvoiceAdress.Country : null,
                                    SendStreet = location != null && location.InvoiceDispatchAdress != null ? location.InvoiceDispatchAdress.Street : null,
                                    SendStreeetNumber = location != null && location.InvoiceDispatchAdress != null ? location.InvoiceDispatchAdress.StreetNumber : null,
                                    SendZipCode = location != null && location.InvoiceDispatchAdress != null ? location.InvoiceDispatchAdress.Zipcode : null,
                                    SendCity = location != null && location.InvoiceDispatchAdress != null ? location.InvoiceDispatchAdress.City : null,
                                    SendCountry = location != null && location.InvoiceDispatchAdress != null ? location.InvoiceDispatchAdress.Country : null
                                };
                    e.Result = query;
                }
            }
            catch (Exception ex)
            {
                RadWindowManagerLocationDetails.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                try
                {
                    dbContext.WriteLogItem("Add Location Error:  " + ex.Message, LogTypes.ERROR, "Location");
                    dbContext.SubmitChanges();
                }
                catch { }
            }
        }
        protected void AllLocationChecked(object sender, EventArgs e)
        {
            AllLocationTable.Visible = false;
            AllCustomerLocations.Visible = true;
            ResetErrorLabels();
        }
        protected void CreateLocationChecked(object sender, EventArgs e)
        {
            AllLocationTable.Visible = true;
            AllCustomerLocations.Visible = false;
            ResetErrorLabels();
        }
        protected void CustomerCombobox_OnLoad(object sender, EventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            var query = from customer in dbContext.Customer
                        join lCustomer in dbContext.LargeCustomer on customer.Id equals lCustomer.Id
                        select new
                        {
                            Name = customer.Name,
                            Id = customer.Id,
                            Matchcode = customer.MatchCode,
                            Kundennummer = customer.CustomerNumber
                        };
            CustomerLocation.Items.Clear();
            CustomerLocation.DataSource = query;
            CustomerLocation.DataBind();
        }
        protected void rbtSaveLocation_Click(object sender, EventArgs e)
        {
            if (CheckFields())
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                    try
                    {
                        if (txbLocationName.Text != string.Empty && CustomerLocation.SelectedIndex != -1)
                        {
                            var selCustomer = dbContext.LargeCustomer.SingleOrDefault(q => q.Id == Int32.Parse(CustomerLocation.SelectedValue));
                            selCustomer.Customer._dbContext = dbContext;
                            if (selCustomer != null)
                            {
                                decimal vat = 0;
                                txbVat.Text = txbVat.Text.Trim();
                                txbVat.Text = txbVat.Text.Replace('.', ',');
                                try
                                {
                                    if (txbVat.Text != string.Empty)
                                        vat = decimal.Parse(txbVat.Text);
                                }
                                catch
                                {
                                    throw new Exception("Die MwSt muss eine Dezimalzahl sein");
                                }
                                var addedLocation = selCustomer.AddNewLocation(txbLocationName.Text, txbLocationStreet.Text, txbLocationNr.Text, txbLocationZipCode.Text,
                                    cmbLocationCity.Text, cmbLocationCountry.Text,
                                      txbLocationPhone.Text, txbLocationFax.Text, txbLocationPhone.Text, txbLocationEmail.Text, vat, dbContext);

                                DeclareInvoiceSendAdress(addedLocation);
                                if (cmbOverLocation.SelectedIndex != -1)
                                {
                                    addedLocation.LogDBContext = dbContext;
                                    addedLocation._dbContext = dbContext;
                                    addedLocation.SuperLocationId = Int32.Parse(cmbOverLocation.SelectedValue);
                                }
                                dbContext.SubmitChanges();
                                ts.Complete();
                                ResetErrorLabels();
                                resultMessage.BackColor = System.Drawing.Color.Green;
                                resultMessage.Text = "Standort wurde erfolgreich angelegt";
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        if (ts != null)
                            ts.Dispose();
                        dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                        dbContext.WriteLogItem("SaveLocation_Click Error:  " + ex.Message, LogTypes.ERROR, "Location");
                        resultMessage.Text = "Fehler: " + ex.Message;
                        resultMessage.BackColor = System.Drawing.Color.Red;
                        dbContext.SubmitChanges();
                    }
                }

            }
        }
        protected void DeclareInvoiceSendAdress(Location createdLocation)
        {
            KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
            if (chbLocationRechnungsaderesse.Checked == false)
            {
                var newInvoiceAdress = Adress.CreateAdress(txbLocationInvoiceAdressStreet.Text,
                                                           txbLocationInvoiceAdressStreetNr.Text,
                                                           cmbLocationInvoiceZip.Text,
                                                           cmbLocationCityInvoice.Text,
                                                           cmbLocationInvoiceCountry.Text,
                                                           dbContext);
                createdLocation.InvoiceAdress = newInvoiceAdress;
            }
            else if (chbLocationRechnungsaderesse.Checked == true)
            {
                createdLocation.InvoiceAdress = createdLocation.Adress;
            }
            if (chbLocationVersandadresse.Checked == false)
            {
                var newInvoiceAdress = Adress.CreateAdress(txbLocationSendAdressStreet.Text,
                                                           txbLocationSendAdressStreetNr.Text,
                                                           cmbLocationSendZip.Text,
                                                           cmbLocationCitySend.Text,
                                                           cmbLocationSendCountry.Text, dbContext);
                createdLocation.InvoiceDispatchAdress = newInvoiceAdress;
            }
            else if (chbLocationVersandadresse.Checked == true && chbLocationRechnungsaderesse.Checked == true)
            {
                createdLocation.InvoiceDispatchAdress = createdLocation.Adress;
            }
            else if (chbLocationRechnungsaderesse.Checked == false && chbLocationVersandadresse.Checked == true)
            {
                createdLocation.InvoiceDispatchAdress = createdLocation.InvoiceAdress;
            }
        }
        protected void CustomerCombobox_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            if (CustomerLocation.SelectedIndex != -1)
            {
                var query = from customer in dbContext.Customer
                            join largeCustomer in dbContext.LargeCustomer on customer.Id equals largeCustomer.Id
                            join _location in dbContext.Location on largeCustomer.MainLocationId equals _location.Id
                            where customer.Id == Int32.Parse(e.Value)
                            select new
                            {
                                _location.Id,
                                _location.Name
                            };
                cmbOverLocation.Items.Clear();
                foreach (var location in query)
                {
                    if (location.Id.ToString() != string.Empty)
                    {
                        cmbOverLocation.Items.Add(new RadComboBoxItem(location.Name, location.Id.ToString()));
                    }
                }
            }
        }
        protected void btnSaveLocation_Click(object sender, EventArgs e)
        {
            bool insertUpdateOk = true;
            using (TransactionScope ts = new TransactionScope())
            {
                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString())); // hier kommt die Loggingid
                try
                {
                    Button myButton = ((Button)sender);
                    TextBox contactId = ((TextBox)myButton.FindControl("txbContactId"));
                    TextBox adressId = ((TextBox)myButton.FindControl("txbAdressId"));
                    TextBox customerId = ((TextBox)myButton.FindControl("txbCustomerId"));
                    TextBox locationId = ((TextBox)myButton.FindControl("txbLocationEditId"));
                    TextBox superLocationId = ((TextBox)myButton.FindControl("txbSuperLocationId"));
                    TextBox name = ((TextBox)myButton.FindControl("Name"));
                    TextBox phone = ((TextBox)myButton.FindControl("txbPhone"));
                    TextBox mobileNumber = ((TextBox)myButton.FindControl("txbMobile"));
                    TextBox fax = ((TextBox)myButton.FindControl("txbEditLocationFax"));
                    TextBox email = ((TextBox)myButton.FindControl("txbEditEmail"));
                    TextBox street = ((TextBox)myButton.FindControl("txbStreet"));
                    TextBox streetNumber = ((TextBox)myButton.FindControl("txbNumber"));
                    RadComboBox zipCode = ((RadComboBox)myButton.FindControl("cmbZipCode"));
                    RadComboBox city = ((RadComboBox)myButton.FindControl("cmbCity"));
                    RadComboBox country = ((RadComboBox)myButton.FindControl("cmbCountry"));
                    RadComboBox superLocation = ((RadComboBox)myButton.FindControl("cmbSuperLocation"));
                    CheckBox mainLocation = ((CheckBox)myButton.FindControl("chbDefaultLocation"));
                    TextBox textVat = ((TextBox)myButton.FindControl("txbVatEdit"));
                    if (name.Text != string.Empty)
                    {
                        var myLocation = dbContext.Location.SingleOrDefault(q => q.CustomerId == Int32.Parse(customerId.Text) && q.Id == Int32.Parse(locationId.Text));
                        var myCustomer = dbContext.Customer.SingleOrDefault(q => q.Id == Int32.Parse(customerId.Text));
                        myLocation.Name = name.Text;
                        decimal vat = 0;
                        textVat.Text = textVat.Text.Trim();
                        textVat.Text = textVat.Text.Replace('.', ',');
                        try
                        {
                            if (textVat.Text != string.Empty)
                                vat = decimal.Parse(textVat.Text);
                        }
                        catch
                        {
                            throw new Exception("Die MwSt muss eine Dezimalzahl sein");
                        }
                        myLocation.VAT = vat;
                        if (mainLocation.Checked == true)
                            myCustomer.LargeCustomer.MainLocationId = myLocation.Id;
                        else
                            myCustomer.LargeCustomer.MainLocationId = null;

                        if (!String.IsNullOrEmpty(superLocation.SelectedValue))
                        {
                            myLocation.LogDBContext = dbContext;
                            myLocation.SuperLocationId = Int32.Parse(superLocation.SelectedValue);
                        }
                        if (myLocation != null)
                        {
                            if (String.IsNullOrEmpty(contactId.Text))
                            {
                                var myContact = Contact.CreateContact(phone.Text, fax.Text, mobileNumber.Text, email.Text, dbContext);
                                myLocation.ContactId = myContact.Id;
                            }
                            else
                            {
                                myLocation.Contact.LogDBContext = dbContext;
                                myLocation.Contact.Phone = phone.Text;
                                myLocation.Contact.Fax = fax.Text;
                                myLocation.Contact.MobilePhone = mobileNumber.Text;
                                myLocation.Contact.Email = email.Text;
                            }
                            if (String.IsNullOrEmpty(adressId.Text))
                            {
                                var myAdress = Adress.CreateAdress(street.Text, streetNumber.Text, zipCode.Text, city.Text, country.Text, dbContext);
                                myLocation.AdressId = myAdress.Id;
                            }
                            else
                            {
                                myLocation.Adress.LogDBContext = dbContext;
                                myLocation.Adress.Street = street.Text;
                                myLocation.Adress.StreetNumber = streetNumber.Text;
                                myLocation.Adress.Zipcode = zipCode.Text;
                                myLocation.Adress.City = city.Text;
                                myLocation.Adress.Country = country.Text;
                            }
                        }
                        dbContext.SubmitChanges();
                        ts.Complete();
                    }
                    else
                    {
                        throw new Exception("Der Name darf nicht leer sein");
                    }

                }
                catch (Exception ex)
                {
                    insertUpdateOk = false;
                    if (ts != null)
                        ts.Dispose();
                    RadWindowManagerLocationDetails.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                    try
                    {
                        dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                        dbContext.WriteLogItem("Location Error " + ex.Message, LogTypes.ERROR, "Location");
                        dbContext.SubmitChanges();
                    }
                    catch { }
                }
            }
            if (insertUpdateOk)
            {
                getAllCustomerLocations.EditIndexes.Clear();
                getAllCustomerLocations.MasterTableView.IsItemInserted = false;
                getAllCustomerLocations.MasterTableView.Rebind();
            }
        }
        protected void btnAbort_Click(object sender, EventArgs e)
        {
            getAllCustomerLocations.EditIndexes.Clear();
            getAllCustomerLocations.MasterTableView.IsItemInserted = false;
            getAllCustomerLocations.MasterTableView.Rebind();
        }
        protected void RemoveLocation_Click(object sender, EventArgs e)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));


                try
                {
                    RadButton rbtSender = ((RadButton)sender);
                    Label lblLocationId = rbtSender.Parent.FindControl("lblLocationId") as Label;
                    Label lblCustomerId = rbtSender.Parent.FindControl("lblCustomerId") as Label;
                    if (!String.IsNullOrEmpty(lblLocationId.Text) && !String.IsNullOrEmpty(lblCustomerId.Text))
                    {
                        var custLocs = dbContext.Location.Where(q => q.CustomerId == Int32.Parse(lblCustomerId.Text));

                        if (custLocs.Count() <= 1)
                        {
                            throw new Exception("Ein Standort muss immer vorhanden sein, deshalb darf er nicht gelöscht werden");
                        }
                        var custLoc = custLocs.FirstOrDefault(q => q.Id == Int32.Parse(lblLocationId.Text));
                        custLoc._dbContext = dbContext;
                        if (custLoc.Order != null && custLoc.Order.Count > 0)
                            throw new Exception("Der Standort " + custLoc.Name + " kann nicht gelöscht werden, diese ist mit Aufträgen verknüft");

                        if (custLoc.Price != null && custLoc.Price.Count > 0)
                            throw new Exception("Der Standort " + custLoc.Name + "  kann nicht gelöscht werden, dieser ist mit Preisen verknüft");

                        KVSCommon.Database.Location.ChangeLocations(dbContext, custLoc);
                        KVSCommon.Database.Location.RemoveLocation(custLoc.Id, dbContext);

                    }

                    dbContext.SubmitChanges();
                    ts.Complete();
                    RadWindowManagerLocationDetails.RadAlert("Standort wurde erfolgreich gelöscht", 380, 180, "Info", "");


                }
                catch (Exception ex)
                {

                    if (ts != null)
                        ts.Dispose();
                    RadWindowManagerLocationDetails.RadAlert(Server.HtmlEncode(ex.Message).RemoveLineEndings(), 380, 180, "Fehler", "");
                    try
                    {
                        dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                        dbContext.WriteLogItem("RemoveLocation_Click Error " + ex.Message, LogTypes.ERROR, "Location");
                        dbContext.SubmitChanges();
                    }
                    catch { }

                }



            }
            getAllCustomerLocations.EditIndexes.Clear();
            getAllCustomerLocations.MasterTableView.IsItemInserted = false;
            getAllCustomerLocations.MasterTableView.Rebind();
        }
    }

}