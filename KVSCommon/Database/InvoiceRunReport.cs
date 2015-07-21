using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    partial class InvoiceRunReport : ILogging
    {
        public DataClasses1DataContext LogDBContext
        {
            get;
            set;
        }

        public object ItemId
        {
            get
            {
                return this.Id;
            }
        }

        public EntityState EntityState
        {
            get;
            set;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnInvoiceTypeIdChanging(int? value)
        {
            this.WriteUpdateLogItem("InvoiceTypeId", this.InvoiceTypeId, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCreateDateChanging(DateTime? value)
        {
            this.WriteUpdateLogItem("CreateDate", this.CreateDate, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCustomerIdChanging(int? value)
        {
            this.WriteUpdateLogItem("CustomerId", this.CustomerId, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnFinishedDateChanging(DateTime? value)
        {
            this.WriteUpdateLogItem("FinishedDate", this.FinishedDate, value);
        }
        partial void OnCreated()
        {
            this.EntityState = Database.EntityState.New;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnLoaded()
        {
            this.EntityState = Database.EntityState.Loaded;
        }



    }
}
