var Electron = require("electron");

Bridge.assembly("TwitterElectron", function ($asm, globals) {
    "use strict";

    Bridge.define("TwitterElectron.RendererProcess.OptionsForm", {
        main: function Main () {
            var okButton = Bridge.cast(document.getElementById("okButton"), HTMLButtonElement);
            var cancelButton = Bridge.cast(document.getElementById("cancelButton"), HTMLButtonElement);

            Electron.ipcRenderer.on("cmd-options-restore", $asm.$.TwitterElectron.RendererProcess.OptionsForm.f1);

            okButton.addEventListener("click", $asm.$.TwitterElectron.RendererProcess.OptionsForm.f2);

            cancelButton.addEventListener("click", $asm.$.TwitterElectron.RendererProcess.OptionsForm.f3);
        },
        statics: {
            fields: {
                Electron: null
            }
        }
    });

    Bridge.ns("TwitterElectron.RendererProcess.OptionsForm", $asm.$);

    Bridge.apply($asm.$.TwitterElectron.RendererProcess.OptionsForm, {
        f1: function (ev, cred) {
            Bridge.cast(document.getElementById("apiKeyInput"), HTMLInputElement).value = cred != null ? cred.ApiKey : null;
            Bridge.cast(document.getElementById("apiSecretInput"), HTMLInputElement).value = cred != null ? cred.ApiSecret : null;
            Bridge.cast(document.getElementById("accessTokenInput"), HTMLInputElement).value = cred != null ? cred.AccessToken : null;
            Bridge.cast(document.getElementById("accessTokenSecretInput"), HTMLInputElement).value = cred != null ? cred.AccessTokenSecret : null;
        },
        f2: function () {
            var cred = { ApiKey: Bridge.cast(document.getElementById("apiKeyInput"), HTMLInputElement).value, ApiSecret: Bridge.cast(document.getElementById("apiSecretInput"), HTMLInputElement).value, AccessToken: Bridge.cast(document.getElementById("accessTokenInput"), HTMLInputElement).value, AccessTokenSecret: Bridge.cast(document.getElementById("accessTokenSecretInput"), HTMLInputElement).value };

            Electron.ipcRenderer.send("cmd-options-updated", cred);
            Electron.remote.getCurrentWindow().close();
        },
        f3: function () {
            Electron.remote.getCurrentWindow().close();
        }
    });
});
