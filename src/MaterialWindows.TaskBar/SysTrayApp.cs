using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialWindows.TaskBar.ViewModels;
using MaterialWindows.TaskBar.Win32Interop;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NHotkey.WindowsForms;
using Keys = System.Windows.Forms.Keys;

namespace MaterialWindows.TaskBar
{
    public class SysTrayApp
    {
        
        
        private MainWindowViewModel UIModel;
        private ThisApplicationContext applicationContext;

        public SysTrayApp(ThisApplicationContext applicationContext, MainWindowViewModel uIModel)
        {
            this.applicationContext = applicationContext;
            UIModel = uIModel;
        }

        public void Init()
        {
            
            var superkey = Keys.Control | Keys.Alt;

            HotkeyManager.Current.AddOrReplace("CycleLayouts", superkey | Keys.Space, cycleLayouts);
            HotkeyManager.Current.AddOrReplace("ReverseCycleLayouts", superkey | Keys.Shift | Keys.Space, reverseCycleLayouts);
            
            HotkeyManager.Current.AddOrReplace("CycleMainWindow", superkey | Keys.Up, cycleMainPane);
            HotkeyManager.Current.AddOrReplace("ReverseCycleMainWindow", superkey | Keys.Down, reverseCycleMainPane);

            HotkeyManager.Current.AddOrReplace("IncreaseMainPane", superkey | Keys.H, increaseMainPane);
            HotkeyManager.Current.AddOrReplace("DecreaseMainPane", superkey | Keys.Shift | Keys.H, decreaseMainPane);
            
            HotkeyManager.Current.AddOrReplace("CycleAssignedScreen", superkey | Keys.Right, cycleAssignedScreen);
            HotkeyManager.Current.AddOrReplace("ReverseCycleAssignedScreen", superkey | Keys.Left, reverseCycleAssignedScreen);

            HotkeyManager.Current.AddOrReplace("DumpWindowList", superkey | Keys.K, dumpWindowList);
            
            // HotkeyManager.Current.AddOrReplace("Exit", superkey | Keys.Q, (s, e) => applicationContext.ExitThread());
        }

        public void Run()
        {
            // applicationContext.ThreadExit += (object sender, EventArgs e) => desktop.Shutdown(0);
            Task.Run(() => System.Windows.Forms.Application.Run(applicationContext));
        }

        private void cycleAssignedScreen(object sender, EventArgs eventArgs)
        {
            int screenCount = System.Windows.Forms.Screen.AllScreens.Count();
            IntPtr activeWindowHandle = Win32.GetForegroundWindow();

            var activeWindow = UIModel.Windows.SingleOrDefault(w => w.Handle == activeWindowHandle);

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

        private void reverseCycleAssignedScreen(object sender, EventArgs eventArgs)
        {
            int screenCount = Screen.AllScreens.Count();
            IntPtr activeWindowHandle = Win32.GetForegroundWindow();

            var activeWindow = UIModel.Windows.SingleOrDefault(w => w.Handle == activeWindowHandle);

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

        private void dumpWindowList(object sender, EventArgs eventArgs)
        {
            Console.WriteLine(JsonConvert.SerializeObject(UIModel));
            // log.Info(new  {
            //     Message = "Window List",
            //     List = UIModel.Windows
            // });
        }
        
        private void reverseCycleLayouts(object sender, EventArgs eventArgs)
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

        private void cycleLayouts(object sender, EventArgs eventArgs)
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

        private void increaseMainPane(object sender, EventArgs eventArgs)
        {
            UIModel.CurrentLayout.MainPaneSize *= 1.1f;

            // log.Info(new
            // {
            //     Message = "Main Pane Size Changed",
            //     UIModel.CurrentLayout.MainPaneSize,
            // });
        }

        private void decreaseMainPane(object sender, EventArgs eventArgs)
        {
            UIModel.CurrentLayout.MainPaneSize /= 1.1f;

            // log.Info(new
            // {
            //     Message = "Main Pane Size Changed",
            //     UIModel.CurrentLayout.MainPaneSize,
            // });
        }

        private void cycleMainPane(object sender, EventArgs eventArgs)
        {
            var lastWindow = UIModel.Windows.Last();
            UIModel.Windows.Remove(lastWindow);
            UIModel.Windows.Insert(0, lastWindow);
        }

        private void reverseCycleMainPane(object sender, EventArgs eventArgs)
        {
            var firstWindow = UIModel.Windows.First();
            UIModel.Windows.RemoveAt(0);
            UIModel.Windows.Add(firstWindow);
        }
    }
}