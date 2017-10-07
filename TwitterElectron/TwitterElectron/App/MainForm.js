var Electron = require("electron");
var jQuery = require("jquery");

Bridge.assembly("TwitterElectron", function ($asm, globals) {
    "use strict";

    Bridge.define("TwitterElectron.RendererProcess.MainForm", {
        main: function Main () {
            TwitterElectron.RendererProcess.MainForm.ToggleTheme();

            jQuery(".play").on("click", function (e, args) {
                Electron.ipcRenderer.send("cmd-start-capture");
                return null;
            });

            jQuery(".pause").on("click", function (e, args) {
                Electron.ipcRenderer.send("cmd-stop-capture");
                return null;
            });

            Electron.ipcRenderer.on("cmd-options-updated", function (ev, cred) {
                TwitterElectron.RendererProcess.MainForm._credentials = cred;
            });

            Electron.ipcRenderer.on("cmd-start-capture", function () {
                jQuery("#placeholder").hide();
                jQuery(".play").hide();
                jQuery(".pause").show();

                TwitterElectron.RendererProcess.MainForm._listener = TwitterElectron.RendererProcess.MainForm.InitListener();

                if (TwitterElectron.RendererProcess.MainForm._listener != null) {
                    var captureFilterInput = document.getElementById("captureFilterInput");
                    TwitterElectron.RendererProcess.MainForm._listener.Filter = captureFilterInput.value;
                    TwitterElectron.RendererProcess.MainForm._listener.Start();
                }
            });

            Electron.ipcRenderer.on("cmd-stop-capture", function () {
                jQuery(".pause").hide();
                jQuery(".play").show();

                TwitterElectron.RendererProcess.MainForm._listener != null ? TwitterElectron.RendererProcess.MainForm._listener.Stop() : null;
            });

            Electron.ipcRenderer.on("cmd-clear-capture", function () {
                var capturedItemsDiv = document.getElementById("capturedItemsDiv");
                capturedItemsDiv.innerHTML = "";
                jQuery("#placeholder").show();
            });

            Electron.ipcRenderer.on("cmd-toggle-theme", function (ev) {
                TwitterElectron.RendererProcess.MainForm.ToggleTheme();
            });
        },
        statics: {
            fields: {
                LightThemeCss: null,
                DarkThemeCss: null,
                Electron: null,
                _listener: null,
                _lastNotificationDate: null,
                _credentials: null
            },
            ctors: {
                init: function () {
                    this.LightThemeCss = "../Assets/Styles/light.css";
                    this.DarkThemeCss = "../Assets/Styles/dark.css";
                }
            },
            methods: {
                InitListener: function () {
                    if (TwitterElectron.RendererProcess.MainForm._credentials == null || System.String.isNullOrEmpty(TwitterElectron.RendererProcess.MainForm._credentials.ApiKey) || System.String.isNullOrEmpty(TwitterElectron.RendererProcess.MainForm._credentials.ApiSecret) || System.String.isNullOrEmpty(TwitterElectron.RendererProcess.MainForm._credentials.AccessToken) || System.String.isNullOrEmpty(TwitterElectron.RendererProcess.MainForm._credentials.AccessTokenSecret)) {
                        alert("Please specify API keys and Access tokens before starting.");

                        return null;
                    }

                    var listener = new TwitterElectron.RendererProcess.TwitterListener(TwitterElectron.RendererProcess.MainForm._credentials.ApiKey, TwitterElectron.RendererProcess.MainForm._credentials.ApiSecret, TwitterElectron.RendererProcess.MainForm._credentials.AccessToken, TwitterElectron.RendererProcess.MainForm._credentials.AccessTokenSecret);

                    listener.addOnReceived(function (sender, tweet) {
                        TwitterElectron.RendererProcess.MainForm.AddRecord(tweet);

                        // Notify:
                        var notificationEnabledCheckbox = document.getElementById("notificationEnabledCheckbox");
                        var notificationEnabled = notificationEnabledCheckbox.checked;

                        if (notificationEnabled) {
                            // Use 20 seconds buffer to not create too many notifications:
                            if (Bridge.equals(TwitterElectron.RendererProcess.MainForm._lastNotificationDate, null) || (System.DateTime.subdd(System.DateTime.getUtcNow(), System.Nullable.getValue(TwitterElectron.RendererProcess.MainForm._lastNotificationDate))).getTotalSeconds() > 20) {
                                TwitterElectron.RendererProcess.MainForm._lastNotificationDate = System.DateTime.getUtcNow();
                                TwitterElectron.RendererProcess.MainForm.CreateNotification(tweet);
                            }
                        } else {
                            TwitterElectron.RendererProcess.MainForm._lastNotificationDate = null;
                        }
                    });

                    listener.addOnError(function (sender, err) {
                        listener.Stop();
                    });

                    return listener;
                },
                ToggleTheme: function () {
                    var lightThemeLink = jQuery(System.String.format("link[href='{0}']", TwitterElectron.RendererProcess.MainForm.LightThemeCss));
                    var darkThemeLink = jQuery(System.String.format("link[href='{0}']", TwitterElectron.RendererProcess.MainForm.DarkThemeCss));

                    var newTheme = lightThemeLink.length === 0 ? TwitterElectron.RendererProcess.MainForm.LightThemeCss : TwitterElectron.RendererProcess.MainForm.DarkThemeCss;

                    if (lightThemeLink.length === 0) {
                        darkThemeLink.remove();
                    } else if (darkThemeLink.length === 0) {
                        lightThemeLink.remove();
                    }

                    jQuery("head").append(System.String.format("<link rel=\"stylesheet\" href=\"{0}\" >", newTheme));
                },
                CreateNotification: function (tweet) {
                    var notifTitle = (tweet.user.name || "") + " is tweeting..";

                    var notifOpts = { };
                    notifOpts.body = tweet.text;
                    notifOpts.icon = tweet.user.profile_image_url;

                    var notif = new Notification(notifTitle, notifOpts);
                    notif.onclick = function (notifEv) {
                        var tweetUrl = System.String.format("https://twitter.com/{0}/status/{1}", tweet.user.screen_name, tweet.id_str);

                        Electron.shell.openExternal(tweetUrl);

                        return null;
                    };
                },
                AddRecord: function (tweet) {
                    var div = document.createElement("div");
                    div.className = "tweet-card animated slideInRight";

                    div.ondblclick = Bridge.fn.combine(div.ondblclick, function (e) {
                        var tweetUrl = System.String.format("https://twitter.com/{0}/status/{1}", tweet.user.screen_name, tweet.id_str);
                        Electron.shell.openExternal(tweetUrl);

                        return null;
                    });

                    var img = document.createElement("img");
                    img.className = "avatar";
                    img.src = tweet.user.profile_image_url;

                    var nameDiv = document.createElement("div");
                    nameDiv.className = "username";
                    nameDiv.innerHTML = (tweet.user.name || "") + "<span class='istweeting'> is tweeting...</span>";

                    var textDiv = document.createElement("div");
                    textDiv.className = "tweet-text";
                    textDiv.innerHTML = tweet.text;

                    var tweetContent = document.createElement("div");
                    tweetContent.className = "tweet-content";
                    tweetContent.appendChild(nameDiv);
                    tweetContent.appendChild(textDiv);

                    div.appendChild(img);
                    div.appendChild(tweetContent);

                    var capturedItemsDiv = document.getElementById("capturedItemsDiv");

                    if (capturedItemsDiv.children.length >= 20) {
                        capturedItemsDiv.removeChild(capturedItemsDiv.children[19]);
                    }

                    if (capturedItemsDiv.children.length > 0) {
                        capturedItemsDiv.insertBefore(div, capturedItemsDiv.children[0]);
                    } else {
                        capturedItemsDiv.appendChild(div);
                    }
                }
            }
        }
    });
});
