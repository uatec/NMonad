using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace NMonad
{
    public abstract class Layout
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public abstract void ReflowScreen(Screen screen, List<Window> windows);
        /// <summary>
        /// The number of Panes which will be considered primary and be reserved for the top N windows.
        /// </summary>
        public int MainPaneCount { get; set; }

        /// <summary>
        /// The proportion of the screen, between 0 and 1, that a main pane will take up
        /// </summary>
        public float MainPaneSize { get; set; }

        protected void SetWindowPosition(Window window, Rectangle windowPosition)
        {
            UFlags flags = 0;
            var success = Win32.SetWindowPos(window.Handle,
                0,
                windowPosition.X, 
                windowPosition.Y, 
                windowPosition.Width, 
                windowPosition.Height,
                flags
                );

            if (!success)
            {
                uint error = Win32.GetLastError();

                log.DebugFormat("Error - {0}", error);
                switch (error)
                {
                    case 1400: // can't find handle
                        break;
                    default:
                        Debugger.Break();
                        break;
                }
            }
        }
        
    }
}