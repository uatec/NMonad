using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MaterialWindows.TaskBar.Win32Interop;

namespace MaterialWindows.TaskBar.Reflow.Layouts
{
    public class ColumnLayout : Layout
    {
        public override void ReflowScreen(Screen screen, List<Window> windows, Window activeWindow)
        {
            if (windows.Count == 0) return;
            
            float windowWidth = screen.WorkingArea.Width/windows.Count;

            Window focusedWindow = windows.First();

            for (int windowIndex = 0; windowIndex < windows.Count; ++windowIndex)
            {
                Window window = windows[windowIndex];
                Rectangle frame = new Rectangle
                {
                    X = (int) (screen.WorkingArea.X + windowIndex * windowWidth),
                    Y = screen.WorkingArea.Y,
                    Width = (int) windowWidth,
                    Height = screen.WorkingArea.Height
                };

                base.SetWindowPosition(window, frame);
            }
        }
    }
}