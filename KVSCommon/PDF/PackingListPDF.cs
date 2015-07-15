using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KVSCommon.Database;
using System.Data;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace KVSCommon.PDF
{
    /// <summary>
    /// Klasse fuer den Lieferschein Dokument
    /// </summary>
    public class PackingListPDF : AbstractPDFCreator
    {
        protected PackingList PackingList
        {
            get;
            set;
        }
        /// <summary>
        /// Standardkonstruktor fuer die Liefescheinklasse
        /// </summary>
        /// <param name="dbContext">DB JIbtext</param>
        /// <param name="packingList">Lieferschein Objekt</param>
        /// <param name="logoFilePath">Pfad zum Logo</param>
        public PackingListPDF(DataClasses1DataContext dbContext, PackingList packingList, string logoFilePath)
            : base(dbContext,logoFilePath)
        {
            this.PackingList = packingList;
            this.Headline = "Lieferschein " + packingList.PackingListNumber.ToString();
            this.Headline2 = (packingList.IsSelfDispatch.HasValue && packingList.IsSelfDispatch.Value==true) ? "Eigenverbringung" : "Paketnummer: " + this.PackingList.DispatchOrderNumber;

            var city = this.PackingList.Adress.City;

            this.Letterhead = new LetterHead(dbContext)
            {
                Lines = new List<string>(){ 
                    this.PackingList.Order.First().Customer.Name,
                    this.PackingList.Recipient,
                    this.PackingList.Adress.Street + " " + this.PackingList.Adress.StreetNumber,
                    this.PackingList.Adress.Zipcode + " " + city,
                    this.PackingList.Adress.Country,
                    "Erstellungsdatum: " + DateTime.Now.ToShortDateString()}
            };
        }
        /// <summary>
        /// Erstelle den PDF Kontext
        /// </summary>
        protected override void WriteContent()
        {
            var dt = this.GetPackingListDataTable();
           // var table = this.GetTableFromDataTable(dt, new List<int>() { 10, 20, 35, 25, 17, 17 }, new List<int> { 0 }, true);
            var table = this.GetTableFromDataTable(dt, new List<int>() { 10, 20, 35, 25, 17 }, new List<int> { 0 }, true);
            table.Columns[4].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Right;
            //table.Columns[5].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Right;
            this.Document.LastSection.Add(table);
        }

        protected override void WriteAppendix()
        {
        }

        protected override void WriteCoverSheet()
        {
        }


     
        /// <summary>
        /// Erstelle die Tabelle fuer den Lieferschein
        /// </summary>
        /// <returns>DataTable</returns>
        private DataTable GetPackingListDataTable()
        {
            //List<string> headers = new List<string>() { "Pos.", "Kennzeichen", "FIN", "Halter", "Kosten", "Gebühren" };
            List<string> headers = new List<string>() { "Pos.", "Kennzeichen", "FIN", "Halter", "Gebühren" };
            DataTable dt = new DataTable();
            foreach (var header in headers)
            {
                dt.Columns.Add(header);
            }

            int i = 1;

            foreach (var order in this.PackingList.Order)
            {

                string licencenumber = string.Empty;
                string vin = string.Empty;
                string carOwnerName = string.Empty;

                if (order.RegistrationOrder != null)
                {
                    licencenumber = order.RegistrationOrder.Registration.Licencenumber;
                    vin = order.RegistrationOrder.Vehicle.VIN;
                    carOwnerName = order.RegistrationOrder.Registration.CarOwner.FullName;
                }
                else if (order.DeregistrationOrder != null)
                {
                    licencenumber = order.DeregistrationOrder.Registration.Licencenumber;
                    vin = order.DeregistrationOrder.Vehicle.VIN;
                    carOwnerName = order.DeregistrationOrder.Registration.CarOwner.FullName;
                }
                if (licencenumber.Split('-').Count() > 2)
                {
                    string[] splittedNumber = licencenumber.Split('-');
                    licencenumber = splittedNumber[0] + "-" + splittedNumber[1] + " " + splittedNumber[2];
                }
                dt.Rows.Add(
                        i,
                        licencenumber,
                        vin,
                        carOwnerName,
                       // order.OrderItem.Where(q => q.IsAuthorativeCharge == false).Sum(q => q.Count * q.Amount).ToString("C2"),
                        order.OrderItem.Where(q => q.IsAuthorativeCharge).Sum(q => q.Count * q.Amount).ToString("C2"));
                i++;
            }

            return dt;
        }
    }
}
