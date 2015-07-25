using KVSCommon.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSCommon.SaveActors
{
    public interface ISaveActorManagerBase
    {
        void DoBeforeSaveAction(object entity, EntityState state);
    }
}
