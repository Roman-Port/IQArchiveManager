using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS
{
    public interface IRdsPatchContext
    {
        DateTime GetTimeOfFrameStart<T>(RdsValue<T> value);
        DateTime GetTimeOfFrameEnd<T>(RdsValue<T> value);
        long GetSampleFromTime(DateTime time);
    }
}
