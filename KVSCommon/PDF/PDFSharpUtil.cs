using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace KVSCommon.PDF
{
    /// <summary>
    /// Hilfklasse fuer Wasserzeichen auf dem PDF Document
    /// </summary>
    class PDFSharpUtil
    {
        /// <summary>
        /// Erstellt eine Wassserzeichen
        /// </summary>
        /// <param name="watermarkText">Text fuer das Wasserzeichen</param>
        /// <param name="source">Quelle PDF Stream</param>
        /// <param name="target">Ziel PDF Stream</param>
        public static void WriteWatermark(string watermarkText, MemoryStream source, MemoryStream target)
        {
            PdfDocument originalPdf = PdfSharp.Pdf.IO.PdfReader.Open(source, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Modify);
            foreach (PdfPage page in originalPdf.Pages)
            {
                var gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend);
                var font = new XFont("Verdana", 120);
                var size = gfx.MeasureString(watermarkText, font);
                var brush = new XSolidBrush(XColor.FromArgb(128, 255, 0, 0));
                gfx.TranslateTransform(page.Width.Value / 2, page.Height.Value / 2);
                gfx.RotateTransform(-Math.Atan(page.Height.Value / page.Width.Value) * 180 / Math.PI);
                gfx.TranslateTransform(-page.Width.Value / 2, -page.Height.Value / 2);
                var format = new XStringFormat();
                format.Alignment = XStringAlignment.Near;
                format.LineAlignment = XLineAlignment.Near;
                gfx.DrawString(watermarkText, font, brush, new XPoint((page.Width.Value - size.Width) / 2, (page.Height.Value - size.Height) / 2), format);
            }

            originalPdf.Save(target, false);
        }
    }
}
