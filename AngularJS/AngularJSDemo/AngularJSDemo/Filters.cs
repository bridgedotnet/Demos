using Bridge.AngularJS;

namespace PhoneCat
{
    public partial class App
    {
        public static void InitFilters()
        {
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
        }
    }
}