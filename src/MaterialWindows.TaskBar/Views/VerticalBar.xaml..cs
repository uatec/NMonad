using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MaterialWindows.TaskBar.Views
{
    public class VerticalBar : Window
    {
        public VerticalBar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}