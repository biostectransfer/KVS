using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;
namespace Zulassungssoftware_Webservice.helper
{
    /// <summary>
    /// Hilfsklasse fuer die XML Dateien
    /// </summary>
    public class XMLHelper
    {
        private string xsdPath = ConfigurationManager.AppSettings["XSDPath"];
        private string errorPath = ConfigurationManager.AppSettings["ErrorPath"];
        private string archivPath = ConfigurationManager.AppSettings["ArchivFolder"];
        private string namespacePath = ConfigurationManager.AppSettings["currentNamespace"];

        TextWriter textWriter;
        XmlTextReader tr;
        /// <summary>
        /// Gibt den XML Dateinamen zurueck
        /// </summary>
        /// <param name="type">type</param>
        /// <returns>XML DATEINAME</returns>
        private string GetXMlFileName(string type)
        {

            return type + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + "_" + DateTime.Now.Ticks + ".xml";

        }
        /// <summary>
        /// Validiert die XML Datei
        /// </summary>
        /// <param name="XMLfilepath">Dateipfad</param>
        /// <param name="xsdpath">XSD Pfad</param>
        /// <returns>string</returns>
        private string validateXML(string XMLfilepath, string xsdpath)
        {
            tr = new XmlTextReader(XMLfilepath);
            XmlValidatingReader vr = new XmlValidatingReader(tr);
            vr.ValidationType = ValidationType.Schema;
            vr.Schemas.Add(namespacePath, xsdpath);


            while (vr.Read()) ;

            vr.Close();
            tr.Close();
            return XMLfilepath;

        }

/// <summary>
/// Serialisiert und speichert die XML Datei
/// </summary>
/// <param name="data">XML Objekt</param>
/// <param name="type">Auftragstyp</param>
/// <returns></returns>
        public string XmlSerializeAndSave(object data, OrderTypes type)
        {
            string filename = "";
            try
            {
                if (OrderTypes.VehicleRegistration == type || OrderTypes.VehicleReadmission == type || OrderTypes.VehicleDeregistration == type || OrderTypes.VehicleRemark == type)
                {
                    if (Directory.Exists(archivPath) == false) { Directory.CreateDirectory(archivPath); }
                    if (Directory.Exists(archivPath + "\\" + type) == false) { Directory.CreateDirectory(archivPath + "\\" + type); }

                    filename = archivPath + "\\" + type + "\\" + GetXMlFileName(type + "_");

                     XmlSerializer serializer = null;
                     if (type == OrderTypes.VehicleRegistration)
                     {
                         serializer = new XmlSerializer(typeof(Registration));
                         textWriter = new StreamWriter(filename);
                         serializer.Serialize(textWriter, data);
                         textWriter.Close();
                         return validateXML(filename, xsdPath + "\\VehicleRegistration.xsd");

                     }
                     else if (type == OrderTypes.VehicleReadmission)
                     {
                         serializer = new XmlSerializer(typeof(Readmission));
                         textWriter = new StreamWriter(filename);
                         serializer.Serialize(textWriter, data);
                         textWriter.Close();
                         return validateXML(filename, xsdPath + "\\VehicleReadmission.xsd");
                     }
                     else if (type == OrderTypes.VehicleDeregistration)
                     {
                         serializer = new XmlSerializer(typeof(Deregistration));
                         textWriter = new StreamWriter(filename);
                         serializer.Serialize(textWriter, data);
                         textWriter.Close();
                         return validateXML(filename, xsdPath + "\\VehicleDeregistration.xsd");
                     }
                     else if (type == OrderTypes.VehicleRemark)
                     {
                         serializer = new XmlSerializer(typeof(Remark));
                         textWriter = new StreamWriter(filename);
                         serializer.Serialize(textWriter, data);
                         textWriter.Close();
                         return validateXML(filename, xsdPath + "\\VehicleRemark.xsd");
                     }



                     return "";
                }
                else
                    throw new Exception("Fehler beim Validieren der XML Datei");

            }
            catch (Exception ex)
            {
                tr.Close();
                textWriter.Close();

                if (File.Exists(filename))
                {
                    if (!Directory.Exists(errorPath)) { Directory.CreateDirectory(errorPath); }

                    if (!(File.Exists(errorPath + "\\" + new FileInfo(filename).Name)))
                    {
                        File.Move(filename, errorPath + "\\" + new FileInfo(filename).Name);
                    }
                    else
                    {
                        File.Delete(filename);

                    }

                }
                return ex.Message;

            }



        }
        //public Angebotsdaten getFileAndDeserialized(string filename)
        //{
        //    Angebotsdaten angebotsdaten = null;
        //    XmlSerializer serializer = new XmlSerializer(typeof(Angebotsdaten));
        //    StreamReader reader = new StreamReader(filename);
        //    angebotsdaten = (Angebotsdaten)serializer.Deserialize(reader);
        //    reader.Close();

        //    return angebotsdaten;


        //}
    }
}