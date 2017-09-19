using System;
using System.Linq;
using Bridge;
using static Retyped.electron.Electron;
using static Retyped.node;
using static Retyped.node.url;
using Platform = Retyped.node.Literals.Options.Platform;
using lit = Retyped.electron.Literals;

namespace TwitterElectron
{
    public class App
    {
        [Init(InitPosition.Top)]
        public static void InitGlobals()
        {
            require.Self("./bridge.js");

            var Electron = (AllElectron)require.Self("electron");

            // Keep a global reference of the window object, if you don't, the window will
            // be closed automatically when the JavaScript object is garbage collected.
            BrowserWindow win = null;
        }

        [Template("Electron")]
        public static AllElectron Electron;

        [Template("win")]
        public static BrowserWindow win;

        public static Tray appIcon;
        public static Menu contextMenu;

        public static void Main()
        {
            var app = Electron.app;

            // This method will be called when Electron has finished
            // initialization and is ready to create browser windows.
            // Some APIs can only be used after this event occurs.
            app.on(lit.ready, CreateWindow);


            // Quit when all windows are closed.
            app.on(lit.window_all_closed, () =>
            {
                // On macOS it is common for applications and their menu bar
                // to stay active until the user quits explicitly with Cmd + Q
                if (process.platform != Platform.darwin)
                {
                    app.quit();
                }

                appIcon?.destroy();
            });

            // On macOS it's common to re-create a window in the app when the
            // dock icon is clicked and there are no other windows open.
            app.on(lit.activate, (ev, hasVisibleWindows) =>
            {
                if (win == null)
                {
                    CreateWindow(null);
                }
            });
        }

        private static void CreateWindow(dynamic launchInfo)
        {
            var options = ObjectLiteral.Create<BrowserWindowConstructorOptions>();
            options.width = 800;
            options.height = 600;
            options.icon = path.join(__dirname, "../../../app_icon.png");
            options.title = "Retyped: Electron Demo";

            // Create the browser window.
            win = new BrowserWindow(options);
            SetMainMenu();

            LoadWindow("DemoWindow.html");

            win.on(lit.closed, () => 
            {
                // Dereference the window object, usually you would store windows
                // in an array if your app supports multi windows, this is the time
                // when you should delete the corresponding element.
                win = null;
            });

            win.on(lit.minimize, () =>
            {
                win.setSkipTaskbar(true);
                ShowTrayIcon();
            });
        }

        private static void LoadWindow(string page)
        {
            var mainUrl = ObjectLiteral.Create<Url>();
            mainUrl.pathname = path.join(__dirname, page);
            mainUrl.protocol = "file:";
            mainUrl.slashes = true;

            var formattedUrl = url.format(mainUrl);

            // and load the index.html of the app.
            win.loadURL(formattedUrl);
        }

        private static void ShowTrayIcon()
        {
            Action showFn = () =>
            {
                if (win.isMinimized())
                {
                    win.show();
                    win.focus();
                }

                win.setSkipTaskbar(false);
                appIcon.destroy();
                appIcon = null;
            };

            var openMenuItem = new MenuItemConstructorOptions
            {
                label = "Open",
                click = delegate { showFn(); }
            };

            var captureMenuItem = new MenuItemConstructorOptions
            {
                label = "Capture",
                submenu = new[]
                {
                    new MenuItemConstructorOptions
                    {
                        label = "Start",
                        click = delegate
                        {
                            win.webContents.send("cmd-start-capture");
                            ToggleStartStopMenuItems();
                        }
                    },
                    new MenuItemConstructorOptions
                    {
                        label = "Stop",
                        enabled = false,
                        click = delegate
                        {
                            win.webContents.send("cmd-stop-capture");
                            ToggleStartStopMenuItems();
                        }
                    }
                }
            };

            var visitMenuItem = new MenuItemConstructorOptions
            {
                label = "Visit Bridge.NET",
                click = delegate { Electron.shell.openExternal("http://bridge.net/"); }
            };

            var exitMenuItem = new MenuItemConstructorOptions
            {
                label = "Exit",
                role = "quit"
            };

            contextMenu = Menu.buildFromTemplate(new[] { openMenuItem, captureMenuItem, visitMenuItem, exitMenuItem });

            appIcon = new Tray(path.join(__dirname, "../../../app_icon.png"));
            appIcon.setToolTip("Retyped: Electron Demo");
            appIcon.setContextMenu(contextMenu);
            appIcon.on("click", () =>
            {
                showFn();
            });
        }

