using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class PipeStream : Stream
    {
        private bool closed;
        private object mutex;
        private Queue<byte> storage;

        public PipeStream()
        {
            storage = new Queue<byte>();
            closed = false;
            mutex = new object();
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override void Close()
        {
            base.Close();
            lock (mutex)
            {
                closed = true;
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var readBytes = 0;
            while (count > 0)
            {
                var waitForData = true;
                lock (mutex)
                {
                    if (storage.Count > 0)
                    {
                        waitForData = false;
                        while (count > 0 && storage.Count > 0)
                        {
                            buffer[offset++] = storage.Dequeue();
                            --count;
                            ++readBytes;
                        }
                    }
                    else if (closed)
                        break;
                }
                if (waitForData)
                    Thread.Sleep(50);
            }
            return readBytes;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (mutex)
            {
                if (!closed)
                    while (offset < count)
                    {
                        storage.Enqueue(buffer[offset++]);
                    }
            }
        }
    }
}