using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Demo.MVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Bootstrap",
                url: "Bootstrap",
                defaults: new { controller = "Home", action = "Bootstrap" }
            );

            routes.MapRoute(
                name: "Html5",
                url: "Html5",
                defaults: new { controller = "Home", action = "Html5" }
            );

            routes.MapRoute(
                name: "jQuery",
                url: "jQuery",
                defaults: new { controller = "Home", action = "jQuery" }
            );

            routes.MapRoute(
                name: "Submit",
                url: "Submit",
                defaults: new { controller = "Home", action = "Submit" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}