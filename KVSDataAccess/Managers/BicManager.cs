using KVSCommon.Database;
using KVSCommon.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSDataAccess
{
    public class BicManager : ManagerBase, IBicManager
    {
        public BIC_DE GetBicByCodeAndName(string code, string name)
        {
            return DataContext.BIC_DE.FirstOrDefault(q =>
                        q.Bankleitzahl.Contains(code) &&
                        (q.Bezeichnung.Contains(name) || q.Kurzbezeichnung.Contains(name)));
        }
    }
}
