/* global Bridge */

Bridge.define('BridgeStub.App', {
    statics: {
        INIT_CS_CODE: "public class App \r\n{ \r\n    [Ready] \r\n    public static void Main() \r\n    { \r\n        Console.WriteLine(\"Hello Bridge.NET\"); \r\n    }\r\n}",
        csEditor: null,
        jsEditor: null,
        config: {
            init: function () {
                Bridge.ready(this.main);
            }
        },
        main: function () {
            BridgeStub.App.initEditors();
            BridgeStub.App.hookTranslateEvent();
            BridgeStub.App.hookRunEvent();
        },
        initEditors: function () {
            // Initialize ace csharp editor

            BridgeStub.App.csEditor = ace.edit("CsEditor");
            BridgeStub.App.csEditor.getSession().setMode("ace/mode/csharp");
            BridgeStub.App.csEditor.setWrapBehavioursEnabled(true);
            BridgeStub.App.csEditor.setValue(BridgeStub.App.INIT_CS_CODE, 1);

            // Initialize ace js editor

            BridgeStub.App.jsEditor = ace.edit("JsEditor");
            BridgeStub.App.jsEditor.getSession().setMode("ace/mode/javascript");
            BridgeStub.App.jsEditor.setValue("");
        },
        translate: function () {
            BridgeStub.App.progress("Compiling...");

            // Make call to Bridge.NET translator and show emitted javascript upon success 

            $.ajax({ url: "Default.aspx?ajax=1", type: "POST", cache: false, data: { cs: BridgeStub.App.csEditor.getValue() }, success: function (data, textStatus, request) {
                BridgeStub.App.progress(null);
                if (!Bridge.cast(data.Success, Boolean)) {
                    BridgeStub.App.jsEditor.setValue(data.ErrorMessage);
                    $("#hash").text("");
                    BridgeStub.App.progress("Finished with error(s)");
                }
                else  {
                    BridgeStub.App.jsEditor.setValue(data.JsCode);
                    $("#hash").text(data.Hash.toString());
                    BridgeStub.App.progress("Finished");
                }
            } });
        },
        hookTranslateEvent: function () {
            // Attach click event handler to the translate html button

            $("#btnTranslate").on("click", BridgeStub.App.translate);
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