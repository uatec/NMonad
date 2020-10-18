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
                // TODO: Inject the working area when we reflow, instead of padding ourselves
                Rectangle frame = new Rectangle
                {
                    X = (int) (screen.WorkingArea.X + windowIndex * windowWidth) + 100,
                    Y = screen.WorkingArea.Y + 100,
                    Width = (int) windowWidth - 100,
                    Height = screen.WorkingArea.Height - 100
                };

                base.SetWindowPosition(window, frame);
            }
        }
    }
}