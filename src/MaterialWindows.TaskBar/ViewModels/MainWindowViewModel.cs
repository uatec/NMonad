using System.Collections.Generic;
using MaterialWindows.TaskBar.Reflow.Layouts;

namespace MaterialWindows.TaskBar.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Window FocusedWindow { get; set; }
        
        public List<Layout> ActiveLayouts { get; set; }

        public int CurrentLayoutIndex = 0;

        public WindowList Windows = new WindowList();

        public Layout CurrentLayout
        {
            get { return ActiveLayouts[CurrentLayoutIndex]; }
        }
        public int ActiveRowIndex { get; set; }
        public WindowRow ActiveRow => WindowRows[ActiveRowIndex];
        public List<WindowRow> WindowRows { get; set; } = new List<WindowRow>
        {
            new WindowRow
            {
                Name = "Web",
                Windows = new List<Window> 
                {
                    new Window
                    {
                        Name = "GMail"
                    },
                    new Window
                    {
                        Name = "Twitter"
                    },
                    new Window
                    {
                        Name = "Reuters"
                    }
                }
            },
            new WindowRow
            {
                Name = "Messaging",
                Windows = new List<Window> 
                {
                    new Window
                    {
                        Name = "Slack"
                    },
                    new Window
                    {
                        Name = "Discord"
                    },
                    new Window
                    {
                        Name = "XChat"
                    }
                }
            }
        };
    }
}
