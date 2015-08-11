using KVSConfiguration;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Unity.WebApi;

namespace KVSWebApplication
{
    public class KVSWebApplicationInstance : HttpApplication
    {
        protected void Application_Start()
        {
            var container = new UnityContainer();
            UnityConfiguration.ConfigureContainer(container);
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

            Spire.Barcode.BarcodeSettings.ApplyKey("SB4UT-1SE20-3T96D-50CTF-K878A");
        }
    }
}