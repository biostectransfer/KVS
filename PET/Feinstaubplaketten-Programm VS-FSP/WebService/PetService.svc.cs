using KVSContracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Threading.Tasks;


namespace PET.WebService
{
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    //[ErrorBehavior(typeof(ErrorHandler))]
    public class PetService : IPrintService
    {
        public PetService()
        {
        }

        public async void EmissionBadgePrint(string licenceNumber)
        {
            //await 
            //System.Windows.Forms.MessageBox.Show(licenceNumber);
            string additionalInfo = System.Configuration.ConfigurationManager.AppSettings["AdditionalInfo"];
            //Etticket wird gedruckt------------------------------------------------------
            //UTF8Encoding utf8 = new UTF8Encoding();
            //byte[] encodedBytes = utf8.GetBytes(strKennzeichen);
            //strKennzeichen = utf8.GetString(encodedBytes);
            //wird Abstand für Kennz. und Zusätz. berechnet um den zweiten Satz zentriert auszudrucken
            string printString = "m m\r\nJ\r\nH 50\r\nO T5\r\nD 0,3\r\nS l2;0,4,99,99,103\r\nT 23,65,0,5,9.85,q88;[J:c57]" + licenceNumber.ToUpper() + "\r\nT 23,69,0,5,2.85;[J:c57]" + additionalInfo + "\r\nA 1\r\n";
            string cabName = System.Configuration.ConfigurationManager.AppSettings["cabName"];
            RawPrinterHelper.SendStringToPrinter(cabName, printString);
        }
    }
}
