using KVSCommon.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSCommon.Database
{
    public partial class LargeCustomerRequiredField : IHasId<int>, IRemovable, ISystemFields
    {
        public int Id
        {
            get
            {
                return LargeCustomerId;
            }
            set
            {
                LargeCustomerId = value;
            }
        }
    }
}
