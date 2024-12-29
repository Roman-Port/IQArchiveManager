using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS.Modes.Csv.Db
{
    class CsvPlaybackItem
    {
        public CsvPlaybackItem(string[] line, CsvProfile profile)
        {
            this.line = line;
            this.profile = profile;
            time = EPOCH.AddSeconds(long.Parse(line[profile.ColumnTime])).AddMilliseconds(profile.Offset).ToLocalTime();
        }

        private readonly string[] line;
        private readonly CsvProfile profile;
        private readonly DateTime time; // cached because it may be loaded many times

        private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public CsvProfile Profile => profile;
        public DateTime Time => time;
        public string Artist => line[profile.ColumnArtist];
        public string Title => line[profile.ColumnTitle];
    }
}
