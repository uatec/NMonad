using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MaterialWindows.TaskBar.Views
{
    public class WindowRow : UserControl
    {
        public static readonly StyledProperty<MaterialWindows.TaskBar.ViewModels.WindowRow> RowProperty =
        AvaloniaProperty.Register<WindowRow, MaterialWindows.TaskBar.ViewModels.WindowRow>(nameof(Row));

        public MaterialWindows.TaskBar.ViewModels.WindowRow Row { get; set; }

        public WindowRow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}