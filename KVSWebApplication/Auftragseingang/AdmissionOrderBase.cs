using KVSCommon.Database;
using KVSCommon.Enums;
using KVSCommon.Managers;
using KVSWebApplication.BasePages;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace KVSWebApplication.Auftragseingang
{
    /// <summary>
    ///  Admission Orders base class
    /// </summary>
    public abstract class AdmissionOrderBase : IncomingOrdersBase
    {
        #region Members

        public AdmissionOrderBase() : base()
        {
        }

        protected override PermissionTypes PagePermission { get { return PermissionTypes.ZULASSUNGSAUFTRAG_ANLEGEN; } }
        protected override OrderTypes OrderType { get { return OrderTypes.Admission; } }

        #endregion
    }
}