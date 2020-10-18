using System.Collections.Generic;
using System.Linq;
using MaterialWindows.TaskBar.Reflow.Layouts;

namespace MaterialWindows.TaskBar.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // Layouts
        public List<Layout> ActiveLayouts { get; set; }

        public int CurrentLayoutIndex = 0;

        public Layout CurrentLayout
        {
            get { return ActiveLayouts[CurrentLayoutIndex]; }
        }
        public int ActiveRowIndex { get; set; }
        public WindowRow ActiveRow => WindowRows[ActiveRowIndex];
        public List<WindowRow> WindowRows { get; set; } = new List<WindowRow>
        {
            new WindowRow { Name = "Default" }
        };
    }
}
