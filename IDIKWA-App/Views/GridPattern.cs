using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class GridPattern : Control, IStyleable
    {
        public static readonly StyledProperty<AlignmentX> AlignmentXProperty = AvaloniaProperty.Register<GridPattern, AlignmentX>(nameof(AlignmentX), AlignmentX.Left);
        public static readonly StyledProperty<AlignmentY> AlignmentYProperty = AvaloniaProperty.Register<GridPattern, AlignmentY>(nameof(AlignmentY), AlignmentY.Top);
        public static readonly StyledProperty<IBrush?> BackgroundProperty = AvaloniaProperty.Register<GridPattern, IBrush?>(nameof(Background), null);
        public static readonly StyledProperty<double> PatternHeightProperty = AvaloniaProperty.Register<GridPattern, double>(nameof(PatternHeight), 10);
        public static readonly StyledProperty<double> PatternWidthProperty = AvaloniaProperty.Register<GridPattern, double>(nameof(PatternWidth), 10);
        public static readonly StyledProperty<IBrush?> StrokeProperty = AvaloniaProperty.Register<GridPattern, IBrush?>(nameof(Stroke), null);

        static GridPattern()
        {
            AlignmentXProperty.Changed.AddClassHandler<GridPattern>(RenderPropertyChanged);
            AlignmentYProperty.Changed.AddClassHandler<GridPattern>(RenderPropertyChanged);
            BackgroundProperty.Changed.AddClassHandler<GridPattern>(RenderPropertyChanged);
            StrokeProperty.Changed.AddClassHandler<GridPattern>(RenderPropertyChanged);
            PatternHeightProperty.Changed.AddClassHandler<GridPattern>(RenderPropertyChanged);
            PatternWidthProperty.Changed.AddClassHandler<GridPattern>(RenderPropertyChanged);
        }

        public GridPattern()
        {
            ClipToBounds = true;
        }

        public AlignmentX AlignmentX { get => GetValue(AlignmentXProperty); set => SetValue(AlignmentXProperty, value); }
        public AlignmentY AlignmentY { get => GetValue(AlignmentYProperty); set => SetValue(AlignmentYProperty, value); }
        public IBrush? Background { get => GetValue(BackgroundProperty); set => SetValue(BackgroundProperty, value); }
        public double PatternHeight { get => GetValue(PatternHeightProperty); set => SetValue(PatternHeightProperty, value); }
        public double PatternWidth { get => GetValue(PatternWidthProperty); set => SetValue(PatternWidthProperty, value); }
        public IBrush? Stroke { get => GetValue(StrokeProperty); set => SetValue(StrokeProperty, value); }
        Type IStyleable.StyleKey => typeof(GridPattern);

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            context.FillRectangle(Background, Bounds);
            var pen = new Pen(Stroke);
            double xOffset = 0;
            if (AlignmentX == AlignmentX.Right)
                xOffset = Bounds.Width % PatternWidth;
            else if (AlignmentX == AlignmentX.Center)
                xOffset = (Bounds.Width % PatternWidth) / 2;
            xOffset = (int)xOffset - .5f;
            double yOffset = 0;
            if (AlignmentY == AlignmentY.Bottom)
                yOffset = Bounds.Height % PatternHeight;
            else if (AlignmentY == AlignmentY.Center)
                yOffset = (Bounds.Height / 2) % PatternHeight;
            yOffset = (int)yOffset - .5f;

            for (int i = -1; i < Bounds.Width / PatternWidth; ++i)
            {
                var x = i * PatternWidth;
                for (int j = -1; j < Bounds.Height / PatternHeight; ++j)
                {
                    var y = j * PatternHeight;
                    context.DrawLine(pen, new Point(x, y + yOffset), new Point(x + PatternWidth, y + yOffset));
                    context.DrawLine(pen, new Point(x + xOffset, y), new Point(x + xOffset, y + PatternWidth));
                }
            }
        }

        private static void RenderPropertyChanged(GridPattern sender, AvaloniaPropertyChangedEventArgs e)
        {
            sender.InvalidateVisual();
        }
    }
}