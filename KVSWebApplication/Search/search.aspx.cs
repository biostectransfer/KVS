using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using KVSCommon.Database;

namespace KVSWebApplication.Search
{
    /// <summary>
    ///Suchmaske fuer die einzelnen Auftraege
    /// </summary>
    public partial class search : System.Web.UI.Page
    {

        PageStatePersister _pers;
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                if (_pers == null)
                {
                    _pers = new SessionPageStatePersister(Page);
                }
                return _pers;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected void RadGridSearch_NeedDataSource_Linq(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetSourceForRadGrid();
        }

        protected object GetSourceForRadGrid()
        {
            SearchErrorLabel.Visible = false;

            DataClasses1DataContext con = new DataClasses1DataContext();

            var newQuery = from ord in con.Order
                           let dockList = con.Document.FirstOrDefault(q => q.Id == ord.DocketList.DocumentId) 
                           let registration = ord.RegistrationOrder != null ? ord.RegistrationOrder.Registration : ord.DeregistrationOrder.Registration
      
                           select new
                              {
                                  OrderId = ord.Id,
                                  CustomerId = ord.CustomerId,
                                  OrderNumber = ord.Ordernumber,
                                  PostBackUrl = (dockList != null)? "<a href=" + '\u0022' + dockList.FileName + '\u0022' + " target=" + '\u0022' + "_blank" + '\u0022' + "> Laufzettel öffnen</a>" : "",
                                  CreateDate = ord.CreateDate,
                                  Status = ord.OrderStatus.Name,
                                  CustomerName =  ord.Customer.SmallCustomer != null &&  ord.Customer.SmallCustomer.Person != null ?  ord.Customer.SmallCustomer.Person.FirstName + " " + 
                                  ord.Customer.SmallCustomer.Person.Name :  ord.Customer.Name, 
                                  Kennzeichen = registration.Licencenumber,
                                  VIN = registration.Vehicle.VIN,
                                  CustomerLocation = ord.Location.Name,
                                  OrderTyp = ord.OrderType.Name,
                                  Haltername = registration.CarOwner.Name != string.Empty && registration.CarOwner.FirstName == string.Empty
                                  ? registration.CarOwner.Name:registration.CarOwner.FirstName,
                                  reg = registration,
                                  HasError = ord.HasError == true ? "Ja" : "Nein",
                                  Inspection = registration.GeneralInspectionDate,
                                  Variant = registration.Vehicle.Variant,
                                  TSN = registration.Vehicle.TSN,
                                  HSN = registration.Vehicle.HSN,
                                  Prevkennzeichen = ord.RegistrationOrder.PreviousLicencenumber,
                                  eVBNum = registration.eVBNumber,
                                  Name = registration.CarOwner.Name,
                                  FirstName = registration.CarOwner.FirstName,
                                  BankName = registration.CarOwner.BankAccount.BankName,
                                  AccountNum = registration.CarOwner.BankAccount.Accountnumber,
                                  BankCode = registration.CarOwner.BankAccount.BankCode,
                                  Street = registration.CarOwner.Adress.Street,
                                  StreetNr = registration.CarOwner.Adress.StreetNumber,
                                  Zip = registration.CarOwner.Adress.Zipcode,
                                  City = registration.CarOwner.Adress.City,
                                  Country = registration.CarOwner.Adress.Country,
                                  Phone = registration.CarOwner.Contact.Phone,
                                  Mobile = registration.CarOwner.Contact.MobilePhone,
                                  Fax = registration.CarOwner.Contact.Fax,
                                  Email = registration.CarOwner.Contact.Email,
                                  locationId = ord.LocationId,
                                  Freitext = ord.FreeText,
                                  VisibleWeiterleitung = (ord.OrderStatus.Name.Contains("Abgerechnet") || ord.OrderStatus.Name.Contains("Storniert") || ord.OrderStatus.Name.Contains("Zulassungsstelle")) ? false : true,
                                  ZumAuftragText = ord.OrderStatus.Name.Contains("Abgerechnet") ? "Schon abgerechnet" : ord.OrderStatus.Name.Contains("Storniert") ? "Schon storniert" : "Zum Auftrag"
                                  
                              };
                         

