using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MaterialWindows.TaskBar.Win32Interop;
using Window = MaterialWindows.TaskBar.ViewModels.Window;

namespace MaterialWindows.TaskBar.Reflow.Layouts
{
    public class FullscreenLayout : Layout
    {

        public override void ReflowScreen(Screen screen, List<Window> windows, Window activeWindow)
        {
            Rectangle frame = new Rectangle
            {
                X = screen.WorkingArea.X + 100,
                Y = screen.WorkingArea.Y + 100,
                Width = screen.WorkingArea.Width - 100,
                Height = screen.WorkingArea.Height - 100
            };
            // Console.WriteLine("active window " + activeWindow?.Name);
            foreach (Window window in windows)
            {
                if ( window != activeWindow )
                {
                    // Console.WriteLine("hiding " + window.Name);
                    Win32.ShowWindowAsync(window.Handle, ShowWindowCommands.Minimized);
                }
                else 
                {
                    // Console.WriteLine("showing " + window.Name);
                    base.SetWindowPosition(window, frame);
                }
            }
        }
    }
}