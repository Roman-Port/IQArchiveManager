using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Common
{
    public class TrackClipInfo
    {
        public const int FLAG_RDS = 0;
        public const int FLAG_HD = 1;
        public const int FLAG_OK = 2;

        /* BASIC DATA */

        [JsonProperty("i")]
        public string Id { get; set; }

        [JsonProperty("s")]
        public long OriginalFileSize { get; set; }

        [JsonProperty("r")]
        public int SampleRate { get; set; }

        [JsonProperty("t")]
        public DateTime Time { get; set; }

        [JsonProperty("h")]
        public string Sha256 { get; set; }

        [JsonProperty("rds")]
        public string RdsParsed { get; set; } // When matched via RDS, contains the string (BASE-64 ENCODED) of the RT field (or derived from a mode). May be used for analysis in the future.

        [JsonProperty("rdsm")]
        public int RdsParser { get; set; } // When matched via RDS, contains the ID of the RDS matcher used.

        [JsonProperty("e")]
        public DateTime? EditedAt { get; set; } // Time this edit was submitted at. This was added later, so may be null.

        [JsonProperty("v")]
        public int EditorVersion { get; set; } // Version of the editor this was entered at. This was added later, so nulls should be interpreted at version 0.

        [JsonProperty("l")]
        public double? Length { get; set; } // Length of the file in seconds. This was added later, so may be null.

        [JsonProperty("ft")]
        public DateTime? OriginalFileTime { get; set; } // Modifed date of original file. This was added later, so may be null.

        /* USER DATA */

        [JsonProperty("uc")]
        public string Station { get; set; }

        [JsonProperty("ua")]
        public string Artist { get; set; }

        [JsonProperty("ut")]
        public string Title { get; set; }

        [JsonProperty("us")]
        public string UserStatus { get; set; }

        [JsonProperty("up")]
        public string Prefix { get; set; }

        [JsonProperty("ux")]
        public string Suffix { get; set; }

        [JsonProperty("sf")]
        public int Flags { get; set; }

        [JsonProperty("un")]
        public string Notes { get; set; }

        /* LATER DATA */

        [JsonProperty("ps")]
        public TrackClipInfoSnr Snr { get; set; }

        /* FLAG READER/SETTERS */

        [JsonIgnore]
        public bool FlagRds
        {
            get => ReadFlag(FLAG_RDS);
            set => SetFlag(FLAG_RDS, value);
        }

        [JsonIgnore]
        public bool FlagHd
        {
            get => ReadFlag(FLAG_HD);
            set => SetFlag(FLAG_HD, value);
        }

        [JsonIgnore]
        public bool FlagOk
        {
            get => ReadFlag(FLAG_OK);
            set => SetFlag(FLAG_OK, value);
        }

        /* MISC */

        public string GetRadioString()
        {
            switch (SampleRate)
            {
                case 750000: return "AirSpy Mini";
                case 768000: return "AirSpy HF+";
                case 900001: return "RTL-SDR";
                default: return "N/A";
            }
        }

        private bool ReadFlag(int index)
        {
            return (Flags & (1 << index)) != 0;
        }

        private void SetFlag(int index, bool set)
        {
            int bitmask = 1 << index;
            if (set)
                Flags |= bitmask;
            else
                Flags &= ~bitmask;
        }

        public override string ToString()
        {
            return $"{Station} -> {Artist} - {Title}";
        }

        /* LEGACY NAMES */

        [JsonProperty("id")]
        private string _Legacy_id { set => Id = value; }

        [JsonProperty("originalFileSize")]
        private long _Legacy_originalFileSize { set => OriginalFileSize = value; }

        [JsonProperty("sampleRate")]
        private int _Legacy_sampleRate { set => SampleRate = value; }

        [JsonProperty("time")]
        private DateTime _Legacy_time { set => Time = value; }

        [JsonProperty("sha256")]
        private string _Legacy_sha256 { set => Sha256 = value; }

        [JsonProperty("station")]
        private string _Legacy_station { set => Station = value; }

        [JsonProperty("artist")]
        private string _Legacy_artist { set => Artist = value; }

        [JsonProperty("title")]
        private string _Legacy_title { set => Title = value; }

        [JsonProperty("userStatus")]
        private string _Legacy_userStatus { set => UserStatus = value; }

        [JsonProperty("prefix")]
        private string _Legacy_prefix { set => Prefix = value; }

        [JsonProperty("suffix")]
        private string _Legacy_suffix { set => Suffix = value; }

        [JsonProperty("flagRds")]
        private bool _Legacy_flagRds { set => FlagRds = value; }

        [JsonProperty("flagHd")]
        private bool _Legacy_flagHd { set => FlagHd = value; }

        [JsonProperty("flagOk")]
        private bool _Legacy_flagOk { set => FlagOk = value; }

        [JsonProperty("notes")]
        private string _Legacy_notes { set => Notes = value; }

        [JsonProperty("snr")]
        private TrackClipInfoSnr _Legacy_snr { set => Snr = value; }
    }

    public class TrackClipInfoSnr
    {
        [JsonProperty("v")]
        public int Version { get; set; }

        [JsonProperty("m")]
        public bool Migrated { get; set; }

        [JsonProperty("s")]
        public float Snr { get; set; }

        /* LEGACY NAMES */

        [JsonProperty("version")]
        private int _Legacy_version { set => Version = value; }

        [JsonProperty("migrated")]
        private bool _Legacy_migrated { set => Migrated = value; }

        [JsonProperty("snr")]
        private float _Legacy_snr { set => Snr = value; }
    }
}
