using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KVSCommon.Database;
using System.Configuration;
namespace InvoiceRunner
{
    class Program
    {
        static void Main(string[] args)
        {
           
            try
            {
                    InvoiceRunPilot p = new InvoiceRunPilot(new Guid(ConfigurationManager.AppSettings["InvoicePilotUser"]));
                    p.RunPilot();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Achtung, Fehler beim Rechnungslauf: " + ex.Message);
                Console.ReadLine();

            }
           
        }
        


    }
}
