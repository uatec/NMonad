using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MaterialWindows.TaskBar.Win32Interop;
using Window = MaterialWindows.TaskBar.ViewModels.Window;

namespace MaterialWindows.TaskBar.Reflow.Layouts
{
    public class WideLayout : Layout
    {
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