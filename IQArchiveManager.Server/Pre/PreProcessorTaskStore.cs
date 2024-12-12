using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IQArchiveManager.Server.Pre
{
    public class PreProcessorTaskStore : BaseArchiveTaskScanStore
    {
        public PreProcessorTaskStore(string rootDir) : base(rootDir + Path.DirectorySeparatorChar + "input")
        {
        }

        protected override IArchiveTask ProcessFile(string f)
        {
            //Validate
            if (!f.EndsWith(".wav"))
                return null;

            //Make sure it's not already been processed
            if (File.Exists(f + ".iqpre") || File.Exists(f + ".iqpost") || File.Exists(f + ".iqedit"))
                return null;

            //Queue
            return new PreProcessorTask(f);
        }
    }
}
