using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IQArchiveManager.Server.Post
{
    public class PostProcessorTaskStore : BaseArchiveTaskScanStore
    {
        public PostProcessorTaskStore(string rootDir) : base(rootDir + Path.DirectorySeparatorChar + "input")
        {
            outputDir = rootDir + Path.DirectorySeparatorChar + "output";
            tempDir = rootDir + Path.DirectorySeparatorChar + "temp";
        }

        private string outputDir;
        private string tempDir;

        protected override IArchiveTask ProcessFile(string f)
        {
            //Validate
            if (!f.EndsWith(".wav"))
                return null;

            //Make sure it has an output file
            if (!File.Exists(f + ".iqedit"))
                return null;

            //Queue
            return new PostProcessorTask(f, outputDir, tempDir);
        }
    }
}
