using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp;
using MigraDoc.DocumentObjectModel.Tables;
using System.IO;
using MigraDoc.DocumentObjectModel.Shapes;

namespace KVSCommon.PDF
{
    /// <summary>
    /// Klasse fuer die Kopfzeile des PDF Dokuments
    /// </summary>
    public class LetterHead
    {

        public string Topline
        {
            get;
            set;
        }
        public List<string> Lines
        {
            get;
            set;
        }
     
        public LetterHead(Database.DataClasses1DataContext dbContext)
        {
            string topline = dbContext.DocumentConfiguration.Where(q => q.Id == "ADDRESS_TOPLINE").Select(q => q.Text).SingleOrDefault();
            
            this.Topline = string.IsNullOrEmpty(topline) ? string.Empty : topline;
        }
    }
    /// <summary>
    /// Klasse fuer die Kopf und Fusszeile
    /// </summary>
    public class HeaderFooter
    {
        public HeaderFooter()
        {
            this.LogoHeight = new Unit(30, UnitType.Millimeter);
            this.FontSize = 8;
            this.WritePageCount = false;
        }

        public List<List<string>> Blocks
        {
            get;
            set;
        }

        public string LogoFilePath
        {
            get;
            set;
        }

        public Unit LogoHeight
        {
            get;
            set;
        }

        public int FontSize
        {
            get;
            set;
        }

        public bool WritePageCount
        {
            get;
            set;
        }
    

        /// <summary>
        /// Erstellt das Dokumentenlayout als eine Micra Doc Tabelle
        /// </summary>
        /// <param name="hf">HeaderFooter</param>
        /// <returns>Tabelle</returns>
        internal virtual Table Write(MigraDoc.DocumentObjectModel.HeaderFooter hf)
        {
     
            var table = hf.AddTable();
            
            if (this.Blocks.Count > 0)
                {
                    table.Borders.Top.Width = "0.5";
                    var tableWidth = hf.Document.LastSection.PageSetup.PageWidth.Millimeter - hf.Document.LastSection.PageSetup.LeftMargin.Millimeter - hf.Document.LastSection.PageSetup.RightMargin.Millimeter;
               
                    var columnWidth = tableWidth / this.Blocks.Count;
                    foreach (var block in this.Blocks)
                    {

                        var myColumn = table.AddColumn(new Unit(columnWidth, UnitType.Millimeter));
                    }

                    var row = table.AddRow();
               
    
                    var cellIndex = 0;
            
                    foreach (var block in this.Blocks)
                    {

                     
                        var par = row.Cells[cellIndex].AddParagraph();
                        par.Format.Font.Size = this.FontSize;
                        row.Cells[cellIndex].VerticalAlignment = VerticalAlignment.Top;
                      
                        foreach (var line in block)
                        {
                            par.AddText(line);
                            par.AddLineBreak();
                        }
                        cellIndex++;
                    }
                }
                else
                {
                    table.AddColumn();
                    table.AddRow();
                }
             
         
                if (!string.IsNullOrEmpty(this.LogoFilePath))
                {
                    var image = hf.AddImage(this.LogoFilePath);
                    image.WrapFormat.Style = MigraDoc.DocumentObjectModel.Shapes.WrapStyle.Through;
                    image.RelativeHorizontal = RelativeHorizontal.Margin;
                    image.RelativeVertical = RelativeVertical.Page;
                    image.Top = hf.Document.LastSection.PageSetup.TopMargin - this.LogoHeight;
                    image.Left = ShapePosition.Right;
                    image.LockAspectRatio = true;
                    image.Height = this.LogoHeight;
                }
            
         
            return table;
        }
    }
    /// <summary>
    /// Abstrakte Klasse fuer die Dokumente
    /// </summary>
    public abstract class AbstractPDFCreator
    {
        protected int leftMargin = 25;
        protected int rightMargin = 25;
        protected int topMargin = 35;
        protected int bottomMargin = 35;
        protected int pageHeight = 297;
        protected int pageWidth = 210;
        protected int spaceAfterHeading = 10;
        protected LetterHead Letterhead
        {
            get;
            set;
        }

