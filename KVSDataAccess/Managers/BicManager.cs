using KVSCommon.Database;
using KVSCommon.Entities;
using KVSCommon.Managers;
using KVSDataAccess.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSDataAccess.Managers
{
    public partial class BicManager : EntityManager<BIC_DE, int>
        , IBicManager
    {

        public BicManager(IKVSEntities context) : base(context) { }

        public BIC_DE GetBicByCodeAndName(string code, string name)
        {
            return DataContext.GetSet<BIC_DE>().FirstOrDefault(q =>
                        q.Bankleitzahl.Contains(code) &&
                        (q.Bezeichnung.Contains(name) || q.Kurzbezeichnung.Contains(name)));
        }
    }
}
