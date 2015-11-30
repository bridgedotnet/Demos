using Bridge;
using Bridge.AngularJS;
using Bridge.AngularJS.Route;
using Bridge.Html5;

namespace PhoneCat
{
    public partial class App
    {
        public App()
        {
        }

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

            Angular.Module("phonecatApp", dependencies)
                   .Config<RouteProvider>(App.RouteProviderFn);

            App.InitControllers();
            App.InitFilters();
            App.InitServices();
            App.InitAnimations();
        }
    }
}