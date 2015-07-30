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
    public partial class RegistrationManager : EntityManager<Registration, int>, IRegistrationManager
    {
        public RegistrationManager(IKVSEntities context) : base(context) { }
    }
}
