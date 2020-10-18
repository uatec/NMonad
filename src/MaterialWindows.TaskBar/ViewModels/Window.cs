using System;
using ReactiveUI;

namespace MaterialWindows.TaskBar.ViewModels
{
    public class Window : ReactiveObject
    {
        public string Name { get; set; }
        
        public IntPtr Handle { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int ScreenId { get; set; }
    }
}
