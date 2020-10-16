using System.Collections.Generic;
using NMonad.Layouts;

namespace NMonad
{
    public class RuntimeModel
    {
        public List<Layout> ActiveLayouts { get; set; }

        public int CurrentLayoutIndex = 0;

        public WindowList Windows = new WindowList();

        public Layout CurrentLayout
        {
            get { return ActiveLayouts[CurrentLayoutIndex]; }
        }
    }
}
