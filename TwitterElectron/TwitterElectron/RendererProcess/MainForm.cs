using System;
using Bridge;
using TwitterElectron.Twitter;
using TwitterElectron.Twitter.API;
using static Retyped.dom;
using static Retyped.electron;
using static Retyped.node;
using static Retyped.jquery;

namespace TwitterElectron.RendererProcess
{
    public static class MainForm
    {
        private const int MaxTweetsOnPage = 20;
        private const int NotificationBufferTimeSec = 20;

        private const string LightThemeCss = "../Assets/Styles/light.css";
        private const string DarkThemeCss = "../Assets/Styles/dark.css";

        [Init(InitPosition.Top)]
        public static void InitGlobals()
        {
            // Init global variables (modules):
            var Electron = (Electron.AllElectron)require.Self("electron");
            var jQuery = require.Self("jquery");
        }

        [Template("Electron")]
        public static Electron.AllElectron Electron;

        private static TwitterListener _listener;

        private static DateTime? _lastNotificationDate;

        public static void Main()
        {
            // Configure handlers for IPC commands:
            ConfigureIPC();

            // Configure handlers for HtmlElement events:
            ConfigureEventHandlers();

            // Set Theme:
            ToggleTheme();
        }

        private static void ConfigureEventHandlers()
        {
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
        }

        private static void ConfigureIPC()
        {
            Electron.ipcRenderer.on(Constants.IPC.StartCapture, () =>
            {
                jQuery.select("#placeholder").hide();
                jQuery.select(".play").hide();
                jQuery.select(".pause").show();

                _listener = InitListener();
                if (_listener != null)
                {
                    _listener.Start();
                }
                else
                {
                    // Can't start capturing due to some error (no filter, no credentials)
                    Electron.ipcRenderer.send(Constants.IPC.StopCapture);
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
                jQuery.select("#capturedItemsDiv").html("");
                jQuery.select("#placeholder").show();
            });

            Electron.ipcRenderer.on(Constants.IPC.ToggleTheme, new Action<Electron.Event>(ev =>
            {
                ToggleTheme();
            }));
        }

        private static TwitterListener InitListener()
        {
            // Get credentials from the main process:
            var credentials = (TwitterCredentials) Electron.ipcRenderer.sendSync(Constants.IPC.GetCredentialsSync);

            // Check credentials:
            if (credentials == null ||
                string.IsNullOrEmpty(credentials.ApiKey) ||
                string.IsNullOrEmpty(credentials.ApiSecret) ||
                string.IsNullOrEmpty(credentials.AccessToken) ||
                string.IsNullOrEmpty(credentials.AccessTokenSecret))
            {
                alert("Please specify API keys and Access tokens before starting.");

                return null;
            }

            // Check filter value:
            var filter = (string) jQuery.select("#captureFilterInput").val();
            if (string.IsNullOrEmpty(filter))
            {
                alert("Please specify filter value.");

                return null;
            }

            // Create Twitter stream listener:
            var listener = new TwitterListener(credentials, filter);

            // Configure handlers for the created listener events:
            listener.OnReceived += (sender, tweet) =>
            {
                AddTweetToPage(tweet);

                // Notify about the obtained tweet:
                var notificationEnabled = jQuery.select("#notificationEnabledCheckbox").@is(":checked");
                if (notificationEnabled)
                {
                    // Use delay to avoid creating too many notifications:
                    if (_lastNotificationDate == null ||
                        (DateTime.UtcNow - _lastNotificationDate.Value).TotalSeconds > NotificationBufferTimeSec)
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
                // Stop capturing on error:
                Electron.ipcRenderer.send(Constants.IPC.StopCapture);
                alert($"Error: {err}");
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

        private static void AddTweetToPage(Tweet tweet)
        {
            var div = new HTMLDivElement
            {
                className = "tweet-card animated slideInRight"
            };

            div.ondblclick += e =>
            {
                var tweetUrl = $"https://twitter.com/{tweet.user.screen_name}/status/{tweet.id_str}";
                Electron.shell.openExternal(tweetUrl);

                return null;
            };

            var img = new HTMLImageElement
            {
                className = "avatar",
                src = tweet.user.profile_image_url
            };

            var nameDiv = new HTMLDivElement
            {
                className = "username",
                innerHTML = tweet.user.name + "<span class='istweeting'> is tweeting...</span>"
            };

            var textDiv = new HTMLDivElement
            {
                className = "tweet-text",
                innerHTML = tweet.text
            };

            var tweetContent = new HTMLDivElement {className = "tweet-content"};
            tweetContent.appendChild(nameDiv);
            tweetContent.appendChild(textDiv);

            div.appendChild(img);
            div.appendChild(tweetContent);

            var capturedItemsDiv = jQuery.select("#capturedItemsDiv");
            var capturedItems = capturedItemsDiv.children();

            if (capturedItems.length > 0)
            {
                if (capturedItems.length >= MaxTweetsOnPage)
                {
                    capturedItems[MaxTweetsOnPage - 1].remove();
                }
            }

            capturedItemsDiv.prepend(div);
        }
    }
}