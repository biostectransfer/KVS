using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using KVSCommon.Database;
using MigraDoc.DocumentObjectModel;
using System.Web;
using KVSCommon.Entities;

namespace KVSDataAccess.PDF
{
    /// <summary>
    /// Klasse fuer das Erstellen eines PDF Dokuments aus einer Rechnung
    /// </summary>
    public class InvoicePDF : AbstractPDFCreator
    {
        /// <summary>
        /// Rechnungsobjekt
        /// </summary>
        protected Invoice Invoice
        {
            get;
            set;
        }
        /// <summary>
        /// Gibt an, ob es sich um eine Vorschau handelt
        /// </summary>
        public bool Preview
        {
            get;
            set;
        }
        /// <summary>
        /// Datenbankkontext
        /// </summary>
        public IEntities _dbContext
        {
            get;
            set;
        }
        /// <summary>
        /// Standardkonstruktort fuer die Klasse InvoicePDF
        /// </summary>
        /// <param name="dbContext">DB Kontext</param>
        /// <param name="invoice">Rechnungsobjekt</param>
        /// <param name="logoFilePath">Pfad zum Logo</param>
        public InvoicePDF(IEntities dbContext, Invoice invoice, string logoFilePath)
            : base(dbContext, logoFilePath, true)
        {
            _dbContext = dbContext;
            this.Invoice = invoice;

            this.Letterhead = new LetterHead(dbContext);
            this.Letterhead.Lines = new List<string>();
            if (invoice.Customer.LargeCustomer != null)
            {
                this.Letterhead.Lines.Add(invoice.Customer.Name);
            }
            if (invoice.InvoiceRecipient != string.Empty)
            {
                this.Letterhead.Lines.Add(invoice.InvoiceRecipient);
            }
            else
            {
                if (this.Invoice.Customer != null && this.Invoice.Customer.SmallCustomer != null && this.Invoice.Customer.SmallCustomer.Person != null)
                {
                    this.Letterhead.Lines.Add(this.Invoice.Customer.SmallCustomer.Person.Name+ " " + this.Invoice.Customer.SmallCustomer.Person.FirstName);
                }
                if (this.Invoice.Customer != null && this.Invoice.Customer.LargeCustomer != null && this.Invoice.Customer.LargeCustomer.Person != null)
                {
                    this.Letterhead.Lines.Add(this.Invoice.Customer.LargeCustomer.Person.Name);
                }
            }
            if (invoice.Adress != null)
            {
                    if(invoice.Adress.Street != string.Empty || invoice.Adress.StreetNumber != string.Empty)
                    this.Letterhead.Lines.Add(invoice.Adress.Street + " " + invoice.Adress.StreetNumber);
                    if (invoice.Adress.Zipcode != string.Empty || invoice.Adress.City != string.Empty)
                    this.Letterhead.Lines.Add(invoice.Adress.Zipcode + " " + invoice.Adress.City);
                    if (invoice.Adress.Country != string.Empty)
                    this.Letterhead.Lines.Add(invoice.Adress.Country);
             }
        
            this.Headline = "Rechnung";
        }
        private void AddHeaderLetter()
        {

        }
        /// <summary>
        /// Erstelle den Kontent
        /// </summary>
        protected override void WriteContent()
        {
            var dt = this.GetOverviewDataTable();
            var table = this.GetTableFromDataTable(dt, new List<int>() { 10, 60, 15, 15, 20, 20, 20 }, new List<int>() { 0, 6 }, true);
            table.Columns[5].Format.Alignment = ParagraphAlignment.Right;
            table.AddRow();
            var tableFooterRow = table.AddRow(null, null, null, "Nettobetrag", null, null, this.Invoice.NetSum.ToString("C2"));
            tableFooterRow[3].MergeRight = 2;
            tableFooterRow[6].Shading.Color = this.TableShadingColor;
            tableFooterRow[3].Format.Font.Bold = true;
            tableFooterRow = table.AddRow(null, null, null, null, "Ust:", null, this.Invoice.TaxValue.ToString("C2"));
            tableFooterRow[4].MergeRight = 1;
            tableFooterRow[6].Shading.Color = this.TableShadingColor;
            tableFooterRow[4].Format.Font.Bold = true;
            tableFooterRow = table.AddRow(null, null, null, "amtl. Gebühren:", null, null, this.Invoice.AuthorativeChargeSum.ToString("C2"));
            tableFooterRow[3].MergeRight = 2;
            tableFooterRow[6].Shading.Color = this.TableShadingColor;
            tableFooterRow[3].Format.Font.Bold = true;

            int summaryRowCount =4;
            // Rechnungspositionen ohne MwSt:
            //foreach (var taxFreeItemsGroup in this.Invoice.TaxFreeItemsGrouped)
            //{
            //    tableFooterRow = table.AddRow(null, null, null, taxFreeItemsGroup.Key, null, null, taxFreeItemsGroup.Value.ToString("C2"));
            //    tableFooterRow[3].MergeRight = 2;
            //    tableFooterRow[6].Shading.Color = this.TableShadingColor;
            //    tableFooterRow[3].Format.Font.Bold = true;
            //    summaryRowCount++;
            //}
            decimal discount = 0;
            if (this.Invoice.discount != null && this.Invoice.discount != 0 )
            {
              
                tableFooterRow = table.AddRow(null, null, null, "Rabattierung:", null, null, this.Invoice.discount + " %");
                tableFooterRow[3].MergeRight = 2;
                tableFooterRow[6].Shading.Color = this.TableShadingColor;
                tableFooterRow[3].Format.Font.Bold = true;
                table.Columns[6].Format.Alignment = ParagraphAlignment.Right;
                summaryRowCount++;
                tableFooterRow = table.AddRow(null, null, null, "Endbetrag:", null, null, this.Invoice.GrandTotal.ToString("C2"));
                tableFooterRow[3].MergeRight = 2;
                tableFooterRow[6].Shading.Color = this.TableShadingColor;
                tableFooterRow[3].Format.Font.Bold = true;
                table.Columns[6].Format.Alignment = ParagraphAlignment.Right;

            }
            else
            {
                tableFooterRow = table.AddRow(null, null, null, "Endbetrag:", null, null, this.Invoice.GrandTotal.ToString("C2"));
                tableFooterRow[3].MergeRight = 2;
                tableFooterRow[6].Shading.Color = this.TableShadingColor;
                tableFooterRow[3].Format.Font.Bold = true;
                table.Columns[6].Format.Alignment = ParagraphAlignment.Right;
            }

       
            table.SetEdge(3, table.Rows.Count - summaryRowCount, 4, summaryRowCount, MigraDoc.DocumentObjectModel.Tables.Edge.Box, BorderStyle.Single, new Unit(1, UnitType.Point));
            table.SetEdge(3, table.Rows.Count - summaryRowCount, 4, summaryRowCount, MigraDoc.DocumentObjectModel.Tables.Edge.Interior, BorderStyle.Single, new Unit(1, UnitType.Point));
            this.Document.LastSection.Add(table);

            string invoiceText = string.Empty;
            if (!string.IsNullOrEmpty(this.Invoice.InvoiceText))
            {
                invoiceText = this.Invoice.InvoiceText;
            }
            else if (this.Invoice.InvoiceText == null)
            {
                invoiceText = GetDefaultInvoiceText(this._dbContext,this.Invoice.CustomerId, this.Invoice.UserId.Value);
            }

            var par = this.Document.LastSection.AddParagraph();
            par.AddLineBreak();
            par.Format.SpaceBefore = "10mm";
            par.Format.SpaceAfter = "10mm";
            par.AddText(invoiceText);
        }

