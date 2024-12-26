using IQArchiveManager.Common.IO.RDS.DSPs;
using System;
using System.Collections.Generic;
using System.Text;

namespace IQArchiveManager.Common.IO.RDS
{
    public static class RdsDspStore
    {
        public const string PRE_FILE_RAW_BITSTREAM = "RDS2"; // Tag of the raw RDS bits in the pre file. Called RDS2 for compatibility reasons

        public static string GetPreFileDspTag(RdsDspId id)
        {
            return $"RDS-DSP{(int)id}";
        }

        public static IRdsDsp CreateDsp(RdsDspId id)
        {
            switch (id)
            {
                case RdsDspId.Original: return new RdsDspOriginal();
                case RdsDspId.SdrPP: return new RdsDspSdrpp();
                default: throw new Exception("Invalid DSP ID.");
            }
        }
    }
}
