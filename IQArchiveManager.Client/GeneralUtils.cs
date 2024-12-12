using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client
{
    public class GeneralUtils
    {
        public static float ConvertAudioSample(byte sample)
        {
            return (sample - 127.5f) / 127.5f;
        }
    }
}
