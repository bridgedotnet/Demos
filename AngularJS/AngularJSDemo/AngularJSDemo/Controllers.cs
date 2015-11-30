using Bridge.AngularJS;

namespace PhoneCat
{
    public partial class App
    {
        public static void InitControllers()
        {
            var controllers = Angular.Module("phonecatControllers");

            controllers.Controller<PhoneListScopeModel, PhoneQueryModel>
                ("PhoneListCtrl", App.PhoneListCtrlFn);

            controllers.Controller<PhoneDetailsScopeModel, PhoneModel,
                PhoneQueryModel>("PhoneDetailCtrl", App.PhoneDetailCtrlFn);
        }
    }
}