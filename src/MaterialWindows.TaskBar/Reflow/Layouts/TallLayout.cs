using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MaterialWindows.TaskBar.Win32Interop;

namespace MaterialWindows.TaskBar.Reflow.Layouts
{
    public class TallLayout : Layout
    {

        public TallLayout()
        {
            base.MainPaneCount = 1;
            base.MainPaneSize = 0.5f;
        }

        public override void ReflowScreen(Screen screen, List<Window> windows, Window activeWindow)
        {
            if (windows.Count == 0) return;

            int mainPanelCount = Math.Min(windows.Count, base.MainPaneCount);
            int secondaryPanelCount = windows.Count - mainPanelCount;

            bool hasSecondaryPane = secondaryPanelCount > 0;

            double mainPaneWindowHeight = Math.Round((double)screen.WorkingArea.Height/mainPanelCount);
            double secondaryPaneWindowHeight = hasSecondaryPane
                ? Math.Round((double)screen.WorkingArea.Height / secondaryPanelCount)
                : 0.0;

            double mainPaneWindowWidth =
                Math.Round((double) screen.WorkingArea.Width*(hasSecondaryPane ? base.MainPaneSize : 1));
            double secondaryPaneWindowWidth = screen.WorkingArea.Width - mainPaneWindowWidth;

            for (int i = 0; i < windows.Count; ++i)
            {
                Window window = windows[i];
                RectangleBuilder frame = new RectangleBuilder();

                if (i < mainPanelCount)
                {
                    frame.X = screen.WorkingArea.X;
                    frame.Y = (int) (screen.WorkingArea.Y + (mainPaneWindowHeight*i));
                    frame.Width = (int) mainPaneWindowWidth;
                    frame.Height = (int) mainPaneWindowHeight;
                }
                else
                {
                    frame.X = (int)(screen.WorkingArea.X + mainPaneWindowWidth);
                    frame.Y = (int) (screen.WorkingArea.Y + (secondaryPaneWindowHeight*(i - mainPanelCount)));
                    frame.Width = (int) secondaryPaneWindowWidth;
                    frame.Height = (int) secondaryPaneWindowHeight;
                }
                base.SetWindowPosition(window, frame);
            }
        }
    }
}