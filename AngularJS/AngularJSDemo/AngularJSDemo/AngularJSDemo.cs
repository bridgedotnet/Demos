using Bridge;
using Bridge.AngularJS;
using Bridge.Html5;
using System;
using System.Collections.Generic;

namespace AngularJSDemo
{
    /// <summary>
    /// Checkpoints:
    /// Alpha Bravo Charlie Delta Echo Foxtrot Golf Hotel India
    /// Juliet Kilo Lima Mike November Oscar Papa Quebec Romeo
    /// Sierra Tango Uniform Victor Whiskey X-ray Yankee Zulu
    /// http://home.earthlink.net/~malcolmhamer/alpha.htm
    /// </summary>
    public class AngularJSDemo
    {

        public class ControllerDataObjectStructure
        {
            public string Message;
            public string Foo;
            public string Bar;
            public object[] Checkpoints;
        }

        static AngularJSDemo()
        {
            Console.Log("Checkpoint Yankee: [ctor] nothng at this point will " +
                "like AngularJS.");
        }

        public static Module hwbApp;

        /// <summary>
        /// AngularJS requires to be defined on document parse.
        /// TODO: create an onParse attribute
        /// </summary>
        public static void StartUp()
        {
            Console.Log("Checkpoint Charlie: I must be printed BEFORE " +
                "Checkpoint 1.");

            AngularJSDemo.hwbApp = Angular.Module("hwbApp");

            var controllerData = new { message = "Hello, AngularJS message " +
                    "defined in Bridge's C#! :D" };
            AngularJSDemo.hwbApp.Controller("hwbctl", controllerData);
            AngularJSDemo.hwbApp.Directive("brdEntryPoint",
                AngularJSDemo.dynMehTemplate);

            var checkpoints = new List<object> {
                new { callsign = "Alpha", id = 98 },
                new { callsign = "Baker", id = 78 },
                new { callsign = "Charlie", id = 58 },
                new { callsign = "Delta", id = 9 }
            };

            var controllerStrongData = new ControllerDataObjectStructure()
            {
                Message = "Hello, AngularJS message defined in Bridge's C#'s " +
                    "strongly typed class! :D",
                Foo = "Foo fighters from strong C#.",
                Bar = "Strong C# 777 slot",
                Checkpoints = checkpoints.ToArray()
            };

            AngularJSDemo.hwbApp.Controller("hwbSctl", controllerStrongData);
            AngularJSDemo.hwbApp.Directive("brdEntryPointForThree",
                AngularJSDemo.ThreeWayFunction);

            Document.DocumentElement.SetNGApp(AngularJSDemo.hwbApp);
        }

        /// <summary>
        /// This function determines the format of the contents of the entry
        /// point where the dynamic code will be injected.
        /// </summary>
        /// <returns></returns>
        public static object dynMehTemplate()
        {
            var span = new SpanElement();
            span.SetNGController("hwbctl");
            span.InnerHTML = "AJS says by directive: [{{message}}]";
            return new { template = span.OuterHTML };
        }
        public static object ThreeWayFunction()
        {
            var span = new SpanElement();
            span.SetNGController("hwbSctl");
            span.InnerHTML = "AJS for three scopined: " +
                "[msg:{{message}}][foo:{{foo}}][bar:{{bar}}]";
            return new { template = span.OuterHTML };
        }

        // This one must be called after the tag has been defined in the
        // HTML DOM, yet before HTML DOM has been loaded (DOMContentLoaded)
        public static void StartUpIdPhase()
        {
            if (AngularJSDemo.hwbApp == null) {
                AngularJSDemo.StartUp();
            }

            var x = Document.GetElementById("entryPoint");
            if (x != null)
            {
                //x.SetAttribute("ng-controller", "hwbctl"); // same as below
                x.SetNGController("hwbctl");
                x.InnerHTML = "AJS says by element ID: [{{message}}]";
            }

            Document.Body.AppendChild(AngularJSDemo.GetRepeatRegion());

            // Broken :(
            AngularJSDemo.DefineNiceController();
        }

