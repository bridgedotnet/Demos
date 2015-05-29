/* global Bridge */

Bridge.define('LiveBridgeBuilder.App', {
    statics: {
        INIT_CS_CODE: "public class App \r\n{ \r\n    [Ready] \r\n    public static void Main() \r\n    { \r\n        Console.WriteLine(\"Hello Bridge.NET\"); \r\n    }\r\n}",
        MAX_KEYSTROKES: 10,
        INTERVAL_DELAY: 2000,
        csEditor: null,
        jsEditor: null,
        keystrokes: 0,
        interval: 0,
        config: {
            init: function () {
                Bridge.ready(this.main);
            }
        },
        main: function () {
            LiveBridgeBuilder.App.initEditors();
            LiveBridgeBuilder.App.hookTranslateEvent();
            LiveBridgeBuilder.App.hookRunEvent();
            LiveBridgeBuilder.App.translate();
        },
        initEditors: function () {
            // Initialize ace csharp editor

            LiveBridgeBuilder.App.csEditor = ace.edit("CsEditor");
            LiveBridgeBuilder.App.csEditor.getSession().setMode("ace/mode/csharp");
            LiveBridgeBuilder.App.csEditor.setWrapBehavioursEnabled(true);
            LiveBridgeBuilder.App.csEditor.setValue(LiveBridgeBuilder.App.INIT_CS_CODE, 1);
            LiveBridgeBuilder.App.hookCsEditorInputEvent();

            // Initialize ace js editor

            LiveBridgeBuilder.App.jsEditor = ace.edit("JsEditor");
            LiveBridgeBuilder.App.jsEditor.getSession().setMode("ace/mode/javascript");
            LiveBridgeBuilder.App.jsEditor.setValue("");
        },
        translate: function () {
            LiveBridgeBuilder.App.progress("Compiling...");

            // Make call to Bridge.NET translator and show emitted javascript upon success 

            $.ajax({ url: "TranslateHandler.ashx?ajax=1", type: "POST", cache: false, data: { cs: LiveBridgeBuilder.App.csEditor.getValue() }, success: function (data, textStatus, request) {
                LiveBridgeBuilder.App.progress(null);
                if (!Bridge.cast(data.Success, Boolean)) {
                    LiveBridgeBuilder.App.jsEditor.setValue(data.ErrorMessage);
                    $("#hash").text("");
                    LiveBridgeBuilder.App.progress("Finished with error(s)");
                }
                else  {
                    LiveBridgeBuilder.App.jsEditor.setValue(data.JsCode);
                    $("#hash").text(data.Hash.toString());
                    LiveBridgeBuilder.App.progress("Finished");
                }
            } });
        },
        onInterval: function () {
            // Translate every INTERVAL_DELAY ms unless there are no changes to the C# editor content

            if (LiveBridgeBuilder.App.keystrokes > 0) {
                LiveBridgeBuilder.App.keystrokes = LiveBridgeBuilder.App.MAX_KEYSTROKES;
                LiveBridgeBuilder.App.onCsEditorInput();
            }
        },
        onCsEditorInput: function () {
            // Translate every MAX_KEYSTROKES keystrokes or after INTERVAL_DELAY msecs since the last keystroke

            Bridge.global.clearInterval(LiveBridgeBuilder.App.interval);

            if (LiveBridgeBuilder.App.keystrokes >= LiveBridgeBuilder.App.MAX_KEYSTROKES) {
                LiveBridgeBuilder.App.keystrokes = 0;
                LiveBridgeBuilder.App.translate();
            }
            else  {
                LiveBridgeBuilder.App.keystrokes++;
                LiveBridgeBuilder.App.interval = Bridge.global.setInterval(LiveBridgeBuilder.App.onInterval, LiveBridgeBuilder.App.INTERVAL_DELAY);
            }
            console.log(LiveBridgeBuilder.App.keystrokes);
        },
        hookCsEditorInputEvent: function () {
            // Attach input event handler to the c# editor

            $("#CsEditor").on("keyup", LiveBridgeBuilder.App.onCsEditorInput);
        },
        hookTranslateEvent: function () {
            // Attach click event handler to the translate html button

            $("#btnTranslate").on("click", LiveBridgeBuilder.App.translate);
        },
        hookRunEvent: function () {
            // Attach click event handler to the run button

            $("#btnRun").on("click", function () {
                window.open("run.html?h=" + $("#hash").text());
            });
        },
        progress: function (message) {
            // Show translation progress message

            var progress = $("#progress");

            if (!Bridge.String.isNullOrEmpty(message)) {
                progress.text(message);
            }
            else  {
                progress.text("");
            }
        }
    }
});