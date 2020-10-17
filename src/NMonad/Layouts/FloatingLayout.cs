using System.Collections.Generic;
using System.Windows.Forms;
using NMonad.Win32Interop;

namespace NMonad.Layouts
{
    public class FloatingLayout : Layout
    {

        public override void ReflowScreen(Screen screen, List<Window> windows, Window activeWindow)
        {
            // noop
        }
    }
}