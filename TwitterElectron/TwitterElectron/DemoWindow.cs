using Bridge;
using static Retyped.dom;
using static Retyped.electron;
using static Retyped.node;

namespace TwitterElectron
{
    public static class DemoWindow
    {
        [Init(InitPosition.Top)]
        public static void InitGlobals()
        {
            var Electron = (Electron.AllElectron)require.Self("electron");
        }

        [Template("Electron")]
        public static Electron.AllElectron Electron;

        private static TwitterListener _listener;

        private static string _notifFilter;

        public static void Main()
        {
            Electron.ipcRenderer.on("cmd-start-capture", () =>
            {
                _listener = InitListener();
                if (_listener != null)
                {

                    var notifFilterInput = (HTMLInputElement) document.getElementById("notifFilterInput");
                    _notifFilter = notifFilterInput.value.ToLower();

                    var captFilterInput = (HTMLInputElement) document.getElementById("captFilterInput");
                    _listener.Filter = captFilterInput.value;
                    _listener.Start();
                }
            });

            Electron.ipcRenderer.on("cmd-stop-capture", () =>
            {
                _listener?.Stop();
            });

            Electron.ipcRenderer.on("cmd-clear-capture", () =>
            {
                var capturedItemsDiv = (HTMLDivElement)document.getElementById("capturedItemsDiv");
                capturedItemsDiv.innerHTML = "";
            });
        }

        private static TwitterListener InitListener()
        {
            var apiKey = ((HTMLInputElement)document.getElementById("apiKeyInput")).value;
            var apiSecret = ((HTMLInputElement)document.getElementById("apiSecretInput")).value;
            var accessToken = ((HTMLInputElement)document.getElementById("accessTokenInput")).value;
            var accessTokenSecret = ((HTMLInputElement)document.getElementById("accessTokenSecretInput")).value;

            if (string.IsNullOrEmpty(apiKey) ||
                string.IsNullOrEmpty(apiSecret) ||
                string.IsNullOrEmpty(accessToken) ||
                string.IsNullOrEmpty(accessTokenSecret))
            {
                alert("Please specify API keys and Access tokens before starting.");
                return null;
            }

            var listener = new TwitterListener(
                consumerKey: apiKey,
                consumerSecret: apiSecret,
                accessTokenKey: accessToken,
                accessTokenSecret: accessTokenSecret);

            listener.OnReceived += (sender, tweet) =>
            {
                AddRecord(tweet);

                if (tweet.text.ToLower().Contains(_notifFilter))
                {
                    CreateNotification(tweet);
                }
            };

            listener.OnError += (sender, err) =>
            {
                listener.Stop();
            };

            return listener;
        }

        private static void CreateNotification(Tweet tweet)
        {
            var notifTitle = tweet.user.name + " is tweeting..";

            var notifOpts = ObjectLiteral.Create<NotificationOptions>();
            notifOpts.body = tweet.text;
            notifOpts.icon = tweet.user.profile_image_url;

            var notif = new Notification(notifTitle, notifOpts);
            notif.onclick = notifEv =>
            {
                var tweetUrl = $"https://twitter.com/{tweet.user.screen_name}/status/{tweet.id_str}";
                Electron.shell.openExternal(tweetUrl);
                return null;
            };
        }

        private static void AddRecord(Tweet tweet)
        {
            var div = (HTMLDivElement)document.createElement("div");

            div.style.padding = "10px";
            div.style.margin = "10px";
            div.style.backgroundColor = "rgba(133, 181, 249, 0.33)";
            div.style.border = "2px solid orange";
            div.ondblclick += e =>
            {
                var tweetUrl = $"https://twitter.com/{tweet.user.screen_name}/status/{tweet.id_str}";
                Electron.shell.openExternal(tweetUrl);
                return null;
            };

            var img = (HTMLImageElement) document.createElement("img");
            img.width = 48;
            img.height = 48;
            img.src = tweet.user.profile_image_url;

            var nameDiv = (HTMLDivElement)document.createElement("div");
            nameDiv.style.marginTop = "-50px";
            nameDiv.style.marginLeft = "60px";
            nameDiv.style.fontStyle = "italic";
            nameDiv.innerHTML = tweet.user.name + " is tweeting..";

            var textDiv = (HTMLDivElement)document.createElement("div");
            textDiv.style.marginTop = "10px";
            textDiv.style.marginLeft = "60px";
            textDiv.innerHTML = tweet.text;

            div.appendChild(img);
            div.appendChild(nameDiv);
            div.appendChild(textDiv);

            var capturedItemsDiv = (HTMLDivElement)document.getElementById("capturedItemsDiv");
            capturedItemsDiv.appendChild(div);
        }
    }
}