        public string GetDefaultInvoiceText(IEntities dbContext, int customerId, int userId)
        {
            var customer = dbContext.GetSet<Customer>().Single(q => q.Id == customerId);
            //User user = dbContext.User.Single(q => q.Id == userId);
            string defaultText = dbContext.GetSet<DocumentConfiguration>().Where(q => q.Id == "INVOICE_TEXT").Select(q => q.Text).SingleOrDefault();
            if (!string.IsNullOrEmpty(defaultText))
            {
                //return string.Format(defaultText, DateTime.Now.AddDays(customer.TermOfCredit.GetValueOrDefault(30)).ToShortDateString(), user.Person.FullName);
                return string.Format(defaultText, DateTime.Now.AddDays(customer.TermOfCredit.GetValueOrDefault(30)).ToShortDateString());
            }

            return string.Empty;
        }

        /// <summary>
        /// Erstelle die Anhaenge/Erweiterungen
        /// </summary>
        protected override void WriteAppendix()
        {
            isAppendix = true;
            var section = this.Document.AddSection();
            section.Footers.EvenPage.SetNull();
            section.Footers.Primary.SetNull();
            var ps = section.PageSetup;
            ps.PageHeight = new Unit(this.pageWidth, UnitType.Millimeter);
            ps.PageWidth = new Unit(this.pageHeight, UnitType.Millimeter);
            ps.LeftMargin = leftMargin - 13 + "mm"; 
            ps.RightMargin = rightMargin -13 + "mm";
            ps.TopMargin = topMargin + "mm";
            ps.BottomMargin = bottomMargin + "mm";
            ps.FooterDistance = FooterDistance;
            string invoicenumber = "0";
            if (this.Invoice.InvoiceNumber != null)
            {
                invoicenumber = this.Invoice.InvoiceNumber.Number.ToString();
            }
            if (this.Invoice.IsPrinted == true)
            {
                invoicenumber = this.Invoice.InvoiceNumber.Number.ToString() + " (Kopie)";
            }
              if (this.Invoice.InvoiceNumber == null)
            {
                invoicenumber = "(Vorschau)";
            }

              this.WriteHeadline("Anhang zu Rechnung für : " + (Invoice.Customer.SmallCustomer != null && Invoice.Customer.SmallCustomer.Person != null ? Invoice.Customer.SmallCustomer.Person.FirstName + " " + Invoice.Customer.SmallCustomer.Person.Name : Invoice.Customer.Name) + " Rechnungsnummer : " + invoicenumber);

            string invoiceDate = this.Invoice.PrintDate.HasValue ? this.Invoice.PrintDate.Value.ToShortDateString() : DateTime.Now.ToShortDateString();
            this.WriteHeadline("Datum " + invoiceDate);

            //this.Document.LastSection.Add(this.GetTableFromDataTable(this.GetDetailDataTable(), new List<int>() { 10, 40, 20, 30, 15, 10, 10, 20 }, new List<int>() { 0, 7 }, true));
            this.Document.LastSection.Add(this.GetTableFromDataTable(this.GetDetailDataTable(), new List<int>() { 25, 20, 25, 15, 10, 10, 10, 10,10 }, new List<int>() { 7, 8 }, true));

        }
        /// <summary>
        /// Gibt die DataTable fuer die Anhaenge aus der Rechnung
        /// </summary>
        /// <returns></returns>
        private DataTable GetDetailDataTable()
        {
           // List<string> headers = new List<string>() { "Pos.", "Bezeichnung", "Auftragsnummer", "FIN", "Einh.", "Menge", "MwSt. %", "Einzelpreis" };
            List<string> headers = new List<string>() { "Bezeichnung", "Haltername", "FIN", "Kennzeichen", "E-Zul.", "Farbe", "AuftragsNr.", "Kosten", "Gebüren" };

            DataTable dt = new DataTable();
            foreach (var header in headers)
            {
                dt.Columns.Add(header);
            }

            int i = 1;
            var itemQuery = this.Invoice.InvoiceItem.Where(q => q.OrderItem != null).OrderBy(q => q.OrderItem.Order.Id).ThenBy(q => q.Name).ThenBy(q => q.OrderItem.IsAuthorativeCharge)
                .Union(this.Invoice.InvoiceItem.Where(q => q.OrderItem == null).OrderBy(q => q.Name));

            List<GroupedAppendix> grAppendixLine = new List<GroupedAppendix>();
            GroupedAppendix grLine;
            foreach (var item in itemQuery)
            {
                string vin = string.Empty;
                string ordernumber = string.Empty;
                string kennzeichen = string.Empty;
                string halter = string.Empty;
                string zulassungsDatum = string.Empty;
                string farbe = string.Empty;

                if (item.OrderItem != null)
                {
                    Order order = item.OrderItem.Order;
                    ordernumber = order.Id.ToString();
                    if (order.DeregistrationOrder != null)
                    {
                        vin = order.DeregistrationOrder.Vehicle.VIN;
                        kennzeichen = order.DeregistrationOrder.Registration.Licencenumber;
                        halter = order.DeregistrationOrder.Registration.CarOwner == null ? "" : order.DeregistrationOrder.Registration.CarOwner.Name + " " + order.DeregistrationOrder.Registration.CarOwner.FirstName;
                        zulassungsDatum = order.DeregistrationOrder.Registration.RegistrationDate == null ? "" : order.DeregistrationOrder.Registration.RegistrationDate.ToString();
                        farbe = order.DeregistrationOrder.Registration.Vehicle.ColorCode == null ? "" : order.DeregistrationOrder.Registration.Vehicle.ColorCode.ToString();                                             
                    }
                    else if (order.RegistrationOrder != null)
                    {
                        vin = order.RegistrationOrder.Vehicle.VIN;
                        kennzeichen = order.RegistrationOrder.Registration.Licencenumber;
                        halter = order.RegistrationOrder.Registration.CarOwner == null ? "" : order.RegistrationOrder.Registration.CarOwner.Name + " " + order.RegistrationOrder.Registration.CarOwner.FirstName;
                        zulassungsDatum = order.RegistrationOrder.Registration.RegistrationDate == null ? "" : order.RegistrationOrder.Registration.RegistrationDate.ToString();
                        farbe = order.RegistrationOrder.Registration.Vehicle.ColorCode == null ? "" : order.RegistrationOrder.Registration.Vehicle.ColorCode.ToString();
                     
                    }
                }
                
                var orderItemId = item.OrderItem.Id;
                var superOrderItemId = item.OrderItem.SuperOrderItemId;

                if (!String.IsNullOrEmpty(zulassungsDatum))
                    zulassungsDatum = DateTime.Parse(zulassungsDatum).ToShortDateString();

                if (item.OrderItem!= null && item.OrderItem.IsAuthorativeCharge)
                {
                   // dt.Rows.Add(i, item.Name, halter, vin, kennzeichen, zulassungsDatum, farbe, ordernumber, "",item.Amount.ToString("C2"));
                    grLine = new GroupedAppendix()
                    {
                        OrderItemId = superOrderItemId,
                        Bezeichnung = item.Name,
                        Halter = halter,
                        Vin = vin,
                        Kennzeichen = kennzeichen,
                        Zulassungsdatum = zulassungsDatum,
                        Farbe = farbe,
                        OrderNumber = ordernumber,
                        Amount = "",
                        AuthorativeCharge = item.Amount.ToString("C2"),
                        IsAuthorativeCharge = true
                    };
                    grAppendixLine.Add(grLine);
                  
                    //dt.Rows.Add(i, item.Name, "", "", "", "", "", "", "", item.Amount.ToString("C2"));
                }
                else
                {

                    grLine = new GroupedAppendix()
                    {
                        OrderItemId = orderItemId,
                        Bezeichnung = item.Name,
                        Halter = halter,
                        Vin = vin,
                        Kennzeichen = kennzeichen,
                        Zulassungsdatum = zulassungsDatum,
                        Farbe = farbe,
                        OrderNumber = ordernumber,
                        Amount = item.Amount.ToString("C2"),
                        AuthorativeCharge = "",
                        IsAuthorativeCharge = false
                    };
                    grAppendixLine.Add(grLine);

                   //dt.Rows.Add(i, item.Name, halter, vin, kennzeichen, zulassungsDatum, farbe, ordernumber, item.Amount.ToString("C2"), "");
                }
                //dt.Rows.Add(i, item.Name, halter, vin, kennzeichen, zulassungsDatum, farbe, ordernumber, item.Amount.ToString("C2"));
                //dt.Rows.Add(i, item.Name, ordernumber, vin, "Stk.", item.Count, item.VAT.ToString("F1"), item.Amount.ToString("C2"));
               
            }
            var groupedLines = grAppendixLine.GroupBy(q => new { q.OrderItemId});
            i = 1;
            foreach(var gLine in groupedLines)
            {
                var price = gLine.FirstOrDefault(q => !q.IsAuthorativeCharge);
                var authPrice = gLine.FirstOrDefault(q=>q.IsAuthorativeCharge);
                if (price != null)
                {
                    dt.Rows.Add(price.Bezeichnung, price.Halter, price.Vin, price.Kennzeichen, price.Zulassungsdatum, price.Farbe,
                        price.OrderNumber, price.Amount, authPrice == null ? "0,00 €" : authPrice.AuthorativeCharge);
                    i++;

                }
                if (price == null && authPrice != null)
                {
                    dt.Rows.Add(authPrice.Bezeichnung, authPrice.Halter, authPrice.Vin, authPrice.Kennzeichen, authPrice.Zulassungsdatum, authPrice.Farbe,
                       authPrice.OrderNumber, "0,00 €", authPrice == null ? "0,00 €" : authPrice.AuthorativeCharge);
                    i++;
                }
            }
            return dt;
        }
        /// <summary>
        /// Erstelle die gruppierten Rechnungspostionen Tabelle
        /// </summary>
        /// <returns></returns>
        private DataTable GetOverviewDataTable()
        {
            //List<string> headers = new List<string>() { "Pos.", "Bezeichnung", "Einh.", "Menge", "Einzelpreis", "MwSt. %", "Ges-Preis" };
            List<string> headers = new List<string>() { "Pos.", "Bezeichnung", "Einh.", "Menge", "Einzelpreis", "MwSt. %", "Ges-Preis" };
          
            DataTable dt = new DataTable();
            foreach (var header in headers)
            {
                dt.Columns.Add(header);
            }

            int i = 1;
            foreach (var item in this.Invoice.InvoiceItem.GroupBy(q => new
            {
                q.Name,
                q.Amount,
                q.VAT,
                IsAuthorativeCharge=q.OrderItem==null ? false: q.OrderItem.IsAuthorativeCharge
            })
            .OrderBy(q => q.Key.Name))
            {
                int sumCount = item.Sum(q => q.Count);
                decimal sum = sumCount * item.Key.Amount;
                if (!item.Key.IsAuthorativeCharge)
                {
                    decimal currVat = 0;
                    if(this.Invoice.Customer.SmallCustomer != null)
                    {
                        currVat = this.Invoice.Customer.VAT;
                    }
                    else
                    {
                      currVat=item.Key.VAT;
                    }
                    dt.Rows.Add(i, Utf8Iso(item.Key.Name), "Stk.", sumCount, item.Key.Amount.ToString("C2"), currVat.ToString("F1"), sum.ToString("C2"));
                    i++;
                }
           

            }
            dt.Rows.Add(i, "amtl. Gebühren", "", "", "", "0.0", this.Invoice.AuthorativeChargeSum.ToString("C2"));
            return dt;
        }
        /// <summary>
        /// Filtere die Sonderzeichen raus
        /// </summary>
        /// <param name="vsStr"></param>
        /// <returns></returns>
        public object Utf8Iso(string vsStr)
        {
            string strTemp = null;
            strTemp = vsStr;
            strTemp = strTemp.Replace( "Ã§", "ç");
            strTemp = strTemp.Replace("ä§", "ç");
            strTemp = strTemp.Replace( "Ã©", "é");
            strTemp = strTemp.Replace( "ä©", "é");
            strTemp = strTemp.Replace( "Ã¨", "è");
            strTemp = strTemp.Replace( "ä¨", "è");
            strTemp = strTemp.Replace( "Ãª", "ê");
            strTemp = strTemp.Replace( "äª", "ê");
            strTemp = strTemp.Replace( "Ã«", "ë");
            strTemp = strTemp.Replace( "ä«", "ë");
            strTemp = strTemp.Replace( "ÃŠ", "Ê");
            strTemp = strTemp.Replace( "äŠ", "Ê");
            strTemp = strTemp.Replace( "Ã‹", "Ë");
            strTemp = strTemp.Replace( "ä‹", "Ë");
            strTemp = strTemp.Replace( "Ã®", "î");
            strTemp = strTemp.Replace( "ä®", "î");
            strTemp = strTemp.Replace( "Ã¯", "ï");
            strTemp = strTemp.Replace( "ä¯", "ï");
            strTemp = strTemp.Replace( "Ã¬", "ì");
            strTemp = strTemp.Replace( "ÃŽ", "Î");
            strTemp = strTemp.Replace( "äŽ", "Î");
            strTemp = strTemp.Replace( "Ã²", "ò");
            strTemp = strTemp.Replace( "ä²", "ò");
            strTemp = strTemp.Replace( "Ã´", "ô");
            strTemp = strTemp.Replace( "ä´", "ô");
            strTemp = strTemp.Replace( "Ã¶", "ö");
            strTemp = strTemp.Replace( "ä¶", "ö");
            strTemp = strTemp.Replace( "Ãµ", "õ");
            strTemp = strTemp.Replace( "Ã³", "ó");
            strTemp = strTemp.Replace( "Ã¸", "ø");
            strTemp = strTemp.Replace( "äµ", "õ");
            strTemp = strTemp.Replace( "ä³", "ó");
            strTemp = strTemp.Replace( "ä¸", "ø");
            strTemp = strTemp.Replace( "Ã\"", "Ô");
            strTemp = strTemp.Replace( "ä\"", "Ô");
            strTemp = strTemp.Replace( "Ã–", "Ö");
            strTemp = strTemp.Replace( "ä–", "Ö");
            strTemp = strTemp.Replace( "Ã ", "à");
            strTemp = strTemp.Replace( "ä ", "à");
            strTemp = strTemp.Replace( "Ã¢", "â");
            strTemp = strTemp.Replace( "ä¢", "â");
            strTemp = strTemp.Replace( "Ã¤", "ä");
            strTemp = strTemp.Replace( "ä¤", "ä");
            strTemp = strTemp.Replace( "Ã¥", "å");
            strTemp = strTemp.Replace( "ä¥", "å");
            strTemp = strTemp.Replace( "Ã‚", "Â");
            strTemp = strTemp.Replace( "ä‚", "Â");
            strTemp = strTemp.Replace( "Ã„", "Ä");
            strTemp = strTemp.Replace( "ä„", "Ä");
            strTemp = strTemp.Replace( "Ã¹", "u");
            strTemp = strTemp.Replace( "Ã»", "û");
            strTemp = strTemp.Replace( "Ã¼", "ü");
            strTemp = strTemp.Replace( "ä¼", "ü");
            strTemp = strTemp.Replace( "Ã›", "Û");
            strTemp = strTemp.Replace( "Ãœ", "Ü");
            strTemp = strTemp.Replace( "ä¹", "u");
            strTemp = strTemp.Replace( "ä»", "û");
            strTemp = strTemp.Replace( "ä¼", "ü");
            strTemp = strTemp.Replace( "ä¼", "ü");
            strTemp = strTemp.Replace( "ä›", "Û");
            strTemp = strTemp.Replace( "äœ", "Ü");
            strTemp = strTemp.Replace( "Ã²", "ñ");
            strTemp = strTemp.Replace( "Ã±", "ñ");
            return strTemp;


        }
    /// <summary>
    /// Erstelle den Rechnugnskopf
    /// </summary>
        protected override void WriteCoverSheet()
        {
            Adress dispatchAdress = null;
            if (!Preview && this.Invoice.OrderInvoice.FirstOrDefault() != null && this.Invoice.OrderInvoice.FirstOrDefault().Order != null)
            {
                dispatchAdress = GetInvoiceDispatchAdress(this.Invoice.Customer.Id, this.Invoice.OrderInvoice.First().Order.LocationId, _dbContext);
            }
            else
            {
                if (this.Invoice.Customer.LargeCustomer != null && this.Invoice.Customer.LargeCustomer.Location1!=null)
                {
                    dispatchAdress = GetInvoiceDispatchAdress(this.Invoice.Customer.Id, this.Invoice.Customer.LargeCustomer.Location1.Id, _dbContext);
                }
                else
                {
                    dispatchAdress = GetInvoiceDispatchAdress(this.Invoice.Customer.Id, null, _dbContext);
                }

            }

            if (this.Invoice.Adress.ContentEquals(dispatchAdress) == false)
            {
                var coverLetterHead = new LetterHead(_dbContext);
                coverLetterHead.Topline = this.Letterhead.Topline;
                coverLetterHead.Lines = new List<string>(){
                        this.Invoice.Customer.Name,
                        this.Invoice.InvoiceRecipient,
                        dispatchAdress.Street + " " + dispatchAdress.StreetNumber,
                        dispatchAdress.Zipcode + " " + dispatchAdress.City,
                        dispatchAdress.Country};


                if (this.Invoice.Customer.LargeCustomer != null)
                {
                    this.WriteLetterHead(coverLetterHead);
                    this.Document.LastSection.AddPageBreak();
                }
            }
            //else
            //{
            //    var coverLetterHead = new LetterHead(_dbContext);
            //    coverLetterHead.Topline = this.Letterhead.Topline;
            //    coverLetterHead.Lines = new List<string>(){
            //            this.Invoice.Customer.Name,
            //            this.Invoice.InvoiceRecipient,
            //            this.Invoice.Adress.Street + " " + this.Invoice.Adress.StreetNumber,
            //            this.Invoice.Adress.Zipcode + " " + this.Invoice.Adress.City,
            //            this.Invoice.Adress.Country};

            //}
        }

