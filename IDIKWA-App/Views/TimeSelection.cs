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
    public class TimeSelection : Control, IStyleable
    {
        public static readonly StyledProperty<IBrush?> BackgroundBrushProperty = AvaloniaProperty.Register<TimeSelection, IBrush?>(nameof(BackgroundBrush), null);
        public static readonly StyledProperty<IBrush?> BoundsBrushProperty = AvaloniaProperty.Register<TimeSelection, IBrush?>(nameof(BoundsBrush), null);
        public static readonly StyledProperty<IBrush?> CursorBrushProperty = AvaloniaProperty.Register<TimeSelection, IBrush?>(nameof(CursorBrush), null);
        public static readonly StyledProperty<TimeSpan> DurationProperty = AvaloniaProperty.Register<TimeSelection, TimeSpan>(nameof(Duration), TimeSpan.Zero);
        public static readonly StyledProperty<IBrush?> GraduationBrushProperty = AvaloniaProperty.Register<TimeSelection, IBrush?>(nameof(GraduationBrush), null);
        public static readonly StyledProperty<double> HeaderSizeProperty = AvaloniaProperty.Register<TimeSelection, double>(nameof(HeaderSize), 30);
        public static readonly StyledProperty<TimeSpan> LeftBoundProperty = AvaloniaProperty.Register<TimeSelection, TimeSpan>(nameof(LeftBound), TimeSpan.Zero);
        public static readonly StyledProperty<TimeSpan> RightBoundProperty = AvaloniaProperty.Register<TimeSelection, TimeSpan>(nameof(RightBound), TimeSpan.Zero);
        public static readonly StyledProperty<TimeSpan> TimeCursorProperty = AvaloniaProperty.Register<TimeSelection, TimeSpan>(nameof(TimeCursor), TimeSpan.Zero);

        static TimeSelection()
        {
            BackgroundBrushProperty.Changed.AddClassHandler<TimeSelection>(RenderPropertyChanged);
            BoundsBrushProperty.Changed.AddClassHandler<TimeSelection>(RenderPropertyChanged);
            CursorBrushProperty.Changed.AddClassHandler<TimeSelection>(RenderPropertyChanged);
            DurationProperty.Changed.AddClassHandler<TimeSelection>(RenderPropertyChanged);
            GraduationBrushProperty.Changed.AddClassHandler<TimeSelection>(RenderPropertyChanged);
            LeftBoundProperty.Changed.AddClassHandler<TimeSelection>(RenderPropertyChanged);
            RightBoundProperty.Changed.AddClassHandler<TimeSelection>(RenderPropertyChanged);
            TimeCursorProperty.Changed.AddClassHandler<TimeSelection>(RenderPropertyChanged);
            HeaderSizeProperty.Changed.AddClassHandler<TimeSelection>(MeasurePropertyChanged);
        }

        public IBrush? BackgroundBrush { get => GetValue(BackgroundBrushProperty); set => SetValue(BackgroundBrushProperty, value); }
        public IBrush? BoundsBrush { get => GetValue(BoundsBrushProperty); set => SetValue(BoundsBrushProperty, value); }
        public IBrush? CursorBrush { get => GetValue(CursorBrushProperty); set => SetValue(CursorBrushProperty, value); }
        public TimeSpan Duration { get => GetValue(DurationProperty); set => SetValue(DurationProperty, value); }
        public IBrush? GraduationBrush { get => GetValue(GraduationBrushProperty); set => SetValue(GraduationBrushProperty, value); }
        public double HeaderSize { get => GetValue(HeaderSizeProperty); set => SetValue(HeaderSizeProperty, value); }
        public TimeSpan LeftBound { get => GetValue(LeftBoundProperty); set => SetValue(LeftBoundProperty, value); }
        public TimeSpan RightBound { get => GetValue(RightBoundProperty); set => SetValue(RightBoundProperty, value); }
        Type IStyleable.StyleKey => typeof(TimeSelection);
        public TimeSpan TimeCursor { get => GetValue(TimeCursorProperty); set => SetValue(TimeCursorProperty, value); }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            var gradutionPen = new Pen(GraduationBrush);
            var boundsPen = new Pen(BoundsBrush);
            var cursorPen = new Pen(CursorBrush);
            context.FillRectangle(BackgroundBrush, new Rect(new Point(0, 0), new Size(Bounds.Width, HeaderSize)));
            var smallIncrement = Duration >= TimeSpan.FromMinutes(4)
                ? TimeSpan.FromSeconds(5)
                : Duration >= TimeSpan.FromMinutes(2)
                    ? TimeSpan.FromSeconds(2)
                    : Duration >= TimeSpan.FromSeconds(30)
                    ? TimeSpan.FromSeconds(1)
                    : TimeSpan.FromSeconds(.5);
            var bigIncrement = Duration >= TimeSpan.FromMinutes(4)
                ? TimeSpan.FromSeconds(60)
                : Duration >= TimeSpan.FromMinutes(2)
                    ? TimeSpan.FromSeconds(30)
                    : Duration >= TimeSpan.FromSeconds(30)
                    ? TimeSpan.FromSeconds(15)
                    : TimeSpan.FromSeconds(5);
            for (TimeSpan time = Duration; time > TimeSpan.Zero; time -= smallIncrement)
            {
                var x = time / Duration * Bounds.Width;
                x = (int)x - .5;
                var bigIncrementMultiple = (Duration - time) / bigIncrement;
                if (Math.Abs(bigIncrementMultiple - Math.Round(bigIncrementMultiple)) < .01)
                {
                    var text = new FormattedText($"-{(Duration - time):m\\:ss}", Typeface.Default, 11, TextAlignment.Left, TextWrapping.NoWrap, new Size());
                    var offset = time > Duration * .95
                        ? -text.Bounds.Width
                        : time < Duration * .05
                            ? 0
                            : -text.Bounds.Width / 2;
                    context.DrawText(GraduationBrush, new Point(x + offset, HeaderSize - 26), text);
                    context.DrawLine(gradutionPen, new Point(x, HeaderSize - 10), new Point(x, HeaderSize));
                }
                else
                {
                    context.DrawLine(gradutionPen, new Point(x, HeaderSize - 5), new Point(x, HeaderSize));
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(availableSize.Width, HeaderSize);
        }

        private static void MeasurePropertyChanged(TimeSelection sender, AvaloniaPropertyChangedEventArgs e)
        {
            sender.InvalidateMeasure();
        }

        private static void RenderPropertyChanged(TimeSelection sender, AvaloniaPropertyChangedEventArgs e)
        {
            sender.InvalidateVisual();
        }
    }
}