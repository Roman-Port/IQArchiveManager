﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS.Parser
{
    public interface IRdsPatchContext
    {
        long FileLengthSamples { get; }

        DateTime GetTimeOfFrameStart<T>(RdsValue<T> value);
        DateTime GetTimeOfFrameEnd<T>(RdsValue<T> value);
        long GetSampleFromTime(DateTime time);
    }
}
