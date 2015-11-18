/* global Bridge */

"use strict";

/** @namespace AngularJSDemo */

/**
 * Checkpoints:
 Alpha Bravo Charlie Delta Echo Foxtrot Golf Hotel India
 Juliet Kilo Lima Mike November Oscar Papa Quebec Romeo
 Sierra Tango Uniform Victor Whiskey X-ray Yankee Zulu
 http://home.earthlink.net/~malcolmhamer/alpha.htm
 *
 * @public
 * @class AngularJSDemo.AngularJSDemo
 */
Bridge.define('AngularJSDemo.AngularJSDemo', {
    statics: {
        constructor: function () {
            console.log("Checkpoint Yankee: [ctor] nothng at this point will like AngularJS.");
        },
        hwbApp: null,
        config: {
            init: function () {
                Bridge.ready(this.updateControls);
            }
        },
        /**
         * AngularJS requires to be defined on document parse.
         TODO: create an onParse attribute
         *
         * @static
         * @public
         * @this AngularJSDemo.AngularJSDemo
         * @memberof AngularJSDemo.AngularJSDemo
         * @return  {void}        
         */
        startUp: function () {
            console.log("Checkpoint Charlie: I must be printed BEFORE Checkpoint 1.");

            AngularJSDemo.AngularJSDemo.hwbApp = angular.module("hwbApp", []);

            var controllerData = { message: "Hello, AngularJS message defined in Bridge's C#! :D" };
            AngularJSDemo.AngularJSDemo.hwbApp.controller("hwbctl", ['$scope', function ($scope) { for (var key in controllerData) $scope[key] = controllerData[key] }]);
            AngularJSDemo.AngularJSDemo.hwbApp.directive("brdEntryPoint", AngularJSDemo.AngularJSDemo.dynMehTemplate);

            var checkpoints = Bridge.merge(new Bridge.List$1(Object)(), [
                [{ callsign: "Alpha", id: 98 }], 
                [{ callsign: "Baker", id: 78 }], 
                [{ callsign: "Charlie", id: 58 }], 
                [{ callsign: "Delta", id: 9 }]
            ] );

            var controllerStrongData = Bridge.merge(new AngularJSDemo.AngularJSDemo.ControllerDataObjectStructure(), {
                message: "Hello, AngularJS message defined in Bridge's C#'s strongly typed class! :D", 
                foo: "Foo fighters from strong C#.", 
                bar: "Strong C# 777 slot", 
                checkpoints: checkpoints.toArray()
            } );

            AngularJSDemo.AngularJSDemo.hwbApp.controller("hwbSctl", ['$scope', function ($scope) { for (var key in controllerStrongData) $scope[key] = controllerStrongData[key] }]);
            AngularJSDemo.AngularJSDemo.hwbApp.directive("brdEntryPointForThree", AngularJSDemo.AngularJSDemo.threeWayFunction);

            document.documentElement.setAttribute('ng-app', AngularJSDemo.AngularJSDemo.hwbApp.name);
        },
        /**
         * This function determines the format of the contents of the entry
         point where the dynamic code will be injected.
         *
         * @static
         * @public
         * @this AngularJSDemo.AngularJSDemo
         * @memberof AngularJSDemo.AngularJSDemo
         * @return  {Object}        
         */
        dynMehTemplate: function () {
            var span = document.createElement('span');
            span.setAttribute('ng-controller', "hwbctl");
            span.innerHTML = "AJS says by directive: [{{message}}]";
            return { template: span.outerHTML };
        },
        threeWayFunction: function () {
            var span = document.createElement('span');
            span.setAttribute('ng-controller', "hwbSctl");
            span.innerHTML = "AJS for three scopined: [msg:{{message}}][foo:{{foo}}][bar:{{bar}}]";
            return { template: span.outerHTML };
        },
        startUpIdPhase: function () {
            if (AngularJSDemo.AngularJSDemo.hwbApp === null) {
                AngularJSDemo.AngularJSDemo.startUp();
            }

            var x = document.getElementById("entryPoint");
            if (x !== null) {
                //x.SetAttribute("ng-controller", "hwbctl"); // same as below
                x.setAttribute('ng-controller', "hwbctl");
                x.innerHTML = "AJS says by element ID: [{{message}}]";
            }

            document.body.appendChild(AngularJSDemo.AngularJSDemo.getRepeatRegion());

            // Broken :(
            AngularJSDemo.AngularJSDemo.defineNiceController();
        },
        getRepeatRegion: function () {
            var itemsSpan = document.createElement('span');
            itemsSpan.innerHTML = "Checkpoint";
            var itemsPara = document.createElement('p');
            itemsPara.innerHTML = "{{checkpoint.callsign}}[{{checkpoint.id}}]";

            var itemsLI = document.createElement('li');
            itemsLI.setAttribute('ng-repeat', '' + "checkpoint" + ' in ' + "checkpoints");
            itemsLI.appendChild(itemsSpan);
            itemsLI.appendChild(itemsPara);

            var itemsUL = document.createElement('ul');
            itemsUL.appendChild(itemsLI);

            var itemsSubSpan = Bridge.merge(document.createElement('span'), {
                innerHTML: "[{{checkpoint.callsign}}.{{checkpoint.id}}] "
            } );

            var itemsSearchBox = document.createElement('input');
            itemsSearchBox.setAttribute('ng-model', "cpFilter");

            var itemsOrderSelector = AngularJSDemo.AngularJSDemo.getOrderBySelector("cpOrderBy");

            itemsSubSpan.setAttribute('ng-repeat', '' + "checkpoint" + ' in ' + "checkpoints" + ' | filter: ' + itemsSearchBox.getAttribute('ng-model') + ' | orderBy: ' + itemsOrderSelector.getAttribute('ng-model'));

            var itemsDiv = document.createElement('div');
            itemsDiv.setAttribute('ng-controller', "hwbSctl");
            itemsDiv.appendChild(itemsUL);
            itemsDiv.appendChild(itemsSearchBox);
            itemsDiv.appendChild(itemsOrderSelector);
            itemsDiv.appendChild(itemsSubSpan);

            return itemsDiv;
        },
        getOrderBySelector: function (ngModelName) {
            var itemsOrderSelector = document.createElement('select');
            itemsOrderSelector.setAttribute('ng-model', ngModelName);

            itemsOrderSelector.add(Bridge.merge(document.createElement('option'), {
                value: "", 
                innerHTML: "No special order"
            } ));

            itemsOrderSelector.add(Bridge.merge(document.createElement('option'), {
                value: "name", 
                innerHTML: "CallSign"
            } ));

            itemsOrderSelector.add(Bridge.merge(document.createElement('option'), {
                value: "id", 
                innerHTML: "Internal"
            } ));

            return itemsOrderSelector;
        },
        defineNiceController: function () {
            AngularJSDemo.AngularJSDemo.hwbApp.controller("hwcSctl", AngularJSDemo.AngularJSDemo.ctlFunction);

            var ctlDiv = document.createElement('div');
            ctlDiv.setAttribute('ng-controller', "hwcSctl");
            document.body.appendChild(ctlDiv);

            var fltFld = document.createElement('input');
            fltFld.setAttribute('ng-model', "hwcFlt");
            ctlDiv.appendChild(fltFld);

            var ordFld = document.createElement('select');
            ordFld.setAttribute('ng-model', "hwcOrderBy");
            ordFld.add(Bridge.merge(document.createElement('option'), {
                value: "Checkpoint", 
                innerHTML: "Alphabetically"
            } ));
            ordFld.add(Bridge.merge(document.createElement('option'), {
                value: "id", 
                innerHTML: "Series ID"
            } ));
            ctlDiv.appendChild(ordFld);

            var rptSpan = document.createElement('span');
            rptSpan.setAttribute('ng-repeat', '' + "checkpoint" + ' in ' + "checkpoints" + ' | filter: ' + fltFld.getAttribute('ng-model') + ' | orderBy: ' + ordFld.getAttribute('ng-model'));
            rptSpan.innerHTML = "{{checkpoint.callsign}}[{{checkpoint.id}}] ";
            ctlDiv.appendChild(rptSpan);
        },
        ctlFunction: function ($scope) {
            $scope.message = "Hello, Angular defined as nice $scope function.";
            $scope.foo = "Foo fighter nice $scope";
            $scope.bar = "Nice $scope 777 slot";
            $scope.checkpoints = Bridge.merge(new Bridge.List$1(Object)(), [
                [{ callsign: "Alpha", id: 85 }], 
                [{ callsign: "Baker", id: 35 }], 
                [{ callsign: "Charlie", id: 55 }], 
                [{ callsign: "Delta", id: 6 }]
            ] ).toArray();
        },
        /**
         * Trying to define AngularJS controller/app after DOM is loaded will
         not work. Angular is supposed to be defined during the page
         composition, not after it is ready.
         *
         * @static
         * @public
         * @this AngularJSDemo.AngularJSDemo
         * @memberof AngularJSDemo.AngularJSDemo
         * @return  {void}        
         */
        updateControls: function () {
            console.log("Checkpoint Zebra: [rdy] nothing at this point will like AngularJS.");

            var lbl = Bridge.merge(document.createElement('span'), {
                innerHTML: "We have Bridge script running."
            } );
            document.body.appendChild(lbl);
        }
    }
});

Bridge.define('AngularJSDemo.AngularJSDemo.ControllerDataObjectStructure', {
    message: null,
    foo: null,
    bar: null,
    checkpoints: null
});

