using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
namespace InvoiceRunner
{
    /// <summary>
    /// Loggerklasse
    /// </summary>
    public class Logger
    {
        static string filepath = ConfigurationManager.AppSettings["LOGPATH"];
        static string filename;
        List<string> errorMessages = new List<string>();
        List<string> companyMessages = new List<string>();
        /// <summary>
        /// Standartkonstruktor um die Logger Klasse zu instanzieren
        /// </summary>
        public Logger()
        {
            errorMessages.Clear();
            companyMessages.Clear();
            if (!Directory.Exists(filepath)) Directory.CreateDirectory(filepath) ;
            filename = DateTime.Now.Date.ToShortDateString() + ".log";
            if (!File.Exists(filepath + "\\" + filename))
            {
                File.Create(filepath + "\\" + filename);
            }
        }
        /// <summary>
        /// Erstelle der Reportpositionen, die fuer den Kunden relevant sind
        /// </summary>
        /// <param name="message"></param>
        public void AddcompanyMessages(string message)
        {
            companyMessages.Add(DateTime.Now + ": " + message);
        }
        /// <summary>
        /// Neuer Logeintrag mit einer Infomessage
        /// </summary>
        /// <param name="message">Message</param>
        public void Info(string message)
        {
            try
            {
            
                using(StreamWriter wr = new StreamWriter(filepath+"\\"+filename, true))
                {
                    wr.WriteLine("[INFO] " + DateTime.Now + ": " + message);
                    wr.Close();
                }
            }
            catch {}
        }
        /// <summary>
        /// Neuer Logeintrag mit Fehlermeldung
        /// </summary>
        /// <param name="message">Message</param>
        public void Error(string message)
        {
            try
            {
                errorMessages.Add(message);
                using (StreamWriter wr = new StreamWriter(filepath + "\\" + filename, true))
                {
                    wr.WriteLine("[Error] " + DateTime.Now + ": " + message);
                    wr.Close();
                }

            }catch{}
        }
        /// <summary>
        /// Gibt den kompletten Fehlerreport zurueck
        /// </summary>
        /// <returns></returns>
        public List<string> returnErrorMessagesComplete() { return errorMessages; }
        /// <summary>
        /// Gibt die komplette Reportinfos zum versenden zurueck
        /// </summary>
        /// <returns></returns>
        public List<string> returnCompanyMessagesComplete() { return companyMessages; }
        /// <summary>
        /// Pruefe ob der Job momentan lauft
        /// </summary>
        /// <returns></returns>
        public static bool isActive()
        {
            if (File.Exists(filepath + "\\active.txt"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Erstelle eine neue Active TXT um einen erneuten Job Start zu verhindern
        /// </summary>
        public static void createActiveTxt()
        {
            using (StreamWriter wr = new StreamWriter(filepath + "\\active.txt", true))
            {
                wr.WriteLine("[Info] " + DateTime.Now + ": Rechnungslauf aktiv");
                wr.Close();
            }
            
        }
        /// <summary>
        /// Loesche die Active TXT, wenn der Job beendet wird
        /// </summary>
        public static void removeActiveTxt()
        {
            File.Delete(filepath + "\\active.txt");
        }
    }
}