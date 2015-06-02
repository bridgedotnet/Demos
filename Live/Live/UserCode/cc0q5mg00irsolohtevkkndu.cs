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
            {public class App 
{ 
    private const string KEY = "KEY";
    
    [Ready] 
    public static void Main()
    {
        var div = new DivElement();
    
        var input = new InputElement()
        {
            Id = "number",
            Type = InputType.Text,
            Placeholder = "Enter a number to store...",
            Style =
            {
                Margin = "5px"
            }
        };
    
        input.AddEventListener(EventType.KeyPress, InputKeyPress);
        
        var buttonSave = new ButtonElement()
        {
            Id = "b",
            InnerHTML = "Save"
        };
    
        buttonSave.AddEventListener(EventType.Click, Save);
        
        var buttonRestore = new ButtonElement()
        {
            Id = "r",
            InnerHTML = "Restore",
            Style =
            {
                Margin = "5px"
            }
        };
        
        buttonRestore.AddEventListener(EventType.Click, Restore);
        
        div.AppendChild(input);
        div.AppendChild(buttonSave);
        div.AppendChild(buttonRestore);
        
        Document.Body.AppendChild(div);
        
        input.Focus();
    } 
    
    private static void InputKeyPress(Event e)
    {
        // We added the listener to EventType.KeyPress so it should be a KeyboardEvent
        if (e.IsKeyboardEvent() && e.As<KeyboardEvent>().KeyCode == 13)
        {
            Save();
        }
    }
 
    private static void Save()
    {
        var input = Document.GetElementById<InputElement>("number");
        int i = Window.ParseInt(input.Value);
 
        if (!Window.IsNaN(i))
        {
            Window.LocalStorage.SetItem(KEY, i);
            Window.Alert(string.Format("Stored {0}", i));
            input.Value = string.Empty;
        }
        else
        {
            Window.Alert("Incorrect value. Please enter a number.");
        }
    }
 
    private static void Restore()
    {
        var input = Document.GetElementById<InputElement>("number");
        var o = Window.LocalStorage[KEY];
 
        if (o != null)
        {
            input.Value = o.ToString();
        }
        else
        {
            input.Value = string.Empty;
        }
    }    
}
}