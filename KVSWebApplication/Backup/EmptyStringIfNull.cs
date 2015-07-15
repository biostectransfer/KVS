using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
namespace KVSWebApplication
{
    /// <summary>
    /// Hilfklasse fuer Pruefmethoden
    /// </summary>
    public static class EmptyStringIfNull
    {
        /// <summary>
        /// Loesche alle Leerzeichen und Zeilenumbrueche
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveLineEndings(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return value.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(lineSeparator, string.Empty).Replace(paragraphSeparator, string.Empty);
        }
        /// <summary>
        /// Gibt einen Leerstring, wenn das Objekt null ist
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public static string ReturnEmptyStringIfNull(object myValue)
        {
            if (myValue == null)
                return "";
            else
                return myValue.ToString();
        }
        /// <summary>
        /// Gibt einen 0 String zurueck, wenn das Objekt null oder leer ist
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public static string ReturnZeroStringIfNull(string myValue)
        {
            if (myValue == string.Empty || myValue ==  null)
                return "0";
            else
                return myValue.ToString();
        }
        /// <summary>
        /// Wenn das Objekt null ist, wird auch null zurueck gegeben. Wenn das Pasen in ein Int nich moeglich ist, wird eine Exception geworfen (Gedacht fuer das Zahlungsziel)
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public static int? ReturnNullableInteger(object myValue)
        {
            if (myValue == null)
                return null;            
            else
            {
                try
                {
                    return int.Parse(myValue.ToString());
                }
                catch
                {
                    throw new Exception("Das Zahlungsziel muss eine Gleitkommazahl darstellen");
                }
            }  
        }
        /// <summary>
        /// Pruefe ob der angegebene String auch ein Integer ist
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public static bool IsNumber(string myValue)
        {         
            try
            {
                decimal.Parse(myValue);
                return true;
            }
            catch
            {
                return false;
            }           
        }
        /// <summary>
        /// Pruefe ob der angegebener String auch eine Guid ist
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public static bool IsGuid(string myValue)
        {
            try
            {
                Guid.Parse(myValue);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Gibt 0 zurueck, wenn der eingegebene Wert null ist
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public static string ReturnZeroStringIfNullObject(object myValue)
        {
            if (myValue == null)
                return "0";
            else
                return myValue.ToString();
        }
        /// <summary>
        /// Gibt eine 0 zurueck, wenn die Eingabe ein null ist
        /// Wenn die Eingabe nicht als Zahl geparst werden kann, dann wird eine Exception geworfen (Gedacht für MwSt)
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public static decimal ReturnZeroDecimalIfNullEditVat(object myValue)
        {
            if (myValue == null)
                return 0;
            else
            {
                decimal myVat = 0;
                myValue = myValue.ToString().Trim();
                myValue = myValue.ToString().Replace('.', ',');              
                try { myVat = decimal.Parse(myValue.ToString()); }
                catch { throw new Exception("Die MwSt muss eine Dezimalzahl darstellen!"); }
                return myVat;
            }
        }
        /// <summary>
        /// Prueft ob der angegebener Order existiert und erstellt die Verzeichnisse
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="isDocket"></param>
        /// <returns></returns>
        public static string CheckIfFolderExistsAndReturnPathForPdf(string currentUserId, bool isDocket=false)
        {
            string newPdfPathAndName = "";
            if (!Directory.Exists(ConfigurationManager.AppSettings["DataPath"]))
            {
                Directory.CreateDirectory(ConfigurationManager.AppSettings["DataPath"]);
            }
            if (!Directory.Exists(ConfigurationManager.AppSettings["DataPath"] + "\\UserData"))
            {
                Directory.CreateDirectory(ConfigurationManager.AppSettings["DataPath"] + "\\UserData");
            }
            if (!Directory.Exists(ConfigurationManager.AppSettings["DataPath"] + "\\UserData\\" + currentUserId))
            {
                Directory.CreateDirectory(ConfigurationManager.AppSettings["DataPath"] + "\\UserData\\" + currentUserId);
            }
            newPdfPathAndName = ConfigurationManager.AppSettings["DataPath"] + "\\UserData\\" + currentUserId + "\\"+ ((!isDocket)?"Lieferschein_":"Laufzettel_") + DateTime.Today.Day + "_" + DateTime.Today.Month + "_" + DateTime.Today.Year + "_" + Guid.NewGuid() + ".pdf";
            return newPdfPathAndName;
        }
        /// <summary>
        /// Gibt eine neue Zahl zurueck, z. B. fuer IDs
        /// </summary>
        /// <param name="myCurrNumber"></param>
        /// <returns></returns>
        public static string generateIndividualNumber(string myCurrNumber)
        {
            if (IsNumber(myCurrNumber))
            {
                return (double.Parse(myCurrNumber) + 1).ToString();
            }
            else
            {
                return myCurrNumber + "1";
            }
        }
    }
}