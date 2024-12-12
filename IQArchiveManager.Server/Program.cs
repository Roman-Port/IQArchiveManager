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
        private static ConcurrentQueue<IArchiveTask> taskQueue = new ConcurrentQueue<IArchiveTask>();
        private static Thread[] workers = new Thread[THREAD_COUNT];
        private static string[] workerStatus = new string[THREAD_COUNT];
        private static volatile bool stop = false;
        private static volatile int finished = 0;
        private static volatile int errors = 0;

        private const int THREAD_COUNT = 6;

        static void Main(string[] args)
        {
            //Create store
            
            taskGenerators.Add(new Post.PostProcessorTaskStore(args[0]));
            taskGenerators.Add(new Pre.PreProcessorTaskStore(args[0]));

            //Create worker threads
            for (int i = 0; i < workers.Length; i++)
            {
                workers[i] = new Thread(WorkerThread);
                workers[i].IsBackground = true;
                workers[i].Start(i);
            }

            //Start end catching thread
            Thread endThread = new Thread(CancelThread);
            endThread.IsBackground = true;
            endThread.Start();

            //Occasionally refresh tasks
            int looped = int.MaxValue;
            while (finished < THREAD_COUNT)
            {
                //Check if we need to refresh
                if (looped++ >= 20)
                {
                    //Refresh all
                    foreach (var g in taskGenerators)
                        g.Refresh(taskQueue);

                    //Reset
                    looped = 0;
                }

                //Write to console header
                if (stop)
                    PrintToLine(0, $"Exiting once items are done...");
                else
                    PrintToLine(0, $"{taskQueue.Count} items in queue...(press ENTER to stop adding more tasks)");

                //Write tasks
                for (int i = 0; i < workers.Length; i++)
                    PrintToLine(i + 1, $"[#{i + 1}] => {workerStatus[i]}");

                //Write errors
                PrintToLine(workers.Length + 1, $"{errors} errors reported.");

                //Wait
                Thread.Sleep(100);
            }
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
        }

        static void WorkerThread(object index)
        {
            IArchiveTask task;
            while (!stop)
            {
                //Set status
                workerStatus[(int)index] = "Idle";

                //Attempt to get a task
                while (!taskQueue.TryDequeue(out task) && !stop)
                    Thread.Sleep(1000);
                if (stop)
                    break;

                //Process
                try
                {
                    task.Process(ref workerStatus[(int)index]);
                } catch (Exception ex)
                {
                    //Update counter
                    errors++;

                    //Write error info to disk
                    File.WriteAllText("error-" + DateTime.Now.Ticks + ".txt", ex.Message + ex.StackTrace);
                }
            }
            workerStatus[(int)index] = "DONE";
            finished++;
        }
    }
}
