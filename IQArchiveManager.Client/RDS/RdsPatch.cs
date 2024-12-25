using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IQArchiveManager.Client.RDS.RdsReader;

namespace IQArchiveManager.Client.RDS
{
    public class RdsPatch
    {
        public RdsPatch(string title, List<RdsValue<string>> rdsRtFrames)
        {
            this.title = title;
            this.rdsRtFrames = rdsRtFrames;
        }

        private string title;
        private List<RdsValue<string>> rdsRtFrames;

        public string Title { get => title; }
        public List<RdsValue<string>> RtFrames { get => rdsRtFrames; }
    }
}
