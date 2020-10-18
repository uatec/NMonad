using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using MaterialWindows.TaskBar.Win32Interop;

namespace MaterialWindows.TaskBar.Reflow.Layouts
{
    public abstract class Layout
    {
        public abstract void ReflowScreen(Screen screen, List<Window> windows, Window activeWindow);
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
            Rectangle rect = new Rectangle();

            // Then we call the GetWindowRect function, passing in a reference to the rect object.
            Win32.GetWindowRect(window.Handle, ref rect);

            // If the window is already in the right position, let's not fuck with it
            if (windowPosition.X == rect.X &&
                windowPosition.Y == rect.Y &&
                windowPosition.Width == rect.Width - rect.X && // include the hack to fix the fact that Width is the Right position, not the X
                windowPosition.Height == rect.Height - rect.Y) // and for Y
            {
                return;
            }

            if (Win32.IsZoomed(window.Handle))
            {
                Win32.ShowWindowAsync(window.Handle, ShowWindowCommands.Normal);
            }

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

                // log.DebugFormat("Error - {0}", error);
                switch (error)
                {
                    case 1400: // can't find handle
                        break;
                    default:
                        Debugger.Break();
                        break;
                }
            }


            // update our record of the window to remember the position we have just put it in
            window.X = windowPosition.X;
            window.Y = windowPosition.Y;
            window.Width = windowPosition.Width;
            window.Height = windowPosition.Height;

            // log.Info(new {
            //     Message = "Window Moved",
            //     Name = window.Name,
            //     From = JsonConvert.SerializeObject(new Rectangle(rect.X, rect.Y, rect.Width - rect.X, rect.Height - rect.Y)),
            //     To = JsonConvert.SerializeObject(windowPosition)
            // });
        }
        
    }
}