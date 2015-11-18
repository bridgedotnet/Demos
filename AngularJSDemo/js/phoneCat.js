/* global Bridge */

"use strict";

Bridge.define('PhoneCat.PhoneCat', {
    statics: {
        init: function () {
            var appDepend = ["ngRoute", "phonecatControllers", "phonecatFilters", "phonecatServices", "phonecatAnimations"];
            var app = angular.module("phonecatApp", appDepend);

            app.config(PhoneCat.PhoneCat.routeProviderFn);

            var catCtl = angular.module("phonecatControllers", []);
            catCtl.controller("PhoneListCtrl", PhoneCat.PhoneCat.phoneListCtrlFn);

            catCtl.controller("PhoneDetailCtrl", PhoneCat.PhoneCat.phoneDetailCtrlFn);

            var catFlt = angular.module("phonecatFilters", []);

            // The following is equivalent to defining methods
            // - string mb(string text) { return "sometext"; }
            // and
            // - Func<string, string> ma() { return mb; }
            // Then calling .Filter("text", ma);
            catFlt.filter("checkmark", function () {
                return function (input) {
                    return (input === "true") ? "✓" : "✘";
                };
            });

            PhoneCat.PhoneCat.initServices();

            PhoneCat.PhoneCat.initAnimations();
        },
        initServices: function () {
            var phonecatServices = angular.module("phonecatServices", ["ngResource"]);

            phonecatServices.factory("phoneService", PhoneCat.PhoneCat.phoneServicesFactoryFn);
        },
        initAnimations: function () {
            var anim = angular.module("phonecatAnimations", ["ngAnimate"]);

            anim.animation(".phone", function () {
                var animateUp = function (element, className, done) {
                    if (className !== "active") {
                        return null;
                    }

                    element.css({ position: "absolute", top: 500, left: 0, display: "block" });

                    element.animate({ top: 0 }, 400, "swing", done);

                    return function (cancel) {
                        if (cancel) {
                            element.stop();
                        }
                        ;
                    };
                };

                var animateDown = function (element, className, done) {
                    if (className !== "active") {
                        return null;
                    }

                    element.css({ position: "absolute", top: 0, left: 0 });

                    element.animate({ top: -500 }, 400, "swing", done);

                    return function (cancel) {
                        if (cancel) {
                            element.stop();
                        }
                        ;
                    };
                };

                return { addClass: animateUp, removeClass: animateDown };
            });
        },
        routeProviderFn: function ($routeProvider) {
            $routeProvider.when("/phones", { templateUrl: "partials/phone-list.html", controller: "PhoneListCtrl" }).when("/phones/:id", { templateUrl: "partials/phone-detail.html", controller: "PhoneDetailCtrl" }).otherwise({ redirectTo: "/phones" });
        },
        phoneListCtrlFn: function ($scope, phoneService) {
            $scope.phones = phoneService.query();

            $scope.orderProp = "age";
        },
        phoneDetailCtrlFn: function ($scope, $routeParams, phoneService) {
            $scope.phone = phoneService.get({ id: $routeParams.id }, function (phone) {
                $scope.mainImageUrl = phone.images[0];
            });

            $scope.setImage = function (imageUrl) {
                $scope.mainImageUrl = imageUrl;
            };
        },
        phoneServicesFactoryFn: function ($resource) {
            return $resource("data/:id.json", { }, { query: { method: "GET", params: { id: "phones" }, isArray: true } });
        }
    },
    constructor: function () {
    }
});
PhoneCat.PhoneCat.init();

