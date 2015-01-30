using System;

namespace NMonad
{
    public class Window
    {
        public IntPtr Handle { get; set; }
        public string Name { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int ScreenId { get; set; }
    }
}