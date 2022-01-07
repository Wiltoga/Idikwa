using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class TemporaryWaveProvider : IWaveProvider
    {
        public TemporaryWaveProvider(IWaveProvider provider, TimeSpan duration)
        {
            Source = provider;
            Stream = new TemporaryStream((int)(provider.WaveFormat.AverageBytesPerSecond * duration.TotalSeconds));
        }

        public WaveFormat WaveFormat => Source.WaveFormat;
        private IWaveProvider Source { get; }
        private TemporaryStream Stream { get; }

        public void InitRead()
        {
            Stream.Seek(0, System.IO.SeekOrigin.Begin);
        }

        public async Task ListenAsync()
        {
            var bytes = new byte[1024 * 16];
            while (true)
            {
                int bytesRead = await Task.Run(() => Source.Read(bytes, 0, bytes.Length));
                if (bytesRead == 0)
                    break;
                await Stream.WriteAsync(bytes, 0, bytesRead);
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return Stream.Read(buffer, offset, count);
        }
    }
}