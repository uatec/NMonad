using System.Collections.Generic;
using MaterialWindows.TaskBar.Reflow.Layouts;
using MaterialWindows.TaskBar.Win32Interop;

namespace MaterialWindows.TaskBar
{
    public class RuntimeModel
    {
        public Window FocusedWindow { get; set; }
        
        public List<Layout> ActiveLayouts { get; set; }

        public int CurrentLayoutIndex = 0;

        public WindowList Windows = new WindowList();

        public Layout CurrentLayout
        {
            get { return ActiveLayouts[CurrentLayoutIndex]; }
        }
    }
}