        protected HeaderFooter Header
        {
            get;
            set;
        }

        protected HeaderFooter Footer
        {
            get;
            set;
        }

        protected string Headline
        {
            get;
            set;
        }
        protected string Headline2
        {
            get;
            set;
        }

        internal Document Document
        {
            get;
            set;
        }

        protected Color TableShadingColor
        {
            get
            {
                return Colors.LightGray;
            }
        }

        protected Unit LogoHeight
        {
            get
            {
                return new Unit(30, UnitType.Millimeter);
            }
        }

        protected Unit FooterDistance
        {
            get
            {
                return new Unit(10, UnitType.Millimeter);
            }
        }
        protected bool isAppendix
        {
            get;
            set;
        }
        public bool IsInvoice
        {
            get;
            set;
        }
        /// <summary>
        /// Erstellt das Grundgeruest des PDF Dokuments
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="logoFilePath"></param>
        /// <param name="_isInvoice"></param>
        protected AbstractPDFCreator(KVSCommon.Database.DataClasses1DataContext dbContext, string logoFilePath, bool _isInvoice = false)
        {
            this.IsInvoice = _isInvoice;  
            this.Document = new Document();
         

                string block1 = string.Empty;
                string block2 = string.Empty;
                string block3 = string.Empty;
                //block1 = dbContext.DocumentConfiguration.Where(q => q.Id == "HEADER_BLOCK1").Select(q => q.Text).SingleOrDefault();
                //block2 = dbContext.DocumentConfiguration.Where(q => q.Id == "HEADER_BLOCK2").Select(q => q.Text).SingleOrDefault();
                //block3 = dbContext.DocumentConfiguration.Where(q => q.Id == "HEADER_BLOCK3").Select(q => q.Text).SingleOrDefault();
                this.Header = new HeaderFooter()
                {
                    Blocks = new List<List<string>>(),
                    LogoFilePath = dbContext.DocumentConfiguration.Where(q => q.Id == "LOGOPATH").Select(q => q.Text).SingleOrDefault(),
                    LogoHeight = this.LogoHeight
                };

                //if (block1 != string.Empty || (block2 != string.Empty || block3 != string.Empty))
                //{
                //    this.Header.Blocks.Add(block1.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList());
                //    if (block2 != string.Empty || block3 != string.Empty)
                //    {
                //        this.Header.Blocks.Add(block2.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList());
                //        if (block3 != string.Empty)
                //        {
                //            this.Header.Blocks.Add(block3.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList());
                //        }
                //    }
                //}

                block1 = string.Empty;
                block2 = string.Empty;
                block3 = string.Empty;
                if (IsInvoice)
                {
                    block1 = dbContext.DocumentConfiguration.Where(q => q.Id == "FOOTER_BLOCK1").Select(q => q.Text).SingleOrDefault();
                    block2 = dbContext.DocumentConfiguration.Where(q => q.Id == "FOOTER_BLOCK2").Select(q => q.Text).SingleOrDefault();
                    block3 = dbContext.DocumentConfiguration.Where(q => q.Id == "FOOTER_BLOCK3").Select(q => q.Text).SingleOrDefault();
                }
                this.Footer = new HeaderFooter()
                {
                    WritePageCount = true,
                    Blocks = new List<List<string>>()
                };
             
                if (block1 != string.Empty || (block2 != string.Empty || block3 != string.Empty))
                {
                    this.Footer.Blocks.Add(block1.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList());
                    if (block2 != string.Empty || block3 != string.Empty)
                    {
                        this.Footer.Blocks.Add(block2.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList());
                        if (block3 != string.Empty)
                        {
                            this.Footer.Blocks.Add(block3.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList());
                        }
                    }
                }
            

            if (!string.IsNullOrEmpty(logoFilePath))
            {
                this.Header.LogoFilePath = logoFilePath;
            }
        }

