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
    ///  Cancellation Orders base class
    /// </summary>
    public abstract class CancellationOrderBase : IncomingOrdersBase
    {
        #region Members

        public CancellationOrderBase() : base()
        {
        }

        protected override PermissionTypes PagePermission { get { return PermissionTypes.ABMELDEAUFTRAG_ANLEGEN; } }
        protected override OrderTypes OrderType { get { return OrderTypes.Cancellation; } }

        #endregion
    }
}