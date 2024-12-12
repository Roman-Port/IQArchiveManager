using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.Pre
{
    public delegate void PreProcessorFileStreamReader_Event(PreProcessorFileStreamReader reader);
    
    public class PreProcessorFileStreamReader
    {
        public PreProcessorFileStreamReader(FileStream fs, string tag, long totalLen, long segmentTablePos)
        {
            this.fs = fs;
            this.tag = tag;
            this.totalLen = totalLen;
            this.segmentTablePos = segmentTablePos;
        }

        private FileStream fs;
        private string tag;
        private long totalLen;
        private long segmentTablePos;

        private long segmentTableLen;
        private long[] segmentTableVirtualPos;
        private long[] segmentTableOffsets;
        private int[] segmentTableLengths;

        private int currentSegment;
        private int currentSegmentPosition;

        public event PreProcessorFileStreamReader_Event OnSegmentChanged;

        private const int OFFSET_TABLE_ENTRY_LEN = 8 + 4;

        public int SegmentPosition { get => currentSegmentPosition; set => currentSegmentPosition = value; }
        public int SegmentRemaining { get => segmentTableLengths[currentSegment] - currentSegmentPosition; }
        public int SegmentCount { get => (int)segmentTableLen; }
        public int CurrentSegment
        {
            get => currentSegment;
            set
            {
                currentSegment = value;
                currentSegmentPosition = 0;
                OnSegmentChanged?.Invoke(this);
            }
        }
        public long Position
        {
            get => currentSegment >= segmentTableLen ? Length : segmentTableVirtualPos[currentSegment] + currentSegmentPosition;
            set
            {
                value = Math.Min(value, Length - 1);
                for (int i = 0; i < segmentTableLen + 1; i++)
                {
                    if (segmentTableVirtualPos[i] + segmentTableLengths[i] > value)
                    {
                        currentSegment = i;
                        currentSegmentPosition = (int)(value - segmentTableVirtualPos[i]);
                        OnSegmentChanged?.Invoke(this);
                        return;
                    }
                }
                throw new Exception("Position is invalid.");
            }
        }
        public long Length { get => totalLen; }

        public void Open()
        {
            //Go to segment table
            fs.Position = segmentTablePos;

            //Read the number of items in the offset table
            byte[] offsetTableBuffer = new byte[8];
            fs.Read(offsetTableBuffer, 0, 8);
            segmentTableLen = BitConverter.ToInt64(offsetTableBuffer, 0);

            //Read the entire offset table
            offsetTableBuffer = new byte[OFFSET_TABLE_ENTRY_LEN * segmentTableLen];
            fs.Read(offsetTableBuffer, 0, offsetTableBuffer.Length);

            //Convert
            long virtualPos = 0;
            segmentTableVirtualPos = new long[segmentTableLen + 1];
            segmentTableOffsets = new long[segmentTableLen + 1];
            segmentTableLengths = new int[segmentTableLen + 1];
            for (int i = 0; i < segmentTableLen; i++)
            {
                segmentTableVirtualPos[i] = virtualPos;
                segmentTableOffsets[i] = BitConverter.ToInt64(offsetTableBuffer, (OFFSET_TABLE_ENTRY_LEN * i) + 0);
                segmentTableLengths[i] = BitConverter.ToInt32(offsetTableBuffer, (OFFSET_TABLE_ENTRY_LEN * i) + 8);
                virtualPos += segmentTableLengths[i];
            }

            //Add a dummy entry
            segmentTableVirtualPos[segmentTableLen] = Length;
            segmentTableOffsets[segmentTableLen] = 0;
            segmentTableLengths[segmentTableLen] = 0;

            //Sanity check
            if (virtualPos != totalLen)
                throw new Exception($"Expected final virtualPos ({virtualPos}) to match the totalLen ({totalLen})!");
        }

        public int Read(byte[] buffer, int offset, int length, bool spanSegments = true)
        {
            int read = 0;
            while(length > 0 && currentSegment < segmentTableLen && fs.CanRead)
            {
                //If the current segment is empty, skip
                if (SegmentRemaining == 0)
                {
                    CurrentSegment++;
                    continue;
                }
                
                //Get how far we have to read to hit the end of this segment OR run out of length in the buffer
                int readable = Math.Min(SegmentRemaining, length);

                //Seek to this
                fs.Position = segmentTableOffsets[currentSegment] + currentSegmentPosition;

                //Read
                fs.Read(buffer, offset, length);

                //Update
                currentSegmentPosition += readable;
                read += readable;
                offset += readable;
                length -= readable;

                //Check if we've reached the end of the segment
                if (SegmentRemaining == 0 && !spanSegments)
                    break;
                if (SegmentRemaining == 0 && spanSegments)
                    CurrentSegment++;
            }
            return read;
        }
    }
}
