using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KVSCommon.Database;
using System.Data;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using MigraDoc.DocumentObjectModel;

namespace KVSCommon.PDF
{
    /// <summary>
    /// Klasse fuer das Laufzettel PDF
    /// </summary>
    public class PackingListPDF_Zulassungsstelle : AbstractPDFCreator
    {
        protected DocketList DocketList
        {
            get;
            set;
        }
       
        /// <summary>
        /// Standardkonstruktor zum erstellen des Laufzettels
        /// </summary>
        /// <param name="docketList">Laufzettel Objekt</param>
        /// <param name="logoFilePath">Pfad zum Logo</param>
        /// <param name="dbContext">DB Kontext</param>
        public PackingListPDF_Zulassungsstelle(DocketList docketList, string logoFilePath, DataClasses1DataContext dbContext)
            : base(dbContext,logoFilePath)
        {
            this.DocketList = docketList;
            this.Headline = "Laufzettel " + docketList.DocketListNumber.ToString();
            this.Letterhead = new LetterHead(dbContext)
            {
                Lines = new List<string>(){ 
                    //this.PackingList.Order.First().Customer.Name,
                    this.DocketList.Recipient,
                    //this.PackingList.Adress.Street + " " + this.PackingList.Adress.StreetNumber,
                    //this.PackingList.Adress.Zipcode + " " + this.PackingList.Adress.City,
                    "Erstellungsdatum: " +  DateTime.Now.ToShortDateString()}
            };
           
         
        }
      /// <summary>
      /// Erstelle den PDF Kontext
      /// </summary>
        protected override void WriteContent()
        {
            var dt = this.GetPackingListDataTable();
            // var table = this.GetTableFromDataTable(dt, new List<int>() { 10, 30, 30, 30, 25, 25, 45 }, new List<int> { 0, 8, 9 }, true);
            var table = this.GetTableFromDataTable(dt, new List<int>() { 10, 60, 30, 60, 45, 25, 60 }, new List<int> { 0 }, true);
            this.Document.LastSection.Add(table);
          
        }

        protected override void WriteAppendix()
        {
        }

        protected override void WriteCoverSheet()
        {
        }
        /// <summary>
        /// Definiere die Seitenbreite und Seitenhoehe
        /// </summary>
        /// <param name="section">aktuelle Sektion</param>
        protected override void DefinePageSettings(Section section)
        {
         
            var ps = section.PageSetup;
            ps.PageHeight = new Unit(this.pageWidth, UnitType.Millimeter);
            ps.PageWidth = new Unit(this.pageHeight, UnitType.Millimeter);
            ps.LeftMargin = leftMargin - 13 + "mm";
            ps.RightMargin = rightMargin - 13 + "mm";
            //ps.TopMargin = topMargin + "mm";
            //ps.BottomMargin = bottomMargin + "mm";
            //ps.FooterDistance = FooterDistance;
            ps.Orientation = Orientation.Portrait;
        }


        /// <summary>
        /// Gibt die DataTable fuer den Laufzettel
        /// </summary>
        /// <returns></returns>
        private DataTable GetPackingListDataTable()
        {
            List<string> headers = new List<string>() { "Pos.", "Kunde", "Kennzeichen", "FIN", "Halter", "A.Gebühr", "Bemerkung"};
            DataTable dt = new DataTable();
            foreach (var header in headers)
            {
                dt.Columns.Add(header);
            }

            int i = 1;

            foreach (var order in this.DocketList.Order)
            {

                string licencenumber = string.Empty;
                string vin = string.Empty;
                string carOwnerName = string.Empty;
                string kunde = string.Empty;

                if (order.RegistrationOrder != null)
                {
                    licencenumber = order.RegistrationOrder.Registration.Licencenumber;
                  
                    vin = order.RegistrationOrder.Vehicle.VIN;
                    //if (vin.Length > 8)
                    //    vin = vin.Substring(0, 9) + " " + vin.Substring(9, vin.Length - 9); 
                    carOwnerName = order.RegistrationOrder.Registration.CarOwner.FullName;
                    kunde = (order.Customer.SmallCustomer!= null && order.Customer.SmallCustomer.Person!=null)? order.Customer.SmallCustomer.Person.Name + " " +
                        order.Customer.SmallCustomer.Person.FirstName :  order.Customer.Name;

                    //if (kunde.Length > 8)
                    //    kunde = kunde.Substring(0, 9) + " " + kunde.Substring(9, kunde.Length - 10);


                }
                else if (order.DeregistrationOrder != null)
                {
                    licencenumber = order.DeregistrationOrder.Registration.Licencenumber;
                    vin = order.DeregistrationOrder.Vehicle.VIN;
                    //if (vin.Length > 8)
                    //    vin = vin.Substring(0, 9) + " " +  vin.Substring(9, vin.Length-9);

                    carOwnerName = order.DeregistrationOrder.Registration.CarOwner.FullName;
                    kunde =  (order.Customer.SmallCustomer!= null && order.Customer.SmallCustomer.Person!=null)? order.Customer.SmallCustomer.Person.Name + " " +
                        order.Customer.SmallCustomer.Person.FirstName :  order.Customer.Name;

                    //if (kunde.Length > 8)
                    //    kunde = kunde.Substring(0, 9) + " " + kunde.Substring(9, kunde.Length - 10);

                }
                if (licencenumber.Split('-').Count() > 2)
                {
                    string[] splittedNumber = licencenumber.Split('-');
                    licencenumber = splittedNumber[0] + "-" + splittedNumber[1] + " " + splittedNumber[2];
                }

                dt.Rows.Add(
                        i,
                        kunde,
                        licencenumber,
                        vin,
                        carOwnerName);
                        //order.OrderItem.Where(q => q.IsAuthorativeCharge == false).Sum(q => q.Count * q.Amount).ToString("C2"),
                        //order.OrderItem.Where(q => q.IsAuthorativeCharge).Sum(q => q.Count * q.Amount).ToString("C2"));
                i++;
            }

            return dt;
        }
    }
}
