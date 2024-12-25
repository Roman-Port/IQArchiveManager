using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IQArchiveManager.Common.IO.RDS
{
    public class RdsSerializer
    {
        public RdsSerializer()
        {

        }

        private readonly List<RdsPacket> packets = new List<RdsPacket>();

        public void Write(RdsPacket packet)
        {
            packets.Add(packet);
        }

        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //Write header
                ms.WriteByte(0); // Version major
                ms.WriteByte(1); // Version minor
                ms.WriteByte(0); // Reserved
                ms.WriteByte(0); // Reserved

                //Encode packets
                foreach (var p in packets)
                {
                    //Get flags for modification
                    RdsFlags flags = p.flags;

                    //Spanning packets isn't implemented yet, so make sure all packets have the end of chunk flag set
                    flags |= RdsFlags.END_OF_CHUNK;

                    //Write
                    ms.Write(BitConverter.GetBytes(p.timestamp), 0, 4);
                    ms.WriteByte((byte)flags);
                    ms.Write(BitConverter.GetBytes(p.frame), 0, 8);
                }

                return ms.ToArray();
            }
        }
    }
}
