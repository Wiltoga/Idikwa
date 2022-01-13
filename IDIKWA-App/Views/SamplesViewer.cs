using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class SamplesViewer : Control, IStyleable
    {
        public static readonly StyledProperty<float[]?> AverageSamplesProperty = AvaloniaProperty.Register<SamplesViewer, float[]?>(nameof(AverageSamples), null);
        public static readonly StyledProperty<IBrush> BrushProperty = AvaloniaProperty.Register<SamplesViewer, IBrush>(nameof(Brush), new SolidColorBrush(Colors.White));
        public static readonly StyledProperty<float> ScaleProperty = AvaloniaProperty.Register<SamplesViewer, float>(nameof(Scale), 1f);

        static SamplesViewer()
        {
            AverageSamplesProperty.Changed.AddClassHandler<SamplesViewer>(RenderPropertyChanged);
            ScaleProperty.Changed.AddClassHandler<SamplesViewer>(RenderPropertyChanged);
            BrushProperty.Changed.AddClassHandler<SamplesViewer>(RenderPropertyChanged);
        }

        public float[]? AverageSamples { get => GetValue(AverageSamplesProperty); set => SetValue(AverageSamplesProperty, value); }
        public IBrush Brush { get => GetValue(BrushProperty); set => SetValue(BrushProperty, value); }
        public float Scale { get => GetValue(ScaleProperty); set => SetValue(ScaleProperty, value); }

        Type IStyleable.StyleKey => typeof(SamplesViewer);

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            if (AverageSamples is not null)
            {
                var geometry = new PathGeometry();
                var figure = new PathFigure()
                {
                    StartPoint = new Point(0, (int)(Bounds.Height / 2) + .5f)
                };
                figure.Segments = new PathSegments();
                for (int i = 0; i < AverageSamples.Length; ++i)
                {
                    figure.Segments.Add(new LineSegment()
                    {
                        Point = new Point(i / (AverageSamples.Length - 1f) * Bounds.Width, Bounds.Height * (.5f - AverageSamples[i] * Scale / 2))
                    });
                }
                figure.Segments.Add(new LineSegment()
                {
                    Point = new Point(Bounds.Width, (int)(Bounds.Height / 2) + .5f)
                });
                geometry.Figures.Add(figure);
                context.DrawGeometry(Brush, new Pen(), geometry);
                figure.Segments = new PathSegments();
                for (int i = 0; i < AverageSamples.Length; ++i)
                {
                    figure.Segments.Add(new LineSegment()
                    {
                        Point = new Point(i / (AverageSamples.Length - 1f) * Bounds.Width, Bounds.Height * (.5f + AverageSamples[i] * Scale / 2))
                    });
                }
                figure.Segments.Add(new LineSegment()
                {
                    Point = new Point(Bounds.Width, (int)(Bounds.Height / 2) + .5f)
                });
                context.DrawGeometry(Brush, new Pen(), geometry);
                figure.Segments = new PathSegments();
                figure.Segments.Add(new LineSegment()
                {
                    Point = new Point(Bounds.Width, (int)(Bounds.Height / 2) + .5f)
                });
                context.DrawGeometry(Brush, new Pen(Brush), geometry);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(40, 20);
        }

        private static void RenderPropertyChanged(SamplesViewer sender, AvaloniaPropertyChangedEventArgs e)
        {
            sender.InvalidateVisual();
        }
    }
}