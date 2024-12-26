using IQArchiveManager.Client.Pre;
using IQArchiveManager.Common.IO.RDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS
{
    /// <summary>
    /// Dummy DSP that just loads from the file
    /// </summary>
    class InFileRdsDsp : IRdsDsp
    {
        public List<RdsPacket> Load(PreProcessorFileStreamReader stream)
        {
            throw new NotImplementedException();
        }
    }
}
