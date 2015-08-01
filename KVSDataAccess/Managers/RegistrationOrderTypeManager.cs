using KVSCommon.Database;
using KVSCommon.Entities;
using KVSCommon.Enums;
using KVSCommon.Managers;
using KVSDataAccess.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSDataAccess.Managers
{
    public partial class RegistrationOrderTypeManager : EntityManager<RegistrationOrderType, int>, IRegistrationOrderTypeManager
    {
        public RegistrationOrderTypeManager(IKVSEntities context) : base(context) { }

    }
}
