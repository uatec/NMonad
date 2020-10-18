using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MaterialWindows.TaskBar.ViewModels;
using MaterialWindows.TaskBar.Views;

namespace MaterialWindows.TaskBar
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var model = new MainWindowViewModel();

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