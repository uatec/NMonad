using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace MaterialWindows.TaskBar.ViewModels
{
    public class WindowRow : ReactiveObject
    {
        public string Name { get; set; }
        public int ActiveWindowIndex { get; set; }
        public Window ActiveWindow => Windows[ActiveWindowIndex];
        public ObservableCollection<Window> Windows { get; set; } = new ObservableCollection<Window>();
    }
}
