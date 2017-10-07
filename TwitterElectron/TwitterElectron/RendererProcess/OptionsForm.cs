using Bridge;
using TwitterElectron.Twitter;
using static Retyped.electron;
using static Retyped.node;
using static Retyped.jquery;

namespace TwitterElectron.RendererProcess
{
    public class OptionsForm
    {
        [Init(InitPosition.Top)]
        public static void InitGlobals()
        {
            // Init global variables (modules):
            var Electron = (Electron.AllElectron)require.Self("electron");
            var jQuery = require.Self("jquery");
        }

        [Template("Electron")]
        public static Electron.AllElectron Electron;

        public static void Main()
        {
            ConfigureEventHandlers();

            // Get credentials from the main process:
            var credentials = (TwitterCredentials)Electron.ipcRenderer.sendSync(Constants.IPC.GetCredentialsSync);

            // Display values on the form:
            jQuery.select("#apiKeyInput").val(credentials?.ApiKey);
            jQuery.select("#apiSecretInput").val(credentials?.ApiSecret);
            jQuery.select("#accessTokenInput").val(credentials?.AccessToken);
            jQuery.select("#accessTokenSecretInput").val(credentials?.AccessTokenSecret);
        }

        private static void ConfigureEventHandlers()
        {
            jQuery.select("#okButton").on("click", (e, args) =>
            {
                var cred = new TwitterCredentials
                {
                    ApiKey = jQuery.select("#apiKeyInput").val() as string,
                    ApiSecret = jQuery.select("#apiSecretInput").val() as string,
                    AccessToken = jQuery.select("#accessTokenInput").val() as string,
                    AccessTokenSecret = jQuery.select("#accessTokenSecretInput").val() as string,
                };

                Electron.ipcRenderer.send(Constants.IPC.SetCredentials, cred);
                Electron.remote.getCurrentWindow().close();

                return null;
            });

            jQuery.select("#cancelButton").on("click", (e, args) =>
            {
                Electron.remote.getCurrentWindow().close();

                return null;
            });
        }
    }
}