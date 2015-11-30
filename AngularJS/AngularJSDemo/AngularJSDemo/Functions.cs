using Bridge;
using Bridge.AngularJS.Resource;
using Bridge.AngularJS.Route;
using System;

namespace PhoneCat
{
    public partial class App
    {
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