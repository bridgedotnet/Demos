using System;
using Bridge;
using static Retyped.dom;
using static Retyped.electron;
using static Retyped.node;
using static Retyped.jquery;

namespace TwitterElectron.RendererProcess
{
    public static class MainForm
    {
        private const string LightThemeCss = "../Assets/Styles/light.css";
        private const string DarkThemeCss = "../Assets/Styles/dark.css";

        [Init(InitPosition.Top)]
        public static void InitGlobals()
        {
            var Electron = (Electron.AllElectron)require.Self("electron");
            var jQuery = require.Self("jquery");
        }

        [Template("Electron")]
        public static Electron.AllElectron Electron;

        private static TwitterListener _listener;

        private static DateTime? _lastNotificationDate;

        private static TwitterCredentials _credentials;

        public static void Main()
        {
            ToggleTheme();

            jQuery.select(".play").on("click", (e, args) =>
            {
                Electron.ipcRenderer.send(Constants.IPC.StartCapture);
                return null;
            });

            jQuery.select(".pause").on("click", (e, args) =>
            {
                Electron.ipcRenderer.send(Constants.IPC.StopCapture);
                return null;
            });

            Electron.ipcRenderer.on(Constants.IPC.OptionsUpdated, new Action<Electron.Event, TwitterCredentials>((ev, cred) =>
            {
                _credentials = cred;
            }));

            Electron.ipcRenderer.on(Constants.IPC.StartCapture, () =>
            {
                jQuery.select("#placeholder").hide();
                jQuery.select(".play").hide();
                jQuery.select(".pause").show();

                _listener = InitListener();

                if (_listener != null)
                {
                    var captureFilterInput = (HTMLInputElement) document.getElementById("captureFilterInput");
                    _listener.Filter = captureFilterInput.value;
                    _listener.Start();
                }
            });

            Electron.ipcRenderer.on(Constants.IPC.StopCapture, () =>
            {
                jQuery.select(".pause").hide();
                jQuery.select(".play").show();

                _listener?.Stop();
            });

            Electron.ipcRenderer.on(Constants.IPC.ClearCapture, () =>
            {
                var capturedItemsDiv = (HTMLDivElement)document.getElementById("capturedItemsDiv");
                capturedItemsDiv.innerHTML = "";
                jQuery.select("#placeholder").show();
            });

            Electron.ipcRenderer.on(Constants.IPC.ToggleTheme, new Action<Electron.Event>(ev =>
            {
                ToggleTheme();
            }));
        }

        private static TwitterListener InitListener()
        {
            if (_credentials == null ||
                string.IsNullOrEmpty(_credentials.ApiKey) ||
                string.IsNullOrEmpty(_credentials.ApiSecret) ||
                string.IsNullOrEmpty(_credentials.AccessToken) ||
                string.IsNullOrEmpty(_credentials.AccessTokenSecret))
            {
                alert("Please specify API keys and Access tokens before starting.");
                return null;
            }

            var listener = new TwitterListener(
                consumerKey: _credentials.ApiKey,
                consumerSecret: _credentials.ApiSecret,
                accessTokenKey: _credentials.AccessToken,
                accessTokenSecret: _credentials.AccessTokenSecret);

            listener.OnReceived += (sender, tweet) =>
            {
                AddRecord(tweet);

                // Notify:
                var notificationEnabledCheckbox = (HTMLInputElement)document.getElementById("notificationEnabledCheckbox");
                var notificationEnabled = notificationEnabledCheckbox.@checked;
                if (notificationEnabled)
                {
                    // Use 20 seconds buffer to not create too many notifications:
                    if (_lastNotificationDate == null ||
                        (DateTime.UtcNow - _lastNotificationDate.Value).TotalSeconds > 20)
                    {
                        _lastNotificationDate = DateTime.UtcNow;
                        CreateNotification(tweet);
                    }
                }
                else
                {
                    _lastNotificationDate = null;
                }
            };

            listener.OnError += (sender, err) =>
            {
                listener.Stop();
            };

            return listener;
        }

        private static void ToggleTheme()
        {
            var lightThemeLink = jQuery.select($"link[href='{LightThemeCss}']");
            var darkThemeLink = jQuery.select($"link[href='{DarkThemeCss}']");

            var newTheme = lightThemeLink.length == 0
                ? LightThemeCss
                : DarkThemeCss;

            if (lightThemeLink.length == 0)
            {
                darkThemeLink.remove();
            }
            else if (darkThemeLink.length == 0)
            {
                lightThemeLink.remove();
            }

            jQuery.select("head").append($"<link rel=\"stylesheet\" href=\"{newTheme}\" >");
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
            var div = new HTMLDivElement();
            div.className = "tweet-card animated slideInRight";

            div.ondblclick += e =>
            {
                var tweetUrl = $"https://twitter.com/{tweet.user.screen_name}/status/{tweet.id_str}";
                Electron.shell.openExternal(tweetUrl);

                return null;
            };

            var img = new HTMLImageElement();
            img.className = "avatar";
            img.src = tweet.user.profile_image_url;

            var nameDiv = new HTMLDivElement();
            nameDiv.className = "username";
            nameDiv.innerHTML = tweet.user.name + "<span class='istweeting'> is tweeting...</span>";

            var textDiv = new HTMLDivElement();
            textDiv.className = "tweet-text";
            textDiv.innerHTML = tweet.text;

            var tweetContent = new HTMLDivElement();
            tweetContent.className = "tweet-content";
            tweetContent.appendChild(nameDiv);
            tweetContent.appendChild(textDiv);

            div.appendChild(img);
            div.appendChild(tweetContent);

            var capturedItemsDiv = (HTMLDivElement)document.getElementById("capturedItemsDiv");
            if (capturedItemsDiv.children.length >= 20)
            {
                capturedItemsDiv.removeChild(capturedItemsDiv.children[19]);
            }

            if (capturedItemsDiv.children.length > 0)
            {
                capturedItemsDiv.insertBefore(div, capturedItemsDiv.children[0]);
            }
            else
            {
                capturedItemsDiv.appendChild(div);
            }
        }
    }
}