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
    /// <summary>
    /// Temporary wave stream that override old audio data when the internal buffer is full
    /// </summary>
    public class TemporaryWaveStream : WaveStream, IWaveProvider
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="provider">Source of audio data to copy</param>
        /// <param name="duration">Max duration of the internal buffer</param>
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

        /// <summary>
        /// Start listening to the audio source and copy the data to the internal buffer.
        /// </summary>
        /// <returns>The task handling the listening that finishes when the audio source is finished copying</returns>
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

        public override void SetLength(long length)
        {
            Stream.SetLength(length);
        }
    }
}