            if (!String.IsNullOrEmpty(CustomerNameBox.SelectedValue))
            {
                try
                {
                    newQuery = newQuery.Where(q => q.CustomerId == new Guid(CustomerNameBox.SelectedValue));
                }

                catch
                {
                    SearchErrorLabel.Visible = true;
                    SearchErrorLabel.Text = "Für " + CustomerNameBox.Text + " wurde keine Aufträge gefunden";
                }
            }

            if (!String.IsNullOrEmpty(KennzeichenSearchBox.Text))
            {
                try
                {
                    newQuery = newQuery.Where(q => q.reg.Licencenumber.Contains(KennzeichenSearchBox.Text));
                }

                catch
                {
                    SearchErrorLabel.Visible = true;
                    SearchErrorLabel.Text = "Für " + KennzeichenSearchBox.Text + " wurde keine Aufträge gefunden";
                }
            }

            if (!String.IsNullOrEmpty(HalterNameBox.Text))
            {
                try
                {
                    newQuery = newQuery.Where(q => q.reg.CarOwner.Name.Contains(HalterNameBox.Text));
                }

                catch
                {
                    SearchErrorLabel.Visible = true;
                    SearchErrorLabel.Text = "Für " + HalterNameBox.Text + " wurde keine Aufträge gefunden";
                }
            }

            if (!String.IsNullOrEmpty(FINBox.Text))
            {
                try
                {
                    newQuery = newQuery.Where(q => q.VIN.Contains(FINBox.Text));
                }

                catch
                {
                    SearchErrorLabel.Visible = true;
                    SearchErrorLabel.Text = "Für " + FINBox.Text + " wurde keine Aufträge gefunden";
                }
            }

            return newQuery;        
        }

        protected void searchButton_Clicked(object sender, EventArgs e)
        {
            RadGridSearch.Rebind();
        }

        protected void CustomerName_Selected(object sender, LinqDataSourceSelectEventArgs e)
        {
            DataClasses1DataContext con = new DataClasses1DataContext();
            var CustomerName = from cust in con.Customer
                               select new {
                                   Name = cust.SmallCustomer != null && cust.SmallCustomer.Person != null ? cust.SmallCustomer.Person.FirstName + " " + cust.SmallCustomer.Person.Name : cust.Name, 
                                   Value = cust.Id,
                                   Matchcode = cust.MatchCode,
                                   Kundennummer = cust.CustomerNumber 
                               };
            e.Result = CustomerName;
        }

        protected void NeueSucheButton_Clicked(object sender, EventArgs e)
        {
            CustomerNameBox.SelectedIndex = -1;
            CustomerNameBox.SelectedValue = string.Empty;
            CustomerNameBox.Text = string.Empty;

            CustomerNameBox.ClearCheckedItems();
            CustomerNameBox.ClearSelection();

            KennzeichenSearchBox.Text = string.Empty;
            HalterNameBox.Text = string.Empty;
            FINBox.Text = string.Empty;

            RadGridSearch.Rebind();
        }

