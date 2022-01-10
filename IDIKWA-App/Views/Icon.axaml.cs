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
        public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<Icon, bool>(nameof(IsSelected), false);
        public static readonly StyledProperty<IBrush?> PrimaryBrushProperty = AvaloniaProperty.Register<Icon, IBrush?>(nameof(PrimaryBrush), null);
        public static readonly StyledProperty<Geometry?> PrimaryFillProperty = AvaloniaProperty.Register<Icon, Geometry?>(nameof(PrimaryFill), null);
        public static readonly StyledProperty<Geometry?> PrimaryStrokeProperty = AvaloniaProperty.Register<Icon, Geometry?>(nameof(PrimaryStroke), null);
        public static readonly StyledProperty<IBrush?> SecondaryBrushProperty = AvaloniaProperty.Register<Icon, IBrush?>(nameof(SecondaryBrush), null);
        public static readonly StyledProperty<Geometry?> SecondaryFillProperty = AvaloniaProperty.Register<Icon, Geometry?>(nameof(SecondaryFill), null);
        public static readonly StyledProperty<Geometry?> SecondaryStrokeProperty = AvaloniaProperty.Register<Icon, Geometry?>(nameof(SecondaryStroke), null);
        public static readonly StyledProperty<IBrush?> TertiaryBrushProperty = AvaloniaProperty.Register<Icon, IBrush?>(nameof(TertiaryBrush), null);
        public static readonly StyledProperty<Geometry?> TertiaryFillProperty = AvaloniaProperty.Register<Icon, Geometry?>(nameof(TertiaryFill), null);
        public static readonly StyledProperty<Geometry?> TertiaryStrokeProperty = AvaloniaProperty.Register<Icon, Geometry?>(nameof(TertiaryStroke), null);

        static Icon()
        {
            IsSelectedProperty.Changed.AddClassHandler<Icon>(OnSelectedChanged);
        }

        public Icon()
        {
            InitializeComponent();
        }

        public bool IsSelected { get => GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
        public IBrush? PrimaryBrush { get => GetValue(PrimaryBrushProperty); set => SetValue(PrimaryBrushProperty, value); }
        public Geometry? PrimaryFill { get => GetValue(PrimaryFillProperty); set => SetValue(PrimaryFillProperty, value); }
        public Geometry? PrimaryStroke { get => GetValue(PrimaryStrokeProperty); set => SetValue(PrimaryStrokeProperty, value); }
        public IBrush? SecondaryBrush { get => GetValue(SecondaryBrushProperty); set => SetValue(SecondaryBrushProperty, value); }
        public Geometry? SecondaryFill { get => GetValue(SecondaryFillProperty); set => SetValue(SecondaryFillProperty, value); }
        public Geometry? SecondaryStroke { get => GetValue(SecondaryStrokeProperty); set => SetValue(SecondaryStrokeProperty, value); }
        public IBrush? TertiaryBrush { get => GetValue(TertiaryBrushProperty); set => SetValue(TertiaryBrushProperty, value); }
        public Geometry? TertiaryFill { get => GetValue(TertiaryFillProperty); set => SetValue(TertiaryFillProperty, value); }
        public Geometry? TertiaryStroke { get => GetValue(TertiaryStrokeProperty); set => SetValue(TertiaryStrokeProperty, value); }

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