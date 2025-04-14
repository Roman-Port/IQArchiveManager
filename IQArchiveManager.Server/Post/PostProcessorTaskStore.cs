using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IQArchiveManager.Server.Post
{
    public class PostProcessorTaskStore : BaseArchiveTaskScanStore
    {
        public PostProcessorTaskStore(string rootDir) : base(rootDir)
        {
            outputDir = rootDir + Path.DirectorySeparatorChar + "output";
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
            finishedDir = rootDir + Path.DirectorySeparatorChar + "edited";
            if (!Directory.Exists(finishedDir))
                Directory.CreateDirectory(finishedDir);
        }

        private string outputDir;
        private string finishedDir;

        protected override ArchiveTask ProcessFile(string f)
        {
            //Validate
            if (!f.EndsWith(".wav"))
                return null;

            //Make sure it has an output file
            if (!File.Exists(f + ".iqedit"))
                return null;

            //Queue
            return new PostProcessorTask(f, outputDir, finishedDir);
        }
    }
}
