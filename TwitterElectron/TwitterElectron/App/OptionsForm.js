var Electron = require("electron");

Bridge.assembly("TwitterElectron", function ($asm, globals) {
    "use strict";

    Bridge.define("TwitterElectron.RendererProcess.OptionsForm", {
        main: function Main () {
            var okButton = document.getElementById("okButton");
            var cancelButton = document.getElementById("cancelButton");

            Electron.ipcRenderer.on("cmd-options-restore", function (ev, cred) {
                document.getElementById("apiKeyInput").value = cred != null ? cred.ApiKey : null;
                document.getElementById("apiSecretInput").value = cred != null ? cred.ApiSecret : null;
                document.getElementById("accessTokenInput").value = cred != null ? cred.AccessToken : null;
                document.getElementById("accessTokenSecretInput").value = cred != null ? cred.AccessTokenSecret : null;
            });

            okButton.addEventListener("click", function () {
                var cred = { ApiKey: document.getElementById("apiKeyInput").value, ApiSecret: document.getElementById("apiSecretInput").value, AccessToken: document.getElementById("accessTokenInput").value, AccessTokenSecret: document.getElementById("accessTokenSecretInput").value };

                Electron.ipcRenderer.send("cmd-options-updated", cred);
                Electron.remote.getCurrentWindow().close();
            });

            cancelButton.addEventListener("click", function () {
                Electron.remote.getCurrentWindow().close();
            });
        },
        statics: {
            fields: {
                Electron: null
            }
        }
    });
});
