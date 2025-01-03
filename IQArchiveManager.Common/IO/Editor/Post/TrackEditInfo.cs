using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Common.IO.Editor.Post
{
    public class TrackEditInfo
    {
        [JsonProperty("data")]
        public TrackClipInfo Data { get; set; }

        [JsonProperty("start")]
        public double Start { get; set; }

        [JsonProperty("end")]
        public double End { get; set; }
    }
}