        /// <summary>
        /// Gibt die Rechnungsversandadresse anhand der Kundendaten und des Standorts der Aufträge in der Rechnung zurück.
        /// </summary>
        /// <returns>Die Rechnungsversandadresse.</returns>
        public Adress GetInvoiceDispatchAdress(int customerId, int? locationId, IEntities dbContext)
        {
            var customer = dbContext.GetSet<Customer>().Single(q => q.Id == customerId);
            LargeCustomer largeCustomer = customer.LargeCustomer;
            if (largeCustomer != null)
            {
                if (largeCustomer.SendInvoiceToMainLocation && largeCustomer.MainLocationId.HasValue)
                {
                    var mainLocation = dbContext.GetSet<Location>().Single(q => q.Id == largeCustomer.MainLocationId.Value);

                    if (mainLocation.InvoiceDispatchAdress != null)
                    {
                        return mainLocation.InvoiceDispatchAdress;
                    }
                }
                else if (locationId.HasValue)
                {
                    Location location = dbContext.GetSet<Location>().Single(q => q.Id == locationId.Value);
                    if (location.InvoiceDispatchAdress != null)
                    {
                        return location.InvoiceDispatchAdress;
                    }
                }
            }

            return customer.InvoiceDispatchAdress;
        }

        /// <summary>
        /// Schreibt den Standort-spezifischen Text direkt unter der überschrift.
        /// </summary>
        protected override void WriteHeadline2()
        {
            if (this.Invoice.Customer.LargeCustomer != null && this.Invoice.Customer.LargeCustomer.Location1 != null)
            {
                var location = this.Invoice.Customer.LargeCustomer.Location1;
                if (!String.IsNullOrEmpty(location.InvoiceText))
                {
                    var text = location.InvoiceText;
                    if (text.Contains("#InvoicePeriod"))
                    {
                        var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                        text = text.Replace("#InvoicePeriod", String.Format("{0} bis {1}", startDate.ToShortDateString(), endDate.ToShortDateString()));
                    }

                    var par = this.Document.LastSection.AddParagraph(text, "Heading2");
                    par.Format.SpaceAfter = this.spaceAfterHeading;
                    par.Format.Font.Bold = false;
                }
            }
        }

