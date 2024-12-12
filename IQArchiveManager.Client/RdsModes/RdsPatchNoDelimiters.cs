using IQArchiveManager.Client.Components;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IQArchiveManager.Client.RdsModes
{
    public class RdsPatchNoDelimiters : BaseRdsMode
    {
        public RdsPatchNoDelimiters(ClipDatabase db)
        {
            this.db = db;
            config = db.GetPersistentStore("PATHCER_NO_DELIMITERS");
        }

        private readonly ClipDatabase db;
        private readonly JObject config;

        public override string Label => "No Delimiters";
        public override RdsModeId Id => RdsModeId.NO_DELIMITERS;
        private JArray Brandings
        {
            get
            {
                if (!config.ContainsKey("brandings"))
                    config.Add("brandings", new JArray());
                return (JArray)config["brandings"];
            }
        }

        private static readonly ushort[] TARGET_PIS = new ushort[]
        {
            0x3C0C,
            0x4F23
        };

        public override bool HasSetupWindow => true;

        public override void ShowSetupWindow()
        {
            StationBrandingsEditor editor = new StationBrandingsEditor();
            if (config.ContainsKey("brandings"))
                editor.Brandings = config["brandings"].ToObject<string[]>();
            editor.ShowDialog();
            config.Remove("brandings");
            config.Add("brandings", new JArray(editor.Brandings));
        }

        public override bool IsRecommended(List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames)
        {
            //Search for problematic PIs
            foreach (var pi in rdsPiFrames)
            {
                if (TARGET_PIS.Contains(pi.value))
                    return true;
            }
            return false;
        }

        public override List<RdsValue<string>> Patch(List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames)
        {
            //Simply remove station known station branding
            List<RdsValue<string>> newFrames = new List<RdsValue<string>>();
            string stripped;
            foreach (var f in rdsRtFrames)
            {
                //Search for and remove branding
                stripped = f.value;
                foreach (var branding in Brandings)
                {
                    string b = ((string)branding);
                    if (stripped.StartsWith(b))
                        stripped = stripped.Substring(b.Length);
                    if (stripped.EndsWith(b))
                        stripped = stripped.Substring(0, stripped.Length - b.Length);
                }

                //Add
                newFrames.Add(new RdsValue<string>(f.first, f.last, stripped));
            }

            return newFrames;
        }

        public override bool TryParse(string rt, out string trackTitle, out string trackArtist, out string stationName, bool fast)
        {
            //Attempt to find a matching artist. Not perfect but its better than nothing. Do this by chopping it off by word
            string search = rt.Trim();
            while (true)
            {
                //Find the last space and trim to that
                int lastSpace = search.LastIndexOf(' ');
                if (lastSpace == -1)
                    break;
                search = search.Substring(0, lastSpace);
                if (search.Length == 0)
                    break;

                //Search the DB for this
                foreach (var c in db.Clips)
                {
                    if (c.Artist.Equals(search, StringComparison.OrdinalIgnoreCase))
                    {
                        //Found artist!
                        trackArtist = search;
                        trackTitle = rt.Trim().Substring(trackArtist.Length + 1);
                        stationName = "";
                        return true;
                    }
                }
            }

            //Fallback
            trackArtist = rt;
            trackTitle = "";
            stationName = "";

            return false;
        }
    }
}
