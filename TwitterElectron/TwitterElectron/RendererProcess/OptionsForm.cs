using System;
using Bridge;
using static Retyped.dom;
using static Retyped.electron;
using static Retyped.node;

namespace TwitterElectron.RendererProcess
{
    public class OptionsForm
    {
        [Init(InitPosition.Top)]
        public static void InitGlobals()
        {
            var Electron = (Electron.AllElectron)require.Self("electron");
        }

        [Template("Electron")]
        public static Electron.AllElectron Electron;

        public static void Main()
        {
            var okButton = (HTMLButtonElement)document.getElementById("okButton");
            var cancelButton = (HTMLButtonElement)document.getElementById("cancelButton");

            Electron.ipcRenderer.on(Constants.IPC.RestoreOptions, new Action<Event, TwitterCredentials>((ev, cred) =>
            {
                ((HTMLInputElement) document.getElementById("apiKeyInput")).value = cred?.ApiKey;
                ((HTMLInputElement) document.getElementById("apiSecretInput")).value = cred?.ApiSecret;
                ((HTMLInputElement) document.getElementById("accessTokenInput")).value = cred?.AccessToken;
                ((HTMLInputElement) document.getElementById("accessTokenSecretInput")).value = cred?.AccessTokenSecret;
            }));

            okButton.addEventListener("click", () => 
            {
                var cred = new TwitterCredentials
                {
                    ApiKey = ((HTMLInputElement) document.getElementById("apiKeyInput")).value,
                    ApiSecret = ((HTMLInputElement) document.getElementById("apiSecretInput")).value,
                    AccessToken = ((HTMLInputElement) document.getElementById("accessTokenInput")).value,
                    AccessTokenSecret = ((HTMLInputElement) document.getElementById("accessTokenSecretInput")).value
                };

                Electron.ipcRenderer.send(Constants.IPC.OptionsUpdated, cred);
                Electron.remote.getCurrentWindow().close();
            });

            cancelButton.addEventListener("click", () =>
            {
                Electron.remote.getCurrentWindow().close();
            });
        }
    }
}