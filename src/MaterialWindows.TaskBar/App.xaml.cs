using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MaterialWindows.TaskBar.Reflow.Layouts;
using MaterialWindows.TaskBar.ViewModels;
using MaterialWindows.TaskBar.Views;
using MaterialWindows.TaskBar.Win32Interop;
using Microsoft.Extensions.Configuration;
using Screen = System.Windows.Forms.Screen;

namespace MaterialWindows.TaskBar
{

    public class App : Application
    {
        private IConfiguration _configuration;
        public App()
        {
            throw new NotImplementedException("you must use DI");
        }

        public App(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private MainWindowViewModel UIModel = new MainWindowViewModel();
        private RuntimeModel ReflowModel = new RuntimeModel();
        private ThisApplicationContext applicationContext = new ThisApplicationContext();

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {

                Console.WriteLine("init complete");

                // init systray and hotkeys
                var systrayapp = new SysTrayApp(applicationContext, ReflowModel, UIModel);
                systrayapp.Init();

                // init UI
                desktop.MainWindow = new HorizontalBar
                {
                    DataContext = UIModel,
                };

                // init reflow

                var config = _configuration.Get<RootConfig>();

                Dictionary<string, Layout> registeredLayout = new Dictionary<string, Layout> {
                    { "basic", new BasicLayout() },
                    { "column", new ColumnLayout() },
                    { "tall", new TallLayout() },
                    { "fullscreen", new FullscreenLayout() },
                    { "floating", new FloatingLayout() },
                    { "wide", new WideLayout() }
                };
                
                ReflowModel.ActiveLayouts = registeredLayout.Where(kvp => config.Layouts.Contains(kvp.Key)).Select(kvp => kvp.Value).ToList();
                
                var newWindow = new VerticalBar { DataContext = UIModel };
                // run reflow
                object syncRoot = new object();
                new Timer(state =>
                    {
                        if (Monitor.TryEnter(syncRoot, 100))
                        {
                            ReflowStep(config);
                            Monitor.Exit(syncRoot);
                        }
                    }, null, 0, 100);
                // run systray

                systrayapp.Run();

                // run ui

                newWindow.Show();
            }

            base.OnFrameworkInitializationCompleted();
        }
        
