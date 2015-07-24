﻿using KVSCommon.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSDataAccess
{
    public abstract class ManagerBase
    {
        public ManagerBase()
        {
            DataContext = new DataClasses1DataContext();
        }

        public DataClasses1DataContext DataContext { get; set; }
    }
}