using Bridge.Html5;

namespace Demo
{
    public class DemoClass
    {
        [Ready]
        public static void Main()
        {
            Console.Log("Demo. Logged on console.");

            var spanEl = new SpanElement()
            {
                InnerHTML = "If you can read this line, then bridge is working!"
            };

            Document.Body.AppendChild(spanEl);

            var button = new ButtonElement
            {
                InnerHTML = "Show Bridge.NET message",
                OnClick = (ev) =>
                {
                    Global.Alert("Welcome to Bridge.NET");
                }
            };

            Document.Body.AppendChild(button);
        }
    }
}
