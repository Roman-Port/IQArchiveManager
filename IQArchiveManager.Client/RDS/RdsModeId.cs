using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS
{
    public enum RdsModeId
    {
        NATIVE = 0,
        TWO_CHAR = 1,
        NO_DELIMITERS = 2,
        KZCR = 3,
        KZCR_LEGACY = 4,
        CSV = 5
    }
}