        private static void SetMainMenu()
        {
            var fileMenu = new MenuItemConstructorOptions
            {
                label = "File",
                submenu = new []
                {
                    new MenuItemConstructorOptions
                    {
                        label = "Exit",
                        role = "quit"
                    }
                }
            };

            var viewMenu = new MenuItemConstructorOptions
            {
                label = "View",
                submenu = new []
                {
                    new MenuItemConstructorOptions
                    {
                        label = "Toggle Developer Tools",
                        accelerator = (process.platform == Platform.darwin
                            ? "Alt+Command+I"
                            : "Ctrl+Shift+I").As<Accelerator>(),
                        click = (i, w, e) =>
                        {
                            w?.webContents.toggleDevTools();
                        }
                    }
                }
            };

            var captureMenu = new MenuItemConstructorOptions
            {
                label = "Capture",
                submenu = new[]
                {
                    new MenuItemConstructorOptions
                    {
                        label = "Start",
                        accelerator = "F5".As<Accelerator>(),
                        click = (i, w, e) =>
                        {
                            win.webContents.send("cmd-start-capture");
                            ToggleStartStopMenuItems();
                        }
                    },
                    new MenuItemConstructorOptions
                    {
                        label = "Stop",
                        accelerator = "F6".As<Accelerator>(),
                        enabled = false,
                        click = (i, w, e) =>
                        {
                            win.webContents.send("cmd-stop-capture");
                            ToggleStartStopMenuItems();
                        }
                    },
                    new MenuItemConstructorOptions
                    {
                        type = lit.separator
                    },
                    new MenuItemConstructorOptions
                    {
                        label = "Clear captured tweets",
                        accelerator = "F7".As<Accelerator>(),
                        click = (i, w, e) =>
                        {
                            win.webContents.send("cmd-clear-capture");
                        }
                    }
                }
            };


            var helpMenu = new MenuItemConstructorOptions
            {
                label = "Help",
                submenu = new[]
                {
                    new MenuItemConstructorOptions
                    {
                        label = "Visit Bridge.NET",
                        click = delegate
                        {
                            Electron.shell.openExternal("http://bridge.net/");
                        }
                    },
                    new MenuItemConstructorOptions
                    {
                        label = "Visit Retyped.com",
                        click = delegate { Electron.shell.openExternal("https://retyped.com/"); }
                    },
                    new MenuItemConstructorOptions
                    {
                        type = lit.separator
                    },
                    new MenuItemConstructorOptions
                    {
                        label = "Visit Electron API demos",
                        click = delegate { Electron.shell.openExternal("https://github.com/electron/electron-api-demos"); }
                    },
                    new MenuItemConstructorOptions
                    {
                        label = "Visit Twitter API reference",
                        click = delegate { Electron.shell.openExternal("https://dev.twitter.com/streaming/overview"); }
                    },
                    new MenuItemConstructorOptions
                    {
                        type = lit.separator
                    },
                    new MenuItemConstructorOptions
                    {
                        label = "About",
                        click = delegate
                        {
                            var msgBoxOpts = ObjectLiteral.Create<MessageBoxOptions>();
                            msgBoxOpts.type = "info";
                            msgBoxOpts.title = "About";
                            msgBoxOpts.buttons = new[] {"OK"};
                            msgBoxOpts.message = @"Retyped: Electron demo application.

Node: " + process.versions.node + @"
Chrome: " + process.versions["chrome"] + @"
Electron: " + process.versions["electron"];

                            Electron.dialog.showMessageBox(msgBoxOpts);
                        }
                    }
                }
            };

            var appMenu = Menu.buildFromTemplate(new[] { fileMenu, captureMenu, viewMenu, helpMenu });
            Menu.setApplicationMenu(appMenu);
        }

        private static void ToggleStartStopMenuItems()
        {
            var appMenu = Menu.getApplicationMenu();
            var captureMenu = appMenu.items
                .First(x => x.label == "Capture")
                ["submenu"]
                .As<Menu>();

            var startMenuItem = captureMenu.items.First(x => x.label == "Start");
            var stopMenuItem = captureMenu.items.First(x => x.label == "Stop");

            var isStarted = !startMenuItem.enabled;
            startMenuItem.enabled = isStarted;
            stopMenuItem.enabled = !isStarted;

            if (appIcon != null && contextMenu != null)
            {
                var captureCtxMenu = contextMenu.items
                    .First(x => x.label == "Capture")
                    ["submenu"]
                    .As<Menu>();

                var startMenuCtxItem = captureCtxMenu.items.First(x => x.label == "Start");
                var stopMenuCtxItem = captureCtxMenu.items.First(x => x.label == "Stop");

                startMenuCtxItem.enabled = isStarted;
                stopMenuCtxItem.enabled = !isStarted;
            }

            win.setTitle($"Retyped: Electron Demo ({(isStarted ? "Stopped" : "Running")})");
        }
    }
}