        private void ReflowStep(RootConfig config)
        {
            // try
            {
                List<IntPtr> extantWindowHandles = new List<IntPtr>();
                IntPtr activeWindow = Win32.GetActiveWindow();

                foreach (IntPtr ptr in Win32.GetAllWindows())
                {
                    string windowName = Win32.GetWindowText(ptr);
                    if (string.IsNullOrEmpty(windowName)) continue;
                    //if (ignoredWindows.Any(w => -1 < windowName.IndexOf(w, StringComparison.CurrentCultureIgnoreCase))) continue;
                    if (config.ExcludedWindows.Contains(windowName)) continue;
                    if (!Win32.IsWindowVisible(ptr)) continue;
                    if (Win32.IsIconic(ptr)) continue;
                    
                    extantWindowHandles.Add(ptr);
                }

                var knownHandles = ReflowModel.Windows.Select(w => w.Handle).ToList();
                var oldWindows = knownHandles.Except(extantWindowHandles).ToList();
                var newWindows = extantWindowHandles.Except(knownHandles).ToList();

                foreach (var w in newWindows)
                {
                    // Only ignore small windows if they're added. If an existing window becomes small we still want to control it... i suspect
                    if (WindowsIsSmall(w)) continue;

                    string windowName = Win32.GetWindowText(w);
                    // Assign this window to screen 0, or the next screen that doesn't have anything assigned
                    int screenId = ReflowModel.Windows.Any() ? ReflowModel.Windows.Max(x => x.ScreenId) + 1 : 0;

                    // if we have tried to assign it to a screen that doesn't exist
                    Dictionary<int, int> screenWindowCounts = new Dictionary<int, int>();
                    if (screenId >= Screen.AllScreens.Length)
                    {

                        // figure out how many windows each screen has
                        // create entries for all screens that default to zero
                        for (int i = 0; i < Screen.AllScreens.Length; i++)
                        {
                            screenWindowCounts[i] = 0;
                        }
                        // then update the ones that have windows with the real numbers
                        var windowGroups = ReflowModel.Windows.GroupBy(x => x.ScreenId);
                        foreach (var wg in windowGroups)
                        {
                            screenWindowCounts[wg.Key] = wg.Count();
                        }
                        // and assign this window to the screen with the fewest windows
                        screenId = screenWindowCounts.OrderBy(kvp => kvp.Value).First().Key;
                    }
                    // log.Info(new
                    // {
                    //     Message = "Window Added",
                    //     screenId,
                    //     windowName,
                    //     ScreenWindowCounts = JsonConvert.SerializeObject(screenWindowCounts)
                    // });
                    
                    ReflowModel.Windows.Add(new MaterialWindows.TaskBar.Win32Interop.Window
                    {
                        Handle = w,
                        Name = windowName,
                        ScreenId = screenId
                    });
                }

                foreach (var w in oldWindows)
                {
                    var removedWindow = ReflowModel.Windows.Single(w1 => w1.Handle == w);
                    ReflowModel.Windows.Remove(removedWindow);

                    Dictionary<int, int> screenWindowCounts = new Dictionary<int, int>();

                    // figure out how many windows each screen has
                    // create entries for all screens that default to zero
                    for (int i = 0; i < Screen.AllScreens.Length; i++)
                    {
                        screenWindowCounts[i] = 0;
                    }
                    // then update the ones that have windows with the real numbers
                    var windowGroups = ReflowModel.Windows.GroupBy(x => x.ScreenId);
                    foreach (var wg in windowGroups)
                    {
                        screenWindowCounts[wg.Key] = wg.Count();
                    }

                    // log.Info(new
                    // {
                    //     Message = "Window Removed",
                    //     removedWindow.ScreenId,
                    //     removedWindow.Name,
                    //     remainWindows = Model.Windows.Count(w2 => w2.ScreenId == removedWindow.ScreenId),
                    //     ScreenWindowCounts = JsonConvert.SerializeObject(screenWindowCounts)
                    // });
                }

                foreach (var windowGroup in ReflowModel.Windows
                    .GroupBy(x => x.ScreenId))
                {
                    if (windowGroup.Key >= Screen.AllScreens.Count())
                    {
                        // log.Warn(new
                        // {
                        //     Message = "Invalid screen Id - Suspected screen count change, reassigning windows to screens and reflowing.",
                        //     AttemptedScreenId = windowGroup.Key,
                        //     NumberOfScreens = Screen.AllScreens.Count()
                        // });
                        ReflowModel.Windows.ForEach(w => w.ScreenId = -1);
                        return;
                    }

                    ReflowModel.CurrentLayout.ReflowScreen(Screen.AllScreens[windowGroup.Key], windowGroup.ToList(), windowGroup.ToList().SingleOrDefault(w => w.Handle == activeWindow));
                }

            }
            // catch (Exception ex)
            // {
            //     log.Fatal("unhandled fatal exception", ex);
            //     throw;
            // }
        }


        private static bool WindowsIsSmall(IntPtr handle)
        {
            // First we intialize an empty Rectangle object.
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle();

            // Then we call the GetWindowRect function, passing in a reference to the rect object.
            Win32.GetWindowRect(handle, ref rect);

            // Threshold is hardcoded here cos i cba to drive it from config yet
            System.Drawing.Size threshold = new System.Drawing.Size(650, 450);
            // the rectangle we just got back returns the Right position in the Width field, subtract X to get the window width.
            // same for Y axis
            return rect.Width - rect.X < threshold.Width && rect.Height - rect.Y < threshold.Height;
        }
    }
}