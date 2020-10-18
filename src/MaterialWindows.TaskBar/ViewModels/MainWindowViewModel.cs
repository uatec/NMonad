using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MaterialWindows.TaskBar.Reflow.Layouts;
using ReactiveUI;

namespace MaterialWindows.TaskBar.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // Layouts
        public ObservableCollection<Layout> ActiveLayouts { get; set; } = new ObservableCollection<Layout>();


        // this.RaiseAndSetIfChanged(ref caption, value);

        private int currentLayoutIndex = 0;
        public Layout CurrentLayout => ActiveLayouts[CurrentLayoutIndex];
        public int CurrentLayoutIndex
        {
            get => currentLayoutIndex; set
            {
                this.RaiseAndSetIfChanged(ref currentLayoutIndex, value);
                this.RaisePropertyChanged(nameof(CurrentLayout));
            }
        }

        private int activeRowIndex;
        public WindowRow ActiveRow => WindowRows[ActiveRowIndex];

        public int ActiveRowIndex
        {
            get => activeRowIndex; set
            {
                this.RaiseAndSetIfChanged(ref activeRowIndex, value);
                this.RaisePropertyChanged(nameof(activeRowIndex));
            }
        }
        
        public ObservableCollection<WindowRow> WindowRows { get; set; } = new ObservableCollection<WindowRow>
        {
            new WindowRow { Name = "Default" }
        };
    }
}
