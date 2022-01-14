using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    internal class OffsetWaveStream : WaveStream
    {
        private long duration;
        private object mutex;
        private long offset;

        public OffsetWaveStream(WaveStream source)
        {
            Source = source;
            mutex = new object();
        }

        public TimeSpan Duration
        {
            get
            {
                lock (mutex)
                {
                    return TimeSpan.FromSeconds(duration / (double)WaveFormat.AverageBytesPerSecond);
                }
            }
            set
            {
                lock (mutex)
                {
                    duration = TimeAsBytes(value);
                    if (duration < Source.Position - offset)
                        Source.Position = duration + offset;
                }
            }
        }

        public override long Length
        {
            get
            {
                lock (mutex)
                {
                    return duration;
                }
            }
        }

        public TimeSpan Offset
        {
            get
            {
                lock (mutex)
                {
                    return TimeSpan.FromSeconds(offset / (double)WaveFormat.AverageBytesPerSecond);
                }
            }
            set
            {
                lock (mutex)
                {
                    offset = TimeAsBytes(value);
                    if (offset > Source.Position)
                        Source.Position = offset;
                }
            }
        }

        public override long Position
        {
            get
            {
                lock (mutex)
                {
                    return Source.Position - offset;
                }
            }
            set
            {
                lock (mutex)
                {
                    Source.Position = value + offset;
                }
            }
        }

        public override WaveFormat WaveFormat => Source.WaveFormat;
        private WaveStream Source { get; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (mutex)
            {
                if (duration > 0)
                    count = (int)Math.Max(0, Math.Min(count, duration - Position));
                return Source.Read(buffer, offset, count);
            }
        }

        public override void SetLength(long length)
        {
            lock (mutex)
            {
                duration = length;
            }
        }

        private long TimeAsBytes(TimeSpan time)
        {
            var result = (long)Math.Round(time.TotalSeconds * WaveFormat.AverageBytesPerSecond);
            return result - (result % (WaveFormat.BitsPerSample * WaveFormat.Channels / 8));
        }
    }
}