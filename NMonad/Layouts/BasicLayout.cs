using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NMonad.Win32Interop;

namespace NMonad.Layouts
{
    public class BasicLayout : Layout
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void ReflowScreen(Screen screen, List<Window> windows, Window activeWindow)
        {
            int width = screen.WorkingArea.Width;
            int height = screen.WorkingArea.Height;
            int cols = 1;
            if (windows.Count <= 3)
            {
                cols = windows.Count;
            }
            else
            {
                cols = (int)Math.Sqrt(windows.Count);
            }

            int colWidth = width / cols;
            int windowsPerCol = windows.Count / cols;
            int cellHeight = height / windowsPerCol;
            int windowId = 0;
            for (int x = 0; x < cols && windowId <= windows.Count; x++)
            {
                for (int y = 0; y + 100 < screen.WorkingArea.Height && windowId <= windows.Count; y += cellHeight)
                {
                    var window = windows[windowId];

                    int xpos = screen.WorkingArea.Left + x * colWidth;
                    int ypos = screen.WorkingArea.Top + y;
                    int windowWidth = colWidth;
                    int windowHeight = cellHeight;
                    
                    log.DebugFormat("x: {0}, y: {1}, w: {2}, h: {3}", xpos, ypos, windowWidth, windowHeight);

                    base.SetWindowPosition(window, new Rectangle(xpos, ypos, windowWidth, windowHeight));

                    windowId++;
                }
            }
        }
    }
}