        public static Element GetRepeatRegion() {
            var itemsSpan = new SpanElement();
            itemsSpan.InnerHTML = "Checkpoint";
            var itemsPara = new ParagraphElement();
            itemsPara.InnerHTML = "{{checkpoint.callsign}}[{{checkpoint.id}}]";

            var itemsLI = new LIElement();
            itemsLI.SetNGRepeat("checkpoint", "checkpoints");
            itemsLI.AppendChild(itemsSpan);
            itemsLI.AppendChild(itemsPara);

            var itemsUL = new UListElement();
            itemsUL.AppendChild(itemsLI);

            var itemsSubSpan = new SpanElement()
            {
                InnerHTML = "[{{checkpoint.callsign}}.{{checkpoint.id}}] "
            };

            var itemsSearchBox = new InputElement();
            itemsSearchBox.SetNGModel("cpFilter");

            var itemsOrderSelector = GetOrderBySelector("cpOrderBy");

            itemsSubSpan.SetNGRepeat("checkpoint", "checkpoints",
                itemsSearchBox.GetNGModel(), itemsOrderSelector.GetNGModel());

            var itemsDiv = new DivElement();
            itemsDiv.SetNGController("hwbSctl");
            itemsDiv.AppendChild(itemsUL);
            itemsDiv.AppendChild(itemsSearchBox);
            itemsDiv.AppendChild(itemsOrderSelector);
            itemsDiv.AppendChild(itemsSubSpan);

            return itemsDiv;
        }

        public static Element GetOrderBySelector(string ngModelName)
        {
            var itemsOrderSelector = new SelectElement();
            itemsOrderSelector.SetNGModel(ngModelName);

            itemsOrderSelector.Add(new OptionElement() {
                Value = "",
                InnerHTML = "No special order"
            });

            itemsOrderSelector.Add(new OptionElement() {
                Value = "name",
                InnerHTML = "CallSign"
            });

            itemsOrderSelector.Add(new OptionElement() {
                Value = "id",
                InnerHTML = "Internal"
            });

            return itemsOrderSelector;
        }

        public static void DefineNiceController()
        {
            AngularJSDemo.hwbApp.Controller<ControllerDataObjectStructure>
                ("hwcSctl", CtlFunction);

            var ctlDiv = new DivElement();
            ctlDiv.SetNGController("hwcSctl");
            Document.Body.AppendChild(ctlDiv);

            var fltFld = new InputElement();
            fltFld.SetNGModel("hwcFlt");
            ctlDiv.AppendChild(fltFld);

            var ordFld = new SelectElement();
            ordFld.SetNGModel("hwcOrderBy");
            ordFld.Add(new OptionElement() {
                Value = "Checkpoint",
                InnerHTML = "Alphabetically"
            });
            ordFld.Add(new OptionElement() {
                Value = "id",
                InnerHTML = "Series ID"
            });
            ctlDiv.AppendChild(ordFld);

            var rptSpan = new SpanElement();
            rptSpan.SetNGRepeat("checkpoint", "checkpoints", fltFld.GetNGModel(),
                ordFld.GetNGModel());
            rptSpan.InnerHTML = "{{checkpoint.callsign}}[{{checkpoint.id}}] ";
            ctlDiv.AppendChild(rptSpan);
        }

        public static void CtlFunction(
            [Name("$scope")] ControllerDataObjectStructure scope)
        {
            scope.Message = "Hello, Angular defined as nice $scope function.";
            scope.Foo = "Foo fighter nice $scope";
            scope.Bar = "Nice $scope 777 slot";
            scope.Checkpoints = new List<object> {
                new { callsign = "Alpha", id = 85 },
                new { callsign = "Baker", id = 35 },
                new { callsign = "Charlie", id = 55 },
                new { callsign = "Delta", id = 6 }
            }.ToArray();
        }

        /// <summary>
        /// Trying to define AngularJS controller/app after DOM is loaded will
        /// not work. Angular is supposed to be defined during the page
        /// composition, not after it is ready.
        /// </summary>
        [Ready]
        public static void UpdateControls()
        {
            Console.Log("Checkpoint Zebra: [rdy] nothing at this point will " +
                "like AngularJS.");

            var lbl = new SpanElement() { InnerHTML = "We have Bridge script " +
                    "running." };
            Document.Body.AppendChild(lbl);
        }
    }
}