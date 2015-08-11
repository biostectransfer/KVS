using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Xml;
using System.IO.Packaging;
using System.Text.RegularExpressions;
using Neodynamic.SDK.Web;

namespace KVSWebApplication.Auftragsbearbeitung_Neuzulassung
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class Print : IHttpHandler
    {
        private static Regex htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        public void ProcessRequest(HttpContext context)
        {
            var cpj = new ClientPrintJob();

            int printType = 0;

            if(String.IsNullOrEmpty(context.Request["LicenceNumber"]))
            {
                throw new Exception("Kennzeichnen ist nicht eigetragen.");
            }

            if (Int32.TryParse(context.Request["printType"], out printType))
            {
                if (printType == 0)
                {
                    var dataDirectory = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data");
                    var path = Path.Combine(dataDirectory, ConfigurationManager.AppSettings["EmissionBadgeFileName"]);

                    var stream = GenerateExportReport(path, context.Request["LicenceNumber"]);

                    var file = new PrintFile(GetBytesFromStream(stream), "Print.docx");
                    cpj.PrintFile = file;
                    cpj.ClientPrinter = new InstalledPrinter(ConfigurationManager.AppSettings["PrinterName"]);

                    cpj.SendToClient(context.Response);
                }
                else if (printType == 1)
                {
                    try
                    {
                        var barsetting = new Spire.Barcode.BarcodeSettings();
                        barsetting.UseChecksum = Spire.Barcode.CheckSumMode.Auto;
                        barsetting.Type = Spire.Barcode.BarCodeType.EAN128;
                        barsetting.ShowText = true;

                        var bargenerator = new Spire.Barcode.BarCodeGenerator(barsetting);
                        
                        barsetting.Data = context.Request["LicenceNumber"];
                        //barsetting.Data2D = barsetting.Data;

                        var image = bargenerator.GenerateImage();

                        var stream = new MemoryStream();
                        image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

                        var dataDirectory = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data");
                        var path = Path.Combine(dataDirectory, "test.jpg");
                        image.Save(path);

                        var file = new PrintFile(GetBytesFromStream(stream), "Barcode.jpg");
                        cpj.PrintFile = file;
                        cpj.ClientPrinter = new InstalledPrinter(ConfigurationManager.AppSettings["PrinterName"]);

                        cpj.SendToClient(context.Response);
                    }
                    catch (Exception ex)
                    {
                        //skip errors
                    }
                }
            }
        }

        private Stream GenerateExportReport(string fullPath, string licenceNumber)
        {
            var result = new MemoryStream();
            try
            {
                var fileStream = File.Open(fullPath, FileMode.Open);
                CopyStream(fileStream, result);
                fileStream.Close();

                var pkg = Package.Open(result, FileMode.Open, FileAccess.ReadWrite);

                var uri = new Uri("/word/document.xml", UriKind.Relative);
                var part = pkg.GetPart(uri);

                var xmlReader = XmlReader.Create(part.GetStream(FileMode.Open, FileAccess.Read));
                var xmlMainXMLDoc = XDocument.Load(xmlReader);

                var templateBody = ReplaceFields(xmlMainXMLDoc, licenceNumber);

                xmlMainXMLDoc = XDocument.Parse(templateBody);

                var partWrt = new StreamWriter(part.GetStream(FileMode.Open, FileAccess.ReadWrite));
                xmlMainXMLDoc.Save(partWrt);

                partWrt.Flush();
                partWrt.Close();
                pkg.Close();

                result.Position = 0;

                xmlReader.Close();
            }
            catch
            {
            }

            return result;
        }

        private string ReplaceFields(XDocument xmlMainXMLDoc, string licenceNumber)
        {
            string result = xmlMainXMLDoc.Root.ToString();
            result = result.Replace("#LicenceNumber", licenceNumber);

            result = result.Replace("#AdditionalInfo", ConfigurationManager.AppSettings["EmissionBadgeAdditionalInfo"]);

            return result;
        }

        private XElement GetRowAndTable(XElement element)
        {
            XElement result = null;

            while (true)
            {
                if (element.Parent == null)
                {
                    break;
                }
                else
                {
                    if (element.Parent.ToString().StartsWith("<w:tr "))
                    {
                        result = element.Parent;
                        break;
                    }

                    element = element.Parent;
                }
            }

            return result;
        }

        private void CopyStream(Stream source, Stream target)
        {
            source.Position = 0;

            int bytesRead;
            byte[] buffer = new byte[source.Length];

            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                target.Write(buffer, 0, bytesRead);
            }

            source.Position = 0;
        }

        private byte[] GetBytesFromStream(Stream source)
        {
            source.Position = 0;

            byte[] buffer = new byte[source.Length];

            source.Read(buffer, 0, buffer.Length);

            source.Position = 0;

            return buffer;
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
