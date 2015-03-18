using System;
using System.Web;
using System.Web.Routing;

namespace Demo.WebForms
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
        }

        protected void RegisterRoutes(RouteCollection routes)
        {
            routes.MapPageRoute("Bootstrap",
                "Bootstrap",
                "~/Bootstrap.aspx");

            routes.MapPageRoute("Html5",
                "Html5",
                "~/Html5.aspx");

            routes.MapPageRoute("jQuery",
                "jQuery",
                "~/jQuery.aspx");

            routes.MapPageRoute("Submit",
                "Submit",
                "~/Submit.aspx");

            routes.Add(new Route("Data/GetServerTime", new GetSeverTimeRouteHandler()));
            routes.Add(new Route("Data/LongRunningProcess", new LongRunningProcessRouteHandler()));

            routes.MapPageRoute("Default",
                "",
                "~/Default.aspx");
        }
    }

    public class GetSeverTimeRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext context)
        {
            return new GetServerTime();
        }
    }

    public class LongRunningProcessRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext context)
        {
            return new LongRunningProcess();
        }
    }
}