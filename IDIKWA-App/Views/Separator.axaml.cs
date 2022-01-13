using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;

namespace IDIKWA_App
{
    public partial class Separator : UserControl
    {
        public static readonly StyledProperty<Orientation> OrientationProperty = AvaloniaProperty.Register<Separator, Orientation>(nameof(Orientation), Orientation.Horizontal);

        public Separator()
        {
            InitializeComponent();
        }

        public Orientation Orientation { get => GetValue(OrientationProperty); set => SetValue(OrientationProperty, value); }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}