using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NHotkey.WindowsForms;
using NMonad.Layouts;
using Timer = System.Threading.Timer;

namespace NMonad
{
    class Program
    {
        // private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType).;

        private static Dictionary<string, Layout> registeredLayout = new Dictionary<string, Layout> {
            { "basic", new BasicLayout() },
            { "column", new ColumnLayout() },
            { "tall", new TallLayout() },
            { "fullscreen", new FullscreenLayout() },
            { "floating", new FloatingLayout() },
            { "wide", new WideLayout() }
        };
        
        private static RuntimeModel Model = new RuntimeModel();

        static Log log = new Log();
        
        private static void Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            log.Info(new
            {
                Message = "NMonad Started"
            });
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var applicationContext = new NMonadApplicationContext();

            var superkey = Keys.Control | Keys.Alt;

            HotkeyManager.Current.AddOrReplace("CycleLayouts", superkey | Keys.Space, cycleLayouts);
            HotkeyManager.Current.AddOrReplace("ReverseCycleLayouts", superkey | Keys.Shift | Keys.Space, reverseCycleLayouts);
            
            HotkeyManager.Current.AddOrReplace("CycleMainWindow", superkey | Keys.Up, cycleMainPane);
            HotkeyManager.Current.AddOrReplace("ReverseCycleMainWindow", superkey | Keys.Down, reverseCycleMainPane);

            HotkeyManager.Current.AddOrReplace("IncreaseMainPane", superkey | Keys.H, increaseMainPane);
            HotkeyManager.Current.AddOrReplace("DecreaseMainPane", superkey | Keys.Shift | Keys.H, decreaseMainPane);
            
            HotkeyManager.Current.AddOrReplace("CycleAssignedScreen", superkey | Keys.Right, cycleAssignedScreen);
            HotkeyManager.Current.AddOrReplace("ReverseCycleAssignedScreen", superkey | Keys.Left, reverseCycleAssignedScreen);

            HotkeyManager.Current.AddOrReplace("DumpWindowList", superkey | Keys.K, dumpWindowList);
            
            HotkeyManager.Current.AddOrReplace("Exit", superkey | Keys.Q, (s, e) => applicationContext.ExitThread());

            object syncRoot = new object();

            var config = Configuration.Get<RootConfig>();

            Model.ActiveLayouts = registeredLayout.Where(kvp => config.Layouts.Contains(kvp.Key)).Select(kvp => kvp.Value).ToList();


            using (new Timer(state =>
                {
                    if (Monitor.TryEnter(syncRoot, 100))
                    {
                        Run(config);
                        Monitor.Exit(syncRoot);
                    }
                }, null, 0, 100))
            {
                Application.Run(applicationContext);
            }
        }


        private static void cycleAssignedScreen(object sender, EventArgs eventArgs)
        {
            int screenCount = Screen.AllScreens.Count();
            IntPtr activeWindowHandle = Win32.GetForegroundWindow();

            Window activeWindow = Model.Windows.SingleOrDefault(w => w.Handle == activeWindowHandle);

            if (activeWindow == null)
            {
                log.Error(new
                {
                    Message = "Tried to cycle window between screens but could not identify the current window.",
                    WindowName = Win32.GetWindowText(activeWindowHandle),
                    WindowHandle = activeWindowHandle
                });
                return;
            }

            int oldScreenId = activeWindow.ScreenId;
            activeWindow.ScreenId++;
            if (activeWindow.ScreenId >= screenCount)
            {
                activeWindow.ScreenId = 0;
            }
            
            log.Info(new
            {
                Message = "Window Moved Screen",
                WindowName = activeWindow.Name,
                NewScreenId = activeWindow.ScreenId,
                OldScreenId = oldScreenId 
            });
        }

