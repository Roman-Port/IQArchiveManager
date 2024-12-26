using IQArchiveManager.Common.IO.RDS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Common.IO.RDS
{
    public interface IRdsDsp
    {
        List<RdsPacket> Load(Stream stream);
    }
}
