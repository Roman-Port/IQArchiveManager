using System;
using System.Collections.Generic;
using System.Text;

namespace IQArchiveManager.Server
{
    public interface IArchiveTaskStore
    {
        void Refresh(Queue<ArchiveTask> queue);
    }
}
