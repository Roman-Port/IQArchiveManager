using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace IQArchiveManager.Server
{
    /// <summary>
    /// A thread that will process data.
    /// </summary>
    class ArchiveWorkerThread : IDisposable
    {
        public delegate void ErrorEventArgs(ArchiveWorkerThread thread, ArchiveTask task, Exception exception);

        public ArchiveWorkerThread()
        {
            worker = new Thread(Work);
        }

        private readonly object mutex = new object();
        private readonly Thread worker;
        private bool stopping = false;
        private ArchiveTask currentTask;

        /// <summary>
        /// Event raised when a task has an error.
        /// </summary>
        public event ErrorEventArgs OnError;

        /// <summary>
        /// True if the thread is currently busy.
        /// </summary>
        public bool Busy
        {
            get
            {
                lock (mutex)
                    return currentTask != null;
            }
        }

        /// <summary>
        /// True if the worker has exited.
        /// </summary>
        public bool Finished
        {
            get => worker.ThreadState == ThreadState.Stopped;
        }

        /// <summary>
        /// Gets the console status text. Thread safe.
        /// </summary>
        public string StatusText
        {
            get
            {
                lock (mutex)
                {
                    //Check if idle
                    if (currentTask == null)
                        return "Idle";

                    //Get shorted filename
                    string filename = currentTask.InputFilename;
                    int lastDirIndex = filename.LastIndexOf(Path.DirectorySeparatorChar);
                    if (lastDirIndex != -1)
                        filename = filename.Substring(lastDirIndex + 1);

                    //Format text
                    string text = $"[{currentTask.TypeDescription}] {filename} -> ";
                    string statusText = currentTask.StatusText;
                    if (statusText != null && statusText.Length != 0)
                        text += statusText + " - ";
                    text += $"{(currentTask.ProgressPercent * 100).ToString("F")}%";

                    return text;
                }
            }
        }

        /// <summary>
        /// Starts the worker thread.
        /// </summary>
        public void Start()
        {
            worker.Start();
        }

        /// <summary>
        /// Pushes a new task. Only callable if not busy.
        /// </summary>
        /// <param name="task"></param>
        public void PushTask(ArchiveTask task)
        {
            lock (mutex)
            {
                //Check if busy
                if (currentTask != null)
                    throw new Exception("Worker is busy.");

                //Set
                currentTask = task;
            }
        }

        /// <summary>
        /// The code ran on the worker thread.
        /// </summary>
        private void Work()
        {
            while (true)
            {
                //Enter mutex to get state
                ArchiveTask task;
                lock (mutex)
                {
                    //Check if exiting
                    if (stopping)
                        break;

                    //Get current task
                    task = currentTask;
                }

                //If there is a task, begin execution
                if (task != null)
                {
                    //Run
                    try
                    {
                        task.Process();
                    } catch (Exception ex)
                    {
                        OnError?.Invoke(this, task, ex);
                    }

                    //Clear
                    lock (mutex)
                        currentTask = null;
                } else
                {
                    //Wait
                    Thread.Sleep(300);
                }
            }
        }

        /// <summary>
        /// Requests that the worker stops but does not wait. Call Dispose to wait.
        /// </summary>
        public void RequestStop()
        {
            lock (mutex)
                stopping = true;
        }

        /// <summary>
        /// Requests that the worker stops and waits for it to exit.
        /// </summary>
        public void Dispose()
        {
            lock (mutex)
                stopping = true;
            worker.Join();
        }
    }
}
