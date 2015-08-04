using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using Telerik.Web.UI;
using System.Data.Common;
using System.Transactions;
namespace KVSWebApplication.Customer
{
    /// <summary>
    /// Codebehind fuer das Hinzufuegen eines neuen Kunden
    /// </summary>
    public partial class CustomerInformation : System.Web.UI.UserControl
    {
        private bool? come = null;
        /// <summary>
        /// Dadurch wird geregelt, welche Kudnenanlagemaske zuerst angezeigt werden soll. True=SmallCustomer, False  = LargeCustomer
        /// </summary>
        public bool? ComeFromOrder
        {
            get { return come; }
            set { come = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (KVSEntities dbContext = new KVSEntities())
                {
                    txbLargeCustomerNumber.Text = EmptyStringIfNull.generateIndividualNumber(dbContext.Customer.Max(q => q.CustomerNumber));
                    txbLargeCustomerCostCenterNumber.Text = EmptyStringIfNull.generateIndividualNumber(dbContext.CostCenter.Max(q => q.CostcenterNumber));
                    txbSmallCustomerNumber.Text = EmptyStringIfNull.generateIndividualNumber(dbContext.Customer.Max(q => q.CustomerNumber));
                }
            }
            setDefaultValues();
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
        protected void rcbInvoiceType_ItemsRequested(object sender, EventArgs e)
        {
            KVSEntities dbContext = new KVSEntities();
            RadComboBox rcbInvoiceType = ((RadComboBox)sender);
            rcbInvoiceType.Items.Clear();
            var types = from tp in dbContext.InvoiceTypes
                        select new { Key = tp.Id, Value = tp.InvoiceTypeName };
            rcbInvoiceType.DataSource = types;
            rcbInvoiceType.DataBind();
            rcbInvoiceType.Items[0].Selected = true;
        }
        protected void setDefaultValues()
        {
            chbSameAsDispatch_Checked(this, new EventArgs());
            chbSmallCustomerVersandadresse_Checked(this, new EventArgs());
            chbSmallCustomerStandartadresse_Checked(this, new EventArgs());
            chbInvoiceSameAsAdress_Checked(this, new EventArgs());
        }
        protected void chbSameAsDispatch_Checked(object sender, EventArgs e)
        {
            if (chbSameAsDispatch.Checked == true)
            {
                cmbLargeCustomerSendZipCode.Enabled = false;
                cmbSendOrt.Enabled = false;
                cmbCountrySend.Enabled = false;
                txbLargeCustomerSendStreet.Enabled = false;
                txbLargeCustomerSendStreetNr.Enabled = false;
            }
            else
            {
                cmbLargeCustomerSendZipCode.Enabled = true;
                cmbSendOrt.Enabled = true;
                cmbCountrySend.Enabled = true;
                txbLargeCustomerSendStreet.Enabled = true;
                txbLargeCustomerSendStreetNr.Enabled = true;
            }
        }
        protected void chbSmallCustomerVersandadresse_Checked(object sender, EventArgs e)
        {
            if (chbSmallCustomerVersandadresse.Checked == true)
            {
                cmbSmallCustomerSendCountry.Enabled = false;
                cmbSmallCustomerSendCity.Enabled = false;
                cmbSmallCustomerSendZip.Enabled = false;
                txbSmallCustomerSendStreetNr.Enabled = false;
                txbSmallCustomerSendStreet.Enabled = false;
            }
            else
            {
                cmbSmallCustomerSendCountry.Enabled = true;
                cmbSmallCustomerSendCity.Enabled = true;
                cmbSmallCustomerSendZip.Enabled = true;
                txbSmallCustomerSendStreetNr.Enabled = true;
                txbSmallCustomerSendStreet.Enabled = true;
            }
        }
        protected void chbSmallCustomerStandartadresse_Checked(object sender, EventArgs e)
        {
            if (chbSmallCustomerRechnungsaderesse.Checked == true)
            {
                txbsmallCustomerInvoiceStreet.Enabled = false;
                txbsmallCustomerInvoiceStreetNr.Enabled = false;
                cmbInvoiceZC.Enabled = false;
                cmbSmallCustomerInvoiceCity.Enabled = false;
                cmbSmallCustomerInvoiceCountry.Enabled = false;
            }
            else
            {
                txbsmallCustomerInvoiceStreet.Enabled = true;
                txbsmallCustomerInvoiceStreetNr.Enabled = true;
                cmbInvoiceZC.Enabled = true;
                cmbSmallCustomerInvoiceCity.Enabled = true;
                cmbSmallCustomerInvoiceCountry.Enabled = true;
            }
        }
        protected void chbInvoiceSameAsAdress_Checked(object sender, EventArgs e)
        {
            if (chbInvoiceSameAsAdress.Checked == true)
            {
                txbLargeCustomerIAStreet.Enabled = false;
                txbLargeCustomerIAStreetNr.Enabled = false;
                cmbLargeCustomerIAZipCode.Enabled = false;
                cmbIAOrt.Enabled = false;
                cmbCountryIA.Enabled = false;
            }
            else
            {
                txbLargeCustomerIAStreet.Enabled = true;
                txbLargeCustomerIAStreetNr.Enabled = true;
                cmbLargeCustomerIAZipCode.Enabled = true;
                cmbIAOrt.Enabled = true;
                cmbCountryIA.Enabled = true;
            }
        }
        protected void ResetErrorLabels()
        {
            lblCustomerNameError.Text = "";
            lblCustomerNameError.Text = "";
            lblCostCenterNameError.Text = "";
            lblStreetError.Text = "";
            lblBankNameError.Text = "";
            lblZipCodeError.Text = "";
            lblAccountError.Text = "";
            lblCityError.Text = "";
            lblErrorCountry.Text = "";
            lblBankCodeError.Text = "";
            lblSmallCustomerNachnameError.Text = "";
            lblcmbIAOrtError.Text = "";
            lblCountyIAInfoError.Text = "";
            lblLargeCustomerIAStreetError.Text = "";
            lblLargeCustomerIAZipCodeError.Text = "";
            lblLargeCustomerSendZipCodeError.Text = "";
            lblSendOrtInfoError.Text = "";
            lblCountrySendError.Text = "";
            lblLargeCustomerSendStreetError.Text = "";
            lblLargeCustomerCostCenterNummberError.Text = "";
            lblLargeCustomerNumberError.Text = "";
            lblSmallCustomerStreetError.Text = "";
            lblSmallCustomerZipCodeError.Text = "";
            lblSmallCustomerCityError.Text = "";
            lblSmallCustomerCountryError.Text = "";
            lblSmallCustomerFirstNameError.Text = "";
            lblSmallCustomerNumberError.Text = "";
            lblSmallCustomerInvoicePLZError.Text = "";
            cmbSmallCustomerInvoiceCityError.Text = "";
            cmbSmallCustomerInvoiceCityError.Text = "";
            lblSmallCustomerRechnungsStrasseError.Text = "";
            lblSmallCustomerSendZipError.Text = "";
            lblSmallCustomerSendCityError.Text = "";
            lblSmallCustomerSendCountryError.Text = "";
            lblSmallCustomerSendStreetError.Text = "";
            lblSmallCustomerInvoiceCountryError.Text = "";
            resultMessage.BackColor = System.Drawing.Color.Transparent;
            resultMessage.Text = "";
        }
        protected void bSaveClick(object sender, EventArgs e)
        {
            bool check = true;
            ResetErrorLabels();
            if (rbtLargeCustomer.Checked == true)
            {
                if (txbCustomerName.Text == string.Empty)
                {
                    lblCustomerNameError.Text = "Bitte den Kundennamen eintragen";
                    check = false;
                }
                if (CostCenterName.Text == string.Empty)
                {
                    lblCostCenterNameError.Text = "Bitte die Kostenstelle eintragen";
                    check = false;
                }
                if (txbLargeCustomerCostCenterNumber.Text == string.Empty)
                {
                    lblLargeCustomerCostCenterNummberError.Text = "Bitte die Kostenstellennummer eintragen";
                    check = false;
                }
                if (txbStreet.Text == string.Empty)
                {
                    lblStreetError.Text = "Bitte die Strasse eintragen";
                    check = false;
                }
                if (txbLargeCustomerNumber.Text == string.Empty)
                {
                    lblLargeCustomerNumberError.Text = "Bitte die Kundennummer eintragen eintragen";
                    check = false;
                }
                if (chbInvoiceSameAsAdress.Checked == false)
                {
                    if (cmbLargeCustomerIAZipCode.SelectedIndex == -1 && cmbLargeCustomerIAZipCode.Text == string.Empty)
                    {
                        lblLargeCustomerIAZipCodeError.Text = "Bitte geben Sie die PLZ ein!";
                        check = false;
                    }
                    if (cmbIAOrt.SelectedIndex == -1 && cmbIAOrt.Text == string.Empty)
                    {
                        lblcmbIAOrtError.Text = "Bitte tragen Sie den Ort ein";
                        check = false;
                    }
                    if (cmbCountryIA.SelectedIndex == -1 && cmbCountryIA.Text == string.Empty)
                    {
                        lblCountyIAInfoError.Text = "Bitte tragen Sie das Land ein";
                        check = false;
                    }
                    if (txbLargeCustomerIAStreet.Text == string.Empty && txbLargeCustomerIAStreetNr.Text == string.Empty)
                    {
                        lblLargeCustomerIAStreetError.Text = "Strasse/Hausnummer fehlt";
                        check = false;
                    }
                }
                if (chbSameAsDispatch.Checked == false)
                {
                    if (cmbLargeCustomerSendZipCode.SelectedIndex == -1 && cmbLargeCustomerSendZipCode.Text == string.Empty)
                    {
                        lblLargeCustomerSendZipCodeError.Text = "Bitte geben Sie die PLZ ein!";
                        check = false;
                    }
                    if (cmbSendOrt.SelectedIndex == -1 && cmbSendOrt.Text == string.Empty)
                    {
                        lblSendOrtInfoError.Text = "Bitte tragen Sie den Ort ein";
                        check = false;
                    }
                    if (cmbCountrySend.SelectedIndex == -1 && cmbCountrySend.Text == string.Empty)
                    {
                        lblCountrySendError.Text = "Bitte tragen Sie das Land ein";
                        check = false;
                    }
                    if (txbLargeCustomerSendStreet.Text == string.Empty && txbLargeCustomerSendStreetNr.Text == string.Empty)
                    {
                        lblLargeCustomerSendStreetError.Text = "Strasse/Hausnummer fehlt";
                        check = false;
                    }
                }
            }
            else if (rbtSmallCustomer.Checked == true)
            {
                if (txbSmallCustomerNachname.Text == string.Empty)
                {
                    lblSmallCustomerNachnameError.Text = "Bitte den Kundennamen eintragen";
                    check = false;
                }
                if (txbSmallCustomerStreet.Text == string.Empty || txbSmallCustomerNr.Text == string.Empty)
                {
                    lblSmallCustomerStreetError.Text = "Strasse/Hausnummer fehlt";
                    check = false;
                }
                if (txbSmallCustomerZipCode.Text == string.Empty)
                {
                    lblSmallCustomerZipCodeError.Text = "Bitte geben Sie die PLZ ein!";
                    check = false;
                }
                if (cmbSmallCustomerCity.Text == string.Empty)
                {
                    lblSmallCustomerCityError.Text = "Bitte tragen Sie den Ort ein";
                    check = false;
                }
                if (cmbSmallCustomerCountry.Text == string.Empty)
                {
                    lblSmallCustomerCountryError.Text = "Bitte tragen Sie das Land ein";
                    check = false;
                }
                if (txbSmallCustomerVorname.Text == string.Empty)
                {
                    lblSmallCustomerFirstNameError.Text = "Bitte tragen Sie den Vornamen ein";
                    check = false;
                }
                if (txbSmallCustomerNumber.Text == string.Empty)
                {
                    lblSmallCustomerNumberError.Text = "Bitte tragen Sie die Kundennummer ein";
                    check = false;
                }
                if (chbSmallCustomerRechnungsaderesse.Checked == false)
                {
                    if (cmbInvoiceZC.SelectedIndex == -1 && cmbInvoiceZC.Text == string.Empty)
                    {
                        lblSmallCustomerInvoicePLZError.Text = "Bitte geben Sie die PLZ ein!";
                        check = false;
                    }
                    if (cmbSmallCustomerInvoiceCity.SelectedIndex == -1 && cmbSmallCustomerInvoiceCity.Text == string.Empty)
                    {
                        cmbSmallCustomerInvoiceCityError.Text = "Bitte tragen Sie den Ort ein";
                        check = false;
                    }
                    if (cmbSmallCustomerInvoiceCountry.SelectedIndex == -1 && cmbSmallCustomerInvoiceCountry.Text == string.Empty)
                    {
                        lblSmallCustomerInvoiceCountryError.Text = "Bitte tragen Sie das Land ein";
                        check = false;
                    }
                    if (txbsmallCustomerInvoiceStreet.Text == string.Empty && txbsmallCustomerInvoiceStreetNr.Text == string.Empty)
                    {
                        lblSmallCustomerRechnungsStrasseError.Text = "Strasse/Hausnummer fehlt";
                        check = false;
                    }
                }
                if (chbSmallCustomerVersandadresse.Checked == false)
                {
                    if (cmbSmallCustomerSendZip.SelectedIndex == -1 && cmbSmallCustomerSendZip.Text == string.Empty)
                    {
                        lblSmallCustomerSendZipError.Text = "Bitte geben Sie die PLZ ein!";
                        check = false;
                    }
                    if (cmbSmallCustomerSendCity.SelectedIndex == -1 && cmbSmallCustomerSendCity.Text == string.Empty)
                    {
                        lblSmallCustomerSendCityError.Text = "Bitte tragen Sie den Ort ein";
                        check = false;
                    }
                    if (cmbSmallCustomerSendCountry.SelectedIndex == -1 && cmbSmallCustomerSendCountry.Text == string.Empty)
                    {
                        lblSmallCustomerSendCountryError.Text = "Bitte tragen Sie das Land ein";
                        check = false;
                    }
                    if (txbSmallCustomerSendStreet.Text == string.Empty && txbSmallCustomerSendStreetNr.Text == string.Empty)
                    {
                        lblSmallCustomerSendStreetError.Text = "Strasse/Hausnummer fehlt";
                        check = false;
                    }
                }
            }
            if (check == true)
            {
                AddCustomerToDatabase();
            }
        }
        protected void AddCustomerToDatabase()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                KVSEntities dbContext = new KVSEntities(Int32.Parse(Session["CurrentUserId"].ToString()));
                try
                {
                    BankAccount bankAccount = null;
                    LargeCustomer lg = null;
                    SmallCustomer sm = null;
                    Adress newAdress = null;
                    Adress newInvoiceAdress = null;
                    Adress newSendAdress = null;
                    if (rbtLargeCustomer.Checked == true)
                    {
                        var checkThisCustomer = dbContext.Customer.FirstOrDefault(q => q.CustomerNumber == txbLargeCustomerNumber.Text);
                        if (checkThisCustomer == null)
                        {
                            decimal vat = 0;
                            txbLargeCustomerVat.Text = txbLargeCustomerVat.Text.Trim();
                            txbLargeCustomerVat.Text = txbLargeCustomerVat.Text.Replace('.', ',');
                            try
                            {
                                vat = decimal.Parse(txbLargeCustomerVat.Text);
                            }
                            catch
                            {
                                throw new Exception("Die MwSt muss eine Dezimalzahl sein");
                            }
                            int zz = 0;
                            txbLargeCustomerZahlungsziel.Text = txbLargeCustomerZahlungsziel.Text.Trim();
                            if (txbLargeCustomerZahlungsziel.Text != string.Empty)
                            {
                                try
                                {
                                    zz = int.Parse(txbLargeCustomerZahlungsziel.SelectedValue);
                                }
                                catch
                                {
                                    throw new Exception("Das Zahlungsziel ausgewählt sein");
                                }
                            }
                            int? myPersonId = null;
                            if (txbLargeCustomerFirstname.Text != string.Empty || txbLargeCustomerName.Text != string.Empty ||
                                txbLargeCustomerZusatz.Text != string.Empty)
                            {
                                myPersonId = Person.CreatePerson(dbContext, txbLargeCustomerFirstname.Text, txbLargeCustomerName.Text, txbLargeCustomerSalutation.Text, txbLargeCustomerZusatz.Text).Id;
                            }
                            var newCustomer = LargeCustomer.CreateLargeCustomer(dbContext, txbCustomerName.Text, txbStreet.Text, txbStreetNumber.Text, ZipCode.Text, cmbCity.Text,
                               cmbCountry.Text, PhoneNumer.Text, txbFax.Text, txbMobil.Text, txbEmailAdress.Text, vat, chbSendToMainLocation.Checked, chbSendViaMail.Checked, zz,
                               txbLargeCustomerNumber.Text, txbLargeCustomerMatchcode.Text, txbLargeCustomerDebitorenNummer.Text, myPersonId,
                               Int32.Parse(rcbInvoiceType.SelectedValue), txbEvbNumber.Text);
                            if (chbInvoiceSameAsAdress.Checked == true && chbSameAsDispatch.Checked == true)
                            {
                                newCustomer.Customer.InvoiceAdressId = newCustomer.Customer.AdressId;
                                newCustomer.Customer.InvoiceDispatchAdressId = newCustomer.Customer.AdressId;
                            }
                            if (chbInvoiceSameAsAdress.Checked == false && chbSameAsDispatch.Checked == true)
                            {
                                newAdress = Adress.CreateAdress(txbLargeCustomerIAStreet.Text, txbLargeCustomerIAStreetNr.Text, cmbLargeCustomerIAZipCode.Text,
                                    cmbIAOrt.Text, cmbCountryIA.Text, dbContext);
                                newCustomer.Customer.InvoiceAdressId = newAdress.Id;
                                newCustomer.Customer.InvoiceDispatchAdressId = newCustomer.Customer.AdressId;
                            }
                            if (chbInvoiceSameAsAdress.Checked == true && chbSameAsDispatch.Checked == false)
                            {
                                newAdress = Adress.CreateAdress(txbLargeCustomerSendStreet.Text, txbLargeCustomerSendStreetNr.Text, cmbLargeCustomerSendZipCode.Text, cmbSendOrt.Text,
                                    cmbCountrySend.Text, dbContext);
                                newCustomer.Customer.InvoiceAdressId = newCustomer.Customer.AdressId;
                                newCustomer.Customer.InvoiceDispatchAdressId = newAdress.Id;
                            }
                            if (chbInvoiceSameAsAdress.Checked == false && chbSameAsDispatch.Checked == false)
                            {
                                newInvoiceAdress = Adress.CreateAdress(txbLargeCustomerIAStreet.Text, txbLargeCustomerIAStreetNr.Text, cmbLargeCustomerIAZipCode.Text, cmbIAOrt.Text,
                                    cmbCountryIA.Text, dbContext);
                                newSendAdress = Adress.CreateAdress(txbLargeCustomerSendStreet.Text, txbLargeCustomerSendStreetNr.Text, cmbLargeCustomerSendZipCode.Text, cmbSendOrt.Text,
                                    cmbCountrySend.Text, dbContext);
                                newCustomer.Customer.InvoiceAdressId = newInvoiceAdress.Id;
                                newCustomer.Customer.InvoiceDispatchAdressId = newSendAdress.Id;
                            }
                            if (LCustomerAuftragNow.Checked == true)
                            {
                                newCustomer.OrderFinishedNoteSendType = 1;
                            }
                            if (LCustomerAuftragHourly.Checked == true)
                            {
                                newCustomer.OrderFinishedNoteSendType = 2;
                            }
                            if (LCustomerAuftragDayly.Checked == true)
                            {
                                newCustomer.OrderFinishedNoteSendType = 3;
                            }
                            else
                            {
                                newCustomer.OrderFinishedNoteSendType = 0;
                            }
                            if (chbLCustomerAuftragKunde.Checked == true)
                            {
                                newCustomer.SendOrderFinishedNoteToCustomer = true;
                            }
                            if (chbLCustomerAuftragStandort.Checked == true)
                            {
                                newCustomer.SendOrderFinishedNoteToLocation = true;
                            }
                            if (chbLCustomerLiefescheinCustomer.Checked == true)
                            {
                                newCustomer.SendPackingListToCustomer = true;
                            }
                            if (chbLCustomerLieferscheinStandort.Checked == true)
                            {
                                newCustomer.SendPackingListToLocation = true;
                            }
                            var costCenter = newCustomer.AddNewCostCenter(CostCenterName.Text, txbLargeCustomerCostCenterNumber.Text, dbContext);
                            bankAccount = new BankAccount();
                            if (txbBankName.Text != string.Empty || txbLargeCustomerIBAN.Text != string.Empty)
                            {
                                bankAccount = BankAccount.CreateBankAccount(dbContext, txbBankName.Text, txbAccount.Text, txbBankCode.Text, txbLargeCustomerIBAN.Text, txbLargeCustomerBIC.Text);
                                costCenter.BankAccountId = bankAccount.Id;
                            }

                            //TODO newCustomer.Customer.InternalId = Guid.NewGuid();

                            lg = newCustomer;
                            dbContext.SubmitChanges();
                            ts.Complete();
                            resultMessage.BackColor = System.Drawing.Color.Green;
                            resultMessage.Text = "Der Kunde wurde erfolgreich angelegt";
                        }
                        else
                        {
                            resultMessage.BackColor = System.Drawing.Color.Red;
                            resultMessage.Text = "Die Kundennummer existiert bereits";
                        }
                    }
                    if (rbtSmallCustomer.Checked == true)
                    {
                        var checkThisCustomer = dbContext.Customer.SingleOrDefault(q => q.CustomerNumber == txbSmallCustomerNumber.Text);
                        if (checkThisCustomer == null)
                        {
                            decimal vat = 0;
                            txbSmallCustomerVat.Text = txbSmallCustomerVat.Text.Trim();
                            txbSmallCustomerVat.Text = txbSmallCustomerVat.Text.Replace('.', ',');
                            try
                            {
                                if (txbSmallCustomerVat.Text != string.Empty)
                                    vat = decimal.Parse(txbSmallCustomerVat.Text);
                            }
                            catch
                            {
                                throw new Exception("Die MwSt muss eine Dezimalzahl sein");
                            }
                            int zz = 0;
                            txbSmallCustomerZahlungsziel.Text = txbSmallCustomerZahlungsziel.Text.Trim();
                            if (txbSmallCustomerZahlungsziel.Text != string.Empty)
                            {
                                try
                                {
                                    zz = int.Parse(txbSmallCustomerZahlungsziel.SelectedValue);
                                }
                                catch
                                {
                                    throw new Exception("Das Zahlungsziel muss eine Gleitkommazahl sein");
                                }
                            }
                            var newSmallCustomer = SmallCustomer.CreateSmallCustomer(txbSmallCustomerVorname.Text, txbSmallCustomerNachname.Text, txbSmallCustomerTitle.Text, cmbSmallCustomerGender.SelectedValue,
                              txbSmallCustomerStreet.Text, txbSmallCustomerNr.Text, txbSmallCustomerZipCode.Text, cmbSmallCustomerCity.Text, cmbSmallCustomerCountry.Text, txbSmallCustomerPhone.Text,
                                txbSmallCustomerFax.Text, txbSmallCustomerMobil.Text, txbSmallCustomerEmail.Text, vat, zz, txbSmallCustomerNumber.Text, dbContext);
                            newSmallCustomer.Customer.LogDBContext = dbContext;
                            if (chbSmallCustomerRechnungsaderesse.Checked == true && chbSmallCustomerVersandadresse.Checked == true)
                            {
                                newSmallCustomer.Customer.InvoiceAdressId = newSmallCustomer.Customer.AdressId;
                                newSmallCustomer.Customer.InvoiceDispatchAdressId = newSmallCustomer.Customer.AdressId;
                            }
                            if (chbSmallCustomerRechnungsaderesse.Checked == false && chbSmallCustomerVersandadresse.Checked == true)
                            {
                                newAdress = Adress.CreateAdress(txbsmallCustomerInvoiceStreet.Text, txbsmallCustomerInvoiceStreetNr.Text, cmbInvoiceZC.Text, cmbSmallCustomerInvoiceCity.Text, cmbSmallCustomerInvoiceCountry.Text, dbContext);
                                newSmallCustomer.Customer.InvoiceAdressId = newAdress.Id;
                                newSmallCustomer.Customer.InvoiceDispatchAdressId = newSmallCustomer.Customer.AdressId;
                            }
                            if (chbSmallCustomerRechnungsaderesse.Checked == true && chbSmallCustomerVersandadresse.Checked == false)
                            {
                                newAdress = Adress.CreateAdress(txbSmallCustomerSendStreet.Text, txbSmallCustomerSendStreetNr.Text, cmbSmallCustomerSendZip.Text, cmbSmallCustomerSendCity.Text, cmbSmallCustomerSendCountry.Text, dbContext);
                                newSmallCustomer.Customer.InvoiceAdressId = newSmallCustomer.Customer.AdressId;
                                newSmallCustomer.Customer.InvoiceDispatchAdressId = newAdress.Id;
                            }
                            if (chbSmallCustomerRechnungsaderesse.Checked == false && chbSmallCustomerVersandadresse.Checked == false)
                            {
                                newInvoiceAdress = Adress.CreateAdress(txbsmallCustomerInvoiceStreet.Text, txbsmallCustomerInvoiceStreetNr.Text, cmbInvoiceZC.Text, cmbSmallCustomerInvoiceCity.Text, cmbSmallCustomerInvoiceCountry.Text, dbContext);
                                newSendAdress = Adress.CreateAdress(txbSmallCustomerSendStreet.Text, txbSmallCustomerSendStreetNr.Text, cmbSmallCustomerSendZip.Text, cmbSmallCustomerSendCity.Text, cmbSmallCustomerSendCountry.Text, dbContext);
                                newSmallCustomer.Customer.InvoiceAdressId = newInvoiceAdress.Id;
                                newSmallCustomer.Customer.InvoiceDispatchAdressId = newSendAdress.Id;
                            }
                            if (txbSmallCustomerKreditinstitut.Text != string.Empty || txbSmallCustomerAccountNumber.Text != string.Empty || txbSmallCustomerBankCode.Text != string.Empty || txbSmallCustomerIBANinfo.Text != string.Empty)
                            {
                                newSmallCustomer.LogDBContext = dbContext;
                                var addBankAccount = BankAccount.CreateBankAccount(dbContext, txbSmallCustomerKreditinstitut.Text, txbSmallCustomerAccountNumber.Text, txbSmallCustomerBankCode.Text, txbSmallCustomerIBANinfo.Text, txbSmallCustomerBIC.Text);
                                newSmallCustomer.BankAccountId = addBankAccount.Id;
                            }
                            sm = newSmallCustomer;
                            dbContext.SubmitChanges();
                            ts.Complete();
                            resultMessage.BackColor = System.Drawing.Color.Green;
                            resultMessage.Text = "Der Kunde wurde erfolgreich angelegt";
                        }
                        else
                        {
                            resultMessage.BackColor = System.Drawing.Color.Red;
                            resultMessage.Text = "Die Kundennummer existiert bereits";
                        }
                    }
                }
                catch (Exception ex)
                {
                    resultMessage.BackColor = System.Drawing.Color.Red;
                    resultMessage.Text = "Fehler: " + ex.Message;
                    dbContext.WriteLogItem("Add/Edit Customer Error:  " + ex.Message, LogTypes.ERROR, "Customer");
                }
            }
        }
        protected void LargeCustomerChecked(object sender, EventArgs e)
        {
            come = null;
            ResetErrorLabels();
            CreateLargeCustomerTable.Visible = true;
            CreateSmallCustomerTable.Visible = false;
        }
        protected void SmallCustomerChecked(object sender, EventArgs e)
        {
            come = null;
            ResetErrorLabels();
            CreateLargeCustomerTable.Visible = false;
            CreateSmallCustomerTable.Visible = true;
        }
        protected void genIban_Click(object sender, EventArgs e)
        {
            if (txbSmallCustomerAccountNumber.Text != string.Empty && txbSmallCustomerBankCode.Text != string.Empty
                && EmptyStringIfNull.IsNumber(txbSmallCustomerAccountNumber.Text) && EmptyStringIfNull.IsNumber(txbSmallCustomerBankCode.Text))
            {
                txbSmallCustomerIBANinfo.Text = "DE" + (98 - ((62 * ((1 + long.Parse(txbSmallCustomerBankCode.Text) % 97)) +
                    27 * (long.Parse(txbSmallCustomerAccountNumber.Text) % 97)) % 97)).ToString("D2");
                txbSmallCustomerIBANinfo.Text += long.Parse(txbSmallCustomerBankCode.Text).ToString("00000000").Substring(0, 4);
                txbSmallCustomerIBANinfo.Text += long.Parse(txbSmallCustomerBankCode.Text).ToString("00000000").Substring(4, 4);
                txbSmallCustomerIBANinfo.Text += long.Parse(txbSmallCustomerAccountNumber.Text).ToString("0000000000").Substring(0, 4);
                txbSmallCustomerIBANinfo.Text += long.Parse(txbSmallCustomerAccountNumber.Text).ToString("0000000000").Substring(4, 4);
                txbSmallCustomerIBANinfo.Text += long.Parse(txbSmallCustomerAccountNumber.Text).ToString("0000000000").Substring(8, 2);
                using (KVSEntities dataContext = new KVSEntities())
                {
                    var bicNr = dataContext.BIC_DE.FirstOrDefault(q => q.Bankleitzahl.Contains(txbSmallCustomerBankCode.Text) && (q.Bezeichnung.Contains(txbSmallCustomerKreditinstitut.Text) || q.Kurzbezeichnung.Contains(txbSmallCustomerKreditinstitut.Text)));
                    if (bicNr != null)
                    {
                        if (!String.IsNullOrEmpty(bicNr.BIC.ToString()))
                            txbSmallCustomerBIC.Text = bicNr.BIC.ToString();
                    }
                }
            }
        }
        protected void genIbanLargeCustomer_Click(object sender, EventArgs e)
        {
            if (txbAccount.Text != string.Empty && txbBankCode.Text != string.Empty
                && EmptyStringIfNull.IsNumber(txbAccount.Text) && EmptyStringIfNull.IsNumber(txbBankCode.Text))
            {
                txbLargeCustomerIBAN.Text = "DE" + (98 - ((62 * ((1 + long.Parse(txbBankCode.Text) % 97)) +
                    27 * (long.Parse(txbAccount.Text) % 97)) % 97)).ToString("D2");
                txbLargeCustomerIBAN.Text += long.Parse(txbBankCode.Text).ToString("00000000").Substring(0, 4);
                txbLargeCustomerIBAN.Text += long.Parse(txbBankCode.Text).ToString("00000000").Substring(4, 4);
                txbLargeCustomerIBAN.Text += long.Parse(txbAccount.Text).ToString("0000000000").Substring(0, 4);
                txbLargeCustomerIBAN.Text += long.Parse(txbAccount.Text).ToString("0000000000").Substring(4, 4);
                txbLargeCustomerIBAN.Text += long.Parse(txbAccount.Text).ToString("0000000000").Substring(8, 2);
                using (KVSEntities dataContext = new KVSEntities())
                {
                    var bicNr = dataContext.BIC_DE.FirstOrDefault(q => q.Bankleitzahl.Contains(txbBankCode.Text) && (q.Bezeichnung.Contains(txbBankName.Text) || q.Kurzbezeichnung.Contains(txbBankName.Text)));
                    if (bicNr != null)
                    {
                        if (!String.IsNullOrEmpty(bicNr.BIC.ToString()))
                            txbLargeCustomerBIC.Text = bicNr.BIC.ToString();
                    }
                }
            }
        }
    }
}