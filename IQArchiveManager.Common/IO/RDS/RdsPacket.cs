using System;
using System.Collections.Generic;
using System.Text;

namespace IQArchiveManager.Common.IO.RDS
{
    public struct RdsPacket
    {
        public long timestamp;
        public RdsFlags flags;
        public ushort a;
        public ushort b;
        public ushort c;
        public ushort d;
    }
}
