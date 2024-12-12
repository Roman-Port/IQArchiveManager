using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IQArchiveManager.Common.IO.IqaWriter
{
    public class IqaFlacWriter : Stream
    {
        public IqaFlacWriter(Stream dst)
        {
            this.dst = dst;
            flac = Process.Start(new ProcessStartInfo
            {
                FileName = "flac.exe",
                Arguments = "- --endian little --sign signed --channels 2 --bps 16 --sample-rate 65535 --totally-silent -8",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            });
            writeTask = new Thread(Worker);
            writeTask.Start();
        }

        private Stream dst;
        private Process flac;
        private Thread writeTask;
        private volatile bool done = false;

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override void Flush()
        {

        }

        private void Worker()
        {
            byte[] transferBuffer = new byte[65536];
            int read;
            do
            {
                read = flac.StandardOutput.BaseStream.Read(transferBuffer, 0, transferBuffer.Length);
                dst.Write(transferBuffer, 0, read);
            } while (read != 0);
            done = true;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
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
            flac.StandardInput.BaseStream.Write(buffer, offset, count);
        }

        public override void Close()
        {
            flac.StandardInput.BaseStream.Close();
            while (!done)
                Thread.Sleep(100);
        }
    }
}
