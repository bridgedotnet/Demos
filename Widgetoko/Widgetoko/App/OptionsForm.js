// Init global variables (modules):
var Electron = require("electron");
var jQuery = require("jquery");

Bridge.assembly("Widgetoko", function ($asm, globals) {
    "use strict";

    Bridge.define("Widgetoko.RendererProcess.OptionsForm", {
        main: function Main () {
            Widgetoko.RendererProcess.OptionsForm.ConfigureEventHandlers();

            // Get credentials from the main process:
            var credentials = Electron.ipcRenderer.sendSync("cmd-get-credentials-sync");

            // Display values on the form:
            jQuery("#apiKeyInput").val(credentials != null ? credentials.ApiKey : null);
            jQuery("#apiSecretInput").val(credentials != null ? credentials.ApiSecret : null);
            jQuery("#accessTokenInput").val(credentials != null ? credentials.AccessToken : null);
            jQuery("#accessTokenSecretInput").val(credentials != null ? credentials.AccessTokenSecret : null);
        },
        statics: {
            fields: {
                Electron: null
            },
            methods: {
                ConfigureEventHandlers: function () {
                    jQuery("#okButton").on("click", function (e, args) {
                        var cred = { ApiKey: Bridge.as(jQuery("#apiKeyInput").val(), System.String), ApiSecret: Bridge.as(jQuery("#apiSecretInput").val(), System.String), AccessToken: Bridge.as(jQuery("#accessTokenInput").val(), System.String), AccessTokenSecret: Bridge.as(jQuery("#accessTokenSecretInput").val(), System.String) };

                        Electron.ipcRenderer.send("cmd-set-credentials", cred);
                        Electron.remote.getCurrentWindow().close();

                        return null;
                    });

                    jQuery("#cancelButton").on("click", function (e, args) {
                        Electron.remote.getCurrentWindow().close();

                        return null;
                    });
                }
            }
        }
    });
});
