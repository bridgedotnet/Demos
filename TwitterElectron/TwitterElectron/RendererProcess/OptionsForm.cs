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
            var okButton = document.getElementById("okButton");
            var cancelButton = document.getElementById("cancelButton");

            Electron.ipcRenderer.on(Constants.IPC.RestoreOptions, new Action<Event, TwitterCredentials>((ev, cred) =>
            {
                document.getElementById("apiKeyInput").As<HTMLInputElement>().value = cred?.ApiKey;
                document.getElementById("apiSecretInput").As<HTMLInputElement>().value = cred?.ApiSecret;
                document.getElementById("accessTokenInput").As<HTMLInputElement>().value = cred?.AccessToken;
                document.getElementById("accessTokenSecretInput").As<HTMLInputElement>().value = cred?.AccessTokenSecret;
            }));

            okButton.addEventListener("click", () => 
            {
                var cred = new TwitterCredentials
                {
                    ApiKey = document.getElementById("apiKeyInput").As<HTMLInputElement>().value,
                    ApiSecret = document.getElementById("apiSecretInput").As<HTMLInputElement>().value,
                    AccessToken = document.getElementById("accessTokenInput").As<HTMLInputElement>().value,
                    AccessTokenSecret = document.getElementById("accessTokenSecretInput").As<HTMLInputElement>().value
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