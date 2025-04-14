using IQArchiveManager.Server.Native;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace IQArchiveManager.Server
{
    class Program
    {
        private static List<IArchiveTaskStore> taskGenerators = new List<IArchiveTaskStore>();
        private static ArchiveWorkerThread[] workers = new ArchiveWorkerThread[6];
        private static volatile bool stop = false;
        private static volatile int errors = 0;

        static void Main(string[] args)
        {
            //Init library
            InitNative();

            //Get the optional location argument. This is just a user-defined string that will make it into output files to identify the source
            string location = null;
            if (args.Length >= 1)
                location = args[0];

            //Get current directory
            string dir = Directory.GetCurrentDirectory().TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

            //Create store
            taskGenerators.Add(new Post.PostProcessorTaskStore(dir));
            taskGenerators.Add(new Pre.PreProcessorTaskStore(dir, location));

            //Create worker threads
            for (int i = 0; i < workers.Length; i++)
            {
                workers[i] = new ArchiveWorkerThread();
                workers[i].OnError += OnWorkerError;
                workers[i].Start();
            }

            //Start end catching thread
            Thread endThread = new Thread(CancelThread);
            endThread.IsBackground = true;
            endThread.Start();

            //Occasionally refresh tasks
            int looped = int.MaxValue;
            Queue<ArchiveTask> pending = new Queue<ArchiveTask>();
            while (true)
            {
                //Check if all have quit
                int running = 0;
                foreach (var w in workers)
                {
                    if (!w.Finished)
                        running++;
                }
                if (running == 0)
                    break;

                //Check if we need to refresh
                if (looped++ >= 20)
                {
                    //Refresh all sources to get new pending items
                    foreach (var g in taskGenerators)
                        g.Refresh(pending);

                    //Dispatch to free workers
                    foreach (var w in workers)
                    {
                        if (!w.Busy && pending.TryDequeue(out ArchiveTask task))
                            w.PushTask(task);
                    }

                    //Reset
                    looped = 0;
                }

                //Write to console header
                if (stop)
                    PrintToLine(0, $"Exiting once items are done...");
                else
                    PrintToLine(0, $"{pending.Count} items in queue...(press ENTER to stop adding more tasks)");

                //Write tasks
                for (int i = 0; i < workers.Length; i++)
                    PrintToLine(i + 1, $"[#{i + 1}] => {workers[i].StatusText}");

                //Write errors
                PrintToLine(workers.Length + 1, $"{errors} errors reported.");

                //Wait
                Thread.Sleep(100);
            }
        }

        private static void OnWorkerError(ArchiveWorkerThread thread, ArchiveTask task, Exception exception)
        {
            errors++;
        }

        static unsafe void InitNative()
        {
            Console.WriteLine("Initializing native...");
            int major;
            int minor;
            IQAMNative.GetVersion(&major, &minor);
            Console.WriteLine($"Loaded native version {major}.{minor}.");
        }

        static void PrintToLine(int line, string message)
        {
            Console.SetCursorPosition(0, line);
            while (message.Length < Console.BufferWidth - 1)
                message += " ";
            Console.Write(message);
        }

        static void CancelThread()
        {
            Console.ReadLine();
            stop = true;
            foreach (var w in workers)
                w.RequestStop();
        }
    }
}
