using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using System;

namespace IDIKWA_App
{
    public partial class Icon : UserControl
    {
        public static readonly StyledProperty<Geometry?> BackgroundDataProperty = AvaloniaProperty.Register<Icon, Geometry?>(nameof(BackgroundData), null);
        public static readonly StyledProperty<Geometry?> ForegroundDataProperty = AvaloniaProperty.Register<Icon, Geometry?>(nameof(ForegroundData), null);
        public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<Icon, bool>(nameof(IsSelected), false);
        public static readonly StyledProperty<IBrush?> MainBrushProperty = AvaloniaProperty.Register<Icon, IBrush?>(nameof(MainBrush), null);
        public static readonly StyledProperty<IBrush?> SecondaryBrushProperty = AvaloniaProperty.Register<Icon, IBrush?>(nameof(SecondaryBrush), null);

        public Icon()
        {
            InitializeComponent();
            IsSelectedProperty.Changed.AddClassHandler<Icon>(OnSelectedChanged);
        }

        public Geometry? BackgroundData { get => GetValue(BackgroundDataProperty); set => SetValue(BackgroundDataProperty, value); }

        public Geometry? ForegroundData { get => GetValue(ForegroundDataProperty); set => SetValue(ForegroundDataProperty, value); }

        public bool IsSelected { get => GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }

        public IBrush? MainBrush { get => GetValue(MainBrushProperty); set => SetValue(MainBrushProperty, value); }

        public IBrush? SecondaryBrush { get => GetValue(SecondaryBrushProperty); set => SetValue(SecondaryBrushProperty, value); }

        private static void OnSelectedChanged(Icon sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is true)
                sender.PseudoClasses.Add(":selected");
            else
                sender.PseudoClasses.Remove(":selected");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}