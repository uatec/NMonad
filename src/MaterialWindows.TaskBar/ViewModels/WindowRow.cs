using System.Collections.Generic;

namespace MaterialWindows.TaskBar.ViewModels
{
    public class WindowRow
    {
        public string Name { get; set; }
        public int ActiveWindowIndex { get; set; }
        public Window ActiveWindow => Windows[ActiveWindowIndex];
        public List<Window> Windows { get; set; } = new List<Window>();
    }
}
