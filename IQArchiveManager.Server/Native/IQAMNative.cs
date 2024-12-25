using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IQArchiveManager.Server.Native
{
    static unsafe class IQAMNative
    {
        public const string LIB_NAME = "libiqam_native";

        [DllImport(LIB_NAME, EntryPoint = "iqam_version")]
        public static extern void GetVersion(int* major, int* minor);
    }
}
