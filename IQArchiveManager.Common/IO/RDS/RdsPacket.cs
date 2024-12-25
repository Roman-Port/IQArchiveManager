using System;
using System.Collections.Generic;
using System.Text;

namespace IQArchiveManager.Common.IO.RDS
{
    public struct RdsPacket
    {
        public uint timestamp;
        public RdsFlags flags;
        public ulong frame;
    }
}
