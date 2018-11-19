using System.Web.Mvc;
using System.Web.Routing;

namespace SL.CAS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "website",
            //    url: "{action}",
            //    defaults: new { controller = "Default" }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "AppExhibition", action = "Index" }
            );

        }
    }
}
