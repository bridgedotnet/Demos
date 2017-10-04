using System;
using System.Linq;
using Bridge;
using TwitterElectron.RendererProcess;
using static Retyped.electron.Electron;
using static Retyped.node;
using static Retyped.node.url;
using Platform = Retyped.node.Literals.Options.Platform;
using lit = Retyped.electron.Literals;

namespace TwitterElectron.MainProcess
{
    public class App
    {
        [Init(InitPosition.Top)]
        public static void InitGlobals()
        {
            require.Self("./bridge.js");
            require.Self("./UserSettings.js");
            require.Self("./newtonsoft.json.js");

            // The call bellow is required to initialize a global var 'Electron'.
            var Electron = (AllElectron)require.Self("electron");

            // Keep a global reference of the window object, if you don't, the window will
            // be closed automatically when the JavaScript object is garbage collected.
            BrowserWindow win = null;
        }

        [Template("Electron")]
        public static AllElectron Electron;

        [Template("win")]
        public static BrowserWindow Win;

        public static Tray AppIcon;
        public static Menu ContextMenu;

        private static UserSettings _settings;

        public static void Main()
        {
            var app = Electron.app;

            // This method will be called when Electron has finished
            // initialization and is ready to create browser windows.
            // Some APIs can only be used after this event occurs.
            app.on(lit.ready, StartApp);

            // Quit when all windows are closed.
            app.on(lit.window_all_closed, () =>
            {
                // On macOS it is common for applications and their menu bar
                // to stay active until the user quits explicitly with Cmd + Q
                if (process.platform != Platform.darwin)
                {
                    app.quit();
                }

                AppIcon?.destroy();
            });

            // On macOS it's common to re-create a window in the app when the
            // dock icon is clicked and there are no other windows open.
            app.on(lit.activate, (ev, hasVisibleWindows) =>
            {
                if (Win == null)
                {
                    StartApp(null);
                }
            });

            // Load User Settings:
            LoadUserSettings();

            // Init IPC message handlers:
            InitIPC();
        }

        private static void InitIPC()
        {
            Electron.ipcMain.on(Constants.IPC.OptionsUpdated, new Action<Event, TwitterCredentials>((e, cred) =>
            {
                _settings.Credentials = cred;
                SaveUserSettings();

                Win.webContents.send(Constants.IPC.OptionsUpdated, cred);
            }));

            Electron.ipcMain.on(Constants.IPC.StartCapture, new Action<Event>(e =>
            {
                ToggleStartStop(true);
            }));

            Electron.ipcMain.on(Constants.IPC.StopCapture, new Action<Event>(e =>
            {
                ToggleStartStop(false);
            }));
        }

        private static void StartApp(object launchInfo)
        {
            var splash = CreateSplashScreen();
            splash.once(lit.ready_to_show, () =>
            {
                // to prevent showing not rendered window:
                splash.show();
            });

            setTimeout(args =>
            {
                CreateMainWindow();
                Win.once(lit.ready_to_show, () =>
                {
                    // to prevent showing not rendered window:
                    Win.show();

                    // Splash screen should be closed after the main window is created
                    // to prevent application from being terminated.
                    splash.close();
                    splash = null;

                    Win.focus();

                    if (_settings != null)
                    {
                        Win.webContents.send(Constants.IPC.OptionsUpdated, _settings.Credentials);
                    }
                });

            }, 2000);
        }

        private static BrowserWindow CreateSplashScreen()
        {
            var options = ObjectLiteral.Create<BrowserWindowConstructorOptions>();
            options.width = 600;
            options.height = 400;
            options.icon = path.join(__dirname, "Assets/Images/app_icon.png");
            options.title = Constants.AppTitle;
            options.frame = false;
            options.skipTaskbar = true;
            options.show = false;

            // Create the browser window.
            var splash = new BrowserWindow(options);
            LoadWindow(splash, "Forms/SplashScreen.html");
            return splash;
        }

        private static void CreateMainWindow()
        {
            var options = ObjectLiteral.Create<BrowserWindowConstructorOptions>();
            options.width = 600;
            options.height = 800;
            options.icon = path.join(__dirname, "Assets/Images/app_icon.png");
            options.title = Constants.AppTitle;
            options.show = false;

            // Create the browser window.
            Win = new BrowserWindow(options);
            SetMainMenu();

            LoadWindow(Win, "Forms/MainForm.html");

            Win.on(lit.closed, () =>
            {
                // Dereference the window object, usually you would store windows
                // in an array if your app supports multi windows, this is the time
                // when you should delete the corresponding element.
                Win = null;
            });

            Win.on(lit.minimize, () =>
            {
                Win.setSkipTaskbar(true);
                ShowTrayIcon();
            });
        }

