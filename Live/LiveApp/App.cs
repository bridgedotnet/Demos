using Bridge.Html5;
using Bridge.jQuery2;
using Bridge.Bootstrap3;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace LiveApp
{
    public class App
    {
        private const int MAX_KEYSTROKES = 10;

        private const int INTERVAL_DELAY = 1500;

        private const int JS_HEADER_LINES = 13;

        public static dynamic CsEditor;
        public static dynamic JsEditor;

        public static int Keystrokes;
        public static int Timeout;

        public static string Hash
        {
            get
            {
                return Global.Location.Hash;
            }
            set
            {
                Global.Location.ToDynamic().hash = value;
            }
        }

        public static bool HasHash
        {
            get
            {
                return !string.IsNullOrEmpty(App.Hash);
            }
        }

        public static bool IsGistHash
        {
            get
            {
                return App.HasHash && !App.Hash.StartsWith("#example");
            }
        }

        [Ready]
        public static void Main()
        {
            App.InitEditors();
            App.SetEditorSize();
            Global.OnResize = SetEditorSize;
            
            App.LoadExamples(!App.HasHash);

            if (App.IsGistHash)
            {
                App.LoadFromGist(App.Hash, "https://api.github.com/gists/" + App.Hash.Substr(1), false);
            }
        }

        protected static void LoadExamples(bool loadDefault)
        {
            jQuery.Ajax(
                new AjaxOptions()
                {
                    Url = "https://api.github.com/gists/fc03fa0d97097d4ceabe",
                    Type = "GET",
                    Cache = false,
                    Success = delegate(object data, string textStatus, jqXHR request)
                    {
                        var files = data["files"];

                        // var names = Global.Keys(files);
                        var names = Global.ToDynamic().Object.keys(files);
                                                                        
                        // If no url hash, auto load the first example by default
                        string autoLoadUrl = (loadDefault) ? files[names[0]]["raw_url"].ToString() : string.Empty;

                        jQuery examples = jQuery.Select("#examples");

                        int count = 1;

                        foreach (string name in names)
                        {
                            string title = "#example" + (count++).ToString();
                            
                            if (!loadDefault && App.Hash == title)
                            {
                                // Mark the example to auto load
                                autoLoadUrl = files[name]["raw_url"].ToString();
                            }

                            new jQuery("<li>")
                                .Attr("role", "presentation")
                                .Append(new AnchorElement
                                {
                                    Href = files[name]["raw_url"].ToString(),
                                    Text = App.FormatExampleFilename(files[name]["filename"].ToString()),
                                    Title = title,
                                    OnClick = App.LoadExample
                                })
                                .AppendTo(examples);
                        }

                        // Auto load requested example or the first example by default
                        if (!string.IsNullOrEmpty(autoLoadUrl))
                        {
                            string hash = (loadDefault) ? "#example1" : App.Hash;

                            App.LoadFromGist(hash, autoLoadUrl);

                            jQuery example = jQuery.Select("#examples > li > a[title='" + hash + "']");
                            example.Parent().AddClass("active");

                            // Update editor title with file name
                            jQuery.Select("#filename").Html(example.Text());
                        }
                    }
                }
            );
        }

        protected static string FormatExampleFilename(string filename)
        {
            // Example Gist files are prefixed with numbering "N." (where N = 1,2,...). 
            // While this is handy to define order it looks ugly in the Samples dropdown.
            string[] res = new Regex(@"\d\.\s(.*)").Exec(filename);

            return (res != null) ? res[1] : filename;
        }

        protected static void LoadExample(Event evt)
        {
            // Click event handler attached to #examples > li > a 
            evt.PreventDefault();
            App.LoadFromGist(jQuery.This.Attr("title"), jQuery.This.Attr("href"));

            // Clear active for all #examples > li
            jQuery.Select("#examples > li").RemoveClass("active");

            // Mark currently selected li as active
            jQuery.This.Parent().AddClass("active");

            // Update editor title with selected file name
            jQuery.Select("#filename").Html(jQuery.This.Text());
        }
        
        protected static void LoadFromGist(string hash, string url, bool isRaw = true)
        {
            // Get C# file content from Gist url and upon success:
            //   set it as value of the CsEditor
            //   translate
            //   add hash to url

            jQuery.Ajax(
                new AjaxOptions()
                {
                    Url = url,
                    Type = "GET",
                    Cache = false,
                    Success = delegate(object data, string textStatus, jqXHR request)
                    {
                        if (isRaw)
                        {
                            // the response data is the c# code
                            App.CsEditor.setValue(data, -1);
                        }
                        else
                        {
                            // the contents of the first file of the json data is the c# code 
                            var files = data["files"];

                            var names = Global.ToDynamic().Object.keys(files);

                            App.CsEditor.setValue(files[names[0]]["content"].ToString(), -1);
                        }

                        App.Translate();

                        App.Hash = hash;
                    }
                }
            );
        }

        protected static void InitEditors()
        {
            // Get an instance of the ace editor
            var ace = Global.Get<dynamic>("ace");

            // Initialize ace csharp editor
            App.CsEditor = ace.edit("CsEditor");
            App.CsEditor.renderer.setPadding(10);
            App.CsEditor.renderer.setScrollMargin(13, 13, 13, 13);
            App.CsEditor.setTheme("ace/theme/terminal");
            App.CsEditor.getSession().setMode("ace/mode/csharp");
            App.CsEditor.setWrapBehavioursEnabled(true);
            
            App.HookCsEditorKeyupEvent();
            App.HookCsEditorKeydownEvent();

            // Initialize ace js editor
            App.JsEditor = ace.edit("JsEditor");
            App.JsEditor.renderer.setPadding(10);
            App.JsEditor.renderer.setScrollMargin(13, 13, 13, 13);
            App.JsEditor.setTheme("ace/theme/terminal");
            App.JsEditor.getSession().setMode("ace/mode/javascript");
            App.JsEditor.setValue("");
        }

        [Bridge.jQuery2.Click("#btnTranslate")]
        protected static void Translate()
        {
            jQuery.Select("#status").Attr("src", "resources/images/loader.gif").Show();
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
                            // error
                            TranslateError error = App.GetErrorMessage(data["ErrorMessage"].ToString());
                            App.JsEditor.setValue(error.ToString(), -1);
                            jQuery.Select("#hash").Text(string.Empty);
                            jQuery.Select("#status").Attr("src", "resources/images/error.png");
                            App.Progress("Finished with error(s)");
                        }
                        else
                        {
                            // success
                            App.JsEditor.setValue(data["JsCode"], -1);
                            jQuery.Select("#hash").Text(data["Hash"].ToString());
                            jQuery.Select("#status").Attr("src", "resources/images/check.png");
                            App.Progress("Compiled successfully!");
                        }
                    }
                }
            );
        }

        protected static void CreatePermaLink(string code)
        {
            // Create Gist with the c# code as content and set new Gist id to url hash

            if (App.HasHash)
            {
                Global.Alert("Shareable link already in the browser's address bar.");
                return;
            }

            string json = "{ \"description\": \"live.bridge.net\",  \"public\": true,"
                               + "\"files\": {   \"livebridgenet.cs\": {"
                               + "\"content\":" + JSON.Stringify(code) + "  } }}";
            jQuery.Ajax(
                new AjaxOptions()
                {
                    Url = "https://api.github.com/gists",
                    Type = "POST",
                    Cache = false,
                    DataType = "json",
                    Data = json,
                    Success = delegate(object data, string textStatus, jqXHR request)
                    {
                        App.Hash = data["id"].ToString();
                        Global.Alert("Shareable link now in the browser's address bar.");
                    }
                }
            );            
        }

        protected static TranslateError GetErrorMessage(string message)
        {
            string[] err = new Regex(@"Line (\d+), Col (\d+)\): (.*)", "g").Exec(message);
            
            string msg = message;

            int line = 0;
            int col = 0;            

            if (err != null)
            {
                if (int.TryParse(err[1], out line))
                {
                    line = line - App.JS_HEADER_LINES;
                }

                int.TryParse(err[2], out col);

                msg = err[3];
            }

            return new TranslateError
            {
                Line = line,
                Column = col,
                Message = msg
            };
        }

        protected static void OnTimeout()
        {
            // Translate every INTERVAL_DELAY msec since the last keyup event of the c# editor

            App.Translate();

            // Clear url hash 
            App.Hash = string.Empty;

            // Clear filename
            jQuery.Select("#filename").Html(string.Empty);
        }

        /// <summary>
        /// Attach keyup event handler to the c# editor
        /// </summary>
        protected static void HookCsEditorKeyupEvent()
        {
            // On c# editor keyup event start the timeout countdown
            jQuery.Select("#CsEditor").KeyUp((Action<KeyboardEvent>)((KeyboardEvent evt) =>
            {
                // except for arrow keys
                if (evt.Which < 37 || evt.Which > 40)
                {
                    Global.ClearTimeout(App.Timeout);
                    App.Timeout = Global.SetTimeout(App.OnTimeout, App.INTERVAL_DELAY);
                }
            }));            
        }

        /// <summary>
        /// Attach keydown event handler to the c# editor
        /// </summary>
        protected static void HookCsEditorKeydownEvent()
        {
            // On c# editor keydown clear the timeout
            jQuery.Select("#CsEditor").KeyDown(() => 
            {
                Global.ClearTimeout(App.Timeout); 
            });            
        }

        /// <summary>
        /// Attach click event handler to the run button
        /// </summary>
        [Bridge.jQuery2.Click("#btnRun")]
        protected static void HookRunEvent(Event evt)
        {
            evt.PreventDefault();
            Window.Open("run.html?h=" + jQuery.Select("#hash").Text());
        }

        /// <summary>
        /// Attach click event handler to the share button
        /// </summary>
        [Bridge.jQuery2.Click("#btnShare")]
        protected static void HookShareEvent(Event evt)
        {
            evt.PreventDefault();

            App.CreatePermaLink(App.CsEditor.getValue());
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

        /// <summary>
        /// Adjust editor size
        /// </summary>
        protected static void SetEditorSize(Event e = null)
        {           
            // Set editor height
            int mastheadHeight = jQuery.Select("#masthead").OuterHeight();
            int titleHeight = jQuery.Select("#title").OuterHeight();
            int editorHeaderHeight = jQuery.Select(".code-description").OuterHeight();
            int sitefooterHeight = jQuery.Select(".site-footer").OuterHeight();
            int padding = 15;

            int editorHeight = Window.InnerHeight - (mastheadHeight + titleHeight  + editorHeaderHeight + sitefooterHeight  + padding);
            jQuery.Select(".ace_editor").Css("height", editorHeight);
        }

        /// <summary>
        /// Attach click handler event to offcanvas sidebar triggers
        /// </summary>
        /// <param name="e"></param>
        [Bridge.jQuery2.Click("#offcanvas-toggle, .offcanvas-close-button, .overlay")]
        protected static void OffCanvasMenu(Event e)
        {
            e.PreventDefault();
            jQuery.Select("#sidebar-offcanvas, .main-wrapper").ToggleClass("active");
            jQuery.Select("body").ToggleClass("offcanvas-open");
        }
    }

    public class TranslateError
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return string.Format("// Line {0}, Col {1} : {2}", this.Line.ToString(), this.Column.ToString(), this.Message);
        }
    }
}