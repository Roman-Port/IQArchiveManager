using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace IQArchiveManager.Server
{
    public interface IArchiveTaskStore
    {
        void Refresh(ConcurrentQueue<IArchiveTask> queue);
    }
}
