var Electron = require("electron");
var jQuery = require("jquery");

Bridge.assembly("TwitterElectron", function ($asm, globals) {
    "use strict";

    Bridge.define("TwitterElectron.RendererProcess.MainForm", {
        main: function Main () {
            TwitterElectron.RendererProcess.MainForm.ToggleTheme();

            jQuery(".play").on("click", $asm.$.TwitterElectron.RendererProcess.MainForm.f1);

            jQuery(".pause").on("click", $asm.$.TwitterElectron.RendererProcess.MainForm.f2);

            Electron.ipcRenderer.on("cmd-options-updated", $asm.$.TwitterElectron.RendererProcess.MainForm.f3);

            Electron.ipcRenderer.on("cmd-start-capture", $asm.$.TwitterElectron.RendererProcess.MainForm.f4);

            Electron.ipcRenderer.on("cmd-stop-capture", $asm.$.TwitterElectron.RendererProcess.MainForm.f5);

            Electron.ipcRenderer.on("cmd-clear-capture", $asm.$.TwitterElectron.RendererProcess.MainForm.f6);

            Electron.ipcRenderer.on("cmd-toggle-theme", $asm.$.TwitterElectron.RendererProcess.MainForm.f7);
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

                    listener.addOnReceived($asm.$.TwitterElectron.RendererProcess.MainForm.f8);

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

                    var capturedItemsDiv = Bridge.cast(document.getElementById("capturedItemsDiv"), HTMLDivElement);
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

    Bridge.ns("TwitterElectron.RendererProcess.MainForm", $asm.$);

    Bridge.apply($asm.$.TwitterElectron.RendererProcess.MainForm, {
        f1: function (e, args) {
            Electron.ipcRenderer.send("cmd-start-capture");
            return null;
        },
        f2: function (e, args) {
            Electron.ipcRenderer.send("cmd-stop-capture");
            return null;
        },
        f3: function (ev, cred) {
            TwitterElectron.RendererProcess.MainForm._credentials = cred;
        },
        f4: function () {
            jQuery("#placeholder").hide();
            jQuery(".play").hide();
            jQuery(".pause").show();

            TwitterElectron.RendererProcess.MainForm._listener = TwitterElectron.RendererProcess.MainForm.InitListener();

            if (TwitterElectron.RendererProcess.MainForm._listener != null) {
                var captureFilterInput = Bridge.cast(document.getElementById("captureFilterInput"), HTMLInputElement);
                TwitterElectron.RendererProcess.MainForm._listener.Filter = captureFilterInput.value;
                TwitterElectron.RendererProcess.MainForm._listener.Start();
            }
        },
        f5: function () {
            jQuery(".pause").hide();
            jQuery(".play").show();

            TwitterElectron.RendererProcess.MainForm._listener != null ? TwitterElectron.RendererProcess.MainForm._listener.Stop() : null;
        },
        f6: function () {
            var capturedItemsDiv = Bridge.cast(document.getElementById("capturedItemsDiv"), HTMLDivElement);
            capturedItemsDiv.innerHTML = "";
            jQuery("#placeholder").show();
        },
        f7: function (ev) {
            TwitterElectron.RendererProcess.MainForm.ToggleTheme();
        },
        f8: function (sender, tweet) {
            TwitterElectron.RendererProcess.MainForm.AddRecord(tweet);

            // Notify:
            var notificationEnabledCheckbox = Bridge.cast(document.getElementById("notificationEnabledCheckbox"), HTMLInputElement);
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
        }
    });
});