        private static BrowserWindow CreateOptionsWindow()
        {
            var options = ObjectLiteral.Create<BrowserWindowConstructorOptions>();
            options.width = 440;
            options.height = 540;
            options.title = "Settings";
            options.icon = path.join(__dirname, "Assets/Images/app_icon.png");
            options.skipTaskbar = true;
            options.parent = Win;
            options.modal = true;
            options.show = false;
            options.maximizable = false;
            options.minimizable = false;
            options.resizable = false;

            // Create the browser window.
            var optionsWin = new BrowserWindow(options);
            LoadWindow(optionsWin, "Forms/OptionsForm.html");
            optionsWin.setMenuBarVisibility(false);

            return optionsWin;
        }

        private static void LoadWindow(BrowserWindow win, string page)
        {
            var windowUrl = ObjectLiteral.Create<Url>();
            windowUrl.pathname = path.join(__dirname, page);
            windowUrl.protocol = "file:";
            windowUrl.slashes = true;

            var formattedUrl = url.format(windowUrl);

            // and load the index.html of the app.
            win.loadURL(formattedUrl);
        }

        private static void ShowTrayIcon()
        {
            Action showFn = () =>
            {
                if (Win.isMinimized())
                {
                    Win.show();
                    Win.focus();
                }

                Win.setSkipTaskbar(false);
                AppIcon.destroy();
                AppIcon = null;
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
                            Win.webContents.send(Constants.IPC.StartCapture);
                            ToggleStartStopMenuItems();
                        }
                    },
                    new MenuItemConstructorOptions
                    {
                        label = "Stop",
                        enabled = false,
                        click = delegate
                        {
                            Win.webContents.send(Constants.IPC.StopCapture);
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

            ContextMenu = Menu.buildFromTemplate(new[] { openMenuItem, captureMenuItem, visitMenuItem, exitMenuItem });

            AppIcon = new Tray(path.join(__dirname, "Assets/Images/app_icon.png"));
            AppIcon.setToolTip(Constants.AppTitle);
            AppIcon.setContextMenu(ContextMenu);
            AppIcon.on("click", () =>
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
                        label = "Options",
                        accelerator = "F2".As<Accelerator>(),
                        click = (i, w, e) =>
                        {
                            var optionsWin = CreateOptionsWindow();
                            optionsWin.once(lit.ready_to_show, () =>
                            {
                                optionsWin.webContents.send(Constants.IPC.RestoreOptions, _settings.Credentials);

                                // to prevent showing not rendered window:
                                optionsWin.show();
                            });
                        }
                    },
                    new MenuItemConstructorOptions
                    {
                        type = lit.separator
                    },
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
                        label = "Reload",
                        accelerator = "Ctrl+R".As<Accelerator>(),
                        click = (i, w, e) =>
                        {
                            w?.webContents.reload();
                        }
                    },
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
                            ToggleStartStop(true);
                        }
                    },
                    new MenuItemConstructorOptions
                    {
                        label = "Stop",
                        accelerator = "F6".As<Accelerator>(),
                        enabled = false,
                        click = (i, w, e) =>
                        {
                            ToggleStartStop(false);
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
                            Win.webContents.send(Constants.IPC.ClearCapture);
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
                            msgBoxOpts.message = Constants.AppTitle + @".

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

        private static void ToggleStartStop(bool isStart)
        {
            Win.webContents.send(isStart ? Constants.IPC.StartCapture : Constants.IPC.StopCapture);
            ToggleStartStopMenuItems();
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

            if (AppIcon != null && ContextMenu != null)
            {
                var captureCtxMenu = ContextMenu.items
                    .First(x => x.label == "Capture")
                    ["submenu"]
                    .As<Menu>();

                var startMenuCtxItem = captureCtxMenu.items.First(x => x.label == "Start");
                var stopMenuCtxItem = captureCtxMenu.items.First(x => x.label == "Stop");

                startMenuCtxItem.enabled = isStarted;
                stopMenuCtxItem.enabled = !isStarted;
            }

            Win.setTitle($"{Constants.AppTitle} ({(isStarted ? "Stopped" : "Running")})");
        }

        private static void LoadUserSettings()
        {
            var userDataPath = Electron.app.getPath("userData");
            var settingsPath = path.join(userDataPath, Constants.UserSettingsFileName);

            if (fs.existsSync(settingsPath))
            {
                var fileData = fs.readFileSync(settingsPath, "utf8");
                _settings = UserSettings.Deserialize(fileData);
            }
            else
            {
                _settings = new UserSettings();
            }
        }

        private static void SaveUserSettings()
        {
            var userDataPath = Electron.app.getPath("userData");
            var settingsPath = path.join(userDataPath, Constants.UserSettingsFileName);

            var data = _settings.Serialize();
            fs.writeFileSync(settingsPath, data);
        }
    }
}