using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zulassungssoftware_Webservice.helper
{


    public class RegTypes
    {

        private RegTypes() { }

        public const string Neuzulassung = "1DF35583-034F-42B9-A3AC-EBD07835B14B";
        public const string Wiederzulassung = "FEEBA978-D01C-40A8-A212-7CB6BB6E74D9";
        public const string Umkennzeichnung = "03C828A1-CB90-4A32-AE0E-8B367A1259DD";
     
    }
       public enum OrderTypes
       {
           VehicleRegistration,
           VehicleReadmission,
           VehicleDeregistration,
           VehicleRemark
       };

}