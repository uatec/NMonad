using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace NMonad
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            try
            {

                Layout layout = new TallLayout();

                log.InfoFormat("Using layout {0}", layout.GetType().Name);

                string[] ignoredWindows = new[]
                {
                    "Program Manager",
                    "Start"
                };

                List<IntPtr> windowHandles = new List<IntPtr>();
                foreach ( IntPtr ptr in Win32.FindWindowsWithText(""))
                {
                    string windowName = Win32.GetWindowText(ptr);
                    if (string.IsNullOrEmpty(windowName)) continue;
                    if (ignoredWindows.Contains(windowName)) continue;
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
