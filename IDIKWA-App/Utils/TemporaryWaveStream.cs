using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class TemporaryWaveStream : WaveStream, IWaveProvider
    {
        public TemporaryWaveStream(IWaveProvider provider, TimeSpan duration)
        {
            Mutex = new SemaphoreSlim(1);
            Source = provider;
            Stream = new TemporaryStream((int)(provider.WaveFormat.AverageBytesPerSecond * duration.TotalSeconds));
        }

        public override long Length => Stream.Length;

        public override long Position
        {
            get
            {
                long result;
                Mutex.Wait();
                try
                {
                    result = Stream.Position;
                }
                finally
                {
                    Mutex.Release();
                }
                return result;
            }
            set
            {
                Mutex.Wait();
                try
                {
                    Stream.Position = value;
                }
                finally
                {
                    Mutex.Release();
                }
            }
        }

        public override WaveFormat WaveFormat => Source.WaveFormat;
        private SemaphoreSlim Mutex { get; }
        private IWaveProvider Source { get; }
        private Stream Stream { get; }

        public async Task ListenAsync()
        {
            var bytes = new byte[1024 * 16];
            while (true)
            {
                int bytesRead = await Task.Run(() => Source.Read(bytes, 0, bytes.Length));
                if (bytesRead == 0)
                    break;
                await Mutex.WaitAsync();
                try
                {
                    await Stream.WriteAsync(bytes, 0, bytesRead);
                }
                finally
                {
                    Mutex.Release();
                }
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int readBytes;
            Mutex.Wait();
            try
            {
                readBytes = Stream.Read(buffer, offset, count);
            }
            finally
            {
                Mutex.Release();
            }

            return readBytes;
        }
    }
}