using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MaterialWindows.TaskBar.Views
{
    public class HorizontalBar : Window
    {
        public HorizontalBar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}