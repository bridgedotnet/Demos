using Bridge.Html5;

namespace Demo
{
    public class DemoClass
    {
        [Ready]
        public static void main()
        {
            Console.Log("Demo. Logged on console.");
            Document.Body.AppendChild(new SpanElement() { InnerHTML = "Check results on the console log." });
        }
    }
}
