using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KVSCommon.Database;
using System.Collections;
using Telerik.Web.UI;

namespace KVSWebApplication.Customer
{
    public partial class ShowInsurance : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                setDefaultValues();
            }
        }
        /// <summary>
        /// Gibt die Mastergrid zurück (Informationen zur allen Versicherungen)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void getAllInsuranceDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            //DataClasses1DataContext dbContext = new DataClasses1DataContext();


            //var query = from insur in dbContext.Insurance
                     
            //            select new
            //            {
            //                TableId = "Outer",
            //                insur.Id,
            //                insur.Name,
            //                insur.AdressId,
            //                insur.Adress.Street,
            //                insur.Adress.StreetNumber,
            //                insur.Adress.Zipcode,
            //                insur.Adress.City,
            //                insur.Adress.Country,
                          

            //            };

            //e.Result = query;
        }
        protected void ShowChecked(object sender, EventArgs e)
        {

            ShowAllInsurance.Visible = true;
            CreateNewInsurance.Visible = false;

        }
        protected void CreateChecked(object sender, EventArgs e)
        {

            ShowAllInsurance.Visible = false;
            CreateNewInsurance.Visible = true;



        }
        protected void bSaveClick(object sender, EventArgs e)
        {
            ResetErrorLabels();
            if (checkFields() == true)
            {
                DataClasses1DataContext dbContext = new DataClasses1DataContext(new Guid("7732DF6B-C1EF-4CA5-BCBE-A21D5828A53F")); // hier kommt die Loggingid
                try
                {
                    var insuranceAdress = Adress.CreateAdress(txbInsuranceStreet.Text, txbInsuranceStreetNumber.Text,
                          cmbInsuraceZipCode.Text, cmbInsuranceCity.Text, cmbInsuranceCountry.Text, dbContext);

                   // Insurance.CreateInsurance(txbInsuranceName.Text, insuranceAdress.Id, dbContext);
                    dbContext.SubmitChanges();
                    RadWindowManagerInsurance.RadAlert("Die Versicherung wurde erfolgreich angelegt", 380, 180, "Information", "");
                    getAllInsurance.Rebind();
                }
                catch (Exception ex)
                {
                    RadWindowManagerInsurance.RadAlert(ex.Message, 380, 180, "Fehler", "");
                    dbContext.WriteLogItem("Insurance Edit Error " + ex.Message, LogTypes.ERROR, "Insurance");


                }
            }


        }
        protected void ResetErrorLabels()
        {

                    lblInsuranceNameError.Text = "";
                    lblInsuranceStreetError.Text = "";
                    lblInsuranceStreetError.Text = "";
                    lblInsuranceZipCodeError.Text = "";
                    lblInsuranceCityError.Text = "";
                    lblInsuranceCountryError.Text = "";
             



        }
        protected bool checkFields()
        {
            bool check=true;
            
            if (rbtInsuranceCreate.Checked == true)
            {

                if (txbInsuranceName.Text == string.Empty)
                {
                    lblInsuranceNameError.Text = "Bitte den Namen der Versicherung eintragen";
                    check = false;
                }
                if (txbInsuranceStreet.Text == string.Empty)
                {
                    lblInsuranceStreetError.Text = "Bitte die Strasse eintragen";
                    check = false;

                }
                if (txbInsuranceStreetNumber.Text == string.Empty)
                {
                    lblInsuranceStreetError.Text = "Bitte die Hausnummer eintragen";
                    check = false;
                }
                if (cmbInsuraceZipCode.SelectedIndex == -1 && cmbInsuraceZipCode.Text == string.Empty)
                {
                    lblInsuranceZipCodeError.Text = "Bitte die PLZ eintragen";
                    check = false;
                    
                }
                if (cmbInsuranceCity.SelectedIndex == -1 && cmbInsuranceCity.Text == string.Empty)
                {

                    lblInsuranceCityError.Text = "Bitte tragen Sie die Stadt ein";
                    check = false;
                }
                if (cmbInsuranceCountry.SelectedIndex == -1 && cmbInsuranceCountry.Text == string.Empty)
                {
                    lblInsuranceCountryError.Text = "Bitte das Land eintragen";
                    check = false;

                }

            }
            return check;

        }
        protected void getInsurance_EditCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            //Hashtable newValues = new Hashtable();
            //((GridEditableItem)e.Item).ExtractValues(newValues);
            //DataClasses1DataContext dbContext = new DataClasses1DataContext(new Guid("7732DF6B-C1EF-4CA5-BCBE-A21D5828A53F")); // hier kommt die Loggingid
            //try
            //{
            //    var checkthisInsurance = dbContext.Insurance.SingleOrDefault(q => q.Id == new Guid(newValues["Id"].ToString()));
            //    if (rbtInsuranceShow.Checked == true)
            //    {
            //        if (checkthisInsurance != null && checkthisInsurance.AdressId !=null)
            //        {

            //            if(newValues["Name"]==null || newValues["Street"]==null || newValues["Zipcode"]==null || newValues["City"]==null || newValues["Country"]==null)
            //                throw new Exception("Alle Felder müssen Informationen enthalten und dürfen nicht leer sein");

            //            checkthisInsurance.LogDBContext = dbContext;
            //            checkthisInsurance.Name = newValues["Name"].ToString();
            //            checkthisInsurance.Adress.LogDBContext = dbContext;
            //            checkthisInsurance.Adress.Street = newValues["Street"].ToString();
            //            checkthisInsurance.Adress.StreetNumber = newValues["StreetNumber"].ToString();
            //            checkthisInsurance.Adress.Zipcode = newValues["Zipcode"].ToString();
            //            checkthisInsurance.Adress.City = newValues["City"].ToString();
            //            checkthisInsurance.Adress.Country = newValues["Country"].ToString();
            //            dbContext.SubmitChanges();
                   
            //            e.Canceled = true;
            //            getAllInsurance.Rebind();
            //        }
            //    }

             
            //}
            //catch (Exception ex)
            //{


            //    RadWindowManagerInsurance.RadAlert(ex.Message, 380, 180, "Fehler", "");
            //    dbContext.WriteLogItem("Insurance Edit Error " + ex.Message, LogTypes.ERROR, "Insurance");

            //}


            //e.Canceled = true;

        }

        protected void getAllInsurance_Init(object sender, System.EventArgs e)
        {
         
                GridFilterMenu menu = getAllInsurance.FilterMenu;
                int i = 0;

                while (i < menu.Items.Count)
                {
                    if (menu.Items[i].Text == "Ist gleich" || menu.Items[i].Text == "Ist ungleich" || menu.Items[i].Text == "Ist größer als" || menu.Items[i].Text == "Ist kleiner als" || menu.Items[i].Text == "Ist größer gleich" || menu.Items[i].Text == "Ist kleiner gleich" || menu.Items[i].Text == "Enthält" || menu.Items[i].Text == "Ist zwischen" || menu.Items[i].Text == "Kein Filter")
                    {
                        i++;
                    }
                    else
                    {
                        menu.Items.RemoveAt(i);

                    }
                }
            
          
        }
        protected void setDefaultValues()
        {
            cmbInsuraceZipCode.Items.Clear();
            cmbInsuranceCity.Items.Clear();
            cmbInsuranceCountry.Items.Clear();
            DataClasses1DataContext dbContext = new DataClasses1DataContext(new Guid("7732DF6B-C1EF-4CA5-BCBE-A21D5828A53F")); // hier kommt die Loggingid
            var myZipCodes = from myList in dbContext.Adress group myList by myList.Zipcode  into zipcodes
                             select new { zipcodes };


            foreach (var Code in myZipCodes)
            {
                if (Code.zipcodes.Key != string.Empty)
                {

                    cmbInsuraceZipCode.Items.Add(new RadComboBoxItem(Code.zipcodes.Key));

                }
             

            }
            var myCitys = from myList in dbContext.Adress
                             group myList by myList.City into Citys
                             select new { Citys };


            foreach (var City in myCitys)
            {
                if (City.Citys.Key != string.Empty)
                {

                    cmbInsuranceCity.Items.Add(new RadComboBoxItem(City.Citys.Key));

                }


            }
            var myCountrys = from myList in dbContext.Adress
                          group myList by myList.Country into Countrys
                          select new { Countrys };


            foreach (var Country in myCountrys)
            {
                if (Country.Countrys.Key != string.Empty)
                {

                    cmbInsuranceCountry.Items.Add(new RadComboBoxItem(Country.Countrys.Key));

                }


            }
                
                


        }
    }
}