        /// <summary>
        /// Definiert die Styles, die im Dokument verwendet werden können.
        /// </summary>
        protected virtual void DefineStyles()
        {
            var style = this.Document.Styles["Normal"];
            style.Font.Name = "Arial";
            style.Font.Size = 10;
            style = this.Document.AddStyle("Fn12", "Normal");
            style.Font.Size = 12;
            style = this.Document.AddStyle("Fn12B", "Fn12");
            style.Font.Bold = true;
            style = this.Document.Styles["Heading1"];
            style.Font.Color = Colors.Black;
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.ParagraphFormat.SpaceAfter = "10mm";
            style = this.Document.AddStyle("Bold", "Normal");
            style.Font.Bold = true;
            style = this.Document.Styles["Heading2"];
            style.Font.Size = 13;
            style.Font.Bold = true;
            style.Font.Color = Colors.Black;
            style.ParagraphFormat.SpaceAfter = "1cm";
            style = this.Document.AddStyle("Note", "Normal");
            style.Font.Size = 8;
            style.Font.Color = Colors.Black;
            style = this.Document.AddStyle("LetterHeadTopLine", "Normal");
            style.Font.Size = 8;
            style.Font.Color = Colors.Black;
            style.Font.Underline = Underline.Single;
        }

        /// <summary>
        /// Definiert die Header und Footer für das Dokument.
        /// </summary>
        protected virtual void DefineHeaderFooter()
        {
            var section = this.Document.AddSection();
            DefinePageSettings(section);
            section.PageSetup.OddAndEvenPagesHeaderFooter = true;
            section.PageSetup.StartingNumber = 1;
            this.Header.Write(section.Headers.Primary);
            this.Header.Write(section.Headers.EvenPage);
            this.Footer.Write(section.Footers.Primary);
            this.Footer.Write(section.Footers.EvenPage);
        }

        /// <summary>
        /// Definiert die Seiteneigenschaften für die angegebene Sektion.
        /// </summary>
        /// <param name="section">Sektion des Dokuments.</param>
        protected virtual void DefinePageSettings(Section section)
        {
            PageSetup ps = section.PageSetup;
            ps.PageHeight = this.pageHeight + "mm";
            ps.PageWidth = this.pageWidth + "mm";
            ps.LeftMargin = leftMargin + "mm";
            ps.RightMargin = rightMargin + "mm";
            ps.TopMargin = topMargin + "mm";
            ps.BottomMargin = bottomMargin + "mm";
            ps.FooterDistance = FooterDistance;
        }

        /// <summary>
        /// Schreibt den Briefkopf.
        /// </summary>
        /// <param name="letterhead"></param>
        protected virtual void WriteLetterHead(LetterHead letterhead)
        {
            if (letterhead != null)
            {
                var par = this.Document.LastSection.AddParagraph();
                par.AddFormattedText(letterhead.Topline, "LetterHeadTopLine");
                par.AddLineBreak();
                par.AddLineBreak();
                foreach (string line in letterhead.Lines)
                {
                    par.AddText(line);
                    par.AddLineBreak();
                }

                par.Format.SpaceAfter = "20mm";
                par.Format.SpaceBefore = "10mm";
            }
        }

        /// <summary>
        /// Schreibt den angegebenen Text als Überschrift.
        /// </summary>
        /// <param name="headline">Überschriftstext.</param>
        protected virtual void WriteHeadline(string headline)
        {
            var par = this.Document.LastSection.AddParagraph(headline, "Heading1");
            par.Format.SpaceAfter = this.spaceAfterHeading;
        }

        /// <summary>
        /// Schreibt den angegebenen Text direkt unter der überschrift.
        /// </summary>
        /// <param name="headline">Überschriftstext.</param>
        protected virtual void WriteHeadline1(string headline)
        {
            if (headline != null && headline != string.Empty)
            {
                var par = this.Document.LastSection.AddParagraph(headline, "Heading2");
                par.Format.SpaceAfter = this.spaceAfterHeading;
                par.Format.Font.Bold = false;
            }
        }

        /// <summary>
        /// Schreibt den Inhalt des Dokuments.
        /// </summary>
        protected abstract void WriteContent();

        /// <summary>
        /// Schreibt den Anhang des Dokuments.
        /// </summary>
        protected abstract void WriteAppendix();

