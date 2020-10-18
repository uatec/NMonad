using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace MaterialWindows.TaskBar.Win32Interop
{
    public static class Win32
    {
        public enum LWA
        {
            ColorKey = 0x1,
            Alpha = 0x2
        }

        public enum GWL
        {
            ExStyle = -20
        }

        public enum WS_EX
        {
            Transparent = 0x20,
            Layered = 0x80000
        }

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, WS_EX dsNewLong);

        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte alpha, LWA dwFlags);
    
        // The GetWindowRect function takes a handle to the window as the first parameter. The second parameter
        // must include a reference to a Rectangle object. This Rectangle object will then have it's values set
        // to the window rectangle properties.
        [DllImport("user32.dll")]
        public static extern long GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);



        [DllImport("Kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("User32.dll")]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            WindowPosition hWndInsertAfter,
            int x,
            int y,
            int cz,
            int cy,
            UFlags uFlags);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        public static string GetWindowText(IntPtr hWnd)
        {
            int size = GetWindowTextLength(hWnd);
            if (size++ > 0)
            {
                var builder = new StringBuilder(size);
                GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return String.Empty;
        }

        public static IEnumerable<IntPtr> GetAllWindows()
        {
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows((wnd, param) =>
            {
                windows.Add(wnd);
                return true;
            }, IntPtr.Zero);

            return windows;
        }

        public static IEnumerable<IntPtr> FindWindowsWithText(string titleText)
        {
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows(delegate(IntPtr wnd, IntPtr param)
            {
                if (GetWindowText(wnd).Contains(titleText))
                {
                    windows.Add(wnd);
                }
                return true;
            },
                IntPtr.Zero);

            return windows;
        } // closing brackeT

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsZoomed(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, ShowWindowCommands cmdShow);



        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
    }
}