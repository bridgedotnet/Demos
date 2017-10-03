require("./bridge.js");

// The call bellow is required to initialize a global var 'Electron'.
var Electron = require("electron");

// Keep a global reference of the window object, if you don't, the window will
// be closed automatically when the JavaScript object is garbage collected.
var win = null;

/**
 * @version 1.0.0.0
 * @copyright Copyright ©  2017
 * @compiler Bridge.NET 16.3.1
 */
Bridge.assembly("TwitterElectron", function ($asm, globals) {
    "use strict";

    var path = require("path");
    var url = require("url");

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
            app.on("activate", $asm.$.TwitterElectron.MainProcess.App.f1);

            // Init IPC message handlers:
            TwitterElectron.MainProcess.App.InitIPC();
        },
        statics: {
            fields: {
                Electron: null,
                Win: null,
                AppIcon: null,
                ContextMenu: null,
                _credentials: null
            },
            methods: {
                InitIPC: function () {
                    Electron.ipcMain.on("cmd-options-updated", $asm.$.TwitterElectron.MainProcess.App.f2);
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
                        });

                    }, 2000);
                },
                CreateSplashScreen: function () {
                    var options = { };
                    options.width = 505;
                    options.height = 330;
                    options.icon = path.join(__dirname, "Assets/Images/app_icon.png");
                    options.title = "Retyped: Electron + Twitter API Demo";
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
                    options.title = "Retyped: Electron + Twitter API Demo";
                    options.show = false;

                    // Create the browser window.
                    win = new Electron.BrowserWindow(options);
                    TwitterElectron.MainProcess.App.SetMainMenu();

                    TwitterElectron.MainProcess.App.LoadWindow(win, "Forms/MainForm.html");

                    win.on("closed", $asm.$.TwitterElectron.MainProcess.App.f3);

                    win.on("minimize", $asm.$.TwitterElectron.MainProcess.App.f4);
                },
                CreateOptionsWindow: function () {
                    var options = { };
                    options.width = 440;
                    options.height = 453;
                    options.title = "Settings";
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
                    var $t, $t1;
                    var showFn = $asm.$.TwitterElectron.MainProcess.App.f5;

                    var openMenuItem = ($t = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t.label = "Open", $t.click = function () {
                        showFn();
                    }, $t);

                    var captureMenuItem = ($t = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t.label = "Capture", $t.submenu = System.Array.init([($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.label = "Start", $t1.click = $asm.$.TwitterElectron.MainProcess.App.f6, $t1), ($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.label = "Stop", $t1.enabled = false, $t1.click = $asm.$.TwitterElectron.MainProcess.App.f7, $t1)], Bridge.virtualc("Electron.MenuItemConstructorOptions")), $t);

                    var visitMenuItem = ($t = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t.label = "Visit Bridge.NET", $t.click = $asm.$.TwitterElectron.MainProcess.App.f8, $t);

                    var exitMenuItem = ($t = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t.label = "Exit", $t.role = "quit", $t);

                    TwitterElectron.MainProcess.App.ContextMenu = Electron.Menu.buildFromTemplate(System.Array.init([openMenuItem, captureMenuItem, visitMenuItem, exitMenuItem], Bridge.virtualc("Electron.MenuItemConstructorOptions")));

                    TwitterElectron.MainProcess.App.AppIcon = new Electron.Tray(path.join(__dirname, "Assets/Images/app_icon.png"));
                    TwitterElectron.MainProcess.App.AppIcon.setToolTip("Retyped: Electron + Twitter API Demo");
                    TwitterElectron.MainProcess.App.AppIcon.setContextMenu(TwitterElectron.MainProcess.App.ContextMenu);
                    TwitterElectron.MainProcess.App.AppIcon.on("click", function () {
                        showFn();
                    });
                },
                SetMainMenu: function () {
                    var $t, $t1;
                    var fileMenu = ($t = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t.label = "File", $t.submenu = System.Array.init([($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.label = "Options", $t1.accelerator = "F2", $t1.click = $asm.$.TwitterElectron.MainProcess.App.f9, $t1), ($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.type = "separator", $t1), ($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.label = "Exit", $t1.role = "quit", $t1)], Bridge.virtualc("Electron.MenuItemConstructorOptions")), $t);

                    var viewMenu = ($t = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t.label = "View", $t.submenu = System.Array.init([($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.label = "Reload", $t1.accelerator = "Ctrl+R", $t1.click = $asm.$.TwitterElectron.MainProcess.App.f10, $t1), ($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.label = "Toggle Developer Tools", $t1.accelerator = (process.platform === "darwin" ? "Alt+Command+I" : "Ctrl+Shift+I"), $t1.click = $asm.$.TwitterElectron.MainProcess.App.f11, $t1)], Bridge.virtualc("Electron.MenuItemConstructorOptions")), $t);

                    var captureMenu = ($t = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t.label = "Capture", $t.submenu = System.Array.init([($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.label = "Start", $t1.accelerator = "F5", $t1.click = $asm.$.TwitterElectron.MainProcess.App.f12, $t1), ($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.label = "Stop", $t1.accelerator = "F6", $t1.enabled = false, $t1.click = $asm.$.TwitterElectron.MainProcess.App.f13, $t1), ($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.type = "separator", $t1), ($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.label = "Clear captured tweets", $t1.accelerator = "F7", $t1.click = $asm.$.TwitterElectron.MainProcess.App.f14, $t1)], Bridge.virtualc("Electron.MenuItemConstructorOptions")), $t);


                    var helpMenu = ($t = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t.label = "Help", $t.submenu = System.Array.init([($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.label = "Visit Bridge.NET", $t1.click = $asm.$.TwitterElectron.MainProcess.App.f8, $t1), ($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.label = "Visit Retyped.com", $t1.click = $asm.$.TwitterElectron.MainProcess.App.f15, $t1), ($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.type = "separator", $t1), ($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.label = "Visit Electron API demos", $t1.click = $asm.$.TwitterElectron.MainProcess.App.f16, $t1), ($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.label = "Visit Twitter API reference", $t1.click = $asm.$.TwitterElectron.MainProcess.App.f17, $t1), ($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.type = "separator", $t1), ($t1 = new (Bridge.virtualc("Electron.MenuItemConstructorOptions"))(), $t1.label = "About", $t1.click = $asm.$.TwitterElectron.MainProcess.App.f18, $t1)], Bridge.virtualc("Electron.MenuItemConstructorOptions")), $t);

                    var appMenu = Electron.Menu.buildFromTemplate(System.Array.init([fileMenu, captureMenu, viewMenu, helpMenu], Bridge.virtualc("Electron.MenuItemConstructorOptions")));
                    Electron.Menu.setApplicationMenu(appMenu);
                },
                ToggleStartStopMenuItems: function () {
                    var appMenu = Electron.Menu.getApplicationMenu();
                    var captureMenu = Bridge.unbox(System.Linq.Enumerable.from(appMenu.items).first($asm.$.TwitterElectron.MainProcess.App.f19).submenu);

                    var startMenuItem = System.Linq.Enumerable.from(captureMenu.items).first($asm.$.TwitterElectron.MainProcess.App.f20);
                    var stopMenuItem = System.Linq.Enumerable.from(captureMenu.items).first($asm.$.TwitterElectron.MainProcess.App.f21);

                    var isStarted = !startMenuItem.enabled;
                    startMenuItem.enabled = isStarted;
                    stopMenuItem.enabled = !isStarted;

                    if (TwitterElectron.MainProcess.App.AppIcon != null && TwitterElectron.MainProcess.App.ContextMenu != null) {
                        var captureCtxMenu = Bridge.unbox(System.Linq.Enumerable.from(TwitterElectron.MainProcess.App.ContextMenu.items).first($asm.$.TwitterElectron.MainProcess.App.f19).submenu);

                        var startMenuCtxItem = System.Linq.Enumerable.from(captureCtxMenu.items).first($asm.$.TwitterElectron.MainProcess.App.f20);
                        var stopMenuCtxItem = System.Linq.Enumerable.from(captureCtxMenu.items).first($asm.$.TwitterElectron.MainProcess.App.f21);

                        startMenuCtxItem.enabled = isStarted;
                        stopMenuCtxItem.enabled = !isStarted;
                    }

                    win.setTitle(System.String.format("{0} ({1})", "Retyped: Electron + Twitter API Demo", (isStarted ? "Stopped" : "Running")));
                }
            }
        }
    });

    Bridge.ns("TwitterElectron.MainProcess.App", $asm.$);

    Bridge.apply($asm.$.TwitterElectron.MainProcess.App, {
        f1: function (ev, hasVisibleWindows) {
            if (win == null) {
                TwitterElectron.MainProcess.App.StartApp(null);
            }
        },
        f2: function (e, cred) {
            TwitterElectron.MainProcess.App._credentials = cred;
            win.webContents.send("cmd-options-updated", cred);
        },
        f3: function () {
            // Dereference the window object, usually you would store windows
            // in an array if your app supports multi windows, this is the time
            // when you should delete the corresponding element.
            win = null;
        },
        f4: function () {
            win.setSkipTaskbar(true);
            TwitterElectron.MainProcess.App.ShowTrayIcon();
        },
        f5: function () {
            if (win.isMinimized()) {
                win.show();
                win.focus();
            }

            win.setSkipTaskbar(false);
            TwitterElectron.MainProcess.App.AppIcon.destroy();
            TwitterElectron.MainProcess.App.AppIcon = null;
        },
        f6: function () {
            win.webContents.send("cmd-start-capture");
            TwitterElectron.MainProcess.App.ToggleStartStopMenuItems();
        },
        f7: function () {
            win.webContents.send("cmd-stop-capture");
            TwitterElectron.MainProcess.App.ToggleStartStopMenuItems();
        },
        f8: function () {
            Electron.shell.openExternal("http://bridge.net/");
        },
        f9: function (i, w, e) {
            var optionsWin = TwitterElectron.MainProcess.App.CreateOptionsWindow();
            optionsWin.once("ready-to-show", function () {
                optionsWin.webContents.send("cmd-options-restore", TwitterElectron.MainProcess.App._credentials);

                // to prevent showing not rendered window:
                optionsWin.show();
            });
        },
        f10: function (i, w, e) {
            w != null ? w.webContents.reload() : null;
        },
        f11: function (i, w, e) {
            w != null ? w.webContents.toggleDevTools() : null;
        },
        f12: function (i, w, e) {
            win.webContents.send("cmd-start-capture");
            TwitterElectron.MainProcess.App.ToggleStartStopMenuItems();
        },
        f13: function (i, w, e) {
            win.webContents.send("cmd-stop-capture");
            TwitterElectron.MainProcess.App.ToggleStartStopMenuItems();
        },
        f14: function (i, w, e) {
            win.webContents.send("cmd-clear-capture");
        },
        f15: function () {
            Electron.shell.openExternal("https://retyped.com/");
        },
        f16: function () {
            Electron.shell.openExternal("https://github.com/electron/electron-api-demos");
        },
        f17: function () {
            Electron.shell.openExternal("https://dev.twitter.com/streaming/overview");
        },
        f18: function () {
            var msgBoxOpts = { };
            msgBoxOpts.type = "info";
            msgBoxOpts.title = "About";
            msgBoxOpts.buttons = System.Array.init(["OK"], System.String);
            msgBoxOpts.message = System.String.concat("Retyped: Electron + Twitter API Demo.\n\nNode: ", process.versions.node, "\nChrome: ", process.versions.chrome, "\nElectron: ", process.versions.electron);

            Electron.dialog.showMessageBox(msgBoxOpts);
        },
        f19: function (x) {
            return Bridge.referenceEquals(x.label, "Capture");
        },
        f20: function (x) {
            return Bridge.referenceEquals(x.label, "Start");
        },
        f21: function (x) {
            return Bridge.referenceEquals(x.label, "Stop");
        }
    });
});
