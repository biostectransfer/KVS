using KVSCommon.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSCommon.SaveActors
{
    public interface ISaveActorBase
    {
        bool NeedDoAction(object entity, EntityState state);
        void DoAction(object entity, EntityState state);
    }
}
