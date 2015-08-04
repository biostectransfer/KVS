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
            System.Windows.Forms.MessageBox.Show(licenceNumber);
        }
    }
}
