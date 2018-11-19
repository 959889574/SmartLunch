using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NLog;
using SL.CAS;
 

namespace SL.CAS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 应用程序入口点
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            AutoFacConfig.RegisterAutofac();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
