using System;
using System.Linq;
using Bridge;
using Retyped;
using Widgetoko.Twitter;
using Platform = Retyped.node.Literals.Options.Platform;
using lit = Retyped.electron.Literals;

namespace Widgetoko.MainProcess
{
    public class App
    {
        [Init(InitPosition.Top)]
        public static void InitGlobals()
        {
            node.require.Self("./bridge.js");
            node.require.Self("./UserSettings.js");
            node.require.Self("./newtonsoft.json.js");

            // The call below is required to initialize a global var 'Electron'.
            var Electron = (electron.Electron.AllElectron)node.require.Self("electron");

            // Keep a global reference of the window object, if you don't, the window will
            // be closed automatically when the JavaScript object is garbage collected.
            electron.Electron.BrowserWindow win = null;
        }

        [Template("Electron")]
        public static electron.Electron.AllElectron Electron;

        [Template("win")]
        public static electron.Electron.BrowserWindow Win;

        public static electron.Electron.Tray AppIcon;
        public static electron.Electron.Menu ContextMenu;

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
                if (node.process.platform != Platform.darwin)
                {
                    app.quit();
                }
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
            ConfigureIPC();
        }

        private static void ConfigureIPC()
        {
            Electron.ipcMain.on(Constants.IPC.GetCredentialsSync, new Action<electron.Electron.Event>(e =>
            {
                e.returnValue = _settings.Credentials;
            }));

            Electron.ipcMain.on(Constants.IPC.SetCredentials, new Action<electron.Electron.Event, TwitterCredentials>((e, cred) =>
            {
                _settings.Credentials = cred;
                SaveUserSettings();
            }));

            Electron.ipcMain.on(Constants.IPC.StartCapture, new Action<electron.Electron.Event>(e =>
            {
                App.ToggleStartStop(true);
            }));

            Electron.ipcMain.on(Constants.IPC.StopCapture, new Action<electron.Electron.Event>(e =>
            {
                App.ToggleStartStop(false);
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

            node.setTimeout(args =>
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

                    // Show the app icon in tray:
                    App.ShowTrayIcon();
                });

            }, 2000);
        }

        private static electron.Electron.BrowserWindow CreateSplashScreen()
        {
            var options = ObjectLiteral.Create<electron.Electron.BrowserWindowConstructorOptions>();
            options.width = 600;
            options.height = 400;
            options.icon = node.path.join(node.__dirname, "Assets/Images/app_icon_32.png");
            options.title = Constants.AppTitle;
            options.frame = false;
            options.skipTaskbar = true;
            options.show = false;

            // Create the browser window.
            var splash = new electron.Electron.BrowserWindow(options);
            App.LoadWindow(splash, "Forms/SplashScreen.html");

            return splash;
        }

