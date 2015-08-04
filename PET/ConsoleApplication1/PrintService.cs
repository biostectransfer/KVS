using KVSContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ConsoleApplication1
{
    public class PrintService : IPrintService
    {
        public void EmissionBadgePrint(string licenceNumber)
        {
            Console.WriteLine("Print Kennzeichen: " + licenceNumber);
        }
    }
}
