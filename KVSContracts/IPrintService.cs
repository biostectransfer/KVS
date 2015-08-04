using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace KVSContracts
{
    [ServiceContract]
    public interface IPrintService
    {
        [OperationContract]
        void EmissionBadgePrint(string licenceNumber);
    }
}
