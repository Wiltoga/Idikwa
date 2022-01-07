using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    internal class TemporaryStream : Stream
    {
        private Memory<byte> cache;

        private int length;
        private int offset;

        public TemporaryStream(int cacheSize) : this() => cache = new byte[cacheSize];

        public TemporaryStream(Memory<byte> cache) : this() => this.cache = cache;

        private TemporaryStream()
        {
            offset = 0;
            length = 0;
            Position = 0;
            cache = Memory<byte>.Empty;
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => length;

        public override long Position { get; set; }

        private int InternalPos => (offset + (int)Position) % cache.Length;

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;

                case SeekOrigin.Current:
                    Position += offset;
                    break;

                case SeekOrigin.End:
                    Position = length + offset;
                    break;
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            length = (int)value;
            Position = Math.Min(Position, length);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var memory = new ReadOnlySpan<byte>(buffer, offset, count);
            for (int i = 0; i < count; i += cache.Length)
            {
                WriteToCache(memory.Slice(i, Math.Min(cache.Length, memory.Length - i)));
            }
        }

        private void WriteToCache(ReadOnlySpan<byte> data)
        {
            var cacheSpan = cache.Span;
            var sampleSize = Math.Min(cache.Length - InternalPos, data.Length);
            if (sampleSize < data.Length)
            {
                data.Slice(0, sampleSize).CopyTo(cacheSpan.Slice(InternalPos));
                data.Slice(sampleSize).CopyTo(cacheSpan);
            }
            else
            {
                data.CopyTo(cacheSpan.Slice(InternalPos));
            }
            Position += data.Length;
            var overflow = (int)Position - cache.Length;
            if (overflow > 0)
            {
                offset += overflow;
                Position -= overflow;
            }
            length = Math.Max(length, (int)Position);
        }
    }
}