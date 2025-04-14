using System;
using System.Collections.Generic;
using System.Text;

namespace IQArchiveManager.Server
{
    /// <summary>
    /// Base class for a task that will run on a worker thread.
    /// </summary>
    public abstract class ArchiveTask
    {
        public ArchiveTask(string typeDescription, string inputFilename)
        {
            this.inputFilename = inputFilename;
            this.typeDescription = typeDescription;
        }

        private readonly string inputFilename;
        private readonly string typeDescription;
        private readonly object mutex = new object();
        private long progressValue;
        private long progressMax;
        private string statusText;

        /// <summary>
        /// The constant name describing this task.
        /// </summary>
        public string TypeDescription => typeDescription;

        /// <summary>
        /// The filename associated with this task.
        /// </summary>
        public string InputFilename => inputFilename;

        /// <summary>
        /// This is the progress amount that's updated - Should be between 0 and ProgressMax.
        /// </summary>
        public long ProgressValue
        {
            get
            {
                lock (mutex)
                    return progressValue;
            }
            protected set
            {
                lock (mutex)
                    progressValue = value;
            }
        }

        /// <summary>
        /// The max the ProgressValue is compared to for a progress percentage.
        /// </summary>
        public long ProgressMax
        {
            get
            {
                lock (mutex)
                    return progressMax;
            }
            protected set
            {
                lock (mutex)
                    progressMax = value;
            }
        }

        /// <summary>
        /// Gets the percentage of completion between 0-1. If ProgressMax is 0, returns 0.
        /// </summary>
        public double ProgressPercent
        {
            get
            {
                lock (mutex)
                {
                    if (progressMax == 0)
                        return progressMax;
                    return (double)progressValue / progressMax;
                }
            }
        }

        /// <summary>
        /// Status text displayed to the user.
        /// </summary>
        public string StatusText
        {
            get
            {
                lock (mutex)
                    return statusText;
            }
            protected set
            {
                lock (mutex)
                    statusText = value;
            }
        }

        /// <summary>
        /// Does processing.
        /// </summary>
        public abstract void Process();
    }
}
