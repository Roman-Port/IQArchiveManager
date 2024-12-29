using IQArchiveManager.Client.RDS.Modes.Csv.Db;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS.Modes.Csv
{
    class RdsPatchCsv : BaseRdsMode
    {
        public RdsPatchCsv(ClipDatabase db)
        {
            this.db = db;
            config = db.GetPersistentStore("PATCHER_CSV");
            if (!config.ContainsKey("profiles"))
                config["profiles"] = new JArray();
        }

        private readonly ClipDatabase db;
        private readonly JObject config;

        private CsvProfile[] Profiles
        {
            get => config["profiles"].ToObject<CsvProfile[]>();
            set => config["profiles"] = JArray.FromObject(value);
        }

        public override RdsModeId Id => RdsModeId.CSV;

        public override string Label => "History CSV";

        public override bool HasSetupWindow => true;

        public override void ShowSetupWindow()
        {
            CsvProfileConfigForm conf = new CsvProfileConfigForm(Profiles);
            conf.ShowDialog();
            Profiles = conf.Profiles;
            db.Save();
        }

        public override bool IsRecommended(IRdsPatchContext ctx, List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames)
        {
            //Collect matching PI and templates
            Dictionary<ushort, string> query = new Dictionary<ushort, string>();
            foreach (var p in Profiles)
            {
                if (!query.ContainsKey(p.PiCode))
                    query.Add(p.PiCode, p.PathTemplate);
            }

            //Scan for any matches
            List<string> checkedFilenames = new List<string>();
            foreach (var f in rdsPiFrames)
            {
                //Check if the PI matches anything
                if (query.TryGetValue(f.value, out string template))
                {
                    //Resolve template to the date of this
                    string filename = ResolveTemplate(template, ctx.GetTimeOfFrameStart(f));

                    //Check if we've not already checked it
                    if (!checkedFilenames.Contains(filename))
                    {
                        //Check if it exists
                        if (File.Exists(filename))
                            return true;

                        //Add to list as to not hit the disk too hard
                        checkedFilenames.Add(filename);
                    }
                }
            }

            return false;
        }

        public override List<RdsValue<string>> Patch(IRdsPatchContext ctx, List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames)
        {
            //Read all profiles
            Dictionary<ushort, CsvProfile> profiles = new Dictionary<ushort, CsvProfile>();
            foreach (var p in Profiles)
            {
                if (!profiles.ContainsKey(p.PiCode))
                    profiles.Add(p.PiCode, p);
            }

            //Begin processing all PI frames
            Dictionary<ushort, CsvHistoryDatabase> databases = new Dictionary<ushort, CsvHistoryDatabase>();
            List<RdsValue<string>> output = new List<RdsValue<string>>();
            RdsValue<string> last = null;
            foreach (var p in rdsPiFrames)
            {
                //Resolve this PI code to a profile, if any
                if (!profiles.TryGetValue(p.value, out CsvProfile profile))
                    continue;

                //Get or create a database for this
                CsvHistoryDatabase db;
                if (!databases.TryGetValue(profile.PiCode, out db))
                {
                    db = new CsvHistoryDatabase();
                    databases.Add(profile.PiCode, db);
                }

                //Get start and end time of this
                DateTime absStart = ctx.GetTimeOfFrameStart(p);
                DateTime absEnd = ctx.GetTimeOfFrameEnd(p);

                //Load all pages between these days
                db.LoadPages(profile, absStart, absEnd);

                //Loop through all events between start and end
                foreach (var i in db.Items.Where(x => x.Time >= absStart && x.Time < absEnd))
                {
                    //Get the sample from the time
                    long sample = ctx.GetSampleFromTime(i.Time);
                    if (sample < 0)
                        continue; // shouldn't ever happen

                    //Update end time of last event if not already terminated
                    if (last != null)
                        last.last = sample;

                    //Insert event (the end time will be updated somewhere else)
                    ParsedRdsValue<string> evt = new ParsedRdsValue<string>(i, sample, $"{i.Artist} // {i.Title}");
                    output.Add(evt);
                    last = evt;
                }
            }

            //We will now terminate messages when the EOM message is triggered
            foreach (var o in output)
            {
                if (o is ParsedRdsValue<string> point)
                {
                    //Get all EOM messages from the profile
                    string[] eomPsValues = point.Item.Profile.EomPsTriggers.Split(',');

                    //Convert grace from milliseconds to samples
                    long graceSamples = (MainEditor.AUDIO_SAMPLE_RATE * (long)point.Item.Profile.EomGrace) / 1000;

                    //Scan for all PS frames between the start and end that have a value matching one of the EOM triggers
                    RdsValue<string> eom = rdsPsFrames.Where(x => x.first >= o.first + graceSamples && x.first < o.last && eomPsValues.Contains(x.value)).FirstOrDefault();

                    //If one was found, set our end point to that
                    if (eom != null)
                        o.last = eom.first;
                }
            }

            //Fill in gaps in data
            if (output.Count == 0)
            {
                //Add empty frame
                output.Add(new RdsValue<string>(0, ""));
            } else
            {
                //Add time to first
                long firstSample = output[0].first;
                if (firstSample > 0)
                    output.Insert(0, new RdsValue<string>(0, firstSample, ""));

                //Fill in gaps
                for (int i = 0; i < output.Count - 1; i++)
                {
                    if (output[i].last < output[i + 1].first)
                    {
                        output.Insert(i + 1, new RdsValue<string>(output[i].last, output[i + 1].first, ""));
                    }
                }
            }

            return output;
        }

        public override bool TryParse(RdsValue<string> rt, out string trackTitle, out string trackArtist, out string stationName, bool fast)
        {
            //Attempt to match our own type
            if (rt is ParsedRdsValue<string> parsed)
            {
                trackTitle = parsed.Item.Title;
                trackArtist = parsed.Item.Artist;
                stationName = "";
                return true;
            }

            //Fail
            trackTitle = "";
            trackArtist = "";
            stationName = "";
            return false;
        }

        public override bool TryParse(string rt, out string trackTitle, out string trackArtist, out string stationName, bool fast)
        {
            throw new NotSupportedException(); // Should never be called
        }

        public static string ResolveTemplate(string template, DateTime time)
        {
            return ResolveTemplate(template, time.Year, time.Month, time.Day);
        }

        public static string ResolveTemplate(string template, int year, int month, int day)
        {
            return template.Replace("%y", year.ToString()).Replace("%m", month.ToString().PadLeft(2, '0')).Replace("%d", day.ToString().PadLeft(2, '0'));
        }

        class ParsedRdsValue<T> : RdsValue<T>
        {
            public ParsedRdsValue(CsvPlaybackItem item, long sample, T value) : base(sample, value)
            {
                this.item = item;
            }

            public ParsedRdsValue(CsvPlaybackItem item, long first, long last, T value) : base(first, last, value)
            {
                this.item = item;
            }

            private CsvPlaybackItem item;

            public CsvPlaybackItem Item => item;
        }
    }
}
