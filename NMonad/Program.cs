using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NHotkey.WindowsForms;
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
            
            HotkeyManager.Current.AddOrReplace("Run", Keys.Control | Keys.Alt | Keys.R, (s, a) => Run());
            
            HotkeyManager.Current.AddOrReplace("CycleLayouts", Keys.Control | Keys.Alt | Keys.Space, cycleLayouts);
            HotkeyManager.Current.AddOrReplace("ReverseCycleLayouts", Keys.Control | Keys.Alt | Keys.Shift | Keys.Space, reverseCycleLayouts);

            HotkeyManager.Current.AddOrReplace("IncreaseMainPane", Keys.Control | Keys.Alt | Keys.H, increaseMainPane);
            HotkeyManager.Current.AddOrReplace("DecreaseMainPane", Keys.Control | Keys.Alt | Keys.Shift | Keys.H, decreaseMainPane);

            
            Timer t = new Timer(state => Run(), null, 0, 100);
            Application.Run(applicationContext);
            t.Dispose();
        }

        private static void reverseCycleLayouts(object sender, EventArgs eventArgs)
        {
            selectedLayout += 1;
            if (selectedLayout >= _layouts.Count) selectedLayout = 0;
            log.InfoFormat("Selected: {0}", layout.GetType().Name);
        }

        private static void cycleLayouts(object sender, EventArgs eventArgs)
        {
            selectedLayout -= 1;
            if (selectedLayout == -1) selectedLayout = _layouts.Count - 1;
            log.InfoFormat("Selected: {0}", layout.GetType().Name);
        }

        private static void increaseMainPane(object sender, EventArgs eventArgs)
        {
            layout.MainPaneSize *= 1.1f;
        }
        private static void decreaseMainPane(object sender, EventArgs eventArgs)
        {
            layout.MainPaneSize /= 1.1f;
        }

        private static void Run()
        {
            try
            {
                log.InfoFormat("Using layout {0}", layout.GetType().Name);

                string[] ignoredWindows = new[]
                {
                    "Program Manager",
                    "Start", 
                    "Start menu",
                    "Task Switching",
                    "LockHunter"
                };

                List<IntPtr> existingWindowHandles = new List<IntPtr>();
                foreach (IntPtr ptr in Win32.FindWindowsWithText(""))
                {
                    string windowName = Win32.GetWindowText(ptr);
                    if (string.IsNullOrEmpty(windowName)) continue;
                    if (ignoredWindows.Any(w => -1 < windowName.IndexOf(w, StringComparison.CurrentCultureIgnoreCase))) continue;
                    if (!Win32.IsWindowVisible(ptr)) continue;
                    if (Win32.IsIconic(ptr)) continue;
                    if (WindowsIsSmall(ptr)) continue;

                    existingWindowHandles.Add(ptr);
                }

                var oldWindows = _windows.Select(w => w.Handle).Except(existingWindowHandles).ToList();
                var newWindows = existingWindowHandles.Except(_windows.Select(w => w.Handle)).ToList();

                foreach (var w in newWindows)
                {
                    _windows.Add(new Window
                    {
                        Handle = w,
                        Name = Win32.GetWindowText(w)
                    });
                }

                foreach (var w in oldWindows)
                {
                    var removedWindow = _windows.Single(w1 => w1.Handle == w);
                    _windows.Remove(removedWindow);
                }  

                int screenCount = Screen.AllScreens.Count();

                IEnumerable<List<Window>> windowGroups = _windows.Select((x, i) => Tuple.Create(i, x))
                                  .GroupBy(x => x.Item1 % screenCount)
                                  .Select(x => x.Select(y => y.Item2).ToList());

                int screenIndex = 0;
                foreach (var windowGroup in windowGroups)
                {
                    log.DebugFormat("Screen {0}", screenIndex);
                    layout.ReflowScreen(Screen.AllScreens[screenIndex++], windowGroup);
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
            Size threshold = new Size(700, 450);
            // the rectangle we just got back returns the Right position in the Width field, subtract X to get the window width.
            // same for Y axis
            return rect.Width - rect.X < threshold.Width && rect.Height - rect.Y < threshold.Height;
        }
    }
}
