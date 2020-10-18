using System;
using System.Linq;
using System.Windows.Forms;
using MaterialWindows.TaskBar.ViewModels;
using MaterialWindows.TaskBar.Win32Interop;
using Newtonsoft.Json;

namespace MaterialWindows.TaskBar
{
    public class Actions
    {
        private MainWindowViewModel UIModel;

        public Actions(MainWindowViewModel uIModel)
        {
            UIModel = uIModel;
        }

        public void cycleAssignedScreen(object sender, EventArgs eventArgs)
        {
            int screenCount = System.Windows.Forms.Screen.AllScreens.Count();
            IntPtr activeWindowHandle = Win32.GetForegroundWindow();

            var activeWindow = UIModel.ActiveRow.Windows.SingleOrDefault(w => w.Handle == activeWindowHandle);

            if (activeWindow == null)
            {
                // log.Error(new
                // {
                //     Message = "Tried to cycle window between screens but could not identify the current window.",
                //     WindowName = Win32.GetWindowText(activeWindowHandle),
                //     WindowHandle = activeWindowHandle
                // });
                return;
            }

            int oldScreenId = activeWindow.ScreenId;
            activeWindow.ScreenId++;
            if (activeWindow.ScreenId >= screenCount)
            {
                activeWindow.ScreenId = 0;
            }
            
            // log.Info(new
            // {
            //     Message = "Window Moved Screen",
            //     WindowName = activeWindow.Name,
            //     NewScreenId = activeWindow.ScreenId,
            //     OldScreenId = oldScreenId 
            // });
        }

        public void reverseCycleAssignedScreen(object sender, EventArgs eventArgs)
        {
            int screenCount = Screen.AllScreens.Count();
            IntPtr activeWindowHandle = Win32.GetForegroundWindow();

            var activeWindow = UIModel.ActiveRow.Windows.SingleOrDefault(w => w.Handle == activeWindowHandle);

            if (activeWindow == null)
            {
                // log.Error(new
                // {
                //     Message = "Tried to cycle window between screens but could not identify the current window.",
                //     WindowName = Win32.GetWindowText(activeWindowHandle),
                //     WindowHandle = activeWindowHandle
                // });
                return;
            }

            int oldScreenId = activeWindow.ScreenId;
            activeWindow.ScreenId--;
            if (activeWindow.ScreenId < 0)
            {
                activeWindow.ScreenId = screenCount - 1;
            }
            
            // log.Info(new
            // {
            //     Message = "Window Moved Screen",
            //     WindowName = activeWindow.Name,
            //     NewScreenId = activeWindow.ScreenId,
            //     OldScreenId = oldScreenId
            // });
        }

        public void dumpWindowList(object sender, EventArgs eventArgs)
        {
            MessageBox.Show(JsonConvert.SerializeObject(UIModel.ActiveRow, Formatting.Indented), "State");
            // Console.WriteLine(JsonConvert.SerializeObject(UIModel.WindowRows, Formatting.Indented));
            // log.Info(new  {
            //     Message = "Window List",
            //     List = UIModel.Windows
            // });
        }
        
        public void reverseCycleLayouts(object sender, EventArgs eventArgs)
        {
            UIModel.CurrentLayoutIndex += 1;
            if (UIModel.CurrentLayoutIndex >= UIModel.ActiveLayouts.Count) UIModel.CurrentLayoutIndex = 0;
            // log.Info(new
            // {
            //     Message = "Layout Changed",
            //     Layout = UIModel.CurrentLayout.GetType().Name,
            //     UIModel.CurrentLayout.MainPaneSize,
            //     UIModel.CurrentLayout.MainPaneCount
            // });
            MessageForm.ShowMessage(UIModel.CurrentLayout.GetType().Name.Replace("Layout", " Layout"), 1000);
        }

        public void cycleLayouts(object sender, EventArgs eventArgs)
        {
            UIModel.CurrentLayoutIndex -= 1;
            if (UIModel.CurrentLayoutIndex == -1) UIModel.CurrentLayoutIndex = UIModel.ActiveLayouts.Count - 1;
            // log.Info(new
            // {
            //     Message = "Layout Changed",
            //     Layout = UIModel.CurrentLayout.GetType().Name,
            //     UIModel.CurrentLayout.MainPaneSize,
            //     UIModel.CurrentLayout.MainPaneCount
            // });
            MessageForm.ShowMessage(UIModel.CurrentLayout.GetType().Name.Replace("Layout", " Layout"), 1000);
        }

        public void increaseMainPane(object sender, EventArgs eventArgs)
        {
            UIModel.CurrentLayout.MainPaneSize *= 1.1f;

            // log.Info(new
            // {
            //     Message = "Main Pane Size Changed",
            //     UIModel.CurrentLayout.MainPaneSize,
            // });
        }

        public void decreaseMainPane(object sender, EventArgs eventArgs)
        {
            UIModel.CurrentLayout.MainPaneSize /= 1.1f;

            // log.Info(new
            // {
            //     Message = "Main Pane Size Changed",
            //     UIModel.CurrentLayout.MainPaneSize,
            // });
        }

        public void cycleMainPane(object sender, EventArgs eventArgs)
        {
            var lastWindow = UIModel.ActiveRow.Windows.Last();
            UIModel.ActiveRow.Windows.Remove(lastWindow);
            UIModel.ActiveRow.Windows.Insert(0, lastWindow);
        }

        public void reverseCycleMainPane(object sender, EventArgs eventArgs)
        {
            var firstWindow = UIModel.ActiveRow.Windows.First();
            UIModel.ActiveRow.Windows.RemoveAt(0);
            UIModel.ActiveRow.Windows.Add(firstWindow);
        }
    }
}