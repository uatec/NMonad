using System.Collections.Generic;
using System.Windows.Forms;
using MaterialWindows.TaskBar.Win32Interop;
using Window = MaterialWindows.TaskBar.ViewModels.Window;

namespace MaterialWindows.TaskBar.Reflow.Layouts
{
    public class FloatingLayout : Layout
    {

        public override void ReflowScreen(Screen screen, List<Window> windows, Window activeWindow)
        {
            // noop
        }
    }
}