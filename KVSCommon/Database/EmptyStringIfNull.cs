using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KVSCommon.Database
{
    /// <summary>
    /// Hilfklasse um eingegebene Felder zu pruefen
    /// </summary>
    public static class EmptyStringIfNull
    {
        /// <summary>
        /// Gibt einen Leeren String zurueck, wenn das Objekt null ist
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
        /// Gibt eine 0 zurueck, wenn das Objekt null ist
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public static string ReturnZeroStringIfNull(string myValue)
        {
            if (myValue == string.Empty || myValue == null)
                return "0";
            else
                return myValue.ToString();

        }
        /// <summary>
        /// Gibt ein null zurück, wenn das Objekt auch null ist (Gedacht für die Zahlungsziele)
        /// Wenn keine Null und keine Zahl, dann wirft eine Exception 
        /// Wenn Eine Zahl, dann gibt ein Int zurueck
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
        /// Prueft ob die einegegebene Zahl auch eine nummer ist
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
    }
}