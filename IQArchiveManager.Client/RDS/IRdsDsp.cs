using IQArchiveManager.Client.Pre;
using IQArchiveManager.Common.IO.RDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS
{
    internal interface IRdsDsp
    {
        List<RdsPacket> Load(PreProcessorFileStreamReader stream);
    }
}
