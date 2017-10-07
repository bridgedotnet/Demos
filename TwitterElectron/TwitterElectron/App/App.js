require("./bridge.js");
require("./UserSettings.js");
require("./newtonsoft.json.js");

// The call below is required to initialize a global var 'Electron'.
var Electron = require("electron");

// Keep a global reference of the window object, if you don't, the window will
// be closed automatically when the JavaScript object is garbage collected.
var win = null;

/**
 * @version 1.0.0.0
 * @copyright Copyright ©  2017
 * @compiler Bridge.NET 16.3.2
 */
Bridge.assembly("TwitterElectron", function ($asm, globals) {
    "use strict";

    var path = require("path");
    var url = require("url");
    var fs = require("fs");

    Bridge.define("TwitterElectron.MainProcess.App", {
        main: function Main () {
            var app = Electron.app;

            // This method will be called when Electron has finished
            // initialization and is ready to create browser windows.
            // Some APIs can only be used after this event occurs.
            app.on("ready", TwitterElectron.MainProcess.App.StartApp);

            // Quit when all windows are closed.
            app.on("window-all-closed", function () {
                // On macOS it is common for applications and their menu bar
                // to stay active until the user quits explicitly with Cmd + Q
                if (process.platform !== "darwin") {
                    app.quit();
                }

                TwitterElectron.MainProcess.App.AppIcon != null ? TwitterElectron.MainProcess.App.AppIcon.destroy() : null;
            });

            // On macOS it's common to re-create a window in the app when the
            // dock icon is clicked and there are no other windows open.
            app.on("activate", function (ev, hasVisibleWindows) {
                if (win == null) {
                    TwitterElectron.MainProcess.App.StartApp(null);
                }
            });

            // Load User Settings:
            TwitterElectron.MainProcess.App.LoadUserSettings();

            // Init IPC message handlers:
            TwitterElectron.MainProcess.App.InitIPC();
        },
        statics: {
            fields: {
                Electron: null,
                Win: null,
                AppIcon: null,
                ContextMenu: null,
                _settings: null
            },
            methods: {
                InitIPC: function () {
                    Electron.ipcMain.on("cmd-options-updated", function (e, cred) {
                        TwitterElectron.MainProcess.App._settings.Credentials = cred;
                        TwitterElectron.MainProcess.App.SaveUserSettings();

                        win.webContents.send("cmd-options-updated", cred);
                    });

                    Electron.ipcMain.on("cmd-start-capture", function (e) {
                        TwitterElectron.MainProcess.App.ToggleStartStop(true);
                    });

                    Electron.ipcMain.on("cmd-stop-capture", function (e) {
                        TwitterElectron.MainProcess.App.ToggleStartStop(false);
                    });
                },
                StartApp: function (launchInfo) {
                    var splash = TwitterElectron.MainProcess.App.CreateSplashScreen();

                    splash.once("ready-to-show", function () {
                        // to prevent showing not rendered window:
                        splash.show();
                    });

                    setTimeout(function (args) {
                        TwitterElectron.MainProcess.App.CreateMainWindow();

                        win.once("ready-to-show", function () {
                            // to prevent showing not rendered window:
                            win.show();

                            // Splash screen should be closed after the main window is created
                            // to prevent application from being terminated.
                            splash.close();
                            splash = null;

                            win.focus();

                            if (TwitterElectron.MainProcess.App._settings != null) {
                                win.webContents.send("cmd-options-updated", TwitterElectron.MainProcess.App._settings.Credentials);
                            }
                        });

                    }, 2000);
                },
                CreateSplashScreen: function () {
                    var options = { };
                    options.width = 600;
                    options.height = 400;
                    options.icon = path.join(__dirname, "Assets/Images/app_icon.png");
                    options.title = "Widgetoko: Tweet Catcher";
                    options.frame = false;
                    options.skipTaskbar = true;
                    options.show = false;

                    // Create the browser window.
                    var splash = new Electron.BrowserWindow(options);
                    TwitterElectron.MainProcess.App.LoadWindow(splash, "Forms/SplashScreen.html");

                    return splash;
                },
                CreateMainWindow: function () {
                    var options = { };
                    options.width = 600;
                    options.height = 800;
                    options.icon = path.join(__dirname, "Assets/Images/app_icon.png");
                    options.title = "Widgetoko: Tweet Catcher";
                    options.show = false;

                    // Create the browser window.
                    win = new Electron.BrowserWindow(options);
                    TwitterElectron.MainProcess.App.SetMainMenu();

                    TwitterElectron.MainProcess.App.LoadWindow(win, "Forms/MainForm.html");

                    win.on("closed", function () {
                        // Dereference the window object, usually you would store windows
                        // in an array if your app supports multi windows, this is the time
                        // when you should delete the corresponding element.
                        win = null;
                    });

                    win.on("minimize", function () {
                        win.setSkipTaskbar(true);
                        TwitterElectron.MainProcess.App.ShowTrayIcon();
                    });
                },
                CreateOptionsWindow: function () {
                    var options = { };
                    options.width = 440;
                    options.height = 540;
                    options.title = "Options";
                    options.icon = path.join(__dirname, "Assets/Images/app_icon.png");
                    options.skipTaskbar = true;
                    options.parent = win;
                    options.modal = true;
                    options.show = false;
                    options.maximizable = false;
                    options.minimizable = false;
                    options.resizable = false;

                    // Create the browser window.
                    var optionsWin = new Electron.BrowserWindow(options);

                    TwitterElectron.MainProcess.App.LoadWindow(optionsWin, "Forms/OptionsForm.html");
                    optionsWin.setMenuBarVisibility(false);

                    return optionsWin;
                },
                LoadWindow: function (win, page) {
                    var windowUrl = { };
                    windowUrl.pathname = path.join(__dirname, page);
                    windowUrl.protocol = "file:";
                    windowUrl.slashes = true;

                    var formattedUrl = url.format(windowUrl);

                    // and load the index.html of the app.
                    win.loadURL(formattedUrl);
                },
                ShowTrayIcon: function () {
                    var showFn = function () {
                        if (win.isMinimized()) {
                            win.show();
                            win.focus();
                        }

                        win.setSkipTaskbar(false);
                        TwitterElectron.MainProcess.App.AppIcon.destroy();
                        TwitterElectron.MainProcess.App.AppIcon = null;
                    };

                    var openMenuItem = { label: "Open", click: function () {
                        showFn();
                    } };

                    var captureMenuItem = { label: "Capture", submenu: System.Array.init([{ label: "Start", click: function () {
                        win.webContents.send("cmd-start-capture");
                        TwitterElectron.MainProcess.App.ToggleStartStopMenuItems();
                    } }, { label: "Stop", enabled: false, click: function () {
                        win.webContents.send("cmd-stop-capture");
                        TwitterElectron.MainProcess.App.ToggleStartStopMenuItems();
                    } }], System.Object) };

                    var visitMenuItem = { label: "Visit Bridge.NET", click: function () {
                        Electron.shell.openExternal("http://bridge.net/");
                    } };

                    var exitMenuItem = { label: "Exit", role: "quit" };

                    TwitterElectron.MainProcess.App.ContextMenu = Electron.Menu.buildFromTemplate(System.Array.init([openMenuItem, captureMenuItem, visitMenuItem, exitMenuItem], System.Object));

                    TwitterElectron.MainProcess.App.AppIcon = new Electron.Tray(path.join(__dirname, "Assets/Images/app_icon.png"));
                    TwitterElectron.MainProcess.App.AppIcon.setToolTip("Widgetoko: Tweet Catcher");
                    TwitterElectron.MainProcess.App.AppIcon.setContextMenu(TwitterElectron.MainProcess.App.ContextMenu);
                    TwitterElectron.MainProcess.App.AppIcon.on("click", function () {
                        showFn();
                    });
                },
                SetMainMenu: function () {
                    var fileMenu = { label: "File", submenu: System.Array.init([{ label: "Options", accelerator: "F2", click: function (i, w, e) {
                        var optionsWin = TwitterElectron.MainProcess.App.CreateOptionsWindow();
                        optionsWin.once("ready-to-show", function () {
                            optionsWin.webContents.send("cmd-options-restore", TwitterElectron.MainProcess.App._settings.Credentials);

                            // to prevent showing not rendered window:
                            optionsWin.show();
                        });
                    } }, { type: "separator" }, { label: "Exit", role: "quit" }], System.Object) };

                    var viewMenu = { label: "View", submenu: System.Array.init([{ label: "Reload", accelerator: "Ctrl+R", click: function (i, w, e) {
                        w != null ? w.webContents.reload() : null;
                    } }, { label: "Toggle Developer Tools", accelerator: (process.platform === "darwin" ? "Alt+Command+I" : "Ctrl+Shift+I"), click: function (i, w, e) {
                        w != null ? w.webContents.toggleDevTools() : null;
                    } }, { type: "separator" }, { label: "Theme", submenu: System.Array.init([{ type: "radio", label: "Light", checked: true, click: function (i, w, e) {
                        win.webContents.send("cmd-toggle-theme");
                    } }, { type: "radio", label: "Dark", click: function (i, w, e) {
                        win.webContents.send("cmd-toggle-theme");
                    } }], System.Object) }], System.Object) };

                    var captureMenu = { label: "Capture", submenu: System.Array.init([{ label: "Start", accelerator: "F5", click: function (i, w, e) {
                        TwitterElectron.MainProcess.App.ToggleStartStop(true);
                    } }, { label: "Stop", accelerator: "F6", enabled: false, click: function (i, w, e) {
                        TwitterElectron.MainProcess.App.ToggleStartStop(false);
                    } }, { type: "separator" }, { label: "Clear captured tweets", accelerator: "F7", click: function (i, w, e) {
                        win.webContents.send("cmd-clear-capture");
                    } }], System.Object) };


                    var helpMenu = { label: "Help", submenu: System.Array.init([{ label: "Visit Bridge.NET", click: function () {
                        Electron.shell.openExternal("http://bridge.net/");
                    } }, { label: "Visit Retyped", click: function () {
                        Electron.shell.openExternal("https://retyped.com/");
                    } }, { type: "separator" }, { label: "Visit Electron API Demos", click: function () {
                        Electron.shell.openExternal("https://github.com/electron/electron-api-demos");
                    } }, { label: "Visit Twitter API Reference", click: function () {
                        Electron.shell.openExternal("https://dev.twitter.com/streaming/overview");
                    } }, { type: "separator" }, { label: "About", click: function () {
                        var msgBoxOpts = { };
                        msgBoxOpts.type = "info";
                        msgBoxOpts.title = "About";
                        msgBoxOpts.buttons = System.Array.init(["OK"], System.String);
                        msgBoxOpts.message = System.String.concat(System.String.concat("Widgetoko: Tweet Catcher.\n\nNode: " + (process.versions.node || "") + "\nChrome: ", process.versions.chrome) + "\nElectron: ", process.versions.electron);

                        Electron.dialog.showMessageBox(msgBoxOpts);
                    } }], System.Object) };

                    var appMenu = Electron.Menu.buildFromTemplate(System.Array.init([fileMenu, captureMenu, viewMenu, helpMenu], System.Object));
                    Electron.Menu.setApplicationMenu(appMenu);
                },
                ToggleStartStop: function (isStart) {
                    win.webContents.send(isStart ? "cmd-start-capture" : "cmd-stop-capture");
                    TwitterElectron.MainProcess.App.ToggleStartStopMenuItems();
                },
                ToggleStartStopMenuItems: function () {
                    var appMenu = Electron.Menu.getApplicationMenu();
                    var captureMenu = System.Linq.Enumerable.from(appMenu.items).first(function (x) {
                            return Bridge.referenceEquals(x.label, "Capture");
                        }).submenu;

                    var startMenuItem = System.Linq.Enumerable.from(captureMenu.items).first(function (x) {
                            return Bridge.referenceEquals(x.label, "Start");
                        });
                    var stopMenuItem = System.Linq.Enumerable.from(captureMenu.items).first(function (x) {
                            return Bridge.referenceEquals(x.label, "Stop");
                        });
                    var isStarted = !startMenuItem.enabled;

                    startMenuItem.enabled = isStarted;
                    stopMenuItem.enabled = !isStarted;

                    if (TwitterElectron.MainProcess.App.AppIcon != null && TwitterElectron.MainProcess.App.ContextMenu != null) {
                        var captureCtxMenu = System.Linq.Enumerable.from(TwitterElectron.MainProcess.App.ContextMenu.items).first(function (x) {
                                return Bridge.referenceEquals(x.label, "Capture");
                            }).submenu;

                        var startMenuCtxItem = System.Linq.Enumerable.from(captureCtxMenu.items).first(function (x) {
                                return Bridge.referenceEquals(x.label, "Start");
                            });
                        var stopMenuCtxItem = System.Linq.Enumerable.from(captureCtxMenu.items).first(function (x) {
                                return Bridge.referenceEquals(x.label, "Stop");
                            });

                        startMenuCtxItem.enabled = isStarted;
                        stopMenuCtxItem.enabled = !isStarted;
                    }

                    win.setTitle(System.String.format("{0} ({1})", "Widgetoko: Tweet Catcher", (isStarted ? "Stopped" : "Running")));
                },
                LoadUserSettings: function () {
                    var userDataPath = Electron.app.getPath("userData");
                    var settingsPath = path.join(userDataPath, "UserSettings.json");

                    if (fs.existsSync(settingsPath)) {
                        var fileData = fs.readFileSync(settingsPath, "utf8");
                        TwitterElectron.MainProcess.App._settings = TwitterElectron.MainProcess.UserSettings.Deserialize(fileData);
                    } else {
                        TwitterElectron.MainProcess.App._settings = { };
                    }
                },
                SaveUserSettings: function () {
                    var userDataPath = Electron.app.getPath("userData");
                    var settingsPath = path.join(userDataPath, "UserSettings.json");
                    var data = TwitterElectron.MainProcess.UserSettings.prototype.Serialize.call(TwitterElectron.MainProcess.App._settings);

                    fs.writeFileSync(settingsPath, data);
                }
            }
        }
    });
});
