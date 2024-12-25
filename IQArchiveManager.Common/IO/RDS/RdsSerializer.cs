using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IQArchiveManager.Common.IO.RDS
{
    public class RdsSerializer : IDisposable
    {
        public RdsSerializer()
        {
            //Write header
            stream.WriteByte(1); // Version major
            stream.WriteByte(1); // Version minor
            stream.WriteByte(0); // Reserved
            stream.WriteByte(0); // Reserved
        }

        private readonly MemoryStream stream = new MemoryStream();
        private readonly byte[] buffer = new byte[8192];

        public void WriteChunk(uint timestamp, byte[] bits, int bitCount)
        {
            //Make sure at least one bit is supplied
            if (bitCount <= 0)
                return;

            //Calculate number of bytes these will fill
            int blockSize = (bitCount + 7) / 8;

            //Check to make sure it'll fit
            if (blockSize >= buffer.Length || bitCount > ushort.MaxValue)
                throw new Exception($"Block of RDS bits is too large.");

            //Clear out buffer
            for (int i = 0; i < blockSize; i++)
                buffer[i] = 0;

            //Pack bits into bytes
            int posBit = 0;
            int posByte = 0;
            for (int i = 0; i < bitCount; i++)
            {
                //Write
                buffer[posByte] |= (byte)((bits[i] & 1) << posBit);

                //Update counter
                posBit++;
                if (posBit == 8)
                {
                    posBit = 0;
                    posByte++;
                }
            }

            //Write timestamp + size + payload to file
            stream.Write(BitConverter.GetBytes(timestamp), 0, 4);
            stream.Write(BitConverter.GetBytes((ushort)bitCount), 0, 2);
            stream.Write(buffer, 0, blockSize);
        }

        public byte[] Serialize()
        {
            return stream.ToArray();
        }

        public void Dispose()
        {
            stream.Dispose();
        }
    }
}
