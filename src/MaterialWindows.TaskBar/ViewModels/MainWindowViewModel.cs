using System;
using System.Collections.Generic;
using System.Text;

namespace MaterialWindows.TaskBar.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Hello World!";

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

    public class WindowRow
    {
        public string Name { get; set; }
        public int ActiveWindowIndex { get; set; }
        public Window ActiveWindow => Windows[ActiveWindowIndex];
        public List<Window> Windows { get; set; } = new List<Window>();
    }

    public class Window
    {
        public string Name { get; set; } = "! Name not set !";
    }
}
