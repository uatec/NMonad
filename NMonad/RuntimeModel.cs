using System.Collections.Generic;
using NMonad.Layouts;
using NMonad.Win32Interop;

namespace NMonad
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
