using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using Window = MaterialWindows.TaskBar.ViewModels.Window;

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

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Console.WriteLine("init complete");
                var actions = new Actions(UIModel);
                ThisApplicationContext applicationContext = new ThisApplicationContext(actions);

                // init systray and hotkeys
                var systrayapp = new SysTrayApp(applicationContext, actions);
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
                    { "twocolumn", new TwoColumnLayout() },
                    { "tall", new TallLayout() },
                    { "fullscreen", new FullscreenLayout() },
                    { "floating", new FloatingLayout() },
                    { "wide", new WideLayout() }
                };
                
                UIModel.ActiveLayouts = new ObservableCollection<Layout>(registeredLayout.Where(kvp => config.Layouts.Contains(kvp.Key)).Select(kvp => kvp.Value).ToList());
                
                var newWindow = new VerticalBar { DataContext = UIModel };
                // run reflow
                
                Task.Run(async () => {
                    do {
                        ReflowStep(config);
                        await Task.Delay(100);
                    } while ( true );
                });

                // run systray

                Task.Run(() => System.Windows.Forms.Application.Run(applicationContext));

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
                IntPtr activeWindow = Win32.GetForegroundWindow();

                foreach (IntPtr ptr in Win32.GetAllWindows())
                {
                    string windowName = Win32.GetWindowText(ptr);
                    if (string.IsNullOrEmpty(windowName)) continue;
                    //if (ignoredWindows.Any(w => -1 < windowName.IndexOf(w, StringComparison.CurrentCultureIgnoreCase))) continue;
                    if (config.ExcludedWindows.Contains(windowName)) continue;
                    if (!Win32.IsWindowVisible(ptr)) continue;
                    // if (Win32.IsIconic(ptr)) continue;
                    // if (WindowsIsSmall(ptr)) continue;
                    
                    extantWindowHandles.Add(ptr);
                }

                Console.Write("extant windows " + extantWindowHandles.Count);
                var knownHandles = UIModel.ActiveRow.Windows.Select(w => w.Handle).ToList();
                Console.WriteLine("Known Windows " + knownHandles.Count);
                var oldWindows = knownHandles.Except(extantWindowHandles).ToList();
                Console.WriteLine("Removed Windows " + oldWindows.Count);
                var newWindows = extantWindowHandles.Except(knownHandles).ToList();
                Console.WriteLine("New windows " + newWindows.Count);

                foreach (var w in newWindows)
                {
                    string windowName = Win32.GetWindowText(w);
                    // Assign this window to screen 0, or the next screen that doesn't have anything assigned
                    // int screenId = UIModel.Windows.Any() ? UIModel.Windows.Max(x => x.ScreenId) + 1 : 0;
                    // TODO: let's get our head around rows and columsn, THEN multiple screens
                    int screenId = 0;

                    // if we have tried to assign it to a screen that doesn't exist
                    // Dictionary<int, int> screenWindowCounts = new Dictionary<int, int>();
                    // if (screenId >= Screen.AllScreens.Length)
                    // {

                    //     // figure out how many windows each screen has
                    //     // create entries for all screens that default to zero
                    //     for (int i = 0; i < Screen.AllScreens.Length; i++)
                    //     {
                    //         screenWindowCounts[i] = 0;
                    //     }
                    //     // then update the ones that have windows with the real numbers
                    //     var windowGroups = UIModel.ActiveRow.Windows.GroupBy(x => x.ScreenId);
                    // #    foreach (var wg in windowGroups)
                    //     {
                    //         screenWindowCounts[wg.Key] = wg.Count();
                    //     }
                    //     // and assign this window to the screen with the fewest windows
                    //     screenId = screenWindowCounts.OrderBy(kvp => kvp.Value).First().Key;
                    // }
                    // log.Info(new
                    // {
                    //     Message = "Window Added",
                    //     screenId,
                    //     windowName,
                    //     ScreenWindowCounts = JsonConvert.SerializeObject(screenWindowCounts)
                    // });
                    Console.WriteLine("added " + windowName);
                    UIModel.ActiveRow.Windows.Add(new Window 
                    {
                        Handle = w,
                        Name = windowName,
                        ScreenId = screenId
                    });
                }

                foreach (var w in oldWindows)
                {
                    var removedWindow = UIModel.ActiveRow.Windows.Single(w1 => w1.Handle == w);
                    UIModel.ActiveRow.Windows.Remove(removedWindow);

                    // Dictionary<int, int> screenWindowCounts = new Dictionary<int, int>();

                    // // figure out how many windows each screen has
                    // // create entries for all screens that default to zero
                    // for (int i = 0; i < Screen.AllScreens.Length; i++)
                    // {
                    //     screenWindowCounts[i] = 0;
                    // }
                    // // then update the ones that have windows with the real numbers
                    // var windowGroups = UIModel.ActiveRow.Windows.GroupBy(x => x.ScreenId);
                    // foreach (var wg in windowGroups)
                    // {
                    //     screenWindowCounts[wg.Key] = wg.Count();
                    // }

                    // log.Info(new
                    // {
                    //     Message = "Window Removed",
                    //     removedWindow.ScreenId,
                    //     removedWindow.Name,
                    //     remainWindows = Model.Windows.Count(w2 => w2.ScreenId == removedWindow.ScreenId),
                    //     ScreenWindowCounts = JsonConvert.SerializeObject(screenWindowCounts)
                    // });
                }
                // TODO: Restore multi-screen
                // foreach (var windowGroup in UIModel.ActiveRow.Windows
                //     .GroupBy(x => x.ScreenId))
                // {
                //     if (windowGroup.Key >= Screen.AllScreens.Count())
                //     {
                //         // log.Warn(new
                //         // {
                //         //     Message = "Invalid screen Id - Suspected screen count change, reassigning windows to screens and reflowing.",
                //         //     AttemptedScreenId = windowGroup.Key,
                //         //     NumberOfScreens = Screen.AllScreens.Count()
                //         // });
                //         UIModel.ActiveRow.Windows.ToList().ForEach(w => w.ScreenId = -1);
                //         return;
                //     }
                // }
                // Console.WriteLine($"active window {activeWindow}");
                Console.WriteLine(UIModel.ActiveRow.Windows.Count);
                UIModel.CurrentLayout.ReflowScreen(Screen.PrimaryScreen, UIModel.ActiveRow.Windows.ToList(), UIModel.ActiveRow.Windows.ToList().SingleOrDefault(w => w.Handle == activeWindow));

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