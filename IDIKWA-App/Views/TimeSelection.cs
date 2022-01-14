using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using Avalonia.Layout;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;

namespace IDIKWA_App
{
    public class TimeSelection : Control, IStyleable
    {
        public static readonly StyledPropertyBase<IBrush?> BackgroundBrushProperty = AvaloniaProperty.Register<TimeSelection, IBrush?>(nameof(BackgroundBrush), null);
        public static readonly StyledPropertyBase<IBrush?> BoundsBrushProperty = AvaloniaProperty.Register<TimeSelection, IBrush?>(nameof(BoundsBrush), null);
        public static readonly StyledPropertyBase<IBrush?> CursorBrushProperty = AvaloniaProperty.Register<TimeSelection, IBrush?>(nameof(CursorBrush), null);
        public static readonly StyledPropertyBase<TimeSpan> DurationProperty = AvaloniaProperty.Register<TimeSelection, TimeSpan>(nameof(Duration), TimeSpan.Zero);
        public static readonly StyledPropertyBase<IBrush?> GraduationBrushProperty = AvaloniaProperty.Register<TimeSelection, IBrush?>(nameof(GraduationBrush), null);
        public static readonly StyledPropertyBase<double> HeaderSizeProperty = AvaloniaProperty.Register<TimeSelection, double>(nameof(HeaderSize), 40);
        public static readonly DirectPropertyBase<bool> IsDraggingProperty = AvaloniaProperty.RegisterDirect<TimeSelection, bool>(nameof(IsDragging), o => o.IsDragging);
        public static readonly StyledPropertyBase<TimeSpan> LeftBoundProperty = AvaloniaProperty.Register<TimeSelection, TimeSpan>(nameof(LeftBound), TimeSpan.Zero);
        public static readonly StyledPropertyBase<Rectangle?> LeftRectangleProperty = AvaloniaProperty.Register<TimeSelection, Rectangle?>(nameof(LeftRectangle), null);
        public static readonly StyledPropertyBase<double> MaxCursorHeightProperty = AvaloniaProperty.Register<TimeSelection, double>(nameof(MaxCursorHeight), double.PositiveInfinity);
        public static readonly StyledPropertyBase<TimeSpan> RightBoundProperty = AvaloniaProperty.Register<TimeSelection, TimeSpan>(nameof(RightBound), TimeSpan.Zero);
        public static readonly StyledPropertyBase<Rectangle?> RightRectangleProperty = AvaloniaProperty.Register<TimeSelection, Rectangle?>(nameof(RightRectangle), null);
        public static readonly StyledPropertyBase<TimeSpan> TimeCursorProperty = AvaloniaProperty.Register<TimeSelection, TimeSpan>(nameof(TimeCursor), TimeSpan.Zero);

        private int draggedBound;
        private bool isDragging;

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
            LeftRectangleProperty.Changed.AddClassHandler<TimeSelection>(RenderPropertyChanged);
            RightRectangleProperty.Changed.AddClassHandler<TimeSelection>(RenderPropertyChanged);
            MaxCursorHeightProperty.Changed.AddClassHandler<TimeSelection>(RenderPropertyChanged);
            IsDraggingProperty.Changed.AddClassHandler<TimeSelection>(RenderPropertyChanged);
            HeaderSizeProperty.Changed.AddClassHandler<TimeSelection>(MeasurePropertyChanged);
        }

        public IBrush? BackgroundBrush { get => GetValue(BackgroundBrushProperty); set => SetValue(BackgroundBrushProperty, value); }
        public IBrush? BoundsBrush { get => GetValue(BoundsBrushProperty); set => SetValue(BoundsBrushProperty, value); }
        public IBrush? CursorBrush { get => GetValue(CursorBrushProperty); set => SetValue(CursorBrushProperty, value); }
        public TimeSpan Duration { get => GetValue(DurationProperty); set => SetValue(DurationProperty, value); }
        public IBrush? GraduationBrush { get => GetValue(GraduationBrushProperty); set => SetValue(GraduationBrushProperty, value); }
        public double HeaderSize { get => GetValue(HeaderSizeProperty); set => SetValue(HeaderSizeProperty, value); }

        public bool IsDragging
        {
            get => isDragging;
            private set
            {
                SetAndRaise(IsDraggingProperty, ref isDragging, value);
            }
        }

