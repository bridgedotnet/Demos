using Bridge;
            using Bridge.Bootstrap3;
            using Bridge.Html5;
            using Bridge.jQuery2;
            using Bridge.Linq;
            using Bridge.WebGL;
            using System.Linq;
            using System.Text;
            using System.Collections.Generic;

            namespace LiveBridgeBuilder
            {// Create a simple class for our application
public class App 
{ 
    // Use the [Ready] attribute to automaically
    // run this method when the page is ready.
    [Ready] 
    public static void Main() 
    {
        // Create a new Button
        var button = new ButtonElement
        {
            // Set the Button text
            InnerHTML = "Click Me",
            
            // Add a Click event handler
            OnClick = (ev) => {
                // Fire an Alert when the Button is clicked
                Global.Alert("Welcome to Bridge.NET");
            } 
        };
 
        // Add the button to the page body
        Document.Body.AppendChild(button);
    }
}}