        private static void CreateMainWindow()
        {
            var options = ObjectLiteral.Create<electron.Electron.BrowserWindowConstructorOptions>();
            options.width = 600;
            options.height = 800;
            options.icon = node.path.join(node.__dirname, "Assets/Images/app_icon_32.png");
            options.title = Constants.AppTitle;
            options.show = false;

            // Create the browser window.
            Win = new electron.Electron.BrowserWindow(options);
            SetContextMenu(Win);

            App.SetMainMenu();

            App.LoadWindow(Win, "Forms/MainForm.html");

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
            });
        }

        private static electron.Electron.BrowserWindow CreateOptionsWindow()
        {
            var options = ObjectLiteral.Create<electron.Electron.BrowserWindowConstructorOptions>();
            options.width = 440;
            options.height = 565;
            options.title = "Options";
            options.icon = node.path.join(node.__dirname, "Assets/Images/app_icon_32.png");
            options.skipTaskbar = true;
            options.parent = Win;
            options.modal = true;
            options.show = false;
            options.maximizable = false;
            options.minimizable = false;
            options.resizable = false;

            // Create the browser window.
            var optionsWin = new electron.Electron.BrowserWindow(options);
            SetMainMenuForOptions(optionsWin);
            SetContextMenu(optionsWin);

            App.LoadWindow(optionsWin, "Forms/OptionsForm.html");

            return optionsWin;
        }

        private static void LoadWindow(electron.Electron.BrowserWindow win, string page)
        {
            var windowUrl = ObjectLiteral.Create<node.url.Url>();
            windowUrl.pathname = node.path.join(node.__dirname, page);
            windowUrl.protocol = "file:";
            windowUrl.slashes = true;

            var formattedUrl = node.url.format(windowUrl);

            // and load the index.html of the app.
            win.loadURL(formattedUrl);
        }

        private static void ShowTrayIcon()
        {
            var icon16Path = node.path.join(node.__dirname, "Assets/Images/app_icon_16.png");
            var icon32Path = node.path.join(node.__dirname, "Assets/Images/app_icon_32.png");

            Action showFn = () =>
            {
                if (!Win.isVisible())
                {
                    Win.show();
                }

                Win.focus();
                Win.setSkipTaskbar(false);
            };

            var openMenuItem = new electron.Electron.MenuItemConstructorOptions
            {
                label = "Open",
                click = delegate { showFn(); }
            };

            var captureMenuItem = new electron.Electron.MenuItemConstructorOptions
            {
                label = "Capture",
                submenu = new[]
                {
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Start",
                        click = delegate
                        {
                            Win.webContents.send(Constants.IPC.StartCapture);
                            App.ToggleStartStopMenuItems();
                        }
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Stop",
                        enabled = false,
                        click = delegate
                        {
                            Win.webContents.send(Constants.IPC.StopCapture);
                            App.ToggleStartStopMenuItems();
                        }
                    }
                }
            };

            var visitMenuItem = new electron.Electron.MenuItemConstructorOptions
            {
                label = "Visit Bridge.NET",
                click = delegate { Electron.shell.openExternal("http://bridge.net/"); }
            };

            var exitMenuItem = new electron.Electron.MenuItemConstructorOptions
            {
                label = "Exit",
                role = "quit"
            };

            ContextMenu = electron.Electron.Menu.buildFromTemplate(new[] { openMenuItem, captureMenuItem, visitMenuItem, exitMenuItem });

            var iconPath = node.process.platform == Platform.darwin ? icon16Path : icon32Path;

            AppIcon = new electron.Electron.Tray(iconPath);
            AppIcon.setToolTip(Constants.AppTitle);
            AppIcon.setContextMenu(ContextMenu);
            AppIcon.on("click", () =>
            {
                showFn();
            });
        }

        private static void SetMainMenu()
        {
            var fileMenu = new electron.Electron.MenuItemConstructorOptions
            {
                label = "File",
                submenu = new []
                {
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Options",
                        accelerator = CreateMenuAccelerator("F2"),
                        click = (i, w, e) =>
                        {
                            var optionsWin = App.CreateOptionsWindow();
                            optionsWin.once(lit.ready_to_show, () =>
                            {
                                // to prevent showing not rendered window:
                                optionsWin.show();
                            });
                        }
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        type = lit.separator
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Exit",
                        role = "quit"
                    }
                }
            };

            var editMenu = new electron.Electron.MenuItemConstructorOptions
            {
                label = "Edit",
                submenu = new[]
                {
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        role = "undo",
                        accelerator = CreateMenuAccelerator("Ctrl+Z")
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        role = "redo",
                        accelerator = CreateMenuAccelerator("Ctrl+Y")
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        type = lit.separator
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        role = "cut",
                        accelerator = CreateMenuAccelerator("Ctrl+X")
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        role = "copy",
                        accelerator = CreateMenuAccelerator("Ctrl+C")
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        role = "paste",
                        accelerator = CreateMenuAccelerator("Ctrl+V")
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        type = lit.separator
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        role = "selectall",
                        accelerator = CreateMenuAccelerator("Ctrl+A")
                    }
                }
            };

            var viewMenu = new electron.Electron.MenuItemConstructorOptions
            {
                label = "View",
                submenu = new []
                {
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Reload",
                        accelerator = CreateMenuAccelerator("Ctrl+R"),
                        click = (i, w, e) =>
                        {
                            w?.webContents.reload();
                        }
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Toggle DevTools",
                        accelerator = CreateMenuAccelerator("F12"),
                        click = (i, w, e) =>
                        {
                            w?.webContents.toggleDevTools();
                        }
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        type = lit.separator
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Theme",
                        submenu = new []
                        {
                            new electron.Electron.MenuItemConstructorOptions
                            {
                                type = lit.radio,
                                label = "Light",
                                @checked = true,
                                click = (i, w, e) =>
                                {
                                    Win.webContents.send(Constants.IPC.ToggleTheme);
                                }

                            },
                            new electron.Electron.MenuItemConstructorOptions
                            {
                                type = lit.radio,
                                label = "Dark",
                                click = (i, w, e) =>
                                {
                                    Win.webContents.send(Constants.IPC.ToggleTheme);
                                }
                            }
                        }
                    }
                }
            };

            var captureMenu = new electron.Electron.MenuItemConstructorOptions
            {
                label = "Capture",
                submenu = new[]
                {
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Start",
                        accelerator = CreateMenuAccelerator("F5"),
                        click = (i, w, e) =>
                        {
                            App.ToggleStartStop(true);
                        }
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Stop",
                        accelerator = CreateMenuAccelerator("F6"),
                        enabled = false,
                        click = (i, w, e) =>
                        {
                            App.ToggleStartStop(false);
                        }
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        type = lit.separator
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Clear captured tweets",
                        accelerator = CreateMenuAccelerator("F7"),
                        click = (i, w, e) =>
                        {
                            Win.webContents.send(Constants.IPC.ClearCapture);
                        }
                    }
                }
            };


            var helpMenu = new electron.Electron.MenuItemConstructorOptions
            {
                label = "Help",
                submenu = new[]
                {
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Visit Bridge.NET",
                        click = delegate
                        {
                            Electron.shell.openExternal("http://bridge.net/");
                        }
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Visit Retyped",
                        click = delegate { Electron.shell.openExternal("https://retyped.com/"); }
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        type = lit.separator
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Visit Electron API Demos",
                        click = delegate { Electron.shell.openExternal("https://github.com/electron/electron-api-demos"); }
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Visit Twitter API Reference",
                        click = delegate { Electron.shell.openExternal("https://dev.twitter.com/streaming/overview"); }
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        type = lit.separator
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "About",
                        click = delegate
                        {
                            var msgBoxOpts = ObjectLiteral.Create<electron.Electron.MessageBoxOptions>();
                            msgBoxOpts.type = "info";
                            msgBoxOpts.title = "About";
                            msgBoxOpts.buttons = new[] {"OK"};
                            msgBoxOpts.message = Constants.AppTitle + @".

Node: " + node.process.versions.node + @"
Chrome: " + node.process.versions["chrome"] + @"
Electron: " + node.process.versions["electron"];

                            Electron.dialog.showMessageBox(msgBoxOpts);
                        }
                    }
                }
            };

            var appMenu = electron.Electron.Menu.buildFromTemplate(new[] { fileMenu, editMenu, captureMenu, viewMenu, helpMenu });
            electron.Electron.Menu.setApplicationMenu(appMenu);
        }

        private static void SetMainMenuForOptions(electron.Electron.BrowserWindow win)
        {
            var fileMenu = new electron.Electron.MenuItemConstructorOptions
            {
                label = "File",
                submenu = new[]
                {
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Close",
                        role = "close"
                    }
                }
            };

            var editMenu = new electron.Electron.MenuItemConstructorOptions
            {
                label = "Edit",
                submenu = new[]
                {
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        role = "undo",
                        accelerator = CreateMenuAccelerator("Ctrl+Z")
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        role = "redo",
                        accelerator = CreateMenuAccelerator("Ctrl+Y")
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        type = lit.separator
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        role = "cut",
                        accelerator = CreateMenuAccelerator("Ctrl+X")
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        role = "copy",
                        accelerator = CreateMenuAccelerator("Ctrl+C")
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        role = "paste",
                        accelerator = CreateMenuAccelerator("Ctrl+V")
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        type = lit.separator
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        role = "selectall",
                        accelerator = CreateMenuAccelerator("Ctrl+A")
                    }
                }
            };

            var viewMenu = new electron.Electron.MenuItemConstructorOptions
            {
                label = "View",
                submenu = new[]
                {
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Reload",
                        accelerator = CreateMenuAccelerator("Ctrl+R"),
                        click = (i, w, e) =>
                        {
                            w?.webContents.reload();
                        }
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Toggle DevTools",
                        accelerator = CreateMenuAccelerator("F12"),
                        click = (i, w, e) =>
                        {
                            w?.webContents.toggleDevTools();
                        }
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        type = lit.separator
                    },
                    new electron.Electron.MenuItemConstructorOptions
                    {
                        label = "Theme",
                        submenu = new []
                        {
                            new electron.Electron.MenuItemConstructorOptions
                            {
                                type = lit.radio,
                                label = "Light",
                                @checked = true,
                                click = (i, w, e) =>
                                {
                                    Win.webContents.send(Constants.IPC.ToggleTheme);
                                }

                            },
                            new electron.Electron.MenuItemConstructorOptions
                            {
                                type = lit.radio,
                                label = "Dark",
                                click = (i, w, e) =>
                                {
                                    Win.webContents.send(Constants.IPC.ToggleTheme);
                                }
                            }
                        }
                    }
                }
            };

            var optionsMenu = electron.Electron.Menu.buildFromTemplate(new[] { fileMenu, editMenu, viewMenu });
            win.setMenu(optionsMenu);
        }

        private static void SetContextMenu(electron.Electron.BrowserWindow win)
        {
            var selectionMenuTemplate = new[]
            {
                new electron.Electron.MenuItemConstructorOptions
                {
                    role = "copy",
                    accelerator = CreateMenuAccelerator("Ctrl+C")
                },
                new electron.Electron.MenuItemConstructorOptions
                {
                    type = lit.separator
                },
                new electron.Electron.MenuItemConstructorOptions
                {
                    role = "selectall",
                    accelerator = CreateMenuAccelerator("Ctrl+A")
                }
            };

            var inputMenuTemplate = new[]
            {
                new electron.Electron.MenuItemConstructorOptions
                {
                    role = "undo",
                    accelerator = CreateMenuAccelerator("Ctrl+Z")
                },
                new electron.Electron.MenuItemConstructorOptions
                {
                    role = "redo",
                    accelerator = CreateMenuAccelerator("Ctrl+Y")
                },
                new electron.Electron.MenuItemConstructorOptions
                {
                    type = lit.separator
                },
                new electron.Electron.MenuItemConstructorOptions
                {
                    role = "cut",
                    accelerator = CreateMenuAccelerator("Ctrl+X")
                },
                new electron.Electron.MenuItemConstructorOptions
                {
                    role = "copy",
                    accelerator = CreateMenuAccelerator("Ctrl+C")
                },
                new electron.Electron.MenuItemConstructorOptions
                {
                    role = "paste",
                    accelerator = CreateMenuAccelerator("Ctrl+V")
                },
                new electron.Electron.MenuItemConstructorOptions
                {
                    type = lit.separator
                },
                new electron.Electron.MenuItemConstructorOptions
                {
                    role = "selectall",
                    accelerator = CreateMenuAccelerator("Ctrl+A")
                }
            };

            var selectionMenu = electron.Electron.Menu.buildFromTemplate(selectionMenuTemplate);
            var inputMenu = electron.Electron.Menu.buildFromTemplate(inputMenuTemplate);

            win.webContents.on(lit.context_menu, (e, props) =>
            {
                if (props.isEditable)
                {
                    inputMenu.popup(win);
                }
                else if(props.selectionText != null && !string.IsNullOrEmpty(props.selectionText.Trim()))
                {
                    selectionMenu.popup(win);
                }
            });
        }

        private static electron.Electron.Accelerator CreateMenuAccelerator(string value)
        {
            if (node.process.platform == Platform.darwin)
            {
                value = value.Replace("Ctrl", "Command");
            }

            return value.As<electron.Electron.Accelerator>();
        }

        private static void ToggleStartStop(bool isStart)
        {
            Win.webContents.send(isStart ? Constants.IPC.StartCapture : Constants.IPC.StopCapture);
            App.ToggleStartStopMenuItems();
        }

        private static void ToggleStartStopMenuItems()
        {
            var appMenu = electron.Electron.Menu.getApplicationMenu();
            var captureMenu = appMenu.items
                .First(x => x.label == "Capture")
                ["submenu"]
                .As<electron.Electron.Menu>();

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
                    .As<electron.Electron.Menu>();

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
            var settingsPath = node.path.join(userDataPath, Constants.UserSettingsFileName);

            if (node.fs.existsSync(settingsPath))
            {
                var fileData = node.fs.readFileSync(settingsPath, "utf8");
                _settings = UserSettings.Deserialize(fileData);
            }
            else
            {
                _settings = new UserSettings
                {
                    Credentials = new TwitterCredentials()
                };
            }
        }

        private static void SaveUserSettings()
        {
            var userDataPath = Electron.app.getPath("userData");
            var settingsPath = node.path.join(userDataPath, Constants.UserSettingsFileName);
            var data = _settings.Serialize();

            node.fs.writeFileSync(settingsPath, data);
        }
    }
}