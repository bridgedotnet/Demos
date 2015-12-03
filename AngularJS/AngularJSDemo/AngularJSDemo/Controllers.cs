using Bridge.AngularJS;
using System;

namespace PhoneCat
{
    public partial class App
    {
        public static void InitControllers()
        {
            var controllers = Angular.Module("phonecatControllers");

            /*
             * If passing only the reference to the function to the
             * PhoneListCtrl, this is how it would be done.
             *
            controllers.Controller < PhoneListScopeModel, PhoneQueryModel >
                ("PhoneListCtrl", App.PhoneListCtrlFn);
            */

            controllers.Controller
                ("PhoneListCtrl", Angular.Fn((Action<PhoneListScopeModel, PhoneQueryModel>)
                ((scope, phoneService) =>
                {
                    scope.Phones = phoneService.Query();
                    scope.OrderProp = "age";
                }
                ), "$scope", "phoneService"));

            controllers.Controller<PhoneDetailsScopeModel, PhoneModel,
                PhoneQueryModel>("PhoneDetailCtrl", App.PhoneDetailCtrlFn);
        }
    }
}