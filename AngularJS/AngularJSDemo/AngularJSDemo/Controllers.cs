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
        public static void InitControllers()
        {
            var controllers = Angular.Module("phonecatControllers");

            controllers.Controller<PhoneListScopeModel, PhoneQueryModel>
                ("PhoneListCtrl", App.PhoneListCtrlFn);


            //[Name("$scope")] PhoneListScopeModel scope, PhoneQueryModel phoneService

            controllers.Controller<PhoneDetailsScopeModel, PhoneModel,
                PhoneQueryModel>("PhoneDetailCtrl", App.PhoneDetailCtrlFn);
        }
    }
}