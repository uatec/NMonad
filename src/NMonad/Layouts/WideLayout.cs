using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NMonad.Win32Interop;

namespace NMonad.Layouts
{
    public class WideLayout : Layout
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void ReflowScreen(Screen screen, List<Window> windows, Window activeWindow)
        {
            if (windows.Count == 0) return;

            int mainPanelCount = Math.Min(windows.Count, base.MainPaneCount);
            int secondaryPanelCount = windows.Count - mainPanelCount;

            bool hasSecondaryPane = secondaryPanelCount > 0;

            double mainPaneWindowWidth = Math.Round((double)screen.WorkingArea.Width / mainPanelCount);
            double secondaryPaneWindowWidth = hasSecondaryPane
                ? Math.Round((double)screen.WorkingArea.Width / secondaryPanelCount)
                : 0.0;

            double mainPaneWindowHeight =
                Math.Round((double)screen.WorkingArea.Height * (hasSecondaryPane ? base.MainPaneSize : 1));
            double secondaryPaneWindowHeight = screen.WorkingArea.Height - mainPaneWindowHeight;

            for (int i = 0; i < windows.Count; ++i)
            {
                Window window = windows[i];
                RectangleBuilder frame = new RectangleBuilder();

                if (i < mainPanelCount)
                {
                    frame.X = (int)(screen.WorkingArea.X + (mainPaneWindowWidth * i));
                    frame.Y = screen.WorkingArea.Y;
                    frame.Width = (int)mainPaneWindowWidth;
                    frame.Height = (int)mainPaneWindowHeight;
                }
                else
                {
                    frame.X = (int)(screen.WorkingArea.X + (secondaryPaneWindowWidth * (i - mainPanelCount)));
                    frame.Y = (int)(screen.WorkingArea.Y + mainPaneWindowWidth);
                    frame.Width = (int)secondaryPaneWindowWidth;
                    frame.Height = (int)secondaryPaneWindowHeight;
                }
                base.SetWindowPosition(window, frame);
            }
        }
    }
}