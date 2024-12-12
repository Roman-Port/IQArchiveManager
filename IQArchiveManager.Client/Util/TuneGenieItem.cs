using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.Util
{
    interface ITuneGenieItem
    {
        string Name { get; set; }
        string Artist { get; set; }
        DateTime PlayedAt { get; set; }
        DateTime EstimatedEndTime { get; set; }
    }
}