        public TimeSpan LeftBound { get => GetValue(LeftBoundProperty); set => SetValue(LeftBoundProperty, value); }
        public Rectangle? LeftRectangle { get => GetValue(LeftRectangleProperty); set => SetValue(LeftRectangleProperty, value); }
        public double MaxCursorHeight { get => GetValue(MaxCursorHeightProperty); set => SetValue(MaxCursorHeightProperty, value); }
        public TimeSpan RightBound { get => GetValue(RightBoundProperty); set => SetValue(RightBoundProperty, value); }
        public Rectangle? RightRectangle { get => GetValue(RightRectangleProperty); set => SetValue(RightRectangleProperty, value); }
        Type IStyleable.StyleKey => typeof(TimeSelection);
        public TimeSpan TimeCursor { get => GetValue(TimeCursorProperty); set => SetValue(TimeCursorProperty, value); }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            var maxheight = Math.Min(Bounds.Height, HeaderSize + MaxCursorHeight);
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
                    var text = new FormattedText($"-{(Duration - time):m\\:ss}", Typeface.Default, 12, TextAlignment.Left, TextWrapping.NoWrap, new Size());
                    var offset = x;
                    if (offset - text.Bounds.Width / 2 < 0)
                        offset = text.Bounds.Width / 2;
                    if (offset + text.Bounds.Width / 2 > Bounds.Width)
                        offset = Bounds.Width - text.Bounds.Width / 2;
                    context.DrawText(GraduationBrush, new Point(offset - text.Bounds.Width / 2, HeaderSize - 37), text);
                    context.DrawLine(gradutionPen, new Point(x, HeaderSize - 20), new Point(x, HeaderSize));
                }
                else
                {
                    context.DrawLine(gradutionPen, new Point(x, HeaderSize - 10), new Point(x, HeaderSize));
                }
            }
            {
                var x = TimeCursor / Duration * Bounds.Width;
                x = (int)x - .5;
                context.DrawLine(cursorPen, new Point(x, HeaderSize + 2), new Point(x, maxheight - 2));
            }
            {
                var x = LeftBound / Duration * Bounds.Width;
                x = (int)x - .5;
                var geometry = new PathGeometry();
                var triangle = new PathFigure()
                {
                    StartPoint = new Point(x, HeaderSize + 2)
                };
                triangle.Segments = new PathSegments();
                triangle.Segments.Add(new LineSegment()
                {
                    Point = new Point(x, HeaderSize - 20)
                });
                triangle.Segments.Add(new LineSegment()
                {
                    Point = new Point(x - 12, HeaderSize - 20)
                });
                triangle.Segments.Add(new LineSegment()
                {
                    Point = new Point(x - 12, HeaderSize - 10)
                });
                var line = new PathFigure()
                {
                    IsClosed = false,
                    StartPoint = new Point(x, HeaderSize + 2)
                };
                line.Segments = new PathSegments();
                line.Segments.Add(new LineSegment()
                {
                    Point = new Point(x, maxheight - 2)
                });
                var grip1 = new PathFigure()
                {
                    IsClosed = false,
                    StartPoint = new Point(x - 10.5, HeaderSize - 18)
                };
                grip1.Segments = new PathSegments();
                grip1.Segments.Add(new LineSegment()
                {
                    Point = new Point(x - 9, HeaderSize - 18)
                });
                grip1.Segments.Add(new LineSegment()
                {
                    Point = new Point(x - 9, HeaderSize - 12)
                });
                grip1.Segments.Add(new LineSegment()
                {
                    Point = new Point(x - 10.5, HeaderSize - 12)
                });
                var grip2 = new PathFigure()
                {
                    IsClosed = false,
                    StartPoint = new Point(x - 7.5, HeaderSize - 18)
                };
                grip2.Segments = new PathSegments();
                grip2.Segments.Add(new LineSegment()
                {
                    Point = new Point(x - 6, HeaderSize - 18)
                });
                grip2.Segments.Add(new LineSegment()
                {
                    Point = new Point(x - 6, HeaderSize - 9)
                });
                grip2.Segments.Add(new LineSegment()
                {
                    Point = new Point(x - 7.5, HeaderSize - 9)
                });
                var grip3 = new PathFigure()
                {
                    IsClosed = false,
                    StartPoint = new Point(x - 4.5, HeaderSize - 18)
                };
                grip3.Segments = new PathSegments();
                grip3.Segments.Add(new LineSegment()
                {
                    Point = new Point(x - 3, HeaderSize - 18)
                });
                grip3.Segments.Add(new LineSegment()
                {
                    Point = new Point(x - 3, HeaderSize - 6)
                });
                grip3.Segments.Add(new LineSegment()
                {
                    Point = new Point(x - 4.5, HeaderSize - 6)
                });
                geometry.Figures.Add(triangle);
                geometry.Figures.Add(line);
                geometry.Figures.Add(grip1);
                geometry.Figures.Add(grip2);
                geometry.Figures.Add(grip3);
                if (LeftRectangle is not null)
                {
                    LeftRectangle.Width = x;
                }
                context.DrawGeometry(BoundsBrush, boundsPen, geometry);
            }
            {
                var x = RightBound / Duration * Bounds.Width;
                x = (int)x - .5;
                var geometry = new PathGeometry();
                var triangle = new PathFigure()
                {
                    StartPoint = new Point(x, HeaderSize + 2)
                };
                triangle.Segments = new PathSegments();
                triangle.Segments.Add(new LineSegment()
                {
                    Point = new Point(x, HeaderSize - 20)
                });
                triangle.Segments.Add(new LineSegment()
                {
                    Point = new Point(x + 12, HeaderSize - 20)
                });
                triangle.Segments.Add(new LineSegment()
                {
                    Point = new Point(x + 12, HeaderSize - 10)
                });
                var line = new PathFigure()
                {
                    IsClosed = false,
                    StartPoint = new Point(x, HeaderSize + 2)
                };
                line.Segments = new PathSegments();
                line.Segments.Add(new LineSegment()
                {
                    Point = new Point(x, maxheight - 2)
                });
                var grip1 = new PathFigure()
                {
                    IsClosed = false,
                    StartPoint = new Point(x + 10.5, HeaderSize - 18)
                };
                grip1.Segments = new PathSegments();
                grip1.Segments.Add(new LineSegment()
                {
                    Point = new Point(x + 9, HeaderSize - 18)
                });
                grip1.Segments.Add(new LineSegment()
                {
                    Point = new Point(x + 9, HeaderSize - 12)
                });
                grip1.Segments.Add(new LineSegment()
                {
                    Point = new Point(x + 10.5, HeaderSize - 12)
                });
                var grip2 = new PathFigure()
                {
                    IsClosed = false,
                    StartPoint = new Point(x + 7.5, HeaderSize - 18)
                };
                grip2.Segments = new PathSegments();
                grip2.Segments.Add(new LineSegment()
                {
                    Point = new Point(x + 6, HeaderSize - 18)
                });
                grip2.Segments.Add(new LineSegment()
                {
                    Point = new Point(x + 6, HeaderSize - 9)
                });
                grip2.Segments.Add(new LineSegment()
                {
                    Point = new Point(x + 7.5, HeaderSize - 9)
                });
                var grip3 = new PathFigure()
                {
                    IsClosed = false,
                    StartPoint = new Point(x + 4.5, HeaderSize - 18)
                };
                grip3.Segments = new PathSegments();
                grip3.Segments.Add(new LineSegment()
                {
                    Point = new Point(x + 3, HeaderSize - 18)
                });
                grip3.Segments.Add(new LineSegment()
                {
                    Point = new Point(x + 3, HeaderSize - 6)
                });
                grip3.Segments.Add(new LineSegment()
                {
                    Point = new Point(x + 4.5, HeaderSize - 6)
                });
                geometry.Figures.Add(triangle);
                geometry.Figures.Add(line);
                geometry.Figures.Add(grip1);
                geometry.Figures.Add(grip2);
                geometry.Figures.Add(grip3);
                if (RightRectangle is not null)
                {
                    RightRectangle.Width = Bounds.Width - x;
                }
                context.DrawGeometry(BoundsBrush, boundsPen, geometry);
            }
            if (isDragging)
            {
                var timeFormat = Duration > TimeSpan.FromSeconds(30)
                    ? "m\\:ss"
                    : "m\\:ss\\.ff";
                if (draggedBound == 0)
                {
                    var x = LeftBound / Duration * Bounds.Width - 11;
                    var text = new FormattedText($"-{(Duration - LeftBound).ToString(timeFormat)}", Typeface.Default, 12, TextAlignment.Left, TextWrapping.NoWrap, new Size());
                    if (x + text.Bounds.Width + 6 > Bounds.Width)
                        x = Bounds.Width - 6 - text.Bounds.Width;
                    context.FillRectangle(BoundsBrush, new Rect(x - 3, HeaderSize - 36, text.Bounds.Width + 6, 18));
                    context.DrawText(BackgroundBrush, new Point(x, HeaderSize - 34), text);
                }
                else if (draggedBound == 1)
                {
                    var x = RightBound / Duration * Bounds.Width + 12;
                    var text = new FormattedText($"-{(Duration - RightBound).ToString(timeFormat)}", Typeface.Default, 12, TextAlignment.Left, TextWrapping.NoWrap, new Size());
                    if (x + 3 > Bounds.Width)
                        x = Bounds.Width - 3;
                    context.FillRectangle(BoundsBrush, new Rect(x - 6 - text.Bounds.Width, HeaderSize - 36, text.Bounds.Width + 6, 18));
                    context.DrawText(BackgroundBrush, new Point(x - 3 - text.Bounds.Width, HeaderSize - 34), text);
                }
                if ((RightBound - LeftBound) / Duration * Bounds.Width > 75)
                {
                    var text = new FormattedText($"{(RightBound - LeftBound).ToString(timeFormat)}", Typeface.Default, 12, TextAlignment.Left, TextWrapping.NoWrap, new Size());
                    var left = LeftBound / Duration * Bounds.Width + 3;
                    var right = RightBound / Duration * Bounds.Width - 3;
                    var center = (left + right) / 2;
                    left = (int)left + .5f;
                    right = (int)right + .5f;
                    center = (int)center + .5f;
                    context.FillRectangle(BoundsBrush, new Rect(center - text.Bounds.Width / 2 - 3, HeaderSize - 18, text.Bounds.Width + 6, 18));
                    context.DrawText(BackgroundBrush, new Point(center - text.Bounds.Width / 2, HeaderSize - 15), text);
                    context.DrawLine(boundsPen, new Point(left, HeaderSize - 9 - 6), new Point(left, HeaderSize - 9 + 6));
                    context.DrawLine(boundsPen, new Point(center - text.Bounds.Width / 2 - 6, HeaderSize - 9 - 6), new Point(center - text.Bounds.Width / 2 - 6, HeaderSize - 9 + 6));
                    context.DrawLine(boundsPen, new Point(right, HeaderSize - 9 - 6), new Point(right, HeaderSize - 9 + 6));
                    context.DrawLine(boundsPen, new Point(center + text.Bounds.Width / 2 + 6, HeaderSize - 9 - 6), new Point(center + text.Bounds.Width / 2 + 6, HeaderSize - 9 + 6));
                    context.DrawLine(boundsPen, new Point(left, HeaderSize - 9.5f), new Point(center - text.Bounds.Width / 2 - 6, HeaderSize - 9.5f));
                    context.DrawLine(boundsPen, new Point(right, HeaderSize - 9.5f), new Point(center + text.Bounds.Width / 2 + 6, HeaderSize - 9.5f));
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(availableSize.Width, HeaderSize);
        }

        protected override void OnPointerLeave(PointerEventArgs e)
        {
            base.OnPointerLeave(e);
            if (IsDragging)
                IsDragging = false;
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
            if (IsDragging)
            {
                var perc = e.GetCurrentPoint(this).Position.X / Bounds.Width;
                if (perc < 0)
                    perc = 0;
                if (perc > 1)
                    perc = 1;
                var time = perc * Duration;
                if (draggedBound == 0)
                {
                    if (time > RightBound)
                    {
                        draggedBound = 1;
                        LeftBound = RightBound;
                        RightBound = time;
                    }
                    else
                        LeftBound = time;
                }
                else if (draggedBound == 1)
                {
                    if (time < LeftBound)
                    {
                        draggedBound = 0;
                        RightBound = LeftBound;
                        LeftBound = time;
                    }
                    else
                        RightBound = time;
                }
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
            {
                IsDragging = true;
                var perc = e.GetCurrentPoint(this).Position.X / Bounds.Width;
                var time = perc * Duration;
                var average = (LeftBound + RightBound) / 2;
                if (time < average)
                    draggedBound = 0;
                else if (time > average)
                    draggedBound = 1;
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
                IsDragging = false;
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