        private static void reverseCycleAssignedScreen(object sender, EventArgs eventArgs)
        {
            int screenCount = Screen.AllScreens.Count();
            IntPtr activeWindowHandle = Win32.GetForegroundWindow();

            Window activeWindow = Model.Windows.SingleOrDefault(w => w.Handle == activeWindowHandle);

            if (activeWindow == null)
            {
                log.Error(new
                {
                    Message = "Tried to cycle window between screens but could not identify the current window.",
                    WindowName = Win32.GetWindowText(activeWindowHandle),
                    WindowHandle = activeWindowHandle
                });
                return;
            }

            int oldScreenId = activeWindow.ScreenId;
            activeWindow.ScreenId--;
            if (activeWindow.ScreenId < 0)
            {
                activeWindow.ScreenId = screenCount - 1;
            }
            
            log.Info(new
            {
                Message = "Window Moved Screen",
                WindowName = activeWindow.Name,
                NewScreenId = activeWindow.ScreenId,
                OldScreenId = oldScreenId
            });
        }

        private static void dumpWindowList(object sender, EventArgs eventArgs)
        {
            log.Info(new  {
                Message = "Window List",
                List = Model.Windows
            });
        }
        
        private static void reverseCycleLayouts(object sender, EventArgs eventArgs)
        {
            Model.CurrentLayoutIndex += 1;
            if (Model.CurrentLayoutIndex >= Model.ActiveLayouts.Count) Model.CurrentLayoutIndex = 0;
            log.Info(new
            {
                Message = "Layout Changed",
                Layout = Model.CurrentLayout.GetType().Name,
                Model.CurrentLayout.MainPaneSize,
                Model.CurrentLayout.MainPaneCount
            });
            MessageForm.ShowMessage(Model.CurrentLayout.GetType().Name.Replace("Layout", " Layout"), 1000);
        }

        private static void cycleLayouts(object sender, EventArgs eventArgs)
        {
            Model.CurrentLayoutIndex -= 1;
            if (Model.CurrentLayoutIndex == -1) Model.CurrentLayoutIndex = Model.ActiveLayouts.Count - 1;
            log.Info(new
            {
                Message = "Layout Changed",
                Layout = Model.CurrentLayout.GetType().Name,
                Model.CurrentLayout.MainPaneSize,
                Model.CurrentLayout.MainPaneCount
            });
            MessageForm.ShowMessage(Model.CurrentLayout.GetType().Name.Replace("Layout", " Layout"), 1000);
        }

        private static void increaseMainPane(object sender, EventArgs eventArgs)
        {
            Model.CurrentLayout.MainPaneSize *= 1.1f;

            log.Info(new
            {
                Message = "Main Pane Size Changed",
                Model.CurrentLayout.MainPaneSize,
            });
        }

        private static void decreaseMainPane(object sender, EventArgs eventArgs)
        {
            Model.CurrentLayout.MainPaneSize /= 1.1f;

            log.Info(new
            {
                Message = "Main Pane Size Changed",
                Model.CurrentLayout.MainPaneSize,
            });
        }

        private static void cycleMainPane(object sender, EventArgs eventArgs)
        {
            var lastWindow = Model.Windows.Last();
            Model.Windows.Remove(lastWindow);
            Model.Windows.Insert(0, lastWindow);
        }

        private static void reverseCycleMainPane(object sender, EventArgs eventArgs)
        {
            var firstWindow = Model.Windows.First();
            Model.Windows.RemoveAt(0);
            Model.Windows.Add(firstWindow);
        }

