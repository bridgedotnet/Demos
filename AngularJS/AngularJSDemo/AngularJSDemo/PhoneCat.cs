using Bridge;
using Bridge.AngularJS;
using Bridge.AngularJS.Route;
using Bridge.AngularJS.Services;
using Bridge.Html5;
using Bridge.jQuery2;
using System;
using System.Collections.Generic;
using Bridge.AngularJS.Resource;

namespace PhoneCat
{
    public class PhoneCat
    {
        public PhoneCat()
        {
        }

        [Init(InitPosition.After)]
        public static void Init() {
            var appDepend = new string[]
            {
                "ngRoute",
                "phonecatControllers",
                "phonecatFilters",
                "phonecatServices",
                "phonecatAnimations"
            };
            var app = Angular.Module("phonecatApp", appDepend);

            app.Config<RouteProvider>(RouteProviderFn);

            var catCtl = Angular.Module("phonecatControllers");
            catCtl.Controller<PhoneListScopeModel,PhoneQueryModel>
                ("PhoneListCtrl", PhoneListCtrlFn);

            catCtl.Controller<PhoneDetailsScopeModel, PhoneModel, 
                PhoneQueryModel>("PhoneDetailCtrl", PhoneDetailCtrlFn);

            var catFlt = Angular.Module("phonecatFilters");

            // The following is equivalent to defining methods
            // - string mb(string text) { return "sometext"; }
            // and
            // - Func<string, string> ma() { return mb; }
            // Then calling .Filter("text", ma);
            catFlt.Filter("checkmark", () =>
            {
                return (input) =>
                {
                    return (input == "true") ? "\u2713" : "\u2718";
                };
            });

            InitServices();

            InitAnimations();
        }

        public static void InitServices()
        {
            var phonecatServices = Angular.Module("phonecatServices",
                                   new string[] { "ngResource" });

            phonecatServices.Factory<Func<Func<string, object, ResourceActions,
                Resource>, Resource>>("phoneService", PhoneServicesFactoryFn);
        }

        public static void InitAnimations() {
            var anim = Angular.Module("phonecatAnimations",
                new string[] { "ngAnimate" });

            anim.Animation(".phone", () =>
            {
                Func<jQuery, string, Action, Action<bool>> animateUp =
                    (jQuery element, string className, Action done) =>
                {
                    if (className != "active") {
                        return null;
                    }

                    element.Css(
                    new {
                        Position = Position.Absolute,
                        Top = 500,
                        Left = 0,
                        Display = Display.Block
                    });

                    element.Animate(new { Top = 0 }, 400, "swing", done);

                    return (cancel) =>
                    {
                        if (cancel)
                        {
                            element.Stop();
                        };
                    };
                };

                Func<jQuery, string, Action, Action<bool>> animateDown =
                    (jQuery element, string className, Action done) =>
                {
                    if (className != "active") {
                        return null;
                    }

                    element.Css(
                    new {
                        Position = Position.Absolute,
                        Top = 0,
                        Left = 0
                    });

                    element.Animate(new { Top = -500 }, 400, "swing", done);

                    return (cancel) =>
                    {
                        if (cancel)
                        {
                            element.Stop();
                        };
                    };
                };

                return new Bridge.AngularJS.jQuery.Animation {
                    AddClass = animateUp,
                    RemoveClass = animateDown
                };
            });
        }

        public static void RouteProviderFn(
            [Name("$routeProvider")] RouteProvider routeProvider)
        {
            routeProvider.When("/phones", new MappingInformation
            {
                TemplateUrl = "partials/phone-list.html",
                Controller = "PhoneListCtrl"
            }).When("/phones/:id", new MappingInformation
            {
                TemplateUrl = "partials/phone-detail.html",
                Controller = "PhoneDetailCtrl"
            }).Otherwise(new MappingInformation
            {
                RedirectTo = "/phones"
            });
        }

