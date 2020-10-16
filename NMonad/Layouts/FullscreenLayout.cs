using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NMonad.Win32Interop;

namespace NMonad.Layouts
{
    public class FullscreenLayout : Layout
    {

        public override void ReflowScreen(Screen screen, List<Window> windows)
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