        protected void RadGridSearch_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e is Telerik.Web.UI.GridPageChangedEventArgs || e is Telerik.Web.UI.GridFilterCommandEventArgs || e is GridPageSizeChangedEventArgs || e.CommandName == "Cancel" || e.CommandName == "Edit")
            {
                //muss nicht bearbeitet werden, da diese Events aus dem RadGrid kommen und mit der Suchfunktion nicht zu tun hat
            }
            else if(e.Item is GridDataItem)
            {
                SearchErrorLabel.Visible = false;
                bool showErrorLabel = false;
                GridDataItem item = (GridDataItem)e.Item;

                Guid customerId = new Guid(item["CustomerId"].Text);
                string status = item["Status"].Text;

                
                    if (!String.IsNullOrEmpty(item["OrderTyp"].Text))
                    {
                        if (item["OrderTyp"].Text.Contains("Zulassung")) // *** ZULASSUNG ***
                        {
                            if (item["HasError"].Text.Contains("Nein")) // falls kein Fehler
                            {
                                if (status.Contains("Offen") )// || status.Contains("Zulassungsstelle"))
                                {
                                    Session["customerIndexSearch"] = String.IsNullOrEmpty(item["CustomerLocation"].Text) ? 1 : 2;
                                    Session["orderStatusSearch"] = status;
                                    Session["customerIdSearch"] = customerId;
                                    Session["orderNumberSearch"] = item["OrderNumber"].Text;
                                    RadAjaxPanel1.Redirect("../Auftragsbearbeitung_Neuzulassung/AuftragsbearbeitungNeuzulassung.aspx");
                                }

                                else if (status.Contains("Abgeschlossen"))
                                    RadAjaxPanel1.Redirect("../Abrechnung/Abrechnung.aspx");
                                else
                                    showErrorLabel = true;
                            }

                            else if (item["HasError"].Text.Contains("Ja")) // soll zu Fehlerhaft redirect werden
                            {
                                Session["customerIndexSearch"] = String.IsNullOrEmpty(item["CustomerLocation"].Text) ? 1 : 2;
                                Session["orderStatusSearch"] = "Error";
                                Session["customerIdSearch"] = customerId;
                                Session["orderNumberSearch"] = item["OrderNumber"].Text;
                                RadAjaxPanel1.Redirect("../Auftragsbearbeitung_Neuzulassung/AuftragsbearbeitungNeuzulassung.aspx");
                            }
                        }

                        else if (item["OrderTyp"].Text.Contains("Abmeldung")) // *** ABMELDUNG ***
                        {
                            if (item["HasError"].Text.Contains("Nein")) // falls kein Fehler
                            {
                                if (status.Contains("Offen") )//|| status.Contains("Zulassungsstelle"))
                                {
                                    Session["customerIndexSearch"] = String.IsNullOrEmpty(item["CustomerLocation"].Text) ? 1 : 2;
                                    Session["orderStatusSearch"] = status;
                                    Session["customerIdSearch"] = customerId;
                                    Session["orderNumberSearch"] = item["OrderNumber"].Text;
                                    RadAjaxPanel1.Redirect("../Nachbearbeitung_Abmeldung/NachbearbeitungAbmeldung.aspx");
                                }

                                else if (status.Contains("Abgeschlossen"))
                                    RadAjaxPanel1.Redirect("../Abrechnung/Abrechnung.aspx");

                                else
                                    showErrorLabel = true;
                            }

                            else if (item["HasError"].Text.Contains("Ja")) // soll zu Fehlerhaft redirect werden
                            {
                                Session["customerIndexSearch"] = String.IsNullOrEmpty(item["CustomerLocation"].Text) ? 1 : 2;
                                Session["orderStatusSearch"] = "Error";
                                Session["customerIdSearch"] = customerId;
                                Session["orderNumberSearch"] = item["OrderNumber"].Text;
                                RadAjaxPanel1.Redirect("../Nachbearbeitung_Abmeldung/NachbearbeitungAbmeldung.aspx");
                            }
                        }

                        if (showErrorLabel == true)
                        {
                            SearchErrorLabel.Visible = true;
                            SearchErrorLabel.Text = "Auftrag mit dem Status " + item["Status"].Text + " kann nicht angezeigt werden";
                        }
                    }
                

                //else
                //{
                //    Session["customerIndexSearch"] = String.IsNullOrEmpty(item["CustomerLocation"].Text) ? 1 : 2;
                //    Session["orderStatusSearch"] = status;
                //    Session["customerIdSearch"] = customerId;
                //    Session["orderNumberSearch"] = item["OrderNumber"].Text;
                //    Response.Redirect("../Auftragsbearbeitung_Neuzulassung/AuftragsbearbeitungNeuzulassung.aspx");
                //}
            }          
        }
    }
}