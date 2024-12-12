using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IQArchiveManager.Common.IO.IqaReader
{
    public class IqaFlacReader : Stream
    {
        public IqaFlacReader(Stream source)
        {
            this.source = source;
            flac = Process.Start(new ProcessStartInfo
            {
                FileName = "flac.exe",
                Arguments = "-d - --endian little --sign signed --force-raw-format",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            });
            writeTask = new Thread(Worker);
            writeTask.Start();
        }

        private Stream source;
        private Process flac;
        private Thread writeTask;
        
        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

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
                read = source.Read(transferBuffer, 0, transferBuffer.Length);
                flac.StandardInput.BaseStream.Write(transferBuffer, 0, read);
            } while (read != 0);
            flac.StandardInput.BaseStream.Close();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return flac.StandardOutput.BaseStream.Read(buffer, offset, count);
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
            throw new NotSupportedException();
        }

        public override void Close()
        {
            
        }
    }
}
