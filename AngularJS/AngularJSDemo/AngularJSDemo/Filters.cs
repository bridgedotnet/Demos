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
        public static void InitFilters()
        {
            var phonecatFilters = Angular.Module("phonecatFilters");

            // The following is equivalent to defining methods
            // - string mb(string text) { return "sometext"; }
            // and
            // - Func<string, string> ma() { return mb; }
            // Then calling .Filter("text", ma);
            phonecatFilters.Filter("checkmark", () =>
            {
                return (input) =>
                {
                    return (input == "true") ? "\u2713" : "\u2718";
                };
            });
        }
    }
}