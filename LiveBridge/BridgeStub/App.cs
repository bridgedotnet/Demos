using Bridge;
using Bridge.Html5;
using Bridge.Linq;
using Bridge.jQuery2;
using Bridge.Bootstrap3;
using Bridge.WebGL;
using System.Linq;
using System.Collections.Generic;

namespace BridgeStub
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

        public static object CsEditor, JsEditor;

        [Ready]
        public static void Main()
        {
            InitEditors();
            HookTranslateEvent();
        }

        protected static void InitEditors()
        {
            // Initialize csharp editor 
            
            App.CsEditor = Script.ToDynamic().ace.edit("CsEditor");
            App.CsEditor.ToDynamic().getSession().setMode("ace/mode/csharp");
            App.CsEditor.ToDynamic().setWrapBehavioursEnabled(true);
            App.CsEditor.ToDynamic().setValue(App.INIT_CS_CODE, 1);

            App.JsEditor = Script.ToDynamic().ace.edit("JsEditor");
            App.JsEditor.ToDynamic().getSession().setMode("ace/mode/javascript");
            App.JsEditor.ToDynamic().setValue("");
        }

        protected static void HookTranslateEvent()
        {
            // Attach click event handler to the translate html button
            
            jQuery.Select("#btnTranslate").On("click", () =>
            {
                App.Progress("Compiling...");

                jQuery.Ajax(
                    new AjaxOptions()
                    {
                        Url = "Default.aspx?ajax=1",
                        Type = "POST",
                        Cache = false,
                        Data = new
                        {
                            cs = App.CsEditor.ToDynamic().getValue()
                        },
                        Success = delegate(object data, string textStatus, jqXHR request)
                        {
                            App.Progress(null);
                            if (!(bool)data["Success"])
                            {
                                App.JsEditor.ToDynamic().setValue(data["ErrorMessage"]);
                                App.Progress("Finished with error(s)");
                            }
                            else
                            {
                                App.JsEditor.ToDynamic().setValue(data["JsCode"]);
                                App.Progress("Finished");
                            }
                        }
                    }
                );
            });
        }

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