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
            if (buffer[0] != 0)
                throw new Exception("Unsupported RDS version.");
        }

        private readonly Stream stream;
        private readonly byte[] buffer = new byte[8];

        private bool inCombinedPacket = false; // True if the current packet is part of a larger one; Don't read the timestamp
        private uint lastTimestamp = 0;

        public RdsPacket? ReadPacket()
        {
            //Read the timestamp or load depending on state
            uint timestamp = lastTimestamp;
            if (!inCombinedPacket)
            {
                //Read timestamp from file
                if (stream.Read(buffer, 0, 4) != 4)
                    return null; // End of stream
                timestamp = BitConverter.ToUInt32(buffer, 0);
            }

            //Read flags
            RdsFlags flags = (RdsFlags)stream.ReadByte();

            //Read frame
            if (stream.Read(buffer, 0, 8) != 8)
                return null; // End of stream
            ulong frame = BitConverter.ToUInt64(buffer, 0);

            //Set up state machine for next packet
            inCombinedPacket = (flags & RdsFlags.END_OF_CHUNK) != RdsFlags.END_OF_CHUNK;
            lastTimestamp = timestamp;

            //Wrap into frame
            return new RdsPacket
            {
                timestamp = timestamp,
                flags = flags,
                frame = frame
            };
        }
    }
}
