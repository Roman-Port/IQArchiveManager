using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IQArchiveManager.Server.Pre
{
    public class PreProcessorFileStreamWriter : IDisposable
    {
        public PreProcessorFileStreamWriter(FileStream stream, string tag)
        {
            this.stream = stream;
            tag.ToCharArray().CopyTo(this.tag, 0);
        }

        private FileStream stream;
        private char[] tag = new char[16];

        private long headerPos = -1;
        private long totalLen = 0;
        private List<long> segmentTableOffset = new List<long>();
        private List<int> segmentTableLength = new List<int>();

        private byte[] segmentBuffer = new byte[2048];
        private int segmentBufferUsage = 0;

        private bool disposed = false;

        private const int FILE_HEADER_SIZE = 16 + 8 + 8;

        public void Start()
        {
            //Save current offset
            headerPos = stream.Position;

            //Skip the size of our header, we'll write it later
            for (int i = 0; i < FILE_HEADER_SIZE; i++)
                stream.WriteByte(0);
        }

        public void StartNewSegment()
        {
            //Flush current buffer
            segmentTableOffset.Add(stream.Position);
            stream.Write(segmentBuffer, 0, segmentBufferUsage);
            segmentTableLength.Add(segmentBufferUsage);

            //Reset
            segmentBufferUsage = 0;
        }

        public void Write(byte[] buffer, int offset, int length)
        {
            //Make sure we have space
            while(segmentBuffer.Length < segmentBufferUsage + length)
            {
                //Create new array
                byte[] newSegmentBuffer = new byte[segmentBuffer.Length * 2];

                //Copy
                Array.Copy(segmentBuffer, newSegmentBuffer, segmentBufferUsage);

                //Set
                segmentBuffer = newSegmentBuffer;
            }

            //Write
            Array.Copy(buffer, offset, segmentBuffer, segmentBufferUsage, length);

            //Update lengths
            totalLen += length;
            segmentBufferUsage += length;
        }

        public void End()
        {
            //End
            StartNewSegment();
            
            //We'll now write the segment table. First, write how many entries there are
            long segmentTablePos = stream.Position;
            stream.Write(BitConverter.GetBytes((long)segmentTableOffset.Count), 0, 8);

            //Write every item
            for (int i = 0; i < segmentTableOffset.Count; i++)
            {
                stream.Write(BitConverter.GetBytes(segmentTableOffset[i]), 0, 8);
                stream.Write(BitConverter.GetBytes(segmentTableLength[i]), 0, 4);
            }

            //Skip back to the header location
            stream.Position = headerPos;

            //Create the header
            byte[] header = new byte[FILE_HEADER_SIZE];
            for (int i = 0; i < 16; i++)
                header[i] = (byte)tag[i];
            BitConverter.GetBytes(totalLen).CopyTo(header, 16);
            BitConverter.GetBytes(segmentTablePos).CopyTo(header, 24);

            //Write header
            stream.Write(header, 0, FILE_HEADER_SIZE);

            //Go to end of file
            stream.Position = stream.Length;

            //Set flag
            disposed = true;
        }

        public void Dispose()
        {
            End();
        }
    }
}
