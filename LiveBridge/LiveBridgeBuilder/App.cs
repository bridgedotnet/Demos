using Bridge.Html5;
using Bridge.jQuery2;

namespace LiveBridgeBuilder
{
    public class App
    {
        private const string INIT_CS_CODE = @"public class App 
{ 
    [Ready] 
    public static void Main() 
    { 
        Console.WriteLine(""Hello Bridge.NET""); 
    }
}";
        private const int MAX_KEYSTROKES = 10;

        private const int INTERVAL_DELAY = 2000;

        public static dynamic CsEditor;
        public static dynamic JsEditor;

        public static int Keystrokes;
        public static int Interval;

        [Ready]
        public static void Main()
        {
            App.InitEditors();
            App.Translate();            
        }

        protected static void InitEditors()
        {
            // Get an instance of the ace editor
            var ace = Global.Get<dynamic>("ace");

            // Initialize ace csharp editor
            App.CsEditor = ace.edit("CsEditor");
            App.CsEditor.getSession().setMode("ace/mode/csharp");
            App.CsEditor.setWrapBehavioursEnabled(true);
            App.CsEditor.setValue(App.INIT_CS_CODE, 1);
            App.HookCsEditorInputEvent();

            // Initialize ace js editor
            App.JsEditor = ace.edit("JsEditor");
            App.JsEditor.getSession().setMode("ace/mode/javascript");
            App.JsEditor.setValue("");
        }

        [Bridge.jQuery2.Click("#btnTranslate")]
        protected static void Translate()
        {
            App.Progress("Compiling...");

            // Make call to Bridge.NET translator and show emitted javascript upon success 

            jQuery.Ajax(
                new AjaxOptions()
                {
                    Url = "TranslateHandler.ashx?ajax=1",
                    Type = "POST",
                    Cache = false,
                    Data = new
                    {
                        cs = App.CsEditor.getValue()
                    },
                    Success = delegate(object data, string textStatus, jqXHR request)
                    {
                        App.Progress(null);

                        if (!(bool)data["Success"])
                        {
                            App.JsEditor.setValue(data["ErrorMessage"]);
                            jQuery.Select("#hash").Text(string.Empty);
                            App.Progress("Finished with error(s)");
                        }
                        else
                        {
                            App.JsEditor.setValue(data["JsCode"]);
                            jQuery.Select("#hash").Text(data["Hash"].ToString());
                            App.Progress("Finished");
                        }
                    }
                }
            );
        }
        
        protected static void OnInterval()
        {
            // Translate every INTERVAL_DELAY ms unless there are no changes to the C# editor content

            if (App.Keystrokes > 0)
            {
                App.Keystrokes = App.MAX_KEYSTROKES;
                App.OnCsEditorInput();
            }
        }

        protected static void OnCsEditorInput()
        {
            // Translate every MAX_KEYSTROKES keystrokes or after INTERVAL_DELAY msecs since the last keystroke
            Global.ClearInterval(App.Interval);

            if (App.Keystrokes >= App.MAX_KEYSTROKES)
            {
                App.Keystrokes = 0;
                App.Translate();
            }
            else
            {
                App.Keystrokes++;
                App.Interval = Global.SetInterval(App.OnInterval, App.INTERVAL_DELAY);
            }
        }

        protected static void HookCsEditorInputEvent()
        {
            // Attach input event handler to the c# editor

            jQuery.Select("#CsEditor").KeyUp(App.OnCsEditorInput);
        }

        /// <summary>
        /// Attach click event handler to the run button
        /// </summary>
        [Bridge.jQuery2.Click("#btnRun")]
        protected static void HookRunEvent()
        {
            Window.Open("run.html?h=" + jQuery.Select("#hash").Text());
        }
        
        /// <summary>
        /// Show translation progress message
        /// </summary>
        /// <param name="message"></param>
        public static void Progress(string message)
        {
            var progress = jQuery.Select("#progress");

            if (!string.IsNullOrEmpty(message))
            {
                progress.Text(message);
            }
            else
            {
                progress.Text(string.Empty);
            }
        }
    }
}