        public static void PhoneListCtrlFn(
            [Name("$scope")] PhoneListScopeModel scope,
            PhoneQueryModel phoneService) // this MUST match the service name
        {
            scope.Phones = phoneService.Query();

            scope.OrderProp = "age";
        }

        public static void PhoneDetailCtrlFn(
            [Name("$scope")] PhoneDetailsScopeModel scope,
            [Name("$routeParams")] PhoneModel routeParams,
            PhoneQueryModel phoneService) // this MUST match the service name
        {
            scope.Phone = phoneService.Get(
                new { Id = routeParams.Id },
                (phone) =>
                {
                    scope.MainImageUrl = phone.Images[0];
                }
            );

            scope.SetImage = (imageUrl) =>
            {
                scope.MainImageUrl = imageUrl;
            };
        }

        public static Resource PhoneServicesFactoryFn(
            [Name("$resource")]
            Func<string, object, ResourceActions, Resource>
            resource)
        {
            return resource("data/:id.json", new object { },
                new ResourceActions
            {
                Query = new ActionInfo()
                {
                    Method = "GET",
                    Params = new { Id = "phones" },
                    IsArray = true
                }
            });
        }
    }

    [Ignore]
    public class PhoneModel
    {
        public int Age;
        public string Id;
        public string ImageUrl;
        public string Name;
        public string Snippet;
    }

    [Ignore]
    public class PhoneListScopeModel
    {
        public PhoneModel[] Phones;
        public string OrderProp;
    }

    // We ignore this because it is just a wrapper to the service's 'query'
    // action.
    [Ignore]
    public class PhoneQueryModel
    {
        public PhoneModel[] Query() {
            return default(PhoneModel[]);
        }

        public PhoneDetailsModel Get(object phoneId,
            Action<PhoneDetailsModel> phoneTask)
        {
            return default(PhoneDetailsModel);
        }
    }

    [Ignore]
    public class PhoneDetailsScopeModel
    {
        public PhoneDetailsModel Phone;
        public string MainImageUrl;

        public Action<string> SetImage;

        /*
         * One could think on making the SetImage function like this:
         *public void SetImage(string imageUrl)
         *{
         * this.MainImageUrl = imageUrl;
         *}
         * But it will not work with AngularJS: it has to be defined on the
         * scope definition (see above).
         */
    }

    [Ignore]
    public class PhoneDetailsModel
    {
        public string Id;
        public string Name;
        public string AdditionalFeatures;
        public AndroidModel Android;
        public string[] Availability;
        public BatteryInfoModel Battery;
        public CameraInfoModel Camera;
        public ConnectivityInfoModel Connectivity;
        public string Description;
        public DisplayInfoModel Display;
        public HardwareInfoModel Hardware;
        public string[] Images;
        public SizeAndWeightInfoModel SizeAndWeight;
        public StorageInfoModel Storage;
    }

    [Ignore]
    public class AndroidModel
    {
        public string Os;
        public string Ui;
    }

    [Ignore]
    public class BatteryInfoModel
    {
        public string StandbyTime;
        public string TalkTime;
        public string Type;
    }

    [Ignore]
    public class CameraInfoModel
    {
        public string[] Features;
        public string Primart;
    }

    [Ignore]
    public class ConnectivityInfoModel
    {
        public string Bluetooth;
        public string Cell;
        public bool Gps;
        public bool Infrared;
        public string Wifi;
    }

    [Ignore]
    public class DisplayInfoModel
    {
        public string ScreenResolution;
        public string ScreenSize;
        public bool TouchScreen;
    }

    [Ignore]
    public class HardwareInfoModel
    {
        public bool Accelerometer;
        public string AudioJack;
        public string Cpu;
        public bool FmRadio;
        public bool PhysicalKeyboard;
        public string Usb;
    }

    [Ignore]
    public class SizeAndWeightInfoModel
    {
        public string[] Dimensions;
        public string Weight;
    }

    [Ignore]
    public class StorageInfoModel
    {
        public string Flash;
        public string Ram;
    }
}
