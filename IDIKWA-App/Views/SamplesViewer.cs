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
        public static readonly StyledProperty<IBrush> BrushProperty = AvaloniaProperty.Register<SamplesViewer, IBrush>(nameof(Brush), new SolidColorBrush(Colors.White));
        public static readonly StyledProperty<ISampleProvider?> SampleProperty = AvaloniaProperty.Register<SamplesViewer, ISampleProvider?>(nameof(Sample), null);
        public static readonly StyledProperty<float> ScaleProperty = AvaloniaProperty.Register<SamplesViewer, float>(nameof(Scale), 1f);
        private static readonly int TotalSampleReduction = 2000;

        static SamplesViewer()
        {
            SampleProperty.Changed.AddClassHandler<SamplesViewer>(SamplePropertyChanged);
            ScaleProperty.Changed.AddClassHandler<SamplesViewer>(RenderPropertyChanged);
            BrushProperty.Changed.AddClassHandler<SamplesViewer>(RenderPropertyChanged);
        }

        public SamplesViewer()
        {
            AverageSamples = null;
        }

        public IBrush Brush { get => GetValue(BrushProperty); set => SetValue(BrushProperty, value); }
        public ISampleProvider? Sample { get => GetValue(SampleProperty); set => SetValue(SampleProperty, value); }
        public float Scale { get => GetValue(ScaleProperty); set => SetValue(ScaleProperty, value); }

        Type IStyleable.StyleKey => typeof(SamplesViewer);

        private float[]? AverageSamples { get; set; }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            if (AverageSamples is not null)
            {
                var pen = new Pen();
                var geometry = new PathGeometry();
                var figure = new PathFigure()
                {
                    StartPoint = new Point(0, Bounds.Height / 2)
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
                    Point = new Point(Bounds.Width, Bounds.Height / 2)
                });
                geometry.Figures.Add(figure);
                context.DrawGeometry(Brush, pen, geometry);
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
                    Point = new Point(Bounds.Width, Bounds.Height / 2)
                });
                context.DrawGeometry(Brush, pen, geometry);
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

        private static void SamplePropertyChanged(SamplesViewer sender, AvaloniaPropertyChangedEventArgs e)
        {
            sender.ProcessSample();
            sender.InvalidateVisual();
        }

        private void ProcessSample()
        {
            if (Sample is null)
                AverageSamples = null;
            else
            {
                AverageSamples = new float[TotalSampleReduction];
                var tmpList = new List<float>(100000 * 60);
                var samplesBuffer = new float[10000 * Sample.WaveFormat.Channels];
                int samplesRead;
                while ((samplesRead = Sample.Read(samplesBuffer, 0, samplesBuffer.Length)) > 0)
                    for (int i = 0; i < samplesRead; ++i)
                        tmpList.Add(Math.Abs(samplesBuffer[i]) * 2);
                for (int i = 0; i < AverageSamples.Length; ++i)
                {
                    var samples = tmpList.Count / AverageSamples.Length;
                    var offset = samples * i;
                    samples = Math.Min(samples, tmpList.Count - offset);
                    if (samples < 0)
                        Console.WriteLine($"{tmpList.Count / AverageSamples.Length}     {offset}");
                    AverageSamples[i] = tmpList.GetRange(offset, samples).Average();
                }
            }
        }
    }
}