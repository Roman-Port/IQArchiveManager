using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.Util
{
    static class StorageFormattingUtils
    {
        private static readonly string[] UNITS = new string[]
        {
            "KB",
            "MB",
            "GB",
            "TB"
        };

        public static string FormatStorage(long bytes, int decimals)
        {
            //Determine unit
            int unit = 0;
            while (bytes > 1000 && (unit + 1) < UNITS.Length)
            {
                bytes /= 1000;
                unit++;
            }

            return (bytes / 1000) + " " + UNITS[unit];
        }
    }
}
