using System;
using System.Collections.Generic;
using System.Text;

namespace IQArchiveManager.Server
{
    public interface IArchiveTask
    {
        void Process(ref string status);
    }
}
