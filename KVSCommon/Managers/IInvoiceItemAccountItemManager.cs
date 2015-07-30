﻿using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IInvoiceItemAccountItemManager : IEntityManager<InvoiceItemAccountItem, int>
    {
        void CreateAccounts(Invoice invoice);
    }
}
