using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using NHotkey.WindowsForms;
using NMonad.Layouts;
using Timer = System.Threading.Timer;

namespace NMonad
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static List<Layout> _layouts = new List<Layout>();
        private static int selectedLayout = 0;

        private static List<Window> _windows = new List<Window>();

        private static Layout layout
        {
            get { return _layouts[selectedLayout]; }
        }

        private static void Main(string[] args)
        {

            log.Info(new
            {
                Message = "NMonad Started"
            });
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var applicationContext = new NMonadApplicationContext();

            _layouts.AddRange(new Layout[]
                {
                    //new BasicLayout(),
                    //new ColumnLayout(),
                    //new FloatingLayout(),
                    new TallLayout(),
                    new FullscreenLayout()
                });
            
            HotkeyManager.Current.AddOrReplace("CycleLayouts", Keys.Control | Keys.Alt | Keys.Space, cycleLayouts);
            HotkeyManager.Current.AddOrReplace("ReverseCycleLayouts", Keys.Control | Keys.Alt | Keys.Shift | Keys.Space, reverseCycleLayouts);
            
            HotkeyManager.Current.AddOrReplace("CycleMainWindow", Keys.Control | Keys.Alt | Keys.J, cycleMainPane);
            HotkeyManager.Current.AddOrReplace("ReverseCycleMainWindow", Keys.Control | Keys.Alt | Keys.Shift | Keys.J, reverseCycleMainPane);

            HotkeyManager.Current.AddOrReplace("IncreaseMainPane", Keys.Control | Keys.Alt | Keys.H, increaseMainPane);
            HotkeyManager.Current.AddOrReplace("DecreaseMainPane", Keys.Control | Keys.Alt | Keys.Shift | Keys.H, decreaseMainPane);

            HotkeyManager.Current.AddOrReplace("DumpWindowList", Keys.Control | Keys.Alt | Keys.K, dumpWindowList);
            
            HotkeyManager.Current.AddOrReplace("DumpWindowList", Keys.Control | Keys.Alt | Keys.Q, (s, e) => applicationContext.ExitThread());

            object o = new object();

            Timer t = new Timer(state =>
            {
                if (Monitor.TryEnter(o, 100))
                {
                    Run();
                    Monitor.Exit(o);
                }
            }, null, 0, 100);
            Application.Run(applicationContext);
            t.Dispose();
        }
        private static void dumpWindowList(object sender, EventArgs eventArgs)
        {
            log.Info(new  {
                Message = "Window List",
                List = JsonConvert.SerializeObject(_windows)
            });
        }
        
        private static void reverseCycleLayouts(object sender, EventArgs eventArgs)
        {
            selectedLayout += 1;
            if (selectedLayout >= _layouts.Count) selectedLayout = 0;
            log.Info(new
            {
                Message = "Layout Changed",
                Layout = layout.GetType().Name
            });
        }

        private static void cycleLayouts(object sender, EventArgs eventArgs)
        {
            selectedLayout -= 1;
            if (selectedLayout == -1) selectedLayout = _layouts.Count - 1;
            log.Info(new
            {
                Message = "Layout Changed",
                Layout = layout.GetType().Name,
                layout.MainPaneSize,
                layout.MainPaneCount
            });
        }

        private static void increaseMainPane(object sender, EventArgs eventArgs)
        {
            layout.MainPaneSize *= 1.1f;

            log.Info(new
            {
                Message = "Main Pane Size Changed",
                layout.MainPaneSize,
            });
        }

        private static void decreaseMainPane(object sender, EventArgs eventArgs)
        {
            layout.MainPaneSize /= 1.1f;

            log.Info(new
            {
                Message = "Main Pane Size Changed",
                layout.MainPaneSize,
            });
        }

        private static void cycleMainPane(object sender, EventArgs eventArgs)
        {
            var lastWindow = _windows.Last();
            _windows.Remove(lastWindow);
            _windows.Insert(0, lastWindow);
        }

        private static void reverseCycleMainPane(object sender, EventArgs eventArgs)
        {
            var firstWindow = _windows.First();
            _windows.RemoveAt(0);
            _windows.Add(firstWindow);
        }

        private static void Run()
        {
            try
            {
                string[] ignoredWindows = new[]
                {
                    "Program Manager",
                    "Start", 
                    "Start menu",
                    "Task Switching",
                    "LockHunter",
                    "Razer Configurator",
                    "Microsoft OneNote 2013 - Windows taskbar",
                    "Open"
                };

                List<IntPtr> extantWindowHandles = new List<IntPtr>();
                foreach (IntPtr ptr in Win32.FindWindowsWithText(""))
                {
                    string windowName = Win32.GetWindowText(ptr);
                    if (string.IsNullOrEmpty(windowName)) continue;
                    //if (ignoredWindows.Any(w => -1 < windowName.IndexOf(w, StringComparison.CurrentCultureIgnoreCase))) continue;
                    if (ignoredWindows.Contains(windowName)) continue;
                    if (!Win32.IsWindowVisible(ptr)) continue;
                    if (Win32.IsIconic(ptr)) continue;
                    if (WindowsIsSmall(ptr)) continue;

                    extantWindowHandles.Add(ptr);
                }

                var knownHandles = _windows.Select(w => w.Handle).ToList();
                var oldWindows = knownHandles.Except(extantWindowHandles).ToList();
                var newWindows = extantWindowHandles.Except(knownHandles).ToList();

                foreach (var w in newWindows)
                {
                    string windowName = Win32.GetWindowText(w);
                    // Assign this window to screen 0, or the next screen that doesn't have anything assigned
                    int screenId = _windows.Any() ? _windows.Max(x => x.ScreenId) + 1 : 0;

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
                        var windowGroups = _windows.GroupBy(x => x.ScreenId);
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
                    
                    _windows.Add(new Window
                    {
                        Handle = w,
                        Name = windowName,
                        ScreenId = screenId
                    });
                }

                foreach (var w in oldWindows)
                {
                    var removedWindow = _windows.Single(w1 => w1.Handle == w);
                    _windows.Remove(removedWindow);

                    Dictionary<int, int> screenWindowCounts = new Dictionary<int, int>();

                    // figure out how many windows each screen has
                    // create entries for all screens that default to zero
                    for (int i = 0; i < Screen.AllScreens.Length; i++)
                    {
                        screenWindowCounts[i] = 0;
                    }
                    // then update the ones that have windows with the real numbers
                    var windowGroups = _windows.GroupBy(x => x.ScreenId);
                    foreach (var wg in windowGroups)
                    {
                        screenWindowCounts[wg.Key] = wg.Count();
                    }

                    log.Info(new
                    {
                        Message = "Window Removed",
                        removedWindow.ScreenId,
                        removedWindow.Name,
                        remainWindows = _windows.Count(w2 => w2.ScreenId == removedWindow.ScreenId),
                        ScreenWindowCounts = JsonConvert.SerializeObject(screenWindowCounts)
                    });
                }  

                foreach (var windowGroup in _windows
                    .GroupBy(x => x.ScreenId))
                {
                    layout.ReflowScreen(Screen.AllScreens[windowGroup.Key], windowGroup.ToList());
                }

            }
            catch (Exception ex)
            {
                log.Fatal("unhandled fatal exception", ex);
            }
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
