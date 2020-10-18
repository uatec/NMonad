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
                X = screen.WorkingArea.X,
                Y = screen.WorkingArea.Y,
                Width = screen.WorkingArea.Width,
                Height = screen.WorkingArea.Height
            };

            foreach (Window window in windows)
            {
                base.SetWindowPosition(window, frame);
            }
        }
    }
}