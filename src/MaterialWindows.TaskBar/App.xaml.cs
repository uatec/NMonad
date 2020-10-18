using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MaterialWindows.TaskBar.Reflow.Layouts;
using MaterialWindows.TaskBar.ViewModels;
using MaterialWindows.TaskBar.Views;
using Microsoft.Extensions.Configuration;

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

                ReflowModel.ActiveLayouts = new System.Collections.Generic.List<Reflow.Layouts.Layout> {
                    new ColumnLayout()
                };

                var newWindow = new VerticalBar { DataContext = UIModel };
                // run reflow

                // run systray

                systrayapp.Run();

                // run ui

                newWindow.Show();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}