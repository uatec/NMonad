using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {

                Console.WriteLine("init complete");
                // reflow model

                // UI model
                var model = new MainWindowViewModel();

                // run reflow

                // run systray

                var applicationContext = new ThisApplicationContext();
                applicationContext.ThreadExit += (object sender, EventArgs e) => desktop.Shutdown(0);
                Task.Run(() => System.Windows.Forms.Application.Run(applicationContext));
                // run ui

                desktop.MainWindow = new HorizontalBar
                {
                    DataContext = model,
                };

                var newWindow = new VerticalBar { DataContext = model };
                newWindow.Show();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}