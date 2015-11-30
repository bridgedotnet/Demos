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
        public App() { }

        [Init(InitPosition.After)]
        public static void Init()
        {
            var dependencies = new string[]
            {
                "ngRoute",
                "phonecatControllers",
                "phonecatFilters",
                "phonecatServices",
                "phonecatAnimations"
            };

            var __scope = "testing";

            Console.Log(__scope);

            Angular.Module("phonecatApp", dependencies)
                   .Config<RouteProvider>(App.RouteProviderFn);

            App.InitControllers();
            App.InitFilters();
            App.InitServices();
            App.InitAnimations();
        }
    }
}