        private static void Run(RootConfig config)
        {
            // try
            {
                List<IntPtr> extantWindowHandles = new List<IntPtr>();
                foreach (IntPtr ptr in Win32.GetAllWindows())
                {
                    string windowName = Win32.GetWindowText(ptr);
                    if (string.IsNullOrEmpty(windowName)) continue;
                    //if (ignoredWindows.Any(w => -1 < windowName.IndexOf(w, StringComparison.CurrentCultureIgnoreCase))) continue;
                    if (config.ExcludedWindows.Contains(windowName)) continue;
                    if (!Win32.IsWindowVisible(ptr)) continue;
                    if (Win32.IsIconic(ptr)) continue;
                    
                    extantWindowHandles.Add(ptr);
                }

                var knownHandles = Model.Windows.Select(w => w.Handle).ToList();
                var oldWindows = knownHandles.Except(extantWindowHandles).ToList();
                var newWindows = extantWindowHandles.Except(knownHandles).ToList();

                foreach (var w in newWindows)
                {
                    // Only ignore small windows if they're added. If an existing window becomes small we still want to control it... i suspect
                    if (WindowsIsSmall(w)) continue;

                    string windowName = Win32.GetWindowText(w);
                    // Assign this window to screen 0, or the next screen that doesn't have anything assigned
                    int screenId = Model.Windows.Any() ? Model.Windows.Max(x => x.ScreenId) + 1 : 0;

                    // if we have tried to assign it to a screen that doesn't exist
                    Dictionary<int, int> screenWindowCounts = new Dictionary<int, int>();
                    if (screenId >= Screen.AllScreens.Length)
                    {

                        // figure out how many windows each screen has
                        // create entries for all screens that default to zero
                        for (int i = 0; i < Screen.AllScreens.Length; i++)
                        {
                            screenWindowCounts[i] = 0;
                        }
                        // then update the ones that have windows with the real numbers
                        var windowGroups = Model.Windows.GroupBy(x => x.ScreenId);
                        foreach (var wg in windowGroups)
                        {
                            screenWindowCounts[wg.Key] = wg.Count();
                        }
                        // and assign this window to the screen with the fewest windows
                        screenId = screenWindowCounts.OrderBy(kvp => kvp.Value).First().Key;
                    }
                    log.Info(new
                    {
                        Message = "Window Added",
                        screenId,
                        windowName,
                        ScreenWindowCounts = JsonConvert.SerializeObject(screenWindowCounts)
                    });
                    
                    Model.Windows.Add(new Window
                    {
                        Handle = w,
                        Name = windowName,
                        ScreenId = screenId
                    });
                }

                foreach (var w in oldWindows)
                {
                    var removedWindow = Model.Windows.Single(w1 => w1.Handle == w);
                    Model.Windows.Remove(removedWindow);

                    Dictionary<int, int> screenWindowCounts = new Dictionary<int, int>();

                    // figure out how many windows each screen has
                    // create entries for all screens that default to zero
                    for (int i = 0; i < Screen.AllScreens.Length; i++)
                    {
                        screenWindowCounts[i] = 0;
                    }
                    // then update the ones that have windows with the real numbers
                    var windowGroups = Model.Windows.GroupBy(x => x.ScreenId);
                    foreach (var wg in windowGroups)
                    {
                        screenWindowCounts[wg.Key] = wg.Count();
                    }

                    log.Info(new
                    {
                        Message = "Window Removed",
                        removedWindow.ScreenId,
                        removedWindow.Name,
                        remainWindows = Model.Windows.Count(w2 => w2.ScreenId == removedWindow.ScreenId),
                        ScreenWindowCounts = JsonConvert.SerializeObject(screenWindowCounts)
                    });
                }

                foreach (var windowGroup in Model.Windows
                    .GroupBy(x => x.ScreenId))
                {
                    if (windowGroup.Key >= Screen.AllScreens.Count())
                    {
                        log.Warn(new
                        {
                            Message = "Invalid screen Id - Suspected screen count change, reassigning windows to screens and reflowing.",
                            AttemptedScreenId = windowGroup.Key,
                            NumberOfScreens = Screen.AllScreens.Count()
                        });
                        Model.Windows.ForEach(w => w.ScreenId = -1);
                        return;
                    }

                    Model.CurrentLayout.ReflowScreen(Screen.AllScreens[windowGroup.Key], windowGroup.ToList());
                }

            }
            // catch (Exception ex)
            // {
            //     log.Fatal("unhandled fatal exception", ex);
            //     throw;
            // }
        }


        private static bool WindowsIsSmall(IntPtr handle)
        {
            // First we intialize an empty Rectangle object.
            Rectangle rect = new Rectangle();

            // Then we call the GetWindowRect function, passing in a reference to the rect object.
            Win32.GetWindowRect(handle, ref rect);

            // Threshold is hardcoded here cos i cba to drive it from config yet
            Size threshold = new Size(650, 450);
            // the rectangle we just got back returns the Right position in the Width field, subtract X to get the window width.
            // same for Y axis
            return rect.Width - rect.X < threshold.Width && rect.Height - rect.Y < threshold.Height;
        }

    }
}
