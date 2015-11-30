using Bridge;
using Bridge.AngularJS;
using Bridge.AngularJS.Resource;
using Bridge.AngularJS.Route;
using Bridge.Html5;
using Bridge.jQuery2;
using System;

namespace PhoneCat
{
    public partial class App
    {
        public static void InitServices()
        {
            var services = Angular.Module("phonecatServices",
                                   new string[] { "ngResource" });

            services.Factory<Func<Func<string, object, ResourceActions,
                Resource>, Resource>>("phoneService", PhoneServicesFactoryFn);
        }
    }
}