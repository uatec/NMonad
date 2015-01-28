using System;
using System.Collections.Generic;
using System.Diagnostics;
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

                List<IntPtr> windowHandles = new List<IntPtr>();
                foreach ( IntPtr ptr in Win32.FindWindowsWithText(""))
                {
                    string windowName = Win32.GetWindowText(ptr);
                    if (string.IsNullOrEmpty(windowName)) continue;
                    if (ignoredWindows.Any(w => -1 < windowName.IndexOf(w, StringComparison.CurrentCultureIgnoreCase))) continue;
                    if (!Win32.IsWindowVisible(ptr)) continue;
                    if (Win32.IsIconic(ptr)) continue;

                    windowHandles.Add(ptr);
                }
                int screenCount = Screen.AllScreens.Count();

                var windowGroups = windowHandles.Select((x, i) => new {Index = i, Value = x})
                    .GroupBy(x => x.Index%screenCount)
                    .Select(x => x.Select(v =>
                        new Window
                        {
                            Handle = v.Value,
                            Name = Win32.GetWindowText(v.Value)
                        }).ToList());

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
    }
}
