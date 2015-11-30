using Bridge;
using Bridge.AngularJS;
using Bridge.AngularJS.Resource;
using Bridge.AngularJS.Route;
using Bridge.Html5;
using Bridge.jQuery2;
using System;

namespace PhoneCat
{
    public class App
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

            var app = Angular.Module("phonecatApp", dependencies);

            app.Config<RouteProvider>(RouteProviderFn);

            var controllers = Angular.Module("phonecatControllers");

            controllers.Controller<PhoneListScopeModel, PhoneQueryModel>
                ("PhoneListCtrl", PhoneListCtrlFn);

            controllers.Controller<PhoneDetailsScopeModel, PhoneModel,
                PhoneQueryModel>("PhoneDetailCtrl", PhoneDetailCtrlFn);

            var filters = Angular.Module("phonecatFilters");

            // The following is equivalent to defining methods
            // - string mb(string text) { return "sometext"; }
            // and
            // - Func<string, string> ma() { return mb; }
            // Then calling .Filter("text", ma);
            filters.Filter("checkmark", () =>
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
            var services = Angular.Module("phonecatServices",
                                   new string[] { "ngResource" });

            services.Factory<Func<Func<string, object, ResourceActions,
                Resource>, Resource>>("phoneService", PhoneServicesFactoryFn);
        }

        public static void InitAnimations()
        {
            var animations = Angular.Module("phonecatAnimations",
                new string[] { "ngAnimate" });

            animations.Animation(".phone", () =>
            {
                Func<jQuery, string, Action, Action<bool>> animateUp =
                    (jQuery element, string className, Action done) =>
                    {
                        if (className != "active")
                        {
                            return null;
                        }

                        element.Css(
                        new
                        {
                            Position = Position.Absolute,
                            Top = 500,
                            Left = 0,
                            Display = Display.Block
                        });

                        element.Animate(new
                        {
                            Top = 0
                        }, 400, "swing", done);

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
                        if (className != "active")
                        {
                            return null;
                        }

                        element.Css(
                        new
                        {
                            Position = Position.Absolute,
                            Top = 0,
                            Left = 0
                        });

                        element.Animate(new
                        {
                            Top = -500
                        }, 400, "swing", done);

                        return (cancel) =>
                        {
                            if (cancel)
                            {
                                element.Stop();
                            };
                        };
                    };

                return new Bridge.AngularJS.jQuery.Animation
                {
                    AddClass = animateUp,
                    RemoveClass = animateDown
                };
            });
        }

        public static void RouteProviderFn([Name("$routeProvider")] RouteProvider provider)
        {
            provider.When("/phones", new MappingInformation
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
                new
                {
                    Id = routeParams.Id
                },
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
            return resource("data/:id.json", new object
            {
            },
                new ResourceActions
            {
                Query = new ActionInfo()
                {
                    Method = "GET",
                    Params = new
                    {
                        Id = "phones"
                    },
                    IsArray = true
                }
            });
        }
    }
}