        /// <summary>
        /// Schreibt das Deckblatt des Dokuments.
        /// </summary>
        protected abstract void WriteCoverSheet();

        protected virtual void WriteInfoBox()
        {
        }

        /// <summary>
        /// Schreibt das PDFDokument in den angegebenen MemoryStream.
        /// </summary>
        /// <param name="streamToWriteTo">MemoryStream, in den geschrieben wird.</param>
        public virtual void WritePDF(MemoryStream streamToWriteTo)
        {
            isAppendix = false;
            this.DefineStyles();
            this.DefineHeaderFooter();
            this.WriteCoverSheet();
            this.WriteLetterHead(this.Letterhead);
            this.WriteInfoBox();
            this.WriteHeadline(this.Headline);
            this.WriteHeadline1(this.Headline2);
            this.WriteContent();
            this.WriteAppendix();
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = this.Document;
           renderer.RenderDocument();
            renderer.Save(streamToWriteTo, false);
        }
        /// <summary>
        /// Erstellt aus einer DataTable eine Micradoc Tabelle
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="columnWidths">Spaltenbreite</param>
        /// <param name="shadedColumns">Spalten mit Schattierung</param>
        /// <param name="stretchToPageWidth">Gibt an ob die Tabelle auf das gesamte Blatt gestretcht werden soll</param>
        /// <returns>Micradoc Tabelle</returns>
        protected Table GetTableFromDataTable(DataTable dt, List<int> columnWidths, List<int> shadedColumns, bool stretchToPageWidth)
        {
            Table table = new Table();
            if (columnWidths != null)
            {
                foreach (int width in columnWidths)
                {
                    table.AddColumn(new Unit((double)width, UnitType.Millimeter));
                }
            }

            while (dt.Columns.Count > table.Columns.Count)
            {
                table.AddColumn();
            }

           

            if (stretchToPageWidth)
            {
                double pageWidth = this.Document.LastSection.PageSetup.PageWidth.Millimeter - this.Document.LastSection.PageSetup.LeftMargin.Millimeter - this.Document.LastSection.PageSetup.RightMargin.Millimeter;
                double columnWidthSum = 0;
                foreach (Column column in table.Columns)
                {
                    columnWidthSum += column.Width.Millimeter;
                }

                foreach (Column column in table.Columns)
                {
                    column.Width = new Unit(column.Width.Millimeter * pageWidth / columnWidthSum, UnitType.Millimeter);

                    if (!IsInvoice)
                    {
                        Border bor = new Border();
                        bor.Style = BorderStyle.Single;

                        column.Borders.Left = bor;

                        bor = new Border();
                        bor.Style = BorderStyle.Single;
                        column.Borders.Right = bor;
                    }
                }
            }

            var headerRow = table.AddRow();
            headerRow.HeadingFormat = true;
            headerRow.Format.Font.Bold = true;
            int i = 0;
            foreach (DataColumn dc in dt.Columns)
            {
                headerRow[i].AddParagraph(dc.ColumnName);
                headerRow[i].Shading.Color = this.TableShadingColor;
                i++;
            }

            foreach (DataRow dr in dt.Rows)
            {
                var row = table.AddRow(dr.ItemArray);
                row.Height = new Unit((IsInvoice)?0.5:0.8, UnitType.Centimeter);
                if (!IsInvoice)
                {
                    Border bor = new Border();
                    bor.Style = BorderStyle.Single;
                    row.Borders.Left = bor;
                    bor = new Border();
                    bor.Style = BorderStyle.Single;
                    row.Borders.Right = bor;

                    bor = new Border();
                    bor.Style = BorderStyle.Single;
                    row.Borders.Bottom = bor;
                }
                foreach (int sc in shadedColumns)
                {
                    row[sc].Shading.Color = this.TableShadingColor;
                }
            }

            table.SetEdge(0, 0, table.Columns.Count, 1, Edge.Box, BorderStyle.Single, new Unit(1, UnitType.Point));
            table.SetEdge(0, 1, table.Columns.Count, table.Rows.Count - 1, Edge.Box, BorderStyle.Single, new Unit(1, UnitType.Point));
            return table;
        }
    }
}
