using System;
using System.IO;
using System.Text;

namespace IQArchiveManager.Common.IO.IqaReader
{
    public class IqaSegmentReader : Stream
    {
        public IqaSegmentReader(Stream baseStream, long startPos)
        {
            this.baseStream = baseStream;
            payloadStartPos = startPos + 16;
            baseStream.Position = startPos;
            ReadSegmentInfo(baseStream, out tag, out len, out crc);
        }

        public string ReadAsText()
        {
            string v;
            using (StreamReader sr = new StreamReader(this))
                v = sr.ReadToEnd();
            return v;
        }

        public static bool ReadSegmentInfo(Stream stream, out string tag, out long len, out uint crc)
        {
            byte[] buffer = new byte[16];
            int read = stream.Read(buffer, 0, 16);
            tag = Encoding.ASCII.GetString(buffer, 0, 4);
            len = BitConverter.ToInt64(buffer, 4);
            crc = BitConverter.ToUInt32(buffer, 12);
            return read == 16;
        }

        private Stream baseStream;
        private long payloadStartPos;
        private long len;
        private uint crc;
        private string tag;

        public string Tag { get => tag; }

        public uint Crc { get => crc; }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => len;

        public override long Position { get => baseStream.Position - payloadStartPos; set => baseStream.Position = value + payloadStartPos; }

        public long Remaining { get => Length - Position; }

        public override void Flush()
        {

        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return baseStream.Read(buffer, offset, (int)Math.Min(count, Remaining));
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}