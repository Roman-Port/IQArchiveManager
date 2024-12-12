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

        public void Refresh(ConcurrentQueue<IArchiveTask> queue)
        {
            RefreshDirectory(queue, rootDir);
        }

        private void RefreshDirectory(ConcurrentQueue<IArchiveTask> queue, string directory)
        {
            //Query
            string[] files = Directory.GetFiles(directory);
            string[] dirs = Directory.GetDirectories(directory);

            //Loop dirs
            foreach (var d in dirs)
                RefreshDirectory(queue, d);

            //Loop files
            foreach (var f in files)
            {
                //Make sure it doesn't already exist
                if (queuedItems.Contains(f))
                    continue;
                
                //Attempt
                IArchiveTask task = ProcessFile(f);

                //Add if we can
                if (task != null)
                {
                    queuedItems.Add(f);
                    queue.Enqueue(task);
                }
            }
        }

        protected abstract IArchiveTask ProcessFile(string path);
    }
}
