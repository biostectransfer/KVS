using KVSCommon.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IBicManager
    {
        BIC_DE GetBicByCodeAndName(string code, string name);
    }
}
