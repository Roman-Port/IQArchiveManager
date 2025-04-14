using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IQArchiveManager.Server
{
    public abstract class BaseArchiveTaskScanStore : IArchiveTaskStore
    {
        public BaseArchiveTaskScanStore(string rootDir)
        {
            this.rootDir = rootDir;
        }

        private string rootDir;
        private List<string> queuedItems = new List<string>();

        public void Refresh(Queue<ArchiveTask> queue)
        {
            //Query
            string[] files = Directory.GetFiles(rootDir);

            //Loop files
            foreach (var f in files)
            {
                //Make sure it doesn't already exist
                if (queuedItems.Contains(f))
                    continue;

                //Attempt
                ArchiveTask task = ProcessFile(f);

                //Add if we can
                if (task != null)
                {
                    queuedItems.Add(f);
                    queue.Enqueue(task);
                }
            }
        }

        protected abstract ArchiveTask ProcessFile(string path);
    }
}