        /// <summary>
        /// Erstelle die Infobox in der Rechnung
        /// </summary>
        protected override void WriteInfoBox()
        {
            var tf = this.Document.LastSection.AddTextFrame();
            tf.RelativeHorizontal = MigraDoc.DocumentObjectModel.Shapes.RelativeHorizontal.Margin;
            tf.Left = MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Right;
            tf.Width = Unit.FromCentimeter(5);
            tf.RelativeVertical = MigraDoc.DocumentObjectModel.Shapes.RelativeVertical.Page;
            tf.Top = Unit.FromMillimeter(45);
            var par = tf.AddParagraph();
            par.Format.Borders.Width = Unit.FromPoint(1);
            par.Format.TabStops.Clear();
            par.Format.TabStops.AddTabStop("3cm");
            par.AddText("Rechnungs-Nr.:");
            par.AddTab();
            if (this.Invoice.InvoiceNumber != null)
            {
                par.AddText(this.Invoice.InvoiceNumber.Number.ToString());
            }
            else
            {
                par.AddText("Vorschau");
            }

            par.AddLineBreak();
            par.AddText("Kunden-Nr.: ");
            par.AddTab();
            par.AddText(this.Invoice.Customer.CustomerNumber);
            par.AddLineBreak();
            par.AddText("Datum:");
            par.AddTab();
            par.AddText(this.Invoice.PrintDate.HasValue ? this.Invoice.PrintDate.Value.ToShortDateString() : DateTime.Now.ToShortDateString());
            //par.AddLineBreak();
            //par.AddText("Kostenstelle:");
            //par.AddTab();
            //if (this.Invoice != null)
            //{
            //    if (this.Invoice.InvoiceItem != null)
            //    {
            //        if (this.Invoice.InvoiceItem.First().CostCenter != null)
            //        {
            //            par.AddText(this.Invoice.InvoiceItem.First().CostCenter.CostcenterNumber);
            //        }
            //    }
            //}
           
        }
    }
}
