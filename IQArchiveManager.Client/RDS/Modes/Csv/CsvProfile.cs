using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS.Modes.Csv
{
    public class CsvProfile
    {
        [JsonProperty("pi")]
        public ushort PiCode { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string PathTemplate { get; set; }

        [JsonProperty("col_time")]
        public int ColumnTime { get; set; }

        [JsonProperty("col_artist")]
        public int ColumnArtist { get; set; }

        [JsonProperty("col_title")]
        public int ColumnTitle { get; set; }

        [JsonProperty("eom")]
        public string EomPsTriggers { get; set; } // comma separated

        [JsonProperty("grace")]
        public int EomGrace { get; set; } // time in milliseconds

        [JsonProperty("offset")]
        public int Offset { get; set; } // time in milliseconds
    }
}
