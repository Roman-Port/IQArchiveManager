using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IQArchiveManager.Common.IO.RDS
{
    public class RdsDeserializer
    {
        public RdsDeserializer(Stream stream)
        {
            //Set
            this.stream = stream;

            //Read header
            if (stream.Read(buffer, 0, 4) != 4)
                throw new Exception("Failed to read RDS header.");

            //Check version
            if (buffer[0] != 1)
                throw new Exception("Unsupported RDS version.");
        }

        private readonly Stream stream;
        private readonly byte[] buffer = new byte[8192];

        private uint lastTimestamp = 0;
        private ushort bitsRemaining = 0; // Bits remaining in buffer
        private int posByte = 0;
        private int posBit = 0;

        public bool ReadBit(out uint timestamp, out byte bit)
        {
            //Check if we need to read a new chunk in
            if (bitsRemaining <= 0)
            {
                //Read header
                int headerLen = stream.Read(buffer, 0, 6);
                if (headerLen == 0)
                {
                    timestamp = 0;
                    bit = 0;
                    return false; // End of stream
                } else if (headerLen != 6)
                {
                    throw new Exception("RDS stream was desynced.");
                }

                //Extract info from header
                lastTimestamp = BitConverter.ToUInt32(buffer, 0);
                bitsRemaining = BitConverter.ToUInt16(buffer, 4);

                //Calculate number of bytes these will fill
                int blockSize = (bitsRemaining + 7) / 8;

                //Read chunk in
                if (stream.Read(buffer, 0, blockSize) != blockSize)
                    throw new Exception("RDS stream reached end in the middle of a block. Stream was desynced.");

                //Reset counters
                posBit = 0;
                posByte = 0;
            }

            //Decode bit
            bit = (byte)((buffer[posByte] >> posBit) & 1);

            //Update state
            bitsRemaining--;
            posBit++;
            if (posBit == 8)
            {
                posBit = 0;
                posByte++;
            }

            //Set timestamp
            timestamp = lastTimestamp;

            return true;
        }
    }
}
